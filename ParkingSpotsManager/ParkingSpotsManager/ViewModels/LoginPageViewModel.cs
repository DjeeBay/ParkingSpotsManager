using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public LoginPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            LogInCommand = new DelegateCommand<object>(LogIn, CanLogIn);
        }

        private bool CanLogIn(object arg)
        {
            //TODO check if user is already authenticated.
            return true;
        }

        private async void LogIn(object parameter)
        {
            if (Username != null && Password != null) {
                var url = APIConstants.AzureAPILoginUrl;
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
                            if (Prism.PrismApplicationBase.Current.Properties.ContainsKey("authToken")) {
                                Prism.PrismApplicationBase.Current.Properties["authToken"] = authUser.AuthToken;
                            } else {
                                Prism.PrismApplicationBase.Current.Properties.Add("authToken", authUser.AuthToken);
                            }
                            await Prism.PrismApplicationBase.Current.SavePropertiesAsync();
                            await NavigationService.NavigateAsync("MaingePage");
                        }
                        //TODO else notify bad login
                    } catch (Exception) { }
                }
            }
        }
    }
}
