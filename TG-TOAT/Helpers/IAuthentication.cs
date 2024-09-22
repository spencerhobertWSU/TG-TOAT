namespace TGTOAT.Helpers;

public interface IAuthentication
{
    public void SetUser(TGTOAT.Models.User user);
    public void Logout();
    public TGTOAT.Models.User CheckUser();

}
