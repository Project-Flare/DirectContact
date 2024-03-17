using DirectContactClient.ViewModel;

namespace DirectContactClient.Views;

public partial class MainView : ContentPage
{
	bool push = false;
	public MainView()
	{
		InitializeComponent();
		BindingContext = new MainViewModel();
	}

	private void AddContacts_Clicked(object sender, EventArgs e)
	{
		//Shell.Current.GoToAsync(nameof(ExploreView));
		// push once..
		Navigation.PushModalAsync(new ExploreView());

	}
}