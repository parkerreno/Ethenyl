using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            var settings = Settings.Default;
            UsePiRGB = settings.UsePiRGB;
        }

        /// <summary>
        /// Property changed notification
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
