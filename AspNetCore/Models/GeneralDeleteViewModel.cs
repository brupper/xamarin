namespace Brupper.AspNetCore.Models;

public class GeneralDeleteViewModel
{
    public string PrimaryKey { get; set; } = default!;
    public string? ForeignKey { get; set; }
    public string SummaryText { get; set; } = default!;
    public string? Controller { get; set; }
}
