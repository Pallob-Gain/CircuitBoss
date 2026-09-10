namespace CircuitBoss.Core.Models;

public sealed record ReviewReport(
    string DesignName,
    IReadOnlyList<ReviewFinding> Findings)
{
    public bool HasErrors => Findings.Any(finding => finding.Severity == ReviewFindingSeverity.Error);
}
