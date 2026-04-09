class LoginResponse {
  final bool succeed;
  final String message;
  final String token;
  final CurrentUser user;

  LoginResponse({
    required this.succeed,
    required this.message,
    required this.token,
    required this.user,
  });

  factory LoginResponse.fromJson(Map<String, dynamic> json) {
    return LoginResponse(
      succeed: json['succeed'] ?? false,
      message: json['message'] ?? '',
      token: json['token'] ?? '',
      user: CurrentUser.fromJson(json['user'] ?? {}),
    );
  }
}

class CurrentUser {
  final int id;
  final String fullName;
  final String email;
  final List<String> roles;

  CurrentUser({
    required this.id,
    required this.fullName,
    required this.email,
    required this.roles,
  });

  factory CurrentUser.fromJson(Map<String, dynamic> json) {
    return CurrentUser(
      id: json['id'] ?? 0,
      fullName: json['fullName'] ?? '',
      email: json['email'] ?? '',
      roles: List<String>.from(json['roles'] ?? []),
    );
  }

  bool get isAdmin =>
      roles.contains('admin') || roles.contains('superadmin');

  bool get isModerator =>
      roles.contains('moderator') || isAdmin;
}
