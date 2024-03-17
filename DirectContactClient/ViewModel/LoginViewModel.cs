using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectContactClient.Views;
using System;

namespace DirectContactClient.ViewModel;

public partial class LoginViewModel : ObservableObject
{
    [RelayCommand]
    async Task SingUp()
    {
        await Shell.Current.GoToAsync(nameof(RegisterView));
    }
}
