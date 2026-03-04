# Bulk User Seeding Guide

## Overview
The bulk user seeding feature allows you to import users from the `users_migration.csv` file into the database with their passwords properly hashed and persisted.

## How to Use

### 1. Prepare the CSV File
The `users_migration.csv` must be in the root of the project with these columns:
```
Title,Firstname,Lastname,UserName,Email,Password,Role,Designation,DutyStation
```

**Example:**
```csv
Mr.,Gilbert,Kermundu,gilbert.kermundu@mlhud.go.ug,gilbert.kermundu@mlhud.go.ug,Gilbert.Kermundu@2026,Users,Overall Supervisor/ Headquarters,CGV
Ms.,Dorothy,Natukunda,dorothy.natukunda@mlhud.go.ug,dorothy.natukunda@mlhud.go.ug,Dorothy.Natukunda@2026,Users,PGV,Regional Supervisor Western
```

### 2. Run the Seed Command

**Option A: From Project Root** (Production/Remote)
```powershell
dotnet run --project CRD.API seedusers
```

**Option B: With Custom CSV Path**
```powershell
dotnet run --project CRD.API seedusers "C:\path\to\custom_migration.csv"
```

**Option C: Using Migration File in Root**
```bash
cd CRD.API
dotnet run seedusers "../users_migration.csv"
```

### 3. Monitor Seeding Progress

The application will log:
- Starting bulk user seeding
- Success/failure count
- Details on each user creation
- Warnings for duplicate users
- Completion summary

Example output:
```
Starting bulk user seeding from CSV: users_migration.csv
User Gilbert Kermundu (gilbert.kermundu@mlhud.go.ug) created successfully.
User Dorothy Natukunda (dorothy.natukunda@mlhud.go.ug) created successfully.
...
Bulk user seeding completed. Success: 62, Failures: 0
```

## Features

✓ **Passwords are hashed and persisted** - Uses the same `IUserManager` that the API uses  
✓ **Duplicate detection** - Skips users that already exist by email  
✓ **Role assignment** - Automatically creates/assigns roles from CSV  
✓ **Full audit logging** - All operations are logged  
✓ **Error handling** - Continues on errors, logs failures  
✓ **CSV parsing** - Handles quoted fields and special characters  

## CSV Column Mapping

| Column | Description | Example |
|--------|-------------|---------|
| **Title** | Honorific title | Mr., Ms., Dr., Ag. |
| **Firstname** | First name (including middle names) | Gilbert, Tonny Kato |
| **Lastname** | Last name | Kermundu, Magembe |
| **UserName** | Login username (typically email) | gilbert.kermundu@mlhud.go.ug |
| **Email** | Email address | gilbert.kermundu@mlhud.go.ug |
| **Password** | Initial password (will be hashed) | Gilbert.Kermundu@2026 |
| **Role** | Role to assign | Users, Admin |
| **Designation** | Job designation | Overall Supervisor |
| **DutyStation** | Duty station/location | CGV, Headquarters |

## Security Notes

✓ Passwords are **never stored in plain text**  
✓ Uses ASP.NET Identity's password hashing  
✓ `EmailConfirmed` set to `true` for bulk imports  
✓ `LockoutEnabled` set to `true` for security  
✓ Only Admin users can trigger seeding via the API

## Database Schema

The `ApplicationUser` table now includes:

```sql
CREATE TABLE AspNetUsers (
    Id INT PRIMARY KEY,
    Title NVARCHAR(MAX),
    Firstname NVARCHAR(MAX),
    Lastname NVARCHAR(MAX),
    Designation NVARCHAR(MAX),
    DutyStation NVARCHAR(MAX),
    Role NVARCHAR(MAX),
    Email NVARCHAR(256),
    UserName NVARCHAR(256),
    PasswordHash NVARCHAR(MAX),
    ...
)
```

## Migration Process (Recommended)

### For Development:
```powershell
# 1. Build the application
dotnet build

# 2. Ensure database is up to date
cd CRD.API
dotnet ef database update

# 3. Run seed command
dotnet run seedusers

# 4. Verify in database
```

### For Production:
```powershell
# 1. Ensure migrations are applied
dotnet ef database update --configuration Release

# 2. Deploy and run seed command
dotnet run --configuration Release seedusers

# 3. Monitor logs for completion
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "CSV file not found" | Ensure CSV path is correct, use absolute path if needed |
| "Invalid CSV line" | Check for missing columns in CSV header |
| "User already exists" | This is logged as warning, check if user needs updating instead |
| "Failed to create role" | Common on re-runs, seeding continues safely |
| Password hashing errors | Verify password meets complexity requirements |

## Password Requirements

Default passwords follow pattern: `FirstName.LastName@2026`

Must contain:
- At least one uppercase letter (FirstName starts with capital)
- At least one lowercase letter (LastName)
- At least one number (2026)
- At least one special character (@)
- Minimum 8 characters

## API Endpoint (Alternative)

Once seeded, you can also bulk import via API:

```http
POST /api/auth/bulk-import
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "users": [
    {
      "title": "Mr.",
      "firstname": "Gilbert",
      "lastname": "Kermundu",
      "userName": "gilbert.kermundu@mlhud.go.ug",
      "email": "gilbert.kermundu@mlhud.go.ug",
      "password": "Gilbert.Kermundu@2026",
      "role": "Users",
      "designation": "Overall Supervisor",
      "dutyStation": "CGV"
    }
  ]
}
```

## Files Modified

- `CRD.Domain/Identity/ApplicationUser.cs` - Added Title, Designation, DutyStation
- `CRD.Persistence/SeedUsers.cs` - Added `SeedUsersFromCsvAsync()` method
- `CRD.Persistence/UserSeedDto.cs` - New DTO for seed data
- `CRD.API/Program.cs` - Added seedusers command handler
