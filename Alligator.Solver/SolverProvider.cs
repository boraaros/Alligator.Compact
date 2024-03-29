﻿using Alligator.Solver.Algorithms;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Alligator.Test")] // TODO: move this into an assemply-info file!
[assembly: InternalsVisibleTo("Alligator.Benchmark")] // TODO: move this into an assemply-info file!
namespace Alligator.Solver
{
    /// <summary>
    /// Provides the solver instances.
    /// </summary>
    /// <typeparam name="TPosition">type of positions in the specified game</typeparam>
    /// <typeparam name="TStep">type of steps in the specified game</typeparam>
    public class SolverProvider<TPosition, TStep>
        where TPosition : IPosition<TStep>
    {
        private readonly IRules<TPosition, TStep> rules;
        private readonly IConfiguration solverConfiguration;
        private readonly Action<string> logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SolverProvider(IRules<TPosition, TStep> rules, IConfiguration solverConfiguration, Action<string> logger = null)
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
            var cacheTables = new CacheTables<TPosition, TStep>();
            var heuristicTables = new HeuristicTables<TStep>();

            return Create(cacheTables, heuristicTables);
        }

        internal ISolver<TStep> Create(ICacheTables<TPosition, TStep> cacheTables, IHeuristicTables<TStep> heuristicTables)
        {
            var searchManager = new SearchManager(6); // TODO: remove this ctor parameter

            return new AlphaBetaSolver<TPosition, TStep>(
                new AlphaBetaPruning<TPosition, TStep>(rules, cacheTables, heuristicTables, searchManager), 
                rules, 
                searchManager, 
                logger);
        }
    }
}