# CircuitBoss

CircuitBoss is an AI-powered Altium Designer plugin project for reviewing electronic circuits before release. It is intended to detect logical design faults, identify optimization opportunities, and provide practical engineering recommendations from schematic data plus component datasheets.

## Current project setup

This repository now contains an initial .NET solution that establishes the first review engine for CircuitBoss:

- `src/CircuitBoss.Core` - core review models and analysis rules
- `src/CircuitBoss.Cli` - command-line entry point for reviewing a structured design file
- `tests/CircuitBoss.Core.Tests` - focused unit tests for the review engine

## Initial review capabilities

The first implementation performs a small but useful subset of the intended checks:

- detects duplicate component reference designators
- flags components that are missing datasheet references
- reports when a component's operating voltage exceeds its maximum rating
- warns when a component is operating with low voltage headroom
- suggests review when the operating voltage differs materially from the recommended value

## Run the CLI

```bash
dotnet run --project /home/runner/work/CircuitBoss/CircuitBoss/src/CircuitBoss.Cli -- /path/to/design.json
```

### Example design input

```json
{
  "name": "Motor Driver Board",
  "components": [
    {
      "referenceDesignator": "U1",
      "partNumber": "DRV8833",
      "operatingVoltage": 12.0,
      "maximumVoltageRating": 11.0,
      "recommendedVoltage": 9.0,
      "datasheetReference": "https://example.com/drv8833.pdf"
    },
    {
      "referenceDesignator": "R1",
      "partNumber": "RES-10K",
      "operatingVoltage": 3.3,
      "maximumVoltageRating": 5.0
    }
  ]
}
```

## Run tests

```bash
dotnet test /home/runner/work/CircuitBoss/CircuitBoss/CircuitBoss.sln
```

## Next logical step

The current scaffold is intentionally small. The next iteration can connect this review engine to exported Altium design data, richer rule sets, and AI-assisted datasheet interpretation.
