using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NQueens.Program;

namespace NQueens
{
    
    internal class Program
    {
        public class Conflicts
        {
            public int[] 
                rowConflicts,
                mainDiagonalConflicts,
                antiDiagonalConflicts;

            public Conflicts(int n)
            {
                rowConflicts = new int[n];
                mainDiagonalConflicts = new int[2 * n - 1];
                antiDiagonalConflicts = new int[2 * n - 1];
            }

            public void Add(int x, int y, int n)
            {
                rowConflicts[x]++;
                mainDiagonalConflicts[x + y]++;
                antiDiagonalConflicts[x - y + (n-1)]++;
            }

            public void Remove(int x, int y, int n)
            {
                rowConflicts[x]--;
                mainDiagonalConflicts[x + y]--;
                antiDiagonalConflicts[x - y + (n - 1)]--;
            }

            public int GetAllConflicts(int x, int y, int n)
            {
                return rowConflicts[x] + mainDiagonalConflicts[x+y] + antiDiagonalConflicts[x-y+(n-1)];
            }  
            public int GetAllConflictsForAQueen(int x, int y, int n)
            {
                return GetAllConflicts(x, y, n) - 3;
            }
        }
        public class Solver
        {
            public int n;
            public int[] queens;
            public Conflicts conflicts;
            Random rand;
            public Solver(int n)
            {
                this.n = n;
                queens = new int[n]; // queen[0] = 1 => col-0, row-1
                conflicts = new Conflicts(n);
                rand = new Random();
            }
            private void GenerateQueens()
            {
                conflicts = new Conflicts(n);
              
                for (int i=0;i<n;i++)
                {
                    int row = rand.Next(0,n);
                    queens[i] = row;
                    conflicts.Add(queens[i], i, n);
                } 
            }
            private int GetRandomQueenWithMostConflicts()
            {
                List<int> mostConflcitedQueens = new List<int>();
                int mostConflicts = 0;
                for (int i = 0; i < n; i++)
                {
                    int current = conflicts.GetAllConflicts(queens[i], i, n);
                    if (current > mostConflicts)
                    {
                        mostConflcitedQueens.Clear();
                        mostConflicts = current;
                        mostConflcitedQueens.Add(i);
                    }
                    else if (current == mostConflicts)
                    {
                        mostConflcitedQueens.Add(i);
                    }
                }
                return mostConflcitedQueens[rand.Next(mostConflcitedQueens.Count)];
            }
            private bool HasConflicts()
            {
                int conflictsCount = 0;
                for( int i = 0;i < n;i++)
                {
                    conflictsCount += conflicts.GetAllConflictsForAQueen(queens[i],i,n);
                }
                return conflictsCount != 0;
            }
            private int GetRowWithLessConflicts(int x, int y)
            {
                List<int> rows = new List<int>();
                int rowConflicts = 10001;
                for (int i = 0; i < n; i++)
                {
                    int conflictsForCell = conflicts.GetAllConflicts(i, y , n);
                    if (rowConflicts > conflictsForCell)
                    {
                        rows.Clear();
                        rowConflicts = conflictsForCell;
                        rows.Add(i);
                    }
                    if(rowConflicts == conflictsForCell)
                    {
                        rows.Add(i);
                    }
                }
                return rows[rand.Next(rows.Count)];
            }
            private void GoToRowWithLessConflicts(int x, int y)
            {
                conflicts.Remove(x, y, n);
                int rowNumber = GetRowWithLessConflicts(x, y);
                queens[y] = rowNumber;
                conflicts.Add(queens[y], y, n);               
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
                for (int i = 0; i < n; i++)
                {
                    board[queens[i], i] = 'Q';
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
            public void printArray()
            {
                Console.Write('[');
                Console.Write(String.Join(", ",queens));
                Console.Write(']');
            }
            public bool Solve()
            {
                GenerateQueens();
                int iter = 0, k = 1;
                while(iter++ <= n*k)
                {
                    if (!HasConflicts()) return true;
                    int column = GetRandomQueenWithMostConflicts();
                    GoToRowWithLessConflicts(queens[column],column);
                }
                return !HasConflicts();
            }
        }
        static void Main(string[] args)
        {
            int n = Convert.ToInt32(Console.ReadLine());
            if (n <= 0 || n==2 || n == 3 )
            {
                Console.WriteLine(-1);
                return;
            }
            Stopwatch stopwatch = new Stopwatch();
            Solver solver = new Solver(n);        
            stopwatch.Start();
            while(solver.Solve()!=true)  
            stopwatch.Stop(); 
            if (n > 100)
            {
                Console.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));

            }
            else
            {
                solver.printArray();
                Console.WriteLine( );
                solver.Print();
            }
        }
    }
}
