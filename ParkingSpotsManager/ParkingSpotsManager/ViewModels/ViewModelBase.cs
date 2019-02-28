using Newtonsoft.Json;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using Prism.Commands;
using Prism.Common;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        }

        public async Task<User> GetCurrentUser()
        {
            if (Prism.PrismApplicationBase.Current.Properties.ContainsKey("authToken") && Prism.PrismApplicationBase.Current.Properties["authToken"] != null) {
                using (var httpClient = new HttpClient()) {
                    try {
                        var token = Prism.PrismApplicationBase.Current.Properties["authToken"].ToString();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var response = await httpClient.GetAsync(APIConstants.GetCurrentUser);
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<User>(content);
                        if (user == null) {
                            await Logout();
                        } else {
                            SetAuthUserProperties(user, token);

                            return user;
                        }
                    } catch (Exception) {
                        await Logout();
                    }
                }
            }

            return null;
        }

        public async Task Logout()
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

        public void SetAuthUserProperties(User user, string token)
        {
            IsAuth = true;
            AuthToken = token;
            CurrentUser = user;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
