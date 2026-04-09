import 'dart:io';
import 'package:flutter/material.dart';
import 'package:path_provider/path_provider.dart';
import 'package:open_file/open_file.dart';
import 'package:share_plus/share_plus.dart';
import 'package:csv/csv.dart';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import '../models/district_models.dart';
import '../models/rate_models.dart';
import '../services/api_service.dart';

class PublishedRatesScreen extends StatefulWidget {
  const PublishedRatesScreen({super.key});

  @override
  State<PublishedRatesScreen> createState() => _PublishedRatesScreenState();
}

class _PublishedRatesScreenState extends State<PublishedRatesScreen> {
  List<PublishedRateSummaryDto> _rates = [];
  List<PublishedRateSummaryDto> _filtered = [];
  bool _loading = true;
  String? _error;
  String _filter = 'all';
  String _search = '';

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final api = ApiService();
      await api.init();
      final data = await api.getPublishedRates(filter: _filter);
      setState(() {
        _rates = data;
        _applySearch();
      });
    } on ApiException catch (e) {
      setState(() => _error = e.message);
    } catch (e) {
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  void _applySearch() {
    _filtered = _rates.where((r) {
      return _search.isEmpty ||
          (r.districtName?.toLowerCase().contains(_search.toLowerCase()) ??
              false);
    }).toList();
  }

  Color _statusColor(String? status) {
    switch (status?.toLowerCase()) {
      case 'published':
        return Colors.green;
      case 'expired':
        return Colors.orange;
      case 'not published':
        return Colors.red;
      default:
        return Colors.grey;
    }
  }

  Future<void> _downloadCsv(PublishedRateSummaryDto rate) async {
    try {
      final api = ApiService();
      await api.init();
      final plants =
          await api.getModeratedPlantRates(rate.districtId);

      final rows = <List<dynamic>>[
        ['District', 'Year', 'Plant', 'Growth Stage', 'Unit', 'Rate', 'Status']
      ];
      for (final p in plants) {
        rows.add([
          rate.districtName ?? '',
          rate.year ?? '',
          p.plantName,
          p.growthStage,
          p.unit,
          p.rate ?? '',
          p.moderationStatusLabel,
        ]);
      }

      final csv = const ListToCsvConverter().convert(rows);
      final dir = await getTemporaryDirectory();
      final file = File(
          '${dir.path}/${rate.districtName ?? 'district'}_rates_${rate.year}.csv');
      await file.writeAsString(csv);

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('CSV saved: ${file.path}'),
          action: SnackBarAction(
            label: 'Open',
            onPressed: () => OpenFile.open(file.path),
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error: $e'), backgroundColor: Colors.red));
    }
  }

  Future<void> _downloadPdf(PublishedRateSummaryDto rate) async {
    try {
      final api = ApiService();
      await api.init();
      final plants = await api.getModeratedPlantRates(rate.districtId);
      final structures =
          await api.getModeratedStructureRates(rate.districtId);

      final pdf = pw.Document();
      pdf.addPage(pw.MultiPage(
        pageFormat: PdfPageFormat.a4,
        build: (ctx) => [
          pw.Header(
              level: 0,
              child: pw.Text(
                  '${rate.districtName} — Approved Rates ${rate.year}/${(rate.year ?? 0) + 1}',
                  style: pw.TextStyle(
                      fontSize: 18, fontWeight: pw.FontWeight.bold))),
          pw.SizedBox(height: 16),
          if (plants.isNotEmpty) ...[
            pw.Text('Plant/Crop Rates',
                style: pw.TextStyle(
                    fontSize: 14, fontWeight: pw.FontWeight.bold)),
            pw.SizedBox(height: 8),
            pw.Table.fromTextArray(
              headers: ['Plant', 'Growth Stage', 'Unit', 'Rate (UGX)', 'Status'],
              data: plants
                  .map((p) => [
                        p.plantName,
                        p.growthStage,
                        p.unit,
                        p.rate?.toStringAsFixed(0) ?? 'N/A',
                        p.moderationStatusLabel,
                      ])
                  .toList(),
              cellAlignment: pw.Alignment.centerLeft,
              headerStyle: pw.TextStyle(fontWeight: pw.FontWeight.bold),
              cellStyle: const pw.TextStyle(fontSize: 9),
              headerDecoration:
                  const pw.BoxDecoration(color: PdfColors.grey200),
            ),
            pw.SizedBox(height: 16),
          ],
          if (structures.isNotEmpty) ...[
            pw.Text('Structure Rates',
                style: pw.TextStyle(
                    fontSize: 14, fontWeight: pw.FontWeight.bold)),
            pw.SizedBox(height: 8),
            pw.Table.fromTextArray(
              headers: ['Structure', 'Unit', 'Rate (UGX)', 'Status'],
              data: structures
                  .map((s) => [
                        s.structureName,
                        s.unit,
                        s.rate?.toStringAsFixed(0) ?? 'N/A',
                        s.moderationStatusLabel,
                      ])
                  .toList(),
              cellAlignment: pw.Alignment.centerLeft,
              headerStyle: pw.TextStyle(fontWeight: pw.FontWeight.bold),
              cellStyle: const pw.TextStyle(fontSize: 9),
              headerDecoration:
                  const pw.BoxDecoration(color: PdfColors.grey200),
            ),
          ],
        ],
      ));

      final dir = await getTemporaryDirectory();
      final file = File(
          '${dir.path}/${rate.districtName ?? 'district'}_rates_${rate.year}.pdf');
      await file.writeAsBytes(await pdf.save());

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('PDF saved: ${file.path}'),
          action: SnackBarAction(
            label: 'Open',
            onPressed: () => OpenFile.open(file.path),
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error: $e'), backgroundColor: Colors.red));
    }
  }

  void _showDownloadOptions(PublishedRateSummaryDto rate) {
    showModalBottomSheet(
      context: context,
      builder: (_) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ListTile(
              leading: const Icon(Icons.picture_as_pdf, color: Colors.red),
              title: const Text('Download as PDF'),
              onTap: () {
                Navigator.pop(context);
                _downloadPdf(rate);
              },
            ),
            ListTile(
              leading: const Icon(Icons.table_chart, color: Colors.green),
              title: const Text('Download as CSV'),
              onTap: () {
                Navigator.pop(context);
                _downloadCsv(rate);
              },
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Approved Rates'),
        backgroundColor: const Color(0xFF1B5E20),
        foregroundColor: Colors.white,
        actions: [
          IconButton(
              icon: const Icon(Icons.refresh),
              onPressed: _load,
              tooltip: 'Refresh'),
        ],
      ),
      body: Column(
        children: [
          // Filter + search
          Container(
            color: const Color(0xFF1B5E20).withOpacity(0.05),
            padding: const EdgeInsets.all(12),
            child: Column(
              children: [
                TextField(
                  decoration: InputDecoration(
                    hintText: 'Search district...',
                    prefixIcon: const Icon(Icons.search),
                    border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(8)),
                    filled: true,
                    fillColor: Colors.white,
                    isDense: true,
                  ),
                  onChanged: (v) =>
                      setState(() {
                        _search = v;
                        _applySearch();
                      }),
                ),
                const SizedBox(height: 8),
                SingleChildScrollView(
                  scrollDirection: Axis.horizontal,
                  child: Row(
                    children: [
                      for (final f in ['all', 'valid', 'expired'])
                        Padding(
                          padding: const EdgeInsets.only(right: 8),
                          child: ChoiceChip(
                            label: Text(
                                f[0].toUpperCase() + f.substring(1)),
                            selected: _filter == f,
                            onSelected: (_) {
                              setState(() => _filter = f);
                              _load();
                            },
                            selectedColor:
                                const Color(0xFF1B5E20).withOpacity(0.2),
                          ),
                        ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          Expanded(
            child: _loading
                ? const Center(child: CircularProgressIndicator())
                : _error != null
                    ? Center(
                        child: Padding(
                        padding: const EdgeInsets.all(32),
                        child: Text(_error!,
                            style: const TextStyle(color: Colors.grey)),
                      ))
                    : _filtered.isEmpty
                        ? const Center(child: Text('No rates found'))
                        : RefreshIndicator(
                            onRefresh: _load,
                            child: ListView.builder(
                              itemCount: _filtered.length,
                              itemBuilder: (ctx, i) {
                                final r = _filtered[i];
                                return Card(
                                  margin: const EdgeInsets.symmetric(
                                      horizontal: 12, vertical: 6),
                                  child: ListTile(
                                    leading: CircleAvatar(
                                      backgroundColor:
                                          _statusColor(r.status)
                                              .withOpacity(0.15),
                                      child: Icon(Icons.verified,
                                          color: _statusColor(r.status)),
                                    ),
                                    title: Text(
                                        r.districtName ?? 'Unknown',
                                        style: const TextStyle(
                                            fontWeight: FontWeight.w600)),
                                    subtitle: Text(r.year != null
                                        ? '${r.year}/${r.year! + 1}  •  ${r.status ?? ''}'
                                        : 'No published rates'),
                                    trailing: r.year != null
                                        ? IconButton(
                                            icon: const Icon(
                                                Icons.download_rounded),
                                            color: const Color(0xFF1B5E20),
                                            onPressed: () =>
                                                _showDownloadOptions(r),
                                            tooltip: 'Download rates',
                                          )
                                        : null,
                                  ),
                                );
                              },
                            ),
                          ),
          ),
        ],
      ),
    );
  }
}
