# Compensation Rates Database (CRD) — General User Manual

**Application URL:** http://172.16.22.175  
**Version:** 2024/2025  
**Audience:** All Users (Administrators, Managers, Field Officers)

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [Getting Started — Logging In](#2-getting-started--logging-in)
3. [Resetting Your Password](#3-resetting-your-password)
4. [Navigating the Portal](#4-navigating-the-portal)
5. [Map View](#5-map-view)
6. [Compensation Rate Reviews](#6-compensation-rate-reviews)
   - 6.1 [Creating a New District Rate Submission](#61-creating-a-new-district-rate-submission)
   - 6.2 [Managing the Review Workflow](#62-managing-the-review-workflow)
   - 6.3 [Workflow Steps and Sub-Steps](#63-workflow-steps-and-sub-steps)
7. [Adding Crops / Trees Rates](#7-adding-crops--trees-rates)
8. [Adding Structure Rates](#8-adding-structure-rates)
9. [Moderation](#9-moderation)
   - 9.1 [Configuring Comparable Districts](#91-configuring-comparable-districts)
   - 9.2 [Moderating Crops / Trees Rates](#92-moderating-crops--trees-rates)
   - 9.3 [Moderating Structure Rates](#93-moderating-structure-rates)
   - 9.4 [Moderation Report](#94-moderation-report)
10. [Published Rates](#10-published-rates)
11. [Crops / Trees Reference List](#11-crops--trees-reference-list)
12. [Structures Reference List](#12-structures-reference-list)
13. [Logging Out](#13-logging-out)
14. [Troubleshooting](#14-troubleshooting)

---

## 1. Introduction

The **Compensation Rates Database (CRD)** is a web-based application used to capture, review, moderate, and publish compensation rates for crops/trees and structures across districts in Uganda.  The system supports a structured workflow that takes district rate data from initial submission through moderation committees up to final publication.

Typical user roles:

| Role | Capabilities |
|------|-------------|
| **User** | View reviews, view published rates, navigate the map |
| **Manager** | All of the above, plus submit new district rates, manage workflow steps, moderate rates, and access reports |
| **Admin** | All of the above, plus manage system users and application settings |

---

## 2. Getting Started — Logging In

1. Open a web browser and navigate to **http://172.16.22.175**.
2. You will be redirected to the **Login** page automatically.

   ![Login page](../WebUI/src/assets/logo.png)

3. Enter your assigned **Email** address and **Password**.
4. Optionally tick **Remember me** to stay signed in on this device.
5. Click **Log in**.

If your credentials are correct you will be taken to the main portal. If an error message appears, verify your email address and password are typed correctly.

> **First-time login:** Your initial password will be sent to you by the system administrator.  You should change it after your first login.  Ask your administrator to reset your password if you have not received it.

---

## 3. Resetting Your Password

### Via the Reset-Password Link (self-service)

When an administrator triggers a password reset for your account, you will receive an email containing a secure reset link.

1. Click the link in the email.  You will be taken to the **Reset Password** page at `http://172.16.22.175/reset-password?token=...`.
2. Enter a **New Password** that satisfies the minimum requirements:
   - At least **6 characters** long (a longer password — 12 or more characters — is strongly recommended for security).
   - Must contain **at least one digit** (0–9).
   - Must contain **at least one special character** (e.g., `!`, `@`, `#`, `$`).
3. Re-enter the same password in the **Confirm Password** field.
4. Click **Reset Password**.
5. A green success message will confirm the change.  Click **Back to Login** and sign in with your new password.

> If the passwords do not match a red warning will appear.  Retype both fields and try again.

---

## 4. Navigating the Portal

After login the top navigation bar is always visible.  The items available depend on your role.

| Menu Item | Available To | Description |
|-----------|-------------|-------------|
| **Reviews** | All | Manage and track district rate review submissions |
| **Crops/Trees** | All | View the master list of crops and trees |
| **Structures** | All | View the master list of structures |
| **Published Rates** | All | Browse all published and expired rate schedules |
| **Users** | Admin only | Manage system user accounts |
| **Settings** | Admin / Manager | Application configuration |

Your name and title appear in the top-right corner.  Click your name to reveal the **Log Out** option.

---

## 5. Map View

The **Map** is the default home screen after login.  It displays districts on an interactive map of Uganda.

- **Click** a district marker or polygon to view its basic information.
- The map provides a geographic overview of which districts have active rate submissions.

---

## 6. Compensation Rate Reviews

Click **Reviews** in the navigation bar to open the Compensation Rate Reviews page.

This page lists all district rate records with:

| Column | Description |
|--------|-------------|
| **District** | Name of the district |
| **Year** | Financial year of the rate (e.g., 2024/2025) |
| **Active Step** | The current workflow step in progress |
| **Progress** | Percentage of workflow steps completed |
| **Assigned Roles** | The roles responsible for the current step |
| **Actions** | Buttons to Manage, Edit, Delete, or Start Review |

Use the **Show Completed** toggle at the top-right to include/hide completed reviews.

### 6.1 Creating a New District Rate Submission

> Requires **Manager** or **Admin** role.

1. Click **Submit New Rate** (top-right of the Reviews page).
2. A modal dialog opens:
   - **District** — select the target district from the drop-down.
   - **Year** — enter the financial year (e.g., `2024`).
   - **Upload Files** — attach the DLB submission documents (PDF, Excel, Word) from the district.  Multiple files can be selected.
3. Click **Save**.  The new district rate record will appear in the table.

> **Example districts with submitted rate data:** Kampala, Rukiga, Kanungu.

### 6.2 Managing the Review Workflow

Once a district rate record exists, start the formal review workflow:

1. Locate the district rate in the table.
2. Click **Start Review** (green button) — this initialises the workflow for that district.
3. Once the workflow is running, click **Manage** (blue button) or click the link under **Active Step** to open the **Workflow Timeline**.

### 6.3 Workflow Steps and Sub-Steps

The **Workflow Timeline** page shows the complete progression of a district rate review.

**Layout:**

- **Left panel** — a vertical timeline of workflow steps (e.g., Data Entry, Moderation, Approval, Publication).
  - Each step contains one or more **sub-steps** assigned to specific roles.
  - A **green circle** indicates a completed step; a **yellow circle** indicates the current active step; **grey circles** indicate pending steps.
- **Right panel** — an Actions & Info sidebar with:
  - DLB Submission file links.
  - Overall progress bar.
  - Quick-action buttons for Data, Moderation, and Reports.

**Working through a step:**

1. The current sub-step is highlighted with a 🟡 **Current** badge.
2. A Manager/Admin can:
   - **Attach a document** — use the file input and click **Upload**.
   - **Add a comment** — type in the comment box and click **Submit Comment**.
   - **Mark a sub-step complete** — click ✅ **Mark Complete** (only visible on the current sub-step).
3. Once all sub-steps within a step are complete the **✅ Complete Step** button appears.  Click it to advance to the next step.
4. To undo a completed step or sub-step, click 🔁 **Revert Step** / 🔁 **Revert** (visible on completed items for Managers/Admins).

**Workflow stages overview:**

| Stage | Description |
|-------|-------------|
| **1 — Data Entry** | Field data entered for crops/trees and structures |
| **2 — Moderation** | Rates reviewed and moderated by the committee |
| **3 — Approval** | Moderated rates reviewed for approval |
| **4 — Publication** | Approved rates published to the public record |

---

## 7. Adding Crops / Trees Rates

When the workflow is in the **Data Entry** step, the right sidebar on the Workflow Timeline page enables data entry:

1. Click **➕ Add Crops/Trees** (or navigate via the sidebar button).
2. You are taken to the **Plant Rates** entry screen for the selected district rate.
3. Enter rates for each crop/tree type, growth stage, and unit.
4. Save each entry.

> The Add Crops/Trees button is **disabled** once the workflow advances past the Data Entry step.

---

## 8. Adding Structure Rates

Similar to Crops/Trees:

1. Click **➕ Add Structures** from the Workflow Timeline sidebar.
2. Enter rates for each structure type and unit.
3. Save each entry.

---

## 9. Moderation

Moderation is available once the workflow reaches the **Moderation** stage.

### 9.1 Configuring Comparable Districts

Before moderating, configure which other districts' published rates will be used for comparison:

1. In the right panel of the Workflow Timeline, locate the **🧮 Moderation** section.
2. Click **Edit** beside **⚙️ Configure Comparables**.
3. In the modal that opens, select one or more comparable district rate records (e.g., Kampala 2023, Rukiga 2024).
4. Click **Save**.

The selected districts will appear in the comparables list and their rates will be shown alongside the current district's rates during moderation.

### 9.2 Moderating Crops / Trees Rates

1. Click **🌾 Moderate Crops/Trees** in the Moderation section of the Workflow Timeline sidebar.
2. The **Moderation** screen opens and displays a table of all submitted plant/crop rates.

**Table columns:**

| Column | Description |
|--------|-------------|
| **Plant Name** | Name of the crop or tree |
| **Unit** | Unit of measurement (e.g., hectare, per plant) |
| **Growth Stage** | Stage of the plant (seedling, mature, etc.) |
| **Current Rate** | Rate submitted for this period |
| **Previous Rate** | Original/baseline rate from the source document |
| **Moderated** | Yes / No — whether this rate has been moderated |
| **Status** | Moderation status: Approved, Revised, Deferred, or Unmoderated |

**Moderating a rate:**

1. Click the pencil (✏️) icon on a row that has been moderated (Status column) to **edit** the moderation decision.
2. Or click **Start Plant Rate Moderation** to begin the moderation session for all rates sequentially.
3. A **Moderation Pop-up** appears for each rate showing:
   - Current and previous rates.
   - Comparable district rates.
   - A field to enter the moderated rate.
   - A status selector: `Approved`, `Revised`, `Deferred`.
   - Notes field.
4. Click **Submit** to save and move to the next rate, or **Submit & Next** to save and advance automatically.
5. Click the **×** (close) button to exit the popup without saving.

**Moderation status meanings:**

| Status | Meaning |
|--------|---------|
| **Approved** | Rate accepted as submitted |
| **Revised** | Rate adjusted by the moderation committee |
| **Deferred** | Rate held pending further information |
| **Unmoderated** | Not yet reviewed |

Use the **Search** box above the table to filter rates by name.

### 9.3 Moderating Structure Rates

1. Click **🏗 Moderate Structures** from the Workflow Timeline sidebar.
2. The process is identical to plant rate moderation above, but for structure types and units.

### 9.4 Moderation Report

Once moderation is complete:

1. In the **📑 Reports** section of the Workflow Timeline sidebar, click **📘 Moderation Report**.
2. The report page shows a summary of all moderated rates.
3. Use the export options to download the report.

> The Moderation Report section is only enabled after the moderation workflow steps are complete.

---

## 10. Published Rates

Click **Published Rates** in the navigation bar to view all finalized rate schedules.

**Filter buttons** at the top allow you to view:

| Filter | Description |
|--------|-------------|
| **All** | All district rate records |
| **Valid** | Currently active published rates |
| **Expired** | Rates whose validity period has ended |
| **Not Published** | Records that have not yet been published |

The table shows:

| Column | Description |
|--------|-------------|
| **District** | Name of the district (click to view detail) |
| **Validity** | Year range the rates are valid (e.g., 2024–2025) |
| **Status** | Published / Expired / Not Published |

Click the district name (blue link) to open the **Published District** detail page showing all approved rates for that district.

---

## 11. Crops / Trees Reference List

Click **Crops/Trees** in the navigation bar to view and manage the master list of crop and tree categories used across all district rate submissions.

---

## 12. Structures Reference List

Click **Structures** in the navigation bar to view and manage the master list of structure types.

---

## 13. Logging Out

1. Click your name in the top-right corner of the navigation bar.
2. A small dropdown appears.
3. Click **Log Out**.

You will be redirected to the Login page.

---

## 14. Troubleshooting

| Problem | Likely Cause | Solution |
|---------|-------------|---------|
| Cannot log in | Wrong email or password | Check caps lock; contact your administrator to reset your password |
| "Whoops! Something went wrong" on login | Account inactive or server error | Contact your system administrator |
| Buttons appear greyed out (disabled) | You are on the wrong workflow step | Verify the current active step on the Workflow Timeline |
| Files not uploading | File too large or unsupported format | Use PDF, Excel (.xlsx), or Word (.docx) files under 20 MB |
| Page not loading / blank screen | Network connectivity | Ensure you are on the office LAN (http://172.16.22.175 is only accessible on the local network) |
| Moderation popup shows no comparable rates | Comparables not configured | See Section 9.1 — Configure Comparable Districts |

For technical assistance contact your **System Administrator**.

---

*Compensation Rates Database — General User Manual*  
*Government of Uganda — Land Valuation*
