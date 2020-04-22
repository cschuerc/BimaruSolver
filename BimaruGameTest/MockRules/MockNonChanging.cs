using BimaruInterfaces;
using BimaruSolver;
using System.Collections.Generic;

namespace BimaruTest
{
    internal class MockNonChanging : ITrialAndErrorRule
    {
        public IEnumerable<IEnumerable<FieldToChange>> GetCompleteChangeTrials(IGame game)
        {
            return new List<List<FieldToChange>>() { new List<FieldToChange>() };
        }
    }
}
