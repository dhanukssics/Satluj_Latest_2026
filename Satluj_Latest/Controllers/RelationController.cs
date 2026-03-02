using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Satluj_Latest.Controllers;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Satluj_Latest.Models.SchoolValue;

namespace Satluj_Latest.Controllers
{
    public class RelationController : BaseController
    {
        private readonly DropdownData _dropdown;
        public RelationController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
        }

        public ActionResult RelationHome()
        {
            RelationModel model = new RelationModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.TeacherList = _dropdown.GetTeachers(model.SchoolId);
            ViewBag.RegionList = _dropdown.GetRegion(model.SchoolId);

            ViewBag.SubjectList = new Satluj_Latest.Data.School(model.SchoolId)
                                        .GetAllSubjects()
                                        .OrderBy(x => x.SubjectName)
                                        .ToList();

            ViewBag.UserTypeList =System.Enum.GetValues(typeof(UsersDesignation))
                .Cast<UsersDesignation>()
                .Select(e => new SelectListItem
                {
                    Text = e.ToString(),
                    Value = ((int)e).ToString()
                })
                .ToList();

            return View(model);
        }










    }
}
