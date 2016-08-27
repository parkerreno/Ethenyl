using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using SoftServe.Properties;

namespace SoftServe.ViewModels
{
    public class EthenylViewModel : INotifyPropertyChanged
    {
        public EthenylViewModel()
        {
            LoadSettings();
        }

        /// <summary>
        /// Property changed notification
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private string hostName;

        public string HostName
        {
            get
            {
                if (hostName == null)
                {
                    hostName = Dns.GetHostName();
                    OnPropertyChanged();
                }
                return hostName;
            }
        }

        private ObservableCollection<string> ipAddresses;

        /// <summary>
        /// Gets a list of IPAddresses
        /// </summary>
        public ObservableCollection<string> IPAddresses
        {
            get
            {
                if (ipAddresses == null)
                {
                    ipAddresses = new ObservableCollection<string>(Dns.GetHostAddresses(HostName).Select(x=>x.ToString()));
                    OnPropertyChanged();
                }
                return ipAddresses;
            }
        }

        private PlayerViewModel player;

        /// <summary>
        /// ViewModel for info regarding the player.
        /// </summary>
        public PlayerViewModel Player
        {
            get
            {
                if (player == null)
                {
                    player = new PlayerViewModel();
                }

                return player;
            }
        }

        private int imageAdjustment;

        public int ImageAdjustment
        {
            get
            {
                return imageAdjustment;
            }
            set
            {
                if (value != imageAdjustment && value >= 0 && value < 256)
                {
                    imageAdjustment = value;
                    OnPropertyChanged();
                    this.Player.ImageAdjustment = new SolidColorBrush(Color.FromArgb(255, (byte) value, (byte) value, (byte) value));
                    Settings.Default.ImageAdjustment = value;
                    Settings.Default.Save();
                }
            }
        }

        private double blurRadius;

        public double BlurRadius
        {
            get { return blurRadius; }
            set
            {
                if (Math.Abs(value - blurRadius) > 0.1)
                {
                    blurRadius = value;
                    OnPropertyChanged();
                    this.Player.BlurEffect = new BlurEffect() { Radius = BlurRadius, RenderingBias = RenderingBias.Quality };
                    Settings.Default.BlurRadius = value;
                    Settings.Default.Save();
                }
            }
        }

        private bool usePiRGB;

        /// <summary>
        /// If PiRGB should be enabled
        /// </summary>
        public bool UsePiRGB
        {
            get
            {
                return usePiRGB;
            }
            set
            {
                if (value != usePiRGB)
                {
                    usePiRGB = value;
                    OnPropertyChanged();
                    Settings.Default.UsePiRGB = value;
                    Settings.Default.Save();
                }
            }
        }

        private string piRGBAddress;

        /// <summary>
        /// Address to use to connect to PiRGB
        /// </summary>
        public string PiRGBAddress
        {
            get { return piRGBAddress; }
            set
            {
                if (PiRGBAddress != value)
                {
                    piRGBAddress = value;
                    OnPropertyChanged();
                    Settings.Default.PiRGBAddress = value;
                    Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// Loads previously saved settings
        /// </summary>
        private void LoadSettings()
        {
            var settings = Settings.Default;
            UsePiRGB = settings.UsePiRGB;
            PiRGBAddress = settings.PiRGBAddress;
            ImageAdjustment = 1; // TODO: Remove this workaround to properly load 0 image adjust
            ImageAdjustment = settings.ImageAdjustment;
            BlurRadius = settings.BlurRadius;
        }

        /// <summary>
        /// Call when property changes
        /// </summary>
        /// <param name="propertyName">Will default to calling member, override that here</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
