using TGTOAT.Data;

namespace TGTOAT.ViewModels
{
    public class CourseListViewModel
    {
        //notifications
        public List<Notifications> Notifications { get; set; }
        public IEnumerable<TGTOAT.Data.Courses> Courses { get; set; }
        public TGTOAT.Models.UserIndexViewModel UserLoginViewModel { get; set; }
    }
}
