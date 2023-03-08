namespace Alligator.Solver.Algorithms
{
    internal interface IAlphaBetaPruning<TPosition>
    {
        int Search(TPosition position, int alpha, int beta);
    }
}