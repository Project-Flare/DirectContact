using DirectContactClient.Views;

namespace DirectContactClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            //MainPage = new LoginView();
            MainPage = new RegisterView();
            MainPage = new ChatView();
        }
    }
}
