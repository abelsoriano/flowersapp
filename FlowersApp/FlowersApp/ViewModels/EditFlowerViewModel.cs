using FlowersApp.Models;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System;
using FlowersApp.Services;
using System.ComponentModel;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using Plugin.Media;
using FlowersApp.Classes;

namespace FlowersApp.ViewModels
{
    public  class EditFlowerViewModel : Flower, INotifyPropertyChanged //hereda de la clase flower y hereda sus miembros
    {
        #region Atributos
        private DialogService dialogService;
        private ApiService apiService;
        private NavigationService navigationService;
        private bool isRunning;
        private bool isEnabled;
        private ImageSource imageSource;
        private MediaFile file;
        #endregion

        #region Propiedades
        public ImageSource ImageSource
        {
            set {
                if (imageSource != value)
                {
                    imageSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                }
            }
            get {
                return imageSource;
            }
        }

        public bool IsRunning
        {
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                }
            }
            get
            {
                return isRunning;
            }
        }

        public bool IsEnabled
        {
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
                }
            }
            get
            {
                return isEnabled;
            }
        }



        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructores
        public EditFlowerViewModel(Flower flower)
        {
            dialogService = new DialogService();
            apiService = new ApiService();
            navigationService = new NavigationService();

            Image = flower.Image;
            IsActive = flower.IsActive;
            Observation = flower.Observation;
            LastPurchase = flower.LastPurchase;
            Description = flower.Description;
            Price = flower.Price;
            FlowerId = flower.FlowerId;

            isEnabled = true;
        }
        #endregion
        //constructor
        #region Comandos
        public ICommand TakePictureCommand { get { return new RelayCommand(TakePicture); } }

        private async void TakePicture()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await App.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "Aceptar");
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = PhotoSize.Small,
            });

            if (file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    file.Dispose();
                    return stream;
                });
            }
        }

        public ICommand SaveFlowerCommand { get { return new RelayCommand(SaveFlower); } }

        private async void SaveFlower()
        {
            if (string.IsNullOrEmpty(Description))
            {
                await dialogService.ShowMessage("Error", "Debe entroducir una descripcion");
                return;
            }
            if (Price <= 0)
            {
                await dialogService.ShowMessage("Error", "Debe entroducir un numero mayor de cero en el precio");
                return;
            }

            var imageArray = FilesHelper.ReadFully(file.GetStream());
            file.Dispose(); //liberando memoria

            var flower = new Flower
            {FlowerId = FlowerId,
            Image= Image,
                Description = Description,
                Price = Price,
                IsActive = IsActive,
                LastPurchase = LastPurchase,
                Observation = Observation,
                ImageArray = imageArray,
            };
            IsRunning = true;
            IsEnabled = false;
            var response = await apiService.Put("http://psflowersback.azurewebsites.net", "/api", "/Flowers", flower);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            await navigationService.Back();
        }
        public ICommand DeleteFlowerCommand { get { return new RelayCommand(DeleteFlower); } }

        private async void DeleteFlower()
        {
            var answer = await dialogService.ShowConfirm("Confirmar", "Estas Seguro que deseas eliminar esto?");

            if (!answer)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;
            var response = await apiService.Delete("http://psflowersback.azurewebsites.net", "/api", "/Flowers", this);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            await navigationService.Back();
        }
        #endregion


    }
}
