using Newtonsoft.Json;
using ParkingSpotsManager.Services;
using ParkingSpotsManager.Shared.Models;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace ParkingSpotsManager.ViewModels
{
    public class MapPageViewModel : ViewModelBase, INavigationAware, IPageLifecycleAware
    {
        private ObservableCollection<Parking> _parkingList;
        public ObservableCollection<Parking> ParkingList
        {
            get => _parkingList;
            set { SetProperty(ref _parkingList, value); }
        }

        private ObservableCollection<Pin> _locations;
        public ObservableCollection<Pin> Locations
        {
            get => _locations;
            set { SetProperty(ref _locations, value); }
        }

        private Location _currentLocation;
        public Location CurrentLocation
        {
            get => _currentLocation;
            set { SetProperty(ref _currentLocation, value); }
        }

        private bool _isRefreshingData = false;
        public bool IsRefreshingData
        {
            get => _isRefreshingData;
            set { SetProperty(ref _isRefreshingData, value); }
        }

        public DelegateCommand<object> RefreshMapDataCommand { get; }

        public MapPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            RefreshMapDataCommand = new DelegateCommand<object>(OnRefreshClicked);
        }

        private async void OnRefreshClicked(object obj)
        {
            await RefreshData();
        }

        private async Task RefreshData()
        {
            IsRefreshingData = true;
            CurrentLocation = await Geolocation.GetLastKnownLocationAsync();
            ParkingList = await GetParkingList().ConfigureAwait(false);
            Locations = new ObservableCollection<Pin>();
            foreach (var parking in ParkingList) {
                if (parking.Address != null && parking.Latitude != null && parking.Longitude != null) {
                    var nbOccupied = 0;
                    foreach (var spot in parking.Spots) {
                        if (spot.OccupiedBy != null) {
                            nbOccupied++;
                        }
                    }
                    var nbAvailable = parking.Spots.Count - nbOccupied;
                    var label = new StringBuilder(parking.Name).Append(" (").Append(nbAvailable.ToString()).Append(" free)");
                    Locations.Add(new Pin {
                        Position = new Position((double)parking.Latitude, (double)parking.Longitude),
                        Address = parking.Address,
                        Label = label.ToString(),
                        Type = PinType.Place
                    });
                }
            }
            IsRefreshingData = false;
        }

        private async Task<ObservableCollection<Parking>> GetParkingList()
        {
            //TODO: refac in a service
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var response = await httpClient.GetAsync(API.GetUserParkingsUrl()).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var parkings = JsonConvert.DeserializeObject<ObservableCollection<Parking>>(content);

                    return parkings;
                } catch (Exception) {
                    await LogoutAsync();
                }
            }

            return null;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            await RefreshData();
        }

        public void OnAppearing()
        {
            return;
        }

        public void OnDisappearing()
        {
            return;
        }
    }
}
