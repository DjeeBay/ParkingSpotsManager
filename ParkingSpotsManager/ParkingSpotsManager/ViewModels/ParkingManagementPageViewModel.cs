using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace ParkingSpotsManager.ViewModels
{
	public class ParkingManagementPageViewModel : ViewModelBase, INavigationAware
	{
        private Parking _currentParking;
        public Parking CurrentParking
        {
            get => _currentParking;
            set { SetProperty(ref _currentParking, value); }
        }

        public DelegateCommand<Spot> ChangeSpotStatusCommand { get; set; }

        public ParkingManagementPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            Title = "Parking Management";
            ChangeSpotStatusCommand = new DelegateCommand<Spot>(OnChangeSpotStatusCommandExecuted, CanChangeSpotStatus);
        }

        private bool CanChangeSpotStatus(Spot arg)
        {
            //TODO verify user rights
            return true;
        }

        private async void OnChangeSpotStatusCommandExecuted(Spot spot)
        {
            var url = new StringBuilder(APIConstants.SpotREST).Append("/").Append(spot.Id).ToString();
            var json = JObject.FromObject(spot);
            var token = PrismApplicationBase.Current.Properties["authToken"].ToString();
            using (var httpClient = new HttpClient())
            {
                try
                {
                    //TODO make a specific method in controller that update occupier and return the updated spot.
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PutAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
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
            CurrentParking = parameters.GetValue<Parking>("parking");
            SetSpotsProperties();
        }

        private void SetSpotsProperties()
        {
            CurrentParking.Spots.ForEach(s => s.IsCurrentUserAdmin = CurrentParking.IsCurrentUserAdmin);
        }
	}
}
