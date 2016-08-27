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
using SoftServe.Properties;

namespace SoftServe.ViewModels
{
    public class EthenylViewModel : INotifyPropertyChanged
    {
        public EthenylViewModel()
        {
            LoadSettings();
            HostName = Dns.GetHostName();
        }

        /// <summary>
        /// Property changed notification
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private string hostName;

        public string HostName
        {
            get { return hostName; }
            set
            {
                if (hostName != value)
                {
                    hostName = value;
                    OnPropertyChanged();
                }
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
