using FlowersApp.Models;
using FlowersApp.Pages;
using FlowersApp.ViewModels;
using System.Threading.Tasks;

namespace FlowersApp.Services
{
    public class NavigationService
    {
        //generico para paginas que no requieran parametros
        public async Task Navigate(string pageName)
        {
            var mainViewModel = MainViewModel.GetInstance();
         
            switch (pageName)

            {
                 
                case "NewFlowerPage":
                    //Los saco de aqui para poder usarlo en cada caso
                    //var mainViewModel = MainViewModel.GetInstance();
                    mainViewModel.NewFlower = new NewFlowerViewModel();
                    await App.Current.MainPage.Navigation.PushAsync(new NewFlowerPage());
                    break;
                //case "EditFlowerPage":
                //    mainViewModel.EditFlower = new EditFlowerViewModel();
                //    await App.Current.MainPage.Navigation.PushAsync(new EditFlowerPage());
                //    break;
                default:
                    break;
            }

        }


        public async Task Back()

        {
            await App.Current.MainPage.Navigation.PopAsync();

        }

        public async Task EditFlower(Flower flower)

        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.EditFlower = new EditFlowerViewModel(flower);
            await App.Current.MainPage.Navigation.PushAsync(new EditFlowerPage());

        }



    }
}
