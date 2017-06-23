using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        
        private readonly ChartingHelper _chart;

        private readonly Action<string> _pauseButtonTextSetter;

        public ChartingActor(Chart chart, Action<string> textSetter) : this(chart, Array.Empty<Series>(), textSetter)
        {
        }

        private ChartingActor(Chart chart, IEnumerable<Series> seriesIndex, Action<string> pauseButtonTextSetter)
        {
            _chart = new ChartingHelper(chart);
            _chart.SetSeries(seriesIndex);
            _pauseButtonTextSetter = pauseButtonTextSetter;
            Charting();
        }

        private void Charting()
        {
            Receive<InitializeChart>(ic => HandleInitialize(ic));
            Receive<AddSeries>(addSeries => HandleAddSeries(addSeries));
            Receive<RemoveSeries>(removeSeries => HandleRemoveSeries(removeSeries));
            Receive<Metric>(metric => HandleMetrics(metric));

            //new receive handler for the TogglePause message type
            Receive<TogglePause>(pause =>
            {
                SetPauseButtonText(true);
                BecomeStacked(Paused);
            });
        }

        private void Paused()
        {
            Receive<Metric>(metric => HandleMetricsPaused(metric));
            Receive<TogglePause>(pause =>
            {
                SetPauseButtonText(false);
                UnbecomeStacked();
            });
        }

        private void SetPauseButtonText(bool paused) => _pauseButtonTextSetter(!paused ? "PAUSE ||" : "RESUME ->");

        private void HandleMetricsPaused(Metric metric) => _chart.AddPoint(0.0f, metric.Series);

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

        public class TogglePause { }
    }
}
