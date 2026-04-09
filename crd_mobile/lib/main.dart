import 'package:flutter/material.dart';
import 'services/auth_service.dart';
import 'services/config_service.dart';
import 'screens/config_screen.dart';
import 'screens/login_screen.dart';
import 'screens/home_screen.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final config = await ConfigService.getInstance();
  final isConfigured = config.isConfigured;
  await AuthService().init();
  runApp(CRDApp(isConfigured: isConfigured));
}

class CRDApp extends StatelessWidget {
  final bool isConfigured;
  const CRDApp({super.key, required this.isConfigured});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'CRD Mobile',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: const Color(0xFF1B5E20),
          primary: const Color(0xFF1B5E20),
        ),
        useMaterial3: true,
        appBarTheme: const AppBarTheme(
          backgroundColor: Color(0xFF1B5E20),
          foregroundColor: Colors.white,
          elevation: 0,
        ),
        elevatedButtonTheme: ElevatedButtonThemeData(
          style: ElevatedButton.styleFrom(
            backgroundColor: const Color(0xFF1B5E20),
            foregroundColor: Colors.white,
          ),
        ),
      ),
      initialRoute: _initialRoute(),
      routes: {
        '/config': (_) => const ConfigScreen(),
        '/login': (_) => const LoginScreen(),
        '/home': (_) => const HomeScreen(),
      },
    );
  }

  String _initialRoute() {
    if (!isConfigured) return '/config';
    if (!AuthService().isAuthenticated) return '/login';
    return '/home';
  }
}
