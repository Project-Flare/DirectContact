using DirectContactClient.ViewModel;

namespace DirectContactClient.Views;

public partial class MainView : ContentPage
{
	public MainView()
	{
		InitializeComponent();
		BindingContext = new MainViewModel();
	}
}