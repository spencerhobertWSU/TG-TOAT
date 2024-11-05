namespace Data
{
    public class Notifications
{
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;
        public int StudentId { get; set; } // Assuming you have a Student model
    }
}
