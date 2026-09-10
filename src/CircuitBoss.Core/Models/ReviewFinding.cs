namespace CircuitBoss.Core.Models;

public sealed record ReviewFinding(
    ReviewFindingSeverity Severity,
    string Rule,
    string Message,
    string? ComponentReference = null);
