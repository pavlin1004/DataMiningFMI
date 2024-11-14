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
            public int populationCount = 100;
            public double mutationRate = 0.05;
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
                    for (int j = 0; j < n; j++)
                    {
                        bool gene = random.NextDouble() <= 0.5;
                        individual[j] = gene;
                    }
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

                    Crossover(population[cross[0]], population[cross[1]]);
                }
                Mutate();

                population = resultPopulation;
                resultPopulation = new List<bool[]>(populationCount);
            }

            private void Crossover(bool[] first, bool[] second)
            {
                int crossPoint1 = n / 3;
                int crossPoint2 = n - n / 3;

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

                resultPopulation.Add(children1);
                resultPopulation.Add(children2);
            }

            private void Mutate()
            {
                for (int i = 0; i < populationCount; i++)
                {
                    bool toMutate = random.NextDouble() <= mutationRate;
                    if (toMutate)
                    {
                        int gene = random.Next(n);
                        resultPopulation[i][gene] = !resultPopulation[i][gene];
                    }
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
                // Make sure there are at least 8 items to sample
                if (maxFitnesses.Count < 8)
                {
                    Console.WriteLine("Not enough data to select 8 items.");
                    return;
                }

                int first = maxFitnesses.First();
                int last = maxFitnesses.Last();

                // Remove first and last elements from the list
                maxFitnesses.RemoveAt(0);  // Remove the first element
                maxFitnesses.RemoveAt(maxFitnesses.Count - 1);  // Remove the last element

                // Shuffle the remaining list and pick 8 random items
                var randomFitnesses = maxFitnesses.OrderBy(x => random.Next()).Take(8).ToList();


                randomFitnesses.Sort();


                Console.WriteLine(first);

                foreach (var value in randomFitnesses)
                {
                    Console.WriteLine(value);
                }


                Console.WriteLine(last);
            }

            public void Solve()
            {
                LoadData(n);
                LoadPopulation();
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
                        if (noBetterFitnessCount == 10)
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
                Console.WriteLine(currentFitness);
                //Print(maxFitnesses);
            }
            static void Main(string[] args)
            {
                var input = Console.ReadLine().Split(' ');
                int m = int.Parse(input[0]);
                int n = int.Parse(input[1]);
                Knapsack ks = new Knapsack(m, n);
                ks.Solve();
            }
        }
    }
}
