using System.Collections.Concurrent;
using CircuitBoss.Domain;

namespace CircuitBoss.Security.Credentials;

public sealed class InMemoryCredentialStore : ICredentialStore
{
    private readonly ConcurrentDictionary<string, string> _secrets = new(StringComparer.Ordinal);

    public Task SaveAsync(string credentialName, string secret, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _secrets[credentialName] = secret;
        return Task.CompletedTask;
    }

    public Task<string?> ReadAsync(string credentialName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _secrets.TryGetValue(credentialName, out var secret);
        return Task.FromResult<string?>(secret);
    }

    public Task DeleteAsync(string credentialName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _secrets.TryRemove(credentialName, out _);
        return Task.CompletedTask;
    }
}
