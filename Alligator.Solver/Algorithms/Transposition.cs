namespace Alligator.Solver.Algorithms
{
    internal class Transposition<TStep>
    {
        public EvaluationMode EvaluationMode;
        public int Value;
        public int Depth;
        public TStep optimalStep;

        public Transposition(EvaluationMode evaluationMode, int value, int depth, TStep optimalStep)
        {
            EvaluationMode = evaluationMode;
            Value = value;
            Depth = depth;
            this.optimalStep = optimalStep;
        }
    }
}