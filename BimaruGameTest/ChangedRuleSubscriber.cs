using BimaruInterfaces;
using System;
using Utility;

namespace BimaruTest
{
    internal class FieldValueChangedRuleSubscriber : IDisposable
    {
        public FieldValueChangedRuleSubscriber(IGame game, IFieldValueChangedRule rule)
        {
            Game = game;
            Rule = rule;

            Game.Grid.FieldValueChanged += OnFieldValueChanged;
        }

        private IGame Game
        {
            get;
            set;
        }

        private IFieldValueChangedRule Rule
        {
            get;
            set;
        }

        void OnFieldValueChanged(object sender, FieldValueChangedEventArgs<BimaruValue> e)
        {
            Rule.FieldValueChanged(Game, e);
        }

        public void Dispose()
        {
            Game.Grid.FieldValueChanged -= OnFieldValueChanged;
        }
    }
}
