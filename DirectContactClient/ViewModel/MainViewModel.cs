using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectContactClient.Model;
using System.Collections.ObjectModel;
using System;
using System.IO;

namespace DirectContactClient.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Mycontacts = new ObservableCollection<MyContact>();
            Read();
        }

        [ObservableProperty]
        ObservableCollection<MyContact> mycontacts;

        void Read()
        {
            // for demonstration purposes only.
            /*if(!File.Exists(@"NEWusers.txt"))
            {
                File.Create(@"NEWusers.txt");
            }    
            string[] lines = File.ReadAllLines(@"NEWusers.txt");
            foreach (string line in lines)
            {
                Mycontacts.Add(new MyContact(line));
            }*/
            Mycontacts.Add(new MyContact("sw"));
            Mycontacts.Add(new MyContact("dp"));
        }
    }
}
