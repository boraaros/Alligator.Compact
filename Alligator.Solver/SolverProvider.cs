namespace Alligator.Solver
{
    /// <summary>
    /// Provides the solver instances.
    /// </summary>
    /// <typeparam name="TPosition">type of positions in the specified game</typeparam>
    /// <typeparam name="TMove">type of moves in the specified game</typeparam>
    public class SolverProvider<TPosition, TMove>
    {
        private readonly IRules<TPosition, TMove> rules;
        private readonly IConfiguration solverConfiguration;
        private readonly Action<string> logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SolverProvider(IRules<TPosition, TMove> rules, IConfiguration solverConfiguration, Action<string> logger)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.solverConfiguration = solverConfiguration ?? throw new ArgumentNullException(nameof(solverConfiguration));
            this.logger = logger ?? new Action<string>((logMsg) => { });
        }

        /// <summary>
        /// Creates a solver instance.
        /// </summary>
        /// <returns>solver instance</returns>
        public ISolver<TMove> Create()
        {
            throw new NotImplementedException("There is no solver implementation yet.");
        }
    }
}