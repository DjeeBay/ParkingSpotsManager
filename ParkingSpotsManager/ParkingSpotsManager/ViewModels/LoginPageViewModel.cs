using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingSpotsManager.ViewModels
{
	public class LoginPageViewModel : BindableBase
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

        public LoginPageViewModel()
        {
            LogInCommand = new DelegateCommand<object>(LogIn, CanLogIn);
        }

        private bool CanLogIn(object arg)
        {
            //TODO check if user is already authenticated.
            return true;
        }

        private void LogIn(object parameter)
        {
            Console.WriteLine(Username + Password);
        }
    }
}
