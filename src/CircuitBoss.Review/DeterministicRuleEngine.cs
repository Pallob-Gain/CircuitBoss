using CircuitBoss.Domain;
using CircuitBoss.Domain.Models;
using CircuitBoss.Review.Rules;

namespace CircuitBoss.Review;

public sealed class DeterministicRuleEngine(IEnumerable<IProjectRule> rules) : IRuleEngine
{
    private readonly IReadOnlyList<IProjectRule> _rules = rules.ToArray();

    public IReadOnlyList<Finding> Evaluate(ProjectSnapshot snapshot)
    {
        return _rules
            .SelectMany(rule => rule.Evaluate(snapshot))
            .OrderBy(finding => finding.Id, StringComparer.Ordinal)
            .ToArray();
    }

    public static DeterministicRuleEngine CreateDefault() => new([new SinglePinNetRule()]);
}
