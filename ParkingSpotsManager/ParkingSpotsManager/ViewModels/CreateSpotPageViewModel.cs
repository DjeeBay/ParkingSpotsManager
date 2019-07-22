using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Services;
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
using System.Threading.Tasks;

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
                var url = API.SpotREST();
                var json = JObject.FromObject(Spot);
                using (var httpClient = new HttpClient()) {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    try {
                        var response = await httpClient.PostAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                        response.EnsureSuccessStatusCode();
                        //TODO notif
                        var navParams = new NavigationParameters {
                            { "parking", CurrentParking }
                        };
                        await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingEditPage", navParams);
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var parking = parameters.GetValue<Parking>("parking");
            CurrentParking = await GetParking(parking.Id).ConfigureAwait(false);
            Spot.ParkingId = CurrentParking.Id;
        }

        private async Task<Parking> GetParking(int parkingID)
        {
            //TODO service
            var url = new StringBuilder(API.ParkingREST()).Append("/").Append(parkingID).ToString();
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var response = await httpClient.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var parking = JsonConvert.DeserializeObject<Parking>(content);

                    return parking;
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }
    }
}
