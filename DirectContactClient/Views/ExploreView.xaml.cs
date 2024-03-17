using DirectContactClient.ViewModel;

namespace DirectContactClient.Views;

public partial class ExploreView : ContentPage
{
	public ExploreView()
	{
		InitializeComponent();
        BindingContext = new ExploreViewModel();
    }
}