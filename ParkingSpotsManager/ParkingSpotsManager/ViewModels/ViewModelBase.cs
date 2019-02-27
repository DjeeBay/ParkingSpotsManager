using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        private int? _currentUserID;
        public int? CurrentUserID
        {
            get => _currentUserID;
            set { SetProperty(ref _currentUserID, value); }
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
            if (Prism.PrismApplicationBase.Current.Properties.ContainsKey("authToken") && Prism.PrismApplicationBase.Current.Properties["authToken"] != null) {
                AuthToken = Prism.PrismApplicationBase.Current.Properties["authToken"].ToString();
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(AuthToken) as JwtSecurityToken;
                var userIDFromToken = jsonToken.Claims.First(claim => claim.Type == "unique_name")?.Value;
                var isUserIDParsed = int.TryParse(userIDFromToken, out int userID);
                if (isUserIDParsed) {
                    CurrentUserID = userID;
                }
            }

            IsAuth = AuthToken != null;
        }

        public async Task Logout()
        {
            IsAuth = false;
            AuthToken = null;
            if (Prism.PrismApplicationBase.Current.Properties.ContainsKey("authToken")) {
                Prism.PrismApplicationBase.Current.Properties["authToken"] = null;
                await Prism.PrismApplicationBase.Current.SavePropertiesAsync();
            }
            await NavigationService.NavigateAsync("NavigationPage/MainPage");
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
