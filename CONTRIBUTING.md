# Contributing to ProTrack

Thank you for your interest in contributing to ProTrack! This document provides guidelines and instructions for contributing to the project.

## 🎯 How to Contribute

### Reporting Bugs

Before creating bug reports, please check the issue list as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible:

- **Clear and descriptive title**
- **Steps to reproduce the issue**
- **Expected behavior**
- **Actual behavior**
- **Screenshots** (if applicable)
- **Environment details** (OS, .NET version, browser, etc.)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, please include:

- **Clear and descriptive title**
- **Detailed description of the proposed enhancement**
- **Use case and motivation**
- **Possible implementation approach** (if you have ideas)

## 🔧 Development Setup

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server LocalDB or SQL Server Express
- Git
- Visual Studio 2022 or Visual Studio Code

### Getting Started

1. **Fork the repository**
   ```bash
   # Click the Fork button on GitHub
   ```

2. **Clone your fork**
   ```bash
   git clone https://github.com/YOUR_USERNAME/Pro-Track.git
   cd Pro-Track
   ```

3. **Add the original repository as upstream**
   ```bash
   git remote add upstream https://github.com/JohnGebert/Pro-Track.git
   ```

4. **Restore dependencies**
   ```bash
   dotnet restore
   ```

5. **Create and seed the database**
   ```bash
   dotnet ef database update
   ```

6. **Run the application**
   ```bash
   dotnet run
   ```

## 📝 Development Workflow

### Creating a Branch

1. **Update your main branch**
   ```bash
   git checkout main
   git pull upstream main
   ```

2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

   Branch naming conventions:
   - `feature/` - New features
   - `bugfix/` - Bug fixes
   - `hotfix/` - Critical production fixes
   - `docs/` - Documentation updates
   - `refactor/` - Code refactoring

### Making Changes

1. **Write clean, readable code**
   - Follow C# coding conventions
   - Add XML documentation comments for public APIs
   - Keep methods focused and single-purpose

2. **Test your changes**
   - Test the functionality manually
   - Ensure existing features still work
   - Check for any console errors

3. **Commit your changes**
   ```bash
   git add .
   git commit -m "Add: Brief description of your changes"
   ```

   Commit message format:
   - `Add:` - New features
   - `Fix:` - Bug fixes
   - `Update:` - Updates to existing features
   - `Refactor:` - Code refactoring
   - `Docs:` - Documentation changes

### Submitting Changes

1. **Push your branch to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

2. **Create a Pull Request**
   - Go to the ProTrack repository on GitHub
   - Click "New Pull Request"
   - Select your branch
   - Fill out the PR template
   - Submit the PR

## 📋 Pull Request Guidelines

### Before Submitting

- [ ] Code follows the project's style guidelines
- [ ] All new code has been tested
- [ ] Documentation has been updated (if applicable)
- [ ] No new warnings or errors
- [ ] Commit messages are clear and descriptive

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
Describe the tests you ran and their results

## Screenshots (if applicable)
Add screenshots here

## Checklist
- [ ] My code follows the style guidelines
- [ ] I have performed a self-review
- [ ] I have commented my code where necessary
- [ ] I have updated the documentation
- [ ] My changes generate no new warnings
```

## 🎨 Code Style Guidelines

### C# Coding Conventions

- Use PascalCase for public properties and methods
- Use camelCase for private fields and local variables
- Use meaningful variable and method names
- Keep methods under 50 lines when possible
- Add XML documentation for public APIs

### Example

```csharp
/// <summary>
/// Retrieves all active clients for the current user
/// </summary>
/// <returns>List of active clients</returns>
public async Task<List<Client>> GetActiveClientsAsync()
{
    var userId = _userManager.GetUserId(User);
    return await _context.Clients
        .Where(c => c.UserId == userId && c.IsActive)
        .OrderBy(c => c.Name)
        .ToListAsync();
}
```

### Razor Views

- Use consistent indentation (4 spaces)
- Keep views focused and avoid complex logic
- Use partial views for reusable components
- Add comments for complex sections

## 🧪 Testing Guidelines

### Manual Testing Checklist

- [ ] Test the new feature with various inputs
- [ ] Test edge cases and error scenarios
- [ ] Verify the UI is responsive
- [ ] Check browser console for errors
- [ ] Test on different browsers (Chrome, Firefox, Edge)
- [ ] Verify database changes are correct

## 📚 Documentation

### Code Documentation

- Add XML comments to all public methods
- Document complex algorithms or business logic
- Update README.md if adding new features
- Update database schema documentation if changing models

### Example

```csharp
/// <summary>
/// Calculates the total billable hours for a project
/// </summary>
/// <param name="projectId">The ID of the project</param>
/// <returns>Total billable hours as a decimal</returns>
public async Task<decimal> CalculateBillableHoursAsync(int projectId)
{
    // Implementation
}
```

## 🐛 Bug Fix Guidelines

1. **Identify the bug**
   - Reproduce the issue
   - Identify the root cause
   - Check for related issues

2. **Fix the bug**
   - Write the minimal code to fix the issue
   - Test the fix thoroughly
   - Ensure no regressions

3. **Document the fix**
   - Add comments explaining the fix
   - Update tests if applicable
   - Update documentation if needed

## 🚀 Feature Development

### Planning

1. **Discuss the feature**
   - Open an issue to discuss the feature
   - Get feedback from maintainers
   - Plan the implementation

2. **Design the feature**
   - Consider the user experience
   - Plan the database changes (if needed)
   - Design the UI/UX

3. **Implement the feature**
   - Follow the development workflow
   - Write clean, maintainable code
   - Add appropriate documentation

### Database Changes

When adding new features that require database changes:

1. Create a new migration
   ```bash
   dotnet ef migrations add FeatureName
   ```

2. Update the database
   ```bash
   dotnet ef database update
   ```

3. Update the database schema documentation in README.md

## 📞 Getting Help

If you need help:

1. Check the existing issues and discussions
2. Review the README.md and documentation
3. Open a new issue with the "question" label
4. Contact the maintainers

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License.

## 🙏 Thank You!

Thank you for taking the time to contribute to ProTrack! Your contributions make this project better for everyone.

---

**Questions?** Open an issue or contact the maintainers.

