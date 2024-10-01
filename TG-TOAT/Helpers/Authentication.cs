using Microsoft.AspNetCore.Authentication;
using TGTOAT.Models;

namespace TGTOAT.Helpers;

public class Authentication : IAuthentication
{
    private static TGTOAT.Models.UserLoginViewModel currUser;

    //Set Current Users Basic Information
    public void SetUser(TGTOAT.Models.UserLoginViewModel user)
    {
        currUser = user;
    }

    public string GetCurrentUserId()
    {
        return currUser?.Id; // Assuming the UserLoginViewModel has an Id property
    }

    //Logout User
    public void Logout()
    {
        currUser = null;
    }

    ///Grab basic User information
    public TGTOAT.Models.UserLoginViewModel CheckUser()
    {
        return currUser;
    }






}
