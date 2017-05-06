using FlowersApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlowersApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlowersPage : ContentPage
    {
        public FlowersPage()
        {
            InitializeComponent();
            var mainViewModel = MainViewModel.GetInstance();
            base.Appearing += (object sender, EventArgs e) =>
            {
                mainViewModel.RefreshFlowerCommand.Execute(this);
            };

        }
    }
}
