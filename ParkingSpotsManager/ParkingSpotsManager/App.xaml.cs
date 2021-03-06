﻿using Prism;
using Prism.Ioc;
using ParkingSpotsManager.ViewModels;
using ParkingSpotsManager.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ParkingSpotsManager
{
    public partial class App
    {
        public bool IsUserAuthenticated { get; set; } = false;

        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            if (PrismApplicationBase.Current.Properties.ContainsKey("authToken") && PrismApplicationBase.Current.Properties["authToken"] != null) {
                await NavigationService.NavigateAsync("/HomePage/NavigationPage/ParkingListPage");
            } else {
                await NavigationService.NavigateAsync("NavigationPage/MainPage");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<CreateAccountPage, CreateAccountPageViewModel>();
            containerRegistry.RegisterForNavigation<ParkingManagementPage, ParkingManagementPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<ParkingListPage, ParkingListPageViewModel>();
            containerRegistry.RegisterForNavigation<CreateParkingPage, CreateParkingPageViewModel>();
            containerRegistry.RegisterForNavigation<ParkingEditPage, ParkingEditPageViewModel>();
            containerRegistry.RegisterForNavigation<CreateSpotPage, CreateSpotPageViewModel>();
            containerRegistry.RegisterForNavigation<SpotEditPage, SpotEditPageViewModel>();
            containerRegistry.RegisterForNavigation<InvitePage, InvitePageViewModel>();
            containerRegistry.RegisterForNavigation<AccountPage, AccountPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
        }
    }
}
