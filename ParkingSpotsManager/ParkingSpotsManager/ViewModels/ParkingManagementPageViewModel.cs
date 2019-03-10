using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace ParkingSpotsManager.ViewModels
{
	public class ParkingManagementPageViewModel : ViewModelBase, INavigationAware
	{
        private IPageDialogService _dialogService;

        private Parking _currentParking;
        public Parking CurrentParking
        {
            get => _currentParking;
            set { SetProperty(ref _currentParking, value); }
        }

        private ObservableCollection<Spot> _spotList;
        public ObservableCollection<Spot> SpotList
        {
            get => _spotList;
            set { SetProperty(ref _spotList, value); }
        }

        private bool _isRefreshingSpotList = false;
        public bool IsRefreshingSpotList
        {
            get => _isRefreshingSpotList;
            set { SetProperty(ref _isRefreshingSpotList, value); }
        }

        public DelegateCommand<Spot> TakeSpotCommand { get; set; }
        public DelegateCommand<Spot> ReleaseSpotCommand { get; set; }
        public DelegateCommand<object> RefreshSpotListCommand { get; set; }

        public ParkingManagementPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base (navigationService)
        {
            _dialogService = dialogService;
            Title = "Parking Management";
            TakeSpotCommand = new DelegateCommand<Spot>(OnTakeSpotCommandExecuted, CanTakeSpot);
            ReleaseSpotCommand = new DelegateCommand<Spot>(OnReleaseSpotCommandExecuted, CanReleaseSpot);
            RefreshSpotListCommand = new DelegateCommand<object>(OnRefreshSpotList, CanRefreshSpotList).ObservesProperty(() => CurrentParking);
        }

        private bool CanRefreshSpotList(object arg)
        {
            return CurrentParking != null;
        }

        private async void OnRefreshSpotList(object obj)
        {
            IsRefreshingSpotList = true;
            SpotList = await GetSpotListAsync(CurrentParking.Id).ConfigureAwait(false);
            IsRefreshingSpotList = false;
        }

        private bool CanReleaseSpot(Spot spot)
        {
            return true;
        }

        private async void OnReleaseSpotCommandExecuted(Spot spot)
        {
            if (spot != null && (spot.IsCurrentUserAdmin || CurrentUser.Id == spot.OccupiedBy || spot.OccupiedBy == null)) {
                spot.OccupiedAt = null;
                SpotList = new ObservableCollection<Spot>(await ChangeSpotStatus(spot).ConfigureAwait(false));
            } else {
                await _dialogService.DisplayAlertAsync("Error", "You can't modify the status of this spot !", "Close");
            }
        }

        private bool CanTakeSpot(Spot spot)
        {
            return true;
        }

        private async void OnTakeSpotCommandExecuted(Spot spot)
        {
            if (spot != null && (spot.IsCurrentUserAdmin || CurrentUser.Id == spot.OccupiedBy || spot.OccupiedBy == null)) {
                spot.OccupiedAt = DateTime.Now;
                SpotList = new ObservableCollection<Spot>(await ChangeSpotStatus(spot).ConfigureAwait(false));
            } else {
                await _dialogService.DisplayAlertAsync("Error", "You can't modify the status of this spot !", "Close");
            }
        }

        private async Task<ObservableCollection<Spot>> ChangeSpotStatus(Spot spot)
        {
            var url = APIConstants.ChangeSpotStatus(spot.Id);
            var json = JObject.FromObject(spot);
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                    var response = await httpClient.PutAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json")).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var spots = JsonConvert.DeserializeObject<ObservableCollection<Spot>>(content);

                    return spots;
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            CurrentParking = parameters.GetValue<Parking>("parking");
            SpotList = await GetSpotListAsync(CurrentParking.Id).ConfigureAwait(false);
        }

        private async Task<ObservableCollection<Spot>> GetSpotListAsync(int parkingID)
        {
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var response = await httpClient.GetAsync(APIConstants.GetParkingSpotsUrl(parkingID)).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var spots = JsonConvert.DeserializeObject<ObservableCollection<Spot>>(content);

                    return spots;
                } catch (Exception) {
                    await LogoutAsync();
                }
            }

            return null;
        }
	}
}
