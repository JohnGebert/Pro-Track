# Pro-Track Database Setup Instructions

## Prerequisites
Ensure you have the following installed:
- .NET 8 SDK
- Entity Framework Core tools
- SQL Server LocalDB (or SQL Server)

## Database Setup Commands

### 1. Install Entity Framework Tools (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

### 2. Create Initial Migration
```bash
dotnet ef migrations add InitialCreate
```

### 3. Create and Update Database
```bash
dotnet ef database update
```

## What the Seeding Process Includes

The database will be automatically populated with the following demonstration data:

### Demo User
- **ID**: `demo-user-123`
- **Email**: `demo@protrack.com`
- **Name**: Demo User
- **Company**: Demo Company

### Clients (2)
1. **Tech Innovators Corp**
   - Email: contact@techinnovators.com
   - Phone: +1-555-TECH-001
   - Address: 456 Innovation Drive, Silicon Valley, CA 94000

2. **Global Marketing Agency**
   - Email: info@globalmarketing.com
   - Phone: +1-555-MARKET-02
   - Address: 789 Marketing Boulevard, New York, NY 10001

### Projects (2)
1. **Q4 E-commerce Platform Relaunch** (Tech Innovators Corp)
   - Hourly Rate: $75.00
   - Status: Active
   - Description: Complete redesign and relaunch of e-commerce platform

2. **2026 Brand Strategy Documentation** (Global Marketing Agency)
   - Hourly Rate: $60.00
   - Status: Active
   - Description: Comprehensive brand strategy documentation

## Running the Application

After database setup, run the application:
```bash
dotnet run
```

## Important Notes

⚠️ **Security Notice**: The seeded user (`demo-user-123`) is created without a password hash for demonstration purposes only. In a production environment, you would need to properly hash passwords and handle user authentication through ASP.NET Identity's registration process.

⚠️ **Development Only**: This seeding data is intended for development and testing purposes. Remove or modify the seeding logic before deploying to production.

## Troubleshooting

If you encounter issues:

1. **Connection String**: Ensure your `appsettings.json` has the correct connection string for SQL Server LocalDB
2. **EF Tools**: Make sure Entity Framework tools are installed globally
3. **Database Permissions**: Ensure your user has permissions to create databases
4. **LocalDB**: Verify SQL Server LocalDB is installed and running

## Sample Connection String (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProTrackDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```
