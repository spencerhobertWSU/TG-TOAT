namespace TGTOAT.ViewModels
{
    public class CourseListViewModel
    {
        public IEnumerable<TGTOAT.Data.Courses> Courses { get; set; }
        public TGTOAT.Models.UserIndexViewModel UserLoginViewModel { get; set; }
    }
}
