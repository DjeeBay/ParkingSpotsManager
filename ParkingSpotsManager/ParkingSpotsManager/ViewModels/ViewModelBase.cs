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
            new NotifyTaskCompletion<User>(GetCurrentUserAsync());
        }

        protected async Task<User> GetCurrentUserAsync()
        {
            var token = GetToken();
            var currentVM = GetType().Name;
            if (token != null) {
                using (var httpClient = new HttpClient()) {
                    try {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var response = await httpClient.GetAsync(APIConstants.GetCurrentUser);
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<User>(content);

                        if (user == null && currentVM != "MainPageViewModel" && currentVM != "LoginPageViewModel") {
                            await LogoutAsync();
                        } else {
                            SetAuthUserProperties(user, token);

                            return user;
                        }
                    } catch (Exception) {
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
                await Prism.PrismApplicationBase.Current.SavePropertiesAsync();
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

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
            //TODO call API here
        }

        public virtual void Destroy()
        {

        }
    }
}
