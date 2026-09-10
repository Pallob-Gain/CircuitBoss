# CircuitBoss — Project Outline

## 1. Project Summary

CircuitBoss is an installable Altium Designer extension that performs AI-assisted electrical schematic reviews. It adds a toolbar icon, project context-menu commands, a settings interface, and a dockable review-results panel inside Altium Designer.

For the initial release, each user supplies an OpenAI API key and CircuitBoss calls the OpenAI Responses API directly. A future release may replace direct API access with CircuitBoss accounts and a hosted backend without changing the Altium-facing workflow.

**Repository description:** AI-powered electrical design review extension for Altium Designer.

## 2. Product Goals

- Install and behave like a native Altium extension.
- Add a CircuitBoss button to the Altium toolbar.
- Add a `CircuitBoss` submenu to the project context menu.
- Review the active schematic sheet, selected sheets, or complete PCB project.
- Combine deterministic checks, Altium ERC/compiler results, and AI reasoning.
- Display findings in a dockable panel.
- Navigate from a finding to the relevant sheet, component, pin, or net.
- Keep the first release read-only; never modify a schematic automatically.
- Protect API credentials and disclose what project data will be transmitted.

## 3. Important Development Constraint

The target is a compiled, distributable Altium extension, not only a `.PrjScr` script. Before implementation, confirm access to the current Altium extension SDK, supported language/toolchain, packaging format, signing requirements, compatible Altium versions, and extension-store submission process with Altium.

A DelphiScript prototype may be used to investigate and validate Altium object-model access, but the production architecture must support native commands, toolbar integration, context menus, a dockable panel, installation, updating, and eventual store distribution.

Do not invent undocumented SDK interfaces. Isolate all Altium-specific APIs behind adapters and document the Altium version used for testing.

## 4. Initial User Experience

### 4.1 Installation

1. User installs CircuitBoss through Altium's supported extension mechanism.
2. Altium loads the extension on startup.
3. A CircuitBoss icon appears in the main toolbar.
4. A `CircuitBoss` submenu appears when the user right-clicks a PCB project.

### 4.2 First-run setup

1. User clicks the CircuitBoss toolbar icon.
2. CircuitBoss opens its dockable panel.
3. If no API key is configured, the panel opens Settings.
4. User enters an OpenAI API key and selects `Test Connection`.
5. CircuitBoss validates access with a minimal request.
6. The key is stored in Windows Credential Manager, never in the project or a plaintext file.

### 4.3 Project review

1. User right-clicks a PCB project.
2. User selects `CircuitBoss > Review Project`.
3. CircuitBoss compiles/validates the project where supported.
4. CircuitBoss extracts a normalized project snapshot.
5. User may preview the data that will be transmitted.
6. CircuitBoss runs local deterministic checks.
7. CircuitBoss sends bounded, structured review requests to OpenAI.
8. CircuitBoss validates, combines, and deduplicates responses.
9. Findings appear in the dockable panel.
10. Clicking a finding opens and zooms to the associated Al object.

## 5. Altium User Interface

### 5.1 Toolbar command

The toolbar icon opens or focuses the CircuitBoss panel. Its menu should provide:

- Review Current Sheet
- Review Selected Sheets
- Review Entire Project
- Open Review Panel
- Review History
- Settings

### 5.2 Project context menu

Add the following submenu when the selected node is a supported PCB project:

```text
CircuitBoss >
    Review Project
    Review Selected Sheets
    Run Quick Check
    Open Review Panel
    Export Latest Report
    Settings
```

Disable commands with a clear explanation when no suitable project or schematic is active.

### 5.3 Dockable review panel

The panel should contain:

- Project name and review profile
- Review status and progress
- Counts for Critical, Warning, Suggestion, and Information
- Search and severity/category filters
- Sortable findings table
- Detailed finding explanation and evidence
- Locate Object button
- Ignore/Acknowledge action
- Copy Finding action
- Export Report action
- Cancel Review action while processing

Suggested finding columns:

| Column | Meaning |
| --- | --- |
| Severity | Critical, Warning, Suggestion, or Information |
| ID | Stable CircuitBoss finding identifier |
| Object | Component, pin, or net reference |
| Sheet | Source schematic document |
| Category | Power, connectivity, interface, protection, etc. |
| Title | Short issue summary |
| Confidence | Model confidence indicator, not proof of correctness |

### 5.4 Settings

Initial settings:

- OpenAI API key
- Test Connection
- Model configuration (`Recommended` by default; avoid hard-coding an obsolete model)
- Review profile
- Include ERC/compiler messages
- Include component parameters
- Include connectivity
- Include schematic images, off by default
- Preview transmitted data, on by default
- Remove project/customer names, on by default
- Request timeout
- Maximum retry count
- Clear saved credentials

## 6. High-level Architecture

```text
Altium Designer
  -> CircuitBoss command and UI layer
  -> Altium project adapter
  -> normalized project snapshot
  -> deterministic rule engine
  -> review orchestrator
  -> OpenAI provider
  -> response validator and deduplicator
  -> findings repository
  -> dockable review panel and object navigation
```

### 6.1 Required modules

1. **Extension Host** — registration, startup/shutdown, commands, menus, toolbar, panel lifecycle.
2. **Altium Adapter** — reads projects and documents and navigates to objects.
3. **Snapshot Builder** — converts Altium objects into a versioned, provider-neutral graph.
4. **Rule Engine** — deterministic checks that do not require AI.
5. **Review Orchestrator** — batching, progress, cancellation, retry, aggregation, and review sessions.
6. **OpenAI Provider** — authentication, HTTPS requests, structured output, errors, and rate limits.
7. **Credential Store** — Windows Credential Manager integration.
8. **Findings Store** — active results, ignored findings, and optional local history.
9. **UI Layer** — settings dialog and dockable results panel.
10. **Reporting** — JSON and Markdown/HTML export.

## 7. Core Domain Interfaces

Use equivalent interfaces appropriate for the confirmed SDK/toolchain.

```csharp
public interface IAltiumProjectAdapter
{
    ProjectSnapshot CaptureActiveProject(CaptureOptions options);
    NavigationResult NavigateTo(ObjectReference target);
}

public interface IReviewProvider
{
    Task<ReviewBatchResult> ReviewAsync(
        ReviewBatch batch,
        ReviewOptions options,
        CancellationToken cancellationToken);
}

public interface ICredentialStore
{
    Task SaveAsync(string credentialName, string secret);
    Task<string?> ReadAsync(string credentialName);
    Task DeleteAsync(string credentialName);
}

public interface IRuleEngine
{
    IReadOnlyList<Finding> Evaluate(ProjectSnapshot snapshot);
}
```

Keep the API provider replaceable. Initial implementation: `OpenAiReviewProvider`. Future implementation: `CircuitBossCloudProvider`.

## 8. Normalized Project Snapshot

The snapshot must not expose raw SDK objects outside the Altium Adapter.

```json
{
  "schema_version": "1.0",
  "project": {
    "id": "local-generated-id",
    "name": "optional-or-redacted",
    "type": "pcb_project"
  },
  "documents": [],
  "components": [],
  "pins": [],
  "nets": [],
  "connections": [],
  "power_objects": [],
  "directives": [],
  "parameters": [],
  "erc_messages": []
}
```

### 8.1 Minimum component fields

- Stable snapshot ID
- Altium document path/reference
- Designator
- Comment/value
- Library reference
- Description
- Manufacturer and part number, when present
- Component parameters
- Sheet coordinates/bounding box

### 8.2 Minimum pin fields

- Component reference
- Pin number
- Pin name
- Electrical type
- Net reference
- Hidden/visible state
- Location

### 8.3 Minimum net fields

- Name
- Generated stable ID
- Connected component pins
- Power-net classification
- Label and port sources
- Cross-sheet relationships

### 8.4 Additional objects

- Power ports
- Net labels
- Sheet entries and ports
- No-connect directives
- Harness objects
- Differential-pair directives
- Parameter sets and design directives
- ERC/compiler messages

## 9. Review Request Strategy

- Never send raw Altium binary project files directly.
- Convert the project into the versioned normalized snapshot.
- Redact local filesystem paths and user/customer names.
- Run deterministic checks before AI review.
- Partition large projects into bounded review batches.
- Preserve cross-sheet/net context needed for each batch.
- Send component datasheet information only when explicitly available and permitted.
- Require schema-constrained JSON responses.
- Reject malformed results instead of displaying guessed fields.
- Deduplicate findings that originate from multiple batches.
- Treat model output as review assistance, not verified electrical truth.

## 10. Finding Schema

```json
{
  "review_id": "uuid",
  "summary": {
    "critical": 0,
    "warnings": 0,
    "suggestions": 0,
    "information": 0
  },
  "findings": [
    {
      "id": "CB-POWER-0001",
      "severity": "warning",
      "category": "power",
      "title": "Possible missing local decoupling",
      "explanation": "No local bypass capacitor was identified for the supply pin.",
      "recommendation": "Verify the device datasheet and capacitor placement.",
      "evidence": [
        "U1 pin 5 connects to +3V3",
        "No capacitor was associated with U1 in the supplied graph"
      ],
      "objects": [
        {
          "document_id": "doc-1",
          "object_type": "component",
          "object_id": "component-42"
        }
      ],
      "confidence": 0.78,
      "requires_datasheet_verification": true
    }
  ]
}
```

The actual JSON Schema must use enums, required fields, bounds, and `additionalProperties: false` where appropriate.

## 11. Deterministic Checks for the MVP

Implement a small local ruleset before relying on AI:

- Unconnected input or power pins, subject to directives
- Single-pin nets
- Output-to-output connections
- Missing no-connect directive on intentionally unused pins
- Multiple conflicting net names
- Common I2C nets with no identifiable pull-up
- Candidate CAN/RS-485 buses without identifiable termination, reported as verification suggestions
- ERC/compiler messages normalized into CircuitBoss findings

Every deterministic rule needs unit tests and must expose its evidence.

## 12. OpenAI Integration Requirements

- Use the OpenAI Responses API.
- Use HTTPS and the user's supplied API key.
- Use structured output with a strict JSON Schema.
- Keep model selection configurable.
- Provide a concise system instruction defining the reviewer role and limitations.
- Send only data required for the selected review.
- Implement timeout, cancellation, retry with backoff, rate-limit handling, and readable error messages.
- Do not log authorization headers, full API keys, or confidential request bodies by default.
- Never claim a finding is confirmed solely because the model produced it.

## 13. Credential and Data Security

- Store the API key in Windows Credential Manager.
- Never save it in an Altium project, repository, environment file distributed to users, plugin binary, registry plaintext, crash report, or application log.
- Mask the key in the UI.
- Provide Test Connection and Clear Credential commands.
- Do not include the key in exception messages.
- Default `Preview data before sending` to enabled.
- Provide a visible summary of transmitted object counts.
- Redact absolute paths, usernames, workspace URLs, and customer names by default.
- Require explicit opt-in before transmitting schematic images or datasheets.
- Add a clear notice that API usage is billed separately by OpenAI.

## 14. Error Handling

Present actionable messages for:

- No project open
- Unsupported project type
- Project compilation failure
- Missing API key
- Invalid API key
- Insufficient API quota/billing configuration
- Network unavailable
- Rate limited
- Request timed out
- Review cancelled
- Response failed schema validation
- Altium object no longer exists when navigating

An AI/API failure must not crash or destabilize Altium Designer.

## 15. Logging

- Use structured local logs with levels: Error, Warning, Information, Debug.
- Disable sensitive payload logging by default.
- Never log credentials.
- Include extension version, Altium version, review ID, duration, batch count, and high-level failure category.
- Provide a user command to open the diagnostics folder.

## 16. Suggested Repository Structure

The exact source structure depends on the confirmed Altium SDK/toolchain.

```text
CircuitBoss/
├── README.md
├── LICENSE
├── CHANGELOG.md
├── SECURITY.md
├── CONTRIBUTING.md
├── docs/
│   ├── architecture.md
│   ├── altium-sdk-setup.md
│   ├── privacy.md
│   └── review-schema.md
├── schemas/
│   ├── project-snapshot.schema.json
│   └── review-result.schema.json
├── src/
│   ├── CircuitBoss.Extension/
│   ├── CircuitBoss.AltiumAdapter/
│   ├── CircuitBoss.Domain/
│   ├── CircuitBoss.Review/
│   ├── CircuitBoss.OpenAI/
│   ├── CircuitBoss.Security/
│   └── CircuitBoss.UI/
├── tests/
│   ├── CircuitBoss.Domain.Tests/
│   ├── CircuitBoss.Rules.Tests/
│   ├── CircuitBoss.OpenAI.Tests/
│   └── fixtures/
├── prototypes/
│   └── altium-script/
└── packaging/
```

## 17. Implementation Milestones

### Milestone 0 — SDK and feasibility spike

- Confirm current Altium SDK/developer access.
- Confirm compiled extension language/toolchain.
- Create a minimal extension that loads successfully.
- Add one toolbar icon.
- Add one project context-menu command.
- Open an empty dockable panel.
- Document packaging and supported Altium version.

**Acceptance:** A clean Altium installation can load the development extension and show all three UI integrations without errors.

### Milestone 1 — Project extraction

- Enumerate project schematic documents.
- Extract components and parameters.
- Extract pins, nets, and connectivity.
- Capture ERC/compiler messages.
- Serialize a valid versioned snapshot.
- Add redaction and transmitted-data preview.

**Acceptance:** A fixture project produces stable JSON across repeated runs, and representative connections match manual inspection.

### Milestone 2 — Local rule engine

- Implement initial deterministic rules.
- Display local findings in the panel.
- Filter and inspect findings.
- Navigate to affected Altium objects.

**Acceptance:** Automated tests cover the rules, and Locate Object works for components and sheets.

### Milestone 3 — OpenAI settings and connectivity

- Build Settings UI.
- Store/retrieve API key through Windows Credential Manager.
- Implement Test Connection.
- Implement OpenAI provider with mocked tests.
- Add strict request/response schemas.

**Acceptance:** No credential appears in configuration or logs, and mocked structured responses become UI findings.

### Milestone 4 — End-to-end AI review

- Implement batching and orchestration.
- Add progress and cancellation.
- Validate and deduplicate responses.
- Merge local and AI findings.
- Handle API/network failures safely.

**Acceptance:** Review Project completes on a representative multi-sheet design and every displayed finding contains evidence and navigation references when available.

### Milestone 5 — Reporting and history

- Save review metadata locally.
- Export JSON and Markdown/HTML reports.
- Track acknowledged/ignored findings by rule and object identity.
- Compare current results with the previous review.

**Acceptance:** Reports can be generated without exposing credentials or absolute local paths.

### Milestone 6 — Packaging and release readiness

- Create supported installer/extension package.
- Add versioning and update metadata.
- Test install, upgrade, disable, and uninstall.
- Add privacy, security, and API-cost documentation.
- Prepare Altium store submission materials after confirming requirements.

**Acceptance:** CircuitBoss installs and uninstalls cleanly on a supported Altium version and leaves user projects unchanged.

## 18. Testing Strategy

### Unit tests

- Snapshot serialization
- Identifier stability
- Redaction
- Deterministic rules
- Schema validation
- Response deduplication
- Provider error mapping

### Integration tests

- Credential Manager round trip
- Mock OpenAI HTTP server
- Cancellation and timeout behavior
- Large-project batching
- Malformed/partial AI responses

### Altium tests

- Extension load/unload
- Toolbar and context-menu registration
- Panel lifecycle
- Active-sheet review
- Multi-sheet hierarchy extraction
- Repeated designators in channels/variants
- Object navigation after document changes
- Altium shutdown during an active review

### Fixture designs

Create small synthetic projects containing known cases:

- Floating input
- Intentional no-connect
- Single-pin net
- Output conflict
- I2C with and without pull-ups
- CAN with and without termination
- Hierarchical cross-sheet net
- Repeated/channelized circuitry

Do not initially use confidential production designs as automated test fixtures.

## 19. Non-goals for Version 1

- Automatically editing or rewiring schematics
- Guaranteeing regulatory or safety compliance
- Replacing Altium ERC/DRC
- Full SPICE simulation
- PCB signal-integrity or power-integrity simulation
- Automatically downloading arbitrary datasheets
- CircuitBoss website accounts or subscription billing
- Team/cloud review history
- Supporting every historical Altium version

## 20. Definition of Done for MVP

The MVP is complete when:

- CircuitBoss installs as a native Altium extension in the supported development environment.
- Toolbar icon, project context menu, Settings, and dockable review panel work.
- The user can securely store and test an OpenAI API key.
- CircuitBoss extracts and previews a normalized multi-sheet project snapshot.
- Local rules and the OpenAI review can run without blocking or crashing Altium.
- Returned data is schema validated.
- Findings display severity, evidence, recommendation, and affected objects.
- Locate Object works for supported references.
- Review cancellation, API errors, and malformed responses are handled safely.
- No automatic design modifications occur.
- Installation, setup, privacy, cost, and limitations are documented.

## 21. First Instructions for the Coding Agent

1. Read this outline fully before changing files.
2. Inspect the repository and preserve existing user changes.
3. Do not assume an Altium SDK interface or toolchain; first document what is actually available.
4. Begin with Milestone 0 only.
5. Produce the smallest loadable extension that adds a toolbar command, project context-menu command, and empty dockable panel.
6. Keep Altium-specific APIs behind `IAltiumProjectAdapter` or an equivalent boundary.
7. Do not implement real OpenAI network calls until the UI shell and normalized domain schemas exist.
8. Never commit an API key or confidential schematic data.
9. Add tests for logic that can run outside Altium.
10. At the end of each milestone, update the README with build/run instructions and record remaining SDK uncertainties.

## 22. Future Roadmap

- CircuitBoss account and hosted API
- Organization policies and centralized billing
- Private/on-premise review service
- Local-model provider
- Datasheet retrieval with source citations
- PCB layout checks and placement-aware decoupling review
- BOM lifecycle and sourcing checks
- Review baselines and pull-request/CI integration
- Team comments and Altium 365 integration
- Controlled, user-approved schematic fix proposals
