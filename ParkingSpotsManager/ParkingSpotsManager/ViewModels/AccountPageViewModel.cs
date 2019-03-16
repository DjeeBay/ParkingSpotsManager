using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Libraries;
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
using System.Text;
using System.Threading.Tasks;

namespace ParkingSpotsManager.ViewModels
{
    public class AccountPageViewModel : ViewModelBase, INavigationAware
    {
        private User _user;
        public User User
        {
            get => _user;
            set { SetProperty(ref _user, value); }
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

        public DelegateCommand<object> SaveCommand { get; set; }

        private IPageDialogService _dialogService;

        public AccountPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base (navigationService)
        {
            _dialogService = dialogService;
            SaveCommand = new DelegateCommand<object>(OnSaveCommandExecuted, CanExecuteSave);
        }

        private bool CanExecuteSave(object arg)
        {
            return true;
        }

        private async void OnSaveCommandExecuted(object obj)
        {
            if (string.IsNullOrEmpty(Password)) {
                Password = null;
                ConfirmedPassword = null;
            }
            if (Password != ConfirmedPassword) {
                await _dialogService.DisplayAlertAsync("Error", "Passwords are not identical.", "Close");
                return;
            }
            if (User.Email == null || User.Email.Length == 0) {
                await _dialogService.DisplayAlertAsync("Error", "Email is required.", "Close");
                return;
            }

            await SaveAsync();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            User = await GetCurrentUserAsync().ConfigureAwait(false);
        }

        private async Task SaveAsync()
        {
            var url = $"{APIConstants.SaveUserAccountUrl}";
            User.Password = Password;
            var userAccount = new UserAccount {
                Id = User.Id,
                Firstname = User.Firstname,
                Lastname = User.Lastname,
                Email = User.Email
            };
            if (!string.IsNullOrEmpty(User.Password)) {
                userAccount.Password = User.Password;
                userAccount.IsPasswordSet = true;
            }
            var json = JObject.FromObject(userAccount);
            using (var httpClient = new HttpClient()) {
                try {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var response = await httpClient.PostAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode) {
                        await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
                    } else if (response.StatusCode == HttpStatusCode.BadRequest) {
                        var content = await response.Content.ReadAsStringAsync();
                        var error = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(content);
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
                    await _dialogService.DisplayAlertAsync("Error", "An error occured.", "OK");
                }
            }
        }
    }
}
