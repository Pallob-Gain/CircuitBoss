using CircuitBoss.Domain;
using CircuitBoss.Domain.Models;
using CircuitBoss.Domain.Serialization;

namespace CircuitBoss.AltiumAdapter;

public sealed class StubAltiumProjectAdapter(ProjectSnapshot snapshot) : IAltiumProjectAdapter
{
    private readonly SnapshotRedactor _redactor = new();

    public ProjectSnapshot CaptureActiveProject(CaptureOptions options)
    {
        return options.RedactProjectIdentity
            ? _redactor.Redact(snapshot, new SnapshotRedactionOptions())
            : snapshot;
    }

    public NavigationResult NavigateTo(ObjectReference target)
    {
        var exists = snapshot.Documents.Any(document => document.Id == target.ObjectId)
            || snapshot.Components.Any(component => component.Id == target.ObjectId)
            || snapshot.Pins.Any(pin => pin.Id == target.ObjectId)
            || snapshot.Nets.Any(net => net.Id == target.ObjectId);

        return exists
            ? new NavigationResult(NavigationStatus.Success)
            : new NavigationResult(NavigationStatus.ObjectNotFound, $"Unable to locate '{target.ObjectId}' in the current snapshot.");
    }
}
