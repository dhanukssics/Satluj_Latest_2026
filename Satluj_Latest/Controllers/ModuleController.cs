using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Data;
using Satluj_Latest.DataLibrary.Repository;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using Satluj_Latest.Controllers;


namespace Satluj_Latest.Controllers
{
    public class ModuleController : BaseController
    {
        private readonly DropdownData _dropdown;
        public ModuleController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities, DropdownData dropdown) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
            _dropdown = dropdown;
        }
        
       
        public IActionResult UserTypeDefineHome()
        {
            var model = new UserModuleModel
            {
                SchoolId = _user.SchoolId,
                mainList = new List<MainModuleList>()
            };

            var list = _Entities.TbSubModules.Where(x => x.IsActive).ToList();

            var mainList = list
                .Select(x => x.Main.MainModule)
                .Distinct()
                .ToList();

            foreach (var item in mainList)
            {
                string subIdString = string.Empty;
                string main = Convert.ToString(item);
                var sub = list.Where(x => x.Main.MainModule == main).ToList();

                var one = new MainModuleList
                {
                    Id = sub.FirstOrDefault()?.Id ?? 0,
                    ModuleName = main,
                    subList = new List<SubModuleList>()
                };

                foreach (var a in sub)
                {
                    var b = new SubModuleList
                    {
                        MainId = one.Id,
                        Id = a.Id,
                        SubMosule = a.SubModule
                    };

                    one.subList.Add(b);

                    if (string.IsNullOrEmpty(subIdString))
                        subIdString = a.Id.ToString();
                    else
                        subIdString = $"{subIdString},{a.Id}";
                }

                one.subIdListString = subIdString;
                model.mainList.Add(one);
            }

            model.IsAdmin = false;
            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitAddUserModule(UserModuleModel model)
        {
            string msg = "Failed";
            bool status = false;

            if (_Entities.TbUserModuleMains.Any(x =>
                    x.UserTypeName.ToUpper().Trim() == model.UserTypeName.ToUpper().Trim()
                    && x.SchoolId == _user.SchoolId
                    && x.IsActive))
            {
                msg = "This user type already exists!";
            }
            else
            {
                var main = new TbUserModuleMain
                {
                    IsTeacher = Convert.ToBoolean(model.userType),
                    SchoolId = _user.SchoolId,
                    UserTypeName = model.UserTypeName,
                    IsActive = true,
                    TimeStamp = CurrentTime,
                    IsAdmin = model.IsAdmin
                };

                _Entities.TbUserModuleMains.Add(main);
                status = _Entities.SaveChanges() > 0;

                if (status)
                {
                    foreach (var item in model.subListOnly ?? Enumerable.Empty<SubModuleList>())
                    {
                        var mainDetails = new SubModule(item.Id); 
                        var sub = new TbUserModuleDetail
                        {
                            UserModuleId = main.Id,
                            MainId = mainDetails.MainId,
                            SubModuleId = item.Id,
                            IsActive = true,
                            TimeStamp = CurrentTime
                        };

                        _Entities.TbUserModuleDetails.Add(sub);
                        status = _Entities.SaveChanges() > 0;
                    }
                }
            }

            if (status)
                msg = "Successfull";

            return Json(new { status, msg });
        }

        public IActionResult UserModuleListView()
        {
            int count = 0;
            var model = new UserTypeListData
            {
                list = new List<ModelList>()
            };

            // Use ThenBy for multiple sort keys
            var data = _Entities.TbUserModuleDetails
                .Include(d => d.UserModule)
                .Include(d => d.SubModule)
                .Include(d => d.Main)
                .Where(x => x.UserModule.SchoolId == _user.SchoolId && x.IsActive)
                .OrderBy(x => x.UserModule.UserTypeName)
                .ThenBy(x => x.UserModule.TbUserModuleDetails)
                .ThenBy(x => x.SubModule.SubModule)
                .ToList();

            foreach (var item in data)
            {
                count++;
                var one = new ModelList
                {
                    Id = item.UserModule.Id,
                    UserType = item.UserModule.UserTypeName,
                    Main = item.Main.MainModule,
                    Sub = item.SubModule.SubModule,
                    SlNo = count,
                    Type = item.UserModule.IsTeacher == true ? "Teacher" : "Staff",
                    SubId = item.Id
                };

                model.list.Add(one);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteModule(string id)
        {
            bool status = false;
            string msg = "Failed";

            if (!long.TryParse(id, out var Id))
                return Json(new { status = false, msg = "Invalid id" });

            var module = _Entities.TbUserModuleDetails.FirstOrDefault(x => x.Id == Id);
            if (module == null)
                return Json(new { status = false, msg = "Record not found" });

            module.IsActive = false;
            status = _Entities.SaveChanges() > 0;

            if (status)
            {
                var mainData = _Entities.TbUserModuleDetails
                    .FirstOrDefault(x => x.UserModuleId == module.UserModuleId && x.IsActive);

                if (mainData == null)
                {
                    var main = _Entities.TbUserModuleMains.FirstOrDefault(x => x.Id == module.UserModuleId);
                    if (main != null)
                    {
                        main.IsActive = false;
                        _Entities.SaveChanges();

                        if (main.IsTeacher == true)
                        {
                            var teachers = _Entities.TbTeachers
                                .Where(x => x.UserType == main.Id && x.IsActive)
                                .ToList();

                            foreach (var item in teachers)
                            {
                                item.UserType = null;
                                _Entities.SaveChanges();
                            }
                        }
                        else
                        {
                            var staff = _Entities.TbStaffs
                                .Where(x => x.UserType == main.Id && x.IsActive)
                                .ToList();

                            foreach (var item in staff)
                            {
                                item.UserType = null;
                                _Entities.SaveChanges();
                            }
                        }
                    }
                }

                msg = "Successful";
            }

            return Json(new { status, msg });
        }

        public IActionResult UserTypeDataEdit()
        {
            var model = new UserModuleModel
            {
                SchoolId = _user.SchoolId,
                mainList = new List<MainModuleList>()
            };

            return View(model);
        }

        public PartialViewResult EditListOfUserType(string id)
        {
            var parts = id.Split('~');
            if (parts.Length < 2)
                return PartialView("~/Views/Module/_pv_Edit_UserType.cshtml", null);

            var userType = Convert.ToInt64(parts[0]);
            var typeId = Convert.ToInt64(parts[1]); // kept for parity though not used below (original code had it)

            var userData = _Entities.TbUserModuleDetails.Where(x => x.UserModuleId == userType).ToList();

            var model = new UserModuleModel
            {
                SchoolId = _user.SchoolId,
                mainList = new List<MainModuleList>()
            };

            var list = _Entities.TbSubModules.Where(x => x.IsActive).ToList();
            var mainList = list.Select(x => x.Main.MainModule).Distinct().ToList();

            foreach (var item in mainList)
            {
                string subIdString = string.Empty;
                string main = Convert.ToString(item);
                var sub = list.Where(x => x.Main.MainModule == main).ToList();
                var userMainExists = userData.Where(x => x.Main.MainModule == main && x.IsActive).ToList();

                var one = new MainModuleList
                {
                    Id = sub.FirstOrDefault()?.Id ?? 0,
                    ModuleName = main,
                    IsExistsMain = userMainExists.Count == sub.Count,
                    subList = new List<SubModuleList>()
                };

                foreach (var a in sub)
                {
                    var b = new SubModuleList
                    {
                        MainId = one.Id,
                        Id = a.Id,
                        SubMosule = a.SubModule
                    };

                    one.subList.Add(b);

                    if (string.IsNullOrEmpty(subIdString))
                        subIdString = a.Id.ToString();
                    else
                        subIdString = $"{subIdString},{a.Id}";

                    var userSubExists = userData.FirstOrDefault(x => x.SubModuleId == a.Id && x.IsActive);
                    b.IsExists = userSubExists != null;
                }

                one.subIdListString = subIdString;
                model.mainList.Add(one);
            }

            return PartialView("~/Views/Module/_pv_Edit_UserType.cshtml", model);
        }

        public IActionResult LoadAllUserTypes(string id)
        {
            var parts = id.Split('~');
            if (parts.Length < 2)
                return Json(new { status = false, list = new List<object>() });

            int IsTeacher = Convert.ToInt32(parts[0]);
            long schoolId = Convert.ToInt64(parts[1]);

            if (IsTeacher == 0) // Staff
            {
                
                var result = _dropdown.GetAllStaffUserTypesData(schoolId);

                return Json(new { status = result.Count > 0, list = result });
            }
            else // Teacher
            {
                
                var result = _dropdown.GetAllTeacherUserTypeData(schoolId);

                return Json(new { status = result.Count > 0, list = result });
            }
        }

        [HttpPost]
        public IActionResult SubmitEditUserModule(UserModuleModel model)
        {
            string msg = "Failed";
            bool status = false;

            var data1 = _Entities.TbUserModuleMains.FirstOrDefault(x => x.Id == model.UserTypeId);
            if (data1 != null)
            {
                if (data1.IsAdmin != model.IsAdmin)
                {
                    data1.IsAdmin = model.IsAdmin;
                    _Entities.SaveChanges();
                }
            }

            foreach (var item in model.subListOnly ?? Enumerable.Empty<SubModuleList>())
            {
                var data = _Entities.TbSubModules.FirstOrDefault(x => x.Id == item.Id && x.IsActive);
                if (data == null)
                    continue;

                var old = _Entities.TbUserModuleDetails
                    .FirstOrDefault(x => x.MainId == data.MainId && x.SubModuleId == item.Id && x.IsActive && x.UserModuleId == model.UserTypeId);

                if (old == null) // avoid duplication
                {
                    var addData = new TbUserModuleDetail
                    {
                        UserModuleId = model.UserTypeId,
                        MainId = data.MainId,
                        SubModuleId = item.Id,
                        IsActive = true,
                        TimeStamp = CurrentTime
                    };

                    _Entities.TbUserModuleDetails.Add(addData);
                    status = _Entities.SaveChanges() > 0;
                }
            }

            if (status)
                msg = "Successfull";

            return Json(new { status, msg });
        }

        public IActionResult CheckThatIsAnAdmin(string id)
        {
            if (!long.TryParse(id, out var userTypeId))
                return Json(new { status = false, msg = "Invalid id", isAdmin = false });

            string msg = "Failed";
            bool status = false;
            bool isAdmin = false;

            var data = _Entities.TbUserModuleMains.FirstOrDefault(x => x.Id == userTypeId && x.IsActive);
            if (data != null)
                isAdmin = data.IsAdmin ?? false;

            if (status)
                msg = "Successfull";

            return Json(new { status, msg, isAdmin });
        }
    }
}
