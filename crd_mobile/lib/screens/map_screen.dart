import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart';
import '../models/district_models.dart';
import '../services/api_service.dart';

class MapScreen extends StatefulWidget {
  const MapScreen({super.key});

  @override
  State<MapScreen> createState() => _MapScreenState();
}

class _MapScreenState extends State<MapScreen> {
  Map<String, PublishedRateSummaryDto> _statusMap = {};
  dynamic _geoJson;
  bool _loading = true;
  String _filter = 'all';
  final MapController _mapController = MapController();

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() => _loading = true);
    try {
      // Load GeoJSON
      final geoStr =
          await rootBundle.loadString('assets/districts.json');
      _geoJson = json.decode(geoStr);

      // Load published rates
      final api = ApiService();
      await api.init();
      final rates = await api.getPublishedRates(filter: _filter);
      final map = <String, PublishedRateSummaryDto>{};
      for (final r in rates) {
        if (r.districtName != null) {
          map[r.districtName!.trim().toUpperCase()] = r;
        }
      }
      setState(() => _statusMap = map);
    } catch (e) {
      // silently fail — map will show without status colours
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Color _colorForStatus(String? status) {
    switch (status?.toLowerCase()) {
      case 'published':
        return Colors.green.withOpacity(0.7);
      case 'expired':
        return Colors.orange.withOpacity(0.7);
      case 'not published':
        return Colors.red.withOpacity(0.7);
      case 'under review':
        return Colors.blue.withOpacity(0.7);
      default:
        return Colors.grey.withOpacity(0.4);
    }
  }

  List<Polygon> _buildPolygons() {
    if (_geoJson == null) return [];
    final polygons = <Polygon>[];
    final features = _geoJson['features'] as List? ?? [];

    for (final feature in features) {
      final props = feature['properties'] ?? {};
      final name =
          (props['District_3'] ?? props['name'] ?? '').toString().trim().toUpperCase();
      final status = _statusMap[name]?.status;
      final color = _colorForStatus(status);

      final geometry = feature['geometry'];
      final type = geometry['type'];
      final coords = geometry['coordinates'];

      if (type == 'Polygon') {
        final ring = coords[0] as List;
        final points = ring
            .map<LatLng>((c) => LatLng((c[1] as num).toDouble(),
                (c[0] as num).toDouble()))
            .toList();
        polygons.add(Polygon(
          points: points,
          color: color,
          borderColor: Colors.white,
          borderStrokeWidth: 1,
          label: name,
          labelStyle: const TextStyle(fontSize: 8, color: Colors.black87),
        ));
      } else if (type == 'MultiPolygon') {
        for (final part in coords as List) {
          final ring = part[0] as List;
          final points = ring
              .map<LatLng>((c) => LatLng((c[1] as num).toDouble(),
                  (c[0] as num).toDouble()))
              .toList();
          polygons.add(Polygon(
            points: points,
            color: color,
            borderColor: Colors.white,
            borderStrokeWidth: 1,
          ));
        }
      }
    }
    return polygons;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Districts Map'),
        backgroundColor: const Color(0xFF1B5E20),
        foregroundColor: Colors.white,
        actions: [
          IconButton(
              icon: const Icon(Icons.refresh),
              onPressed: _loadData,
              tooltip: 'Refresh'),
        ],
      ),
      body: Column(
        children: [
          // Filter chips
          Container(
            color: const Color(0xFF1B5E20).withOpacity(0.05),
            padding:
                const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
            child: SingleChildScrollView(
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
                          _loadData();
                        },
                        selectedColor:
                            const Color(0xFF1B5E20).withOpacity(0.2),
                      ),
                    ),
                ],
              ),
            ),
          ),
          // Map
          Expanded(
            child: Stack(
              children: [
                FlutterMap(
                  mapController: _mapController,
                  options: const MapOptions(
                    initialCenter: LatLng(1.3, 32.5),
                    initialZoom: 6.5,
                    minZoom: 5,
                    maxZoom: 14,
                  ),
                  children: [
                    TileLayer(
                      urlTemplate:
                          'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                      userAgentPackageName: 'com.crd.mobile',
                    ),
                    if (!_loading)
                      PolygonLayer(polygons: _buildPolygons()),
                  ],
                ),
                if (_loading)
                  const Center(child: CircularProgressIndicator()),
                // Legend
                Positioned(
                  bottom: 16,
                  right: 16,
                  child: Card(
                    child: Padding(
                      padding: const EdgeInsets.all(10),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisSize: MainAxisSize.min,
                        children: const [
                          Text('Legend',
                              style: TextStyle(
                                  fontWeight: FontWeight.bold,
                                  fontSize: 12)),
                          SizedBox(height: 6),
                          _LegendItem(color: Colors.green, label: 'Published'),
                          _LegendItem(color: Colors.blue, label: 'Under Review'),
                          _LegendItem(color: Colors.orange, label: 'Expired'),
                          _LegendItem(color: Colors.red, label: 'Not Published'),
                          _LegendItem(color: Colors.grey, label: 'Unknown'),
                        ],
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _LegendItem extends StatelessWidget {
  final Color color;
  final String label;
  const _LegendItem({required this.color, required this.label});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 4),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 14,
            height: 14,
            decoration: BoxDecoration(
              color: color.withOpacity(0.7),
              border: Border.all(color: Colors.grey),
              borderRadius: BorderRadius.circular(2),
            ),
          ),
          const SizedBox(width: 6),
          Text(label, style: const TextStyle(fontSize: 11)),
        ],
      ),
    );
  }
}
