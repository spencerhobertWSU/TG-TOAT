
using System.Security.Cryptography;

namespace TGTOAT.Helpers;

public class Authentication : IAuthentication
{
    private static TGTOAT.Models.UserIndexViewModel currUser;
    private static string UserRole;

    //Set Current Users Basic Information
    public void SetUser(TGTOAT.Models.UserIndexViewModel user)
    {
        currUser = user;
    }

    public int GetCurrentUserId()
    {
        return currUser.Id; // Assuming the UserLoginViewModel has an Id property
    }

    //Logout User
    public void Logout()
    {
        currUser = null;
    }

    ///Grab basic User information
    public TGTOAT.Models.UserIndexViewModel GetUser()
    {
        return currUser;
    }

    public void SetRole(string role)
    {
        UserRole = role;
    }

    public string GetRole()
    {
        return UserRole;
    }

    public string createToken(int ranNum)
    {
        var randomNumber = new byte[ranNum];
        string token = "";


        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            token = Convert.ToBase64String(randomNumber);
        }

        return token;
    }




}
