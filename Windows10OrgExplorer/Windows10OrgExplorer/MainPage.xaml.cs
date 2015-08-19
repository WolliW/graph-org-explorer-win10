using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Windows10OrgExplorer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            refreshView("me");
        }

        private async void refreshView(string path)
        {
            wait.Visibility = Visibility.Visible;
            var data = await EmployeeOrgModel.GetEmployeeModel(path);
            this.DataContext = data;
            wait.Visibility = Visibility.Collapsed;
        }

        private async void Manager_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EmployeeOrgModel model = (EmployeeOrgModel)((TextBlock)sender).DataContext;
            refreshView("myorganization/users/" + model.Manager.objectId);
        }

        private async void DirectReport_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EmployeeModel model = (EmployeeModel)((TextBlock)sender).DataContext;
            refreshView("myorganization/users/" + model.objectId);
        }
    }
}
