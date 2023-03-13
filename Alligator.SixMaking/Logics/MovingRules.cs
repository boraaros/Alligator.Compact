using Alligator.SixMaking.Model;

namespace Alligator.SixMaking.Logics
{
    public class MovingRules : IMovingRules
    {
        #region Constants (Move calculation boosters)

        #region Pawn Moves

        private static int[][] PawnMoves = new int[][]
        {
            new int[]{ 1, 5 },           // [0,0]
            new int[]{ 0, 2, 6 },        // [0,1]
            new int[]{ 1, 3, 7 },        // [0,2]   
            new int[]{ 2, 4, 8 },        // [0,3]
            new int[]{ 3, 9 },           // [0,4]
            new int[]{ 0, 6, 10 },       // [1,0]
            new int[]{ 1, 5, 7, 11 },    // [1,1]
            new int[]{ 2, 6, 8, 12 },    // [1,2]
            new int[]{ 3, 7, 9, 13 },    // [1,3]
            new int[]{ 4, 8, 14 },       // [1,4]
            new int[]{ 5, 11, 15 },      // [2,0]
            new int[]{ 6, 10, 12, 16 },  // [2,1]
            new int[]{ 7, 11, 13, 17 },  // [2,2]
            new int[]{ 8, 12, 14, 18 },  // [2,3]
            new int[]{ 9, 13, 19 },      // [2,4]
            new int[]{ 10, 16, 20 },     // [3,0]
            new int[]{ 11, 15, 17, 21 }, // [3,1] 
            new int[]{ 12, 16, 18, 22 }, // [3,2]
            new int[]{ 13, 17, 19, 23 }, // [3,3]
            new int[]{ 14, 18, 24 },     // [3,4]
            new int[]{ 15, 21 },         // [4,0]
            new int[]{ 16, 20, 22 },     // [4,1]
            new int[]{ 17, 21, 23 },     // [4,2]
            new int[]{ 18, 22, 24 },     // [4,3]
            new int[]{ 19, 23 },         // [4,4]
        };

        #endregion

        #region Rook Moves

        private static int[][][] RookMoves = new int[][][]
        {
            new int[][]                               // [0,0]
            {
                new int[]{ 1, 2, 3, 4 },
                new int[]{ 5, 10, 15, 20 }
            },
            new int[][]                               // [0,1]
            {
                new int[]{ 0 },
                new int[]{ 2, 3, 4 },
                new int[]{ 6, 11, 16, 21 }
            },
            new int[][]                               // [0,2]
            {
                new int[]{ 1, 0 },
                new int[]{ 3, 4 },
                new int[]{ 7, 12, 17, 22 }
            },
            new int[][]                               // [0,3]
            {
                new int[]{ 2, 1, 0 },
                new int[]{ 4 },
                new int[]{ 8, 13, 18, 23 }
            },
            new int[][]                               // [0,4]
            {
                new int[]{ 3, 2, 1, 0 },
                new int[]{ 9, 14, 19, 24 }
            },
            new int[][]                               // [1,0]
            {
                new int[]{ 6, 7, 8, 9 },
                new int[]{ 0 },
                new int[]{ 10, 15, 20 }
            },
            new int[][]                               // [1,1]
            {
                new int[]{ 7, 8, 9 },
                new int[]{ 1 },
                new int[]{ 5 },
                new int[]{ 11, 16, 21 }
            },
            new int[][]                               // [1,2]
            {
                new int[]{ 8, 9 },
                new int[]{ 2 },
                new int[]{ 6, 5 },
                new int[]{ 12, 17, 22 }
            },
            new int[][]                               // [1,3]
            {
                new int[]{ 9 },
                new int[]{ 3 },
                new int[]{ 7, 6, 5 },
                new int[]{ 13, 18, 23 }
            },
            new int[][]                               // [1,4]
            {
                new int[]{ 4 },
                new int[]{ 8, 7, 6, 5 },
                new int[]{ 14, 19, 24 }
            },
            new int[][]                               // [2,0]
            {
                new int[]{ 5, 0 },
                new int[]{ 11, 12, 13, 14 },
                new int[]{ 15, 20 },
            },
            new int[][]                               // [2,1]
            {
                new int[]{ 6, 1 },
                new int[]{ 12, 13, 14 },
                new int[]{ 16, 21 },
                new int[]{ 10 }
            },
            new int[][]                               // [2,2]
            {
                new int[]{ 7, 2 },
                new int[]{ 13, 14 },
                new int[]{ 17, 22 },
                new int[]{ 11, 10 }
            },
            new int[][]                               // [2,3]
            {
                new int[]{ 8, 3 },
                new int[]{ 14 },
                new int[]{ 18, 23 },
                new int[]{ 12, 11, 10 }
            },
            new int[][]                               // [2,4]
            {
                new int[]{ 9, 4 },
                new int[]{ 19, 24 },
                new int[]{ 13, 12, 11, 10 }
            },
            new int[][]                               // [3,0]
            {
                new int[]{ 10, 5, 0 },
                new int[]{ 16, 17, 18, 19 },
                new int[]{ 20 },
            },
            new int[][]                               // [3,1]
            {
                new int[]{ 11, 6, 1 },
                new int[]{ 17, 18, 19 },
                new int[]{ 21 },
                new int[]{ 15 }
            },
            new int[][]                               // [3,2]
            {
                new int[]{ 12, 7, 2 },
                new int[]{ 18, 19 },
                new int[]{ 22 },
                new int[]{ 16, 15 }
            },
            new int[][]                               // [3,3]
            {
                new int[]{ 13, 8, 3 },
                new int[]{ 19 },
                new int[]{ 23 },
                new int[]{ 17, 16, 15 }
            },
            new int[][]                               // [3,4]
            {
                new int[]{ 14, 9, 4 },
                new int[]{ 24 },
                new int[]{ 18, 17, 16, 15 }
            },
            new int[][]                               // [4,0]
            {
                new int[]{ 15, 10, 5, 0 },
                new int[]{ 21, 22, 23, 24 }
            },
            new int[][]                               // [4,1]
            {
                new int[]{ 16, 11, 6, 1 },
                new int[]{ 22, 23, 24 },
                new int[]{ 20 }
            },
            new int[][]                               // [4,2]
            {
                new int[]{ 17, 12, 7, 2 },
                new int[]{ 23, 24 },
                new int[]{ 21, 20 }
            },
            new int[][]                               // [4,3]
            {
                new int[]{ 18, 13, 8, 3 },
                new int[]{ 24 },
                new int[]{ 22, 21, 20 }
            },
            new int[][]                               // [4,4]
            {
                new int[]{ 19, 14, 9, 4 },
                new int[]{ 23, 22, 21, 20 }
            }
        };

        #endregion

        #region Knight Moves

        private static int[][] KnightMoves = new int[][]
        {
            new int[]{ 7, 11 },                       // [0,0]
            new int[]{ 8, 10, 12 },                   // [0,1]
            new int[]{ 5, 9, 11, 13 },                // [0,2]   
            new int[]{ 6, 12, 14 },                   // [0,3]
            new int[]{ 7, 13 },                       // [0,4]
            new int[]{ 2, 12, 16 },                   // [1,0]
            new int[]{ 3, 13, 15, 17 },               // [1,1]
            new int[]{ 0, 4, 10, 14, 16, 18 },        // [1,2]
            new int[]{ 1, 11, 17, 19 },               // [1,3]
            new int[]{ 2, 12, 18 },                   // [1,4]
            new int[]{ 1, 7, 17, 21 },                // [2,0]
            new int[]{ 0, 2, 8, 18, 20, 22 },         // [2,1]
            new int[]{ 1, 3, 5, 9, 15, 19, 21, 23 },  // [2,2]
            new int[]{ 2, 4, 6, 16, 22, 24 },         // [2,3]
            new int[]{ 3, 7, 17, 23 },                // [2,4]
            new int[]{ 6, 12, 22 },                   // [3,0]
            new int[]{ 5, 7, 13, 23 },                // [3,1] 
            new int[]{ 6, 8, 10, 14, 20, 24 },        // [3,2]
            new int[]{ 7, 9, 11, 21 },                // [3,3]
            new int[]{ 8, 12, 22 },                   // [3,4]
            new int[]{ 11, 17 },                      // [4,0]
            new int[]{ 10, 12, 18 },                  // [4,1]
            new int[]{ 11, 13, 15, 19 },              // [4,2]
            new int[]{ 12, 14, 16 },                  // [4,3]
            new int[]{ 13, 17 },                      // [4,4]
        };

        #endregion

        #region Bishop Moves

        private static int[][][] BishopMoves = new int[][][]
            {
                new int[][]               // [0,0]
                {
                    new int[]{ 6, 12, 18, 24 }
                },
                new int[][]               // [0,1]
                {
                    new int[]{ 5 },
                    new int[]{ 7, 13, 19 }
                }, 
                new int[][]               // [0,2]
                {
                    new int[]{ 6, 10 },
                    new int[]{ 8, 14 }
                },
                new int[][]               // [0,3]
                {
                    new int[]{ 7, 11, 15 },
                    new int[]{ 9 }
                },
                new int[][]               // [0,4]
                {
                    new int[]{ 8, 12, 16, 20 }
                },

                new int[][]               // [1,0]
                {
                    new int[]{ 1 },
                    new int[]{ 11, 17, 23 }
                },
                new int[][]               // [1,1]
                {
                    new int[]{ 0 },
                    new int[]{ 2 },
                    new int[]{ 10 },
                    new int[]{ 12, 18, 24 }
                }, 
                new int[][]               // [1,2]
                {
                    new int[]{ 1 },
                    new int[]{ 3 },
                    new int[]{ 11, 15 },
                    new int[]{ 13, 19 }
                },
                new int[][]               // [1,3]
                {
                    new int[]{ 2 },
                    new int[]{ 4 },
                    new int[]{ 12, 16, 20 },
                    new int[]{ 14 }
                },
                new int[][]               // [1,4]
                {
                    new int[]{ 3 },
                    new int[]{ 13, 17, 21 }
                },
                new int[][]               // [2,0]
                {
                    new int[]{ 6, 2 },
                    new int[]{ 16, 22 }
                },
                new int[][]               // [2,1]
                {
                    new int[]{ 5 },
                    new int[]{ 7, 3 },
                    new int[]{ 17, 23 },
                    new int[]{ 15 }
                }, 
                new int[][]               // [2,2]
                {
                    new int[]{ 6, 0 },
                    new int[]{ 8, 4 },
                    new int[]{ 16, 20 },
                    new int[]{ 18, 24 }
                },
                new int[][]               // [2,3]
                {
                    new int[]{ 7, 1 },
                    new int[]{ 9 },
                    new int[]{ 19 },
                    new int[]{ 17, 21 }
                },
                new int[][]               // [2,4]
                {
                    new int[]{ 8, 2 },
                    new int[]{ 18, 22 }
                },
                new int[][]               // [3,0]
                {
                    new int[]{ 11, 7, 3 },
                    new int[]{ 21 }
                },
                new int[][]               // [3,1]
                {
                    new int[]{ 10 },
                    new int[]{ 12, 8, 4 },
                    new int[]{ 22 },
                    new int[]{ 20 }
                }, 
                new int[][]               // [3,2]
                {
                    new int[]{ 11, 5 },
                    new int[]{ 13, 9 },
                    new int[]{ 23 },
                    new int[]{ 21 }
                },
                new int[][]               // [3,3]
                {
                    new int[]{ 12, 6, 0 },
                    new int[]{ 14 },
                    new int[]{ 24 },
                    new int[]{ 22 }
                },
                new int[][]               // [3,4]
                {
                    new int[]{ 13, 7, 1 },
                    new int[]{ 23 }
                },
                new int[][]               // [4,0]
                {
                    new int[]{ 16, 12, 8, 4 },
                },
                new int[][]               // [4,1]
                {
                    new int[]{ 15 },
                    new int[]{ 17, 13, 9 }
                }, 
                new int[][]               // [4,2]
                {
                    new int[]{ 16, 10 },
                    new int[]{ 18, 14 }
                },
                new int[][]               // [4,3]
                {
                    new int[]{ 17, 11, 5 },
                    new int[]{ 19 }
                },
                new int[][]               // [4,4]
                {
                    new int[]{ 18, 12, 6, 0 }
                },
            };

        #endregion

        #region Queen Moves

        // Queen Moves = Rook Moves + Bishop Moves 

        #endregion

        #endregion

        public bool AreInverses(Step one, Step other)
        {
            return one.From == other.To && one.To == other.From && one.Count == other.Count;
        }

        public IEnumerable<int> LegalMovementsAt(IPosition state, int from)
        {
            switch (state.ColumnHeightAt(from))
            {
                case 1:
                    return GetPawnMoves(state, from);
                case 2:
                    return GetRookMoves(state, from);
                case 3:
                    return GetKnightMoves(state, from);
                case 4:
                    return GetBishopMoves(state, from);
                case 5:
                    return GetQueenMoves(state, from);
                default:
                    throw new InvalidOperationException(string.Format("Invalid height value: {0}", state.ColumnHeightAt(from)));
            }
        }

        private static IEnumerable<int> GetPawnMoves(IPosition state, int from)
        {
            foreach (var i in PawnMoves[from])
            {
                if (state.DiskAt(i, 0) != Disk.None)
                {
                    yield return i;
                }
            }
        }

        private static IEnumerable<int> GetRookMoves(IPosition state, int from)
        {
            foreach (var direction in RookMoves[from])
            {
                foreach (var i in direction)
                {
                    if (state.DiskAt(i, 0) != Disk.None)
                    {
                        yield return i;
                        break;
                    }
                }
            }
        }

        private static IEnumerable<int> GetKnightMoves(IPosition state, int from)
        {
            foreach (var i in KnightMoves[from])
            {
                if (state.DiskAt(i, 0) != Disk.None)
                {
                    yield return i;
                }
            }
        }

        private static IEnumerable<int> GetBishopMoves(IPosition state, int from)
        {
            foreach (var direction in BishopMoves[from])
            {
                foreach (var i in direction)
                {
                    if (state.DiskAt(i, 0) != Disk.None)
                    {
                        yield return i;
                        break;
                    }
                }
            }
        }

        private static IEnumerable<int> GetQueenMoves(IPosition state, int from)
        {
            foreach (var direction in RookMoves[from])
            {
                foreach (var i in direction)
                {
                    if (state.DiskAt(i, 0) != Disk.None)
                    {
                        yield return i;
                        break;
                    }
                }
            }

            foreach (var direction in BishopMoves[from])
            {
                foreach (var i in direction)
                {
                    if (state.DiskAt(i, 0) != Disk.None)
                    {
                        yield return i;
                        break;
                    }
                }
            }
        }
    }
}