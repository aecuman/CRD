enum ModerationStatus {
  approved,
  revised,
  deferred,
  deleted,
  newEntry,
}

extension ModerationStatusLabel on ModerationStatus {
  String get label {
    switch (this) {
      case ModerationStatus.approved:
        return 'Approved';
      case ModerationStatus.revised:
        return 'Revised';
      case ModerationStatus.deferred:
        return 'Deferred';
      case ModerationStatus.deleted:
        return 'Deleted';
      case ModerationStatus.newEntry:
        return 'New Entry';
    }
  }

  int get value {
    switch (this) {
      case ModerationStatus.approved:
        return 1;
      case ModerationStatus.revised:
        return 3;
      case ModerationStatus.deferred:
        return 4;
      case ModerationStatus.deleted:
        return 5;
      case ModerationStatus.newEntry:
        return 6;
    }
  }
}

class PlantRateViewModel {
  final int id;
  final String plantName;
  final String growthStage;
  final String unit;
  final double? rate;
  final String? assumptions;
  final int moderationStatus;
  final String? moderationNotes;
  final String? quality;

  PlantRateViewModel({
    required this.id,
    required this.plantName,
    required this.growthStage,
    required this.unit,
    this.rate,
    this.assumptions,
    required this.moderationStatus,
    this.moderationNotes,
    this.quality,
  });

  factory PlantRateViewModel.fromJson(Map<String, dynamic> json) {
    return PlantRateViewModel(
      id: json['id'] ?? 0,
      plantName: json['plantName'] ?? json['plant'] ?? '',
      growthStage: json['growthStageName'] ?? json['growthStage'] ?? '',
      unit: json['unitName'] ?? json['unit'] ?? '',
      rate: (json['rate'] as num?)?.toDouble(),
      assumptions: json['assumptions'],
      moderationStatus: json['moderationStatus'] ?? json['status'] ?? 0,
      moderationNotes: json['moderationNotes'],
      quality: json['quality'],
    );
  }

  String get moderationStatusLabel {
    switch (moderationStatus) {
      case 1:
        return 'Approved';
      case 3:
        return 'Revised';
      case 4:
        return 'Deferred';
      case 5:
        return 'Deleted';
      case 6:
        return 'New Entry';
      default:
        return 'Pending';
    }
  }
}

class StructureRateViewModel {
  final int id;
  final String structureName;
  final String unit;
  final double? rate;
  final int moderationStatus;
  final String? moderationNotes;

  StructureRateViewModel({
    required this.id,
    required this.structureName,
    required this.unit,
    this.rate,
    required this.moderationStatus,
    this.moderationNotes,
  });

  factory StructureRateViewModel.fromJson(Map<String, dynamic> json) {
    return StructureRateViewModel(
      id: json['id'] ?? 0,
      structureName: json['structureName'] ?? json['structure'] ?? '',
      unit: json['unitName'] ?? json['unit'] ?? '',
      rate: (json['rate'] as num?)?.toDouble(),
      moderationStatus: json['moderationStatus'] ?? json['status'] ?? 0,
      moderationNotes: json['moderationNotes'],
    );
  }

  String get moderationStatusLabel {
    switch (moderationStatus) {
      case 1:
        return 'Approved';
      case 3:
        return 'Revised';
      case 4:
        return 'Deferred';
      case 5:
        return 'Deleted';
      case 6:
        return 'New Entry';
      default:
        return 'Pending';
    }
  }
}

class ModerationReportViewModel {
  final int districtRateId;
  final String districtName;
  final int year;
  final List<PlantRateViewModel> plantRates;
  final List<StructureRateViewModel> structureRates;

  ModerationReportViewModel({
    required this.districtRateId,
    required this.districtName,
    required this.year,
    required this.plantRates,
    required this.structureRates,
  });

  factory ModerationReportViewModel.fromJson(Map<String, dynamic> json) {
    return ModerationReportViewModel(
      districtRateId: json['districtRateId'] ?? 0,
      districtName: json['districtName'] ?? '',
      year: json['year'] ?? 0,
      plantRates: (json['plantRates'] as List<dynamic>? ?? [])
          .map((r) => PlantRateViewModel.fromJson(r))
          .toList(),
      structureRates: (json['structureRates'] as List<dynamic>? ?? [])
          .map((r) => StructureRateViewModel.fromJson(r))
          .toList(),
    );
  }
}
