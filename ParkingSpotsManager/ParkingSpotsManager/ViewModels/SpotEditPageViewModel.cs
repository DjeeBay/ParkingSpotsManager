using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Services;
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

        private User _defaultOccupier;
        public User DefaultOccupier
        {
            get => _defaultOccupier;
            set { SetProperty(ref _defaultOccupier, value); }
        }

        private Parking _currentParking;
        public Parking CurrentParking
        {
            get => _currentParking;
            set { SetProperty(ref _currentParking, value); }
        }

        private List<User> _userList;
        public List<User> UserList
        {
            get => _userList;
            set { SetProperty(ref _userList, value); }
        }

        private bool _isOccupiedByDefault;
        public bool IsOccupiedByDefault
        {
            get => _isOccupiedByDefault;
            set
            {
                SetProperty(ref _isOccupiedByDefault, value);
                CurrentSpot.IsOccupiedByDefault = _isOccupiedByDefault;
            }
        }

        private bool _hasDefaultOccupier;
        public bool HasDefaultOccupier
        {
            get => _hasDefaultOccupier;
            set { SetProperty(ref _hasDefaultOccupier, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                OnTextChanged(_searchText);
            }
        }

        private User _userSelected;
        public User UserSelected
        {
            get => _userSelected;
            set
            {
                SetProperty(ref _userSelected, value);
                OnUserSelected(_userSelected);
            }
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
                var url = new StringBuilder(API.SpotREST()).Append("/").Append(CurrentSpot.Id).ToString();
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
            var url = new StringBuilder(API.SpotREST()).Append("/").Append(CurrentSpot.Id).ToString();
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

        private async void OnUserSelected(User user)
        {
            if (user != null) {
                var answer = await _dialogService.DisplayAlertAsync("Occupier", $"Do you want to set {user.Username} as the default occupier ?", "Yes", "No");
                if (answer) {
                    var updatedSpot = await SetDefaultOccupier(CurrentSpot, user);
                    if (updatedSpot != null) {
                        await _dialogService.DisplayAlertAsync("Occupier", $"{user.Username} is the default occupier.", "OK");
                    }
                    else {
                        await _dialogService.DisplayAlertAsync("Error", $"An error occured, please try later.", "OK");
                    }
                    UserList = null;
                    SearchText = null;
                }
            }
        }

        private async Task<Spot> SetDefaultOccupier(Spot spot, User user)
        {
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.GetAsync(API.SetDefaultOccupier(spot.Id, user.Id));
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    Spot updatedSpot = JsonConvert.DeserializeObject<Spot>(content);
                    HasDefaultOccupier = updatedSpot != null && updatedSpot.OccupiedByDefaultBy != null;
                    if (HasDefaultOccupier) {
                        DefaultOccupier = await GetDefaultOccupier(spot.Id);
                        CurrentSpot.OccupiedByDefaultBy = DefaultOccupier.Id;
                    }

                    return updatedSpot;
                }
                catch (Exception) {
                    await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
                }
            }

            return null;
        }

        private async void OnTextChanged(string search)
        {
            if (search != null && search.Length >= 3) {
                UserList = await GetUserList(CurrentParking.Id, search);
            } else {
                UserList = new List<User>();
            }
        }

        private async Task<List<User>> GetUserList(int parkingID, string search)
        {
            //TODO: refac in a service
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.GetAsync(API.GetParkingUserListUrl(parkingID, search));
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<User>>(content);
                } catch (Exception) {
                    await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
                }
            }

            return null;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var spot = parameters.GetValue<Spot>("spot");
            CurrentParking = parameters.GetValue<Parking>("parking");
            CurrentSpot = await GetSpot(spot.Id).ConfigureAwait(false);
            IsOccupiedByDefault = CurrentSpot != null ? CurrentSpot.IsOccupiedByDefault : false;
            HasDefaultOccupier = CurrentSpot != null && CurrentSpot.OccupiedByDefaultBy != null;
            if (HasDefaultOccupier) {
                DefaultOccupier = await GetDefaultOccupier(spot.Id);
                CurrentSpot.OccupiedByDefaultBy = DefaultOccupier.Id;
            }
        }

        private async Task<Spot> GetSpot(int spotID)
        {
            //TODO service
            var url = new StringBuilder(API.SpotREST()).Append("/").Append(spotID).ToString();
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

        private async Task<User> GetDefaultOccupier(int spotID)
        {
            //TODO service
            var url = new StringBuilder(API.GetDefaultOccupierUrl(spotID)).ToString();
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var response = await httpClient.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var occupier = JsonConvert.DeserializeObject<User>(content);

                    return occupier;
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }
    }
}
