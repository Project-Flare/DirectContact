using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactClient.Model;
// Contact
public class ListContact
{
    public string userName { get; set; }

    public ListContact(string userName)
    {
        this.userName = userName;
    }

    public string getUserName()
    {
        return userName;
    }
}
