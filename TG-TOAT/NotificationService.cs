﻿using Data;

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



        // Retrieves notifications for a specific user
        public IEnumerable<Notifications> GetNotificationsForUser(int studentId)
        {
            return _context.Notifications
                .Where(n => n.StudentId == studentId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }


        public void ClearNotificationsOnLogout(int studentId)
        {
            var notifications = _context.Notifications
                .Where(n => n.StudentId == studentId)
                .ToList();

            _context.Notifications.RemoveRange(notifications);
            _context.SaveChanges();
        }

    }

}