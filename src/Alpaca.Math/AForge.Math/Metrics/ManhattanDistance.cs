﻿// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

using Alpaca.Math.Distances;

namespace Alpaca.Math.AForge.Math.Metrics
{
    /// <summary>
    ///   Please use Alpaca.Math.Distances.Manhattan instead.
    /// </summary>
    [Obsolete("Please use Alpaca.Math.Distances.Manhattan instead.")]
    public sealed class ManhattanDistance : IDistance
    {
        /// <summary>
        ///   Please use Alpaca.Math.Distances.Manhattan instead.
        /// </summary>
        public double GetDistance(double[] p, double[] q)
        {
            return new Manhattan().Distance(p, q);
        }
    }
}
