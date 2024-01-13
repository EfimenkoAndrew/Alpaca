using System.Globalization;
using System.Text;
using Alpaca.Integrations;
using AlpacaAnalytics.Clustering;
using AlpacaAnalytics.Evaluation.Internal;
using AlpacaAnalytics.Linkage;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace EvaluationsTests
{
    record DataDemoResult([Index(0)] double x, [Index(1)] double y, [Index(2)] int id);
    record DataDemo([Index(0)] double x, [Index(1)] double y);

    internal class ClusterKMeansProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nBegin C# k-means ");

            //var path = @"C:\Work\personal\Diploma\datasets\aggregation.csv";
            var path = @"C:\Work\personal\Diploma\datasets\data+y\4.txt";
            using var streamReader = new StreamReader(path);
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();

            //var data = csv.GetRecords<Data>();
            var data = csv.GetRecords<DataDemo>();
            var parsed = data.Select(x => new[] { x.x, x.y }).ToArray();
            
            var km = new KMeans(parsed, 3); 
            var metric = new DataPoint();
            var linkage = new AverageLinkage<DataPoint>(metric);
            var algorithm = new AgglomerativeClusteringAlgorithm<DataPoint>(linkage);

            var result = algorithm.GetClustering(parsed.Select((x, i) => new DataPoint(i.ToString(), x)).ToHashSet());
            var c = result.Count;
            result.SaveToCsv(@"C:\Work\personal\Diploma\datasets\data+y\4.csv");

            int[] clustering = km.Cluster();
            Console.WriteLine("\nclustering = ");
            VecShow(clustering, parsed, 3);


            var resultPath = @"C:\Work\personal\Diploma\datasets\data+y\4-result.csv";
            using var streamWriter= new StreamWriter(resultPath);
            using var resultsWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            var results = parsed.Select((e,i) => new DataDemoResult(e[0], e[1], clustering[i]+1));
            
            resultsWriter.WriteRecords<DataDemoResult>(results); 
            resultsWriter.NextRecord();
            resultsWriter.Flush();
        } // Main
        
        static void VecShow(int[] vec, double[][] m, int wid)
        {
            for (int i = 0; i < m.Length; ++i)
            {
                for (int j = 0; j < m[0].Length; ++j)
                {
                    double v = m[i][j];
                    Console.Write(v.ToString("F" + 2).
                        PadLeft(wid));
                    Console.Write($" {vec[i].ToString().PadLeft(wid)}");
                }
                Console.WriteLine();
            }
        }
    } // Program

    // class KMeans
} // ns
