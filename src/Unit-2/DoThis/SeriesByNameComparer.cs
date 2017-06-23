using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartApp
{
    public class SeriesByNameComparer : IEqualityComparer<Series>
    {
        public bool Equals(Series x, Series y) => x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode(Series series) => series.Name.GetHashCode();
    }
}