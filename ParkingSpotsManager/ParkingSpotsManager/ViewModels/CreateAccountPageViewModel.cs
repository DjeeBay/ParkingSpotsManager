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
	public class CreateAccountPageViewModel : ViewModelBase
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

        private string _confirmedPassword;
        public string ConfirmedPassword
        {
            get => _confirmedPassword;
            set { SetProperty(ref _confirmedPassword, value); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { SetProperty(ref _email, value); }
        }

        public DelegateCommand<object> CreateAccountCommand { get; private set; }

        public CreateAccountPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            CreateAccountCommand = new DelegateCommand<object>(CreateAccountAsync, CanCreateAccount);
        }

        private bool CanCreateAccount(object arg)
        {
            //TODO check if user is auth
            return true;
        }

        private async void CreateAccountAsync(object obj)
        {
            if (Username != null && Password != null && ConfirmedPassword != null && Email != null && Password == ConfirmedPassword) {
                var url = $"{APIConstants.AzureAPICreateAccountUrl}";
                var json = JObject.FromObject(new {
                    username = Username,
                    password = Password,
                    email = Email
                });
                using (var httpClient = new HttpClient()) {
                    try {
                        var response = await httpClient.PostAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync();
                        var createdUser = JsonConvert.DeserializeObject<User>(content);
                        //TODO test createdUser / notif
                        if (createdUser.Username != null) {
                            await NavigationService.NavigateAsync("MainPage");
                        }
                    } catch (Exception) { }
                }
            }
        }
    }
}
