
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using System.Security.Claims;


namespace Satluj_Latest.Controllers
{
    public class BaseController : Controller
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
        public BaseController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities)
        {
            _schoolRepository = schoolRepository;
            _parentRepository = parentRepository;
            _teacherRepository = teacherRepository;
            _Entities = Entities;
        }



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectResult("/Account/Home");
                return;
            }

            
            if (!HttpContext.Request.Cookies.TryGetValue("UserType", out string userTypeStr))
            {
                context.Result = new RedirectResult("/Account/Home");
                return;
            }

            int userType = int.Parse(userTypeStr);

            //long userId = long.Parse(User.Identity.Name);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userIdClaim, out long userId))
            {
                // handle error or redirect to login
            }


            if (userType == (int)UserRole.School ||
                userType == (int)UserRole.Staff ||
                userType == (int)UserRole.Teacher ||
                userType == (int)UserRole.Master)
            {
                if (HttpContext.Session.GetString("User") == null)
                {
                    var user = _Entities.TbLogins.FirstOrDefault(x => x.UserId == userId);
                    HttpContext.Session.SetString("User", System.Text.Json.JsonSerializer.Serialize(user));
                    HttpContext.Session.SetInt32("UserType", userType);
                }

                // Deserialize stored user
                var userJson = HttpContext.Session.GetString("User");
                _user = System.Text.Json.JsonSerializer.Deserialize<TbLogin>(userJson);

                // Admin logic
                if (_user.RoleId == (int)UserRole.Teacher)
                {
                    var teacher = _Entities.TbTeachers.FirstOrDefault(x => x.UserId == _user.UserId);
                    bool isAdmin = false;

                    if (teacher?.UserType != null)
                    {
                        isAdmin = _Entities.TbUserModuleMains
                                    .Where(x => x.Id == teacher.UserType && x.IsActive)
                                    .Select(x => x.IsAdmin)
                                    .FirstOrDefault() ?? false;
                    }

                    HttpContext.Session.SetString("IsAdmin", isAdmin.ToString());
                }
                else if (_user.RoleId == (int)UserRole.School || _user.RoleId == (int)UserRole.Master)
                {
                    HttpContext.Session.SetString("IsAdmin", true.ToString());
                }
                else
                {
                    HttpContext.Session.SetString("IsAdmin", false.ToString());
                }
            }

          
            else if (userType == (int)UserRole.Parent)
            {
                if (HttpContext.Session.GetString("Parent") == null)
                {
                    var parent = _Entities.TbParents.FirstOrDefault(x => x.ParentId == userId);

                    HttpContext.Session.SetString("Parent",
                        System.Text.Json.JsonSerializer.Serialize(parent));

                    HttpContext.Session.SetInt32("UserType", userType);
                }

                var pJson = HttpContext.Session.GetString("Parent");
                _parentUser = System.Text.Json.JsonSerializer.Deserialize<TbParent>(pJson);
            }

            base.OnActionExecuting(context);
        }

    }
}