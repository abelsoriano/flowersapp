using FlowersApp.Models;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System;
using FlowersApp.Services;

namespace FlowersApp.ViewModels
{
    //para mantener el modelo limpio 
    public class FlowerItemViewModel : Flower
    {
        private NavigationService navigationService;

        //constructuro
        public FlowerItemViewModel()
        {
            navigationService = new Services.NavigationService();
        }

        public ICommand EditFlowerCommand { get { return new RelayCommand(EditFlower); }}

        private async void EditFlower()
        {
            await navigationService.EditFlower(this);
        }
    }
}
