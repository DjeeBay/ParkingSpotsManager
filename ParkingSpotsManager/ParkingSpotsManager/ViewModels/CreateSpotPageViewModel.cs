using Newtonsoft.Json;
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
	public class CreateSpotPageViewModel : ViewModelBase, INavigationAware
	{
        private Parking _currentParking;
        public Parking CurrentParking
        {
            get => _currentParking;
            set { SetProperty(ref _currentParking, value); }
        }

        private Spot _spot;
        public Spot Spot {
            get => _spot;
            set { SetProperty(ref _spot, value); }
        }

        public DelegateCommand<object> CreateSpotCommand { get; set; }

        public CreateSpotPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            Spot = new Spot();
            CreateSpotCommand = new DelegateCommand<object>(OnCreateSpotCommandExecutedAsync, CanCreateSpot);
        }

        private bool CanCreateSpot(object arg)
        {
            return true;
        }

        private async void OnCreateSpotCommandExecutedAsync(object obj)
        {
            if (Spot.Name != null && Spot.Name.Length > 0) {
                var url = APIConstants.SpotREST;
                var token = Prism.PrismApplicationBase.Current.Properties["authToken"].ToString();
                var json = JObject.FromObject(Spot);
                using (var httpClient = new HttpClient()) {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    try {
                        var response = await httpClient.PostAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync();
                        var createdSpot = JsonConvert.DeserializeObject<Spot>(content);
                        CurrentParking.Spots.Add(createdSpot);
                        //TODO notif
                        var navParams = new NavigationParameters();
                        navParams.Add("parking", CurrentParking);
                        await NavigationService.NavigateAsync("ParkingEditPage", navParams);
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var parking = parameters.GetValue<Parking>("parking");
            CurrentParking = parking;
            Spot.ParkingId = parking.Id;
        }
    }
}
