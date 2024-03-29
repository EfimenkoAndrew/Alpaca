﻿using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UnicornAnalytics.Clustering;
using UnicornAnalytics.Indexes.External;
using UnicornAnalytics.Indexes.Internal;
using UpdatedEvaluations;

var name = "3";
var path = $@"C:\Work\personal\Diploma\datasets\data+y\{name}.txt";
using var streamReader = new StreamReader(path);
using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false});

var data = csv.GetRecords<Data>();
var parsed = data.Select(x => new[] { x.x, x.y }).ToArray();
var points = parsed.Select(x => new Point(x)).ToArray();
var sim = SimilarityMatrix.SparseSimilarityMatrix(points);

KMeans kMeans = new KMeans();
kMeans.Fit(parsed, 2);

FuzzyCMeans fuzzyCMeans = new FuzzyCMeans(2, 10.0);
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
CIndexIndex cIndex = new CIndexIndex(); 
SilhouetteIndex sIndex = new SilhouetteIndex();

var dat = new double[][]
{
    [1d,1d],
    [2d,3d],
    [3d,2d],
    [2d,2d],
    [6d,5d],
    [7d,4d],
    [7d,5d],
    [8d,4d],
};
var clasts = new int[] { 0, 1, 1, 1, 2, 2, 2, 2 };
var d = sIndex.Calculate(dat, clasts);

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
    var chValuation = ch.Calculate(allData, clusters);
    indexValidations.Add("CalinskiHarabaszIndex", chValuation);

    var dbValuation = db.Calculate(allData, clusters);
    indexValidations.Add("DaviesBouldinIndex", dbValuation);

    var cValuation = cIndex.Calculate(allData, clusters);
    indexValidations.Add("CIndexIndex", cValuation);

    var sValuation = sIndex.Calculate(allData, clusters);
    indexValidations.Add("SilhouetteIndex", sValuation);
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