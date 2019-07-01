using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Services;
using ParkingSpotsManager.Shared.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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

        private IPageDialogService _dialogService;

        public DelegateCommand<object> CreateAccountCommand { get; private set; }

        public CreateAccountPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
            Title = "New account";
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
                var url = $"{API.CreateAccountUrl()}";
                var json = JObject.FromObject(new {
                    username = Username,
                    password = Password,
                    email = Email
                });
                using (var httpClient = new HttpClient()) {
                    try {
                        var response = await httpClient.PostAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                        if (response.IsSuccessStatusCode) {
                            var content = await response.Content.ReadAsStringAsync();
                            var createdUser = JsonConvert.DeserializeObject<User>(content);
                            if (createdUser.Username != null) {
                                await NavigationService.NavigateAsync("LoginPage");
                            }
                        } else if (response.StatusCode == HttpStatusCode.BadRequest) {
                            var content = await response.Content.ReadAsStringAsync();
                            var error = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(content);
                            //TODO make an exception class to catch API badrequest results
                            if (error != null && error.Count > 0) {
                                var errorMsg = new StringBuilder();
                                var firstError = error.First();
                                errorMsg.Append(firstError.Key).Append(" : ");
                                if (firstError.Value != null && firstError.Value.Count > 0) {
                                    errorMsg.Append(firstError.Value.First());
                                } else {
                                    errorMsg.Append("Incorrect value.");
                                }
                                await _dialogService.DisplayAlertAsync("Error", errorMsg.ToString(), "OK");
                            } else {
                                await _dialogService.DisplayAlertAsync("Error", "An error occured.", "OK");
                            }
                        }
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
