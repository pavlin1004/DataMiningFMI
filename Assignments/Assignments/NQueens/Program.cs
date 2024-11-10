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

            public string QueensToString()
            {
                return string.Join(',',queens);
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
                int rowConflicts = 10001;
                int rowNumber = 10001;
                for (int i = 0; i < n; i++)
                {
                    int conflictsForCell = conflicts.GetAllConflicts(i, y , n);
                    if (rowConflicts > conflictsForCell)
                    {
                        rowConflicts = conflictsForCell;
                        rowNumber = i;
                    }
                }
                return rowNumber;
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
                for(int i=0;i<n;i++)
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
                Console.WriteLine(String.Join(',',queens));
            }
            public bool Solve()
            {
                GenerateQueens();
                int iter = 0,k = 1+2*n/100;
                while(iter++ <= n*k)
                {
                    int column = rand.Next(0, n);
                    GoToRowWithLessConflicts(queens[column],column);
                }
                return HasConflicts();
            }
        }
        static int Main(string[] args)
        {
            int n = Convert.ToInt32(Console.ReadLine());
            if (n < 0 || n == 3 || n > 10000) return -1;
            Stopwatch stopwatch = new Stopwatch();
            Solver solver = new Solver(n);        
            stopwatch.Start();
            while(solver.Solve())
            solver.Solve();           
            stopwatch.Stop(); // Stop the timer
            if (n >= 100)
            {
                Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds:F2}"); // Print elapsed time in seconds
            }
            else
            {
                solver.printArray();
            }
            return 0;
            //solver.Print();
            

        }
    }
}
