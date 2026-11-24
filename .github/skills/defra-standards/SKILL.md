---
name: defra-standards
description: Defra software development standards for Node.js and .NET government services. Use when writing, reviewing, or refactoring code in a Defra digital service repository. Activates for Hapi framework projects, CDP platform services, and .NET ASP.NET Core Minimal API services.
license: OGL-UK-3.0
metadata:
  author: defra-digital
  version: "1.0"
---

# Defra Standards

Apply these standards when generating or reviewing code in a Defra digital service.

## Runtime and language

### Node.js
- Use Node.js Active LTS version only
- Use vanilla JavaScript — do not use TypeScript without an approved exception
- Use ES modules (`import`/`export`) by default; CommonJS (`require`) only where required (e.g. Jest config)
- Use `const` by default, `let` only when reassignment is required, never `var`
- Use `async/await` — do not use callbacks or raw `.then()` chains

### .NET
- Use .NET Active LTS version only
- Use C# latest stable with nullable reference types enabled
- Use `record` types for immutable data
- .NET is for backend services and APIs only — do not use Razor Pages, Blazor, MVC views, or any other .NET UI framework. Frontends must be built with Node.js + Hapi + GOV.UK Frontend (Nunjucks). See [Defra .NET Standards](https://defra.github.io/software-development-standards/standards/net_standards/).

## Approved and discouraged packages

### Node.js — approved

| Package | Purpose |
|---------|---------|
| `@hapi/hapi` | HTTP server |
| `@hapi/boom` | HTTP error responses |
| `@hapi/crumb` | CSRF protection |
| `@hapi/blankie` | Content Security Policy |
| `@hapi/inert` | Static file serving |
| `@hapi/vision` | Template rendering (Nunjucks) |
| `@hapi/yar` | Session management |
| `joi` | Schema validation (standalone) |
| `nunjucks` | Server-side templating |
| `winston` | Structured JSON logging |
| `convict` | Configuration with validation |

### Node.js — discouraged

- `express`, `fastify`, `koa` — use Hapi
- `@hapi/joi` — deprecated, use standalone `joi`
- `mongoose` — use the native MongoDB driver
- `moment` — use native `Intl.DateTimeFormat` or `date-fns`
- `lodash` — use native array/object methods
- `request` or `axios` — use native `fetch` or `undici`
- `jquery` — not needed with progressive enhancement
- `typescript` — requires approved exception

## CDP platform conventions

If the service runs on the Defra Core Delivery Platform (CDP):

- Use `convict` with `convict-format-with-validator` for all configuration — never access `process.env` directly in application code
- Use `~` as an absolute import alias for project files
- Use `request.logger` for logging inside route handlers — do not use `console.log`
- Use `@defra/hapi-secure-context` for TLS certificate management
- Use `@defra/cdp-metrics` for Prometheus metrics
- Prefix all custom CSS classes with `app-` using BEM — e.g. `.app-heading`
- Add `data-testid` attributes to all key UI elements

## Containerisation

- Use Docker with Defra base images:
  - `defradigital/node` (production)
  - `defradigital/node-development` (development/CI)
  - `defradigital/dotnetcore` (production .NET)
  - `defradigital/dotnetcore-development` (development/CI .NET)
- Do not use the generic `node:lts` or `mcr.microsoft.com/dotnet` images directly

## Architecture

Keep controllers thin: validate input → call a service → render a view. Business logic belongs in service modules, not route handlers.

```
Route handler → validates input (joi) → calls service → renders view or returns response
```

## Naming conventions

- **Directories and template files**: `kebab-case` (e.g. `my-component.njk`)
- **JavaScript files**: `camelCase` (e.g. `applicationService.js`)
- **Routes (URL paths)**: lowercase with hyphens (e.g. `/submit-application`)
- **Environment variables**: `UPPER_SNAKE_CASE` (e.g. `DATABASE_HOST`)
- **C# classes**: `PascalCase`
- **C# private fields**: `_camelCase`

## Version control

- Main branch is always shippable
- Branch naming: `<type>/<brief-description>` (types: `feature/`, `fix/`, `docs/`, `refactor/`, `test/`, `chore/`)
- Commit messages: conventional format (`feat:`, `fix:`, `docs:`, `test:`, `refactor:`, `chore:`)
- Squash and merge PRs

## Quality gates

- Linter passes (`npx neostandard`) — do not extend or modify the ruleset
- All tests pass (`npm test`)
- Unit test coverage ≥90% — must not decrease from project or SonarCloud baseline
- SonarCloud quality gate passes (SonarWay profile) — no new bugs, vulnerabilities, or code smells
