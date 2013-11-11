using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RaionReminder.Throbber
{

    /// <summary>
    /// Логика взаимодействия для ThrobberControl.xaml
    /// </summary>
    public partial class ThrobberControl : UserControl
    {
        #region Data
        private readonly DispatcherTimer animationTimer;
        private ThrobberBindings tb;
        private double radius;

        public string Tooltip
        {
            get
            {
                return tb.Tooltip;
            }
            set
            {
                tb.Tooltip = value;
            }
        }

        public Brush Color
        {
            get
            {
                return tb.ThrobberColor;
            }
            set
            {
                tb.ThrobberColor = value;
            }
        }
        #endregion

        #region Constructor
        public ThrobberControl()
        {
            InitializeComponent();

            tb = new ThrobberBindings();
            tb.Tooltip = "";
            tb.ThrobberColor = new SolidColorBrush(Colors.Black);
            tb.BallSize = 15.7;
            this.DataContext = tb;

            animationTimer = new DispatcherTimer(
                DispatcherPriority.Render, Dispatcher);
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        #endregion

        #region Private Methods
        private void Start()
        {
            //Mouse.OverrideCursor = Cursors.Wait;
            animationTimer.Tick += HandleAnimationTick;
            animationTimer.Start();
        }

        private void Stop()
        {
            animationTimer.Stop();
            //Mouse.OverrideCursor = Cursors.Arrow;
            animationTimer.Tick -= HandleAnimationTick;
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI ;
            const double step = Math.PI * 2 / 10.0;

            double min_size = 0;
            if (CntrlCanvas.ActualHeight < CntrlCanvas.ActualWidth) min_size = CntrlCanvas.ActualHeight;
            else min_size = CntrlCanvas.ActualWidth;



            tb.BallSize = Math.PI * min_size / 20;

            radius = (min_size - tb.BallSize) / 2;

            this.DataContext = null;
            this.DataContext = tb;
            this.UpdateLayout();

            SetPosition(C0, offset, 0.0, step);
           SetPosition(C1, offset, 1.0, step);
           SetPosition(C2, offset, 2.0, step);
            SetPosition(C3, offset, 3.0, step);
            SetPosition(C4, offset, 4.0, step);
            SetPosition(C5, offset, 5.0, step);
            SetPosition(C6, offset, 6.0, step);
            SetPosition(C7, offset, 7.0, step);
            SetPosition(C8, offset, 8.0, step);

            Start();
        }

        private void SetPosition(Ellipse ellipse, double offset,
            double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, CntrlCanvas.ActualWidth / 2 - tb.BallSize / 2
                + Math.Sin(offset + posOffSet * step) * radius);

            ellipse.SetValue(Canvas.TopProperty, CntrlCanvas.ActualHeight / 2 - tb.BallSize / 2
                + Math.Cos(offset + posOffSet * step) * radius);
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void HandleVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;

            if (isVisible)
                HandleLoaded(null, null);
            else
                HandleUnloaded(null, null);
        }
        #endregion



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            

           
            
        }
    }

    public class ThrobberBindings
    {
        public string Tooltip { get; set; }
        public Brush ThrobberColor { get; set; }
        public double BallSize { get; set; }
    }

}
