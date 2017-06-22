using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using ChartApp.Actors;
using CountersDictionary = System.Collections.Generic.Dictionary<ChartApp.Actors.CounterType, System.Func<System.Diagnostics.PerformanceCounter>>;
using SeriesDictionary = System.Collections.Generic.Dictionary<ChartApp.Actors.CounterType, System.Func<System.Windows.Forms.DataVisualization.Charting.Series>>;

namespace ChartApp
{
    public static class Generators
    {
        private static readonly string CpuTypeName = CounterType.Cpu.ToString();

        private static readonly string MemoryTypeName = CounterType.Memory.ToString();

        private static readonly string DiskTypeName = CounterType.Disk.ToString();

        public static readonly CountersDictionary CounterGenerators = new CountersDictionary
        {
            {CounterType.Cpu, CpuCounterGenerator},
            {CounterType.Memory, MemoryCounterGenerator},
            {CounterType.Disk, DiskCounterGenerator},
        };

        public static readonly SeriesDictionary SeriesGenerators = new SeriesDictionary
        {
            {CounterType.Cpu, CpuSeriesGenerator},
            {CounterType.Memory, MemorySeriesGenerator},
            {CounterType.Disk, DiskSeriesGenerator},
        };

        private static PerformanceCounter CpuCounterGenerator() => new PerformanceCounter("Processor", "% Processor Time", "_Total", true);

        private static PerformanceCounter MemoryCounterGenerator() => new PerformanceCounter("Memory", "% Committed Bytes In Use", true);

        private static PerformanceCounter DiskCounterGenerator() => new PerformanceCounter("LogicalDisk", "% Disk Time", "_Total", true);

        private static Series CpuSeriesGenerator() => new Series(CpuTypeName)
        {
            ChartType = SeriesChartType.SplineArea,
            Color = Color.DarkGreen
        };

        private static Series MemorySeriesGenerator() => new Series(MemoryTypeName)
        {
            ChartType = SeriesChartType.FastLine,
            Color = Color.MediumBlue
        };

        private static Series DiskSeriesGenerator() => new Series(DiskTypeName)
        {
            ChartType = SeriesChartType.SplineArea,
            Color = Color.DarkRed
        };
    }
}