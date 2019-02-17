using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism;
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
	public class ParkingEditPageViewModel : ViewModelBase, INavigationAware
	{
        private Parking _currentParking;
        public Parking CurrentParking
        {
            get => _currentParking;
            set { SetProperty(ref _currentParking, value); }
        }

        public DelegateCommand<object> SaveParkingCommand { get; }
        public DelegateCommand<object> GoToAddSpotCommand { get; }

        public ParkingEditPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            SaveParkingCommand = new DelegateCommand<object>(OnSaveParkingCommandExecutedAsync, CanSaveParking);
            GoToAddSpotCommand = new DelegateCommand<object>(OnGoToAddSpotCommandExecutedAsync, CanAddSpot);
        }

        private bool CanAddSpot(object arg)
        {
            return true;
        }

        private async void OnGoToAddSpotCommandExecutedAsync(object obj)
        {
            var navParams = new NavigationParameters();
            navParams.Add("parking", CurrentParking);
            await NavigationService.NavigateAsync("CreateSpotPage", navParams);
        }

        private bool CanSaveParking(object arg)
        {
            return true;
        }

        private async void OnSaveParkingCommandExecutedAsync(object obj)
        {
            var url = new StringBuilder(APIConstants.ParkingREST).Append("/").Append(CurrentParking.Id).ToString();
            var json = JObject.FromObject(CurrentParking);
            var token = PrismApplicationBase.Current.Properties["authToken"].ToString();
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PutAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    await NavigationService.NavigateAsync("ParkingListPage");
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            CurrentParking = parameters.GetValue<Parking>("parking");
        }
    }
}
