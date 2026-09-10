using CircuitBoss.Domain.Models;
using CircuitBoss.Review;

namespace CircuitBoss.Rules.Tests;

public sealed class SinglePinNetRuleTests
{
    [Fact]
    public void Evaluate_Returns_Finding_For_Single_Pin_Net()
    {
        var engine = DeterministicRuleEngine.CreateDefault();
        var snapshot = CreateSnapshot(["pin-1"]);

        var findings = engine.Evaluate(snapshot);

        var finding = Assert.Single(findings);
        Assert.Equal(FindingSeverity.Warning, finding.Severity);
        Assert.Equal("Single-pin net detected", finding.Title);
    }

    [Fact]
    public void Evaluate_Ignores_Multi_Pin_Net()
    {
        var engine = DeterministicRuleEngine.CreateDefault();
        var snapshot = CreateSnapshot(["pin-1", "pin-2"]);

        var findings = engine.Evaluate(snapshot);

        Assert.Empty(findings);
    }

    private static ProjectSnapshot CreateSnapshot(IReadOnlyList<string> connectedPinIds)
    {
        return new ProjectSnapshot(
            SchemaVersion: "1.0",
            Project: new ProjectIdentity("project-1", "Demo", "pcb_project"),
            Documents: [new DocumentSnapshot("doc-1", "Sheet1", "schematic")],
            Components: [],
            Pins: [],
            Nets: [new NetSnapshot("net-1", "SCL", connectedPinIds, false, ["SCL"], [])],
            Connections: [],
            PowerObjects: [],
            Directives: [],
            Parameters: [],
            ErcMessages: []);
    }
}
