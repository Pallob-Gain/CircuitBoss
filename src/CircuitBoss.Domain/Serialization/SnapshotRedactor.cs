using CircuitBoss.Domain.Models;

namespace CircuitBoss.Domain.Serialization;

public sealed record SnapshotRedactionOptions(bool RedactProjectName = true, bool RedactAbsolutePaths = true);

public sealed class SnapshotRedactor
{
    public ProjectSnapshot Redact(ProjectSnapshot snapshot, SnapshotRedactionOptions options)
    {
        var project = options.RedactProjectName
            ? snapshot.Project with { Name = "[redacted]" }
            : snapshot.Project;

        var documents = snapshot.Documents
            .Select(document => document with
            {
                SourceReference = options.RedactAbsolutePaths
                    ? RedactPath(document.SourceReference)
                    : document.SourceReference,
            })
            .ToArray();

        return snapshot with
        {
            Project = project,
            Documents = documents,
        };
    }

    private static string? RedactPath(string? sourceReference)
    {
        if (string.IsNullOrWhiteSpace(sourceReference) || !IsAbsolutePath(sourceReference))
        {
            return sourceReference;
        }

        return Path.GetFileName(sourceReference.Replace('\\', Path.DirectorySeparatorChar));
    }

    private static bool IsAbsolutePath(string path)
    {
        return Path.IsPathRooted(path)
            || (path.Length >= 3
                && char.IsLetter(path[0])
                && path[1] == ':'
                && (path[2] == '\\' || path[2] == '/'));
    }
}
