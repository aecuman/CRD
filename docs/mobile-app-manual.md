# Compensation Rates Database (CRD) — Mobile App Manual

**Application Name:** CRD Mobile  
**Platform:** Android (Flutter)  
**Backend URL:** Configured in-app (default: http://172.16.22.175/api)  
**Audience:** Field Officers, Managers, and All Users

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [Installation](#2-installation)
3. [Configuring the API URL](#3-configuring-the-api-url)
4. [Logging In](#4-logging-in)
5. [Dashboard (Home)](#5-dashboard-home)
6. [Districts Tab](#6-districts-tab)
7. [Map Tab](#7-map-tab)
8. [Approved Rates Tab](#8-approved-rates-tab)
   - 8.1 [Filtering and Searching](#81-filtering-and-searching)
   - 8.2 [Downloading Rates (PDF / CSV)](#82-downloading-rates-pdf--csv)
9. [Settings and Logout](#9-settings-and-logout)
10. [Troubleshooting](#10-troubleshooting)

---

## 1. Introduction

**CRD Mobile** is the Android companion app for the Compensation Rates Database.  It allows field officers and managers to:

- View a live dashboard of district compensation rate statuses.
- Browse district details and published rate summaries.
- View an interactive geographic map of districts.
- Download approved compensation rates in PDF or CSV format for offline reference.

> **Note:** CRD Mobile is a **read-only** viewer and reporting tool.  Creating or moderating district rates is done through the web application at `http://172.16.22.175`.

---

## 2. Installation

1. Obtain the CRD Mobile APK file (`crd_mobile.apk`) from your system administrator.
2. On your Android device go to **Settings → Security** and enable **Install from unknown sources** (the exact path varies by Android version).
3. Open the APK file from your Downloads folder and tap **Install**.
4. Once installed, tap the **CRD Mobile** icon to launch the app.

> The app requires Android 6.0 (Marshmallow) or later.

---

## 3. Configuring the API URL

The mobile app must be pointed to the CRD server before you can log in.

1. On the **Login** screen, tap **Configure API URL** (below the Sign In button).
2. The **Settings / Config** screen opens.
3. Enter the server address:

   ```
   http://172.16.22.175/api
   ```

4. Tap **Save** (or the equivalent confirm button).
5. You will be returned to the Login screen.

> You only need to do this once.  The URL is saved on the device.  If the server address changes, repeat these steps.

---

## 4. Logging In

1. Launch **CRD Mobile**.
2. On the **Sign In** screen enter your:
   - **Email** — your registered email address.
   - **Password** — your account password.
3. Tap **Sign In**.

If login succeeds you will be taken to the **Dashboard**.  If an error appears:

- Verify your email and password.
- Tap the eye icon (👁) to reveal the password and check for typos.
- Ensure the API URL is configured correctly (see Section 3).
- Ensure your device is connected to the **office Wi-Fi** or local network (`172.16.22.175` is only accessible on the internal network).

> **Forgot your password?**  Contact your system administrator.  They can initiate a password reset from the web application, which will send a link to your email.

---

## 5. Dashboard (Home)

After login the **Dashboard** tab is shown by default.

### Top Bar

| Item | Action |
|------|--------|
| **CRD Dashboard** (title) | — |
| 🔄 Refresh icon | Reload dashboard data from the server |
| ⋮ Menu (top-right) | Opens Settings or Logout |

### Welcome Message

Your full name is displayed below the top bar: *"Welcome, [Your Name]"*.

### Overview Cards

Six metric cards display a live count of district rate records:

| Card | Description |
|------|-------------|
| **Total Districts** | Total number of district rate records in the system |
| **Published** | Districts with currently published (active) rates |
| **Expired** | Districts whose published rates have passed the validity date |
| **Not Published** | Districts without any published rates |
| **In Workflow** | Districts actively going through the review/moderation process |

Tap a card to navigate to the relevant section (where applicable).

### Recent Districts

Below the cards a list of the five most recently updated district rate records is shown.  Each item displays:

- District name.
- Financial year (e.g., 2024/2025).
- Status badge (Published / Expired / Not Published).

Pull down on the list to **refresh** all data.

---

## 6. Districts Tab

Tap the **Districts** icon (📋) in the bottom navigation bar.

This screen shows a complete list of all districts in the system with their rate status.

- **Tap** a district row to open the **District Detail** screen.
- The detail screen shows rate information for that district, including validity dates and publication status.

---

## 7. Map Tab

Tap the **Map** icon (🗺) in the bottom navigation bar.

An interactive map of Uganda is displayed.

- District areas or markers are plotted on the map.
- **Tap** a district on the map to see its basic rate information in a popup.
- Use standard pinch-to-zoom and drag gestures to navigate the map.

---

## 8. Approved Rates Tab

Tap the **Rates** icon (✅) in the bottom navigation bar.

This screen lists all district rate records with their publication status.

### 8.1 Filtering and Searching

**Search bar** — type a district name to filter the list in real time.

**Filter chips** — tap a chip to filter by status:

| Chip | Description |
|------|-------------|
| **All** | Show all district rate records |
| **Valid** | Show only currently active published rates |
| **Expired** | Show only expired rates |

Tap the 🔄 refresh icon (top-right) to reload data from the server.

### 8.2 Downloading Rates (PDF / CSV)

For any district that has published rates a **⬇ download** icon appears on the right of its row.

1. Tap the **⬇ download** icon next to the desired district.
2. A bottom sheet appears with two options:
   - **📄 Download as PDF** — generates a formatted PDF report containing:
     - District name and financial year.
     - Table of all approved Plant/Crop rates (name, growth stage, unit, rate in UGX, status).
     - Table of all approved Structure rates (name, unit, rate in UGX, status).
   - **📊 Download as CSV** — generates a CSV spreadsheet of approved plant rates suitable for import into Excel.
3. Tap your preferred format.  The file is saved to the device's temporary storage.
4. A notification bar ("snack bar") appears at the bottom:
   - *"PDF saved: /path/to/file.pdf"*
   - Tap **Open** in the snack bar to open the file immediately with the device's default viewer.

> **CSV note:** The CSV currently includes crop/tree rates.  Structure rates are included in the PDF only.

---

## 9. Settings and Logout

### Accessing Settings

1. Tap the **⋮ menu** (three-dot menu) in the top-right corner of the Dashboard.
2. Tap **Settings**.
3. The Config screen opens where you can update the **API URL** (see Section 3).

### Logging Out

1. Tap the **⋮ menu** in the top-right corner of the Dashboard.
2. Tap **Logout** (shown in red).
3. You are returned to the Login screen.  Your session is cleared from the device.

---

## 10. Troubleshooting

| Problem | Likely Cause | Solution |
|---------|-------------|---------|
| "Connection error. Check URL in Settings." | API URL not set or incorrect | Go to Configure API URL and enter `http://172.16.22.175/api` |
| "Login failed. Check credentials." | Wrong email or password | Double-check credentials; contact administrator for a password reset |
| Dashboard shows zeros / no data | Server not reachable | Ensure you are on the office Wi-Fi; tap the refresh button |
| Download fails or file won't open | Storage permission denied | Go to device Settings → Apps → CRD Mobile → Permissions and enable Storage |
| Map does not load | Internet / network issue | The map tiles require network access; check connectivity |
| App crashes on launch | Outdated APK version | Obtain the latest APK from your system administrator and reinstall |

For technical assistance contact your **System Administrator**.

---

*Compensation Rates Database — Mobile App Manual*  
*Government of Uganda — Land Valuation*
