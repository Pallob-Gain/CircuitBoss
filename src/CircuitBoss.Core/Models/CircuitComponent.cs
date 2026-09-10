namespace CircuitBoss.Core.Models;

public sealed record CircuitComponent(
    string ReferenceDesignator,
    string PartNumber,
    decimal? OperatingVoltage = null,
    decimal? MaximumVoltageRating = null,
    decimal? RecommendedVoltage = null,
    string? DatasheetReference = null,
    string? Notes = null);
