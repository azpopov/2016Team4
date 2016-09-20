﻿using StarMeter.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StarMeter.Controllers;

namespace StarMeter.View
{
    public partial class PacketPopup
    {
        public PacketPopup()
        {
            InitializeComponent();
        }

        private Packet _p;
        public Controller Controller;

        public void SetupElements(Packet p)
        {
            if (!(p is RmapPacket))
            {
                ViewRmapPropertiesButton.Visibility = Visibility.Hidden;
            }
            Brush br = GetBrush(p.IsError);

            _p = p;
            Width = 500;
            Height = 500;

            lblErrorMsg.Background = br;

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();

            var converter = new System.Windows.Media.BrushConverter();

            if (!p.IsError)
            {
                logo.UriSource = new Uri("pack://application:,,,/Resources/tick.png");
                logo.EndInit();

                lblErrorMsg.Content = "SUCCESS";
            }
            else
            {

                logo.UriSource = new Uri("pack://application:,,,/Resources/Error.png");
                logo.EndInit();

                lblErrorMsg.Content = "ERROR: " + p.ErrorType;
            }


            IconBG.Background = br;
            ErrorIcon.Source = logo;

            TimeLabel.Content = p.DateRecieved.ToString("dd-MM-yyyy HH:mm:ss.fff");

            var protocolId = p.ProtocolId;

            if (protocolId == 1)
            {
                ProtocolLabel.Content = "Protocol: " + protocolId + " (RMAP)";
            }
            else
            {
                ProtocolLabel.Content = "Protocol: " + protocolId;
            }

            SequenceNumberLabel.Content = "Sequence Number: " + p.SequenceNum;

            var addressArray = p.Address;
            var finalAddressString = "";

            if (addressArray != null)
            {
                if (addressArray.Length > 1)
                {
                    finalAddressString += "Physical Path: ";
                    for (var i = 0; i < addressArray.Length - 1; i++)
                        finalAddressString += Convert.ToInt32(addressArray[i]) + "  ";
                }
                else
                    finalAddressString = "Logical Address: " + Convert.ToInt32(addressArray[0]);
            }
            else
            {
                finalAddressString = "No Address";
            }

            AddressLabel.Content = finalAddressString;

            LeftArrow.Visibility = _p.PrevPacket == null ? Visibility.Collapsed : Visibility.Visible;
            RightArrow.Visibility = _p.NextPacket == null ? Visibility.Collapsed : Visibility.Visible;
        }

        Brush GetBrush(bool isError) 
        {
            if (isError)
            {
                return Brushes.Red;
            }
            else
            {

                var converter = new System.Windows.Media.BrushConverter();

               return (Brush)converter.ConvertFromString("#6699ff");
            }
        }

        private void NextPacket(object sender, RoutedEventArgs e)
        {
            if (_p.NextPacket != null)
            {
                Packet p = Controller.FindPacket(_p.NextPacket.GetValueOrDefault());
                SetupElements(p);
            }
        }

        private void PrevPacket(object sender, RoutedEventArgs e)
        {
            if (_p.PrevPacket != null)
            {
                Packet p = Controller.FindPacket(_p.PrevPacket.GetValueOrDefault());
                SetupElements(p);
            }
        }

        private void ViewCargo(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var br = b.Background;

            if (_p.Cargo != null)
            {
                CargoView cv = new CargoView();
                cv.SetupElements(br, _p);
                cv.Owner = this;
                cv.Show();
            }
            else
            {
                MessageBox.Show("No Cargo");
            }
        }

        private void ExitButtonEvent(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowRmapProperties(object sender, RoutedEventArgs e)
        {
            var b = (Button) sender;
            var br = b.Background;

            var cv = new RmapView();
            cv.SetupElements(br, (RmapPacket)_p);
            cv.Owner = this;
            cv.Show();
        }
    }
}