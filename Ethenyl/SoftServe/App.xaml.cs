using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SoftServe.ViewModels;

namespace SoftServe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static EthenylViewModel viewModel;

        public static EthenylViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = new EthenylViewModel();
                }

                return viewModel;
            }
        }
    }
}
