using System;

namespace Models
{
    public class AllSubmissions
    {
        public string type { get; set; }
        public int CourseId { get; set; }
        public List<StudentSubmission>? Submissions { get; set; }

    }
}
