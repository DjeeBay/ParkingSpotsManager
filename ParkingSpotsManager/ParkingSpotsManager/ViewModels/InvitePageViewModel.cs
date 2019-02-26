using Newtonsoft.Json;
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
using System.Threading.Tasks;

namespace ParkingSpotsManager.ViewModels
{
	public class InvitePageViewModel : ViewModelBase
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
            set
            { SetProperty(ref _selectedParking, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                if (value != null && value.Length >= 3) {
                    OnTextChanged(_searchText);
                }
            }
        }

        public InvitePageViewModel(INavigationService navigationService) : base (navigationService)
        {
            GetParkingList();
        }

        private async void OnTextChanged(string search)
        {
            if (SelectedParking != null) {
                UserList = await GetUserList(search);
            }
        }

        private async Task<List<User>> GetUserList(string search)
        {
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.GetAsync(APIConstants.GetInvitableUsersUrl(SelectedParking.Id, search));
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<User>>(content);
                } catch (Exception) {
                    await NavigationService.NavigateAsync("NavigationPage/MainPage");
                }
            }

            return null;
        }

        private async void GetParkingList()
        {
            //TODO: refac in a service
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.GetAsync(APIConstants.GetUserParkingsUrl);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    ParkingList = JsonConvert.DeserializeObject<List<Parking>>(content);
                } catch (Exception) {
                    await NavigationService.NavigateAsync("NavigationPage/MainPage");
                }
            }
        }
    }
}
