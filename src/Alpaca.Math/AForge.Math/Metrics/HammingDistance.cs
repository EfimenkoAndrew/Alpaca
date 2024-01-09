﻿// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

using Accord.Math.Distances;

namespace Alpaca.Math.AForge.Math.Metrics
{
    /// <summary>
    ///   Please use Accord.Math.Distances.Hamming instead.
    /// </summary>
    [Obsolete("Please use Accord.Math.Distances.Hamming instead.")]
    public sealed class HammingDistance : IDistance
    {
        /// <summary>
        ///   Please use Accord.Math.Distances.Hamming instead.
        /// </summary>
        public double GetDistance(double[] p, double[] q)
        {
            return new Hamming<double>().Distance(p, q);
        }
    }
}