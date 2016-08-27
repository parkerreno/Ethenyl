using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using SoftServe.ViewModels;

namespace SoftServe
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
