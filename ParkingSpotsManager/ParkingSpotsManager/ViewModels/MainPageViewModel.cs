using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParkingSpotsManager.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand<string> GoToCommand {get; private set;}

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";

            GoToCommand = new DelegateCommand<string>(GoToPage, CanGoToPage);
        }

        private bool CanGoToPage(string arg)
        {
            //TODO check if user is auth
            return true;
        }

        private void GoToPage(string param)
        {
            NavigationService.NavigateAsync(param);
        }
    }
}
