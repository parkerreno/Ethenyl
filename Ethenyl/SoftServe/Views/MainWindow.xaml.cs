using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using SoftServe.Helpers;
using SoftServe.PCL;
using SoftServe.ViewModels;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Web;

namespace SoftServe.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Properties & Members

        private PlayerViewModel _playerModel;

        private SpotifyLocalAPI _localApi;
        private SoftServeDB _db;

        /// <summary>
        /// Used to limit user to one settings window to avoid confusion
        /// </summary>
        private SettingsWindow _settingsWindow;

        #endregion

        /// <summary>
        /// Creates a new main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            _playerModel = (DataContext as EthenylViewModel).Player;

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SoftServe/SoftServe.db");
            _db = new SoftServeDB(path);

            QueueList.ItemsSource = _playerModel.SongQueue;

            InitializeSpotifyApi();
            SyncStartingData();

            SizeChanged += MainWindow_SizeChanged;
            _playerModel.MaxEdge = ActualWidth > ActualHeight ? ActualWidth : ActualHeight;
            
            _settingsWindow = new SettingsWindow();

            SocketListener sl = new SocketListener("5452");
            sl.ConnectionReceived += Sl_ConnectionReceived;
        }

        /// <summary>
        /// Initializes the local spotify api and starts listening to various events
        /// </summary>
        public void InitializeSpotifyApi()
        {
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
        }
        
        /// <summary>
        /// Handles connections from the socketlistener *this will be changed in the future*
        /// </summary>
        /// <param name="e">Queue of commands</param>
        private void Sl_ConnectionReceived(object sender, Queue<string> e)
        {
            if (e.Count % 2 != 0)
            {
                var mbres = MessageBox.Show("A malformed instruction was received - try to parse? (This could crash SoftServe)",
                "Client Messaging Error", MessageBoxButton.YesNo);
                if (mbres == MessageBoxResult.No || mbres == MessageBoxResult.Cancel || mbres == MessageBoxResult.None)
                {
                    return;
                }
            }

            string currentUser;
            int currentUserId;
            if (e.Peek().ToUpper() != "ACTIONBY" && e.Peek().ToUpper() != "REGISTERUSER")
            {
                MessageBox.Show("A command was sent without authentication and was ignored.");
                return;
            }
            else
            {
                e.Dequeue(); // dequeue the command
                string[] userData = e.Dequeue().Split(':');
                if (userData.Length != 2)
                {
                    return; // Malformed args (not just username:PIN)
                }
                currentUser = userData[0];
                if (!_db.TryAuthenticateUser(userData[0], userData[1], out currentUserId) && !_db.TryCreateUser(userData[0], userData[1], out currentUserId))
                {
                    return; // bad credentials
                }
            }

            if (e.Count > 0 && e.Count % 2 == 0)
            {
                HandleCommands(e, currentUser, currentUserId);
            }
        }

        /// <summary>
        /// Command processor - can handle any number number of command pairs
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="authUser">Username of *authorized* user</param>
        private void HandleCommands(Queue<string> commands, string authUser, int userId)
        {
            switch (commands.Dequeue().ToUpper())
            {
                case "QUEUEID":
                    string id = commands.Dequeue();
                    AddIdToQueue(id, authUser);
                    _db.QueueSong(userId, id);
                    break;
                case "PAUSESONG":
                    //TODO:Special authentication for play controls
                    _localApi.Pause();
                    MessageBox.Show($"{authUser} paused the music.", "Music Paused");
                    commands.Dequeue(); // dequeue null arguments.
                    break;
                case "PLAYPAUSE":
                    //TODO: check permissions
                    PlayPauseClick(null,null);
                    commands.Dequeue();
                    break;
                case "SKIPSONG":
                    //TODO: Special auth
                    _localApi.Skip();
                    commands.Dequeue();// dequeue null arguments.
                    break;
                case "FORCEDEQUEUE": // Can be used to get out of bad state
                    // TODO: Check permissions
                    PlayNextSong();
                    commands.Dequeue();
                    break;
                default:
                    MessageBox.Show("Unknown Command from Client");
                    break;
            }

            if (commands.Count > 0 && commands.Count % 2 ==0)
            {
                HandleCommands(commands, authUser, userId);
            }
        }

        /// <summary>
        /// Resizes the background album art when the window size changes
        /// </summary>
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height > e.NewSize.Width)
            {
                _playerModel.MaxEdge = e.NewSize.Height * 1.05; // Make it a little bigger to avoid white borders
            }
            else
            {
                _playerModel.MaxEdge = e.NewSize.Width * 1.05;
            }
        }

        /// <summary>
        /// Get the initial track data until we get a TrackChanged
        /// </summary>
        private void SyncStartingData()
        {
            if (!SpotifyLocalAPI.IsSpotifyRunning() || !SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                MessageBox.Show(
                    "It appears Spotify failed to launch in time or is not installed.  Please restart Ethenyl to try again.", "Error Starting Ethenyl");
                Application.Current.Shutdown();
            }
            var status = _localApi.GetStatus();
            _playerModel.PlayButton = status.Playing ? "" : ""; // Pause button, play button
            if (status.Track == null || status.Track.IsAd())
            {
                _playerModel.ProgressInd = true;
                _playerModel.CurrentSong = "Advertisement";
                _playerModel.CurrentArtist = "If only the current user paid for premium...  Oh well.";
                //CurrentAlbumArt = new Uri("NoArt.png");
            }
            else
            {
                _playerModel.CurrentArtist = status.Track.ArtistResource.Name;
                _playerModel.CurrentSong = status.Track.TrackResource.Name;
                _playerModel.CurrentAlbumArt = new Uri(status.Track.GetAlbumArtUrl(AlbumArtSize.Size640));

                _playerModel.TrackMax = status.Track.Length;
            }
        }

        /// <summary>
        /// Handles playstate changes (basically just changes the icon on the play/pause button)
        /// </summary>
        private void LocalAPI_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            _playerModel.PlayButton = e.Playing ? "" : "";
        }

        /// <summary>
        /// Handles track time changes (changes playing position)
        /// </summary>
        private void LocalAPI_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            _playerModel.TrackStatus = e.TrackTime;
        }

        /// <summary>
        /// Handles track changing (currently the home of queueing logic)
        /// </summary>
        private async void LocalAPI_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            var status = _localApi.GetStatus();

            if (!e.NewTrack.IsAd() && !status.Playing  && _playerModel.SongQueue.Count > 0) // requirements for queueing a new song
            {
                PlayNextSong();
            }
            else if (!e.NewTrack.IsAd()) // update now playing info
            {
                _playerModel.ProgressInd = false;
                _playerModel.CurrentSong = e.NewTrack.TrackResource.Name;
                _playerModel.CurrentArtist = e.NewTrack.ArtistResource.Name;
                _playerModel.TrackMax = e.NewTrack.Length;
                _playerModel.CurrentAlbumArt = new Uri(e.NewTrack.GetAlbumArtUrl(AlbumArtSize.Size640));
                bool useRgb = App.ViewModel.UsePiRGB;
                string host = App.ViewModel.HostName;

                if (useRgb) //RGB Lighting communication
                {
                    var color = BitmapProcessor.AveragesAreSometimesCool(e.NewTrack.GetAlbumArt(AlbumArtSize.Size160));
                    TcpClient client = new TcpClient();
                    try
                    {
                        await client.ConnectAsync(host, 5453);
                        using (var writer = new StreamWriter(client.GetStream()))
                        {
                            await writer.WriteLineAsync("SETRGB");
                            await writer.WriteLineAsync($"{color.R}:{color.G}:{color.B}");
                            await writer.WriteLineAsync("ENDTRANSMISSION");
                            await writer.FlushAsync();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else //Can't get data for ads
            {
                _playerModel.ProgressInd = true;
                _playerModel.CurrentSong = "Advertisement";
                _playerModel.CurrentArtist = "If only the current user paid for premium...  Oh well.";
            }
        }

        /// <summary>
        /// Plays next song in the queue
        /// </summary>
        private void PlayNextSong()
        {
            var toPlay = _playerModel.SongQueue[0];
            _localApi.PlayURL(toPlay.SpotifyUri);

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate ()
            {
                _playerModel.SongQueue.Remove(toPlay);
            });
        }

        /// <summary>
        /// Forward/ skip button handler
        /// </summary>
        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            _localApi.Skip();
        }

        /// <summary>
        /// Back/ previous button handler
        /// </summary>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _localApi.Previous();
        }

        /// <summary>
        /// Play/pause button handler
        /// </summary>
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

        /// <summary>
        /// Add to queue from ID *may be moved with command handling logic*
        /// </summary>
        /// <param name="SID"></param>
        /// <param name="username"></param>
        private void AddIdToQueue(string SID, string username)
        {
            var spotify = new SpotifyWebAPI() { UseAuth = false };
            var track = spotify.GetTrack(SID);
            var status = _localApi.GetStatus();

            if (!status.Playing && _playerModel.SongQueue.Count < 1) // If queue is empty and new song is queued, play it!
            {
                _localApi.PlayURL(track.Uri);
                return;
            }

            var newSong = new QueuedSongViewModel(track.Name, track.Artists.First().Name, track.Uri, username);

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate ()
            {
                _playerModel.SongQueue.Add(newSong);
            });
        }

        /// <summary>
        /// Close additional windows when main window is closed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            _settingsWindow.Close(); // Close settings window if open.
            base.OnClosing(e);
        }

        /// <summary>
        /// Handles click of settings button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow.Close(); // Closes and reopens the window to bring into focus
            _settingsWindow = new SettingsWindow();
            _settingsWindow.Show();
        }

        /// <summary>
        /// Toggles between fullscreen and normal window mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullscreenToggle_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowStyle == WindowStyle.None)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
        }

        #region INPC Boilerplate
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
