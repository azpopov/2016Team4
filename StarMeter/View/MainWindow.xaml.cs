﻿using Microsoft.Win32;
using StarMeter.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using StarMeter.Controllers;

namespace StarMeter.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Controller controller = new Controller();

        private StackPanel[] _portStacks = new StackPanel[8];



        

        public MainWindow()
        {
            InitializeComponent();

            // initialise the stack array
            _portStacks[0] = Port1AHolder;
            _portStacks[1] = Port1BHolder;
            _portStacks[2] = Port2AHolder;
            _portStacks[3] = Port2BHolder;
            _portStacks[4] = Port3AHolder;
            _portStacks[5] = Port3BHolder;
            _portStacks[6] = Port4AHolder;
            _portStacks[7] = Port4BHolder;

        }

        // needed for drawing rectangle - for zooming
        private bool _mouseDown; 
        private Point _mouseDownPos;

        // TODO - Find reference to this stuff
        #region Drag Rectangle Methods 
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Capture and track the mouse.
            _mouseDown = true;
            _mouseDownPos = e.GetPosition(MainGrid);
            MainGrid.CaptureMouse();

            // Initial placement of the drag selection box.         
            Canvas.SetLeft(selectionBox, _mouseDownPos.X);
            Canvas.SetTop(selectionBox, _mouseDownPos.Y);
            selectionBox.Width = 0;
            selectionBox.Height = 0;

            // Make the drag selection box visible.
            selectionBox.Visibility = Visibility.Visible;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Release the mouse capture and stop tracking it.
            _mouseDown = false;
            MainGrid.ReleaseMouseCapture();

            // Hide the drag selection box.
            selectionBox.Visibility = Visibility.Collapsed;

            var s = new Size(selectionBox.Width, selectionBox.Height);

            SizeLabelTest.Content = s.ToString();

            // TODO: 
            //
            // The mouse has been released, check to see if any of the items 
            // in the other canvas are contained within mouseDownPos and 
            // mouseUpPos, for any that are, select them!
            //
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown) return;
            // When the mouse is held down, reposition the drag selection box.

            var mousePos = e.GetPosition(MainGrid);

            if (_mouseDownPos.X < mousePos.X)
            {
                Canvas.SetLeft(selectionBox, _mouseDownPos.X);
                selectionBox.Width = mousePos.X - _mouseDownPos.X;
            }
            else
            {
                Canvas.SetLeft(selectionBox, mousePos.X);
                selectionBox.Width = _mouseDownPos.X - mousePos.X;
            }

            if (_mouseDownPos.Y < mousePos.Y)
            {
                Canvas.SetTop(selectionBox, _mouseDownPos.Y);
                selectionBox.Height = mousePos.Y - _mouseDownPos.Y;
            }
            else
            {
                Canvas.SetTop(selectionBox, mousePos.Y);
                selectionBox.Height = _mouseDownPos.Y - mousePos.Y;
            }
        }
        #endregion


        private Button GetPacketButton(Packet p) 
        {
            // create a label for the time
            # region Time label
            Label l = new Label();
            l.Content = "00:00:00.000";
            l.SetResourceReference(Control.StyleProperty, "Timestamp");

            TimeList.Children.Add(l);
            #endregion


            #region Create Button for the packet
            string sty = "";
            
            var b = new Button();
            b.Click += OpenPopup;

            var lab = new Label();

            try
            {
                lab.Content = p.Cargo[0];
            }
            catch (Exception e) 
            {
                lab.Content = "Unknown Packet Type";
            }

            try
            {
                b.Tag = p.PacketId;
            }
            catch (Exception) 
            {
                b.Tag = "";
            }
           
            b.Content = lab;

            try
            {
                if (p.IsError)
                {
                    sty = "Error";
                }
                else
                {
                    sty = "Success";
                }
            }
            catch (Exception) 
            {
                sty = "Error";
            }

            b.SetResourceReference(Control.StyleProperty, sty);
            return b;
            #endregion

        }

        // work out which panel to assign the packet to
        StackPanel GetPanelToUse(int portNum) 
        {
            return _portStacks[portNum];
        }


        void AddPacketCollection(Packet[] packets) 
        {
            foreach (var p in packets) 
            {
                AddPacket(p);
            }
        }

        private void AddPacket(Packet p) 
        {
            Button b = GetPacketButton(p);

            StackPanel sp = GetPanelToUse(p.PortNumber);            

            sp.Children.Add(b);
        }

        //This function will remove all packets from the screen which are being displayed.
        void RemoveAllPackets()
        {
            for (int i = 0; i < 8; i++ )
            {
                var childElements = _portStacks[i].Children;

                while (childElements.Count > 0)
                {
                    childElements.Remove((UIElement)childElements[0]);
                }
            }

        }
        #region TEMP
        Packet[] packets = new Packet[3];

        private void TestTimeCreation(object sender, RoutedEventArgs e) 
        {
            AddPacketCollection(packets);


            
            RatesLineChart.Series.Clear();

            var valueList = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("00:00:00:000", 4),
                new KeyValuePair<string, int>("00:00:01:000", 1),
                new KeyValuePair<string, int>("00:00:04:102", 42),
                new KeyValuePair<string, int>("00:00:14:000", 21),
                new KeyValuePair<string, int>("00:00:41:000", 41),
                new KeyValuePair<string, int>("00:01:12:050", 24),
                new KeyValuePair<string, int>("00:01:17:000", 17),
                new KeyValuePair<string, int>("00:01:60:100", 19)
            };


            var lineSeries1 = new LineSeries
            {
                Title = "Data Rate",
                Foreground = Brushes.White,
                DependentValuePath = "Value",
                IndependentValuePath = "Key",
                ItemsSource = valueList
            };
            RatesLineChart.Series.Add(lineSeries1);



            var valueList2 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("00:00:00:000", 8),
                new KeyValuePair<string, int>("00:00:01:000", 2),
                new KeyValuePair<string, int>("00:00:04:102", 42),
                new KeyValuePair<string, int>("00:00:14:000", 23),
                new KeyValuePair<string, int>("00:00:41:000", 19),
                new KeyValuePair<string, int>("00:01:12:050", 25),
                new KeyValuePair<string, int>("00:01:17:000", 18),
                new KeyValuePair<string, int>("00:01:60:100", 19)
            };


            LineSeries lineSeries2 = new LineSeries
            {
                Foreground = Brushes.White,
                Title = "Error Rate",
                DependentValuePath = "Value",
                IndependentValuePath = "Key",
                ItemsSource = valueList2
            };
            RatesLineChart.Series.Add(lineSeries2);

            //ScrollBackToBottom();

        }
        #endregion
        

        //This will allow us to read the files or remove the files later.
        private readonly List<Grid> _fileGrids = new List<Grid>();

        private void FileSelection(object sender, RoutedEventArgs e) 
        {
            var ofd = new OpenFileDialog
            {
                // only allow .rec files
                Filter = "Record Files (.rec)|*.rec",
                Multiselect = true
            };

            bool? confirmed = ofd.ShowDialog();

            if (confirmed != true) return;

            // display file name
            controller.AddFileNames(ofd.FileNames);
                
            foreach (string fileName in controller.GetFileNames())
            {
                string actualName = fileName.Split('.')[0];
                var g = new Grid
                {
                    Name = "grid_" + actualName, //remove file extension for name
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Height = 30,
                    Margin = new Thickness(0, 0, 0, 5),
                    Background = Brushes.White
                };
                var cd = new ColumnDefinition();
                var cd2 = new ColumnDefinition();
                cd.Width = new GridLength(8, GridUnitType.Star);
                cd2.Width = new GridLength(1, GridUnitType.Star);

                g.ColumnDefinitions.Add(cd);
                g.ColumnDefinitions.Add(cd2);

                Label l = new Label
                {
                    Name = "label_" + actualName,
                    Style = (Style) Application.Current.Resources["FileSelected"],
                    Content = fileName
                };

                var b = new Button { Name = actualName };

                b.Tag = fileName;
                b.Content = "X";
                b.Click += CancelUpload;
                b.Background = Brushes.Red;
                b.Foreground = Brushes.White;
                b.HorizontalContentAlignment = HorizontalAlignment.Center;
                b.VerticalContentAlignment = VerticalAlignment.Center;
                b.HorizontalAlignment = HorizontalAlignment.Stretch;
                b.VerticalAlignment = VerticalAlignment.Stretch;
                b.Name = "RemoveButton";

                Grid.SetColumn(l, 0);
                Grid.SetColumn(b, 1);

                g.Children.Add(l);
                g.Children.Add(b);
                SelectedFiles.Children.Add(g);
                _fileGrids.Add(g);

            }
        }

        void CancelUpload(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string tag = b.Tag.ToString(); //fileName

            int id = controller.RemoveFile(tag);

            SelectedFiles.Children.RemoveAt(id);
            _fileGrids.RemoveAt(id);
        }

        private void RemoveFile(Panel grid)
        {
            foreach(UIElement child in grid.Children)
            {
                grid.Children.Remove(child);
            }

            SelectedFiles.Children.Remove(grid);
        }

        public void OpenPopup(object sender, RoutedEventArgs e) 
        {
            var b = (Button)sender;

            var br = b.Background;

            var text = b.Tag.ToString();
            var guid = new Guid(text);
            
            PacketPopup pp = new PacketPopup();

            Packet p = controller.FindPacket(guid);

            if (p != null)
            {
                pp.SetupElements(br, p); // send the packet as a parameter, along with the colour to make the header
                pp.ShowDialog();
            }
            
        }

		
        void SearchForAddress(object sender, RoutedEventArgs e)
        {
            RemoveAllPackets();

            var search = addressSearch.Text;

            for (int i = 0; i < packets.Length; i++)
            {
                string address = "";
                if (packets[i] != null)
                {
                    for (int j = 0; j < packets[i].Address.Length; i++)
                    {
                        address += packets[i].Address[j] + ".";
                    }
                }
            }
        }

        Packet FindPacket(Guid guid) 
        {

            // TODO: change this to be a lookup from dictionary

            foreach (var p in packets) 
            {
                if (guid.Equals(p.PacketId)) 
                {
                    return p;
                }
            }

            return null;
        }
        
        //This lets us know which image to change to.
        private bool _isUpArrow = true;

        private void ShowDataVisPopup(object sender, RoutedEventArgs e)
        {
            ImageBrush image;

            if (_isUpArrow)
            {
                DataVisButton.VerticalAlignment = VerticalAlignment.Top;
                image = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Resources/down-arrow.png")));
            }
            else
            {
                image = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Resources/up-arrow.png")))
                {
                    Stretch = Stretch.UniformToFill
                };
            }

            if (_t == null)
            {
                _t = new System.Timers.Timer();
                _t.Elapsed += TimerEventProcessor;
                _t.Interval = 10;
                _t.Start();
            }

            DataVisButton.Background = image;
        }

        public delegate void UpdateSlider();
        
        // This is the method to run when the timer is raised.
        private void TimerEventProcessor(object myObject,EventArgs myEventArgs)
        {

            // Restarts the timer and increments the counter.
            if (_isUpArrow)
            {
                _count += 1;
            }
            else 
            {
                _count -= 1;
            }

            if ((_count > 10 && _isUpArrow) || (_count < 2 && !_isUpArrow))
            {
                _t.Stop();

                DataVisualisationPopup.Dispatcher.Invoke(new UpdateSlider(FixStretch));

                _isUpArrow = !_isUpArrow;
                _t = null;
            }

            DataVisualisationPopup.Dispatcher.Invoke(new UpdateSlider(MoveSlider));
        }

        System.Timers.Timer _t;
        int _count;

        /// <summary>
        /// set the height of the packet buttons
        /// </summary>
        private void MoveSlider()
        {
            DataVisualisationPopup.Height = new GridLength(_count, GridUnitType.Star); ;
        }

        /// <summary>
        /// Fixes the button at the bottom - else it looks silly
        /// </summary>
        private void FixStretch()
        {
            if (!_isUpArrow)
            {
                DataVisButton.VerticalAlignment = VerticalAlignment.Stretch;
            }

        }

        /// <summary>
        /// Get the style for an error
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        static Style GetErrorStyle(double val)
        {

            var style = new Style { TargetType = typeof(Button) };
            style.Setters.Add(new Setter(MarginProperty, new Thickness(0, 0, 0, (val / 10) - 1)));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            style.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(ForegroundProperty, Brushes.White));
            style.Setters.Add(new Setter(BackgroundProperty, Brushes.Red));
            style.Setters.Add(new Setter(HeightProperty, val));

            return style;
        }

        /// <summary>
        /// get the style for a successful packet
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static Style GetSuccessStyle(double val) 
        {

            var style = new Style { TargetType = typeof(Button) };
            style.Setters.Add(new Setter(MarginProperty, new Thickness(0, 0, 0, (val / 10) - 1)));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            style.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(ForegroundProperty, Brushes.Black));


            var converter = new System.Windows.Media.BrushConverter();

            style.Setters.Add(new Setter(Button.BackgroundProperty, (Brush)converter.ConvertFromString("#6699ff")));
            style.Setters.Add(new Setter(Button.HeightProperty, val));

            return style;
        }

        /// <summary>
        /// Get the style for a timestamp
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public Style GetTimeStyle(double val)
        {

            var style = new Style { TargetType = typeof(Label) };
            style.Setters.Add(new Setter(MarginProperty, new Thickness(0, 0, 0, (val / 10) - 1)));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            style.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(VerticalContentAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(ForegroundProperty, Brushes.Black));


            var converter = new System.Windows.Media.BrushConverter();

            style.Setters.Add(new Setter(Button.BackgroundProperty, (Brush)converter.ConvertFromString("#d9d9d9")));
            style.Setters.Add(new Setter(HeightProperty, val));

            return style;
        }

        /// <summary>
        /// change the height of the objects - xzoom in and out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Application.Current.Resources["Success"] = GetSuccessStyle(HeightScroller.Value);
            Application.Current.Resources["Error"] = GetErrorStyle(HeightScroller.Value);
            Application.Current.Resources["Timestamp"] = GetTimeStyle(HeightScroller.Value);            
        }


        //The following code hides or shows each port depending on if the filter checkbox has been checked or not.
        #region Hide and Show Ports

        void DisplayPort1A(object sender, RoutedEventArgs e)
        {
            Port1A.Width = new GridLength(1, GridUnitType.Star);
            Port1AHeader.Width = new GridLength(1, GridUnitType.Star);
        }
        void HidePort1A(object sender, RoutedEventArgs e)
        {
            Port1A.Width = new GridLength(0, GridUnitType.Star);
            Port1AHeader.Width = new GridLength(0, GridUnitType.Star);
        }
        void DisplayPort1B(object sender, RoutedEventArgs e)
        {
            Port1B.Width = new GridLength(1, GridUnitType.Star);
            Port1BHeader.Width = new GridLength(1, GridUnitType.Star);
        }
        void HidePort1B(object sender, RoutedEventArgs e)
        {
            Port1B.Width = new GridLength(0, GridUnitType.Star);
            Port1BHeader.Width = new GridLength(0, GridUnitType.Star);
        }
        

        void DisplayPort2A(object sender, RoutedEventArgs e)
        {
            Port2A.Width = new GridLength(1, GridUnitType.Star);
            Port2AHeader.Width = new GridLength(1, GridUnitType.Star);
        }
        void HidePort2A(object sender, RoutedEventArgs e)
        {
            Port2A.Width = new GridLength(0, GridUnitType.Star);
            Port2AHeader.Width = new GridLength(0, GridUnitType.Star);
        }
        void DisplayPort2B(object sender, RoutedEventArgs e)
        {
            Port2B.Width = new GridLength(1, GridUnitType.Star);
            Port2BHeader.Width = new GridLength(1, GridUnitType.Star);
        }
        void HidePort2B(object sender, RoutedEventArgs e)
        {
            Port2B.Width = new GridLength(0, GridUnitType.Star);
            Port2BHeader.Width = new GridLength(0, GridUnitType.Star);
        }


        void DisplayPort3A(object sender, RoutedEventArgs e)
        {
            Port3A.Width = new GridLength(1, GridUnitType.Star);
            Port3AHeader.Width = new GridLength(1, GridUnitType.Star);
        }
        void HidePort3A(object sender, RoutedEventArgs e)
        {
            Port3A.Width = new GridLength(0, GridUnitType.Star);
            Port3AHeader.Width = new GridLength(0, GridUnitType.Star);
        }
        void DisplayPort3B(object sender, RoutedEventArgs e)
        {
            Port3B.Width = new GridLength(1, GridUnitType.Star);
            Port3BHeader.Width = new GridLength(1, GridUnitType.Star);
        }
        void HidePort3B(object sender, RoutedEventArgs e)
        {
            Port3B.Width = new GridLength(0, GridUnitType.Star);
            Port3BHeader.Width = new GridLength(0, GridUnitType.Star);
        }


        void DisplayPort4A(object sender, RoutedEventArgs e)
        {
            Port4A.Width = new GridLength(1, GridUnitType.Star);
            Port4AHeader.Width = new GridLength(1, GridUnitType.Star);
        }
        void HidePort4A(object sender, RoutedEventArgs e)
        {
            Port4A.Width = new GridLength(0, GridUnitType.Star);
            Port4AHeader.Width = new GridLength(0, GridUnitType.Star);
        }

        void DisplayPort4B(object sender, RoutedEventArgs e)
        {
            Port4B.Width = new GridLength(1, GridUnitType.Star);
            Port4BHeader.Width = new GridLength(1, GridUnitType.Star);
        }
        void HidePort4B(object sender, RoutedEventArgs e)
        {
            Port4B.Width = new GridLength(0, GridUnitType.Star);
            Port4BHeader.Width = new GridLength(0, GridUnitType.Star);
        }


        void SelectAllPorts(object sender, RoutedEventArgs e)
        {
            Check1A.IsChecked = true;
            Check1B.IsChecked = true;
            Check2A.IsChecked = true;
            Check2B.IsChecked = true;
            Check3A.IsChecked = true;
            Check3B.IsChecked = true;
            Check4A.IsChecked = true;
            Check4B.IsChecked = true;
        }
        void DeselectAllPorts(object sender, RoutedEventArgs e)
        {
            Check1A.IsChecked = false;
            Check1B.IsChecked = false;
            Check2A.IsChecked = false;
            Check2B.IsChecked = false;
            Check3A.IsChecked = false;
            Check3B.IsChecked = false;
            Check4A.IsChecked = false;
            Check4B.IsChecked = false;
        }

        #endregion

        private void cmdBeginAnalysis_Click(object sender, RoutedEventArgs e)
        {
            Packet[] packets = controller.ParsePackets();
            AddPacketCollection(packets);
        }

    }


}