using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ParkingSpotsManager.ViewModels
{
	public class CreateParkingPageViewModel : ViewModelBase
	{
        private string _name;
        public string Name
        {
            get => _name;
            set { SetProperty(ref _name, value); }
        }

        public DelegateCommand<object> CreateParkingCommand { get; private set; }

        public CreateParkingPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            CreateParkingCommand = new DelegateCommand<object>(CreateParkingAsync, CanCreateParking).ObservesProperty(() => IsAuth);
        }

        private bool CanCreateParking(object arg)
        {
            return true;
        }

        private async void CreateParkingAsync(object obj)
        {
            if (Name != null && Name.Length > 0) {
                var url = APIConstants.ParkingREST;
                var token = Prism.PrismApplicationBase.Current.Properties["authToken"].ToString();
                var json = JObject.FromObject(new Parking { Name = Name });
                using (var httpClient = new HttpClient()) {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    try {
                        var response = await httpClient.PostAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                        response.EnsureSuccessStatusCode();
                        //TODO notif
                        await NavigationService.NavigateAsync("NavigationPage/ParkingListPage");
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
