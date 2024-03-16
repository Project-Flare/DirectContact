using DirectContactClient.ViewModel;

namespace DirectContactClient.Views;

public partial class LoginView : ContentPage
{
	bool navigated = false;
	public LoginView()
	{
		InitializeComponent();
	}

	private void LoginButton_Clicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//{nameof(MainView)}");
	}

	private void NavigateToRegister_Clicked(object sender, EventArgs e)
	{
		//Navigation.PushAsync(new RegisterView());
		Shell.Current.GoToAsync(nameof(RegisterView));

	}

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}