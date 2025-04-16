using System;
using System.Collections.Generic;
using System.Linq;
using static DecisionTree.Program;

#pragma warning disable CS8618
#pragma warning disable CS8604
namespace DecisionTree
{
    internal class Program
    {
        public class DecisionTree
        {
            public bool IsLeaf;
            public int AttributeColumn;
            public string Classification;
            public string AttributeValue;
            public List<List<string>> currentTestData;
            public List<DecisionTree> Subtrees;

            public double CalculateEntropy(List<List<string>> dataset)
            {
                var targetValues = dataset.Select(row => row[0]).ToList();
                var valueCounts = targetValues.GroupBy(v => v).ToDictionary(g => g.Key, g => g.Count());
                double entropy = 0.0;
                int total = targetValues.Count;

                foreach (var count in valueCounts.Values)
                {
                    double probability = (double)count / total;
                    entropy -= probability * Math.Log2(probability);
                }
                return entropy;
            }

            public double CalculateAttributeEntropy(List<List<string>> data, int attributeIndex)
            {
                var attributeGroups = data.GroupBy(row => row[attributeIndex]);
                double entropy = 0.0;

                foreach (var group in attributeGroups)
                {
                    int groupSize = group.Count();
                    double probability = (double)groupSize / data.Count();
                    if (probability > 0)
                    {
                        entropy -= probability * Math.Log2(probability);
                    }
                }
                return entropy;
            }

            public double CalculateInformationGain(List<List<string>> data, int attributeIndex)
            {
                return CalculateEntropy(data) - CalculateAttributeEntropy(data, attributeIndex);
            }

            public void BuildTree(List<List<string>> data, int buildOption, List<List<string>>? validationData = null)
            {
                if (data.All(row => row[0] == data[0][0]))
                {
                    IsLeaf = true;
                    Classification = data[0][0];
                    return;
                }
                if (data[0].Count == 1) 
                {
                    IsLeaf = true;
                    Classification = data.GroupBy(row => row[0]).OrderByDescending(g => g.Count()).First().Key;
                    return;
                }
                currentTestData = data;
                double bestInformationGain = double.MinValue;
                int bestAttribute = -1;

                for (int i = 1; i < data[0].Count; i++)
                {
                    double infoGain = CalculateInformationGain(data, i);
                    if (infoGain > bestInformationGain)
                    {
                        bestInformationGain = infoGain;
                        bestAttribute = i;
                    }
                }

                if (buildOption == 1 || buildOption == 3)
                {
                    if (PrePruning(data, bestInformationGain)) return;
                }

                AttributeColumn = bestAttribute;
                Subtrees = new List<DecisionTree>();
                var attributeGroups = data.GroupBy(row => row[bestAttribute]);

                foreach (var group in attributeGroups)
                {
                    var subgroup = group.ToList();
                    string currentAttributeValue = group.Key;

                    subgroup = RemoveAttributeColumn(subgroup, bestAttribute);

                    DecisionTree subtree = new DecisionTree
                    {
                        AttributeValue = currentAttributeValue
                    };
                    subtree.BuildTree(subgroup, buildOption, validationData);
                    Subtrees.Add(subtree);
                }

                if (buildOption == 2 || buildOption == 3)
                {
                    PostPruning(validationData);
                }
            }

            private void PostPruning(List<List<string>> validationData)
            {
                if (IsLeaf) return;

                var originalClassification = Classification;
                var originalIsLeaf = IsLeaf;
                var originalAttributeColumn = AttributeColumn;
                var originalSubtrees = Subtrees;

                IsLeaf = true;
                Classification = GetMajorityClass(validationData);

                double prunedError = CalculateError(validationData);

                IsLeaf = false;
                Classification = originalClassification;
                AttributeColumn = originalAttributeColumn;
                Subtrees = originalSubtrees;
                double originalError = CalculateError(validationData);

                if (prunedError < originalError)
                {
                    return; // Keep the pruned tree
                }
                IsLeaf = false;
                Classification = originalClassification;
                AttributeColumn = originalAttributeColumn;
                Subtrees = originalSubtrees;
            }

            private bool PrePruning(List<List<string>> data, double bestInformationGain)
            {
                double informationGainThreshold = 0.1;
                if (bestInformationGain < informationGainThreshold)
                {
                    IsLeaf = true;
                    Classification = GetMajorityClass(data);
                    return true;
                }
                return false;
            }

            public string Predict(List<string> example)
            {
                if (IsLeaf)
                    return Classification;

                string attributeValue = example[AttributeColumn];
                var matchingSubtree = Subtrees.FirstOrDefault(t => t.AttributeValue == attributeValue);

                if (matchingSubtree != null)
                {
                    var modifiedExample = new List<string>(example);
                    modifiedExample.RemoveAt(AttributeColumn); 
                    return matchingSubtree.Predict(modifiedExample);
                }
                //throw new Exception("Marry Chrismas");
                return GetMajorityClass(currentTestData);
            }

            private string GetMajorityClass(List<List<string>> data)
            {
                var classCounts = data.GroupBy(row => row[0]).ToDictionary(g => g.Key, g => g.Count());
                return classCounts.OrderByDescending(c => c.Value).First().Key;
            }

            public double CalculateError(List<List<string>> validationData)
            {
                int incorrectCount = validationData.Count(example =>
                {
                    string actualClass = example[0];
                    string predictedClass = Predict(example);
                    return actualClass != predictedClass;
                });

                return (double)incorrectCount / validationData.Count;
            }

            public double CalculateAccuracy(List<List<string>> data)
            {
                int correctCount = data.Count(example =>
                {
                    string actualClass = example[0];
                    string predictedClass = Predict(example);
                    return actualClass == predictedClass;
                });

                return (double)correctCount / data.Count;
            }

            private List<List<string>> RemoveAttributeColumn(List<List<string>> data, int attributeIndex)
            {
                return data.Select(row =>
                {
                    List<string> newRow = new List<string>(row);
                    newRow.RemoveAt(attributeIndex);
                    return newRow;
                }).ToList();
            }

            public void PrintTree(int indent = 0)
            {
                string indentStr = new string(' ', indent);

                if (IsLeaf)
                {
                    Console.WriteLine($"{indentStr}Leaf: {Classification}");
                    return;
                }

                Console.WriteLine($"{indentStr}If attribute {AttributeColumn} == {AttributeValue}, then:");
                foreach (var subtree in Subtrees)
                {
                    Console.WriteLine($"{indentStr}  Branch for value {subtree.AttributeValue}:");
                    subtree.PrintTree(indent + 4);
                }
            }
        }

        public class Solution
        {
            public List<List<string>> Data;
            public List<List<string>> TrainData;
            public List<List<string>> TestData;
            public DecisionTree Model;

            public Solution()
            {
                LoadData();
            }

            public void LoadData()
            {
                Data = new List<List<string>>();
                foreach (var line in System.IO.File.ReadLines("./breast-cancer.data"))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var row = line.Split(',', StringSplitOptions.TrimEntries).ToList();
                        Data.Add(row);
                    }
                }
                HandleMissingValues();
            }

            public void HandleMissingValues()
            {
                for (int col = 0; col < Data[0].Count; col++)
                {
                    var values = Data.Where(row => row[col] != "?").Select(row => row[col]);
                    string mostCommonValue = values.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key;

                    foreach (var row in Data)
                    {
                        if (row[col] == "?")
                        {
                            row[col] = mostCommonValue;
                        }
                    }
                }
            }

            public void StratifiedSplit(List<List<string>> data, double trainRatio)
            {
                Random rand = new Random();

                var classGroups = data.GroupBy(row => row[0]).ToList();
                List<List<string>> trainData = new List<List<string>>();
                List<List<string>> testData = new List<List<string>>();

                foreach (var group in classGroups)
                {
                    int groupSize = group.Count();
                    int trainSize = (int)(groupSize * trainRatio);

                    List<List<string>> current = group.OrderBy(x => rand.Next()).ToList();

                    var trainGroup = current.Take(trainSize).ToList();
                    var testGroup = current.Skip(trainSize).ToList();

                    trainData.AddRange(trainGroup);
                    testData.AddRange(testGroup);
                }
                TrainData = trainData;
                TestData = testData;
            }

            public void BuildLearnModel(int buildOption)
            {
                Model = new DecisionTree();
                if (buildOption == 2 || buildOption == 3)
                {
                    Model.BuildTree(TrainData, buildOption, TestData);
                    return;
                }
                if (buildOption == 1)
                {
                    Model.BuildTree(TrainData, buildOption);
                    return;
                }
               
            }

            public (double averageAccuracy, double standardDeviation, List<double> foldAccuracies) CrossValidation(int numFolds, int buildOption)
            {
                Random rand = new Random();
                Data = Data.OrderBy(x => rand.Next()).ToList();

                int foldSize = Data.Count / numFolds;
                List<double> foldAccuracies = new List<double>();

                for (int fold = 0; fold < numFolds; fold++)
                {
                    StratifiedSplit(Data, 0.8);
                    DecisionTree model = new DecisionTree();
                    model.BuildTree(TrainData, buildOption, TestData);

                    double accuracy = model.CalculateAccuracy(TestData);
                    foldAccuracies.Add(accuracy);

                    // Print fold result
                    Console.WriteLine($"Fold {fold + 1} Accuracy: {accuracy * 100}%");
                }

                double averageAccuracy = foldAccuracies.Average();
                double standardDeviation = Math.Sqrt(foldAccuracies.Select(a => Math.Pow(a - averageAccuracy, 2)).Average());

                return (averageAccuracy, standardDeviation, foldAccuracies);
            }
        }

        static void Main(string[] args)
        {
            Solution solution = new Solution();
            solution.StratifiedSplit(solution.Data, 0.8); // Stratified split with 80% train, 20% test

            Console.WriteLine("Choose pruning option (1: Pre-pruning, 2: Post-pruning, 3: Both): ");
            int buildOption = Convert.ToInt32(Console.ReadLine());

            solution.BuildLearnModel(buildOption);

            double TestDataAccuracy = solution.Model.CalculateAccuracy(solution.TestData);
            Console.WriteLine($"Test Accuracy: {TestDataAccuracy * 100}%");

            var (avgAccuracy, stdDev, _) = solution.CrossValidation(10, buildOption);
            Console.WriteLine($"Average accuracy: {avgAccuracy * 100}%");
            Console.WriteLine($"Standard deviation: {stdDev}");

            Console.WriteLine();
            double TrainDataAccuracy = solution.Model.CalculateAccuracy(solution.TrainData);
            Console.WriteLine($"Train Accuracy: {TrainDataAccuracy * 100}%");


        }
    }
}
