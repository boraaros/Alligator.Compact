using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Alligator.Test")] // TODO: move this into an assemply-info file!
namespace Alligator.Solver
{
    /// <summary>
    /// Provides the solver instances.
    /// </summary>
    /// <typeparam name="TPosition">type of positions in the specified game</typeparam>
    /// <typeparam name="TStep">type of steps in the specified game</typeparam>
    public class SolverProvider<TPosition, TStep>
    {
        private readonly IRules<TPosition, TStep> rules;
        private readonly IConfiguration solverConfiguration;
        private readonly Action<string> logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SolverProvider(IRules<TPosition, TStep> rules, IConfiguration solverConfiguration, Action<string> logger)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.solverConfiguration = solverConfiguration ?? throw new ArgumentNullException(nameof(solverConfiguration));
            this.logger = logger ?? new Action<string>((logMsg) => { });
        }

        /// <summary>
        /// Creates a solver instance.
        /// </summary>
        /// <returns>solver instance</returns>
        public ISolver<TStep> Create()
        {
            throw new NotImplementedException("There is no solver implementation yet.");
        }
    }
}