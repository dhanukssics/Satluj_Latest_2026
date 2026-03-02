using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Satluj_Latest.Repository;
using Satluj_Latest;

namespace Satluj_Latest.Controllers
{
    public class PreLoginController : Controller
    {
        
        public DateTime CurrentTime = TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        // public tb_Satluj_LatestEntities _Entities = new tb_Satluj_LatestEntities();
        protected readonly SchoolRepository _schoolRepository;
        protected readonly ParentRepository _parentRepository;
        protected readonly TbLogin _user;
        protected  TbParent _parentUser;

        public PreLoginController(SchoolRepository schoolRepository,ParentRepository parentRepository)
        {
            _schoolRepository = schoolRepository;
            _parentRepository = parentRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;

            if (http.User?.Identity?.IsAuthenticated == true)
            {
                // Read cookie safely in ASP.NET Core
                if (http.Request.Cookies.TryGetValue("UserType", out string userTypeStr))
                {
                    // No Server.HtmlEncode in Core — use WebUtility.HtmlEncode if needed
                    long userType = Convert.ToInt64(userTypeStr);

                    // ───────────────────────────────────────────────
                    // SESSION (OPTIONAL): ASP.NET Core uses HttpContext.Session
                    // Make sure AddSession() + UseSession() are added in Program.cs
                    // ───────────────────────────────────────────────
                    /*
                    if (!http.Session.TryGetValue("User", out _))
                    {
                        long userId = long.Parse(http.User.Identity.Name);
                        var user = _Entities.TbLogins.FirstOrDefault(x => x.UserId == userId);

                        // Store user as JSON (because Session only supports byte[])
                        string json = System.Text.Json.JsonSerializer.Serialize(user);
                        http.Session.SetString("User", json);
                    }
                    */

                    // ───────────────────────────────────────────────
                    // REDIRECT DEPENDING ON USER ROLE
                    // ───────────────────────────────────────────────

                    if (userType == (int)UserRole.School)
                        context.Result = new RedirectResult("/School/Home");

                    else if (userType == (int)UserRole.Staff)
                        context.Result = new RedirectResult("/School/Home");

                    else if (userType == (int)UserRole.Teacher)
                        context.Result = new RedirectResult("/School/Home");

                    else if (userType == (int)UserRole.Parent)
                        context.Result = new RedirectResult("/Parent/ParentHome");

                    else if (userType == (int)UserRole.Master)
                        context.Result = new RedirectResult("/School/Home");

                    return;
                }
            }

            base.OnActionExecuting(context);
        }



       
    }
}

