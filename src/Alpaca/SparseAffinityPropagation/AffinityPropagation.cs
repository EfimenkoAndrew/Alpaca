﻿using System;
using System.Collections.Generic;

namespace Alpaca.SparseAffinityPropagation
{
    public class AffinityPropagation
    {
        private readonly int _convergence;
        private readonly float _damping;
        private readonly Graph _graph;
        private readonly int _maxIteration;

        public AffinityPropagation(
            int numberOfPoints,
            float damping = 0.9f,
            int maxIteration = 1000,
            int convergence = 200)
        {
            if (numberOfPoints < 1)
                throw new ArgumentOutOfRangeException("Number of points can't be 0 or a negative value");

            _graph = new Graph(numberOfPoints);
            _damping = damping;
            _maxIteration = maxIteration;
            _convergence = convergence;
            Centers = new HashSet<int>();
        }

        public HashSet<int> Centers { get; }

        private float __preference()
        {
            var m = _graph.SimMatrixElementsCount - _graph.VerticesCount - 1;
            //get the middle element of the array with quickselect without sorting the array 
            var s = Algorithms.k2thSmallest(ref _graph.Edges, 0, m, m / 2 + 1);
            return Convert.ToSingle(m % 2 == 0 ? (s[0] + s[1]) / 2 : s[0]);
        }

        private void __build_graph(Edge[] points)
        {
            var rand = new Random();
            _graph.Edges = points;
            var _preference = __preference();

            for (var i = 0; i < _graph.VerticesCount; ++i)
                _graph.Edges[_graph.Edges.Length - (_graph.VerticesCount - i)] = new Edge(i, i, _preference);

            var indexes_source = new int[_graph.VerticesCount];
            var indexes_destination = new int[_graph.VerticesCount];
            for (var i = 0; i < _graph.Edges.Length; ++i)
            {
                var p = _graph.Edges[i];
                //Add noise to avoid degeneracies
                p.Similarity +=
                    Convert.ToSingle((1e-16 * p.Similarity + 1e-300) * (rand.Next() / (int.MaxValue + 1.0)));

                //add out/in edges to vertices
                _graph.outEdges[p.Source][indexes_source[p.Source]] = p;
                _graph.inEdges[p.Destination][indexes_destination[p.Destination]] = p;
                ++indexes_source[p.Source];
                ++indexes_destination[p.Destination];
            }

            Console.WriteLine("Graph Constructed");
        }

        private void __update(ref float variable, float newValue)
        {
            variable = Convert.ToSingle(_damping * variable + (1.0 - _damping) * newValue);
        }

        private void __update_responsabilities()
        {
            Edge[] edges;
            float max1, max2, argmax1;
            var Similarity = 0.0f;
            for (var i = 0; i < _graph.VerticesCount; ++i)
            {
                edges = _graph.outEdges[i];
                max1 = -float.PositiveInfinity;
                max2 = -float.PositiveInfinity;
                argmax1 = -1;
                for (var k = 0; k < edges.Length; k++)
                {
                    Similarity = edges[k].Similarity + edges[k].Availability;
                    if (Similarity > max1)
                    {
                        Algorithms.Swap(ref max1, ref Similarity);
                        argmax1 = k;
                    }

                    if (Similarity > max2) max2 = Similarity;
                }

                //Update the Responsability
                var temp = 0.0f;
                for (var k = 0; k < edges.Length; ++k)
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

        private void __update_availabilities()
        {
            Edge[] edges;
            float sum = 0.0f, temp = 0.0f, temp1 = 0.0f, last = 0.0f;

            for (var k = 0; k < _graph.VerticesCount; ++k)
            {
                edges = _graph.inEdges[k];
                //calculate sum of positive responsabilities
                sum = 0.0f;
                for (var i = 0; i < edges.Length - 1; ++i)
                    sum += Math.Max(0.0f, edges[i].Responsability);

                //calculate the availabilities
                last = edges[edges.Length - 1].Responsability;
                for (var i = 0; i < edges.Length - 1; ++i)
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
            var changed = false;
            Edge[] edges;
            float Similarity = 0.0f, maxValue = 0.0f;
            int argmax;
            for (var i = 0; i < _graph.VerticesCount; ++i)
            {
                edges = _graph.outEdges[i];
                maxValue = -float.PositiveInfinity;
                argmax = i;
                for (var k = 0; k < edges.Length; ++k)
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
            var examplar = new int[_graph.VerticesCount];
            for (var i = 0; i < _graph.VerticesCount; ++i)
                examplar[i] = -1;

            for (int i = 0, nochange = 0; i < _maxIteration && nochange < _convergence; ++i, ++nochange)
            {
                __update_responsabilities();
                __update_availabilities();
                if (__update_examplars(examplar))
                    nochange = 0;
            }

            return examplar;
        }
    }
}