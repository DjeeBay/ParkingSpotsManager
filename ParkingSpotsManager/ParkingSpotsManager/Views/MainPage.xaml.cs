using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ParkingSpotsManager.Shared.Constants;

namespace ParkingSpotsManager.Views
{
    public partial class MainPage : ContentPage
    {
        private bool authenticated = false;

        public MainPage()
        {
            InitializeComponent();
        }
    }
}