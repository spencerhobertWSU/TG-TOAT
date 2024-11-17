using Data;
using Models;

namespace TGTOAT.Helpers;

public interface IAuthentication
{

    public void setUser(CurrUser User);
    public CurrUser getUser();
    public void Logout();

    public void setIndex();

    public UserIndexViewModel getIndex();

    public void updateRegistration();
    public CourseRegisterViewModel getRegistration();

    public string createToken(int ranNum);

    public void CreateNotification(string message, int studentId);

    public IEnumerable<Notifications> GetNotificationsForUser(int studentId);

    public void MarkAsRead(int notificationId);
}
