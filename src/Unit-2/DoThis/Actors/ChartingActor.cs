using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        
        private readonly ChartingHelper _chart;

        public ChartingActor(Chart chart) : this(chart, Array.Empty<Series>())
        {
        }

        private ChartingActor(Chart chart, IEnumerable<Series> seriesIndex)
        {
            _chart = new ChartingHelper(chart);
            _chart.SetSeries(seriesIndex);

            Receive<InitializeChart>(ic => HandleInitialize(ic));
            Receive<AddSeries>(addSeries => HandleAddSeries(addSeries));
            Receive<RemoveSeries>(removeSeries => HandleRemoveSeries(removeSeries));
            Receive<Metric>(metric => HandleMetrics(metric));
        }

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                _chart.SetSeries(ic.InitialSeries);
            }
            _chart.Initialize();
        }

        private void HandleAddSeries(AddSeries series) => _chart.AddSeries(series.Series);

        private void HandleRemoveSeries(RemoveSeries series) => _chart.RemoveSeries(series.SeriesName);

        private void HandleMetrics(Metric metric) => _chart.AddPoint(metric.CounterValue, metric.Series);
       
        
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
    }
}
