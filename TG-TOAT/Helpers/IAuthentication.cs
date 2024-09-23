namespace TGTOAT.Helpers;

public interface IAuthentication
{
    //Set user with login info
    public void SetUser(TGTOAT.Models.UserLoginViewModel user);

    //Logout User
    public void Logout();

    //Grab user information
    public TGTOAT.Models.UserLoginViewModel CheckUser();

}
