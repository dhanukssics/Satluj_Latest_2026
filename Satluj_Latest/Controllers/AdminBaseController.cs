using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest.DataLibrary.Repository;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;


namespace Satluj_Latest.Controllers
{
    public class AdminBaseController : Controller
    {
        protected readonly SchoolRepository _schoolRepository;
        protected readonly ParentRepository _parentRepository;
        protected readonly TeacherRepository _teacherRepository;
        protected readonly SchoolDbContext _Entities;

        public DateTime CurrentTime =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

        protected TbSchool _schoolUser;
        protected TbParent _parentUser;
        protected TbLogin _user;
        public AdminBaseController(SchoolRepository schoolRepository,ParentRepository parentRepository,TeacherRepository teacherRepository,SchoolDbContext Entities)
        {
            _schoolRepository = schoolRepository;
            _parentRepository = parentRepository;
            _teacherRepository = teacherRepository;
            _Entities = Entities;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User != null && User.Identity.IsAuthenticated)
            {

                var routeValues = filterContext.RouteData.Values;

            }

            else
            {
                filterContext.Result = new RedirectResult("/Account/Home");
                return;
            }
        }

    }

}
