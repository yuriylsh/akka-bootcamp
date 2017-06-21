using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : UntypedActor
    {
        public class InitializeChart
        {
            public InitializeChart(ICollection<Series> initialSeries) => InitialSeries = initialSeries;

            public ICollection<Series> InitialSeries { get; }
        }

        private readonly Chart _chart;
        private ICollection<Series> _seriesIndex;

        public ChartingActor(Chart chart) : this(chart, Array.Empty<Series>())
        {
        }

        private ChartingActor(Chart chart, ICollection<Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;
        }

        protected override void OnReceive(object message)
        {
            if (message is InitializeChart ic)
            {
                HandleInitialize(ic);
            }
        }

        private void HandleInitialize(InitializeChart initializeChart)
        {
            if (initializeChart.InitialSeries != null)
            {
                //swap the two series out
                _seriesIndex = initializeChart.InitialSeries;
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
    }
}
