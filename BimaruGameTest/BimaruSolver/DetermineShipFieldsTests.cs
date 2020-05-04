using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class DetermineShipFieldsTests
    {
        private static Game SetupGame(int numRows, int numColumns)
        {
            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            RollbackGrid grid = new RollbackGrid(numRows, numColumns);

            return new Game(rowTally, columnTally, settings, grid);
        }

        private static void SubscribeRule(IGame game)
        {
            var rule = new DetermineShipFields();

            void OnFieldValueChanged(object sender, FieldValueChangedEventArgs<BimaruValue> e)
            {
                rule.FieldValueChanged(game, e);
            }

            game.Grid.FieldValueChanged += OnFieldValueChanged;
        }



        [TestMethod]
        public void TestDetShipSingle()
        {
            int numRows = 3;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);
            SubscribeRule(game);

            var p11 = new GridPoint(1, 1);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_UNDETERMINED);

            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(2, 1), BimaruValue.WATER);

            // ?W?
            // WSW
            // ?W?

            Assert.AreEqual(BimaruValue.SHIP_SINGLE, game.Grid.GetFieldValue(p11));
        }

        [TestMethod]
        public void TestDetShipMiddle()
        {
            int numRows = 3;
            int numColumns = 3;

            var p11 = new GridPoint(1, 1);

            var game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_UNDETERMINED);

            // ???
            // SSS
            // ???

            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid.GetFieldValue(p11));


            game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(2, 1), BimaruValue.SHIP_UNDETERMINED);

            // ?S?
            // ?S?
            // ?S?

            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid.GetFieldValue(p11));
        }

        [TestMethod]
        public void TestDetContShips()
        {
            int numRows = 3;
            int numColumns = 3;

            var p11 = new GridPoint(1, 1);

            var game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_UNDETERMINED);

            // ???
            // WSS
            // ???

            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid.GetFieldValue(p11));


            game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.WATER);

            // ???
            // SSW
            // ???

            Assert.AreEqual(BimaruValue.SHIP_CONT_LEFT, game.Grid.GetFieldValue(p11));


            game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(2, 1), BimaruValue.SHIP_UNDETERMINED);

            // ?S?
            // ?S?
            // ?W?

            Assert.AreEqual(BimaruValue.SHIP_CONT_UP, game.Grid.GetFieldValue(p11));


            game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(2, 1), BimaruValue.WATER);

            // ?W?
            // ?S?
            // ?S?

            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, game.Grid.GetFieldValue(p11));
        }

        [TestMethod]
        public void TestShipMiddleNeighbours()
        {
            int numRows = 3;
            int numColumns = 3;

            var p11 = new GridPoint(1, 1);

            var game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_MIDDLE);

            // ???
            // ?S?
            // ???

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(2, 1)));


            game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_MIDDLE);
            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(2, 1), BimaruValue.SHIP_CONT_DOWN);

            // ?S?
            // ?S?
            // ???

            Assert.AreEqual(BimaruValue.SHIP_CONT_UP, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 2)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, game.Grid.GetFieldValue(new GridPoint(2, 1)));


            game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_MIDDLE);
            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(2, 1), BimaruValue.WATER);

            // ?W?
            // ?S?
            // ???

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_LEFT, game.Grid.GetFieldValue(new GridPoint(1, 2)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(2, 1)));
        }

        [TestMethod]
        public void TestSeveralShips()
        {
            int numRows = 5;
            int numColumns = 5;

            var game = SetupGame(numRows, numColumns);
            SubscribeRule(game);
            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.SHIP_MIDDLE);
            game.Grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(0, 3), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(0, 4), BimaruValue.WATER);

            game.Grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(2, 2), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(3, 2), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(4, 2), BimaruValue.WATER);

            game.Grid.SetFieldValue(new GridPoint(4, 0), BimaruValue.SHIP_UNDETERMINED);

            game.Grid.SetFieldValue(new GridPoint(2, 4), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(3, 3), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(3, 4), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(4, 4), BimaruValue.WATER);

            // S? ?? WW ?? WW
            // ?? ?? S? WW S?
            // ?? ?? S? ?? WW
            // ?? ?? WW ?? ??
            // S? SM S? S? WW

            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_LEFT, game.Grid.GetFieldValue(new GridPoint(0, 3)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 4)));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 2)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_UP, game.Grid.GetFieldValue(new GridPoint(2, 2)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, game.Grid.GetFieldValue(new GridPoint(3, 2)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(4, 2)));

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(4, 0)));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(2, 4)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(3, 3)));
            Assert.AreEqual(BimaruValue.SHIP_SINGLE, game.Grid.GetFieldValue(new GridPoint(3, 4)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(4, 4)));

            int numUndet = game.Grid.AllPoints().Where(p => game.Grid.GetFieldValue(p) == BimaruValue.UNDETERMINED).Count();

            Assert.AreEqual(10, numUndet);
        }
    }
}
