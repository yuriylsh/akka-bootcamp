using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        public const int MaxPoints = 250;

        private int xPosCounter = 0;

        private readonly Chart _chart;

        private static readonly SeriesByNameComparer SeriesComparer = new SeriesByNameComparer();

        private readonly HashSet<Series> _seriesIndex = new HashSet<Series>(SeriesComparer);

        public ChartingActor(Chart chart) : this(chart, Array.Empty<Series>())
        {
        }

        private ChartingActor(Chart chart, ICollection<Series> seriesIndex)
        {
            _chart = chart;
            SetSeries(seriesIndex);

            Receive<InitializeChart>(ic => HandleInitialize(ic));
            Receive<AddSeries>(addSeries => HandleAddSeries(addSeries));
            Receive<RemoveSeries>(removeSeries => HandleRemoveSeries(removeSeries));
            Receive<Metric>(metric => HandleMetrics(metric));
        }

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                SetSeries(ic.InitialSeries);
            }
            _chart.Series.Clear();
            SetTheAxesUp();
            SetChartBoundaries();
            foreach (var series in _seriesIndex)
            {
                _chart.Series.Add(series);
            }
            SetChartBoundaries();
        }

        private void SetTheAxesUp()
        {
            var area = _chart.ChartAreas[0];
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisY.IntervalType = DateTimeIntervalType.Number;
        }

        private void HandleAddSeries(AddSeries series)
        {
            var seriesToAdd = series.Series;
            if(!string.IsNullOrEmpty(seriesToAdd.Name) && !_seriesIndex.Contains(seriesToAdd))
            {
                _seriesIndex.Add(seriesToAdd);
                _chart.Series.Add(seriesToAdd);
                SetChartBoundaries();
            }
        }

        private void HandleRemoveSeries(RemoveSeries series)
        {
            var seriesToRemove = GetSeriesFromIndexByName(series.SeriesName);
            if (seriesToRemove != null)
            {
                _seriesIndex.Remove(seriesToRemove);
                _chart.Series.Remove(seriesToRemove);
                SetChartBoundaries();
            }
        }

        private void HandleMetrics(Metric metric)
        {
            var points = GetSeriesFromIndexByName(metric.Series)?.Points;
            if (points != null)
            {
                points.AddXY(xPosCounter++, metric.CounterValue);
                while(points.Count > MaxPoints) points.RemoveAt(0);
                SetChartBoundaries();
            }
        }

        private Series GetSeriesFromIndexByName(string name)
            => _seriesIndex.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        private void SetSeries(ICollection<Series> series)
        {
            _seriesIndex.Clear();
            foreach (var s in series)
            {
                _seriesIndex.Add(s);
            }
        }

        private void SetChartBoundaries()
        {
            var allPoints = _seriesIndex.SelectMany(series => series.Points).ToArray();
            var yValues = allPoints.SelectMany(point => point.YValues).ToArray();
            double maxAxisX = xPosCounter;
            double minAxisX = xPosCounter - MaxPoints;
            var hasYValues = yValues.Length > 0;
            var maxAxisY = hasYValues ? Math.Ceiling(yValues.Max()) : 1.0d;
            var minAxisY = hasYValues ? Math.Floor(yValues.Min()) : 0.0d;
            if (allPoints.Length > 2)
            {
                var area = _chart.ChartAreas[0];
                area.AxisX.Minimum = minAxisX;
                area.AxisX.Maximum = maxAxisX;
                area.AxisY.Minimum = minAxisY;
                area.AxisY.Maximum = maxAxisY;
            }
        }

        public class InitializeChart
        {
            public InitializeChart(ICollection<Series> initialSeries) => InitialSeries = initialSeries;

            public ICollection<Series> InitialSeries { get; }
        }

        public class AddSeries
        {
            public Series Series { get; }

            public AddSeries(Series series)
            {
                Series = series;
            }
        }

        public class RemoveSeries
        {
            public string SeriesName { get; }

            public RemoveSeries(string seriesName) => SeriesName = seriesName;
        }

        private class SeriesByNameComparer : IEqualityComparer<Series>
        {
            public bool Equals(Series x, Series y) => x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(Series series) => series.Name.GetHashCode();
        }
    }
}
