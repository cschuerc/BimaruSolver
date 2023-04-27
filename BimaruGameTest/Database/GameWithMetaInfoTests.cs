using System;
using System.Collections.Generic;
using Bimaru.Database;
using Bimaru.Game;
using Bimaru.Interfaces;
using Xunit;

namespace Bimaru.Test
{
    public class GameWithMetaInfoTests
    {
        [Theory]
        [MemberData(nameof(CreateDataToTestNullArguments))]
        public void TestNullArguments(IGameMetaInfo metaInfo, IBimaruGame game, Type expectedExceptionType)
        {
            var exceptionCaught = Record.Exception(() => new GameWithMetaInfo(metaInfo, game));

            Assert.Equal(expectedExceptionType, exceptionCaught?.GetType());
        }

        public static IEnumerable<object[]> CreateDataToTestNullArguments()
        {
            var metaInfo = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game = new GameFactory().GenerateEmptyGame(1, 1);

            yield return new object[] { null, game, typeof(ArgumentNullException) };
            yield return new object[] { metaInfo, null, typeof(ArgumentNullException) };
            yield return new object[] { metaInfo, game, null };
        }
    }
}
