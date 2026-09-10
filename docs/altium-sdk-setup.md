# Altium SDK Setup Notes

CircuitBoss is intentionally not bound to a specific Altium SDK package yet.

## Open questions that must be resolved before a production extension is wired up

1. Which current Altium Designer versions expose a supported compiled-extension SDK?
2. What language and runtime are supported for toolbar, context-menu, and dockable-panel registration?
3. What installer or package format is required for local installation and store distribution?
4. Are there signing or certificate requirements for internal testing versus store submission?
5. Which APIs expose project compilation/ERC results and object navigation for schematic entities?

## Current repository stance

Until those questions are answered, all Altium-specific behavior must stay behind `IAltiumProjectAdapter`, and the extension host should remain a testable shell.
