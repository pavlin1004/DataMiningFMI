using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NQueens.Program;

namespace NQueens
{
    //UP TO 10 000 QUEENS
    internal class Program
    {
        public class Queen 
        {
            public ushort x, y;
            public Queen(ushort x, ushort y)
            {
                this.x = x; 
                this.y = y;
            }

        }

        public class Conflicts
        {
            public ushort[] 
                rowConflicts,
                mainDiagonalConflicts,
                antiDiagonalConflicts;
            public Conflicts(ushort n)
            {
                rowConflicts = new ushort[n];
                mainDiagonalConflicts = new ushort[2 * n - 1];
                antiDiagonalConflicts = new ushort[2 * n - 1];
            }

            public void Add(ushort x, ushort y,ushort n)
            {
                rowConflicts[x]++;
                mainDiagonalConflicts[x + y]++;
                antiDiagonalConflicts[x - y + (n-1)]++;
            }

            public void Remove(ushort x, ushort y,ushort n)
            {
                rowConflicts[x]--;
                mainDiagonalConflicts[x + y]--;
                antiDiagonalConflicts[x - y + (n - 1)]--;
            }

            public ushort GetAllConflicts(ushort x, ushort y, ushort n)
            {
                return (ushort)(rowConflicts[x] + mainDiagonalConflicts[x+y] + antiDiagonalConflicts[x-y+(n-1)]);
            }  
            public ushort GetAllConflictsForAQueen(ushort x, ushort y, ushort n)
            {
                return (ushort)(GetAllConflicts(x, y, n) - 3);
            }
        }
        public class Solver
        {
            public ushort n;
            public Dictionary<Queen, ushort> queens;
            public Conflicts conflicts;
            public Solver(ushort n)
            {
                this.n = n;
                queens = new Dictionary<Queen, ushort>();
                conflicts = new Conflicts(n);

            }
            private Dictionary<Queen, ushort> GenerateQueens()
            {
                Dictionary<Queen, ushort> initial = new Dictionary<Queen, ushort>();
                conflicts = new Conflicts(n);
                Random rand = new Random();

                for (ushort col = 0; col < n; col++)
                {
                    ushort row = (ushort)rand.Next(0, n);
                    initial[new Queen(row, col)] = 0; ;
                    conflicts.Add(row, col, n);
                }
                return initial;
            }

            private Queen GetQueenWithMostConflicts()
            {
               ushort mostConflicts = 0;
               Queen mostConflictsQueen = queens.First().Key;
               foreach(var q in queens)
               {
                    ushort currentConflicts = CalculateConflictsForQueen(q.Key);
                    if(mostConflicts < currentConflicts)
                    {
                        mostConflicts = currentConflicts;
                        mostConflictsQueen = q.Key;
                    }
               }
               return mostConflictsQueen;
            }
            private bool HasConflicts()
            {
                int conflictsCount = 0;
                foreach(var q in queens)
                {
                    conflictsCount += conflicts.GetAllConflictsForAQueen(q.Key.x, q.Key.y, n);
                }
                return conflictsCount != 0;
            }
            private ushort CalculateConflictsForQueen(Queen queen)
            {
                return conflicts.GetAllConflictsForAQueen(queen.x, queen.y, n);
            }
            private void GoToRowWithLessConflicts(Queen queen)
            {
                ushort rowConflicts = 10001;
                ushort rowNumber = 10001;
                for (ushort i = 0; i < n; i++)
                {
                    ushort conflictsForCell = conflicts.GetAllConflicts(i, queen.y, n);
                    if (rowConflicts > conflictsForCell) 
                    {
                        rowConflicts = conflictsForCell;
                        rowNumber = i;
                    }
                }
                conflicts.Remove(queen.x, queen.y, n);
                queen.x = rowNumber;
                conflicts.Add(queen.x, queen.y, n);
            }
            
            public void Print()
            {
                
                char[,] board = new char[n, n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        board[i, j] = '.';
                    }
                }
                foreach (var queen in queens)
                {
                    board[queen.Key.x, queen.Key.y] = 'Q';
                }
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Console.Write(board[i, j] + " ");
                    }
                    Console.WriteLine();
                }
                
            }
            public void Solve()
            {
                this.queens = GenerateQueens();
                Console.WriteLine("k");
                int iter = 0, k=5;
                while(iter++ <= n*k)
                {
                        GoToRowWithLessConflicts(GetQueenWithMostConflicts());
                }
                if(HasConflicts())
                {
                    Solve();
                }
            }
        }
        static void Main(string[] args)
        {
            ushort n = Convert.ToUInt16(Console.ReadLine());
            Stopwatch stopwatch = new Stopwatch();
            Solver solver = new Solver(n);        
            stopwatch.Start();
            solver.Solve();           
            stopwatch.Stop(); // Stop the timer      
            Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds:F2}"); // Print elapsed time in seconds

            Console.WriteLine("Solution!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            solver.Print();
            Console.WriteLine();
        }
    }

    
}
