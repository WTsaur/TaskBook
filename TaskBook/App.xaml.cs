using Xamarin.Forms;

namespace TaskBook
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new TaskBook.Pages.TaskListPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
