﻿using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : UntypedActor
    {
        public class InitializeChart
        {
            public InitializeChart(Dictionary<string, Series> initialSeries) => InitialSeries = initialSeries;

            public Dictionary<string, Series> InitialSeries { get; }
        }

        private readonly Chart _chart;
        private Dictionary<string, Series> _seriesIndex;

        public ChartingActor(Chart chart) : this(chart, new Dictionary<string, Series>())
        {
        }

        private ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
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
                series.Value.Name = series.Key;
                _chart.Series.Add(series.Value);
            }
        }
    }
}
