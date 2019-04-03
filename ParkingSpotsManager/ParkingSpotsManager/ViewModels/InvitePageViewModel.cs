using Newtonsoft.Json;
using ParkingSpotsManager.Services;
using ParkingSpotsManager.Shared.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ParkingSpotsManager.ViewModels
{
	public class InvitePageViewModel : ViewModelBase, INavigationAware
	{
        private List<Parking> _parkingList;
        public List<Parking> ParkingList
        {
            get => _parkingList;
            set { SetProperty(ref _parkingList, value); }
        }

        private List<User> _userList;
        public List<User> UserList
        {
            get => _userList;
            set { SetProperty(ref _userList, value); }
        }

        private Parking _selectedParking;
        public Parking SelectedParking
        {
            get => _selectedParking;
            set {
                SetProperty(ref _selectedParking, value);
                OnSelectedParkingChanged(_selectedParking);
            }
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

        private IPageDialogService _dialogService;

        public InvitePageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base (navigationService)
        {
            _dialogService = dialogService;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            ParkingList = await GetParkingList().ConfigureAwait(false);
        }

        private async void OnTextChanged(string search)
        {
            if (SelectedParking != null && search != null && search.Length >= 3) {
                UserList = await GetUserList(search);
            } else {
                UserList = new List<User>();
            }
        }

        private async void OnSelectedParkingChanged(Parking selectedParking)
        {
            if (selectedParking != null && SearchText != null && SearchText.Length >= 3) {
                UserList = await GetUserList(SearchText);
            }
        }

        private async void OnUserSelected(User user)
        {
            if (user != null && SelectedParking != null) {
                var answer = await _dialogService.DisplayAlertAsync("Invitation", $"Do you want to invite {user.Username} to {SelectedParking.Name} ?", "Yes", "No");
                if (answer) {
                    var invitationSent = await SendInvitation(SelectedParking, user);
                    if (invitationSent) {
                        await _dialogService.DisplayAlertAsync("Invitation", $"{user.Username} has been invited.", "OK");
                    } else {
                        await _dialogService.DisplayAlertAsync("Error", $"An error occured, please try later.", "OK");
                    }
                    UserList = null;
                    SearchText = null;
                }
            }
        }

        private async Task<bool> SendInvitation(Parking parking, User user)
        {
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.GetAsync(API.SendInvitationUrl(parking.Id, user.Id));
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();

                    return true;
                } catch (Exception) {
                    await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
                }
            }

            return false;
        }

        private async Task<List<User>> GetUserList(string search)
        {
            //TODO: refac in a service
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.GetAsync(API.GetInvitableUsersUrl(SelectedParking.Id, search));
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<User>>(content);
                } catch (Exception) {
                    await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
                }
            }

            return null;
        }

        private async Task<List<Parking>> GetParkingList()
        {
            //TODO: refac in a service
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.GetAsync(API.GetUserParkingsUrl()).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var parkingList = JsonConvert.DeserializeObject<List<Parking>>(content);

                    return parkingList;
                } catch (Exception) {
                    await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
                }

                return null;
            }
        }
    }
}
