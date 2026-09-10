using CircuitBoss.Domain.Models;

namespace CircuitBoss.Domain;

public interface IAltiumProjectAdapter
{
    ProjectSnapshot CaptureActiveProject(CaptureOptions options);
    NavigationResult NavigateTo(ObjectReference target);
}

public interface ICredentialStore
{
    Task SaveAsync(string credentialName, string secret, CancellationToken cancellationToken = default);
    Task<string?> ReadAsync(string credentialName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string credentialName, CancellationToken cancellationToken = default);
}

public interface IRuleEngine
{
    IReadOnlyList<Finding> Evaluate(ProjectSnapshot snapshot);
}

public interface IReviewProvider
{
    Task<ReviewBatchResult> ReviewAsync(
        ReviewBatch batch,
        ReviewOptions options,
        CancellationToken cancellationToken);
}
