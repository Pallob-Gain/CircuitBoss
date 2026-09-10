using CircuitBoss.Domain.Models;

namespace CircuitBoss.UI;

public sealed record CircuitBossSettings(
    string Model = "Recommended",
    string ReviewProfile = "Default",
    bool IncludeErcMessages = true,
    bool IncludeComponentParameters = true,
    bool IncludeConnectivity = true,
    bool IncludeSchematicImages = false,
    bool PreviewTransmittedData = true,
    bool RemoveProjectNames = true,
    int RequestTimeoutSeconds = 30,
    int MaximumRetryCount = 3)
{
    public CaptureOptions ToCaptureOptions(ReviewScope scope) => new(
        Scope: scope,
        IncludeComponentParameters: IncludeComponentParameters,
        IncludeConnectivity: IncludeConnectivity,
        IncludeErcMessages: IncludeErcMessages,
        RedactProjectIdentity: RemoveProjectNames);
}
