using CircuitBoss.Domain;
using CircuitBoss.Domain.Models;
using CircuitBoss.UI.State;

namespace CircuitBoss.Extension.Hosting;

public sealed class CircuitBossExtensionHost(
    IAltiumProjectAdapter altiumProjectAdapter,
    IRuleEngine ruleEngine,
    ReviewPanelState panelState)
{
    public const string ExtensionId = "CircuitBoss";
    public const string DockPanelId = "CircuitBoss.ReviewPanel";
    public const string ToolbarCommandId = "CircuitBoss.OpenPanel";
    public const string ReviewProjectCommandId = "CircuitBoss.ReviewProject";
    public const string ReviewSelectedSheetsCommandId = "CircuitBoss.ReviewSelectedSheets";
    public const string ReviewCurrentSheetCommandId = "CircuitBoss.ReviewCurrentSheet";
    public const string SettingsCommandId = "CircuitBoss.Settings";

    public ExtensionManifest Register() => new(
        ExtensionId,
        DockPanelId,
        [ToolbarCommandId],
        [
            ReviewProjectCommandId,
            ReviewSelectedSheetsCommandId,
            ReviewCurrentSheetCommandId,
            SettingsCommandId,
        ],
        [
            "Confirm the supported compiled Altium extension SDK and packaging format.",
            "Confirm dockable panel registration APIs for the target Altium release.",
            "Confirm toolbar and project context-menu integration points in the production SDK.",
        ]);

    public void OpenPanel()
    {
        panelState.Focus();
    }

    public IReadOnlyList<Finding> ReviewProject(CaptureOptions options)
    {
        panelState.SetBusy("Reviewing project");
        var snapshot = altiumProjectAdapter.CaptureActiveProject(options);
        var findings = ruleEngine.Evaluate(snapshot);
        panelState.ApplyFindings(findings);
        return findings;
    }
}
