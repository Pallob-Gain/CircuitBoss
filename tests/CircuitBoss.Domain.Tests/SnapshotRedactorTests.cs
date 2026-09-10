using CircuitBoss.Domain.Models;
using CircuitBoss.Domain.Serialization;

namespace CircuitBoss.Domain.Tests;

public sealed class SnapshotRedactorTests
{
    [Fact]
    public void Redact_Replaces_Project_Name_And_Absolute_Document_Path()
    {
        var snapshot = TestProjectFactory.CreateSampleProject();
        var redactor = new SnapshotRedactor();

        var redacted = redactor.Redact(snapshot, new SnapshotRedactionOptions());

        Assert.Equal("[redacted]", redacted.Project.Name);
        Assert.Equal("power.schdoc", redacted.Documents[0].SourceReference);
    }

    [Fact]
    public void ProjectSnapshot_ToJson_Uses_SnakeCase_Contract()
    {
        var snapshot = TestProjectFactory.CreateSampleProject();

        var json = snapshot.ToJson();

        Assert.Contains("\"schema_version\"", json);
        Assert.Contains("\"erc_messages\"", json);
        Assert.Contains("\"connected_pin_ids\"", json);
    }
}

internal static class TestProjectFactory
{
    public static ProjectSnapshot CreateSampleProject()
    {
        return new ProjectSnapshot(
            SchemaVersion: "1.0",
            Project: new ProjectIdentity("project-1", "Motor Controller", "pcb_project"),
            Documents:
            [
                new DocumentSnapshot("doc-1", "Power", "schematic", @"C:\\Users\\alice\\Projects\\motor\\power.schdoc"),
            ],
            Components:
            [
                new ComponentSnapshot(
                    "component-1",
                    "doc-1",
                    "U1",
                    "MCU",
                    "LibRef",
                    "Microcontroller",
                    "Vendor",
                    "Part-1",
                    new BoundingBox(0, 0, 10, 10),
                    [new ParameterSnapshot("component-1", "Voltage", "3V3")]),
            ],
            Pins:
            [
                new PinSnapshot("pin-1", "component-1", "1", "VCC", "power", "net-1", false, 0, 0),
            ],
            Nets:
            [
                new NetSnapshot("net-1", "+3V3", ["pin-1"], true, ["+3V3"], []),
            ],
            Connections:
            [
                new ConnectionSnapshot("pin-1", "net-1"),
            ],
            PowerObjects: ["power-port-1"],
            Directives: ["directive-1"],
            Parameters: [new ParameterSnapshot("project-1", "Variant", "A")],
            ErcMessages:
            [
                new ErcMessageSnapshot(
                    "erc-1",
                    FindingSeverity.Warning,
                    "Example ERC",
                    [new ObjectReference("doc-1", "component", "component-1")]),
            ]);
    }
}
