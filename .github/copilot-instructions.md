# Copilot instructions

## Project Context
- Database Sampler is a sample application demonstrating how to use SQL Server, Postgres, Cosmos DB and Redis.
- It runs with local services and Docker-hosted databases.

## Repository overview
- Solution contains 4 projects:
  - `DatabaseSampler` (ASP.NET Core web app)
  - `DatabaseSampler.Application` (application/library code)
  - `DatabaseSampler.Application.Tests` (xUnit tests)
  - `DatabaseSampler.Functions` (Azure Functions)
- Shared build settings live in `Directory.Build.props`.
- Central package management is enabled via `Directory.Packages.props`.

## Coding conventions
- Always follow .editorconfig and CONTRIBUTING.md for formatting, naming, and style.
- Use existing patterns and naming conventions.
- Prefer minimal, targeted changes.
- Keep code style consistent with the file (indentation, usings, etc.).
- Avoid adding comments unless necessary or the file already uses them.
- Prefer async/await for I/O and database operations.
- Use dependency injection for services and configuration.
- Use clear, descriptive names for classes, methods, and variables.
- Write code that is testable and maintainable.
- Use C# 14 features where appropriate.
- Use collection expression for array where possible.
- Use nullable reference types and implicit usings (both enabled).
- Follow SonarAnalyzer rules (code analysis warnings are treated as errors).
- Use file-scoped namespaces.
- Don't specify package versions in .csproj files (use central package management).
- Use Primary Constructors. **DO NOT** replace primary constructors when refactoring or adding code.
- **DO NOT** add using statements when they are not needed, or when the project .csproj file has a <Using include="" /> for that namespace.

## Project-Specific Practices
- For Azure Functions, follow the function signature and binding conventions.
- For configuration, use appsettings and secrets. **DO NOT** write secrets into files in the solution directories.

## .NET / project guidelines
- Projects target `.NET 10` (inherited from `Directory.Build.props` unless a project overrides it).
- Prefer modern C# features compatible with the repo (C# 14).

## Tests
- **Test Framework**: xUnit.v3 v3.2.2 or above
- Assertion library: **Shouldly** (do not introduce FluentAssertions in new/updated tests).
- Test doubles library: **NSubstitute**, not Moq.
- **Always use Shouldly syntax**: `result.ShouldBe(expected);` instead of `Assert.Equal(expected, result);`
- **Always use NSubstitute** syntax: `var mock = Substitute.For<IService>();` for test mocks.
- **Test naming**: `MethodName_Scenario_ExpectedBehavior` (e.g., `SaveDocument_WithNullInput_ShouldThrowArgumentNullException`)
- Write tests that are isolated, repeatable, and fast
- Mock external dependencies (databases, HTTP clients, file system)
- Use Aspire.Hosting.Testing for container-based integration tests
- Include both positive and negative test cases
- Test edge cases and boundary conditions

## Packages
- Prefer adding/updating package versions in `Directory.Packages.props`.
- Avoid specifying `Version` on `PackageReference` items unless central package management is explicitly disabled.

## Azure Functions
- Keep Azure Functions settings in the Functions project; avoid changing target frameworks there unless necessary for SDK compatibility.

## Build/run
- After changes, run a solution build and fix any errors introduced by the change.

## Documentation
- DO NOT add XML comments to public APIs.
- Update README.md and CONTRIBUTING.md (if present) when introducing new patterns or requirements.
