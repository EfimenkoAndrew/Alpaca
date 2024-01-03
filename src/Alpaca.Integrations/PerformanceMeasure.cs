using System.Diagnostics;

namespace Alpaca.Integrations;

public class PerformanceMeasure
{
    private readonly Stopwatch _timer = new();
    private long _memoryStart;


    public PerformanceMeasure()
    {
        TimeElapsed = new TimeSpan();
    }


    public long MemoryUsage { get; protected set; }

    public TimeSpan TimeElapsed { get; protected set; }


    public virtual void Start()
    {
        //starts measures (time and memory)
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        _memoryStart = Process.GetCurrentProcess().PrivateMemorySize64;
        _timer.Start();
    }

    public virtual void Stop()
    {
        //stops timers and measures
        _timer.Stop();
        var memoryEnd = Process.GetCurrentProcess().PrivateMemorySize64;

        TimeElapsed = _timer.Elapsed;
        MemoryUsage += memoryEnd - _memoryStart;
    }

    public void Reset()
    {
        // "zero"s all measures
        _timer.Stop();
        MemoryUsage = 0;
        TimeElapsed = new TimeSpan();
    }

    public override string ToString()
    {
        return $"time elapsed: {TimeElapsed}, memory spent: {BytesToString(MemoryUsage)}";
    }

    /// <remarks>From <a href="http://stackoverflow.com/a/4975942" /></remarks>
    private static string BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        var bytes = Math.Abs(byteCount);
        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        var num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return Math.Sign(byteCount) * num + suf[place];
    }
}