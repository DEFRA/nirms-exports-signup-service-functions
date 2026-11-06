---
applyTo: "**/*.cs,**/*.csproj"
---

# C# Backend

## Language and runtime

- Use the latest LTS version of .NET (currently .NET 10)
- Use modern C# features — pattern matching, records for immutable data, nullable reference types
- Enable nullable reference types (`<Nullable>enable</Nullable>` in csproj)
- Enable implicit usings (`<ImplicitUsings>enable</ImplicitUsings>` in csproj)
- Use `async`/`await` for all asynchronous operations — do not use `.Result` or `.Wait()`
- Follow Microsoft's official C# coding conventions for naming, formatting, and style
- Prefer `var` for local variables when the type is obvious from the right-hand side
- Use containerisation with Docker

## Framework

- Use ASP.NET Core Minimal APIs for new services — do not use MVC controllers, Razor Pages, Blazor, or any other .NET UI framework. Per [Defra .NET Standards](https://defra.github.io/software-development-standards/standards/net_standards/){:target="_blank"} (opens in new tab), .NET is for backend services only; frontends must be built with Node.js + Hapi + GOV.UK Frontend (Nunjucks).
- Register endpoints using `MapGet`, `MapPost`, `MapPut`, `MapDelete`
- Use the built-in DI container; do not develop frontends in .NET

### Endpoint structure

Organise endpoints as static extension methods grouped by domain area:

```csharp
public static class ApplicationEndpoints
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/applications/{id}", async (
            string id,
            IApplicationService applicationService) =>
        {
            var application = await applicationService.GetByIdAsync(id);
            return application is not null
                ? Results.Ok(application)
                : Results.NotFound();
        });

        return app;
    }
}
```

## Architecture

- Follow SOLID principles
- Keep endpoint handlers thin — validate input, call a service, return a result
- Use records for DTOs and value objects
- Use the built-in DI container; register all services at startup
- Do not develop frontend applications in .NET — use Node.js with Nunjucks

```
Config/           # Configuration
Endpoints/        # Minimal API endpoint registrations
Models/           # Domain models and DTOs (prefer records)
Services/         # Business logic
Validators/       # FluentValidation validators
Utils/            # Shared utilities
```

## Validation

- Use FluentValidation for request validation
- Register validators in the DI container
- Return structured error responses with field-level detail
- Validate all user input before processing

✅ Validator example:
```csharp
public class CreateApplicationValidator : AbstractValidator<CreateApplicationRequest>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Reference)
            .NotEmpty()
            .Matches(@"^[A-Z]{2}\d{6}$")
            .WithMessage("Reference must be two letters followed by six digits");
    }
}
```

## Error handling

- Use `Results.Problem()` for structured error responses
- Return appropriate HTTP status codes (400, 404, 500)
- Return useful error messages without leaking internals (no stack traces, no connection strings)
- Log the full exception server-side for debugging
- Use global exception handling middleware for unhandled exceptions

## Logging

- Use Serilog in Elastic Common Schema (ECS) format, configured via `appsettings.json`
- Log levels: `Fatal`, `Error`, `Warning`, `Information`, `Debug`, `Verbose`
- Never log PII: no names, addresses, emails, phone numbers, NI numbers, bank details, API keys, or tokens
- Include correlation IDs for request tracing
- Production: `Error` and above. Development: `Debug`

## Security

- Follow OWASP and Microsoft Secure Coding Guidelines
- Validate all input using FluentValidation
- Use parameterised queries — never concatenate user input
- Never commit secrets — use environment variables or a secrets manager
- Set security headers: `X-Content-Type-Options: nosniff`, `Referrer-Policy: no-referrer`, HSTS
- Do not use `dynamic` types or reflection on user-supplied data

## Database access

- Use MongoDB.Driver for MongoDB (CDP platform default)
- For SQL, use Entity Framework Core or Dapper with parameterised queries
- Set connection timeouts and configure connection pooling
- Use repository or service patterns — do not query directly from endpoints

## Environment and configuration

- Load configuration from environment variables or `appsettings.json`
- Never hard-code secrets or connection strings
- Use `IOptions<T>` for typed configuration with environment-specific overrides
- Validate required configuration at startup — fail fast if values are missing

## Health checks

- Expose `/health` returning `200 OK` when healthy (CDP requirement)
- Include lightweight checks for database connectivity and critical dependencies

## Testing

- Use xUnit v3, FluentAssertions, and NSubstitute
- Use `Microsoft.AspNetCore.Mvc.Testing` for integration tests
- 90% minimum coverage — do not decrease existing baseline
- Test behaviour, not implementation; mock external dependencies
- Use `[ExcludeFromCodeCoverage]` only on bootstrapping code, never business logic

```csharp
public class ApplicationServiceTests
{
    private readonly IApplicationRepository _repository = Substitute.For<IApplicationRepository>();
    private readonly ApplicationService _sut;

    public ApplicationServiceTests() => _sut = new ApplicationService(_repository);

    [Fact]
    public async Task GetByIdAsync_WhenApplicationExists_ReturnsApplication()
    {
        var expected = new Application { Id = "123", Name = "Test" };
        _repository.GetByIdAsync("123").Returns(expected);
        var result = await _sut.GetByIdAsync("123");
        result.Should().BeEquivalentTo(expected);
    }
}
```

## Code style

- 4-space indentation, UTF-8, Allman brace style
- Private fields: `_camelCase`, constants: `PascalCase`
- Use predefined types (`int`, `string`) over framework types (`Int32`, `String`)
- Explicit accessibility modifiers on non-interface members

## Containers

- Use Defra base images: `defradigital/dotnetcore` (production), `defradigital/dotnetcore-development` (dev)
- Linux containers only, run as non-root, use multi-stage builds
- Tag images with semver; do not store secrets in images
- Use Docker Compose for local development

## Dependencies

- Use NuGet; only packages with OSI-approved licences compatible with OGL
- Run security scanning and resolve vulnerabilities before merging
- Configure Dependabot for automated updates

## References

- [Defra .NET standards](https://github.com/DEFRA/software-development-standards/blob/main/docs/standards/net_standards.md)
- [Defra C# coding standards](https://github.com/DEFRA/software-development-standards/blob/main/docs/standards/csharp_coding_standards.md)
- [CDP .NET backend template](https://github.com/DEFRA/cdp-dotnet-backend-template)
- [Microsoft C# conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [OWASP Secure Coding Practices](https://owasp.org/www-project-secure-coding-practices-quick-reference-guide/)
