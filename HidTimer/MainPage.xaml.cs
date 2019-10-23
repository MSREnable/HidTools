using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Input.Preview;
using System.Collections.ObjectModel;
using System.Timers;
using System;
using Windows.System.Threading;
using Windows.UI.Core;

namespace HidTimer
{
    public sealed partial class MainPage : Page
    {
        private GazeInputSourcePreview gazeInputSourcePreview;
        private readonly Frame rootFrame;

        public ObservableCollection<Point> GazeHistory { get; set; } = new ObservableCollection<Point>();

        public int TracePointDiameter { get; set; }

        public int MaxGazeHistorySize { get; set; }

        public bool ShowIntermediatePoints { get; set; }

        private readonly Timer timer;

        private DateTime startTime;
        private DateTime lastGazeTime;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;

            ShowIntermediatePoints = false;
            MaxGazeHistorySize = 100;

            rootFrame = Window.Current.Content as Frame;
            gazeInputSourcePreview = GazeInputSourcePreview.GetForCurrentView();
            gazeInputSourcePreview.GazeMoved += GazeInputSourcePreview_GazeMoved;

            startTime = DateTime.Now;
            TimerLog.Text += "Start Time: " + startTime + "\n";

            ThreadPoolTimer timer = ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                var currentTime = DateTime.Now;
                if ((currentTime - lastGazeTime).TotalSeconds > 10)
                {
                    _ = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            TimerLog.Text += "Timeout Triggered!\n";
                            TimerLog.Text += "Current Time: " + currentTime + "\n";
                            TimerLog.Text += "Last Time: " + lastGazeTime + "\n";
                        });
                }
            }, TimeSpan.FromSeconds(2));
        }

        private void UpdateGazeHistory(GazePointPreview pt)
        {
            if (!pt.EyeGazePosition.HasValue)
            {
                return;
            }

            var transform = rootFrame.TransformToVisual(this);
            var point = transform.TransformPoint(pt.EyeGazePosition.Value);
            GazeHistory.Add(point);
            if (GazeHistory.Count > MaxGazeHistorySize)
            {
                GazeHistory.RemoveAt(0);
            }
        }

        private void GazeInputSourcePreview_GazeMoved(GazeInputSourcePreview sender, GazeMovedPreviewEventArgs args)
        {
            lastGazeTime = DateTime.Now;

            if (!ShowIntermediatePoints)
            {
                UpdateGazeHistory(args.CurrentPoint);
                return;
            }

            var points = args.GetIntermediatePoints();
            foreach (var pt in points)
            {
                UpdateGazeHistory(pt);
            }
        }
    }
}
