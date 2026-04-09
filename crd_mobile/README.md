# CRD Mobile App

A Flutter-based mobile application for the Compensation Rate Database (CRD) system. Allows field users and moderators to view district statuses, browse approved compensation rates, interact with the Uganda district map, and perform moderation actions — all from an Android device.

## Features

| Screen | Description |
|--------|-------------|
| **Config** | Set the API base URL (no Play Store — sideloaded via Android Studio) |
| **Login** | Email / password authentication via CRD JWT API |
| **Dashboard** | Overview metrics: total districts, published, expired, in-workflow |
| **District List** | Filterable/searchable list of all districts with status badges |
| **District Detail** | Workflow step drill-down with progress indicator; tap → Moderate |
| **Map** | Colour-coded Uganda choropleth map (flutter_map + OpenStreetMap tiles) |
| **Approved Rates** | Browse published district rates; download as **PDF** or **CSV** |
| **Moderation** | Approve / Revise / Defer / Delete / New-Entry individual plant & structure rates |

## Project Structure

```
crd_mobile/
├── android/                  # Android native project (open in Android Studio)
├── assets/
│   ├── districts.json        # Uganda districts GeoJSON for the map
│   └── logo.png
├── lib/
│   ├── main.dart             # Entry point & routing
│   ├── models/               # Dart data models (auth, districts, rates)
│   ├── services/
│   │   ├── config_service.dart   # Persists API base URL
│   │   ├── auth_service.dart     # JWT token storage (secure storage)
│   │   └── api_service.dart      # HTTP client for all API endpoints
│   └── screens/
│       ├── config_screen.dart
│       ├── login_screen.dart
│       ├── home_screen.dart
│       ├── district_list_screen.dart
│       ├── district_detail_screen.dart
│       ├── map_screen.dart
│       ├── published_rates_screen.dart
│       └── moderation_screen.dart
└── pubspec.yaml
```

## Getting Started (Android Studio)

### Prerequisites
- [Flutter SDK](https://docs.flutter.dev/get-started/install) ≥ 3.19
- Android Studio (Hedgehog or later) with Flutter plugin
- Android SDK ≥ 21 (Android 5.0)

### Steps

1. **Open the project in Android Studio**
   ```
   File → Open → select the `crd_mobile/` folder
   ```

2. **Get dependencies**
   ```bash
   flutter pub get
   ```

3. **Configure the API URL** — on first launch the app shows a *Config* screen. Set:
   - **Emulator**: `http://10.0.2.2:5100`
   - **Physical device**: `http://<your-PC-local-IP>:5100`

4. **Run / Debug**
   ```bash
   flutter run
   ```
   Or use the Android Studio Run button.

5. **Build a release APK**
   ```bash
   flutter build apk --release
   # Output: build/app/outputs/flutter-apk/app-release.apk
   ```

## API Compatibility

The app targets the CRD backend (`CRD.API`) and uses these endpoints:

| Purpose | Endpoint |
|---------|----------|
| Login | `POST /api/Auth/login` |
| District rates | `GET /api/district-rates` |
| All districts | `GET /api/district-rates/all` |
| Published rates | `GET /api/district-rates/published` |
| Plant rates | `GET /api/district-rates/plants/{districtRateId}` |
| Moderated plant rates | `GET /api/district-rates/moderation/plant-rates/{id}` |
| Moderated structure rates | `GET /api/district-rates/moderation/structure-rates/{id}` |
| Moderate a rate | `PUT /api/district-rates/moderate/{id}` |
| Workflow status | `GET /api/district-workflow/status/{districtId}/{districtRateId}` |

## Permissions

The app requests the following Android permissions:
- `INTERNET` — required to call the CRD API
- `WRITE_EXTERNAL_STORAGE` (≤ API 28) — for saving exported files
- `READ_EXTERNAL_STORAGE` (≤ API 32) — for opening exported files
