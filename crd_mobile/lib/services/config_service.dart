import 'package:shared_preferences/shared_preferences.dart';

class ConfigService {
  static const String _baseUrlKey = 'base_url';
  static const String _defaultUrl = 'http://localhost:5100';

  static ConfigService? _instance;
  static SharedPreferences? _prefs;

  ConfigService._();

  static Future<ConfigService> getInstance() async {
    _instance ??= ConfigService._();
    _prefs ??= await SharedPreferences.getInstance();
    return _instance!;
  }

  String get baseUrl {
    final saved = _prefs?.getString(_baseUrlKey);
    if (saved == null || saved.trim().isEmpty) return _defaultUrl;
    // Remove trailing slash
    return saved.trim().replaceAll(RegExp(r'/+$'), '');
  }

  Future<void> setBaseUrl(String url) async {
    final trimmed = url.trim().replaceAll(RegExp(r'/+$'), '');
    await _prefs?.setString(_baseUrlKey, trimmed);
  }

  bool get isConfigured {
    final saved = _prefs?.getString(_baseUrlKey);
    return saved != null && saved.trim().isNotEmpty;
  }
}
