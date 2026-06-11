# Compensation Rates Database (CRD) — System Administrator Manual
## Operations and Maintenance

**System URL:** http://172.16.22.175  
**API Endpoint:** http://172.16.22.175/api  
**Server Type:** Linux server running Docker (API + Databases) and Apache (Frontend)  
**Audience:** System Administrators

---

## Table of Contents

1. [System Architecture Overview](#1-system-architecture-overview)
2. [Server Access and Prerequisites](#2-server-access-and-prerequisites)
3. [User Management](#3-user-management)
   - 3.1 [Adding a New User (Web Application)](#31-adding-a-new-user-web-application)
   - 3.2 [Editing a User](#32-editing-a-user)
   - 3.3 [Resetting a User's Password](#33-resetting-a-users-password)
   - 3.4 [User Roles Explained](#34-user-roles-explained)
4. [Docker Container Management](#4-docker-container-management)
   - 4.1 [Starting All Services](#41-starting-all-services)
   - 4.2 [Stopping All Services](#42-stopping-all-services)
   - 4.3 [Viewing Running Containers](#43-viewing-running-containers)
   - 4.4 [Viewing Container Logs](#44-viewing-container-logs)
   - 4.5 [Restarting a Container](#45-restarting-a-container)
   - 4.6 [Rebuilding and Redeploying the API](#46-rebuilding-and-redeploying-the-api)
5. [Apache Frontend Maintenance](#5-apache-frontend-maintenance)
   - 5.1 [Apache Configuration for Reverse Proxy](#51-apache-configuration-for-reverse-proxy)
   - 5.2 [Deploying a Frontend Update](#52-deploying-a-frontend-update)
   - 5.3 [Restarting Apache](#53-restarting-apache)
6. [Database Maintenance](#6-database-maintenance)
   - 6.1 [Connecting to SQL Server (MSSQL)](#61-connecting-to-sql-server-mssql)
   - 6.2 [Connecting to MongoDB](#62-connecting-to-mongodb)
   - 6.3 [Backing Up the MSSQL Database](#63-backing-up-the-mssql-database)
   - 6.4 [Restoring the MSSQL Database](#64-restoring-the-mssql-database)
   - 6.5 [Backing Up MongoDB](#65-backing-up-mongodb)
7. [Email / Notification Configuration](#7-email--notification-configuration)
8. [Application Settings (In-App)](#8-application-settings-in-app)
9. [Workflow Configuration](#9-workflow-configuration)
10. [Common Administrative Tasks](#10-common-administrative-tasks)
    - 10.1 [Publishing District Rates](#101-publishing-district-rates)
    - 10.2 [Reverting a Workflow Step](#102-reverting-a-workflow-step)
    - 10.3 [Deleting a District Rate Submission](#103-deleting-a-district-rate-submission)
11. [Monitoring and Health Checks](#11-monitoring-and-health-checks)
12. [Security Notes](#12-security-notes)
13. [Troubleshooting Reference](#13-troubleshooting-reference)

---

## 1. System Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                  Linux Server — 172.16.22.175                │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Apache HTTP Server (frontend)                       │   │
│  │  • Serves Angular web app from /var/www/html         │   │
│  │  • Reverse-proxies /api/* → localhost:5100           │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌────────────────────────────────────────────────────┐     │
│  │  Docker                                            │     │
│  │  ┌─────────────────┐  ┌──────────────┐            │     │
│  │  │  crd.api        │  │  crd-mssql   │            │     │
│  │  │  (.NET 9 / 5100)│  │  (SQL Server │            │     │
│  │  │                 │  │   port 1433) │            │     │
│  │  └─────────────────┘  └──────────────┘            │     │
│  │  ┌─────────────────┐                               │     │
│  │  │  crd-mongo-db   │                               │     │
│  │  │  (MongoDB 27017)│                               │     │
│  │  └─────────────────┘                               │     │
│  └────────────────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────────────┘
```

| Component | Technology | Port | Notes |
|-----------|-----------|------|-------|
| Web Frontend | Angular (Apache) | 80 | Deployed to `/var/www/html` |
| REST API | .NET 9 (Docker) | 5100 (internal) | Proxied via `/api` on Apache |
| Primary Database | Microsoft SQL Server (Docker) | 1433 | Persistent volume `mssql-data` |
| Document Store | MongoDB (Docker) | 27017 | Persistent volume `mongo-data` |

---

## 2. Server Access and Prerequisites

### SSH Access

```bash
ssh <admin_user>@172.16.22.175
```

You will need `sudo` privileges to manage Docker and Apache.

### Required Tools on Server

| Tool | Purpose |
|------|---------|
| `docker` | Container runtime |
| `docker-compose` or `docker compose` | Multi-container management |
| `apache2` | Web server and reverse proxy |
| `systemctl` | Service management |

---

## 3. User Management

User management is performed through the **web application** by users with the **Admin** role.

### 3.1 Adding a New User (Web Application)

1. Log in to http://172.16.22.175 with an **Admin** account.
2. Click **Users** in the top navigation bar.
3. Click **Add User** (top-right, with an upload icon).
4. The **Register New User** modal opens.  Fill in the fields:

   | Field | Required | Description |
   |-------|----------|-------------|
   | **First Name** | Yes | User's first name (minimum 3 characters) |
   | **Last Name** | Yes | User's last name (minimum 3 characters) |
   | **Email** | Yes | User's email address — used as login username |
   | **Title** | No | Honorific title (e.g., Mr., Mrs., Dr.) |
   | **Designation** | No | Job title / designation (e.g., Valuer, Analyst) |
   | **Duty Station** | No | Office location (e.g., Kampala, Jinja) |
   | **Roles** | Yes | Select one or more roles (hold Ctrl to multi-select): `admin`, `User`, `Manager` |

5. Click **Register User**.
6. A green confirmation message confirms the user was created.
7. The system will **send an email** to the new user with a password-reset link so they can set their own password.
8. Click **Close** to dismiss the modal.

> **Note:** New users appear in the paginated users table.  Use **Next / Previous** buttons to navigate pages.

### 3.2 Editing a User

1. Find the user in the **Users** table.
2. Click the **Edit** button (grey) on their row.
3. The same modal opens pre-filled with their current details.
4. Modify the required fields.
5. Click **Update User**.

### 3.3 Resetting a User's Password

1. Find the user in the **Users** table.
2. Click **Reset Password** (grey button) on their row.
3. A loading overlay appears: *"Resetting Password… Please wait."*
4. The system sends a password-reset email to the user's registered email address.
5. The user clicks the link in the email and sets a new password on the Reset Password page.

> If the user does not receive the email, check the email configuration (see Section 7) and verify the email address in the user record is correct.

### 3.4 User Roles Explained

| Role | Web Portal Access | Notes |
|------|-----------------|-------|
| `User` | Reviews (view), Published Rates, Map, Crops/Trees, Structures | Read-only access |
| `Manager` | All User access + Submit District Rate, Manage Workflow, Moderate Rates, View Reports, Settings | Day-to-day operational role |
| `admin` | All Manager access + User Management, full Settings | Reserved for system administrators |

---

## 4. Docker Container Management

All commands below are run on the Linux server via SSH.

Navigate to the project directory first:

```bash
cd /path/to/CRD   # Replace with the actual deployment directory
```

### 4.1 Starting All Services

```bash
sudo docker compose up -d
```

The `-d` flag runs containers in detached (background) mode.

### 4.2 Stopping All Services

```bash
sudo docker compose down
```

> Data volumes (`mssql-data`, `mongo-data`) are **not** removed by `down`.  Add `--volumes` only if you intend to wipe all data.

### 4.3 Viewing Running Containers

```bash
sudo docker ps
```

Expected output (three running containers):

```
CONTAINER ID   IMAGE        COMMAND                  STATUS    PORTS
xxxxxxxxxxxx   crdapi       "dotnet CRD.API.dll"     Up        0.0.0.0:5100->5100/tcp
xxxxxxxxxxxx   mssql/...    "/opt/mssql/bin/..."     Up        0.0.0.0:1433->1433/tcp
xxxxxxxxxxxx   mongo        "docker-entrypoint.s…"   Up        0.0.0.0:27017->27017/tcp
```

If a container is not listed, it may have exited.  Run `sudo docker ps -a` to see all containers including stopped ones.

### 4.4 Viewing Container Logs

```bash
# API logs (live, last 100 lines)
sudo docker logs --tail 100 -f <api_container_name_or_id>

# MSSQL logs
sudo docker logs --tail 100 crd-mssql-db

# MongoDB logs
sudo docker logs --tail 100 crd-mongo-db
```

Replace `<api_container_name_or_id>` with the actual container name shown by `docker ps` (often `crd-crd.api-1` or similar).

### 4.5 Restarting a Container

```bash
# Restart the API (e.g., after a configuration change)
sudo docker compose restart crd.api

# Restart SQL Server
sudo docker compose restart mssql

# Restart MongoDB
sudo docker compose restart mongo
```

### 4.6 Rebuilding and Redeploying the API

When a new version of the API has been deployed:

```bash
# Pull latest code (if using git)
git pull origin main

# Rebuild and restart only the API container
sudo docker compose up -d --build crd.api
```

The `--build` flag forces Docker to rebuild the image from the Dockerfile.

---

## 5. Apache Frontend Maintenance

### 5.1 Apache Configuration for Reverse Proxy

The Apache virtual host configuration maps `/api` requests to the Docker API container.  A typical configuration looks like the following (location: `/etc/apache2/sites-available/crd.conf` or similar):

```apacheconf
<VirtualHost *:80>
    ServerName 172.16.22.175

    # Serve Angular frontend
    DocumentRoot /var/www/html

    # Reverse proxy: /api → Docker API on port 5100
    ProxyPreserveHost On
    ProxyPass /api http://localhost:5100/api
    ProxyPassReverse /api http://localhost:5100/api

    # Angular HTML5 routing — return index.html for all non-file requests
    <Directory /var/www/html>
        Options -Indexes
        AllowOverride All
        Require all granted

        RewriteEngine On
        RewriteBase /
        RewriteRule ^index\.html$ - [L]
        RewriteCond %{REQUEST_FILENAME} !-f
        RewriteCond %{REQUEST_FILENAME} !-d
        RewriteRule . /index.html [L]
    </Directory>
</VirtualHost>
```

Required Apache modules:

```bash
sudo a2enmod proxy proxy_http rewrite headers
sudo systemctl restart apache2
```

### 5.2 Deploying a Frontend Update

When the Angular web application has been updated and a new production build is available:

1. Build the Angular application (if building on the server):

   ```bash
   cd /path/to/CRD/WebUI
   npm install
   npm run build
   ```

   The compiled output appears in `WebUI/dist/web-ui/browser/` (or `WebUI/dist/web-ui/`).

2. Copy the build output to the Apache web root:

   ```bash
   sudo cp -r WebUI/dist/web-ui/browser/* /var/www/html/
   # OR (if the dist folder layout differs):
   sudo cp -r WebUI/dist/web-ui/* /var/www/html/
   ```

3. Set correct ownership:

   ```bash
   sudo chown -R www-data:www-data /var/www/html/
   ```

4. Verify the site loads at http://172.16.22.175.

### 5.3 Restarting Apache

```bash
# Graceful reload (no downtime)
sudo systemctl reload apache2

# Full restart
sudo systemctl restart apache2

# Check status
sudo systemctl status apache2
```

---

## 6. Database Maintenance

### 6.1 Connecting to SQL Server (MSSQL)

From the Linux server:

```bash
# Open an interactive session inside the SQL Server container
sudo docker exec -it crd-mssql-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'Str0p@ssword'
```

Once connected you can run T-SQL queries:

```sql
USE [crd-mssql-db];
SELECT name FROM sys.tables;
GO
```

Type `exit` to quit.

### 6.2 Connecting to MongoDB

```bash
# Open a mongo shell inside the MongoDB container
sudo docker exec -it crd-mongo-db mongosh \
  -u admin -p password --authenticationDatabase admin
```

Switch to the CRD database:

```javascript
use CRD
show collections
```

Type `exit` to quit.

### 6.3 Backing Up the MSSQL Database

```bash
# Create a backup file inside the container, then copy it to the host
sudo docker exec crd-mssql-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'Str0p@ssword' \
  -Q "BACKUP DATABASE [crd-mssql-db] TO DISK='/var/opt/mssql/backup/crd_$(date +%Y%m%d).bak' WITH FORMAT"

# Copy backup file from container to host
sudo docker cp crd-mssql-db:/var/opt/mssql/backup/ /home/<admin_user>/db_backups/
```

**Recommended:** Create a cron job to automate daily backups:

```bash
sudo crontab -e
```

Add the following line to run a backup every night at 02:00:

```
0 2 * * * docker exec crd-mssql-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Str0p@ssword' -Q "BACKUP DATABASE [crd-mssql-db] TO DISK='/var/opt/mssql/backup/crd_$(date +\%Y\%m\%d).bak' WITH FORMAT" >> /var/log/crd_backup.log 2>&1
```

### 6.4 Restoring the MSSQL Database

```bash
# Copy the backup file into the container
sudo docker cp /path/to/crd_20240101.bak crd-mssql-db:/var/opt/mssql/backup/

# Restore
sudo docker exec crd-mssql-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'Str0p@ssword' \
  -Q "RESTORE DATABASE [crd-mssql-db] FROM DISK='/var/opt/mssql/backup/crd_20240101.bak' WITH REPLACE"
```

> **Warning:** `WITH REPLACE` overwrites the existing database.  Ensure the API container is stopped before restoring: `sudo docker compose stop crd.api`.  Start it again after restore: `sudo docker compose start crd.api`.

### 6.5 Backing Up MongoDB

MongoDB stores published rate documents.  Back up using `mongodump`:

```bash
# Run mongodump inside the container
sudo docker exec crd-mongo-db mongodump \
  -u admin -p password --authenticationDatabase admin \
  --db CRD --out /data/backup/

# Copy the dump to the host
sudo docker cp crd-mongo-db:/data/backup/ /home/<admin_user>/mongo_backups/
```

---

## 7. Email / Notification Configuration

The API sends password-reset emails using the settings in `docker-compose.yml`.  To change the email configuration:

1. Open the `docker-compose.yml` file in the project directory.
2. Edit the `crd.api` service environment variables:

   ```yaml
   - MailSettings__Mail=notifications@yourdomain.com
   - MailSettings__DisplayName=CRD Notifications
   - MailSettings__Password=YourEmailPassword
   - MailSettings__Host=smtp.office365.com   # or your SMTP server
   - MailSettings__Port=587
   - MailSettings__AppUrl=http://172.16.22.175
   ```

3. Also update `CorsSettings__AllowedOrigins` if the server IP has changed:

   ```yaml
   - CorsSettings__AllowedOrigins=["http://172.16.22.175"]
   ```

4. Restart the API container to apply changes:

   ```bash
   sudo docker compose restart crd.api
   ```

> **Important:** If you change the `AppUrl`, password-reset email links will point to the new address.  Ensure the URL is reachable from users' email clients.

---

## 8. Application Settings (In-App)

Navigate to **Settings** in the web portal (Admin/Manager role required).

The Settings page provides configuration for:

- **Crops / Trees Languages** — manage the language tags used for crop and tree names in rate submissions.

Future settings (if applicable) for workflows, rate categories, and district configuration are also accessible here.

---

## 9. Workflow Configuration

The CRD uses a configurable multi-step workflow for each district rate submission.  To manage workflow templates:

1. Navigate to the **Workflow Management** section (accessible via the URL `/portal/settings` or via admin menus).
2. The Workflow Management page lists all defined workflows and their steps.
3. Use the **Workflow Step Management** sub-section to add, edit, or reorder steps and sub-steps.

Each step can have:
- A name and order position.
- One or more sub-steps with assigned roles.

Changes to workflow templates affect **new** district rate submissions.  Existing submissions continue to use the workflow configuration at the time they were created.

---

## 10. Common Administrative Tasks

### 10.1 Publishing District Rates

Publishing is the final step in the district rate workflow.  When the workflow reaches the Publication step:

1. Open the district rate's **Workflow Timeline** page.
2. Complete all sub-steps under the Publication step (mark each sub-step complete).
3. Once all sub-steps are done, click **✅ Complete Step**.
4. The district rate status changes to **Published** and becomes visible in the **Published Rates** page and in the mobile app.

### 10.2 Reverting a Workflow Step

If an error is discovered after a step has been marked complete:

1. Open the Workflow Timeline for the district rate.
2. Locate the completed step (green circle).
3. Click 🔁 **Revert Step** next to the step name.
4. The step status returns to active and its sub-steps can be redone.

> Reverting a step may affect subsequent steps.  Review the timeline carefully before reverting.

### 10.3 Deleting a District Rate Submission

1. Go to the **Reviews** page.
2. Find the district rate record.
3. Click the **Delete** button (red) on that row.
4. Confirm the deletion.

> **Caution:** Deleting a district rate removes all associated rates, workflow history, comments, and uploaded files.  This action cannot be undone.

---

## 11. Monitoring and Health Checks

### API Health

Check the API is running:

```bash
curl http://localhost:5100/api/health
# Or from a browser: http://172.16.22.175/api/health
```

A `200 OK` response confirms the API is live.

### Container Status

```bash
sudo docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

### Disk Usage

```bash
# Check overall disk usage
df -h

# Check Docker volumes
sudo docker system df

# Check Apache web root
du -sh /var/www/html
```

### Apache Access and Error Logs

```bash
sudo tail -100 /var/log/apache2/access.log
sudo tail -100 /var/log/apache2/error.log
```

---

## 12. Security Notes

1. **Change default passwords** after initial deployment:
   - MSSQL SA password (update in `docker-compose.yml` and `ConnectionStrings`).
   - MongoDB root password (update in `docker-compose.yml`).
   - All initial admin user passwords via the Reset Password feature.

2. **Restrict access** to ports 1433 (MSSQL) and 27017 (MongoDB) at the firewall level — these should only be accessible from `localhost` / Docker network, not from external hosts.

3. **HTTPS:** The current deployment uses plain HTTP on the internal network.  For sensitive deployments consider enabling HTTPS on Apache using a self-signed certificate or a certificate from an internal CA.

4. **Backup regularly:** Follow the backup procedures in Section 6 and store backups on a separate machine or network share.

5. **Keep Docker images updated:**

   ```bash
   sudo docker compose pull
   sudo docker compose up -d
   ```

---

## 13. Troubleshooting Reference

| Symptom | Likely Cause | Action |
|---------|-------------|--------|
| Web page returns 502 Bad Gateway | API container is down | Run `sudo docker compose up -d crd.api`; check logs |
| Web page returns 404 for Angular routes | Apache rewrite rules missing | Ensure `mod_rewrite` is enabled and `.htaccess` / RewriteRules are correct |
| Login fails for all users | API or database unreachable | Check all three Docker containers are running; check API logs |
| Password reset email not received | Email configuration incorrect | Verify SMTP settings in `docker-compose.yml`; restart API |
| Database connection error in API logs | MSSQL container not ready | Restart MSSQL container and then restart the API container |
| Disk space low | Database growth / log files | Run `sudo docker system prune` to remove unused images; prune old database backups |
| Container keeps restarting | Application error or config issue | Run `sudo docker logs <container>` to identify the error |
| Apache not serving updated frontend | Browser cache | Instruct users to perform a hard refresh (Ctrl + Shift + R); check `/var/www/html` contains the latest build |

---

## Quick Reference — Common Commands

```bash
# Start all services
sudo docker compose up -d

# Stop all services
sudo docker compose down

# View container status
sudo docker ps

# View API logs (live)
sudo docker logs -f <api_container_id>

# Restart API
sudo docker compose restart crd.api

# Restart Apache
sudo systemctl restart apache2

# Deploy updated frontend
sudo cp -r WebUI/dist/web-ui/browser/* /var/www/html/
sudo chown -R www-data:www-data /var/www/html/

# MSSQL backup
sudo docker exec crd-mssql-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'Str0p@ssword' \
  -Q "BACKUP DATABASE [crd-mssql-db] TO DISK='/var/opt/mssql/backup/crd_backup.bak' WITH FORMAT"
```

---

*Compensation Rates Database — System Administrator Manual*  
*Government of Uganda — Land Valuation*
