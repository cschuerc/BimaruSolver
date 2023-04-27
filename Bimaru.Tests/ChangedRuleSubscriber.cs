using System;
using Bimaru.Interface;
using Bimaru.Interface.Game;
using Bimaru.Interface.Solver;

namespace Bimaru.Tests
{
    internal sealed class FieldValueChangedRuleSubscriber : IDisposable
    {
        public FieldValueChangedRuleSubscriber(IBimaruGame game, IFieldValueChangedRule rule)
        {
            Game = game;
            Rule = rule;

            Game.Grid.FieldValueChanged += OnFieldValueChanged;
        }

        private IBimaruGame Game
        {
            get;
        }

        private IFieldValueChangedRule Rule
        {
            get;
        }

        private void OnFieldValueChanged(object sender, FieldValueChangedEventArgs<BimaruValue> e)
        {
            Rule.FieldValueChanged(Game, e);
        }

        public void Dispose()
        {
            Game.Grid.FieldValueChanged -= OnFieldValueChanged;
        }
    }
}
