using CircuitBoss.Domain.Models;

namespace CircuitBoss.UI.State;

public sealed class ReviewPanelState
{
    public bool IsOpen { get; private set; }
    public string StatusMessage { get; private set; } = "Ready";
    public ReviewSummary Summary { get; private set; } = new(0, 0, 0, 0);
    public IReadOnlyList<Finding> Findings { get; private set; } = [];

    public void Open()
    {
        IsOpen = true;
    }

    public void Focus()
    {
        IsOpen = true;
    }

    public void SetBusy(string message)
    {
        IsOpen = true;
        StatusMessage = message;
    }

    public void ApplyFindings(IReadOnlyList<Finding> findings)
    {
        Findings = findings;
        Summary = ReviewSummary.FromFindings(findings);
        StatusMessage = findings.Count == 0 ? "No findings" : $"Loaded {findings.Count} finding(s)";
    }
}
