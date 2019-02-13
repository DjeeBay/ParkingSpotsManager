using Newtonsoft.Json;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ParkingSpotsManager.ViewModels
{
	public class ParkingListPageViewModel : ViewModelBase
	{
        private ObservableCollection<Parking> _parkings;
        public ObservableCollection<Parking> Parkings
        {
            get => _parkings;
            set { SetProperty(ref _parkings, value); }
        }

        public DelegateCommand<Parking> ShowParkingCommand { get; }
        public DelegateCommand<Parking> EditParkingCommand { get; }

        public ParkingListPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            ShowParkingCommand = new DelegateCommand<Parking>(OnShowParkingClicked, CanShowParking);
            EditParkingCommand = new DelegateCommand<Parking>(OnEditParkingClicked, CanEditParking);
            GetParkingList();
        }

        private bool CanEditParking(object arg)
        {
            //TODO check selected parking rights
            return true;
        }

        private async void OnEditParkingClicked(Parking parking)
        {
            var navParams = new NavigationParameters();
            navParams.Add("parking", parking);
            await NavigationService.NavigateAsync("ParkingEditPage", navParams);
        }

        private bool CanShowParking(object arg)
        {
            return true;
        }

        private async void OnShowParkingClicked(Parking parking)
        {
            var navParams = new NavigationParameters();
            navParams.Add("parking", parking);
            await NavigationService.NavigateAsync("ParkingManagementPage", navParams);
        }

        private async void GetParkingList()
        {
            var token = Prism.PrismApplicationBase.Current.Properties["authToken"].ToString();
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.GetAsync(APIConstants.GetUserParkingsUrl);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    Parkings = JsonConvert.DeserializeObject<ObservableCollection<Parking>>(content);
                    Console.WriteLine(Parkings.Count);

                } catch (Exception) {
                    await NavigationService.NavigateAsync("MainPage");
                }
            }
        }
	}
}
