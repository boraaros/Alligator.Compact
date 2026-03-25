![Icon](https://user-images.githubusercontent.com/29038605/222264516-e8132d83-e9dc-4436-a1fd-7bdf046c0034.png)

# Alligator.Compact

**Algorithm Library for Game Theory** — a generic, high-performance solver for two-player zero-sum games.

[![.NET](https://github.com/boraaros/Alligator.Compact/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/boraaros/Alligator.Compact/actions/workflows/build.yml)

## Overview

Alligator.Compact provides an **abstract AI solver** that can play any two-player zero-sum game optimally — you only need to define the rules and the board representation. The solver handles the rest: search, pruning, caching, and move ordering.

The core idea is simple: implement three interfaces (`IRules`, `IPosition`, `IConfiguration`), and get a fully working game AI out of the box.

## Quick Start

```csharp
// 1. Implement IRules<TPosition, TStep> and IPosition<TStep> for your game

// 2. Create a solver
var rules = new MyGameRules();
var config = new MyGameConfiguration();
var provider = new SolverProvider<MyPosition, MyStep>(rules, config);
ISolver<MyStep> solver = provider.Create();

// 3. Get the optimal move
IList<MyStep> moveHistory = new List<MyStep>();
MyStep bestMove = solver.OptimizeNextStep(moveHistory);
```

## Interfaces to Implement

### `IPosition<TStep>` — the game board

| Member | Description |
|---|---|
| `ulong Identifier` | Unique hash of the position (e.g. [Zobrist hashing](https://en.wikipedia.org/wiki/Zobrist_hashing)) |
| `sbyte Value` | [Static evaluation](https://en.wikipedia.org/wiki/Evaluation_function) from the current player's perspective |
| `void Take(TStep step)` | Apply a move to the board |
| `void TakeBack()` | Undo the last move |

### `IRules<TPosition, TStep>` — the game logic

| Member | Description |
|---|---|
| `TPosition InitialPosition()` | Create the starting board |
| `IEnumerable<TStep> LegalStepsAt(TPosition)` | Enumerate all legal moves at a position |
| `bool IsGoal(TPosition)` | `true` if the game ended with a decisive result (not a draw) |

## How the Solver Works

The solver uses **iterative deepening** with **MTD(f)** (Memory-enhanced Test Driver with null-window alpha-beta), also known as *Best Node Search*. At each iteration depth (2, 4, 6, …), it performs a series of null-window alpha-beta searches to converge on the minimax value, then narrows the set of candidate moves.

### Algorithm Features

| Feature | Status | Notes |
|---|---|---|
| Negamax alpha-beta pruning | ✅ Implemented | Core search framework |
| Iterative deepening | ✅ Implemented | Even depths: 2, 4, 6 |
| MTD(f) / Best Node Search | ✅ Implemented | Null-window search with candidate narrowing |
| Transposition table | ✅ Implemented | Zobrist hash, exact/lower/upper bound entries |
| Killer move heuristic | ✅ Implemented | 2 killer moves stored per depth |
| Static evaluation cache | ✅ Implemented | Avoids redundant `IPosition.Value` calls |
| Principal Variation move ordering | ✅ Implemented | Best move from TT tried first |
| Quiescence search | ❌ Not yet | Would reduce horizon effect |
| History heuristic ordering | ❌ Not yet | Move ordering by historical beta-cutoff frequency (scores already recorded) |
| Late move reductions (LMR) | ❌ Not yet | Reduce depth for moves unlikely to be good |
| Null move pruning | ❌ Not yet | Skip a turn to get quick upper bound |
| Parallel search | ❌ Not yet | e.g. Lazy SMP or YBWC |
| Opening book support | ❌ Not yet | Pre-computed opening moves |
| Configurable search depth | ❌ Not yet | Currently hardcoded max depth |

## Solution Structure

| Project | Description |
|---|---|
| **Alligator.Solver** | The abstract solver — game-agnostic core library |
| **Alligator.SixMaking** | [Six Making](https://www.youtube.com/watch?v=FHdltzwaAJg) game implementation |
| **Alligator.TicTacToe** | Tic-tac-toe implementation (minimal example) |
| **Alligator.Demo** | Interactive console demo (human vs AI tic-tac-toe) |
| **Alligator.Benchmark** | Performance benchmarks for the solver |
| **Alligator.Test** | Unit tests |

## Requirements

- .NET 10

## License

See [LICENSE](LICENSE) for details.
