using FlowersApp.ViewModels;

namespace FlowersApp.Infraestructure
{
    public  class InstanceLocator
    {
        public MainViewModel Main { get; set; }

        public InstanceLocator()
        {

            Main = new MainViewModel();

        }

    }
}
