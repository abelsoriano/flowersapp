using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Windows.Input;
using FlowersApp.Services;
using FlowersApp.Models;
using System;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using Plugin.Media;
using FlowersApp.Classes;

namespace FlowersApp.ViewModels
{
    public class NewFlowerViewModel : Flower, INotifyPropertyChanged
    {
        #region Atributos
        private DialogService dialogService;
        private ApiService apiService;
        private NavigationService navigationService;
        //se hacen inecesarios al momento de hacerlos heredar de la clase flower
        //private string description;
        //private decimal price;
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
        //public string Description
        //{
        //    set
        //    {
        //        if (description != value)

        //        {
        //            description = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
        //        }
        //    }
        //    get
        //    {
        //        return description;
        //    }

        //}

        //public decimal Price
        //{
        //    set
        //    {
        //        if (price != value)

        //        {
        //            price = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Price"));
        //        }
        //    }
        //    get
        //    {
        //        return price;
        //    }

        //}

        public bool IsRunning
        {
            set {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                }
            }
            get {
                return isRunning;
            }
        }

        public bool IsEnabled
        {
            set {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
                }
            }
            get {
                return isEnabled;
            }
        }

        #endregion
        
        #region Constructores
        public NewFlowerViewModel()
        {
            dialogService = new DialogService();
            apiService = new ApiService();
            navigationService = new Services.NavigationService();
            IsEnabled = true;
            LastPurchase = DateTime.Now;
        }
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Comandos
        public ICommand TakePictureCommand { get { return new RelayCommand(TakePicture); } }

        private async void TakePicture()
        {   //activa la camara
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await dialogService.ShowMessage("No Camera", ":'( No camera available");
            }

            file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = PhotoSize.Small,
            });

            IsRunning = true;

            if (file != null)
            {
                ImageSource = ImageSource.FromStream(() => 
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
            IsRunning = false;
        }

        public ICommand NewFlowerCommand { get { return new RelayCommand(NewFlower); } }

        private async void NewFlower()
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
            //IsRunning = true;
            //IsEnabled = false;
            var imageArray = FilesHelper.ReadFully(file.GetStream());
            file.Dispose(); //liberando memoria

            var flower = new Flower
            {
                Description = Description,
                Price = Price,
                IsActive = IsActive,
                LastPurchase = LastPurchase,
                Observation = Observation,
                ImageArray= imageArray,
            };

            IsRunning = true;
            IsEnabled = false;
           // var response = await apiService.Post("http://psflowersback.azurewebsites.net", "/api", "/Flowers", this);
            var response = await apiService.Post("http://psflowersback.azurewebsites.net", "/api", "/Flowers", flower);
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
