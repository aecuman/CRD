import csv
import re

# Read the current CSV
input_csv = r'c:\Users\aecum\GitHub\CRD\data.csv'
output_csv = r'c:\Users\aecum\GitHub\CRD\users_migration.csv'

# Function to extract title and parse name
def parse_name_with_title(full_name):
    # Extract title
    title_match = re.match(r'^(Mr\.|Ms\.|Dr\.|Ag\.|AGV|SGV|GV|AC/V|PGV)\s+', full_name)
    title = title_match.group(1) if title_match else ''
    
    # Remove title from name
    cleaned_name = re.sub(r'^(Mr\.|Ms\.|Dr\.|Ag\.|AGV|SGV|GV|AC/V|PGV)\s+', '', full_name).strip()
    
    # Split on space
    parts = cleaned_name.split()
    
    if len(parts) == 0:
        return '', '', ''
    elif len(parts) == 1:
        # Single name - treat as first name
        return parts[0], '', title
    elif len(parts) == 2:
        # Two parts: first and last
        return parts[0], parts[1], title
    else:
        # More than 2 parts: first + middle names, last part is last name
        first_name = ' '.join(parts[:-1])
        last_name = parts[-1]
        return first_name, last_name, title

# Function to generate email
def generate_email(first_name, last_name):
    # Use only base first name (first word)
    first_word = first_name.split()[0].lower()
    if not last_name:
        return f"{first_word}@mlhud.go.ug"
    return f"{first_word}.{last_name.lower()}@mlhud.go.ug"

# Function to generate password
def generate_password(first_name, last_name):
    # Use only base first name (first word)
    first_word = first_name.split()[0]
    if not last_name:
        return f"{first_word}@2026"
    return f"{first_word}.{last_name}@2026"

# Read and process
users = []
with open(input_csv, 'r', encoding='utf-8') as f:
    reader = csv.DictReader(f)
    for row in reader:
        name = row['Name']
        first_name, last_name, title = parse_name_with_title(name)
        email = generate_email(first_name, last_name)
        
        user = {
            'Title': title,
            'Firstname': first_name,
            'Lastname': last_name,
            'UserName': email,
            'Email': email,
            'Password': generate_password(first_name, last_name),
            'Role': 'Users',
            'Designation': row['Designation'],
            'DutyStation': row['MZODutyStation']
        }
        users.append(user)

# Write migration CSV
fieldnames = ['Title', 'Firstname', 'Lastname', 'UserName', 'Email', 'Password', 'Role', 'Designation', 'DutyStation']
with open(output_csv, 'w', newline='', encoding='utf-8') as f:
    writer = csv.DictWriter(f, fieldnames=fieldnames)
    writer.writeheader()
    writer.writerows(users)

print(f"✓ Migration file created: {output_csv}")
print(f"✓ Total users: {len(users)}")
print(f"\nColumn mapping:")
print(f"  - Title ← Extracted (Mr., Ms., Dr., etc.)")
print(f"  - Firstname ← First name(s) including middle names")
print(f"  - Lastname ← Last name only")
print(f"  - UserName ← Email address")
print(f"  - Email ← firstname.lastname@mlhud.go.ug")
print(f"  - Password ← FirstName.LastName@2026")
print(f"  - Role ← Default 'Users'")
print(f"\nSample entries:")
for user in users[:5]:
    print(f"  {user['Title']:5} {user['Firstname']:20} {user['Lastname']:15} | {user['Email']}")


