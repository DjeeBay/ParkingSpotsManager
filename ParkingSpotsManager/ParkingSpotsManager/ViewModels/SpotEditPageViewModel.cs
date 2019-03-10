using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSpotsManager.ViewModels
{
    public class SpotEditPageViewModel : ViewModelBase
    {
        private IPageDialogService _dialogService;

        private Spot _currentSpot;
        public Spot CurrentSpot
        {
            get => _currentSpot;
            set { SetProperty(ref _currentSpot, value); }
        }

        private Parking _currentParking;
        public Parking CurrentParking
        {
            get => _currentParking;
            set { SetProperty(ref _currentParking, value); }
        }

        public DelegateCommand<object> SaveSpotCommand { get; }
        public DelegateCommand<object> DeleteSpotCommand { get; }

        public SpotEditPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
            SaveSpotCommand = new DelegateCommand<object>(OnSaveSpotCommandExecutedAsync, CanSaveSpot);
            DeleteSpotCommand = new DelegateCommand<object>(OnDeleteSpotCommandExecutedAsync, CanDeleteSpot);
        }

        private bool CanDeleteSpot(object arg)
        {
            return true;
        }

        private async void OnDeleteSpotCommandExecutedAsync(object obj)
        {
            var confirmdelete = await _dialogService.DisplayAlertAsync("Delete the spot", "Are you sure ?", "Yes", "No");
            if (confirmdelete) {
                var url = new StringBuilder(APIConstants.SpotREST).Append("/").Append(CurrentSpot.Id).ToString();
                using (var httpClient = new HttpClient()) {
                    try {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                        var response = await httpClient.DeleteAsync(url);
                        response.EnsureSuccessStatusCode();
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

        private bool CanSaveSpot(object arg)
        {
            return true;
        }

        private async void OnSaveSpotCommandExecutedAsync(object obj)
        {
            var url = new StringBuilder(APIConstants.SpotREST).Append("/").Append(CurrentSpot.Id).ToString();
            var json = JObject.FromObject(CurrentSpot);
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.PutAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var navParams = new NavigationParameters {
                        { "parking", CurrentParking }
                    };
                    await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingEditPage", navParams);
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var spot = parameters.GetValue<Spot>("spot");
            CurrentParking = parameters.GetValue<Parking>("parking");
            CurrentSpot = await GetSpot(spot.Id).ConfigureAwait(false);
        }

        private async Task<Spot> GetSpot(int spotID)
        {
            //TODO service
            var url = new StringBuilder(APIConstants.SpotREST).Append("/").Append(spotID).ToString();
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var response = await httpClient.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var spot = JsonConvert.DeserializeObject<Spot>(content);

                    return spot;
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }
    }
}
