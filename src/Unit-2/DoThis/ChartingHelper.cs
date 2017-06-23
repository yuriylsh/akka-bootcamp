using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartApp
{
    public class ChartingHelper
    {
        public const int MaxPoints = 250;

        private static readonly SeriesByNameComparer SeriesComparer = new SeriesByNameComparer();

        private readonly HashSet<Series> _seriesIndex = new HashSet<Series>(SeriesComparer);

        private readonly Chart _chart;

        private int _xPosCounter;

        public ChartingHelper(Chart chart)
        {
            _chart = chart;
        }

        public void Initialize()
        {
            _chart.Series.Clear();
            SetTheAxesUp();
            SetChartBoundaries();
            foreach (var series in _seriesIndex)
            {
                _chart.Series.Add(series);
            }
            SetChartBoundaries();
        }

        public void SetSeries(IEnumerable<Series> series)
        {
            _seriesIndex.Clear();
            foreach (var s in series)
            {
                _seriesIndex.Add(s);
            }
        }

        private void SetTheAxesUp()
        {
            var area = _chart.ChartAreas[0];
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisY.IntervalType = DateTimeIntervalType.Number;
        }

        private void SetChartBoundaries()
        {
            var allPoints = _seriesIndex.SelectMany(series => series.Points).ToArray();
            var yValues = allPoints.SelectMany(point => point.YValues).ToArray();
            double maxAxisX = _xPosCounter;
            double minAxisX = _xPosCounter - MaxPoints;
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

        public void AddSeries(Series series)
        {
            if(!string.IsNullOrEmpty(series.Name) && !_seriesIndex.Contains(series))
            {
                _seriesIndex.Add(series);
                _chart.Series.Add(series);
                SetChartBoundaries();
            }
        }

        public void RemoveSeries(string seriesName)
        {
            var seriesToRemove = GetSeriesFromIndexByName(seriesName);
            if (seriesToRemove != null)
            {
                RemoveSeries(seriesToRemove);
            }
        }

        private void RemoveSeries(Series series)
        {
            _seriesIndex.Remove(series);
            _chart.Series.Remove(series);
            SetChartBoundaries();
        }

        public void AddPoint(float pointValue, string seriesName)
            => AddPoint(pointValue, GetSeriesFromIndexByName(seriesName));

        private void AddPoint(float pointValue, Series series)
        {
            var points = series?.Points;
            if (points != null)
            {
                points.AddXY(_xPosCounter++, pointValue);
                while(points.Count > MaxPoints) points.RemoveAt(0);
                SetChartBoundaries();
            }
        }

        private Series GetSeriesFromIndexByName(string name)
            => _seriesIndex.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}