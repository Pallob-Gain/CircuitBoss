using System.Text.Json;
using System.Text.Json.Serialization;

namespace CircuitBoss.Domain.Models;

public enum ReviewScope
{
    CurrentSheet,
    SelectedSheets,
    EntireProject,
}

public enum NavigationStatus
{
    Success,
    ObjectNotFound,
    Unsupported,
}

public enum FindingSeverity
{
    Critical,
    Warning,
    Suggestion,
    Information,
}

public enum FindingCategory
{
    Power,
    Connectivity,
    Interface,
    Protection,
    Compiler,
    General,
}

public sealed record CaptureOptions(
    ReviewScope Scope,
    bool IncludeComponentParameters = true,
    bool IncludeConnectivity = true,
    bool IncludeErcMessages = true,
    bool RedactProjectIdentity = true);

public sealed record NavigationResult(NavigationStatus Status, string? Message = null);

public sealed record ObjectReference(string DocumentId, string ObjectType, string ObjectId);

public sealed record ProjectIdentity(string Id, string? Name, string Type);

public sealed record DocumentSnapshot(string Id, string Name, string Kind, string? SourceReference = null);

public sealed record BoundingBox(double X1, double Y1, double X2, double Y2);

public sealed record ParameterSnapshot(string OwnerId, string Name, string Value);

public sealed record ComponentSnapshot(
    string Id,
    string DocumentId,
    string Designator,
    string? Comment,
    string? LibraryReference,
    string? Description,
    string? Manufacturer,
    string? PartNumber,
    BoundingBox? Bounds,
    IReadOnlyList<ParameterSnapshot>? Parameters);

public sealed record PinSnapshot(
    string Id,
    string ComponentId,
    string Number,
    string Name,
    string ElectricalType,
    string? NetId,
    bool IsHidden,
    double X,
    double Y);

public sealed record NetSnapshot(
    string Id,
    string Name,
    IReadOnlyList<string> ConnectedPinIds,
    bool IsPowerNet,
    IReadOnlyList<string>? LabelSources,
    IReadOnlyList<string>? RelatedNetIds);

public sealed record ConnectionSnapshot(string PinId, string NetId);

public sealed record ErcMessageSnapshot(
    string Id,
    FindingSeverity Severity,
    string Message,
    IReadOnlyList<ObjectReference> Objects);

public sealed record ProjectSnapshot(
    string SchemaVersion,
    ProjectIdentity Project,
    IReadOnlyList<DocumentSnapshot> Documents,
    IReadOnlyList<ComponentSnapshot> Components,
    IReadOnlyList<PinSnapshot> Pins,
    IReadOnlyList<NetSnapshot> Nets,
    IReadOnlyList<ConnectionSnapshot> Connections,
    IReadOnlyList<string> PowerObjects,
    IReadOnlyList<string> Directives,
    IReadOnlyList<ParameterSnapshot> Parameters,
    IReadOnlyList<ErcMessageSnapshot> ErcMessages)
{
    public string ToJson() => JsonSerializer.Serialize(this, CircuitBossJson.SerializerOptions);
}

public sealed record Finding(
    string Id,
    FindingSeverity Severity,
    FindingCategory Category,
    string Title,
    string Explanation,
    string Recommendation,
    IReadOnlyList<string> Evidence,
    IReadOnlyList<ObjectReference> Objects,
    double Confidence,
    bool RequiresDatasheetVerification);

public sealed record ReviewSummary(int Critical, int Warnings, int Suggestions, int Information)
{
    public static ReviewSummary FromFindings(IEnumerable<Finding> findings)
    {
        var critical = 0;
        var warnings = 0;
        var suggestions = 0;
        var information = 0;

        foreach (var finding in findings)
        {
            switch (finding.Severity)
            {
                case FindingSeverity.Critical:
                    critical++;
                    break;
                case FindingSeverity.Warning:
                    warnings++;
                    break;
                case FindingSeverity.Suggestion:
                    suggestions++;
                    break;
                default:
                    information++;
                    break;
            }
        }

        return new ReviewSummary(critical, warnings, suggestions, information);
    }
}

public sealed record ReviewBatch(string BatchId, ProjectSnapshot Snapshot);

public sealed record ReviewOptions(
    string Profile,
    bool PreviewDataBeforeSending = true,
    int MaximumRetryCount = 3,
    int RequestTimeoutSeconds = 30);

public sealed record ReviewBatchResult(string BatchId, ReviewSummary Summary, IReadOnlyList<Finding> Findings);
