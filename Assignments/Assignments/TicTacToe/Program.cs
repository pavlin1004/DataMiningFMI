using System.ComponentModel;
using System.Xml.Schema;

namespace TicTacToe
{
    //MiniMax + Alpha-Beta pruning
    internal class Program
    {

        class TicTacToe
        {
            public char[][] board;
            public bool humanToPlay;
            public bool isAIMaximising;
            public TicTacToe(bool humanToPlay)
            {
                board = new char[3][];
                InitialiseBoard();
                this.humanToPlay = humanToPlay;
                this.isAIMaximising = !humanToPlay;
            }
            private void InitialiseBoard()
            {
                for (int i = 0; i < 3; i++)
                {
                    board[i] = new char[3];
                    for (int j = 0; j < 3; j++)
                    {
                        board[i][j] = ' ';
                    }
                }
            }
            public void Print()
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.Write(string.Join('|', board[i]));
                    Console.WriteLine();
                }
            }
            private int GetState(int x, int y)
            {
                if(isAIMaximising) return board[x][y] == 'O' ? 1 : -1;
                return board[x][y] == 'X' ? 1 : -1;

            }
            public int GetResult()
            {
                if (board[0][0] == board[1][1] && board[1][1] == board[2][2] && board[1][1] != ' ')
                {
                    return GetState(1, 1); // win/lose
                }
                else if (board[0][2] == board[1][1] && board[1][1] == board[2][0] && board[1][1] != ' ')
                {
                    return GetState(1, 1);
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (board[i][0] == board[i][1] && board[i][1] == board[i][2] && board[i][0] != ' ')
                        {
                            return GetState(i, 1);
                        }
                        if (board[0][i] == board[1][i] && board[1][i] == board[2][i] && board[0][i] != ' ')
                        {
                            return GetState(1, i);
                        }
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    if (board[i].Contains(' '))
                    {
                        return -2; // no draw
                    }
                }
                
                return 0;
            }

            public int MiniMax(int alpha, int beta, bool isMaximising)
            {
                int result = GetResult();
                if (result != -2)
                {
                    return result;
                }

                int bestResult;
                if (isMaximising)
                {
                    bestResult = int.MinValue;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (board[i][j] == ' ')
                            {                               
                                board[i][j] = 'X';
                                int current = MiniMax(alpha, beta, false);
                                board[i][j] = ' ';
                                alpha = Math.Max(alpha, current);
                                if (bestResult < alpha)
                                {
                                    bestResult = alpha;
                                }
                                if (alpha > beta)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    bestResult = int.MaxValue;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (board[i][j] == ' ')
                            {
                                board[i][j] = 'O';
                                int current = MiniMax(alpha, beta, true);
                                board[i][j] = ' ';
                                beta = Math.Min(beta, current);
                                if (bestResult > beta)
                                {
                                    bestResult = beta;
                                }
                                if (alpha >= beta)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                return bestResult;

            }


            public void AIMove()
            {
                (int, int) bestMove = (1, 0);
                if (isAIMaximising)
                {
                    int bestResult = int.MinValue;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (board[i][j] == ' ')
                            {
                                board[i][j] = 'O';
                                int current = MiniMax(int.MinValue, int.MaxValue, false);
                                board[i][j] = ' ';
                                if (current > bestResult)
                                {
                                    bestResult = current;
                                    bestMove = (i, j);
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    int bestResult = int.MaxValue;               
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (board[i][j] == ' ')
                            {
                                board[i][j] = 'O';
                                int current = MiniMax(int.MinValue, int.MaxValue, true);
                                board[i][j] = ' ';
                                if (current < bestResult)
                                {
                                    bestResult = current;
                                    bestMove = (i, j);
                                }
                            }
                        }
                    }
                   
                }
                board[bestMove.Item1][bestMove.Item2] = 'O';
            }
            public void HumanMove()
            {
                int i, j;
                while (true)
                {
                    var input = Console.ReadLine().Split(' ').ToArray();
                    i = int.Parse(input[0]);
                    j = int.Parse(input[1]);
                    if (board[i][j] != ' ')
                    {
                        Console.WriteLine("Invalid move! Try again.");
                    }
                    else
                    {
                        board[i][j] = 'X';
                        break;
                    }
                }
            }
            public void Play()
            {
                while (true)
                {
                    if (humanToPlay)
                    {
                        HumanMove();
                    }
                    else
                    {
                        AIMove();
                    }
                    humanToPlay = !humanToPlay;
                    Print();
                    int result = GetResult();   
                    switch (result)
                    {
                        case 1: Console.WriteLine("Human wins!"); break;
                        case -1: Console.WriteLine("AI wins!"); break;
                        case 0: Console.WriteLine("Draw!"); break;
                        default: continue;
                    }
                    break;
                }
            }

            static void Main(string[] args)
            {
                Console.WriteLine("Who should go first? Enter '1' for Human or '2' for AI:");
                int firstPlayer = Convert.ToInt32(Console.ReadLine());
                bool humanToPlay = firstPlayer == 1 ? true : false;
                TicTacToe game = new TicTacToe(humanToPlay);
                game.Play();
            }
        }
    }
}
