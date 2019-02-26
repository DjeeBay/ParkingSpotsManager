using Newtonsoft.Json;
using ParkingSpotsManager.Shared.Constants;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;

namespace ParkingSpotsManager.ViewModels
{
	public class HomePageViewModel : ViewModelBase
	{
        public DelegateCommand<string> NavigateCommand { get; }
        public DelegateCommand<string> LogoutCommand { get; }

        public HomePageViewModel(INavigationService navigationService) : base (navigationService)
        {
            NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted, CanExecuteNavigateCommand).ObservesProperty(() => IsAuth);
            LogoutCommand = new DelegateCommand<string>(OnLogoutCommandExecuted, CanExecuteLogoutCommand).ObservesProperty(() => IsAuth);
            CheckToken();
        }

        private bool CanExecuteLogoutCommand(string arg)
        {
            return true;
        }

        private async void OnLogoutCommandExecuted(string obj)
        {
            IsAuth = false;
            AuthToken = null;
            PrismApplicationBase.Current.Properties["authToken"] = null;
            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        private bool CanExecuteNavigateCommand(string arg)
        {
            return IsAuth;
        }

        private async void OnNavigateCommandExecuted(string page)
        {
            await NavigationService.NavigateAsync(page);
        }

        private async void CheckToken()
        {
            var token = PrismApplicationBase.Current.Properties["authToken"].ToString();
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.GetAsync(APIConstants.ValidTokenUrl);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var isValid = JsonConvert.DeserializeObject<string>(content) == "true";
                    if (!isValid) {
                        await NavigationService.NavigateAsync("NavigationPage/MainPage");
                    } else {
                        IsAuth = true;
                    }
                } catch (Exception) {
                    await NavigationService.NavigateAsync("NavigationPage/MainPage");
                }
            }
        }
    }
}
