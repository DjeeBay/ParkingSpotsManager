using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Services;
using ParkingSpotsManager.Shared.Constants;
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
using System.Text;

namespace ParkingSpotsManager.ViewModels
{
	public class LoginPageViewModel : ViewModelBase
	{
        private string _username;
        public string Username
        {
            get => _username;
            set { SetProperty(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { SetProperty(ref _password, value); }
        }

        public DelegateCommand<object> LogInCommand { get; private set; }

        private IPageDialogService _dialogService;

        public LoginPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base (navigationService)
        {
            _dialogService = dialogService;
            Title = "Log in";
            LogInCommand = new DelegateCommand<object>(LogIn, CanLogIn);
        }

        private bool CanLogIn(object arg)
        {
            return true;
        }

        private async void LogIn(object parameter)
        {
            if (Username != null && Password != null) {
                var url = API.LoginUrl();
                var json = JObject.FromObject(new {
                    login = Username,
                    password = Password
                });
                using (var httpClient = new HttpClient()) {
                    try {
                        var response = await httpClient.PostAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync();
                        var authUser = JsonConvert.DeserializeObject<User>(content);
                        if (authUser.Username != null && authUser.Id != 0 && authUser.AuthToken != null) {
                            //TODO store user data
                            if (Prism.PrismApplicationBase.Current.Properties.ContainsKey("authToken")) {
                                Prism.PrismApplicationBase.Current.Properties["authToken"] = authUser.AuthToken;
                            } else {
                                Prism.PrismApplicationBase.Current.Properties.Add("authToken", authUser.AuthToken);
                            }
                            await Prism.PrismApplicationBase.Current.SavePropertiesAsync();
                            SetAuthUserProperties(authUser, authUser.AuthToken);
                            await NavigationService.NavigateAsync("HomePage/NavigationPage/ParkingListPage");
                        } else {
                            await _dialogService.DisplayAlertAsync("Whoops !", "Bad credentials", "OK");
                        }
                    } catch (Exception) {
                        await _dialogService.DisplayAlertAsync("Whoops !", "Bad credentials", "OK");
                    }
                }
            }
        }
    }
}
