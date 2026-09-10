# CircuitBoss Milestone 0 Architecture

This repository currently implements a buildable .NET solution that mirrors the boundaries described in the project outline without binding to undocumented Altium SDK APIs.

## Implemented boundaries

- `CircuitBoss.Domain`: normalized snapshot models, review contracts, JSON serialization, and redaction helpers.
- `CircuitBoss.Review`: deterministic rule-engine shell with the initial single-pin-net rule.
- `CircuitBoss.Security`: credential-store abstraction with an in-memory placeholder implementation.
- `CircuitBoss.AltiumAdapter`: stub adapter that keeps Altium-facing logic behind a single interface.
- `CircuitBoss.UI`: review panel state and settings model.
- `CircuitBoss.Extension`: extension-host shell that defines toolbar, project-menu, and dockable-panel command IDs.

## Milestone 0 status

The codebase now has the basic command/panel architecture required to wire into a compiled Altium extension after the production SDK is confirmed. Actual SDK registration, packaging, signing, and dockable-panel hosting remain blocked on verified Altium documentation.
