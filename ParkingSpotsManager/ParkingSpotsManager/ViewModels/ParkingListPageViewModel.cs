﻿using Newtonsoft.Json;
using ParkingSpotsManager.Services;
using ParkingSpotsManager.Shared.Constants;
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
using System.Threading.Tasks;

namespace ParkingSpotsManager.ViewModels
{
	public class ParkingListPageViewModel : ViewModelBase, INavigationAware, IPageLifecycleAware
    {
        private ObservableCollection<Parking> _parkingList;
        public ObservableCollection<Parking> ParkingList
        {
            get => _parkingList;
            set { SetProperty(ref _parkingList, value); }
        }

        private Parking _selectedParking;
        public Parking SelectedParking
        {
            get => _selectedParking;
            set { SetProperty(ref _selectedParking, value); }
        }

        private bool _isRefreshingParkingList = false;
        public bool IsRefreshingParkingList
        {
            get => _isRefreshingParkingList;
            set { SetProperty(ref _isRefreshingParkingList, value); }
        }

        public DelegateCommand<Parking> ShowParkingCommand { get; }
        public DelegateCommand<Parking> EditParkingCommand { get; }
        public DelegateCommand<object> RefreshParkingListCommand { get; }

        public ParkingListPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            ShowParkingCommand = new DelegateCommand<Parking>(OnShowParkingClicked, CanShowParking);
            EditParkingCommand = new DelegateCommand<Parking>(OnEditParkingClicked, CanEditParking);
            RefreshParkingListCommand = new DelegateCommand<object>(OnRefreshParkingList, CanRefreshParkingList);
        }

        private bool CanRefreshParkingList(object arg)
        {
            return true;
        }

        private async void OnRefreshParkingList(object obj)
        {
            IsRefreshingParkingList = true;
            ParkingList = await GetParkingList().ConfigureAwait(false);
            IsRefreshingParkingList = false;
        }

        private bool CanEditParking(object arg)
        {
            //TODO check selected parking rights (set buttons on viewcell intead of context actions)
            return true;
        }

        private async void OnEditParkingClicked(Parking parking)
        {
            var navParams = new NavigationParameters {
                { "parking", parking }
            };
            await NavigationService.NavigateAsync("ParkingEditPage", navParams);
        }

        private bool CanShowParking(object arg)
        {
            return true;
        }

        private async void OnShowParkingClicked(Parking parking)
        {
            var navParams = new NavigationParameters {
                {"parking", parking }
            };
            await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingManagementPage", navParams);
        }

        private async Task<ObservableCollection<Parking>> GetParkingList()
        {
            //TODO: refac in a service
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var response = await httpClient.GetAsync(APIConstants.GetUserParkingsUrl).ConfigureAwait(false);
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
            ParkingList = await GetParkingList().ConfigureAwait(false);
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
