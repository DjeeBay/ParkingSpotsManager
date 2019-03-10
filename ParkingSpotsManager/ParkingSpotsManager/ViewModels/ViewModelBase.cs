using Newtonsoft.Json;
using ParkingSpotsManager.Services;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ParkingSpotsManager.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }

        private string _authToken;
        public string AuthToken
        {
            get => _authToken;
            set { SetProperty(ref _authToken, value); }
        }

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set { SetProperty(ref _currentUser, value); }
        }

        private bool _isAuth = false;
        public bool IsAuth
        {
            get => _isAuth;
            set { SetProperty(ref _isAuth, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
            AuthToken = GetToken();
        }

        protected async Task<User> GetCurrentUserAsync()
        {
            var token = GetToken();
            var currentVM = GetType().Name;
            if (token != null && currentVM != "MainPageViewModel" && currentVM != "LoginPageViewModel") {
                using (var httpClient = new HttpClient()) {
                    try {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var response = await httpClient.GetAsync(APIConstants.GetCurrentUser).ConfigureAwait(false);
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var user = JsonConvert.DeserializeObject<User>(content);

                        if (user == null) {
                            await LogoutAsync();
                        } else {
                            SetAuthUserProperties(user, token);

                            return user;
                        }
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                        await LogoutAsync();
                    }
                }
            }

            return null;
        }

        protected async Task LogoutAsync()
        {
            IsAuth = false;
            AuthToken = null;
            CurrentUser = null;
            if (Prism.PrismApplicationBase.Current.Properties.ContainsKey("authToken")) {
                Prism.PrismApplicationBase.Current.Properties["authToken"] = null;
                await Prism.PrismApplicationBase.Current.SavePropertiesAsync().ConfigureAwait(false);
            }
            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected void SetAuthUserProperties(User user, string token)
        {
            IsAuth = user != null && token != null;
            AuthToken = token;
            CurrentUser = user;
        }

        protected string GetToken()
        {
            if (Prism.PrismApplicationBase.Current.Properties.ContainsKey("authToken") && Prism.PrismApplicationBase.Current.Properties["authToken"] != null) {
                return Prism.PrismApplicationBase.Current.Properties["authToken"].ToString();
            }

            return null;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual async void OnNavigatingTo(INavigationParameters parameters)
        {
            await GetCurrentUserAsync().ConfigureAwait(false);
        }

        public virtual void Destroy()
        {

        }
    }
}
