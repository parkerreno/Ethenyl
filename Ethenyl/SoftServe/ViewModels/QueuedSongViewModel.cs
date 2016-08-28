using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SoftServe.ViewModels
{
    public class QueuedSongViewModel : INotifyPropertyChanged
    {
        public QueuedSongViewModel(string trackName, string trackArtist, string spotifyUri, string queuedBy)
        {
            TrackName = trackName;
            TrackArtist = trackArtist;
            SpotifyUri = spotifyUri;
            QueuedBy = queuedBy;
        }

        private string _trackName;

        public string TrackName
        {
            get
            {
                return _trackName;
            }
            private set
            {
                if (value != _trackName)
                {
                    _trackName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _trackArtist;
        public string TrackArtist {
            get
            {
                return _trackArtist;
            }
            private set
            {
                if (value != _trackArtist)
                {
                    _trackArtist = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _spotifyUri;
        public string SpotifyUri {
            get
            {
                return _spotifyUri;
            }
            private set
            {
                if (value != _spotifyUri)
                {
                    _spotifyUri = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _queuedBy;

        public string QueuedBy
        {
            get
            {
                return _queuedBy;
            }
            private set
            {
                if (value != _queuedBy)
                {
                    _queuedBy = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
