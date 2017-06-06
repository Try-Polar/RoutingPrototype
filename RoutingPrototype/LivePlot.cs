using System;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Linq;

namespace RoutingPrototype
{
    public partial class LivePlot : Form
    {
        public ChartValues<MeasureModel> ChartValuesWorstSkeining { get; set; }
        public ChartValues<MeasureModel> ChartValuesBestSkeining { get; set; }
        public ChartValues<MeasureModel> ChartValuesNonSkeining { get; set; }

        private int fontSize = 14;

        private Queue<float> worstSkeinCostFiveSecs; // Worst forecast skein cost values past 5 seconds
        private Queue<float> bestSkeinCostFiveSecs; // Best forecast skein cost values past 5 seconds
        private Queue<float> nonSkeinCostFiveSecs; // Non skein cost values past 5 seconds

        private float timePassed = 0;

        public LivePlot()
        {
            InitializeComponent();

            // Populate total skien cost in the past 5 seconds queues 
            List<float> list = new List<float> {0,0,0,0,0};
            worstSkeinCostFiveSecs = new Queue<float>(list);
            bestSkeinCostFiveSecs = new Queue<float>(list);
            nonSkeinCostFiveSecs = new Queue<float>(list);

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
                .X(model => model.Time)             //use Time as X
                .Y(model => model.Value);           //use the value property as Y
            
            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the ChartValues property will store our values array
            ChartValuesWorstSkeining = new ChartValues<MeasureModel>();
            ChartValuesBestSkeining = new ChartValues<MeasureModel>();
            ChartValuesNonSkeining = new ChartValues<MeasureModel>();

            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Worst Case Operation Cost \r With Skeining (past 2 hours)",
                    FontSize = fontSize,
                    Foreground = System.Windows.Media.Brushes.White,
                    Values = ChartValuesWorstSkeining,
                    PointGeometrySize = 9,
                    StrokeThickness = 4,
                    Stroke = System.Windows.Media.Brushes.Blue
                },

                new LineSeries
                {
                    Title = "Best Case Skyline Operation \r With Skeining (past 2 hours)",
                    FontSize = fontSize,
                    Foreground = System.Windows.Media.Brushes.White,
                    Values = ChartValuesBestSkeining,
                    PointGeometrySize = 9,
                    StrokeThickness = 4,
                    Stroke = System.Windows.Media.Brushes.Green
                },

                new LineSeries
                {
                    Title = "Operation Cost \r Without Skeining (past 2 hours)",
                    FontSize = fontSize,
                    Foreground = System.Windows.Media.Brushes.White,
                    Values = ChartValuesNonSkeining,
                    PointGeometrySize = 9,
                    StrokeThickness = 4,
                    Stroke = System.Windows.Media.Brushes.Red
                },
            };

            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "Time Elaspsed (hours)",
                FontSize = fontSize,
                Foreground = System.Windows.Media.Brushes.White,
                DisableAnimations = true,
                LabelFormatter = value => value + " h",
                Separator = new Separator
                {
                    Step = 0.4
                }
            });

            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Operation Cost for past 2 hours",
                FontSize = fontSize,
                Foreground = System.Windows.Media.Brushes.White,
                LabelFormatter = val => val.ToString("C") //as currency,

        });

            cartesianChart1.LegendLocation = LegendLocation.Right;
            cartesianChart1.DefaultLegend.Foreground = System.Windows.Media.Brushes.White;
            SetAxisLimits(System.DateTime.Now);
        }

        private void SetAxisLimits(System.DateTime now)
        {
            cartesianChart1.AxisX[0].MaxValue = timePassed + 0.4; // lets force the axis to be 100ms ahead
            cartesianChart1.AxisX[0].MinValue = timePassed - 3; //we only care about the last 8 seconds
        }

        public void updatePlot(bool isSkiening, float worstSkeinValue, float bestSkeinValue, float nonSkeinValue)
        {
            var now = System.DateTime.Now;

            // Calculats total cost in the past 5 seconds
            worstSkeinCostFiveSecs.Enqueue(worstSkeinValue);
            float oldWorstSkeinCost = worstSkeinCostFiveSecs.Dequeue();
            bestSkeinCostFiveSecs.Enqueue(bestSkeinValue);
            float oldBestSkeinCost = bestSkeinCostFiveSecs.Dequeue();
            nonSkeinCostFiveSecs.Enqueue(nonSkeinValue);
            float oldNonSkeinCost = nonSkeinCostFiveSecs.Dequeue();

            ChartValuesWorstSkeining.Add(new MeasureModel
            {
                Time = timePassed,
                Value = worstSkeinValue - oldWorstSkeinCost
            });

            ChartValuesBestSkeining.Add(new MeasureModel
            {
                Time = timePassed,
                Value = bestSkeinValue - oldBestSkeinCost
            });

            ChartValuesNonSkeining.Add(new MeasureModel
            {
                Time = timePassed,
                Value = nonSkeinValue - oldNonSkeinCost
            });

            if (!isSkiening)
            {
                ChartValuesWorstSkeining.Clear();
                ChartValuesBestSkeining.Clear();
            }

            timePassed += (float)0.4;

            SetAxisLimits(now);

            //lets only use the last 10 values
            if (ChartValuesWorstSkeining.Count > 10) ChartValuesWorstSkeining.RemoveAt(0);
            if (ChartValuesBestSkeining.Count > 10) ChartValuesBestSkeining.RemoveAt(0);
            if (ChartValuesNonSkeining.Count > 10) ChartValuesNonSkeining.RemoveAt(0);

        }

        public void clearPlot()
        {
            // Populate total skien cost in the past 5 seconds queues 
            List<float> list = new List<float> { 0, 0, 0, 0, 0 };
            worstSkeinCostFiveSecs = new Queue<float>(list);
            bestSkeinCostFiveSecs = new Queue<float>(list);
            nonSkeinCostFiveSecs = new Queue<float>(list);
        }


    }
}
