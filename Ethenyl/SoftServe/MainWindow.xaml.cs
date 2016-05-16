﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Effects;
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

        public BlurEffect BlurEffect { get; set; } = new BlurEffect() {Radius = 57, RenderingBias = RenderingBias.Quality};

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

        private double _maxEdge;

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

        private SpotifyLocalAPI _localApi;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!SpotifyLocalAPI.IsSpotifyRunning())
                SpotifyLocalAPI.RunSpotify();
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
                SpotifyLocalAPI.RunSpotifyWebHelper();
            _localApi = new SpotifyLocalAPI();
            _localApi.Connect();
            _localApi.ListenForEvents = true;

            _localApi.OnTrackTimeChange += LocalAPI_OnTrackTimeChange;
            _localApi.OnTrackChange += LocalAPI_OnTrackChange;
            _localApi.OnPlayStateChange += LocalAPI_OnPlayStateChange;
            SizeChanged += MainWindow_SizeChanged;

            MaxEdge = ActualWidth > ActualHeight ? ActualWidth : ActualHeight;

            SyncStartingData();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height > e.NewSize.Width)
            {
                MaxEdge = e.NewSize.Height*1.05;
            }
            else
            {
                MaxEdge = e.NewSize.Width*1.05;
            }
        }

        private void SyncStartingData()
        {
            var status = _localApi.GetStatus();
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
            _localApi.Skip();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _localApi.Previous();
        }

        private void PlayPauseClick(object sender, RoutedEventArgs e)
        {
            if (_localApi.GetStatus().Playing)
            {
                _localApi.Pause();
            }
            else
            {
                _localApi.Play();
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