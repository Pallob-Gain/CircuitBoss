using CircuitBoss.Domain.Models;

namespace CircuitBoss.Review.Rules;

public sealed class SinglePinNetRule : IProjectRule
{
    public IReadOnlyList<Finding> Evaluate(ProjectSnapshot snapshot)
    {
        return snapshot.Nets
            .Where(net => net.ConnectedPinIds.Count == 1)
            .Select(net => new Finding(
                Id: $"CB-CONNECTIVITY-SINGLE-PIN-{net.Id}",
                Severity: FindingSeverity.Warning,
                Category: FindingCategory.Connectivity,
                Title: "Single-pin net detected",
                Explanation: $"Net '{net.Name}' only connects to one pin in the normalized snapshot.",
                Recommendation: "Verify whether the net is intentionally isolated or missing a connection.",
                Evidence:
                [
                    $"Net {net.Name} has {net.ConnectedPinIds.Count} connected pin.",
                    $"Connected pin id: {net.ConnectedPinIds[0]}",
                ],
                Objects:
                [
                    new ObjectReference(net.Id, "net", net.Id),
                ],
                Confidence: 0.95,
                RequiresDatasheetVerification: false))
            .ToArray();
    }
}
