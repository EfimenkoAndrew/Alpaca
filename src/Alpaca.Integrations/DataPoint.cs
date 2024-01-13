using System.Text;
using AlpacaAnalytics.Clustering;

namespace Alpaca.Integrations;

public class DataPoint : IEquatable<DataPoint>, IDissimilarityMetric<DataPoint>, IComparable<DataPoint>
{
    public DataPoint(string id, double[] value)
    {
        ID = id;
        Value = value;
    }

    public DataPoint()
    {
    }


    public string ID { get; }

    public double[] Value { get; }

    public int CompareTo(DataPoint other)
    {
        return string.Compare(ID, other.ID, StringComparison.Ordinal);
    }

    public double Calculate(DataPoint instance1, DataPoint instance2)
    {
        return instance1.DistanceTo(instance2);
    }

    public bool Equals(DataPoint other)
    {
        return string.Equals(ID, other.ID);
    }


    public override bool Equals(object obj)
    {
        return obj is DataPoint && Equals((DataPoint)obj);
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }

    public override string ToString()
    {
        return ID;
    }


    public static DataPoint GetCentroid(Cluster<DataPoint> cluster)
    {
        if (cluster.Count == 1) return cluster.First();

        // gets sum for all variables
        var id = new StringBuilder();
        var sums = new double[cluster.First().Value.Length];
        foreach (var dataPoint in cluster)
        {
            id.Append(dataPoint.ID);
            for (var i = 0; i < sums.Length; i++)
                sums[i] += dataPoint.Value[i];
        }

        // gets average of all variables (centroid)
        for (var i = 0; i < sums.Length; i++)
            sums[i] /= cluster.Count;

        return new DataPoint(id.ToString(), sums);
    }

    public static DataPoint GetMedoid(Cluster<DataPoint> cluster)
    {
        return cluster.GetMedoid(new DataPoint());
    }

    public static bool operator ==(DataPoint left, DataPoint right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DataPoint left, DataPoint right)
    {
        return !left.Equals(right);
    }

    public double DistanceTo(DataPoint other)
    {
        var sum2 = 0d;
        var length = Math.Min(Value.Length, other.Value.Length);
        for (var idx1 = 0; idx1 < length; ++idx1)
        {
            var delta = Value[idx1] - other.Value[idx1];
            sum2 += delta * delta;
        }

        return Math.Sqrt(sum2);
    }
}