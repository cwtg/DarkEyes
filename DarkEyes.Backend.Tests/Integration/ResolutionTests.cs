using System.Collections.Generic;
using DarkEyes.Backend.Core.Modifier;
using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Tests.Integration
{
    public sealed class ResolutionTests
    {
        [Fact]
        public void Should_GetSameResult_When_UsingSameSeedAndInput()
        {
            // Arrange
            Attribute intuition = new Attribute(new("Intuition"));
            intuition.SetValue(14);
            Attribute dexterity1 = new Attribute(new("Dexterity"));
            dexterity1.SetValue(12);
            Attribute dexterity2 = new Attribute(new("Dexterity"));
            dexterity2.SetValue(12);
            Skill skill = new Skill(new("Lockpicking"), intuition, dexterity1, dexterity2);
            skill.Value = 5;
            ResolutionRequest request = new ResolutionRequest
            {
                RelevantStat = skill,
                Modifiers = [new ConcentrationPotion()]
            };

            List<ResolutionResult> results = [];
            for (int i = 0; i < 2; i++)
            {
                IRandomSource random = new SeededRandomSource(12345);

                // Act
                results.Add(new SkillCheck(random).Resolve(request));
            }
            // Assert
            Assert.Equal(results[0].Type, results[1].Type);
            Assert.Equal(results[0].QualityLevel, results[1].QualityLevel);
        }
    }

    sealed file class ConcentrationPotion : IModifier<IPreRollContext>
    {
        public IActivationCondition<IPreRollContext> Condition { get; } = new AlwaysCondition();

        public CombinationType Combination { get; } = CombinationType.Additive;

        public int Value { get; } = 2;

        public StatId AffectedStat { get; } = new("Intuition");
    }
}
