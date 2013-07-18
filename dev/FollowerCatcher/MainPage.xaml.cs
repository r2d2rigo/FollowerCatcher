using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FollowerCatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SwapChainBackgroundPanel
    {
        Accelerometer accelerometer;
        private Scene scene;
        private GestureRecognizer recognizer;
        private Point dragStartPoint;
        private bool needsAccelReset;

        public MainPage()
        {
            this.DataContext = new MainViewModel();
            this.recognizer = new GestureRecognizer();
            this.recognizer.GestureSettings = GestureSettings.Drag;
            this.recognizer.Tapped += recognizer_Tapped;
            this.recognizer.Dragging += recognizer_Dragging;
            this.InitializeComponent();
            accelerometer = Accelerometer.GetDefault();
            if (accelerometer != null)
            {
                accelerometer.ReportInterval = accelerometer.MinimumReportInterval;
                accelerometer.ReadingChanged += accelerometer_ReadingChanged;
            }
            MainAnim.Begin();
            needsAccelReset = false;
        }

        void accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            float xAccel = (float)args.Reading.AccelerationX;

            if (xAccel < -0.7f)
            {
                if (!needsAccelReset)
                {
                    scene.MovePlayer(RoadPaths.Left);
                    needsAccelReset = true;
                }
            }
            else if (xAccel > 0.7f)
            {
                if (!needsAccelReset)
                {
                    scene.MovePlayer(RoadPaths.Right);
                    needsAccelReset = true;
                }
            }
            else
            {
                needsAccelReset = false;
            }
        }

        public void SetScene(Scene scene)
        {
            this.scene = scene;
            this.scene.GameEnded += scene_GameEnded;
        }

        void scene_GameEnded(object sender, EventArgs e)
        {
            EndAnim.Begin();
        }

        void recognizer_Tapped(GestureRecognizer sender, TappedEventArgs args)
        {
        }

        void recognizer_Dragging(GestureRecognizer sender, DraggingEventArgs args)
        {
            if (args.DraggingState == DraggingState.Started)
            {
                dragStartPoint = args.Position;
            }
            if (args.DraggingState == DraggingState.Completed)
            {
                Point dragDirection = new Point(args.Position.X - dragStartPoint.X, args.Position.Y - dragStartPoint.Y);
                if (dragDirection.X < 0)
                {
                    scene.MovePlayer(RoadPaths.Left);
//                    scene.playerEntity.Transform = Matrix.Translation(-50.0f, 0, 0);
                }
                else
                {
                    scene.MovePlayer(RoadPaths.Right);
//                    scene.playerEntity.Transform = Matrix.Translation(50.0f, 0, 0);
                }
            }
        }

        private void SwapChainBackgroundPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void SwapChainBackgroundPanel_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var points = e.GetIntermediatePoints(null);
            if (points != null && points.Count > 0)
            {
                recognizer.ProcessUpEvent(points[0]);
                e.Handled = true;
                recognizer.CompleteGesture();
            }
        }

        private void SwapChainBackgroundPanel_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var points = e.GetIntermediatePoints(null);
            if (points != null && points.Count > 0)
            {
                recognizer.ProcessDownEvent(points[0]);
                e.Handled = true;
            }
        }

        private void SwapChainBackgroundPanel_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            recognizer.ProcessMoveEvents(e.GetIntermediatePoints(null));
            e.Handled = true;
        }

        private void PlayClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GameAnim.Begin();
            scene.LoadContent();
            scene.StartGame();
        }

        private void ScoreClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ScoreAnim.Begin();
        }
        private void BackClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MainAnim.Begin();
        }

        private void SwapChainBackgroundPanel_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            int a = 0;
        }

        private void SwapChainBackgroundPanel_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void SwapChainBackgroundPanel_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

    }
}
