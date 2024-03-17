using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using DirectContactClient.Model;

namespace DirectContactClient.ViewModel
{
    public partial class ExploreViewModel : ObservableObject
    {
        public ExploreViewModel() 
        {
            Availablecontacts = new ObservableCollection<ListContact>();
            GetAvailable();
        }

        [ObservableProperty]
        ObservableCollection<ListContact> availablecontacts;

        void GetAvailable()
        {
            // demonstracijai...
            Availablecontacts.Add(new ListContact("kazkas"));
            Availablecontacts.Add(new ListContact("kazka"));
            Availablecontacts.Add(new ListContact("belenka"));
        }
    }
}
