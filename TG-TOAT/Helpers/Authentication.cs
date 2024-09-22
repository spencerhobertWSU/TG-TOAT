using Microsoft.AspNetCore.Authentication;
using TGTOAT.Models;

namespace TGTOAT.Helpers;

public class Authentication : IAuthentication
{
    private static TGTOAT.Models.User currUser;

    public void SetUser(TGTOAT.Models.User user)
    {
        currUser = user;
    }

    public void Logout()
    {
        currUser = null;
    }

    public TGTOAT.Models.User CheckUser()
    {
        return currUser;
    }






}
