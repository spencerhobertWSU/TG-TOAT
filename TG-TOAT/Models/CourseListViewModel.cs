using Data;

namespace Models
{
    public class CourseListViewModel
    {

        public List<Notifications> Notifications { get; set; }
        public List<CourseInfo> Courses { get; set; } = new List<CourseInfo>();

    }
}
