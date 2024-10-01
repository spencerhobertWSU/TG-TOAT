namespace TGTOAT.Helpers;

public interface IAuthentication
{
    //Set user with login info
    public void SetUser(TGTOAT.Models.UserIndexViewModel user);

    //Logout User
    public void Logout();

    //Grab user information
    public TGTOAT.Models.UserIndexViewModel GetUser();
    int GetCurrentUserId();

    public void SetRole(string role);

    public string GetRole();

    public string createToken(int ranNum);
}
