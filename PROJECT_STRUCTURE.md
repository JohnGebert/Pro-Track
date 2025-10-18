# Pro-Track Project Structure

## High-Level Directory Structure

```
ProTrack/
├── Controllers/                 # MVC Controllers
│   ├── HomeController.cs
│   ├── ClientController.cs
│   ├── ProjectController.cs
│   ├── TimeEntryController.cs
│   ├── InvoiceController.cs
│   └── AccountController.cs
├── Views/                       # MVC Views
│   ├── Shared/                 # Shared layouts and partials
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── _LoginPartial.cshtml
│   ├── Home/                   # Home controller views
│   ├── Client/                 # Client management views
│   ├── Project/                # Project management views
│   ├── TimeEntry/              # Time tracking views
│   ├── Invoice/                # Invoice management views
│   └── Account/                # Authentication views
├── Models/                     # Data Models
│   ├── BaseModel.cs           # Base class with common properties
│   ├── ApplicationUser.cs     # Extended IdentityUser
│   ├── Client.cs              # Client model
│   ├── Project.cs             # Project model
│   ├── TimeEntry.cs           # Time entry model
│   ├── Invoice.cs             # Invoice model
│   └── ProjectStatus.cs       # Project status enum
├── Data/                      # Data Access Layer
│   ├── ApplicationDbContext.cs # EF Core DbContext
│   └── Migrations/            # EF Core migrations
├── Services/                  # Business Logic Services
│   ├── ITimeEntryService.cs
│   ├── TimeEntryService.cs
│   ├── IInvoiceService.cs
│   ├── InvoiceService.cs
│   ├── IProjectService.cs
│   └── ProjectService.cs
├── ViewModels/                # View Models for UI
│   ├── ClientViewModel.cs
│   ├── ProjectViewModel.cs
│   ├── TimeEntryViewModel.cs
│   └── InvoiceViewModel.cs
├── wwwroot/                   # Static files
│   ├── css/
│   ├── js/
│   ├── images/
│   └── lib/
├── Areas/                     # Areas for feature organization
│   └── Identity/              # Identity management area
├── Program.cs                 # Application entry point
├── appsettings.json          # Configuration
├── appsettings.Development.json
└── ProTrack.csproj           # Project file
```

## Key Features of the Structure

### 1. **Models Layer**
- **BaseModel.cs**: Provides common properties (Id, UserId) for all entities
- **ApplicationUser.cs**: Extends IdentityUser with custom properties
- **Entity Models**: Client, Project, TimeEntry, Invoice with proper relationships
- **Enums**: ProjectStatus for type safety

### 2. **Data Layer**
- **ApplicationDbContext**: Inherits from IdentityDbContext for user management
- Proper entity configurations with relationships
- Data annotations for validation and display names
- Computed properties for business logic

### 3. **Controllers Layer**
- Separate controllers for each main entity
- RESTful action methods (Index, Create, Edit, Delete, Details)
- Authentication and authorization

### 4. **Views Layer**
- Organized by controller
- Shared layouts and partials
- Responsive design with Bootstrap

### 5. **Services Layer**
- Business logic separation
- Interface-based design for testability
- Dependency injection ready

### 6. **ViewModels Layer**
- Clean separation between domain models and UI models
- Data transfer objects for complex views

## Database Relationships

```
ApplicationUser (1) ──── (*) Client
ApplicationUser (1) ──── (*) Project
ApplicationUser (1) ──── (*) TimeEntry
ApplicationUser (1) ──── (*) Invoice
Client (1) ──── (*) Project
Client (1) ──── (*) Invoice
Project (1) ──── (*) TimeEntry
Invoice (*) ──── (*) TimeEntry (Many-to-Many)
```

## Key Design Patterns

1. **Repository Pattern**: Through Entity Framework DbContext
2. **Dependency Injection**: For services and DbContext
3. **Model-View-Controller**: Standard ASP.NET Core MVC
4. **Identity Framework**: For authentication and authorization
5. **Data Annotations**: For validation and display metadata
