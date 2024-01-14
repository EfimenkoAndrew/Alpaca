using System.Globalization;
using CsvHelper;
using UnicornAnalytics.Clustering;
using UnicornAnalytics.Indexes.Internal;
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
CalinskiHarabaszIndex ch = new CalinskiHarabaszIndex(); 
DaviesBouldinIndex db = new DaviesBouldinIndex(); 
CIndexCalculatorIndex cIndex = new CIndexCalculatorIndex(); 
SilhouetteIndex sIndex = new SilhouetteIndex();

var chValuation = ch.Calculate(km.Centroids, parsed, km.ClusterLabels);
Console.WriteLine(chValuation);

var dbValuation = db.Calculate(km.Centroids, parsed, km.ClusterLabels);
Console.WriteLine(dbValuation);

var cValuation = cIndex.Calculate(parsed, km.ClusterLabels);
Console.WriteLine(cValuation);

var sValuation = sIndex.Calculate(parsed, km.ClusterLabels);
Console.WriteLine(sValuation);

