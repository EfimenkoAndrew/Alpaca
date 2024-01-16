using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UnicornAnalytics.Clustering;
using UnicornAnalytics.Indexes.External;
using UnicornAnalytics.Indexes.Internal;
using UpdatedEvaluations;

var name = "4";
var path = $@"C:\Work\personal\Diploma\datasets\data+y\{name}.txt";
using var streamReader = new StreamReader(path);
using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false});

var data = csv.GetRecords<Data>();
var parsed = data.Select(x => new[] { x.x, x.y }).ToArray();
var points = parsed.Select(x => new Point(x)).ToArray();
var sim = SimilarityMatrix.SparseSimilarityMatrix(points);

KMeans kMeans = new KMeans();
kMeans.Fit(parsed, 3);

FuzzyCMeans fuzzyCMeans = new FuzzyCMeans(3, 10.0);
fuzzyCMeans.Fit(parsed, 100);

var meanShift = new MeanShift(75);
meanShift.Fit(parsed);

AffinityPropagation affinityPropagation = new AffinityPropagation(parsed.Length);
var clusters = affinityPropagation.Fit(sim);
ClusterUtility.AssignClusterCenters(points, clusters);
int[] centers_index = new int[affinityPropagation.Centers.Count];
affinityPropagation.Centers.CopyTo(centers_index);
var t = ClusterUtility.GroupClusters(points, clusters, centers_index);
var afpCentroids = points.Where((element, index) => centers_index.Contains(index)).Select(x=>x.Data).ToArray();
var afpClusterLabels = t.Select(x => x.Item2).ToArray();

CalinskiHarabaszIndex ch = new CalinskiHarabaszIndex(); 
DaviesBouldinIndex db = new DaviesBouldinIndex(); 
CIndexCalculatorIndex cIndex = new CIndexCalculatorIndex(); 
SilhouetteIndex sIndex = new SilhouetteIndex();
HubertIndex hubertIndex = new HubertIndex();

RandIndex randIndex = new RandIndex();
Dictionary<string, double> indexValidations = new();


ValidateIndexes(parsed, meanShift.Labels, meanShift.Centers);
WriteResultsToFile($@"C:\Work\personal\Diploma\datasets\data+y\{name}.meanShift.txt", parsed, meanShift.Labels);

ValidateIndexes(parsed, afpClusterLabels, afpCentroids);
WriteResultsToFile($@"C:\Work\personal\Diploma\datasets\data+y\{name}.affinityPropagation.txt", parsed, afpClusterLabels);

ValidateIndexes(parsed, kMeans.ClusterLabels, kMeans.Centroids);
WriteResultsToFile($@"C:\Work\personal\Diploma\datasets\data+y\{name}.kMeans.txt", parsed, kMeans.ClusterLabels);

ValidateIndexes(parsed, fuzzyCMeans.GetClusterAssignments(), fuzzyCMeans.GetCentroids());
WriteResultsToFile($@"C:\Work\personal\Diploma\datasets\data+y\{name}.fuzzyCMeans.txt", parsed, fuzzyCMeans.GetClusterAssignments());

var kMeans_fuzzyCMeans = randIndex.Calculate(kMeans.ClusterLabels, fuzzyCMeans.GetClusterAssignments());
using var writer_r = File.CreateText($@"C:\Work\personal\Diploma\datasets\data+y\{name}.randIndex.txt");
writer_r.WriteLine($"kMeans_fuzzyCMeans: {kMeans_fuzzyCMeans}");
writer_r.Close();



void ValidateIndexes(double[][] allData, int[] clusters, double[][] centroids)
{
    indexValidations.Clear();
    var chValuation = ch.Calculate(allData, clusters, centroids);
    indexValidations.Add("CalinskiHarabaszIndex", chValuation);

    var dbValuation = db.Calculate(centroids, allData, clusters);
    indexValidations.Add("DaviesBouldinIndex", dbValuation);

    var cValuation = cIndex.Calculate(allData, clusters);
    indexValidations.Add("CIndexCalculatorIndex", cValuation);

    var sValuation = sIndex.Calculate(allData, clusters);
    indexValidations.Add("SilhouetteIndex", sValuation);

    var hValuation = hubertIndex.Calculate(allData, clusters);
    indexValidations.Add("HubertIndex", hValuation);
}

void WriteResultsToFile(string path, double[][] data, int[] clusters)
{
    using var sw = File.CreateText(path);
    var results = data.Select((element, index) => new DataResults(element[0], element[1], clusters[index]));
    var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
    csv.WriteRecords(results);
    sw.Close();
    using var resultsWriter = File.CreateText($"{path}.index.txt");
    foreach (var indexValidation in indexValidations)
    {
        resultsWriter.WriteLine($"{indexValidation.Key}: {indexValidation.Value}");
    }
    resultsWriter.Close();
}