namespace CircuitBoss.Core.Models;

public sealed record CircuitDesign(
    string Name,
    IReadOnlyList<CircuitComponent> Components)
{
    public static CircuitDesign Empty(string name = "Unnamed Design") => new(name, Array.Empty<CircuitComponent>());
}
