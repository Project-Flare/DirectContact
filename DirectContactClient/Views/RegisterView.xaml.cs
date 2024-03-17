namespace DirectContactClient.Views;

public partial class RegisterView : ContentPage
{
	Color buttonColor;
	public RegisterView()
	{
		InitializeComponent();
		this.buttonColor = registerButton.BackgroundColor;
	}

	private void Register_Clicked(object sender, EventArgs e)
	{
		//Animation();

		if (username.Text != string.Empty)
		{
			username.Text = string.Empty;
		}


    }

    private async void Animation()
    {
		// Scaling
		//await registerButton.ScaleTo(registerButton.Scale * 1.05, 100, Easing.Linear);
		//await registerButton.ScaleTo(registerButton.Scale / 1.05, 100, Easing.Linear);
    }

	private async void GoBack_Clicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}