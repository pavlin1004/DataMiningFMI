using System.Diagnostics;
using System.Reflection;

namespace Knapsack
{
    internal class Program
    {
        public class Knapsack
        {
            public static int noBetterFitnessCount = 0;
            public int n;
            public int m;
            public int populationCount = 10000;
            public double mutationRate = 0.4;
            int[] weights;
            int[] costs;
            List<bool[]> population;
            List<bool[]> resultPopulation;
            Random random;
            public Knapsack(int m, int n)
            {
                this.m = m;
                this.n = n;
                this.weights = new int[n];
                this.costs = new int[n];
                this.population = new List<bool[]>(populationCount);
                this.resultPopulation = new List<bool[]>(populationCount);
                this.random = new Random();
            }
            public void LoadData(int n)
            {
                for (int i = 0; i < n; i++)
                {
                    var input = Console.ReadLine().Split(' ');
                    weights[i] = int.Parse(input[0]);
                    costs[i] = int.Parse(input[1]);
                }
            }
            public void LoadPopulation()
            {
                for (int i = 0; i < populationCount; i++)
                {
                    var individual = new bool[n];
                    var weight = 0;
                    for (int j = 0; j < n; j++)
                    {
                        bool gene = random.NextDouble() <= 0.05;
                        individual[j] = gene;
                        if (gene) weight += weights[j];
                    }
                    //if(weight == 0 || weight > m) i--;
                     population.Add(individual);
                }
            }

            public int[] GetFitness()
            {
                int[] fitnesses = new int[populationCount];
                for (int i = 0; i < populationCount; i++)
                {
                    int currentFitness = 0;
                    int currentTotalWeight = 0;
                    for (int j = 0; j < n; j++)
                    {
                        // Console.WriteLine($"i {i} j {j}");
                        if (population[i][j])
                        {
                            currentTotalWeight += weights[j];
                            currentFitness += costs[j];
                        }
                    }
                    if (currentTotalWeight <= m)
                    {
                        fitnesses[i] = currentFitness;
                    }
                    else
                    {
                        fitnesses[i] = 0;
                    }
                }
                return fitnesses;
            }
            private int Fight(int index1, int index2, int[] fitnesses)
            {
                if (fitnesses[index1] > fitnesses[index2])
                {
                    return index1;
                }
                else if (fitnesses[index1] < fitnesses[index2])
                {
                    return index2;
                }
                else return random.NextDouble() <= 0.5 ? index1 : index2;
            }

            private void GenerateNewPopulation()
            {
                List<int> indexes = Enumerable.Range(0, populationCount).ToList();
                List<bool[]> children = new List<bool[]>();
                int[] fitnesses = GetFitness();

                int[] cross = new int[2];

                while (indexes.Count > 0)
                {
                    int chosen = 0;
                    while (chosen < 2)
                    {
                        int index1 = random.Next(0, indexes.Count);
                        indexes.RemoveAt(index1);

                        int index2 = random.Next(0, indexes.Count);
                        indexes.RemoveAt(index2);

                        cross[chosen] = Fight(index1, index2, fitnesses);
                        resultPopulation.Add(population[cross[chosen]]);
                        chosen++;
                    }

                    Crossover(population[cross[0]], population[cross[1]],children);
                }
                Mutate(children);

                population = resultPopulation;
                resultPopulation = new List<bool[]>(populationCount);
            }

            private void Crossover(bool[] first, bool[] second, List<bool[]> children)
            {
                int crossPoint1 = random.Next(n/2);
                int crossPoint2 = random.Next(n / 2 + 1, n);

                bool[] children1 = new bool[n];
                bool[] children2 = new bool[n];
                for (int i = 0; i < crossPoint1; i++)
                {
                    children1[i] = first[i];
                    children2[i] = second[i];
                }
                for (int i = crossPoint1; i < crossPoint2; i++)
                {
                    children1[i] = second[i];
                    children2[i] = first[i];
                }
                for (int i = crossPoint2; i < n; i++)
                {
                    children1[i] = first[i];
                    children2[i] = second[i];
                }

                children.Add(children1);
                children.Add(children2);
            }

            private void Mutate(List<bool[]> children)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    bool toMutate = random.NextDouble() <= mutationRate;
                    if (toMutate)
                    {
                        int gene = random.Next(n);
                        children[i][gene] = !children[i][gene];
                    }
                }
                for(int i=0;i<children.Count;i++)
                {
                    resultPopulation.Add(children[i]);
                }
            }

            private int getMaxFitnessIndex(int[] fitnesses)
            {
                int maxFitness = 0;
                int index = 0;
                for (int i = 0; i < populationCount; i++)
                {
                    if (maxFitness < fitnesses[i])
                    {
                        index = i;
                        maxFitness = fitnesses[i];
                    }
                }
                return index;
            }
            public void Print(List<int> maxFitnesses)
            {
                int k = maxFitnesses.Count / 9; 
                int[] idx = new int[]
                {
                    k * 1,  
                    k * 2,  
                    k * 3,  
                    k * 4,  
                    k * 5,  
                    k * 6,
                    k * 7,
                    k * 8
                };
                Console.WriteLine(maxFitnesses.First()); 
                foreach (int index in idx)
                {
                        Console.WriteLine(maxFitnesses[index]);
                }
                Console.WriteLine(maxFitnesses.Last());
            }
        

            public void Solve()
            {
                LoadData(n);
                LoadPopulation();
                Console.WriteLine();
                List<int> maxFitnesses = new List<int>();
                int currentFitness = 0;
                while (true)
                {

                    int[] fitnesses = GetFitness();
                    int maxFitness = getMaxFitnessIndex(fitnesses);
                    maxFitnesses.Add(fitnesses[maxFitness]);
                    if (currentFitness >= fitnesses[maxFitness])
                    {
                        noBetterFitnessCount++;
                        if (noBetterFitnessCount ==  20)
                        {
                            break;
                        }
                    }
                    else
                    {
                        currentFitness = fitnesses[maxFitness];
                        noBetterFitnessCount = 0;
                    }
                    //Console.WriteLine(noBetterFitnessCount);
                    GenerateNewPopulation();
                }
                //Console.WriteLine(currentFitness);
                Print(maxFitnesses);
            }
            static void Main(string[] args)
            {
                var input = Console.ReadLine().Split(' ');
                int m = int.Parse(input[0]);
                int n = int.Parse(input[1]);
                Knapsack ks = new Knapsack(m, n);
                Stopwatch stopwatch = new Stopwatch();               
                stopwatch.Start();
                ks.Solve();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
            }
        }
    }
}