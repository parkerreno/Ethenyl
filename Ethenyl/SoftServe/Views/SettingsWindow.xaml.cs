using System.Windows;
using Newtonsoft.Json;
using SoftServe.ViewModels;

namespace SoftServe.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private EthenylViewModel viewModel;

        /// <summary>
        /// Initializes the settings window
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            viewModel = App.ViewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Displays JSON representation of view model for easy debugging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModelGet_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(JsonConvert.SerializeObject(App.ViewModel, Formatting.Indented), "Current ViewModel Information");
        }
    }
}
