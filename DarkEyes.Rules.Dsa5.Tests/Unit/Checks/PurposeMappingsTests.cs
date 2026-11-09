using DarkEyes.Backend.Checks;
using DarkEyes.Rules.Dsa5.Checks;

namespace DarkEyes.Rules.Dsa5.Tests.Unit.Checks;

public sealed class PurposeMappingsTests
{
    [Fact]
    public void Should_MapEveryCheckPurpose_When_TheBackendCatalogListsIt()
    {
        // Assert: the contract with the backend; a purpose without mapping would only fail at the first such check
        foreach (CheckPurpose purpose in CheckPurposes.All)
        {
            Assert.True(PurposeMappings.TryGet(purpose, out _), $"No DSA5 mapping for check purpose '{purpose}'.");
        }
    }
}
