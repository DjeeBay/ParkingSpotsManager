﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Services;
using ParkingSpotsManager.Shared.Models;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ParkingSpotsManager.ViewModels
{
	public class ParkingEditPageViewModel : ViewModelBase, INavigationAware
	{
        private string _address;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private double? _latitude;
        public double? Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        private double? _longitude;
        public double? Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        private Parking _currentParking;
        public Parking CurrentParking
        {
            get => _currentParking;
            set { SetProperty(ref _currentParking, value); }
        }

        private ObservableCollection<UserParking> _userParkings;
        public ObservableCollection<UserParking> UserParkings
        {
            get => _userParkings;
            set { SetProperty(ref _userParkings, value); }
        }

        private bool _isSpotListVisible = true;
        public bool IsSpotListVisible
        {
            get => _isSpotListVisible;
            set { SetProperty(ref _isSpotListVisible, value); }
        }

        public DelegateCommand<object> SaveParkingCommand { get; }
        public DelegateCommand<object> GoToAddSpotCommand { get; }
        public DelegateCommand<Spot> EditSpotCommand { get; }
        public DelegateCommand<string> DisplayListCommand { get; }
        public DelegateCommand<UserParking> ChangeUserStatusCommand { get; }
        public DelegateCommand<UserParking> RemoveUserCommand { get; }
        public DelegateCommand<string> GetLocationCommand { get; }

        private IPageDialogService _dialogService;

        public ParkingEditPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base (navigationService)
        {
            _dialogService = dialogService;
            SaveParkingCommand = new DelegateCommand<object>(OnSaveParkingCommandExecutedAsync, CanSaveParking);
            GoToAddSpotCommand = new DelegateCommand<object>(OnGoToAddSpotCommandExecutedAsync, CanAddSpot);
            EditSpotCommand = new DelegateCommand<Spot>(EditSpotCommandExecutedAsync, CanEditSpot);
            DisplayListCommand = new DelegateCommand<string>(OnDisplayListExecuted, CanDisplayList);
            ChangeUserStatusCommand = new DelegateCommand<UserParking>(OnChangeUserStatusExecuted, CanChangeUserStatus);
            RemoveUserCommand = new DelegateCommand<UserParking>(OnRemoveUserExecuted, CanRemoveUser);
            GetLocationCommand = new DelegateCommand<string>(OnGetLocationExecuted, CanGetLocation);
        }

        private bool CanGetLocation(string arg) => true;

        private async void OnGetLocationExecuted(string obj)
        {
            if (Address != null && Address.Length > 0) {
                var locations = await Geocoding.GetLocationsAsync(Address);
                var location = locations.FirstOrDefault();
                if (location != null) {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                    CurrentParking.Latitude = location.Latitude;
                    CurrentParking.Longitude = location.Longitude;
                    CurrentParking.Address = Address;
                } else {
                    await _dialogService.DisplayAlertAsync("No address", "Geocoding system couldn't find the coordinates.", "Close");
                    Address = null;
                    Latitude = 0;
                    Longitude = 0;
                    CurrentParking.Latitude = null;
                    CurrentParking.Longitude = null;
                    CurrentParking.Address = null;
                }
            }
        }

        private bool CanRemoveUser(UserParking arg) => true;

        private async void OnRemoveUserExecuted(UserParking obj)
        {
            if (obj != null && typeof(UserParking) == obj.GetType()) {
                var answer = await _dialogService.DisplayAlertAsync("Confirm", $"Do you want to remove {obj.User.Username} from {CurrentParking.Name} ?", "Yes", "No");
                if (answer) {
                    var apiData = await RemoveUser(CurrentParking.Id, obj.UserId);
                    if (apiData != null) {
                        UserParkings = apiData;
                    }
                }
            }
        }

        private bool CanChangeUserStatus(UserParking arg)
        {
            return true;
        }

        private async void OnChangeUserStatusExecuted(UserParking obj)
        {
            if (obj != null && typeof(UserParking) == obj.GetType()) {
                obj.IsAdmin = obj.IsAdmin == 1 ? 0 : 1;
                var apiData = await ChangeUserRole(CurrentParking.Id, obj).ConfigureAwait(false);
                if (apiData != null) {
                    UserParkings = apiData;
                }
            }
        }

        private bool CanDisplayList(string arg)
        {
            return true;
        }

        private void OnDisplayListExecuted(string obj)
        {
            IsSpotListVisible = obj != null && obj == "spots";
        }

        private bool CanEditSpot(Spot spot)
        {
            return true;
        }

        private async void EditSpotCommandExecutedAsync(Spot spot)
        {
            var navParams = new NavigationParameters {
                { "spot", spot },
                { "parking", CurrentParking }
            };
            await NavigationService.NavigateAsync("SpotEditPage", navParams);
        }

        private bool CanAddSpot(object arg)
        {
            return true;
        }

        private async void OnGoToAddSpotCommandExecutedAsync(object obj)
        {
            var navParams = new NavigationParameters {
                { "parking", CurrentParking }
            };
            await NavigationService.NavigateAsync("CreateSpotPage", navParams);
        }

        private bool CanSaveParking(object arg)
        {
            return true;
        }

        private async void OnSaveParkingCommandExecutedAsync(object obj)
        {
            var url = new StringBuilder(API.ParkingREST()).Append("/").Append(CurrentParking.Id).ToString();
            var json = JObject.FromObject(CurrentParking);
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.PutAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var parking = parameters.GetValue<Parking>("parking");

            if (parking != null) {
                CurrentParking = await GetParking(parking.Id).ConfigureAwait(false);
                UserParkings = new ObservableCollection<UserParking>(CurrentParking.UserParkings);
                Address = CurrentParking.Address;
                Latitude = CurrentParking.Latitude;
                Longitude = CurrentParking.Longitude;
            } else {
                await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
            }
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

        private async Task<ObservableCollection<UserParking>> ChangeUserRole(int parkingID, UserParking userParking)
        {
            //TODO service
            var url = API.ChangeParkingUserRoleUrl(parkingID);
            var json = JObject.FromObject(userParking);
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var response = await httpClient.PostAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json")).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var userParkings = JsonConvert.DeserializeObject<ObservableCollection<UserParking>>(content);

                    return userParkings;
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }

        private async Task<ObservableCollection<UserParking>> RemoveUser(int parkingID, int userID)
        {
            //TODO service
            var url = API.RemoveParkingUserUrl(parkingID, userID);
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var response = await httpClient.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var userParkings = JsonConvert.DeserializeObject<ObservableCollection<UserParking>>(content);

                    return userParkings;
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }
    }
}
