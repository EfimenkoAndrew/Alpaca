using System;
using System.Collections.Generic;
using System.Linq;

namespace UnicornAnalytics.Clustering
{
    public class AffinityPropagation
    {
        private int _max_iteration, _convergence;
        private double _damping;
        private Graph _graph;
        public HashSet<int> Centers { get; private set; }

        public AffinityPropagation(int number_of_points, double damping = 0.9f, int max_iteration = 1000,
            int convergence = 200)
        {
            if (number_of_points < 1)
                throw new ArgumentOutOfRangeException("Number of points can't be 0 or a negative value");

            _graph = new Graph(number_of_points);
            _damping = damping;
            _max_iteration = max_iteration;
            _convergence = convergence;
            Centers = new HashSet<int>();
        }

        private double __preference()
        {

            int m = _graph.SimMatrixElementsCount - _graph.VerticesCount - 1;
            //get the middle element of the array with quickselect without sorting the array 
            var s = k2thSmallest(ref _graph.Edges, 0, m, (m / 2) + 1);
            return Convert.ToSingle(m % 2 == 0 ? ((s[0] + s[1]) / 2) : s[0]);

        }

        public double[] k2thSmallest(ref Edge[] a, int left, int right, int k)
        {

            double[] s = new double[2];
            int temp = 0;
            while (left <= right)
            {

                // Partition a[left..right] around a pivot
                // and find the position of the pivot
                int pivotIndex = qpartition(a, left, right);

                // If pivot itself is the k-th smallest element
                if (pivotIndex == k - 1)
                {
                    s[1] = a[pivotIndex].Similarity;
                    s[0] = a[temp].Similarity;

                    return s;
                }

                // If there are more than k-1 elements on
                // left of pivot, then k-th smallest must be
                // on left side.
                else if (pivotIndex > k - 1)
                    right = pivotIndex - 1;

                // Else k-th smallest is on right side.
                else
                    left = pivotIndex + 1;

                temp = pivotIndex;
            }
            return null;
        }

        private int qpartition(Edge[] arr, int low, int high)
        {
            Edge temp;
            Edge pivot = arr[high];
            int i = (low - 1);
            for (int j = low; j <= high - 1; j++)
            {
                if (arr[j].Similarity <= pivot.Similarity)
                {
                    i++;
                    temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }

            temp = arr[i + 1];
            arr[i + 1] = arr[high];
            arr[high] = temp;

            return (i + 1);
        }

        private void __build_graph(Edge[] points)
        {
            Random rand = new Random();
            _graph.Edges = points;
            double _preference = __preference();

            for (int i = 0; i < _graph.VerticesCount; ++i)
            {
                _graph.Edges[_graph.Edges.Length - (_graph.VerticesCount - i)] = new Edge(i, i, _preference);
            }

            int[] indexes_source = new int[_graph.VerticesCount];
            int[] indexes_destination = new int[_graph.VerticesCount];
            for (int i = 0; i < _graph.Edges.Length; ++i)
            {
                Edge p = _graph.Edges[i];
                //Add noise to avoid degeneracies
                p.Similarity +=
                    Convert.ToSingle((1e-16 * p.Similarity + 1e-300) * (rand.Next() / (Int32.MaxValue + 1.0)));

                //add out/in edges to vertices
                _graph.outEdges[p.Source][indexes_source[p.Source]] = p;
                _graph.inEdges[p.Destination][indexes_destination[p.Destination]] = p;
                ++indexes_source[p.Source];
                ++indexes_destination[p.Destination];
            }
        }

        private void __update(ref double variable, double newValue)
        {
            variable = Convert.ToSingle(_damping * variable + (1.0 - _damping) * newValue);
        }

        private void __update_responsabilities()
        {
            Edge[] edges;
            double max1, max2, argmax1;
            double Similarity = 0.0f;
            for (int i = 0; i < _graph.VerticesCount; ++i)
            {
                edges = _graph.outEdges[i];
                max1 = -Single.PositiveInfinity;
                max2 = -Single.PositiveInfinity;
                argmax1 = -1;
                for (int k = 0; k < edges.Length; k++)
                {
                    Similarity = edges[k].Similarity + edges[k].Availability;
                    if (Similarity > max1)
                    {
                        (max1, Similarity) = (Similarity ,max1);
                        argmax1 = k;
                    }

                    if (Similarity > max2)
                    {
                        max2 = Similarity;
                    }

                }

                //Update the Responsability
                double temp = 0.0f;
                for (int k = 0; k < edges.Length; ++k)
                {
                    if (k != argmax1)
                    {
                        temp = edges[k].Responsability;
                        __update(ref temp, edges[k].Similarity - max1);
                        edges[k].Responsability = temp;
                    }
                    else
                    {
                        temp = edges[k].Responsability;
                        __update(ref temp, edges[k].Similarity - max2);
                        edges[k].Responsability = temp;
                    }
                }
            }
        }

        private void __update_availabilities()
        {
            Edge[] edges;
            double sum = 0.0f, temp = 0.0f, temp1 = 0.0f, last = 0.0f;

            for (int k = 0; k < _graph.VerticesCount; ++k)
            {
                edges = _graph.inEdges[k];
                //calculate sum of positive responsabilities
                sum = 0.0f;
                for (int i = 0; i < edges.Length - 1; ++i)
                    sum += Math.Max(0.0f, edges[i].Responsability);

                //calculate the availabilities
                last = edges[edges.Length - 1].Responsability;
                for (int i = 0; i < edges.Length - 1; ++i)
                {
                    temp1 = edges[i].Availability;
                    __update(ref temp1, Math.Min(0.0f, last + sum - Math.Max(0.0f, edges[i].Responsability)));

                    edges[i].Availability = temp1;
                }

                //calculate self-Availability
                temp = edges[edges.Length - 1].Availability;
                __update(ref temp, sum);
                edges[edges.Length - 1].Availability = temp;
            }
        }

        private bool __update_examplars(int[] examplar)
        {
            bool changed = false;
            Edge[] edges;
            double Similarity = 0.0f, maxValue = 0.0f;
            int argmax;
            for (int i = 0; i < _graph.VerticesCount; ++i)
            {
                edges = _graph.outEdges[i];
                maxValue = -Single.PositiveInfinity;
                argmax = i;
                for (int k = 0; k < edges.Length; ++k)
                {
                    Similarity = edges[k].Availability + edges[k].Responsability;

                    if (Similarity > maxValue)
                    {
                        maxValue = Similarity;
                        argmax = edges[k].Destination;
                    }
                }

                if (examplar[i] != argmax)
                {
                    examplar[i] = argmax;
                    changed = true;
                    Centers.Clear();
                }

                Centers.Add(argmax);
            }

            return changed;
        }

        public int[] Fit(Edge[] input)
        {
            if (input.Length != _graph.SimMatrixElementsCount)
                throw new Exception(
                    $"The provided array size mismatch with the size given in the constructor  ({input.Length}!={_graph.SimMatrixElementsCount})");

            __build_graph(input);
            int[] examplar = new int[_graph.VerticesCount];
            for (int i = 0; i < _graph.VerticesCount; ++i)
                examplar[i] = -1;

            for (int i = 0, nochange = 0; i < _max_iteration && nochange < _convergence; ++i, ++nochange)
            {
                __update_responsabilities();
                __update_availabilities();
                if (__update_examplars(examplar))
                    nochange = 0;
            }

            return examplar;
        }
    }
    public class Point
    {
        protected double[] _coordinates;

        public Point Center { get; set; }
        public Point(params double[] coordinates)
        {
            Data = new double[coordinates.Length];
            for (int i = 0; i < coordinates.Length; ++i)
                Data[i] = coordinates[i];

            Center = null;
        }
        public int Dimension { get { return Data.Length; } }

        public double[] Data
        {
            get => _coordinates;
            set => _coordinates = value;
        }

        public double Coordinates(int index) { return Data[index]; }

        public bool Equals(Point obj)
        {
            // Perform an equality check on two rectangles (Point object pairs).
            if (obj == null || GetType() != obj.GetType() || obj.Dimension != Dimension)
                return false;

            for (int i = 0; i < Dimension; ++i)
            {
                if (Data[i] != obj.Data[i])
                    return false;
            }
            return true;
        }


    }
    public class Edge : IComparable<Edge>
    {
        public int Source { get; set; }
        public int Destination { get; set; }
        public double Similarity { get; set; }
        public double Responsability { get; set; }
        public double Availability { get; set; }

        public Edge()
        {
            Source = Destination = 0;
            Similarity = Responsability = Availability = 0.0f;
        }
        public Edge(int Source, int Destination, double Similarity)
        {
            this.Source = Source;
            this.Destination = Destination;
            this.Similarity = Similarity;
            this.Responsability = 0;
            this.Availability = 0;
        }
        public int CompareTo(Edge obj)
        {
            return Similarity.CompareTo(obj.Similarity);
        }
    }

    public class Graph
    {
        public int VerticesCount { get; private set; }
        public int SimMatrixElementsCount;

        public Edge[][] outEdges;
        public Edge[][] inEdges;
        public Edge[] Edges;


        public Graph(int vertices)
        {
            VerticesCount = vertices < 0 ? 0 : vertices;
            SimMatrixElementsCount = ((VerticesCount - 1) * VerticesCount) + VerticesCount;

            outEdges = new Edge[VerticesCount][];
            inEdges = new Edge[VerticesCount][];
            Edges = new Edge[SimMatrixElementsCount];

            for (int i = 0; i < VerticesCount; ++i)
            {
                outEdges[i] = new Edge[VerticesCount];
                inEdges[i] = new Edge[VerticesCount];

            }

        }

    }


    public static class Distance
    {
        public static double NegEuclidienDistance(Point x, Point y)
        {   //checking for dim x == dim y will hurt performance this should be done at init
            double f = 0.0f;
            for (int i = 0; i < x.Dimension; ++i)
                f += ((y.Coordinates(i) - x.Coordinates(i)) * (y.Coordinates(i) - x.Coordinates(i)));

            return -1 * f;

        }
    }
    public static class SimilarityMatrix
    {
        public static Edge[] SparseSimilarityMatrix(Point[] point, Func<Point, Point, double> distance)
        {
            /// Create the similarity matrix with a user defined distance measure
            Edge[] items = new Edge[point.Length * point.Length];
            int p = 0;
            for (int i = 0; i < point.Length - 1; i++)
                for (int j = i + 1; j < point.Length; j++)
                {
                    items[p] = new Edge(i, j, distance(point[i], point[j]));
                    items[p + 1] = new Edge(j, i, distance(point[i], point[j]));
                    p += 2;
                }
            return items;
        }

        public static Edge[] SparseSimilarityMatrix(Point[] point)
        {
            Edge[] items = new Edge[point.Length * point.Length];
            int p = 0;
            for (int i = 0; i < point.Length - 1; i++)
                for (int j = i + 1; j < point.Length; j++)
                {
                    items[p] = new Edge(i, j, Distance.NegEuclidienDistance(point[i], point[j]));
                    items[p + 1] = new Edge(j, i, Distance.NegEuclidienDistance(point[i], point[j]));
                    p += 2;
                }
            return items;
        }
    }

    public class ClusterUtility
    {
        public static List<(Point, int)> GroupClusters(Point[] points, int[] centers, int[] CentersIndecies)
        {
           var result = new List<(Point, int)>();

            for (int i = 0; i < points.Length; ++i)
            {
                for (int j = 0; j < CentersIndecies.Length; ++j)
                {
                    if (points[i].Center == null && points[i].Equals(points[CentersIndecies[j]]))
                    {
                        result.Add((points[i], j));
                    }
                    else if (points[i].Center != null && points[i].Center.Equals(points[CentersIndecies[j]]))
                    {
                        result.Add((points[i], j));
                    }
                }
            }
            return result;

        }
        public static void AssignClusterCenters(Point[] input, int[] result)
        {
            // assign the center for each point
            // if the point itself is the center of the cluster then
            // assign null for its center value

            for (int i = 0; i < result.Length; ++i)
            {
                if (!input[i].Equals(input[result[i]]))
                    input[i].Center = input[result[i]];
                else
                    input[i].Center = null;
            }
        }
    }
}
