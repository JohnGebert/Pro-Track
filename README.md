# ProTrack - Professional Time Tracking & Project Management System

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-green)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-orange)
![License](https://img.shields.io/badge/License-MIT-yellow)

**ProTrack** is a comprehensive web-based application designed for freelancers and small businesses to efficiently manage clients, projects, time entries, and invoicing. Built with ASP.NET Core MVC and Entity Framework Core, it provides a robust solution for tracking billable hours and generating professional invoices.

## 🌟 Features

### Phase 1 (Current Release)
- **Client Management**: Add, edit, and organize client information with contact details
- **Project Tracking**: Create and manage multiple projects per client with status tracking
- **Time Logging**: Record detailed time entries with descriptions and billable status
- **Invoice Generation**: Create professional invoices linked to clients and time entries
- **AI Assistants**: Generate polished time entry descriptions and professional invoice notes from quick prompts
- **User Authentication**: Secure login and registration with ASP.NET Core Identity
- **Dashboard**: Overview of clients, projects, time entries, and revenue statistics
- **Responsive Design**: Modern, mobile-friendly interface

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server LocalDB** (included with Visual Studio) or **SQL Server Express**
- **Git** for version control ([Download](https://git-scm.com/downloads))
- **Visual Studio 2022** (recommended) or **Visual Studio Code** with C# extension

## 🚀 Quick Start Guide

### Option 1: Clone from GitHub (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/JohnGebert/Pro-Track.git
   cd Pro-Track
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update database connection string** (if needed)
   - Open `appsettings.json`
   - Verify the connection string points to your SQL Server instance:
   ```json
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProTrackDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   ```

4. **Create and seed the database**
   ```bash
   dotnet ef database update
   ```
   This will create the database and populate it with demo data.

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the application**
   - Open your browser and navigate to: `http://localhost:5000`
   - Login with demo credentials:
     - **Email**: `demo@protrack.com`
     - **Password**: (you'll need to set this during first run)

### Option 2: Download ZIP

1. **Download the project**
   - Click the green "Code" button on GitHub
   - Select "Download ZIP"
   - Extract the ZIP file to your desired location

2. **Follow steps 2-6 from Option 1**

## 🔧 Installation for Development

### Visual Studio 2022

1. Open Visual Studio 2022
2. Select "Open a project or solution"
3. Navigate to the ProTrack folder and select `ProTrack.sln`
4. Press `F5` to build and run the application

### Visual Studio Code

1. Open Visual Studio Code
2. Open the ProTrack folder (File → Open Folder)
3. Install the C# extension if prompted
4. Open the integrated terminal (Ctrl + `)
5. Run `dotnet restore` then `dotnet run`

## 🗄️ Database Schema

### Entity Relationship Diagram

```
┌─────────────────┐
│ ApplicationUser │
│ (Identity)      │
└────────┬────────┘
         │ 1
         │
         │ *
    ┌────┴──────────────────────────────┐
    │                                    │
    │ *                                  │ *
┌───▼──────┐                       ┌────▼──────┐
│  Client  │                       │  Project  │
├──────────┤                       ├───────────┤
│ Id (PK)  │                       │ Id (PK)   │
│ UserId   │◄────┐                 │ UserId    │◄───┐
│ Name     │     │                 │ ClientId  │    │
│ Contact  │     │                 │ Title     │    │
│ Email    │     │                 │ Description│   │
│ Phone    │     │                 │ HourlyRate│    │
│ Address  │     │                 │ Status    │    │
│ Notes    │     │                 │ StartDate │    │
│ IsActive │     │                 │ EndDate   │    │
│ Created  │     │                 │ Created   │    │
└──────────┘     │                 │ Modified  │    │
                 │                 └──────┬────┘    │
                 │                        │ *       │
                 │                        │         │
                 │                        │         │
                 │                  ┌─────▼──────┐  │
                 │                  │ TimeEntry  │  │
                 │                  ├────────────┤  │
                 │                  │ Id (PK)    │  │
                 │                  │ UserId     │◄─┘
                 │                  │ ProjectId  │◄──┐
                 │                  │ Description│   │
                 │                  │ StartTime  │   │
                 │                  │ EndTime    │   │
                 │                  │ IsBilled   │   │
                 │                  │ Created    │   │
                 │                  │ Modified   │   │
                 │                  └────────────┘   │
                 │                                   │
                 │                                   │
                 │                            ┌──────▼──────┐
                 │                            │  Invoice    │
                 │                            ├─────────────┤
                 │                            │ Id (PK)     │
                 │                            │ UserId      │◄─┘
                 │                            │ ClientId    │◄──┘
                 │                            │ InvoiceNum  │
                 │                            │ InvoiceDate │
                 │                            │ TotalAmount │
                 │                            │ IsPaid      │
                 │                            │ Notes       │
                 │                            │ Created     │
                 │                            │ Modified    │
                 │                            └─────────────┘
                 │
                 │
    ┌────────────▼────────────┐
    │  InvoiceTimeEntries     │
    │  (Many-to-Many)         │
    ├─────────────────────────┤
    │ InvoiceId (FK)          │
    │ TimeEntriesId (FK)      │
    └─────────────────────────┘
```

### Database Tables

#### **AspNetUsers** (Identity)
- `Id` (PK, nvarchar(450))
- `UserName` (nvarchar(256))
- `NormalizedUserName` (nvarchar(256))
- `Email` (nvarchar(256))
- `NormalizedEmail` (nvarchar(256))
- `EmailConfirmed` (bit)
- `PasswordHash` (nvarchar(max))
- `SecurityStamp` (nvarchar(max))
- `ConcurrencyStamp` (nvarchar(max))
- `PhoneNumber` (nvarchar(max))
- `PhoneNumberConfirmed` (bit)
- `TwoFactorEnabled` (bit)
- `LockoutEnd` (datetimeoffset)
- `LockoutEnabled` (bit)
- `AccessFailedCount` (int)
- `FirstName` (nvarchar(100))
- `LastName` (nvarchar(100))
- `CompanyName` (nvarchar(200))
- `Address` (nvarchar(500))
- `CreatedDate` (datetime2)

#### **Clients**
- `Id` (PK, int, identity)
- `UserId` (FK, nvarchar(450))
- `Name` (nvarchar(200), required)
- `ContactEmail` (nvarchar(256), nullable)
- `PhoneNumber` (nvarchar(20), nullable)
- `Address` (nvarchar(500), nullable)
- `Notes` (nvarchar(1000), nullable)
- `CreatedDate` (datetime2, default: GETUTCDATE())
- `IsActive` (bit, default: true)

#### **Projects**
- `Id` (PK, int, identity)
- `UserId` (FK, nvarchar(450))
- `ClientId` (FK, int)
- `Title` (nvarchar(200), required)
- `Description` (nvarchar(2000), nullable)
- `HourlyRate` (decimal(18,2))
- `Status` (int, enum: Planning, Active, OnHold, Completed, Cancelled)
- `StartDate` (datetime2)
- `EndDate` (datetime2, nullable)
- `CreatedDate` (datetime2, default: GETUTCDATE())
- `LastModified` (datetime2, default: GETUTCDATE())

#### **TimeEntries**
- `Id` (PK, int, identity)
- `UserId` (FK, nvarchar(450))
- `ProjectId` (FK, int)
- `Description` (nvarchar(1000), required)
- `StartTime` (datetime2)
- `EndTime` (datetime2)
- `IsBilled` (bit, default: false)
- `CreatedDate` (datetime2, default: GETUTCDATE())
- `LastModified` (datetime2, default: GETUTCDATE())

#### **Invoices**
- `Id` (PK, int, identity)
- `UserId` (FK, nvarchar(450))
- `ClientId` (FK, int)
- `InvoiceNumber` (nvarchar(100), required)
- `InvoiceDate` (datetime2, default: GETUTCDATE())
- `TotalAmount` (decimal(18,2))
- `IsPaid` (bit, default: false)
- `Notes` (nvarchar(500), nullable)
- `CreatedDate` (datetime2, default: GETUTCDATE())
- `LastModified` (datetime2, default: GETUTCDATE())

#### **InvoiceTimeEntries** (Many-to-Many Junction Table)
- `InvoiceId` (FK, int)
- `TimeEntriesId` (FK, int)

## 📁 Project Structure

```
ProTrack/
├── Areas/
│   └── Identity/
│       └── Pages/
│           └── Account/          # Authentication pages
├── Controllers/
│   ├── ClientsController.cs      # Client CRUD operations
│   ├── HomeController.cs         # Dashboard
│   ├── InvoicesController.cs     # Invoice management
│   ├── ProjectsController.cs     # Project management
│   └── TimeEntriesController.cs  # Time entry management
├── Data/
│   └── ApplicationDbContext.cs   # EF Core context & configuration
├── Migrations/                   # Database migrations
├── Models/
│   ├── ApplicationUser.cs        # Extended Identity user
│   ├── BaseModel.cs              # Base entity class
│   ├── Client.cs                 # Client entity
│   ├── Invoice.cs                # Invoice entity
│   ├── Project.cs                # Project entity
│   ├── ProjectStatus.cs          # Project status enum
│   └── TimeEntry.cs              # Time entry entity
├── Views/
│   ├── Clients/                  # Client views
│   ├── Home/                     # Dashboard view
│   ├── Invoices/                 # Invoice views
│   ├── Projects/                 # Project views
│   ├── Shared/                   # Shared layouts & partials
│   └── TimeEntries/              # Time entry views
├── wwwroot/                      # Static files (CSS, JS, images)
├── appsettings.json              # Application configuration
├── Program.cs                    # Application entry point
└── ProTrack.csproj              # Project file
```

## 🔐 Default Demo Account

After running the initial migration, you can log in with:

- **Email**: `demo@protrack.com`
- **Password**: (Set during first registration or use ASP.NET Identity's default password reset)

> **Note**: The demo account is pre-populated with sample clients, projects, and time entries for demonstration purposes.

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server (LocalDB/Express)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Razor Views, Bootstrap 5
- **Language**: C# 12

## 📝 Configuration

### Connection String

The default connection string in `appsettings.json` uses SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProTrackDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

To use SQL Server Express or a remote server, update the connection string accordingly.

### Application Settings

Key settings can be modified in `appsettings.json`:

- **Password Requirements**: Configured in `Program.cs` via Identity options
- **Session Timeout**: Default ASP.NET Core settings
- **HTTPS Redirect**: Configured for production environments

### AI Assistants for Time Entries & Invoices

The time entry and invoice forms include optional AI helpers that turn short prompts into professional descriptions and notes. The app never ships with an API key—you must supply your own via environment-managed configuration before the helpers appear.

#### Quick Setup (per developer machine)

1. Ensure you are in the project directory, then enable the assistants with your key using [.NET Secret Manager](https://learn.microsoft.com/aspnet/core/security/app-secrets):
   ```bash
   dotnet user-secrets set "AiDescriptions:Enabled" "true"
   dotnet user-secrets set "AiDescriptions:ApiKey" "<your-api-key>"
   dotnet user-secrets set "AiDescriptions:Model" "gpt-4o-mini"
   ```
2. If you are using Azure OpenAI (or another vendor that changes the URL), add:
   ```bash
   dotnet user-secrets set "AiDescriptions:BaseUrl" "https://<resource>.openai.azure.com/"
   dotnet user-secrets set "AiDescriptions:Endpoint" "openai/deployments/<deployment>/chat/completions"
   dotnet user-secrets set "AiDescriptions:ApiVersion" "2024-02-15-preview"
   dotnet user-secrets set "AiDescriptions:AuthenticationScheme" "ApiKey"
   ```
3. Restart the application (`dotnet run`). Once the key is in place the AI widgets on the pages become visible automatically.

#### Alternative: Environment Variables

If you prefer, export variables instead of using Secret Manager:

```powershell
$env:AiDescriptions__Enabled = "true"
$env:AiDescriptions__ApiKey = "<your-api-key>"
$env:AiDescriptions__Model = "gpt-4o-mini"
dotnet run
```

> **Security Tip**: Never commit API keys to source control or hard-code them in `appsettings`. Use secrets or environment variables so each user can plug in their own credentials safely.

## 🚀 Deployment

### Publish to IIS

1. **Build the project**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Configure IIS**
   - Install ASP.NET Core Hosting Bundle
   - Create a new application pool (No Managed Code)
   - Create a new website pointing to the publish folder

3. **Update connection string** for production database

### Publish to Azure

1. Right-click project → Publish
2. Select Azure App Service
3. Follow the deployment wizard
4. Update connection string in Azure Configuration

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**John Gebert**
- GitHub: [@JohnGebert](https://github.com/JohnGebert)
- Project: [Pro-Track](https://github.com/JohnGebert/Pro-Track)

## 🙏 Acknowledgments

- Built with [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- UI components styled with [Bootstrap](https://getbootstrap.com/)
- Icons provided by [Font Awesome](https://fontawesome.com/)

## 📧 Support

For support, email support@protrack.com or open an issue on GitHub.

## 🗺️ Roadmap

### Phase 1 (Current) ✅
- [x] Client management
- [x] Project tracking
- [x] Time logging
- [x] Invoice generation
- [x] User authentication
- [x] Dashboard

### Phase 2 (Planned)
- [ ] Email notifications
- [ ] PDF export for invoices
- [ ] Advanced reporting
- [ ] Multi-currency support
- [ ] Expense tracking
- [ ] Client portal

### Phase 3 (Future)
- [ ] Mobile app (iOS/Android)
- [ ] API for third-party integrations
- [ ] Team collaboration features
- [ ] Advanced analytics
- [ ] Custom branding

---

**Version**: 1.0.0  
**Release Date**: January 2025  
**Phase**: Phase 1  
**Status**: ✅ Production Ready

