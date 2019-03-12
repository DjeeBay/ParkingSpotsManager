using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism;
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

namespace ParkingSpotsManager.ViewModels
{
	public class ParkingEditPageViewModel : ViewModelBase, INavigationAware
	{
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

        public ParkingEditPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            SaveParkingCommand = new DelegateCommand<object>(OnSaveParkingCommandExecutedAsync, CanSaveParking);
            GoToAddSpotCommand = new DelegateCommand<object>(OnGoToAddSpotCommandExecutedAsync, CanAddSpot);
            EditSpotCommand = new DelegateCommand<Spot>(EditSpotCommandExecutedAsync, CanEditSpot);
            DisplayListCommand = new DelegateCommand<string>(OnDisplayListExecuted, CanDisplayList);
            ChangeUserStatusCommand = new DelegateCommand<UserParking>(OnChangeUserStatusExecuted, CanChangeUserStatus);
        }

        private bool CanChangeUserStatus(UserParking arg)
        {
            return true;
        }

        private void OnChangeUserStatusExecuted(UserParking obj)
        {
            //TODO call API
            if (obj != null && typeof(UserParking) == obj.GetType()) {
                obj.IsAdmin = 0;
                UserParkings = new ObservableCollection<UserParking>(UserParkings.ToList());
            }
            Console.WriteLine(obj);
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
            var url = new StringBuilder(APIConstants.ParkingREST).Append("/").Append(CurrentParking.Id).ToString();
            var json = JObject.FromObject(CurrentParking);
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.PutAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json")).ConfigureAwait(false);
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
            } else {
                await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
            }
        }

        private async Task<Parking> GetParking(int parkingID)
        {
            //TODO service
            var url = new StringBuilder(APIConstants.ParkingREST).Append("/").Append(parkingID).ToString();
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
