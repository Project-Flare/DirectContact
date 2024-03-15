using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactClient.Model;
//User for MainView users
public class User
{
    public string userName { get; set; }

    public User(string userName)
    {
        this.userName = userName;
    }

    public string getUserName()
    {
        return userName;
    }
}
