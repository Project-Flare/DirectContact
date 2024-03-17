using DirectContactClient.Views;

namespace DirectContactClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
            Routing.RegisterRoute(nameof(MainView), typeof(MainView));
            Routing.RegisterRoute(nameof(ChatView), typeof(ChatView));
            Routing.RegisterRoute(nameof(ExploreView), typeof(ExploreView));
        }
    }
}
