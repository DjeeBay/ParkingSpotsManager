using Newtonsoft.Json;
using ParkingSpotsManager.Shared.Constants;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace ParkingSpotsManager.ViewModels
{
	public class ParkingManagementPageViewModel : ViewModelBase
	{
        public ParkingManagementPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            Title = "Parking Management";
        }
	}
}
