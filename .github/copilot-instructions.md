# Copilot Instructions for the ReMoS Status-Update Functions

Use these instructions when working in `nirms-exports-signup-service-functions` or creating documentation about how the Functions app interacts with Service Bus, SQL Server, the API, and downstream approval processes.

## Application Role

This project is the Azure Functions application that processes inbound status feedback for the ReMoS sign-up service. It consumes Service Bus messages and updates the shared ReMoS SQL database through stored procedures.

It is one of three separate deployable applications in the SUS ReMoS estate:

- `nirms-exports-signup-service-ui`: user journey, page flow, validation, authentication-session handling, and calls to the API.
- `nirms-exports-signup-service-api`: HTTP API, SQL persistence, address lookup facade, and outbound Service Bus publishing.
- `nirms-exports-signup-service-functions`: inbound Service Bus status-feedback consumers that update the shared SQL database.

Document this application as asynchronous feedback processing. Despite names such as `Rejections` and `RejectTrader`, it handles approval, rejection, suspension, removal, and establishment enrichment outcomes.

## Responsibilities

The Functions app is responsible for:

- Consuming business-level status feedback from the `defra.trade.sus.rejections` Service Bus queue using the `RejectTrader` function.
- Consuming establishment-level status and inspection-location feedback from the `defra.trade.sus.establishment` Service Bus queue using the `EstablishmentFunction` function.
- Deserializing incoming JSON messages into function-domain DTOs.
- Mapping external status strings such as `Approved`, `Rejected`, `Suspended`, and inactive establishment state to internal numeric approval statuses.
- Executing stored procedures such as `sp_UpdateTradeParty` and `sp_UpdateEstablishmentStatus` against the ReMoS SQL database.
- Completing null messages, dead-lettering malformed JSON, and rethrowing processing or SQL failures so Service Bus retry/dead-letter policy can apply.

The Functions app is not responsible for:

- Exposing HTTP endpoints for the UI.
- Rendering or sequencing the registration journey.
- Creating the original trade-party or establishment records.
- Publishing outbound Trade Platform messages.
- Deciding approval outcomes; it applies outcomes received from downstream processes.

## Important Source Areas

- Function triggers: `Defra.Trade.ReMoS.AssuranceService.Rejections/Functions/`.
- Function startup and dependency registration: `Defra.Trade.ReMoS.AssuranceService.Rejections/Startup.cs` and `Infrastructure/`.
- DTOs: `Defra.Trade.ReMoS.AssuranceService.Rejections.Domain/DTOs/`.
- Status enums: `Defra.Trade.ReMoS.AssuranceService.Rejections.Domain/Enums/`.
- SQL execution context: `Defra.Trade.ReMoS.AssuranceService.Rejections.Domain/Contexts/`.
- Stored procedure scripts: `StoredProcedures/`.
- Service Bus and SQL settings: `appsettings.json`, `appsettings.Development.json`, and pipeline files.

## Interaction Model

Use this as the default interaction model unless source code proves a narrower case:

1. The UI submits registration or self-service changes through the API.
2. The API persists the change to SQL Server and can publish Service Bus messages for downstream Trade Platform processing.
3. Downstream processes decide or propagate business and establishment outcomes outside these repositories.
4. Downstream feedback messages arrive on Service Bus queues consumed by this Functions app.
5. `RejectTrader` maps business approval status and updates the trade party by Defra organisation ID.
6. `EstablishmentFunction` maps establishment approval/active status and updates the logistics location by ReMoS establishment scheme number.
7. Stored procedures update the shared SQL database.
8. The API later reads the changed records, and the UI displays the updated statuses.

## Message Handling Details

When documenting function behaviour, distinguish these outcomes:

- Valid body and successful SQL execution: the invocation succeeds and the Service Bus trigger completes the message.
- Null body or literal JSON `null`: the message is explicitly completed with no database change.
- Malformed JSON: the message is explicitly dead-lettered with the JSON exception as the reason.
- Valid JSON that fails during mapping, missing nested payload access, or SQL execution: the exception is rethrown and Service Bus retry/dead-letter policy applies.

Status matching is exact and case-sensitive in the reviewed implementation. Do not imply tolerant or case-insensitive matching unless code changes prove it.

## Domain Language

- `TraderUpdateDTO`: top-level business status feedback envelope.
- `TraderPartyUpdateDTO`: nested business status payload containing status and Defra organisation ID.
- `EstablishmentUpdateDTO`: top-level establishment status feedback envelope.
- `EstablishmentStatusUpdateDTO`: nested establishment payload containing inspection location ID, ReMoS establishment ID, approval status, and active status.
- `TradeParty`: the business registration aggregate persisted by the API and updated by this function by organisation ID.
- `LogisticsLocation` or establishment: a place of dispatch or destination persisted by the API and updated by this function by ReMoS establishment scheme number.
- `InspectionLocationId`: downstream inspection-system identity stored on an establishment when supplied.

## Documentation Guidance

- Write in UK English.
- Distinguish queue names, function names, DTO names, stored procedures, and database tables.
- Explain that this app updates shared database state rather than calling the API.
- Do not describe `RejectTrader` as rejection-only; include approval and suspension handling.
- Mention where deployed behaviour may depend on which stored procedure script has been applied.
- When uncertain, cite the owning function, DTO, enum, stored procedure, or configuration setting to ground the explanation.
