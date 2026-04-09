import 'dart:convert';
import 'dart:io';
import 'package:http/http.dart' as http;
import '../models/auth_models.dart';
import '../models/district_models.dart';
import '../models/rate_models.dart';
import 'auth_service.dart';
import 'config_service.dart';

class ApiException implements Exception {
  final int statusCode;
  final String message;
  ApiException(this.statusCode, this.message);

  @override
  String toString() => 'ApiException($statusCode): $message';
}

class ApiService {
  static final ApiService _instance = ApiService._internal();
  factory ApiService() => _instance;
  ApiService._internal();

  final AuthService _auth = AuthService();
  late ConfigService _config;

  Future<void> init() async {
    _config = await ConfigService.getInstance();
  }

  String get _base => _config.baseUrl;

  Map<String, String> get _headers => _auth.authHeaders;

  Future<dynamic> _get(String path,
      {Map<String, String>? query}) async {
    final uri = Uri.parse('$_base$path').replace(queryParameters: query);
    final response = await http.get(uri, headers: _headers);
    return _handle(response);
  }

  Future<dynamic> _post(String path, Map<String, dynamic> body) async {
    final uri = Uri.parse('$_base$path');
    final response = await http.post(uri,
        headers: _headers, body: json.encode(body));
    return _handle(response);
  }

  Future<dynamic> _put(String path, Map<String, dynamic> body) async {
    final uri = Uri.parse('$_base$path');
    final response = await http.put(uri,
        headers: _headers, body: json.encode(body));
    return _handle(response);
  }

  dynamic _handle(http.Response response) {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      if (response.body.isEmpty) return null;
      return json.decode(response.body);
    }
    String msg;
    try {
      final err = json.decode(response.body);
      msg = err['message'] ?? err['title'] ?? response.body;
    } catch (_) {
      msg = response.body;
    }
    throw ApiException(response.statusCode, msg);
  }

  // ──────────────────────────────────────────────
  // Auth
  // ──────────────────────────────────────────────
  Future<LoginResponse> login(String email, String password) async {
    final data = await _post('/api/Auth/login', {
      'email': email,
      'password': password,
    });
    return LoginResponse.fromJson(data);
  }

  // ──────────────────────────────────────────────
  // Districts
  // ──────────────────────────────────────────────
  Future<List<DistrictRateDto>> getDistrictRates({
    int? year,
    String? status,
    int? districtId,
  }) async {
    final q = <String, String>{};
    if (year != null) q['year'] = year.toString();
    if (status != null && status.isNotEmpty) q['status'] = status;
    if (districtId != null) q['districtId'] = districtId.toString();

    final data = await _get('/api/district-rates', query: q);
    return (data as List).map((d) => DistrictRateDto.fromJson(d)).toList();
  }

  Future<List<District>> getAllDistricts() async {
    final data = await _get('/api/district-rates/all');
    return (data as List).map((d) => District.fromJson(d)).toList();
  }

  Future<List<PublishedRateSummaryDto>> getPublishedRates(
      {String filter = 'all'}) async {
    final data = await _get('/api/district-rates/published',
        query: {'filter': filter});
    return (data as List)
        .map((d) => PublishedRateSummaryDto.fromJson(d))
        .toList();
  }

  Future<PublishedRateSummaryDto> getDistrictPublishedRates(
      int districtId) async {
    final data = await _get('/api/district-rates/published/$districtId');
    return PublishedRateSummaryDto.fromJson(data);
  }

  // ──────────────────────────────────────────────
  // District workflow status
  // ──────────────────────────────────────────────
  Future<List<DistrictWorkflowStatusDto>> getDistrictWorkflowStatus(
      int districtId, int districtRateId) async {
    final data = await _get(
        '/api/district-workflow/status/$districtId/$districtRateId');
    return (data as List)
        .map((d) => DistrictWorkflowStatusDto.fromJson(d))
        .toList();
  }

  // ──────────────────────────────────────────────
  // Plant rates
  // ──────────────────────────────────────────────
  Future<List<PlantRateViewModel>> getPlantRates(int districtRateId,
      {int? plantId}) async {
    String path = '/api/district-rates/plants/$districtRateId';
    if (plantId != null) path += '/$plantId';
    final data = await _get(path);
    return (data as List)
        .map((d) => PlantRateViewModel.fromJson(d))
        .toList();
  }

  Future<List<PlantRateViewModel>> getModeratedPlantRates(
      int districtRateId) async {
    final data = await _get(
        '/api/district-rates/moderation/plant-rates/$districtRateId',
        query: {'includeUnmoderated': 'true'});
    return (data as List)
        .map((d) => PlantRateViewModel.fromJson(d))
        .toList();
  }

  Future<List<StructureRateViewModel>> getModeratedStructureRates(
      int districtRateId) async {
    final data = await _get(
        '/api/district-rates/moderation/structure-rates/$districtRateId',
        query: {'includeUnmoderated': 'true'});
    return (data as List)
        .map((d) => StructureRateViewModel.fromJson(d))
        .toList();
  }

  // ──────────────────────────────────────────────
  // Moderation
  // ──────────────────────────────────────────────
  Future<ModerationReportViewModel> getModerationReport(
      int districtRateId, {String? filter}) async {
    final q = <String, String>{};
    if (filter != null) q['filter'] = filter;
    final data = await _get(
        '/api/district-rates/moderation/report/$districtRateId',
        query: q);
    return ModerationReportViewModel.fromJson(data);
  }

  Future<void> moderateRate({
    required int rateId,
    required int status,
    String? notes,
    String? deferredReason,
  }) async {
    await _put('/api/district-rates/moderate/$rateId', {
      'rateId': rateId,
      'status': status,
      if (notes != null) 'moderationNotes': notes,
      if (deferredReason != null) 'deferredReason': deferredReason,
    });
  }

  // ──────────────────────────────────────────────
  // District workflow overall status
  // ──────────────────────────────────────────────
  Future<List<dynamic>> getDistrictStatuses() async {
    final data = await _get('/api/workflow/districts/status');
    return data as List;
  }
}
