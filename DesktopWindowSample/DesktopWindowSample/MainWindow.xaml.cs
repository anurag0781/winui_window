using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Brushes;
using System;
using WinUIExtensions.Desktop;
using Windows.Foundation;
using System.Runtime.InteropServices;
using System.Collections.Generic;


namespace DesktopWindowSample
{
    public sealed partial class MainWindow : DesktopWindow
    {
        private static int savedPrevioustopology;
        Dictionary<int, string> topologyIdsToValuesMap;

        public MainWindow()
        {
            this.InitializeComponent();

            savedPrevioustopology = GetTopology();
            topologyIdsToValuesMap = new Dictionary<int, string>();
            topologyIdsToValuesMap[1] = "Show on One";
            topologyIdsToValuesMap[2] = "Duplicate";
            topologyIdsToValuesMap[4] = "Extend";
            topologyIdsToValuesMap[8] = "Show on Two";

            _canvasSwapChainPanel.Width = 100;
            _canvasSwapChainPanel.SwapChain = new CanvasSwapChain(CanvasDevice.GetSharedDevice(), 100, 100, 1.25f * 96);

            this.Activated += WindowActivated;                        

            // MUX.Window features
            this.Title = "Showcase DesktopWindow API";
            this.ExtendsContentIntoTitleBar = true;
            SetTitleBar(myTitleBar);

            //DesktopWindow Features
            
            // Set the Max height and width in effective pixels
            this.MaxHeight = 800;
            this.MaxWidth = 1024;

            // Set the Min height and width in effective pixels
            this.MinHeight = 400;
            this.MinWidth = 400;

            // Set the width and Heigh in effective pixels
            this.Width = 600;
            this.Height = 600;

            // Set the Icon of the window. 
            this.Icon = "WinUI3.ico";

            // Set the placement of the Window top, left
            SetWindowPlacement(0, 0);

            //Fire before closing the window so you can cancel the close event
            this.Closing += MainWindow_Closing;

            //Fire when the user moves the window
            this.Moving += MainWindow_Moving;
            //Fire when the users change the size of the window
            this.Sizing += MainWindow_Sizing;
            //Fire when the Dpi of the display that host the window changes
            this.DpiChanged += MainWindow_DpiChanged;
            //Fire when the orientation (portrait and landscape) of the display that host the window changes
            this.OrientationChanged += MainWindow_OrientationChanged;

            this.MonitorTopologyChanged += MainWindow_MonitorTopologyChanged;

            //Get the heigh and the width
            debugTBox.Text = $" Size (Height: { this.Height } Width: { this.Width })";


            //Get the List of monitors/displays
            var list = WinUIExtensions.Desktop.DisplayInformation.GetDisplays();

            foreach (var item in list)
            {
                systemTBox.Text += $"Display ->" +
                    $"\n  Device Name: {item.DeviceName}" +
                    $"\n  Work Area: ({item.WorkArea.top},{item.WorkArea.left},{item.WorkArea.bottom},{item.WorkArea.right})" +
                    $"\n  ScreenHeight: {item.ScreenHeight}" +
                    $"\n  ScreenWidth:{item.ScreenWidth}" +
                    $"\n  Effective Pixels: width({item.ScreenEfectiveWidth}), height({item.ScreenEfectiveHeight})" +
                    $"\n  Availability {item.Availability}\n\n";
            }

            //Get the current DPI of the display that host the window
            dpiChangedTBox.Text =  $"DPI: {this.Dpi}";
        }

        [DllImport("Win32NativeAPIsImpl.dll")]
        static extern void DisplayWindowInSecondaryMonitor(IntPtr hwnd);

        [DllImport("Win32NativeAPIsImpl.dll")]
        static extern int GetTopology();

        void WindowActivated(Object o, WindowActivatedEventArgs args)
        {
            using (CanvasDrawingSession ds = _canvasSwapChainPanel.SwapChain.CreateDrawingSession(Microsoft.UI.Colors.White))
            {
                ds.DrawCircle(50, 50, 25, Microsoft.UI.Colors.Red, 20);
            }

            _canvasSwapChainPanel.SwapChain.Present();            

            DisplayWindowInSecondaryMonitor(this.Hwnd);            

        }

        private void MainWindow_OrientationChanged(object sender, WindowOrientationChangedEventArgs e)
        {
            dpiChangedTBox.Text = $"DPI: {this.Dpi} + Orientation: { e.Orientation.ToString("g")}";
        }

        private async void MainWindow_MonitorTopologyChanged()
        {
            int currentTopology = GetTopology();
            if (savedPrevioustopology != currentTopology)
            {
                ContentDialog contentDialog = new ContentDialog();
                contentDialog.Content = "Topology changed from \"" + topologyIdsToValuesMap[savedPrevioustopology] + "\" to \"" + topologyIdsToValuesMap[currentTopology] + "\".";
                contentDialog.XamlRoot = this.Content.XamlRoot;
                contentDialog.CloseButtonText = "OK";
                contentDialog.IsPrimaryButtonEnabled = false;
                var r = await contentDialog.ShowAsync();
                savedPrevioustopology = currentTopology;
            }
        }

        private void MainWindow_DpiChanged(object sender, WindowDpiChangedEventArgs e)
        {
            dpiChangedTBox.Text = $"DPI Changed:{ e.Dpi} - {this.Dpi} ";
        }

        //public static T FindControl<T>(DependencyObject parent, string ControlName) where T : FrameworkElement
        //{
        //    if (parent == null)
        //        return null;

        //    if (parent.GetType() == typeof(T) && ((T)parent).Name == ControlName)
        //    {
        //        return (T)parent;
        //    }
        //    T result = null;
        //    int count = VisualTreeHelper.GetChildrenCount(parent);
        //    for (int i = 0; i < count; i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(parent, i);

        //        if (FindControl<T>(child, ControlName) != null)
        //        {
        //            result = FindControl<T>(child, ControlName);
        //            break;
        //        }
        //    }
        //    return result;
        //}

        private void MainWindow_Sizing(object sender, WindowSizingEventArgs e)
        {
            var windowPosition = GetWindowPosition();
            debugTBox.Text = $"Height: { this.Height } Width: { this.Width }\n " +
                $"Top: {windowPosition.Top} Left:{windowPosition.Left}";
        }

        private void MainWindow_Moving(object sender, WindowMovingEventArgs e)
        {
            debugTBox.Text = $"Height: { this.Height } Width: { this.Width }\n " +
                $"Top: {e.NewPosition.Top} Left:{e.NewPosition.Left}";
        }

        private async void MainWindow_Closing(object sender, WindowClosingEventArgs e)
        {
            ContentDialog contentDialog = new ContentDialog();
            contentDialog.Content = "Close it?";
            contentDialog.XamlRoot = this.Content.XamlRoot;
            contentDialog.PrimaryButtonText = "Yes";
            contentDialog.CloseButtonText = "No";
            contentDialog.IsPrimaryButtonEnabled = true;
            var r = await contentDialog.ShowAsync();
            if (r == ContentDialogResult.Primary)
            {
                // Cancel the close event 
                e.TryCancel();
            }
        }

        //Center the Window on the display
        private void OnCenterClick(object sender, RoutedEventArgs e)
        {
            SetWindowPlacement(Placement.Center);
        }

        //Place the Window on the Top Left
        private void OnTopLeft(object sender, RoutedEventArgs e)
        {
            SetWindowPlacement(Placement.TopLeftCorner);
        }

        //Place the Window on the Bottom Left
        private void OnBottomLeft(object sender, RoutedEventArgs e)
        {
            SetWindowPlacement(Placement.BottomLeftCorner);
        }

        //Maximize the Window
        private void OnMaximize(object sender, RoutedEventArgs e)
        {
            Maximize();
        }

        //Minimize the Window
        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            Minimize();
        }

        //Restore the Window
        private void OnRestore(object sender, RoutedEventArgs e)
        {
            Restore();
        }

        //Bring the Window to the top
        private void OnBringToTop(object sender, RoutedEventArgs e)
        {
            BringToTop();
        }
        
        private void OnClosingApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
