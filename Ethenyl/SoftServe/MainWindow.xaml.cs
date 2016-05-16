using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;


namespace SoftServe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string _playButton;

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

        private SpotifyLocalAPI LocalAPI;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!SpotifyLocalAPI.IsSpotifyRunning())
                SpotifyLocalAPI.RunSpotify();
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
                SpotifyLocalAPI.RunSpotifyWebHelper();
            LocalAPI = new SpotifyLocalAPI();
            LocalAPI.Connect();
            LocalAPI.ListenForEvents = true;
            LocalAPI.OnTrackTimeChange += LocalAPI_OnTrackTimeChange;
            LocalAPI.OnTrackChange += LocalAPI_OnTrackChange;
            LocalAPI.OnPlayStateChange += LocalAPI_OnPlayStateChange;

            SyncStartingData();
        }

        private void SyncStartingData()
        {
            var status = LocalAPI.GetStatus();
            PlayButton = status.Playing ? "" : "";
            if (!status.Track.IsAd())
            {
                CurrentArtist = status.Track.ArtistResource.Name;
                CurrentSong = status.Track.TrackResource.Name;
                CurrentAlbumArt = new Uri(status.Track.GetAlbumArtUrl(AlbumArtSize.Size640));

                TrackMax = status.Track.Length;
            }
            else
            {
                ProgressInd = true;
                CurrentSong = "Advertisement";
                CurrentArtist = "If only the current user paid for premium...  Oh well.";
                //CurrentAlbumArt = new Uri("NoArt.png");
            }
        }

        private void LocalAPI_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            if (e.Playing)
            {
                PlayButton = "";
            }
            else
            {
                PlayButton = "";
            }
        }

        private void LocalAPI_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            TrackStatus = e.TrackTime;
        }

        private void LocalAPI_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            if (!e.NewTrack.IsAd())
            {
                ProgressInd = false;
                CurrentSong = e.NewTrack.TrackResource.Name;
                CurrentArtist = e.NewTrack.ArtistResource.Name;
                TrackMax = e.NewTrack.Length;
                CurrentAlbumArt = new Uri(e.NewTrack.GetAlbumArtUrl(AlbumArtSize.Size640));
            }
            else
            {
                ProgressInd = true;
                CurrentSong = "Advertisement";
                CurrentArtist = "If only the current user paid for premium...  Oh well.";
                //CurrentAlbumArt = new Uri("NoArt.png");

            }
        }

        private void QueueDelete_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("LOL");
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            LocalAPI.Skip();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            LocalAPI.Previous();
        }

        private void PlayPauseClick(object sender, RoutedEventArgs e)
        {
            if (LocalAPI.GetStatus().Playing)
            {
                LocalAPI.Pause();
            }
            else
            {
                LocalAPI.Play();
            }
        }

        private async void LOL_Click(object sender, RoutedEventArgs e)
        {
            var spotify = new SpotifyWebAPI() { UseAuth = false };

            var tracks = await spotify.SearchItemsAsync("Say Something", SearchType.Track);
            var uri = tracks.Tracks.Items.First().Uri;
            var api = new SpotifyLocalAPI();
            api.Connect();

            api.PlayURL(uri);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
