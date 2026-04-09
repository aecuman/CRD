import 'package:flutter/material.dart';
import '../models/district_models.dart';
import '../services/api_service.dart';
import 'moderation_screen.dart';

class DistrictDetailScreen extends StatefulWidget {
  final DistrictRateDto rate;
  const DistrictDetailScreen({super.key, required this.rate});

  @override
  State<DistrictDetailScreen> createState() => _DistrictDetailScreenState();
}

class _DistrictDetailScreenState extends State<DistrictDetailScreen> {
  List<DistrictWorkflowStatusDto> _steps = [];
  bool _loading = true;
  String? _error;

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
      final steps = await api.getDistrictWorkflowStatus(
          widget.rate.districtId, widget.rate.id);
      setState(() => _steps = steps);
    } on ApiException catch (e) {
      setState(() => _error = e.message);
    } catch (e) {
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Color _stepColor(String status) {
    switch (status.toLowerCase()) {
      case 'completed':
        return Colors.green;
      case 'inprogress':
      case 'in progress':
        return Colors.blue;
      case 'pending':
        return Colors.orange;
      default:
        return Colors.grey;
    }
  }

  IconData _stepIcon(String status) {
    switch (status.toLowerCase()) {
      case 'completed':
        return Icons.check_circle;
      case 'inprogress':
      case 'in progress':
        return Icons.pending;
      default:
        return Icons.radio_button_unchecked;
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = widget.rate;
    return Scaffold(
      appBar: AppBar(
        title: Text(r.districtName),
        backgroundColor: const Color(0xFF1B5E20),
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.rate_review),
            tooltip: 'Moderate Rates',
            onPressed: () {
              Navigator.of(context).push(MaterialPageRoute(
                builder: (_) => ModerationScreen(districtRate: r),
              ));
            },
          ),
        ],
      ),
      body: Column(
        children: [
          // Header card
          Container(
            width: double.infinity,
            margin: const EdgeInsets.all(16),
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: const Color(0xFF1B5E20).withOpacity(0.08),
              borderRadius: BorderRadius.circular(12),
              border: Border.all(
                  color: const Color(0xFF1B5E20).withOpacity(0.2)),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(r.districtName,
                        style: const TextStyle(
                            fontSize: 20, fontWeight: FontWeight.bold)),
                    _StatusBadge(r.status),
                  ],
                ),
                const SizedBox(height: 8),
                Text('Year: ${r.year}/${r.year + 1}',
                    style: const TextStyle(color: Colors.grey)),
                const SizedBox(height: 8),
                LinearProgressIndicator(
                  value: (r.progressPercent ?? 0) / 100,
                  backgroundColor: Colors.grey[200],
                  color: const Color(0xFF1B5E20),
                  minHeight: 8,
                  borderRadius: BorderRadius.circular(4),
                ),
                const SizedBox(height: 4),
                Text(
                    '${(r.progressPercent ?? 0).toStringAsFixed(0)}% complete',
                    style:
                        const TextStyle(fontSize: 12, color: Colors.grey)),
                if (r.inWorkflowProcess == true &&
                    r.currentWorkflowStatusName != null)
                  Padding(
                    padding: const EdgeInsets.only(top: 8),
                    child: Row(
                      children: [
                        const Icon(Icons.pending_actions,
                            size: 14, color: Colors.blue),
                        const SizedBox(width: 4),
                        Text(r.currentWorkflowStatusName!,
                            style: const TextStyle(
                                fontSize: 12, color: Colors.blue)),
                      ],
                    ),
                  ),
              ],
            ),
          ),
          // Workflow steps
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            child: Row(
              children: [
                const Text('Workflow Steps',
                    style: TextStyle(
                        fontSize: 16, fontWeight: FontWeight.bold)),
                const Spacer(),
                IconButton(
                    icon: const Icon(Icons.refresh),
                    onPressed: _load,
                    iconSize: 20),
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
                        child: Text(_error!,
                            style: const TextStyle(color: Colors.grey)),
                      ))
                    : _steps.isEmpty
                        ? const Center(
                            child: Text('No workflow steps found'))
                        : ListView.builder(
                            padding: const EdgeInsets.all(16),
                            itemCount: _steps.length,
                            itemBuilder: (ctx, i) {
                              final step = _steps[i];
                              return Card(
                                margin: const EdgeInsets.only(bottom: 12),
                                child: ExpansionTile(
                                  leading: Icon(
                                    _stepIcon(step.stepStatus),
                                    color: _stepColor(step.stepStatus),
                                  ),
                                  title: Text(step.stepName,
                                      style: const TextStyle(
                                          fontWeight: FontWeight.w600)),
                                  subtitle: Text(step.stepStatus),
                                  children: step.subSteps.isEmpty
                                      ? [
                                          const Padding(
                                            padding: EdgeInsets.all(12),
                                            child: Text('No sub-steps',
                                                style: TextStyle(
                                                    color: Colors.grey)),
                                          )
                                        ]
                                      : step.subSteps
                                          .map((sub) => ListTile(
                                                dense: true,
                                                leading: Icon(
                                                  _stepIcon(sub.subStepStatus),
                                                  color: _stepColor(
                                                      sub.subStepStatus),
                                                  size: 20,
                                                ),
                                                title:
                                                    Text(sub.subStepName),
                                                subtitle:
                                                    Text(sub.subStepStatus),
                                              ))
                                          .toList(),
                                ),
                              );
                            },
                          ),
          ),
        ],
      ),
    );
  }
}

class _StatusBadge extends StatelessWidget {
  final String status;
  const _StatusBadge(this.status);

  Color get _color {
    switch (status.toLowerCase()) {
      case 'published':
        return Colors.green;
      case 'approved':
        return Colors.teal;
      case 'underreview':
        return Colors.blue;
      case 'pending':
        return Colors.orange;
      case 'rejected':
        return Colors.red;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: _color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: _color.withOpacity(0.4)),
      ),
      child: Text(status,
          style: TextStyle(
              color: _color, fontSize: 12, fontWeight: FontWeight.w600)),
    );
  }
}
