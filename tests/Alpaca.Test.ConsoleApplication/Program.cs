using Accord.MachineLearning;
using Alpaca.Integrations;
using CsvHelper;
using System.Globalization;
using Accord.Statistics.Kernels;
using Alpaca;
using Alpaca.Evaluation.External;
using Alpaca.Linkage;
using CsvParser = Alpaca.Integrations.CsvParser;

Console.WriteLine("Hello");
var path = @"C:\Work\personal\Diploma\datasets\aggregation.csv";
using var streamReader = new StreamReader(path);
using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
csv.Read();
csv.ReadHeader();

var data = csv.GetRecords<Data>();
var parsed = data.Select(x=> new []{x.x,x.y}).ToArray();

var kmeans = new KMeans(7);
var clusters = kmeans.Learn(parsed);
var clustersCount = (uint)clusters.Count;

foreach (var cluster in kmeans.Clusters)
{
    foreach (var points in cluster.Covariance)
    {
        foreach (var point in points)
        {
            Console.Write(@$"{point} ");
        }
        Console.WriteLine();
    }
}
streamReader.Close();

Console.WriteLine($"Loading data-points from {path}...");

var parser = new CsvParser();
var dataPoints = parser.Load(path);

Console.WriteLine($"Clustering {dataPoints.Count} data-points...");

var metric = new DataPoint(null, null);
var linkages = new Dictionary<ILinkageCriterion<DataPoint>, string>
{
    {new AverageLinkage<DataPoint>(metric), "average"},
    {new CompleteLinkage<DataPoint>(metric), "complete"},
    {new SingleLinkage<DataPoint>(metric), "single"},
    {new MinimumEnergyLinkage<DataPoint>(metric), "min-energy"},
    {new CentroidLinkage<DataPoint>(metric, DataPoint.GetCentroid), "centroid"},
    {new WardsMinimumVarianceLinkage<DataPoint>(metric, DataPoint.GetCentroid), "ward"}
};

// executes agglomerative clustering with several linkage criteria

foreach (var linkage in linkages)
    EvaluateClustering(dataPoints, linkage.Key, linkage.Value, clustersCount);

Console.WriteLine("\nDone!");
void EvaluateClustering(
    ISet<DataPoint> dataPoints, ILinkageCriterion<DataPoint> linkage, string linkageName, uint numClusters)
{
    var clusteringAlg = new AgglomerativeClusteringAlgorithm<DataPoint>(linkage);
    var clustering = clusteringAlg.GetClustering(dataPoints);

    // gets cluster set according to predefined number of clusters
    var clusterSet = clustering.First(cs => cs.Count == numClusters);

    // gets classes for each data-point (first character of the ID in the dataset)
    var pointClasses = dataPoints.ToDictionary(dataPoint => dataPoint, dataPoint => dataPoint.ID[0]);

    Console.WriteLine("=============================================");
    Console.WriteLine($"Evaluating {linkageName} clustering using Euclidean distance...");

    // evaluates the clustering according to different criteria
    var evaluations =
        new Dictionary<string, double>
        {
            {"Purity", new Purity<DataPoint, char>().Evaluate(clusterSet, pointClasses)},
            {"NMI", new NormalizedMutualInformation<DataPoint, char>().Evaluate(clusterSet, pointClasses)},
            {"Accuracy", new RandIndex<DataPoint, char>().Evaluate(clusterSet, pointClasses)},
            {"Precision", new Precision<DataPoint, char>().Evaluate(clusterSet, pointClasses)},
            {"Recall", new Recall<DataPoint, char>().Evaluate(clusterSet, pointClasses)},
            {"F1Measure", new FMeasure<DataPoint, char>(1).Evaluate(clusterSet, pointClasses)},
            {"F2Measure", new FMeasure<DataPoint, char>(2).Evaluate(clusterSet, pointClasses)},
            {"F05Measure", new FMeasure<DataPoint, char>(0.5).Evaluate(clusterSet, pointClasses)}
        };
    foreach (var evaluation in evaluations)
        Console.WriteLine($" - {evaluation.Key}: {evaluation.Value:0.000}");
}