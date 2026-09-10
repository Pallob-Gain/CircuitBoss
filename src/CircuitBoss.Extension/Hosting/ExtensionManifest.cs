namespace CircuitBoss.Extension.Hosting;

public sealed record ExtensionManifest(
    string ExtensionId,
    string DockPanelId,
    IReadOnlyList<string> ToolbarCommands,
    IReadOnlyList<string> ProjectContextCommands,
    IReadOnlyList<string> OutstandingSdkQuestions);
