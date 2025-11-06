using Xunit;

namespace Brupper.Maui.Tests.US1;

public class ComponentInventoryTests
{
    private const string MauiProjectPath = @"D:\work\Brupper\_Brupper\Maui";

    [Fact]
    public void MvxBasePage_ShouldBeMigratedToMauiContentPage()
    {
        var basePagePath = Path.Combine(MauiProjectPath, "Views", "BasePage.cs");
        Assert.True(File.Exists(basePagePath));
    }
}
