using CircuitBoss.Core.Models;

namespace CircuitBoss.Core.Analysis;

public sealed class CircuitReviewAnalyzer
{
    private const decimal VoltageHeadroomThreshold = 0.9m;
    private const decimal OptimizationTolerance = 0.1m;

    public ReviewReport Analyze(CircuitDesign design)
    {
        ArgumentNullException.ThrowIfNull(design);

        var findings = new List<ReviewFinding>();
        var seenReferences = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var component in design.Components)
        {
            if (string.IsNullOrWhiteSpace(component.ReferenceDesignator))
            {
                findings.Add(new ReviewFinding(
                    ReviewFindingSeverity.Error,
                    "missing-reference",
                    "A component is missing a reference designator."));
                continue;
            }

            if (!seenReferences.Add(component.ReferenceDesignator))
            {
                findings.Add(new ReviewFinding(
                    ReviewFindingSeverity.Error,
                    "duplicate-reference",
                    $"Component reference '{component.ReferenceDesignator}' is duplicated, which can lead to incorrect BOM or netlist results.",
                    component.ReferenceDesignator));
            }

            if (string.IsNullOrWhiteSpace(component.DatasheetReference))
            {
                findings.Add(new ReviewFinding(
                    ReviewFindingSeverity.Warning,
                    "missing-datasheet",
                    $"Component '{component.ReferenceDesignator}' does not include a datasheet reference for AI-backed review.",
                    component.ReferenceDesignator));
            }

            if (component.OperatingVoltage is { } operatingVoltage && component.MaximumVoltageRating is { } maximumVoltageRating)
            {
                if (operatingVoltage > maximumVoltageRating)
                {
                    findings.Add(new ReviewFinding(
                        ReviewFindingSeverity.Error,
                        "voltage-rating-exceeded",
                        $"Component '{component.ReferenceDesignator}' operates at {operatingVoltage} V, which exceeds its maximum rated voltage of {maximumVoltageRating} V.",
                        component.ReferenceDesignator));
                }
                else if (maximumVoltageRating > 0 && operatingVoltage / maximumVoltageRating >= VoltageHeadroomThreshold)
                {
                    findings.Add(new ReviewFinding(
                        ReviewFindingSeverity.Warning,
                        "low-voltage-headroom",
                        $"Component '{component.ReferenceDesignator}' operates close to its maximum voltage rating ({operatingVoltage} V of {maximumVoltageRating} V).",
                        component.ReferenceDesignator));
                }
            }

            if (component.OperatingVoltage is { } supplyVoltage && component.RecommendedVoltage is { } recommendedVoltage &&
                recommendedVoltage > 0)
            {
                var variance = Math.Abs(supplyVoltage - recommendedVoltage) / recommendedVoltage;
                if (variance > OptimizationTolerance)
                {
                    findings.Add(new ReviewFinding(
                        ReviewFindingSeverity.Info,
                        "voltage-optimization",
                        $"Component '{component.ReferenceDesignator}' runs at {supplyVoltage} V instead of its recommended {recommendedVoltage} V. Review whether this can be optimized.",
                        component.ReferenceDesignator));
                }
            }
        }

        if (design.Components.Count == 0)
        {
            findings.Add(new ReviewFinding(
                ReviewFindingSeverity.Info,
                "empty-design",
                "No components were provided for review."));
        }

        return new ReviewReport(design.Name, findings);
    }
}
