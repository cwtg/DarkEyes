using System.Collections.Generic;
using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Tests.Unit.Core.Resolution
{
    public sealed class ResolutionTests
    {
        [Fact]
        public void Should_GetSameResult_When_UsingSameSeedAndInput()
        {
            // Arrange
            var intuition = new Attribute(new("Intuition"), 14);
            var dexterity1 = new Attribute(new("Dexterity"), 12);
            var dexterity2 = new Attribute(new("Dexterity"), 12);
            var skill = new Skill(new("Lockpicking"), 5, intuition, dexterity1, dexterity2);
            ResolutionRequest request = new(skill, 0, [new ConcentrationPotion()], []);
            List<ResolutionResult> results = [];

            SkillCheck check = new();
            for (int i = 0; i < 2; i++)
            {
                IRandomSource random = new SeededRandomSource(1234567);

                // Act
                results.Add(check.Resolve(request, random));
            }
            // Assert
            Assert.Equal(ResultType.Success, results[0].Type);
            Assert.Equal(ResultType.Success, results[1].Type);
            Assert.Equal(1, results[0].QualityLevel);
            Assert.Equal(1, results[1].QualityLevel);
        }
    }

    sealed file class ConcentrationPotion : IPreRollModifier
    {
        public StatId Target { get; } = new("Intuition");
        public int Order { get; } = 1;
        public int Value { get; } = 2;
        public int Apply(int input) => input + 2;
        public bool IsActive(in PreRollContext context) => true;
    }
}
