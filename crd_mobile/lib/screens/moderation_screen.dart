import 'package:flutter/material.dart';
import '../models/district_models.dart';
import '../models/rate_models.dart';
import '../services/api_service.dart';
import '../services/auth_service.dart';

class ModerationScreen extends StatefulWidget {
  final DistrictRateDto districtRate;
  const ModerationScreen({super.key, required this.districtRate});

  @override
  State<ModerationScreen> createState() => _ModerationScreenState();
}

class _ModerationScreenState extends State<ModerationScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  List<PlantRateViewModel> _plants = [];
  List<StructureRateViewModel> _structures = [];
  bool _loading = true;
  String? _error;
  String _filter = 'all'; // all | approved | pending

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _load();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final api = ApiService();
      await api.init();
      final plants =
          await api.getModeratedPlantRates(widget.districtRate.id);
      final structures =
          await api.getModeratedStructureRates(widget.districtRate.id);
      setState(() {
        _plants = plants;
        _structures = structures;
      });
    } on ApiException catch (e) {
      setState(() => _error = e.message);
    } catch (e) {
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  List<PlantRateViewModel> get _filteredPlants {
    if (_filter == 'approved') {
      return _plants
          .where((p) => p.moderationStatus == 1)
          .toList();
    } else if (_filter == 'pending') {
      return _plants
          .where((p) => p.moderationStatus != 1)
          .toList();
    }
    return _plants;
  }

  List<StructureRateViewModel> get _filteredStructures {
    if (_filter == 'approved') {
      return _structures
          .where((s) => s.moderationStatus == 1)
          .toList();
    } else if (_filter == 'pending') {
      return _structures
          .where((s) => s.moderationStatus != 1)
          .toList();
    }
    return _structures;
  }

  Color _statusColor(int status) {
    switch (status) {
      case 1:
        return Colors.green;
      case 3:
        return Colors.blue;
      case 4:
        return Colors.orange;
      case 5:
        return Colors.red;
      case 6:
        return Colors.purple;
      default:
        return Colors.grey;
    }
  }

  void _showModerateDialog({
    required int rateId,
    required String name,
    required bool isPlant,
  }) {
    final notesCtrl = TextEditingController();
    int selectedStatus = 1; // default Approved
    final statuses = [
      {'value': 1, 'label': 'Approve'},
      {'value': 3, 'label': 'Revise'},
      {'value': 4, 'label': 'Defer'},
      {'value': 5, 'label': 'Delete'},
      {'value': 6, 'label': 'Mark New Entry'},
    ];

    showDialog(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          title: Text('Moderate: $name'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text('Action:',
                  style: TextStyle(fontWeight: FontWeight.w600)),
              const SizedBox(height: 8),
              Wrap(
                spacing: 8,
                children: statuses.map((s) {
                  final v = s['value'] as int;
                  return ChoiceChip(
                    label: Text(s['label'] as String),
                    selected: selectedStatus == v,
                    onSelected: (_) =>
                        setDialogState(() => selectedStatus = v),
                    selectedColor: _statusColor(v).withOpacity(0.2),
                  );
                }).toList(),
              ),
              const SizedBox(height: 16),
              TextField(
                controller: notesCtrl,
                maxLines: 3,
                decoration: InputDecoration(
                  labelText: 'Notes (optional)',
                  border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8)),
                ),
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(ctx),
              child: const Text('Cancel'),
            ),
            ElevatedButton(
              onPressed: () async {
                Navigator.pop(ctx);
                await _submitModeration(
                  rateId: rateId,
                  status: selectedStatus,
                  notes: notesCtrl.text.trim(),
                );
              },
              style: ElevatedButton.styleFrom(
                  backgroundColor: const Color(0xFF1B5E20),
                  foregroundColor: Colors.white),
              child: const Text('Submit'),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _submitModeration({
    required int rateId,
    required int status,
    String? notes,
  }) async {
    try {
      final api = ApiService();
      await api.init();
      await api.moderateRate(
          rateId: rateId, status: status, notes: notes);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
            content: Text('Rate moderated successfully'),
            backgroundColor: Colors.green),
      );
      _load();
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error: $e'), backgroundColor: Colors.red));
    }
  }

  bool get _canModerate =>
      AuthService().currentUser?.isModerator ?? false;

  @override
  Widget build(BuildContext context) {
    final r = widget.districtRate;
    return Scaffold(
      appBar: AppBar(
        title: Text('Moderate: ${r.districtName}'),
        backgroundColor: const Color(0xFF1B5E20),
        foregroundColor: Colors.white,
        bottom: TabBar(
          controller: _tabController,
          labelColor: Colors.white,
          unselectedLabelColor: Colors.white70,
          indicatorColor: Colors.white,
          tabs: const [
            Tab(icon: Icon(Icons.grass), text: 'Plant Rates'),
            Tab(icon: Icon(Icons.home_work), text: 'Structure Rates'),
          ],
        ),
        actions: [
          IconButton(
              icon: const Icon(Icons.refresh),
              onPressed: _load,
              tooltip: 'Refresh'),
        ],
      ),
      body: Column(
        children: [
          // Filter chips
          Container(
            color: const Color(0xFF1B5E20).withOpacity(0.05),
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
            child: Row(
              children: [
                const Text('Show:',
                    style: TextStyle(
                        fontSize: 13, fontWeight: FontWeight.w600)),
                const SizedBox(width: 8),
                for (final f in ['all', 'approved', 'pending'])
                  Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: ChoiceChip(
                      label: Text(f[0].toUpperCase() + f.substring(1)),
                      selected: _filter == f,
                      onSelected: (_) => setState(() => _filter = f),
                      selectedColor:
                          const Color(0xFF1B5E20).withOpacity(0.2),
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
                          padding: const EdgeInsets.all(24),
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              const Icon(Icons.error_outline,
                                  size: 48, color: Colors.red),
                              const SizedBox(height: 16),
                              Text(_error!,
                                  style:
                                      const TextStyle(color: Colors.grey)),
                              const SizedBox(height: 16),
                              ElevatedButton(
                                  onPressed: _load,
                                  child: const Text('Retry')),
                            ],
                          ),
                        ),
                      )
                    : TabBarView(
                        controller: _tabController,
                        children: [
                          // Plant rates tab
                          _filteredPlants.isEmpty
                              ? const Center(
                                  child: Text('No plant rates found'))
                              : ListView.builder(
                                  itemCount: _filteredPlants.length,
                                  itemBuilder: (ctx, i) {
                                    final p = _filteredPlants[i];
                                    return Card(
                                      margin: const EdgeInsets.symmetric(
                                          horizontal: 12, vertical: 4),
                                      child: ListTile(
                                        leading: CircleAvatar(
                                          backgroundColor:
                                              _statusColor(p.moderationStatus)
                                                  .withOpacity(0.15),
                                          child: const Icon(Icons.grass,
                                              size: 18),
                                        ),
                                        title: Text(p.plantName,
                                            style: const TextStyle(
                                                fontWeight: FontWeight.w600,
                                                fontSize: 14)),
                                        subtitle: Text(
                                            '${p.growthStage}  •  ${p.unit}'
                                            '${p.rate != null ? '  •  UGX ${p.rate!.toStringAsFixed(0)}' : ''}'),
                                        trailing: Row(
                                          mainAxisSize: MainAxisSize.min,
                                          children: [
                                            Container(
                                              padding:
                                                  const EdgeInsets.symmetric(
                                                      horizontal: 6,
                                                      vertical: 2),
                                              decoration: BoxDecoration(
                                                color: _statusColor(
                                                        p.moderationStatus)
                                                    .withOpacity(0.1),
                                                borderRadius:
                                                    BorderRadius.circular(8),
                                                border: Border.all(
                                                    color: _statusColor(
                                                            p.moderationStatus)
                                                        .withOpacity(0.4)),
                                              ),
                                              child: Text(
                                                  p.moderationStatusLabel,
                                                  style: TextStyle(
                                                      fontSize: 10,
                                                      color: _statusColor(
                                                          p.moderationStatus),
                                                      fontWeight:
                                                          FontWeight.w600)),
                                            ),
                                            if (_canModerate) ...[
                                              const SizedBox(width: 4),
                                              IconButton(
                                                icon: const Icon(
                                                    Icons.edit_note,
                                                    size: 20),
                                                color: const Color(0xFF1B5E20),
                                                onPressed: () =>
                                                    _showModerateDialog(
                                                  rateId: p.id,
                                                  name: p.plantName,
                                                  isPlant: true,
                                                ),
                                              ),
                                            ],
                                          ],
                                        ),
                                      ),
                                    );
                                  },
                                ),
                          // Structure rates tab
                          _filteredStructures.isEmpty
                              ? const Center(
                                  child:
                                      Text('No structure rates found'))
                              : ListView.builder(
                                  itemCount: _filteredStructures.length,
                                  itemBuilder: (ctx, i) {
                                    final s = _filteredStructures[i];
                                    return Card(
                                      margin: const EdgeInsets.symmetric(
                                          horizontal: 12, vertical: 4),
                                      child: ListTile(
                                        leading: CircleAvatar(
                                          backgroundColor:
                                              _statusColor(s.moderationStatus)
                                                  .withOpacity(0.15),
                                          child: const Icon(Icons.home_work,
                                              size: 18),
                                        ),
                                        title: Text(s.structureName,
                                            style: const TextStyle(
                                                fontWeight: FontWeight.w600,
                                                fontSize: 14)),
                                        subtitle: Text(
                                            '${s.unit}'
                                            '${s.rate != null ? '  •  UGX ${s.rate!.toStringAsFixed(0)}' : ''}'),
                                        trailing: Row(
                                          mainAxisSize: MainAxisSize.min,
                                          children: [
                                            Container(
                                              padding:
                                                  const EdgeInsets.symmetric(
                                                      horizontal: 6,
                                                      vertical: 2),
                                              decoration: BoxDecoration(
                                                color: _statusColor(
                                                        s.moderationStatus)
                                                    .withOpacity(0.1),
                                                borderRadius:
                                                    BorderRadius.circular(8),
                                                border: Border.all(
                                                    color: _statusColor(
                                                            s.moderationStatus)
                                                        .withOpacity(0.4)),
                                              ),
                                              child: Text(
                                                  s.moderationStatusLabel,
                                                  style: TextStyle(
                                                      fontSize: 10,
                                                      color: _statusColor(
                                                          s.moderationStatus),
                                                      fontWeight:
                                                          FontWeight.w600)),
                                            ),
                                            if (_canModerate) ...[
                                              const SizedBox(width: 4),
                                              IconButton(
                                                icon: const Icon(
                                                    Icons.edit_note,
                                                    size: 20),
                                                color: const Color(0xFF1B5E20),
                                                onPressed: () =>
                                                    _showModerateDialog(
                                                  rateId: s.id,
                                                  name: s.structureName,
                                                  isPlant: false,
                                                ),
                                              ),
                                            ],
                                          ],
                                        ),
                                      ),
                                    );
                                  },
                                ),
                        ],
                      ),
          ),
        ],
      ),
    );
  }
}
