using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ParkingSpotsManager.ViewModels
{
    public class SpotEditPageViewModel : ViewModelBase
    {
        private Spot _currentSpot;
        public Spot CurrentSpot
        {
            get => _currentSpot;
            set { SetProperty(ref _currentSpot, value); }
        }

        public DelegateCommand<object> SaveSpotCommand { get; }
        public DelegateCommand<object> DeleteSpotCommand { get; }

        public SpotEditPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            SaveSpotCommand = new DelegateCommand<object>(OnSaveSpotCommandExecutedAsync, CanSaveSpot);
            DeleteSpotCommand = new DelegateCommand<object>(OnDeleteSpotCommandExecutedAsync, CanDeleteSpot);
        }

        private bool CanDeleteSpot(object arg)
        {
            return true;
        }

        private async void OnDeleteSpotCommandExecutedAsync(object obj)
        {
        }

        private bool CanSaveSpot(object arg)
        {
            return true;
        }

        private async void OnSaveSpotCommandExecutedAsync(object obj)
        {
            var url = new StringBuilder(APIConstants.SpotREST).Append("/").Append(CurrentSpot.Id).ToString();
            var json = JObject.FromObject(CurrentSpot);
            var token = PrismApplicationBase.Current.Properties["authToken"].ToString();
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PutAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    await NavigationService.NavigateAsync("ParkingListPage");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            CurrentSpot = parameters.GetValue<Spot>("spot");
        }

    }
}
