using CircuitBoss.Domain.Models;

namespace CircuitBoss.Review.Rules;

public interface IProjectRule
{
    IReadOnlyList<Finding> Evaluate(ProjectSnapshot snapshot);
}
