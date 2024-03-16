using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactClient.Model;
// Contact for MainView users
public class MyContact
{
    public string userName { get; set; }

    public MyContact(string userName)
    {
        this.userName = userName;
    }

    public string getUserName()
    {
        return userName;
    }
}
