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
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ParkingSpotsManager.ViewModels
{
	public class HomePageViewModel : ViewModelBase
	{
        public DelegateCommand<string> NavigateCommand { get; }
        public DelegateCommand<string> LogoutCommand { get; }

        public HomePageViewModel(INavigationService navigationService) : base (navigationService)
        {
            //TODO find a way to await
            GetCurrentUser();
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
            await Logout();
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
            if (!PrismApplicationBase.Current.Properties.ContainsKey("authToken")) {
                await Logout();
            } else {
                using (var httpClient = new HttpClient()) {
                    try {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                        var response = await httpClient.GetAsync(APIConstants.ValidTokenUrl);
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync();
                        var isValid = JsonConvert.DeserializeObject<string>(content) == "true";
                        if (!isValid) {
                            await Logout();
                        } else {
                            IsAuth = true;
                        }
                    } catch (Exception) {
                        await Logout();
                    }
                }
            }
        }
    }
}
