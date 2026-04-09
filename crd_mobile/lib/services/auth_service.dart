import 'dart:convert';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../models/auth_models.dart';

class AuthService {
  static const String _tokenKey = 'auth_token';
  static const String _userKey = 'current_user';

  static final AuthService _instance = AuthService._internal();
  factory AuthService() => _instance;
  AuthService._internal();

  final _storage = const FlutterSecureStorage();

  CurrentUser? _currentUser;
  String? _token;

  CurrentUser? get currentUser => _currentUser;
  String? get token => _token;
  bool get isAuthenticated => _token != null && _token!.isNotEmpty;

  Future<void> init() async {
    _token = await _storage.read(key: _tokenKey);
    final userJson = await _storage.read(key: _userKey);
    if (userJson != null) {
      try {
        _currentUser = CurrentUser.fromJson(json.decode(userJson));
      } catch (_) {
        _currentUser = null;
      }
    }
  }

  Future<void> saveSession(String token, CurrentUser user) async {
    _token = token;
    _currentUser = user;
    await _storage.write(key: _tokenKey, value: token);
    await _storage.write(key: _userKey, value: json.encode({
      'id': user.id,
      'fullName': user.fullName,
      'email': user.email,
      'roles': user.roles,
    }));
  }

  Future<void> logout() async {
    _token = null;
    _currentUser = null;
    await _storage.delete(key: _tokenKey);
    await _storage.delete(key: _userKey);
  }

  Map<String, String> get authHeaders {
    return {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      if (_token != null) 'Authorization': 'Bearer $_token',
    };
  }
}
