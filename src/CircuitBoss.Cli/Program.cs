using System.Text.Json;
using CircuitBoss.Core.Analysis;
using CircuitBoss.Core.Models;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: CircuitBoss.Cli <path-to-design.json>");
    return 1;
}

var inputPath = Path.GetFullPath(args[0]);
if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Input file not found: {inputPath}");
    return 1;
}

CircuitDesign? design;
try
{
    await using var stream = File.OpenRead(inputPath);
    design = await JsonSerializer.DeserializeAsync<CircuitDesign>(stream, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    });
}
catch (JsonException exception)
{
    Console.Error.WriteLine($"Invalid design file: {exception.Message}");
    return 1;
}

if (design is null)
{
    Console.Error.WriteLine("The design file could not be deserialized.");
    return 1;
}

var analyzer = new CircuitReviewAnalyzer();
var report = analyzer.Analyze(design);

Console.WriteLine($"CircuitBoss review for '{report.DesignName}'");
Console.WriteLine(new string('=', 32));

if (report.Findings.Count == 0)
{
    Console.WriteLine("No findings detected.");
    return 0;
}

foreach (var finding in report.Findings)
{
    var componentSuffix = string.IsNullOrWhiteSpace(finding.ComponentReference)
        ? string.Empty
        : $" [{finding.ComponentReference}]";
    Console.WriteLine($"- {finding.Severity}: {finding.Rule}{componentSuffix} - {finding.Message}");
}

return report.HasErrors ? 2 : 0;
