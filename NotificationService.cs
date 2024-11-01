using TGTOAT.Data;

namespace TGTOAT
{
    public class NotificationService
    {
        private readonly UserContext _context; // Your EF DbContext

        public NotificationService(UserContext context)
        {
            _context = context;
        }

        public void CreateNotification(string message, int studentId)
        {
            var notification = new Notifications
            {
                Message = message,
                CreatedAt = DateTime.UtcNow,
                StudentId = studentId
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public IEnumerable<Notifications> GetNotificationsForUser(int studentId)
        {
            return _context.Notifications
                .Where(n => n.StudentId == studentId)
                .ToList();
        }

        public void MarkAsRead(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
        }
    }
}
