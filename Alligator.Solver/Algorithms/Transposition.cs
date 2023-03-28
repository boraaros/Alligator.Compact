namespace Alligator.Solver.Algorithms
{
    internal class Transposition<TStep>
    {
        public EvaluationMode EvaluationMode;
        public int Value;
        public int Depth;
        public TStep OptimalStep;

        public Transposition(EvaluationMode evaluationMode, int value, int depth, TStep optimalStep)
        {
            EvaluationMode = evaluationMode;
            Value = value;
            Depth = depth;
            OptimalStep = optimalStep;
        }
    }
}