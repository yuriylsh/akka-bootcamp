using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        private readonly Chart _chart;

        private static readonly SeriesByNameComparer seriesComparer = new SeriesByNameComparer();

        private readonly HashSet<Series> _seriesIndex = new HashSet<Series>(seriesComparer);

        public ChartingActor(Chart chart) : this(chart, Array.Empty<Series>())
        {
        }

        private ChartingActor(Chart chart, ICollection<Series> seriesIndex)
        {
            _chart = chart;
            SetSeries(seriesIndex);

            Receive<InitializeChart>(ic => HandleInitialize(ic));
            Receive<AddSeries>(addSeries => HandleAddSeries(addSeries));
        }

        private void HandleInitialize(InitializeChart initializeChart)
        {
            if (initializeChart.InitialSeries != null)
            {
                //swap the two series out
               SetSeries(initializeChart.InitialSeries);
            }

            //delete any existing series
            _chart.Series.Clear();

            //attempt to render the initial chart
            foreach (var series in _seriesIndex)
            {
                //force both the chart and the internal index to use the same names
                _chart.Series.Add(series);
            }
        }

        private void HandleAddSeries(AddSeries series)
        {
            var seriesToAdd = series.Series;
            if(!string.IsNullOrEmpty(seriesToAdd.Name) && !_seriesIndex.Contains(seriesToAdd))
            {
                _seriesIndex.Add(seriesToAdd);
                _chart.Series.Add(seriesToAdd);
            }
        }

        private void SetSeries(ICollection<Series> series)
        {
            _seriesIndex.Clear();
            foreach (var s in series)
            {
                _seriesIndex.Add(s);
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

        private class SeriesByNameComparer : IEqualityComparer<Series>
        {
            public bool Equals(Series x, Series y) => x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(Series series) => series.Name.GetHashCode();
        }
    }
}
