using System.Globalization;
using AlpacaAnalytics.Clustering;
using AlpacaAnalytics.Indexes.Internal;
using CsvHelper;
using UpdatedEvaluations;

var path = @"C:\Work\personal\Diploma\datasets\data+y\4.txt";
using var streamReader = new StreamReader(path);
using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
csv.Read();
csv.ReadHeader();

//var data = csv.GetRecords<Data>();
var data = csv.GetRecords<Data>();
var parsed = data.Select(x => new[] { x.x, x.y }).ToArray();

KMeans km = new KMeans();
km.Fit(parsed, 10);
CalinskiHarabaszIndex ch = new CalinskiHarabaszIndex(); // higher better
DaviesBouldinIndex db = new DaviesBouldinIndex(); // 0 1 (1 - better)
CIndexCalculator cIndex = new CIndexCalculator(); // 0 1 (0 better)
SilhouetteIndex sIndex = new SilhouetteIndex(); // -1 1 (1 better)

var chValuation = ch.Calculate(km.Centroids, parsed, km.ClusterLabels);
Console.WriteLine(chValuation);

var dbValuation = db.Calculate(km.Centroids, parsed, km.ClusterLabels);
Console.WriteLine(dbValuation);

var cValuation = cIndex.Calculate(parsed, km.ClusterLabels);
Console.WriteLine(cValuation);

var sValuation = sIndex.Calculate(parsed, km.ClusterLabels);
Console.WriteLine(sValuation);

