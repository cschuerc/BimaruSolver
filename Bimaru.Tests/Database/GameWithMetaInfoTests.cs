using System;
using System.Collections.Generic;
using Bimaru.Interface.Database;
using Bimaru.Interface.Game;
using Xunit;

namespace Bimaru.Tests.Database
{
    public class GameWithMetaInfoTests
    {
        [Theory]
        [MemberData(nameof(CreateDataToTestNullArguments))]
        public void TestNullArguments(GameMetaInfo metaInfo, IBimaruGame game, Type expectedExceptionType)
        {
            var exceptionCaught = Record.Exception(() => new GameWithMetaInfo(metaInfo, game));

            Assert.Equal(expectedExceptionType, exceptionCaught?.GetType());
        }

        public static IEnumerable<object[]> CreateDataToTestNullArguments()
        {
            var metaInfo = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game = GameFactoryForTesting.GenerateEmptyGame(1, 1);

            yield return new object[] { metaInfo, null, typeof(ArgumentNullException) };
            yield return new object[] { metaInfo, game, null };
        }
    }
}
