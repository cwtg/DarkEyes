using System.Linq;
using System.Reflection;
using DarkEyes.Backend.Checks;

namespace DarkEyes.Backend.Tests.Unit.Checks;

public sealed class CheckPurposesTests
{
    [Fact]
    public void Should_ListEveryPurposeInAll_When_TheCatalogDeclaresIt()
    {
        // Arrange: reflection is fine in tests; it guards the hand-maintained list the rule module's completeness test relies on
        CheckPurpose[] declared = typeof(CheckPurposes)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(CheckPurpose))
            .Select(property => (CheckPurpose)property.GetValue(null)!)
            .ToArray();

        // Assert
        Assert.NotEmpty(declared);
        Assert.Equal(declared.OrderBy(purpose => purpose.Value, System.StringComparer.Ordinal), CheckPurposes.All.OrderBy(purpose => purpose.Value, System.StringComparer.Ordinal));
    }
}
