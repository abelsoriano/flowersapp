using FlowersApp.Models;
using FlowersApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlowersApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        #region Atributos
        private ApiService apiService;
        private NavigationService navigationService;
        private DialogService dialogService;
        private bool isRefreshing;

        #endregion

        #region Propiedades
        public ObservableCollection<FlowerItemViewModel> Flowers { get; set; }
        public NewFlowerViewModel NewFlower { get; set; }
        public EditFlowerViewModel EditFlower { get; set; }

        public bool IsRefreshing
        {
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshing"));
                }
            }
            get
            {
                return isRefreshing;
            }
        }

        #endregion

        #region Contructores
        public MainViewModel()
        {
            //singleton
            instance = this;

            //servicios
            apiService = new ApiService();
            navigationService = new NavigationService();
            dialogService = new DialogService();

            //viwe models
            Flowers = new ObservableCollection<FlowerItemViewModel>();

            //loading datas no la necesito mas porque la instancio en el onapearing
           // LoadFlowers();
        }


        #endregion

        #region Metodos
        private async void LoadFlowers()
        {
            // var flowers = await apiService.Get<Flower>("http://psflowersback.azurewebsites.net", "/api", "/Flowers");
            var response = await apiService.Get<Flower>("http://psflowersback.azurewebsites.net", "/api", "/Flowers");
            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            ReloadFlowers((List<Flower>)response.Result);
        }

        private void ReloadFlowers(List<Flower> flowers)
        {
            Flowers.Clear();
            //foreach (var flower in flowers)
            foreach (var flower in flowers.OrderBy(f => f.Description))
            {
                Flowers.Add(new FlowerItemViewModel
                {
                    Description = flower.Description,
                    FlowerId = flower.FlowerId,
                    Price = flower.Price,
                    Image = flower.Image,
                    IsActive = flower.IsActive,
                    LastPurchase = flower.LastPurchase,
                    Observation = flower.Observation,
                });
            }
        }
        #endregion

        #region Comandos
        public ICommand AddFlowerCommand { get { return new RelayCommand(AddFlower); } }
        public ICommand RefreshFlowerCommand { get { return new RelayCommand(RefreshFlower); } }

        private void RefreshFlower()
        {
            IsRefreshing = true;
            LoadFlowers();
            IsRefreshing = false;
        }

        private async void AddFlower()
        {
            await navigationService.Navigate("NewFlowerPage");

        }

        #endregion
        //usar la main view model desdeotras clases
        #region Singleton
        private static MainViewModel instance;



        public static MainViewModel GetInstance()

        {
            if (instance == null)

            {
                instance = new MainViewModel();

            }
            return instance;

        }

        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}
