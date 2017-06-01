using System;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace RoutingPrototype
{
    public partial class LivePlot : Form
    {
        public ChartValues<MeasureModel> ChartValuesSkeining { get; set; }
        public ChartValues<MeasureModel> ChartValuesNonSkeining { get; set; }

        private int fontSize = 14;

        public LivePlot()
        {
            InitializeComponent();

            // Shamelessly copy and pasted from: https://lvcharts.net/App/examples/v1/wf/Constant%20Changes

            //To handle live data easily, in this case we built a specialized type
            //the MeasureModel class, it only contains 2 properties
            //DateTime and Value
            //We need to configure LiveCharts to handle MeasureModel class
            //The next code configures MeasureModel  globally, this means
            //that livecharts learns to plot MeasureModel and will use this config every time
            //a ChartValues instance uses this type.
            //this code ideally should only run once, when application starts is recommended.
            //you can configure series in many ways, learn more at http://lvcharts.net/App/examples/v1/wpf/Types%20and%20Configuration

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y
            
            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the ChartValues property will store our values array
            ChartValuesSkeining = new ChartValues<MeasureModel>();
            ChartValuesNonSkeining = new ChartValues<MeasureModel>();

            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Skyline Cost (£) \r With Skeining",
                    FontSize = fontSize,
                    Foreground = System.Windows.Media.Brushes.White,
                    Values = ChartValuesSkeining,
                    PointGeometrySize = 9,
                    StrokeThickness = 4
                },
                new LineSeries
                {
                    Title = "Skyline Cost (£) \r Without Skeining",
                    FontSize = fontSize,
                    Foreground = System.Windows.Media.Brushes.White,
                    Values = ChartValuesNonSkeining,
                    PointGeometrySize = 9,
                    StrokeThickness = 4
                },
            };

            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "Time",
                FontSize = fontSize,
                Foreground = System.Windows.Media.Brushes.White,
                DisableAnimations = true,
                LabelFormatter = value => new System.DateTime((long)value).ToString("mm:ss"),
                Separator = new Separator
                {
                    Step = TimeSpan.FromSeconds(1).Ticks
                }
            });

            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Skyline Cost (£)",
                FontSize = fontSize,
                Foreground = System.Windows.Media.Brushes.White,
                
            });

            cartesianChart1.LegendLocation = LegendLocation.Right;
            cartesianChart1.DefaultLegend.Foreground = System.Windows.Media.Brushes.White;
            SetAxisLimits(System.DateTime.Now);


        }

        private void SetAxisLimits(System.DateTime now)
        {
            cartesianChart1.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            cartesianChart1.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(8).Ticks; //we only care about the last 8 seconds
        }

        public void updatePlot(float skeinValue, float nonSkeinValue)
        {
            var now = System.DateTime.Now;

            ChartValuesSkeining.Add(new MeasureModel
            {
                DateTime = now,
                Value = skeinValue
            });

            ChartValuesNonSkeining.Add(new MeasureModel
            {
                DateTime = now,
                Value = nonSkeinValue
            });

            SetAxisLimits(now);

            //lets only use the last 10 values
            if (ChartValuesSkeining.Count > 10) ChartValuesSkeining.RemoveAt(0);
            if (ChartValuesNonSkeining.Count > 10) ChartValuesNonSkeining.RemoveAt(0);

        }

    }
}
