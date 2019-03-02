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
            NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted, CanExecuteNavigateCommand).ObservesProperty(() => IsAuth);
            LogoutCommand = new DelegateCommand<string>(OnLogoutCommandExecuted, CanExecuteLogoutCommand).ObservesProperty(() => IsAuth);
        }

        private bool CanExecuteLogoutCommand(string arg)
        {
            return true;
        }

        private async void OnLogoutCommandExecuted(string obj)
        {
            await LogoutAsync();
        }

        private bool CanExecuteNavigateCommand(string arg)
        {
            return IsAuth;
        }

        private async void OnNavigateCommandExecuted(string page)
        {
            await NavigationService.NavigateAsync(page);
        }
    }
}
