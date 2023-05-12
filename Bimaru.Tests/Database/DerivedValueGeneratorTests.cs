using System;
using Bimaru.Database.Entities;
using Bimaru.Interface.Database;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Xunit;

namespace Bimaru.Tests.Database;

public class DerivedValueGeneratorTests
{
    [Fact]
    public void TestGenerateTemporaryValues()
    {
        var generator = new DerivedValueGenerator<GameEntity, GameSize>(new GameSizeGenerator());

        Assert.False(generator.GeneratesTemporaryValues);
    }

    [Fact]
    public void TestNextWithWrongEntityType()
    {
        var generator = new DerivedValueGenerator<GameEntity, GameSize>(new GameSizeGenerator());

        var mock = new Mock<EntityEntry>(MockBehavior.Default, null);
        mock.Setup(e => e.Entity).Returns(new object());

        Assert.Throws<ArgumentException>(() => generator.Next(mock.Object));
    }

    [Fact]
    public void TestNextWithCorrectEntityType()
    {
        // Arrange
        var mockIntegerGenerator = new Mock<IDerivedValueGenerator<GameEntity, int>>();
        mockIntegerGenerator
            .Setup(g => g.GenerateValue(It.IsAny<GameEntity>()))
            .Returns(7);

        var generator = new DerivedValueGenerator<GameEntity, int>(mockIntegerGenerator.Object);

        var mockEntry = new Mock<EntityEntry>(MockBehavior.Default, null);
        mockEntry.Setup(e => e.Entity).Returns(new GameEntity());

        // Act
        var actualInt = generator.Next(mockEntry.Object);

        // Assert
        Assert.Equal(7, actualInt);
    }
}