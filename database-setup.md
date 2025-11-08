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

### Clients (12)
1. **Tech Innovators Corp**
   - Email: contact@techinnovators.com
   - Phone: +1-555-TECH-001
   - Address: 456 Innovation Drive, Silicon Valley, CA 94000

2. **Global Marketing Agency**
   - Email: info@globalmarketing.com
   - Phone: +1-555-MARKET-02
   - Address: 789 Marketing Boulevard, New York, NY 10001

3. **Creative Vision Studios**
   - Email: hello@creativevision.com
   - Phone: +1-555-CREATIVE
   - Address: 1010 Inspiration Lane, Los Angeles, CA 90012

4. **Northwind Logistics**
   - Email: ops@northwindlogistics.com
   - Phone: +1-555-NORTH-01
   - Address: 820 Freight Avenue, Chicago, IL 60654

5. **BrightPath Consulting**
   - Email: team@brightpathco.com
   - Phone: +1-555-BRIGHT-5
   - Address: 550 Strategy Street, Austin, TX 78701

6. **Summit Financial Group**
   - Email: info@summitfg.com
   - Phone: +1-555-SUMMIT-6
   - Address: 300 Peaks Plaza, Denver, CO 80202

7. **GreenEarth Initiatives**
   - Email: contact@greenearth.org
   - Phone: +1-555-GREEN-07
   - Address: 88 Sustainability Way, Portland, OR 97205

8. **Apex Healthcare Solutions**
   - Email: support@apexhealth.com
   - Phone: +1-555-APEX-CARE
   - Address: 212 Wellness Parkway, Boston, MA 02110

9. **BlueWave Analytics**
   - Email: partners@bluewaveanalytics.com
   - Phone: +1-555-WAVE-009
   - Address: 77 Insight Terrace, Seattle, WA 98104

10. **UrbanEdge Architects**
    - Email: studio@urbanedgearch.com
    - Phone: +1-555-URBAN-10
    - Address: 640 Skyline Boulevard, Miami, FL 33130

11. **Silverline Retailers**
    - Email: hq@silverlineretail.com
    - Phone: +1-555-SILVER-1
    - Address: 400 Commerce Court, Minneapolis, MN 55401

12. **NextGen Manufacturing**
    - Email: projects@nextgenmfg.com
    - Phone: +1-555-NEXTGEN
    - Address: 950 Innovation Parkway, Detroit, MI 48226

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
