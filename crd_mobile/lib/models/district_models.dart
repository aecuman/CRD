class DistrictRateDto {
  final int id;
  final int districtId;
  final String districtName;
  final int year;
  final String status;
  final double? progressPercent;
  final bool? inWorkflowProcess;
  final String? currentWorkflowStatusName;

  DistrictRateDto({
    required this.id,
    required this.districtId,
    required this.districtName,
    required this.year,
    required this.status,
    this.progressPercent,
    this.inWorkflowProcess,
    this.currentWorkflowStatusName,
  });

  factory DistrictRateDto.fromJson(Map<String, dynamic> json) {
    return DistrictRateDto(
      id: json['id'] ?? 0,
      districtId: json['districtId'] ?? 0,
      districtName: json['districtName'] ?? '',
      year: json['year'] ?? 0,
      status: json['status'] ?? '',
      progressPercent: (json['progressPercent'] as num?)?.toDouble(),
      inWorkflowProcess: json['inWorkflowProcess'],
      currentWorkflowStatusName: json['currentWorkflowStatusName'],
    );
  }
}

class District {
  final int id;
  final String name;

  District({required this.id, required this.name});

  factory District.fromJson(Map<String, dynamic> json) {
    return District(
      id: json['id'] ?? 0,
      name: json['name'] ?? '',
    );
  }
}

class PublishedRateSummaryDto {
  final int districtId;
  final String? districtName;
  final int? year;
  final String? status;
  final bool? isExpired;
  final bool? inWorkflowProcess;
  final String? currentWorkflowStatusName;

  PublishedRateSummaryDto({
    required this.districtId,
    this.districtName,
    this.year,
    this.status,
    this.isExpired,
    this.inWorkflowProcess,
    this.currentWorkflowStatusName,
  });

  factory PublishedRateSummaryDto.fromJson(Map<String, dynamic> json) {
    return PublishedRateSummaryDto(
      districtId: json['districtId'] ?? 0,
      districtName: json['districtName'],
      year: json['year'],
      status: json['status'],
      isExpired: json['isExpired'],
      inWorkflowProcess: json['inWorkflowProcess'],
      currentWorkflowStatusName: json['currentWorkflowStatusName'],
    );
  }
}

class DistrictWorkflowStatusDto {
  final int stepId;
  final String stepName;
  final String stepStatus;
  final int order;
  final List<SubStepDto> subSteps;

  DistrictWorkflowStatusDto({
    required this.stepId,
    required this.stepName,
    required this.stepStatus,
    required this.order,
    required this.subSteps,
  });

  factory DistrictWorkflowStatusDto.fromJson(Map<String, dynamic> json) {
    return DistrictWorkflowStatusDto(
      stepId: json['stepId'] ?? 0,
      stepName: json['stepName'] ?? '',
      stepStatus: json['stepStatus'] ?? '',
      order: json['order'] ?? 0,
      subSteps: (json['subSteps'] as List<dynamic>? ?? [])
          .map((s) => SubStepDto.fromJson(s))
          .toList(),
    );
  }
}

class SubStepDto {
  final int subStepId;
  final String subStepName;
  final String subStepStatus;
  final int order;

  SubStepDto({
    required this.subStepId,
    required this.subStepName,
    required this.subStepStatus,
    required this.order,
  });

  factory SubStepDto.fromJson(Map<String, dynamic> json) {
    return SubStepDto(
      subStepId: json['subStepId'] ?? 0,
      subStepName: json['subStepName'] ?? '',
      subStepStatus: json['subStepStatus'] ?? '',
      order: json['order'] ?? 0,
    );
  }
}
