namespace Alligator.Solver.Algorithms
{
    internal class CacheTables<TPosition, TStep> : ICacheTables<TPosition, TStep>
        where TPosition : IPosition<TStep>
    {
        private const int TranspositionTableSize = 1 << 21;
        private const int TranspositionTableMask = TranspositionTableSize - 1;

        private const int EvaluationTableSize = 1 << 23;
        private const int EvaluationTableMask = EvaluationTableSize - 1;

        private const ulong KeyMarker = 0xBF58476D1CE4E5B9UL;

        private readonly ulong[] transpositionKeys;
        private readonly Transposition<TStep>[] transpositionValues;

        private readonly ulong[] evaluationKeys;
        private readonly int[] evaluationValues;

        public CacheTables()
        {
            transpositionKeys = new ulong[TranspositionTableSize];
            transpositionValues = new Transposition<TStep>[TranspositionTableSize];
            evaluationKeys = new ulong[EvaluationTableSize];
            evaluationValues = new int[EvaluationTableSize];
        }

        public void AddTransposition(TPosition position, Transposition<TStep> transposition)
        {
            int index = (int)(position.Identifier & TranspositionTableMask);
            transpositionKeys[index] = position.Identifier ^ KeyMarker;
            transpositionValues[index] = transposition;
        }

        public void AddValue(TPosition position, int value)
        {
            int index = (int)(position.Identifier & EvaluationTableMask);
            evaluationKeys[index] = position.Identifier ^ KeyMarker;
            evaluationValues[index] = value;
        }

        public bool TryGetTransposition(TPosition position, out Transposition<TStep> transposition)
        {
            int index = (int)(position.Identifier & TranspositionTableMask);
            if (transpositionKeys[index] == (position.Identifier ^ KeyMarker))
            {
                transposition = transpositionValues[index];
                return true;
            }
            transposition = default;
            return false;
        }

        public bool TryGetValue(TPosition position, out int value)
        {
            int index = (int)(position.Identifier & EvaluationTableMask);
            if (evaluationKeys[index] == (position.Identifier ^ KeyMarker))
            {
                value = evaluationValues[index];
                return true;
            }
            value = 0;
            return false;
        }
    }
}
