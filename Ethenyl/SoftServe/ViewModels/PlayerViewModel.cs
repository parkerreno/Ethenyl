using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;

namespace SoftServe.ViewModels
{
    public class PlayerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Blur effect used for background album art on the player
        /// </summary>
        public BlurEffect BlurEffect { get; private set; } = new BlurEffect() { Radius = 57, RenderingBias = RenderingBias.Quality };

        private string _playButton;

        /// <summary>
        /// Text on play button
        /// </summary>
        public string PlayButton
        {
            get
            {
                return _playButton;
            }
            set
            {
                if (value != _playButton)
                {
                    _playButton = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _currentSong;

        /// <summary>
        /// Title of current song
        /// </summary>
        public string CurrentSong
        {
            get { return _currentSong; }
            set
            {
                if (value != _currentSong)
                {
                    _currentSong = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _currentArtist;

        /// <summary>
        /// Name of artist for current song
        /// </summary>
        public string CurrentArtist
        {
            get
            {
                return _currentArtist;
            }
            set
            {
                if (value != _currentArtist)
                {
                    _currentArtist = value;
                    OnPropertyChanged();
                }
            }
        }

        private Uri _currentAlbumArt;

        /// <summary>
        /// Album art uri for the currently playing song
        /// </summary>
        public Uri CurrentAlbumArt
        {
            get { return _currentAlbumArt; }
            set
            {
                if (!value.Equals(_currentAlbumArt))
                {
                    _currentAlbumArt = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _trackStatus;

        /// <summary>
        /// Current place in track (in seconds, I think)
        /// </summary>
        public double TrackStatus
        {
            get
            {
                return _trackStatus;
            }
            set
            {
                if (value != _trackStatus)
                {
                    _trackStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _trackMax;

        /// <summary>
        /// Max value for <see cref="TrackStatus"/> 
        /// </summary>
        public double TrackMax
        {
            get
            {
                return _trackMax;
            }
            set
            {
                if (_trackMax != value)
                {
                    _trackMax = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _progressInd;

        /// <summary>
        /// Is progress bar indeterminate
        /// </summary>
        /// <remarks>Used mainly for when ads are playing</remarks>
        public bool ProgressInd
        {
            get
            {
                return _progressInd;
            }
            set
            {
                if (value != _progressInd)
                {
                    _progressInd = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _maxEdge;

        /// <summary>
        /// Length of longer edge of player
        /// </summary>
        /// <remarks>
        /// Used to scale background image
        /// </remarks>
        public double MaxEdge
        {
            get { return _maxEdge; }
            set
            {
                if (value != _maxEdge)
                {
                    _maxEdge = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<QueuedSong> _songQueue;

        /// <summary>
        /// Current list of queued songs
        /// </summary>
        public ObservableCollection<QueuedSong> SongQueue
        {
            get
            {
                if (_songQueue == null)
                {
                    _songQueue = new ObservableCollection<QueuedSong>();
                }
                return _songQueue;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
