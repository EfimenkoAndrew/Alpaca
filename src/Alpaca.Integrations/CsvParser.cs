using System.Text;

namespace Alpaca.Integrations;

public class CsvParser
{
    private readonly char[] _separator;


    public CsvParser(char separator = ',')
    {
        _separator = new[] { separator };
    }


    public ISet<DataPoint> Load(string filePath)
    {
        return !File.Exists(filePath) ? null : LoadData(filePath);
    }

    public void Save(string filePath, ISet<DataPoint> dataPoints)
    {
        // writes a line for each given transaction, converts to csv
        using (var fs = new FileStream(filePath, FileMode.Create))
        using (var sw = new StreamWriter(fs, Encoding.UTF8) { AutoFlush = true })
        {
            foreach (var transaction in dataPoints)
                sw.WriteLine(GetDataPointString(transaction));
        }
    }


    private string GetDataPointString(DataPoint dataPoint)
    {
        var sb = new StringBuilder();
        foreach (var val in dataPoint.Value)
            sb.Append($"{val}{_separator[0]}");
        sb.Append(dataPoint.ID);
        return sb.ToString();
    }

    private ISet<DataPoint> LoadData(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        var dataPoints = new HashSet<DataPoint>();
        using (var fs = new FileStream(filePath, FileMode.Open))
        using (var sr = new StreamReader(fs))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                // checks empty / incorrect line
                var fields = line.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length == 0) continue;

                // tries to convert all fields into numbers
                var list = new List<double>();
                for (var i = 0; i < fields.Length - 1; i++)
                {
                    var field = fields[i];
                    if (double.TryParse(field, out var val)) list.Add(val);
                }

                // creates data-point, assume last field is ID
                if (list.Count > 0) dataPoints.Add(new DataPoint(fields[fields.Length - 1], list.ToArray()));
            }
        }

        return dataPoints;
    }
}