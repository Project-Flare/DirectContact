using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectContactClient.Model;
using System.Collections.ObjectModel;
using System;

namespace DirectContactClient.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Users = new ObservableCollection<User>();
            Read();
        }

        [ObservableProperty]
        ObservableCollection<User> users;

        void Read()
        {
            /*using(StreamReader reader = new StreamReader(@"users.txt"))
            {
                /*string ln;
                while((ln = reader.ReadLine()) != null)
                {
                    Items.Add(new User(ln));
                }*/
            //}
            Users.Add(new User("sw"));
            Users.Add(new User("dp"));
        }
    }
}
