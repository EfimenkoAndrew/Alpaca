using System;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClusterKMeans
{
    record Data([Index(0)] double x, [Index(1)] double y, [Index(2)] string id);
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
            
            Console.WriteLine("\nClustering with k=3 ");
            KMeans km = new KMeans(parsed, 3);
            Console.WriteLine("Done ");

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

        // ------------------------------------------------------

        static void MatShow(double[][] m, int dec, int wid)
        {
            for (int i = 0; i < m.Length; ++i)
            {
                for (int j = 0; j < m[0].Length; ++j)
                {
                    double v = m[i][j];
                    Console.Write(v.ToString("F" + dec).
                      PadLeft(wid));
                }
                Console.WriteLine("");
            }
        }

        // ------------------------------------------------------

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

    public class KMeans
    {
        public double[][] data;
        public int k;
        public int N;
        public int dim;
        public int trials;  // to find best
        public int maxIter; // inner loop
        public Random rnd;
        public int[] clustering;
        public double[][] means;

        public KMeans(double[][] data, int k)
        {
            this.data = data;  // by ref
            this.k = k;
            this.N = data.Length;
            this.dim = data[0].Length;
            this.trials = N;  // for Cluster()
            this.maxIter = N * 2;  // for ClusterOnce()
            this.Initialize(0); // seed, means, clustering
        }

        public void Initialize(int seed)
        {
            this.rnd = new Random(seed);
            this.clustering = new int[this.N];
            this.means = new double[this.k][];
            for (int i = 0; i < this.k; ++i)
                this.means[i] = new double[this.dim];
            // Random Partition (not Forgy)
            int[] indices = new int[this.N];
            for (int i = 0; i < this.N; ++i)
                indices[i] = i;
            Shuffle(indices);
            for (int i = 0; i < this.k; ++i)  // first k items
                this.clustering[indices[i]] = i;
            for (int i = this.k; i < this.N; ++i)
                this.clustering[indices[i]] =
                  this.rnd.Next(0, this.k); // remaining items
                                            // VecShow(this.clustering, 4);
            this.UpdateMeans();
        }

        private void Shuffle(int[] indices)
        {
            int n = indices.Length;
            for (int i = 0; i < n; ++i)
            {
                int r = this.rnd.Next(i, n);
                int tmp = indices[i];
                indices[i] = indices[r];
                indices[r] = tmp;
            }
        }
        private static double SumSquared(double[] v1,
          double[] v2)
        {
            int dim = v1.Length;
            double sum = 0.0;
            for (int i = 0; i < dim; ++i)
                sum += (v1[i] - v2[i]) * (v1[i] - v2[i]);
            return sum;
        }

        private static double Distance(double[] item,
          double[] mean)
        {
            double ss = SumSquared(item, mean);
            return Math.Sqrt(ss);
        }

        private static int ArgMin(double[] v)
        {
            int dim = v.Length;
            int minIdx = 0;
            double minVal = v[0];
            for (int i = 0; i < v.Length; ++i)
            {
                if (v[i] < minVal)
                {
                    minVal = v[i];
                    minIdx = i;
                }
            }
            return minIdx;
        }

        private static bool AreEqual(int[] a1, int[] a2)
        {
            int dim = a1.Length;
            for (int i = 0; i < dim; ++i)
                if (a1[i] != a2[i]) return false;
            return true;
        }

        private static int[] Copy(int[] arr)
        {
            int dim = arr.Length;
            int[] result = new int[dim];
            for (int i = 0; i < dim; ++i)
                result[i] = arr[i];
            return result;
        }

        public bool UpdateMeans()
        {
            // verify no zero-counts
            int[] counts = new int[this.k];
            for (int i = 0; i < this.N; ++i)
            {
                int cid = this.clustering[i];
                ++counts[cid];
            }
            for (int kk = 0; kk < this.k; ++kk)
            {
                if (counts[kk] == 0)
                    throw
                      new Exception("0-count in UpdateMeans()");
            }

            // compute proposed new means
            for (int kk = 0; kk < this.k; ++kk)
                counts[kk] = 0;  // reset
            double[][] newMeans = new double[this.k][];
            for (int i = 0; i < this.k; ++i)
                newMeans[i] = new double[this.dim];
            for (int i = 0; i < this.N; ++i)
            {
                int cid = this.clustering[i];
                ++counts[cid];
                for (int j = 0; j < this.dim; ++j)
                    newMeans[cid][j] += this.data[i][j];
            }
            for (int kk = 0; kk < this.k; ++kk)
                if (counts[kk] == 0)
                    return false;  // bad attempt to update

            for (int kk = 0; kk < this.k; ++kk)
                for (int j = 0; j < this.dim; ++j)
                    newMeans[kk][j] /= counts[kk];

            // copy new means
            for (int kk = 0; kk < this.k; ++kk)
                for (int j = 0; j < this.dim; ++j)
                    this.means[kk][j] = newMeans[kk][j];

            return true;
        } // UpdateMeans()

        public bool UpdateClustering()
        {
            // verify no zero-counts
            int[] counts = new int[this.k];
            for (int i = 0; i < this.N; ++i)
            {
                int cid = this.clustering[i];
                ++counts[cid];
            }
            for (int kk = 0; kk < this.k; ++kk)
            {
                if (counts[kk] == 0)
                    throw new
                      Exception("0-count in UpdateClustering()");
            }

            // proposed new clustering
            int[] newClustering = new int[this.N];
            for (int i = 0; i < this.N; ++i)
                newClustering[i] = this.clustering[i];

            double[] distances = new double[this.k];
            for (int i = 0; i < this.N; ++i)
            {
                for (int kk = 0; kk < this.k; ++kk)
                {
                    distances[kk] =
                      Distance(this.data[i], this.means[kk]);
                    int newID = ArgMin(distances);
                    newClustering[i] = newID;
                }
            }

            if (AreEqual(this.clustering, newClustering) == true)
                return false;  // no change; short-circuit

            // make sure no count went to 0
            for (int i = 0; i < this.k; ++i)
                counts[i] = 0;  // reset
            for (int i = 0; i < this.N; ++i)
            {
                int cid = newClustering[i];
                ++counts[cid];
            }
            for (int kk = 0; kk < this.k; ++kk)
                if (counts[kk] == 0)
                    return false;  // bad update attempt

            // no 0 counts so update
            for (int i = 0; i < this.N; ++i)
                this.clustering[i] = newClustering[i];

            return true;
        } // UpdateClustering()

        public int[] ClusterOnce()
        {
            bool ok = true;
            int sanityCt = 1;
            while (sanityCt <= this.maxIter)
            {
                if ((ok = this.UpdateClustering() == false)) break;
                if ((ok = this.UpdateMeans() == false)) break;
                ++sanityCt;
            }
            return this.clustering;
        } // ClusterOnce()

        public double WCSS()
        {
            // within-cluster sum of squares
            double sum = 0.0;
            for (int i = 0; i < this.N; ++i)
            {
                int cid = this.clustering[i];
                double[] mean = this.means[cid];
                double ss = SumSquared(this.data[i], mean);
                sum += ss;
            }
            return sum;
        }

        public int[] Cluster()
        {
            double bestWCSS = this.WCSS();  // initial clustering
            int[] bestClustering = Copy(this.clustering);

            for (int i = 0; i < this.trials; ++i)
            {
                this.Initialize(i);  // new seed, means, clustering
                int[] clustering = this.ClusterOnce();
                double wcss = this.WCSS();
                if (wcss < bestWCSS)
                {
                    bestWCSS = wcss;
                    bestClustering = Copy(clustering);
                }
            }
            return bestClustering;
        } // Cluster()

    } // class KMeans
} // ns
