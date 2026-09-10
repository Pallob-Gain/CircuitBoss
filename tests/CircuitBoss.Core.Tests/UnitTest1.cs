using CircuitBoss.Core.Analysis;
using CircuitBoss.Core.Models;

namespace CircuitBoss.Core.Tests;

public sealed class CircuitReviewAnalyzerTests
{
    private readonly CircuitReviewAnalyzer _analyzer = new();

    [Fact]
    public void Analyze_FlagsVoltageRatingViolations()
    {
        var design = new CircuitDesign("Voltage Review", new[]
        {
            new CircuitComponent("U1", "LM7805", OperatingVoltage: 12m, MaximumVoltageRating: 10m, DatasheetReference: "https://example.com/lm7805.pdf")
        });

        var report = _analyzer.Analyze(design);

        Assert.Contains(report.Findings, finding =>
            finding.Rule == "voltage-rating-exceeded" &&
            finding.Severity == ReviewFindingSeverity.Error &&
            finding.ComponentReference == "U1");
        Assert.True(report.HasErrors);
    }

    [Fact]
    public void Analyze_FlagsMissingDatasheetReferences()
    {
        var design = new CircuitDesign("Datasheet Review", new[]
        {
            new CircuitComponent("R1", "RES-10K", OperatingVoltage: 3.3m, MaximumVoltageRating: 5m)
        });

        var report = _analyzer.Analyze(design);

        Assert.Contains(report.Findings, finding =>
            finding.Rule == "missing-datasheet" &&
            finding.Severity == ReviewFindingSeverity.Warning &&
            finding.ComponentReference == "R1");
    }

    [Fact]
    public void Analyze_FlagsDuplicateReferences()
    {
        var design = new CircuitDesign("Reference Review", new[]
        {
            new CircuitComponent("C1", "CAP-1UF", DatasheetReference: "https://example.com/cap.pdf"),
            new CircuitComponent("C1", "CAP-1UF", DatasheetReference: "https://example.com/cap.pdf")
        });

        var report = _analyzer.Analyze(design);

        Assert.Contains(report.Findings, finding =>
            finding.Rule == "duplicate-reference" &&
            finding.Severity == ReviewFindingSeverity.Error &&
            finding.ComponentReference == "C1");
    }
}
