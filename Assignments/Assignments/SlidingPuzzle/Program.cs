using System.Diagnostics;

namespace SlidingPuzzle
{
    internal class Program
    {
#pragma warning disable CS8602
        public class Node
        {
            public int[,] currentState;
            public int distance;
            public List<string> path;
            public Node(int[,] state, int distance, List<string> path)
            {
                this.currentState = state;
                this.distance = distance;
                this.path = path;
            }
        }
        public class TargetState
        {
            public int n;
            public int size;
            public int zeroPos;
            public int[,] targetState;


            public TargetState(int n, int size, int zeroPos = -1)
            {
                this.n = n;
                this.size = size;
                this.zeroPos = zeroPos;
                targetState = new int[size, size];
                setTargetState();
            }

            void setTargetState()
            {
                int tileValue = 0;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        int currentIndex = i * size + j;

                        if (currentIndex == zeroPos)
                        {
                            targetState[i, j] = 0;
                        }
                        else
                        {
                            targetState[i, j] = tileValue + 1;
                            tileValue++;
                        }
                    }
                }
                if (zeroPos == -1)
                {
                    targetState[size - 1, size - 1] = 0;
                }
            }
        }

        //Initialise grid
        static void LoadGrid(int[,] grid, int size)
        {
            for (int i = 0; i < size; i++)
            {


                string Input = Console.ReadLine().ToString();
                int[] row = Input.Split(' ').Select(int.Parse).ToArray();
                for (int j = 0; j < size; j++)
                {
                    grid[i, j] = row[j];
                }
            }
        }
        //Board Validation
        static bool isBoardValid(int[,] board)
        {

            return (board.GetLength(0) == board.GetLength(1) && board.GetLength(0) >= 2);

        }

        static (int, int) getZeroCoordinates(int[,] currentState)
        {
            for (int i = 0; i < currentState.GetLength(0); i++)
            {
                for (int j = 0; j < currentState.GetLength(1); j++)
                {
                    if (currentState[i, j] == 0)
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }
        public static List<int> convertToList(int[,] arr)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    list.Add(arr[i, j]);
                }
            }
            return list;
        }
        static int findZeroRow(List<int> initalList, List<int> targetList, int rowCount)
        {
            return initalList.IndexOf(0) / rowCount + 1;         
        }
        static bool isSolvable(int[,] initialState, int[,] target)
        {
            if (!isBoardValid(initialState))
            {
                return false;
            }
            List<int> initialList = convertToList(initialState);
            List<int> targetList = convertToList(target);
            int rowCount = initialState.GetLength(0);
            int inversions = 0;

            for (int i = 0; i < initialList.Count - 1; i++)
            {
                if (initialList[i] == 0) continue;
                for (int j = i + 1; j < initialList.Count; j++)
                {
                    if (initialList[j] == 0) continue;
                    if (initialList[i] > initialList[j])
                    {
                        inversions++;
                    }
                }
            }

            if (rowCount % 2 == 1)
            {
                return inversions % 2 == 0;
            }
            else
            {
                int zeroRow = findZeroRow(initialList, targetList, rowCount);
                if (zeroRow == -1) return false;

                return (zeroRow + inversions) % 2 == 1 ? true : false;
            }
        }


        //Heuristic
        static int GetManhattanDistance(TargetState target, int[,] currentState)
        {
            int sum = 0;
            Dictionary<int, (int, int)> currentPositions = new Dictionary<int, (int, int)>();
            Dictionary<int, (int, int)> targetPositions = new Dictionary<int, (int, int)>();

            for (int i = 0; i < target.size; i++)
            {
                for (int j = 0; j < target.size; j++)
                {

                    currentPositions[currentState[i, j]] = (i, j);
                    targetPositions[target.targetState[i, j]] = (i, j);
                }
            }

            for (int i = 1; i <= target.n; i++)
            {
                if (currentPositions.ContainsKey(i) && targetPositions.ContainsKey(i))
                {
                    var currentPos = currentPositions[i];
                    var targetPos = targetPositions[i];
                    sum += Math.Abs(currentPos.Item1 - targetPos.Item1) + Math.Abs(currentPos.Item2 - targetPos.Item2);
                }
                else
                    throw new InvalidDataException();
            }

            return sum;
        }

        //Options
        public static List<(int[,], string)> GenerateNewGrids(int size, int[,] currentNode)
        {
            List<(int[,], string)> neighbours = new List<(int[,], string)>();
            (int, int)[] movements = { (-1, 0), (1, 0), (0, -1), (0, 1) };
            (int, int) zeroPos = getZeroCoordinates(currentNode);
            if (zeroPos == (-1, -1)) throw new Exception();
            for (int i = 0; i < 4; i++)
            {
                int newRow = zeroPos.Item1 + movements[i].Item1;
                int newCol = zeroPos.Item2 + movements[i].Item2;

                if (newRow >= 0 && newRow < size && newCol >= 0 && newCol < size)
                {
                    //newState
                    int[,] newState = (int[,])currentNode.Clone();
                    newState[zeroPos.Item1, zeroPos.Item2] = newState[newRow, newCol];
                    newState[newRow, newCol] = 0;

                    neighbours.Add((newState, GetDirection(i)));
                }
            }
            return neighbours;
        }

        //Algorhitm
        public static void IDAStar(TargetState target, Node currentNode)
        {
            int bound = GetManhattanDistance(target, currentNode.currentState);
            List<(int[,], string)> path = new List<(int[,], string)>();
            path.Add((currentNode.currentState, "")); // initial state with no direction
            while (true)
            {
                int temp = Search(target, path, 0, bound);
                if (temp == -1)
                {
                    PrintSolution(path);
                    break;
                }
                else if (temp == int.MaxValue)
                {
                    Console.WriteLine("No solution found!");
                    break;
                }
                bound = temp;
            }
        }

        public static int Search(TargetState target, List<(int[,], string)> path, int distance, int bound)
        {
            int[,] currentState = path.Last().Item1;
            int f = distance + GetManhattanDistance(target, currentState);
            if (f > bound)
            {
                return f;
            }
            if (f == distance)
            {
                return -1;
            }
            int min = int.MaxValue;
            List<(int[,], string)> successors = new List<(int[,], string)>();
            successors = GenerateNewGrids(target.size, currentState);

            foreach (var successor in successors)
            {
                if (!path.Any(pair => pair.Item1 == successor.Item1))
                {
                    path.Add(successor);
                    int temp = Search(target, path, distance + 1, bound);
                    if (temp == -1)
                    {
                        return -1;
                    }
                    else if (temp < min)
                    {
                        min = temp;
                    }
                    path.Remove(successor);
                }
            }
            return min;
        }

        //Converts move to string
        public static string GetDirection(int i)
        {
            switch (i)
            {
                case 0: return "down";
                case 1: return "up";
                case 2: return "right";
                case 3: return "left";
                default: return "";
            }

        }
        public static void PrintSolution(List<(int[,] l, string)> path)
        {
            Console.WriteLine(path.Count - 1);
            for (int i = 1; i < path.Count; i++)
            {
                Console.WriteLine(path[i].Item2.ToString());
            }
        }

        static int Main(string[] args)
        {
            //Read input
            int n = Convert.ToInt32(Console.ReadLine());
            int zeroPos = Convert.ToInt32(Console.ReadLine());
            int size = (int)Math.Sqrt(n + 1);
            int[,] grid = new int[size, size];
            LoadGrid(grid, size);

            //Initial and target state
            Node current = new Node(grid, 0, new List<string>());
            TargetState target = new TargetState(n, size, zeroPos);

            //Return -1 if not solvable
            //if (isSolvable(grid, target.targetState) == false)
            //    return -1;
            Stopwatch stopwatch = new Stopwatch();
          
            stopwatch.Start();
            
            
            IDAStar(target, current);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));


            return 0;
        }
    }
}