using System.Collections.Generic;
using DarkEyes.Backend.Core.Modifiers;
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
            ResolutionRequest request = new(skill, [new ConcentrationPotion(2)]);
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

        [Fact]
        public void Should_GetDifferentResult_When_UsingSameSeedAndButDifferentInput()
        {
            // Arrange
            var intuition = new Attribute(new("Intuition"), 14);
            var dexterity1 = new Attribute(new("Dexterity"), 12);
            var dexterity2 = new Attribute(new("Dexterity"), 12);
            var skill = new Skill(new("Lockpicking"), 5, intuition, dexterity1, dexterity2);
            List<ResolutionResult> results = [];

            ResolutionRequest request1 = new(skill, [new ConcentrationPotion(2)]);
            ResolutionRequest request2 = new(skill, []);

            SkillCheck check = new();
            IRandomSource random = new SeededRandomSource(1234567);

            // Act
            results.Add(check.Resolve(request1, random));
            results.Add(check.Resolve(request2, random));

            // Assert
            Assert.Equal(ResultType.Success, results[0].Type);
            Assert.Equal(ResultType.Failed, results[1].Type);
            Assert.Equal(1, results[0].QualityLevel);
            Assert.Equal(0, results[1].QualityLevel);
        }

        [Fact]
        public void Should_GetSameResult_When_UsingSameSeedAndNeverActiveModifier()
        {
            // Arrange
            var intuition = new Attribute(new("Intuition"), 14);
            var dexterity1 = new Attribute(new("Dexterity"), 12);
            var dexterity2 = new Attribute(new("Dexterity"), 12);
            var skill = new Skill(new("Lockpicking"), 5, intuition, dexterity1, dexterity2);
            List<ResolutionResult> results = [];

            ResolutionRequest request1 = new(skill, []);
            ResolutionRequest request2 = new(skill, [new NeverPotion()]);

            SkillCheck check = new();
            IRandomSource random = new SeededRandomSource(1234567);

            // Act
            results.Add(check.Resolve(request1, random));
            results.Add(check.Resolve(request2, random));

            // Assert
            Assert.Equal(ResultType.Failed, results[0].Type);
            Assert.Equal(ResultType.Failed, results[1].Type);
            Assert.Equal(0, results[0].QualityLevel);
            Assert.Equal(0, results[1].QualityLevel);
        }

        [Fact]
        public void Should_GetCriticalBlunder_When_RollingThreeTwenties()
        {
            // Arrange
            var intuition = new Attribute(new("Intuition"), 14);
            var dexterity1 = new Attribute(new("Dexterity"), 12);
            var dexterity2 = new Attribute(new("Dexterity"), 12);
            var skill = new Skill(new("Lockpicking"), 5, intuition, dexterity1, dexterity2);
            ResolutionRequest request = new(skill, []);

            SkillCheck check = new();

            IRandomSource random = new SeededRandomSource(79);

            // Act
            ResolutionResult result = check.Resolve(request, random);

            // Assert
            Assert.Equal(ResultType.CriticalBlunder, result.Type);
        }

        [Fact]
        public void Should_GetBlunder_When_RollingThreeTwenties()
        {
            // Arrange
            var intuition = new Attribute(new("Intuition"), 14);
            var dexterity1 = new Attribute(new("Dexterity"), 12);
            var dexterity2 = new Attribute(new("Dexterity"), 12);
            var skill = new Skill(new("Lockpicking"), 5, intuition, dexterity1, dexterity2);
            ResolutionRequest request = new(skill, []);

            SkillCheck check = new();

            IRandomSource random = new SeededRandomSource(770);

            // Act
            ResolutionResult result = check.Resolve(request, random);

            // Assert
            Assert.Equal(ResultType.Blunder, result.Type);
        }

        [Fact]
        public void Should_GetCriticalSuccess_When_RollingThreeTwenties()
        {
            // Arrange
            var intuition = new Attribute(new("Intuition"), 14);
            var dexterity1 = new Attribute(new("Dexterity"), 12);
            var dexterity2 = new Attribute(new("Dexterity"), 12);
            var skill = new Skill(new("Lockpicking"), 5, intuition, dexterity1, dexterity2);
            ResolutionRequest request = new(skill, []);

            SkillCheck check = new();

            IRandomSource random = new SeededRandomSource(437);

            // Act
            ResolutionResult result = check.Resolve(request, random);

            // Assert
            Assert.Equal(ResultType.CriticalSuccess, result.Type);
        }

        [Fact]
        public void Should_GetSuperCriticalSuccess_When_RollingThreeTwenties()
        {
            // Arrange
            var intuition = new Attribute(new("Intuition"), 14);
            var dexterity1 = new Attribute(new("Dexterity"), 12);
            var dexterity2 = new Attribute(new("Dexterity"), 12);
            var skill = new Skill(new("Lockpicking"), 5, intuition, dexterity1, dexterity2);
            ResolutionRequest request = new(skill, []);

            SkillCheck check = new();

            IRandomSource random = new SeededRandomSource(9022);

            // Act
            ResolutionResult result = check.Resolve(request, random);

            // Assert
            Assert.Equal(ResultType.SuperCriticalSuccess, result.Type);
        }

        [Fact]
        public void Should_ProcessModifiersInCorrectOrder()
        {
            // Arrange
            var intuition = new Attribute(new("Intuition"), 5);
            var dexterity1 = new Attribute(new("Dexterity"), 12);
            var dexterity2 = new Attribute(new("Dexterity"), 12);
            var skill = new Skill(new("Lockpicking"), 8, intuition, dexterity1, dexterity2);

            List<ResolutionRequest> requests = [];

            requests.Add(new(skill, [new Blessed(), new ConcentrationPotion(4)]));
            requests.Add(new(skill, [new ConcentrationPotion(4), new Blessed()]));

            List<ResolutionResult> results = [];

            SkillCheck check = new();
            for (int i = 0; i < 2; i++)
            {
                IRandomSource random = new SeededRandomSource(19);

                // Act
                results.Add(check.Resolve(requests[i], random));
            }

            // Assert
            Assert.Equal(2, results[0].QualityLevel);
            Assert.Equal(3, results[1].QualityLevel);
        }
    }

    sealed file class ConcentrationPotion(int strength) : IModifier
    {
        public StatId Target { get; } = new("Intuition");
        public int Order { get; } = 1;
        public int Value { get; } = strength;
        public int Apply(int input) => input + 2;
        public bool IsActive(in ModifierContext context) => true;
    }

    sealed file class NeverPotion : IModifier
    {
        public StatId Target { get; } = new("Intuition");
        public int Order { get; } = 1;
        public int Value { get; } = 2;
        public int Apply(int input) => input + 2;
        public bool IsActive(in ModifierContext context) => false;
    }

    sealed file class Blessed : IModifier
    {
        public StatId Target { get; } = new("Intuition");
        public int Order { get; } = 1;
        public int Value { get; } = 2;
        public int Apply(int input) => input * 2;
        public bool IsActive(in ModifierContext context) => true;
    }
}
