using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Satluj_Latest.Data
{
    public class DropdownData
    {
               
        private readonly SchoolDbContext _Entities;       

        public DropdownData(SchoolDbContext entities)
        {
            _Entities = entities;
        }

        
        public List<SelectListItem> GetUnPublishedClasses(long schoolId)
        {
            var input = _Entities.TbClasses
                .Where(z => z.IsActive && z.PublishStatus == false && z.SchoolId == schoolId)
                .OrderBy(z => z.ClassOrder)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllClasses(long schoolId, long acYearId)
        {
            var input = _Entities.TbClasses
                .Where(x => x.AcademicYearId == acYearId && x.SchoolId == schoolId && x.IsActive)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
        }

        public List<SelectListItem> GetCurrentAcademicYear()
        {
            var years = _Entities.TbAcademicYears.ToList();
            var input = years.Where(z => z.IsActive && z.CurrentYear == true).OrderBy(z => z.YearId).ToList();
            return input.Select(x => new SelectListItem { Text = x.AcademicYear, Value = x.YearId.ToString() }).ToList();
        }

        public List<SelectListItem> GetClassAllList()
        {
            var input = _Entities.TbClassLists.Where(z => z.IsActive).OrderBy(z => z.OrderValue).ToList();
            return input.Select(x => new SelectListItem { Text = x.ClassName, Value = x.ClassName.ToString() }).ToList();
        }

        public List<SelectListItem> GetOtherAcademicYear()
        {
            var years = _Entities.TbAcademicYears.ToList();
            var notYearId = years.FirstOrDefault()?.YearId ?? 0;
            var input = years.Where(z => z.YearId != notYearId).OrderBy(z => z.YearId).ToList();
            return input.Select(x => new SelectListItem { Text = x.AcademicYear, Value = x.YearId.ToString() }).ToList();
        }

        public  List<SelectListItem> GetClasses(long schoolId)
        {
            var input = _Entities.TbClasses
                .Where(z => z.IsActive && z.PublishStatus && z.SchoolId == schoolId)
                .OrderBy(z => z.ClassOrder)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
        }

        public List<SelectListItem> GetClassList()
        {
            var input = _Entities.TbClassLists.Where(z => z.IsActive).OrderBy(z => z.OrderValue).ToList();
            return input.Select(x => new SelectListItem { Text = x.ClassName, Value = x.OrderValue.ToString() }).ToList();
        }

        public  List<SelectListItem> GetDivision(long classId)
        {
            var input = _Entities.TbDivisions
                .Where(z => z.IsActive && z.ClassId == classId)
                .OrderBy(z => z.Division)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() }).ToList();
        }

        public List<SelectListItem> GetBus(long schoolId)
        {
            var input = _Entities.TbBus
                .Where(z => z.IsActive && z.SchoolId == schoolId)
                .OrderBy(z => z.BusSpecialId)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.BusSpecialId, Value = x.BusId.ToString() }).ToList();
        }

        public List<SelectListItem> GetBusName(long schoolId)
        {
            var input = _Entities.TbBus
                .Where(z => z.IsActive && z.SchoolId == schoolId)
                .OrderBy(z => z.BusSpecialId)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.BusName, Value = x.BusId.ToString() }).ToList();
        }

        public  List<SelectListItem> GetFreeSchoolDivision(long id, long schoolid)
        {
            var result = _Entities.UnassignedDivisionResults
                    .FromSqlRaw("EXEC SP_UnassignedDivisions @SchoolId = {0}, @ClassId = {1}", schoolid, id)
                    .ToList();

            return result.Select(x => new SelectListItem
            {
                Text = x.Division,
                Value = x.DivisionId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetSchoolList()
        {
            var input = _Entities.TbLogins.Where(z => z.IsActive && z.RoleId == (int)UserRole.School).ToList();
            return input.Select(x => new SelectListItem { Text = x.School.SchoolName, Value = x.SchoolId.ToString() }).ToList();
        }

        public List<SelectListItem> GetSchoolPaymentGatwayList()
        {
            var input = _Entities.TbLogins
                .Where(z => z.IsActive && z.RoleId == (int)UserRole.School && z.School.PaymentOption == true)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.School.SchoolName, Value = x.SchoolId.ToString() }).ToList();
        }

        public List<SelectListItem> GetFreeClasses(long schoolId)
        {
            var result = _Entities.Database
                .SqlQueryRaw<SP_UnassignedTeachers_Result>(
                    "EXEC SP_UnassignedTeachers @SchoolId = {0}", schoolId)
                .ToList();

            return result.Select(x => new SelectListItem
            {
                Text = x.ClassName,
                Value = x.ClassId.ToString()
            }).ToList();
        }


        public List<SelectListItem> GetFreeDivision(long classId)
        {
            var assignedIds = _Entities.TbTeacherClasses
                .Where(t => t.ClassId == classId)
                .Select(t => t.DivisionId)
                .ToList();

            var divisions = _Entities.TbDivisions
                .Where(d => d.IsActive && d.ClassId == classId)
                .ToList();

            var filtered = divisions
                .Where(d => !assignedIds.Contains(d.DivisionId))
                .OrderBy(d => d.Division)
                .ToList();

            return filtered.Select(d => new SelectListItem
            {
                Text = d.Division,
                Value = d.DivisionId.ToString()
            }).ToList();
        }


        public List<SelectListItem> RefreshClasses(long schoolId)
        {
            var input = _Entities.TbClasses
                .Where(z => z.IsActive && z.SchoolId == schoolId && z.PublishStatus)
                .OrderBy(z => z.ClassOrder)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
        }

        public List<SelectListItem> RefreshClassesUnPublished(long schoolId)
        {
            var year = _Entities.TbAcademicYears.FirstOrDefault();
            var input = _Entities.TbClasses
                .Where(z => z.IsActive && z.SchoolId == schoolId && (year != null && year.YearId != z.AcademicYearId))
                .OrderBy(z => z.ClassOrder)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
        }

        public List<SelectListItem> RefreshSchoolFees(long schoolId)
        {
            var input = _Entities.TbFees
                .Where(z => z.IsActive && z.SchoolId == schoolId && z.FeeType == (int)FeeType.CommonFee)
                .OrderBy(z => z.FeeId)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.FeesName, Value = x.FeeId.ToString() }).ToList();
        }

        public List<SelectListItem> SchoolSpecialFeesList(long schoolId)
        {
            // Defensive null check for DbContext
            if (_Entities == null || _Entities.TbFees == null)
                return new List<SelectListItem>();

            var input = _Entities.TbFees
                .Where(z => z.IsActive
                            && z.SchoolId == schoolId
                            && z.FeeType == (int)FeeType.SpecialFee)
                .OrderBy(z => z.FeeId)
                .ToList();

            if (input == null || !input.Any())
                return new List<SelectListItem>();

            return input
                .Select(x => new SelectListItem
                {
                    Text = x.FeesName ?? "(No Name)",
                    Value = x.FeeId.ToString()
                })
                .ToList();
        }

        public List<SelectListItem> GetFreeTeacherClasses(long schoolId, long teacherId)
        {
            var teacherClass = _Entities.TbTeacherClasses
                .Where(z => z.Class.SchoolId == schoolId && z.TeacherId != teacherId)
                .Select(z => z.DivisionId)
                .ToList();

            var inputClassIds = _Entities.TbDivisions
                .Where(z => z.IsActive && !teacherClass.Contains(z.DivisionId) && z.Class.SchoolId == schoolId)
                .Select(z => z.ClassId)
                .Distinct()
                .ToList();

            return _Entities.TbClasses
                .Where(z => inputClassIds.Contains(z.ClassId) && z.IsActive && z.SchoolId == schoolId && z.PublishStatus == true)
                .OrderBy(z => z.ClassOrder)
                .ToList()
                .Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() })
                .ToList();
        }

        public List<SelectListItem> GetFreeTeacherDivision(string ClassId, long teacherId)
        {
            long classId = Convert.ToInt64(ClassId);
            var teacherClass = _Entities.TbTeacherClasses
                .Where(z => z.Class.ClassId == classId && z.TeacherId != teacherId)
                .Select(z => z.DivisionId)
                .ToList();

            var input = _Entities.TbDivisions
                .Where(z => z.IsActive && !teacherClass.Contains(z.DivisionId) && z.Class.ClassId == classId)
                .OrderBy(z => z.Class.ClassOrder)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() }).ToList();
        }

        public List<SelectListItem> GetTeacherDivision(long classId, long userId)
        {
            var teacher = _Entities.TbTeachers
                .FirstOrDefault(x => x.UserId == userId);

            if (teacher == null)
                return new List<SelectListItem>();

            var divisions = (from tc in _Entities.TbTeacherClasses
                             join d in _Entities.TbDivisions
                             on tc.DivisionId equals d.DivisionId
                             where tc.TeacherId == teacher.TeacherId
                             && tc.ClassId == classId
                             select new SelectListItem
                             {
                                 Text = d.Division,
                                 Value = d.DivisionId.ToString()
                             }).ToList();

            return divisions;
        }

        public List<SelectListItem> GetTeacherClass(long userId)
        {
            var teacher = _Entities.TbTeachers.FirstOrDefault(x => x.UserId == userId);

            if (teacher == null)
                return new List<SelectListItem>();

            return _Entities.TbTeacherClasses
                .Where(x => x.TeacherId == teacher.TeacherId && x.Class != null && x.Class.PublishStatus)
                .Select(x => new SelectListItem
                {
                    Text = x.Class.Class,
                    Value = x.ClassId.ToString()
                })
                .ToList();
        }

        public List<SelectListItem> GetBookCategory(long schoolId)
        {
            var input = _Entities.TbBookCategories
                .Where(z => z.IsActive && z.SchoolId == schoolId)
                .OrderBy(z => z.Category)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Category, Value = x.CategoryId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllExams(long classId, long schoolId)
        {
            var input = _Entities.TbExams.Where(x => x.ClassId == classId && x.SchoolId == schoolId && x.IsActive).ToList();
            return input.Select(x => new SelectListItem { Text = x.ExamName, Value = x.ExamId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllSubjects(long examId)
        {
            var input = _Entities.TbExamSubjects.Where(x => x.ExamId == examId && x.IsActive).ToList();
            return input.Select(x => new SelectListItem { Text = x.Subject, Value = x.SubId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllSchoolDivision(long id, long schoolid)
        {
            var input = _Entities.TbDivisions.Where(x => x.ClassId == id && x.Class.SchoolId == schoolid && x.IsActive).ToList();
            return input.Select(x => new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAccountHeads(long schoolId)
        {
            var input = _Entities.TbAccountHeads
                .Where(x => x.IsActive && x.SchoolId == schoolId && (x.ForBill == false || x.ForBill == null))
                .OrderBy(x => x.AccHeadName)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.AccHeadName, Value = x.AccountId.ToString() }).ToList();
        }

        public List<SelectListItem> GetSubLedgerList(long accHeadId)
        {
            var input = _Entities.TbSubLedgerData
                .Where(z => z.IsActive && z.AccHeadId == accHeadId)
                .OrderBy(z => z.SubLedgerName)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.SubLedgerName, Value = x.LedgerId.ToString() }).ToList();
        }

        public List<SelectListItem> GetBankLists(long schoolId)
        {
            var input = _Entities.TbBanks
                .Where(x => x.IsActive && x.SchoolId == schoolId)
                .OrderBy(x => x.BankName)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.BankName, Value = x.BankId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAccountHeadLists(long schoolId)
        {
            var headList = _Entities.LedgerFilterResults
                .FromSqlInterpolated($"EXEC sp_LedgerFilter @SchoolId={schoolId}")
                .ToList();

            return headList.Select(x => new SelectListItem
            {
                Text = x.AccHeadName,
                Value = x.ValueId.ToString()
            }).ToList();
        }
        public List<SelectListItem> GetTeachers(long schoolId)
        {
            var input = _Entities.TbTeachers.Where(z => z.IsActive && z.SchoolId == schoolId).OrderBy(z => z.TeacherName).ToList();
            return input.Select(x => new SelectListItem { Text = x.TeacherName, Value = x.TeacherId.ToString() }).ToList();
        }

        public List<SelectListItem> GetSubjectss(long schoolId)
        {
            var input = _Entities.TbSubjects.Where(z => z.IsActive && z.SchoolI == schoolId).OrderBy(z => z.SubjectName).ToList();
            return input.Select(x => new SelectListItem { Text = x.SubjectName, Value = x.SubId.ToString() }).ToList();
        }

        public List<SelectListItem> GetFeeses(long schoolId)
        {
            var input = _Entities.TbFees.Where(z => z.IsActive && z.SchoolId == schoolId).OrderBy(z => z.FeesName).ToList();
            return input.Select(x => new SelectListItem { Text = x.FeesName, Value = x.FeeId.ToString() }).ToList();
        }

        public  List<SelectListItem> GetLabCategories(long schoolId)
        {
            var input = _Entities.TbLaboratoryCategories.Where(z => z.IsActive && z.SchoolId == schoolId).OrderBy(z => z.LaboratoryName).ToList();
            return input.Select(x => new SelectListItem { Text = x.LaboratoryName, Value = x.CategoryId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAccountHeadsWithFeeHead(long schoolId)
        {
            var input = _Entities.TbAccountHeads.Where(x => x.IsActive && x.SchoolId == schoolId).OrderBy(x => x.AccHeadName).ToList();
            return input.Select(x => new SelectListItem { Text = x.AccHeadName, Value = x.AccountId.ToString() }).ToList();
        }

        public List<SelectListItem> GetRegion(long schoolId)
        {
            var input = _Entities.TbRegionss
                .Where(z => z.IsActive && z.SchoolId == schoolId && z.TbRegionsClasses.Any(x => x.Class.PublishStatus == true))
                .OrderBy(z => z.Id)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.RegionName.ToUpper(), Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllSchoolClassesByRegion(long regionId, long schoolId)
        {
            var result = (from rc in _Entities.TbRegionsClasses
                          join c in _Entities.TbClasses
                          on rc.ClassId equals c.ClassId
                          where rc.RegionId == regionId
                                && rc.IsActive
                                && c.IsActive
                                && c.PublishStatus
                                && c.SchoolId == schoolId
                          orderby c.ClassOrder
                          select new SelectListItem
                          {
                              Text = c.Class,
                              Value = c.ClassId.ToString()
                          }).ToList();

            return result;
        }

        public List<SelectListItem> GetAllExamsDeclared(long id, long schoolid, long classId)
        {
            var input = _Entities.TbDeclaredExams.Where(x => x.ClassId == classId && x.Class.SchoolId == schoolid && x.IsActive).ToList();
            return input.Select(x => new SelectListItem { Text = x.Exam.ExamName + " " + x.Exam.Term.DefaultExam, Value = x.ExamId.ToString() }).ToList();
        }

        public List<SelectListItem> GetTermExams(long schoolId)
        {
            var input = _Entities.TbExamBooks.Where(z => z.IsActive && z.SchoolId == schoolId).OrderBy(z => z.Id).ToList();
            return input.Select(x => new SelectListItem { Text = x.ExamName + " - " + x.Term.DefaultExam.ToUpper(), Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> LoadTermsWithoutExamDeclaration(long classId, long schoolId)
        {
            var input = _Entities.WithoutDeclaredExamTermResults
                .FromSqlInterpolated($"EXEC sp_WithoutDeclaredExamTerms @SchoolId={schoolId}, @ClassId={classId}")
                .OrderBy(x => x.DefaultExam)
                .ToList();

            return input.Select(x => new SelectListItem
            {
                Text = x.DefaultExam,
                Value = x.Id.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetAllExamsSubjects(long id, long schoolid, long ClassId)
        {
            var input = _Entities.TbDeclaredExamSubjects
                .Where(x => x.DeclaredExam.SchoolId == schoolid && x.DeclaredExamId == id && x.DeclaredExam.ClassId == ClassId && x.IsActive)
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Subject.SubjectName, Value = x.SubjectId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllScholasticAreas(long schoolid, long ClassId)
        {
            var regionEntry = _Entities.TbRegionsClasses.FirstOrDefault(x => x.ClassId == ClassId && x.Class.SchoolId == schoolid && x.IsActive);
            if (regionEntry == null) return new List<SelectListItem>();

            var RegionId = regionEntry.RegionId;
            var input = _Entities.TbScholasticAreas.Where(x => x.RegionId == RegionId && x.SchoolId == schoolid && x.IsActive).ToList();
            return input.Select(x => new SelectListItem { Text = x.ItemName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllCoScholasticAreas(long schoolid, long ClassId)
        {
            var regionEntry = _Entities.TbRegionsClasses.FirstOrDefault(x => x.ClassId == ClassId && x.Class.SchoolId == schoolid && x.IsActive);
            if (regionEntry == null) return new List<SelectListItem>();

            var RegionId = regionEntry.RegionId;
            var input = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionId && x.SchoolId == schoolid && x.IsActive && (x.ClassId == ClassId || x.ClassId == null)).ToList();
            return input.Select(x => new SelectListItem { Text = x.ItemName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllOptionalSubjects(long schoolid)
        {
            var input = _Entities.TbSubjects.Where(x => x.SchoolI == schoolid && x.IsOptonal == true && x.IsActive).ToList();
            return input.Select(x => new SelectListItem { Text = x.SubjectName, Value = x.SubId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllDepartment(long schoolId)
        {
            var input = _Entities.TbDepartments.Where(x => x.SchoolId == schoolId && x.IsActive).OrderBy(x => x.DepartmentName).ToList();
            return input.Select(x => new SelectListItem { Text = x.DepartmentName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllDesignation(long schoolId)
        {
            var input = _Entities.TbDesignations.Where(x => x.SchoolId == schoolId && x.IsActive).OrderBy(x => x.DesignationName).ToList();
            return input.Select(x => new SelectListItem { Text = x.DesignationName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllRoles(long schoolId)
        {
            var input = _Entities.TbRoleDetails.Where(x => x.SchoolId == schoolId && x.IsActive).OrderBy(x => x.RoleName).ToList();
            return input.Select(x => new SelectListItem { Text = x.RoleName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllUserTypeData(long schoolId)
        {
            var input = _Entities.TbUserModuleMains.Where(x => x.SchoolId == schoolId && x.IsActive).OrderBy(x => x.UserTypeName).ToList();
            return input.Select(x => new SelectListItem { Text = x.UserTypeName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllTeacherUserTypeData(long schoolId)
        {
            var input = _Entities.TbUserModuleMains.Where(x => x.SchoolId == schoolId && x.IsActive && x.IsTeacher == true).OrderBy(x => x.UserTypeName).ToList();
            return input.Select(x => new SelectListItem { Text = x.UserTypeName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllStaffUserTypesData(long schoolId)
        {
            var input = _Entities.TbUserModuleMains.Where(x => x.SchoolId == schoolId && x.IsActive && x.IsTeacher == false).OrderBy(x => x.UserTypeName).ToList();
            return input.Select(x => new SelectListItem { Text = x.UserTypeName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllTeacherUserTypeDataAdd(long schoolId)
        {
            return GetAllTeacherUserTypeData(schoolId);
        }

        public List<SelectListItem> GetAllStaffUserTypesDataAdd(long schoolId)
        {
            return GetAllStaffUserTypesData(schoolId);
        }

        public List<SelectListItem> GetTeachersNotAdmins(long schoolId)
        {
            var input = _Entities.TbTeachers.Where(z => z.IsActive && z.SchoolId == schoolId).OrderBy(z => z.TeacherName).ToList();
            return input.Select(x => new SelectListItem { Text = x.TeacherName, Value = x.TeacherId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllExamsDeclaredByRegion(long id, long schoolid, long ClassId)
        {
            return GetAllExamsDeclared(id, schoolid, ClassId);
        }

        public List<SelectListItem> GetPeriods(long classId)
        {
            var input = _Entities.TbAcademicPeriods.Where(z => z.IsActive && z.ClassId == classId).OrderBy(z => z.StartDate).ToList();
            return input.Select(x => new SelectListItem { Text = x.PeriodsName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetClassesUserWise(long schoolId, long userId)
        {
            var allowed = _Entities.TbTeacherClassSubjects.Where(x => x.SchoolId == schoolId && x.Teacher.User.UserId == userId && x.IsActive && x.Teacher.IsActive).ToList();
            var input = _Entities.TbClasses.Where(z => z.IsActive && z.PublishStatus && z.SchoolId == schoolId).OrderBy(z => z.ClassOrder).ToList();
            var data = from a in input where (from o in allowed select o.ClassId).Contains(a.ClassId) select a;
            return data.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllSchoolDivisionUserWise(long id, long schoolid, long userId)
        {
            var allowed = _Entities.TbTeacherClassSubjects.Where(x => x.SchoolId == schoolid && x.Teacher.User.UserId == userId && x.IsActive && x.Teacher.IsActive).ToList();
            var input = _Entities.TbDivisions.Where(x => x.ClassId == id && x.Class.SchoolId == schoolid && x.IsActive).ToList();
            var data = from a in input where (from o in allowed select o.DivisionId).Contains(a.DivisionId) select a;
            return data.Select(x => new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllSubjectsUserWise(long examId, long userId, long schoolid)
        {
            var allowed = _Entities.TbTeacherClassSubjects.Where(x => x.SchoolId == schoolid && x.Teacher.User.UserId == userId && x.IsActive && x.Teacher.IsActive).ToList();
            var input = _Entities.TbExamSubjects.Where(x => x.ExamId == examId && x.IsActive).ToList();
            var data = from a in input where (from o in allowed select o.SubjectId).Contains(a.SubjectId) select a;
            return data.Select(x => new SelectListItem { Text = x.Subject, Value = x.SubId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllClassesTeacherWise(long teacherId, long schoolId)
        {
            var input = _Entities.TbTeacherClassSubjects
                .Where(x => x.SchoolId == schoolId && x.TeacherId == teacherId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive)
                .OrderBy(x => x.Class.ClassOrder)
                .Select(x => new { x.ClassId, x.Class.Class })
                .Distinct()
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Class.ToString(), Value = x.ClassId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllDivisionTeacherWise(long classId, long schoolId, long teacherId)
        {
            var input = _Entities.TbTeacherClassSubjects
                .Where(x => x.SchoolId == schoolId && x.ClassId == classId && x.TeacherId == teacherId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive)
                .ToList()
                .Select(x => new { x.DivisionId, x.Division.Division })
                .Distinct()
                .ToList();

            return input.Select(x => new SelectListItem { Text = x.Division.ToString(), Value = x.DivisionId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllSubjectsOfExamInTeacherWise(long classId, long schoolId, long teacherId, long examId, long divisionId)
        {
            List<long> allowed;
            if (divisionId != 0)
            {
                allowed = _Entities.TbTeacherClassSubjects
                    .Where(x => x.SchoolId == schoolId && x.DivisionId == divisionId && x.ClassId == classId && x.TeacherId == teacherId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive)
                    .Select(x => x.SubjectId)
                    .ToList();
            }
            else
            {
                allowed = _Entities.TbTeacherClassSubjects
                    .Where(x => x.SchoolId == schoolId && x.ClassId == classId && x.TeacherId == teacherId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive)
                    .Select(x => x.SubjectId)
                    .ToList();
            }

            var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == examId && x.IsActive).ToList();
            var input = from a in exams where allowed.Contains(a.SubjectId) select a;
            return input.Select(x => new SelectListItem { Text = x.Subject, Value = x.SubId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllAllowdedExamsSubjects(long id, long schoolid, long classId, long userId, long divisionId)
        {
            List<long> allowed;
            if (divisionId != 0)
            {
                allowed = _Entities.TbTeacherClassSubjects
                    .Where(x => x.SchoolId == schoolid && x.DivisionId == divisionId && x.ClassId == classId && x.Teacher.UserId == userId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive)
                    .Select(x => x.SubjectId)
                    .ToList();
            }
            else
            {
                allowed = _Entities.TbTeacherClassSubjects
                    .Where(x => x.SchoolId == schoolid && x.ClassId == classId && x.Teacher.UserId == userId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive)
                    .Select(x => x.SubjectId)
                    .ToList();
            }

            var exams = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExam.SchoolId == schoolid && x.DeclaredExamId == id && x.DeclaredExam.ClassId == classId && x.IsActive).ToList();
            var input = from a in exams where allowed.Contains(a.SubjectId) select a;
            return input.Select(x => new SelectListItem { Text = x.Subject.SubjectName, Value = x.SubjectId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllSubjectsTeacherWise(long id, long schoolid, long classId, long userId, long divisionId)
        {
            var allowed = _Entities.TbTeacherClassSubjects
                .Where(x => x.SchoolId == schoolid && x.ClassId == classId && x.Teacher.UserId == userId && x.DivisionId == divisionId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive)
                .Select(x => x.SubjectId)
                .ToList();

            
            var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == id && x.IsActive && x.Exam.Class.ClassId == classId && x.Exam.SchoolId == schoolid).ToList();
            var input = from a in exams where allowed.Contains(a.SubjectId) select a;
            return input.Select(x => new SelectListItem { Text = x.Subject, Value = x.SubId.ToString() }).ToList();
        }

        public List<SelectListItem> GetPeriodsLists(long schoolId)
        {
            var classFive = _Entities.TbClasses.FirstOrDefault(x => x.SchoolId == schoolId && x.Class.Trim().ToUpper() == "V" && x.IsActive && x.PublishStatus);
            if (classFive == null) return new List<SelectListItem>();

            var input = _Entities.TbAcademicPeriods.Where(x => x.IsActive && x.SchoolId == schoolId && x.ClassId == classFive.ClassId).OrderBy(x => x.PeriodsName).ToList();
            return input.Select(x => new SelectListItem { Text = x.PeriodsName, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllRemarks(long schoolId)
        {
            var input = _Entities.TbRemarks.Where(x => x.SchoolId == schoolId && x.IsActive).OrderBy(x => x.Remark).ToList();
            var list = new List<RemarkClass>();

            foreach (var item in input)
            {
                list.Add(new RemarkClass { Id = item.Id, Remark = item.Remark, SchoolId = item.SchoolId });
            }

            var xx = new RemarkClass { Id = 0, Remark = " --Choose-- ", SchoolId = (input.Count > 0 ? input[0].SchoolId : 0) };
            list.Add(xx);
            list = list.OrderBy(x => x.Id).ToList();

            return list.Select(x => new SelectListItem { Text = x.Remark, Value = x.Remark.ToString() }).ToList();
        }

        public List<SelectListItem> GetVClassDivisinLists(long schoolId)
        {
            var classFive = _Entities.TbClasses.FirstOrDefault(x => x.SchoolId == schoolId && x.Class.Trim().ToUpper() == "V" && x.IsActive && x.PublishStatus);
            if (classFive == null) return new List<SelectListItem>();

            var input = _Entities.TbDivisions.Where(z => z.IsActive && z.ClassId == classFive.ClassId).OrderBy(z => z.Division).ToList();
            return input.Select(x => new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() }).ToList();
        }

        public List<SelectListItem> GetSpecialSubjects(long Id, long SchoolId)
        {
            IQueryable<TbSubject> input;
            if (Id == (int)VSpecialMarks.ReadingSkill || Id == (int)VSpecialMarks.WritingSkill || Id == (int)VSpecialMarks.SpeakingSkills || Id == (int)VSpecialMarks.ListeningSkill)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 0);
            }
            else if (Id == (int)VSpecialMarks.AspectsI)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 1);
            }
            else if (Id == (int)VSpecialMarks.AspectsII)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 2);
            }
            else if (Id == (int)VSpecialMarks.ArtAndCraft || Id == (int)VSpecialMarks.MusicAndDance)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 3);
            }
            else if (Id == (int)VSpecialMarks.AspectsIII)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 4);
            }
            else if (Id == (int)VSpecialMarks.AspectsIV)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 5);
            }
            else
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 1000);
            }

            return input.ToList().Select(x => new SelectListItem { Text = x.SubjectName.ToString(), Value = x.SubId.ToString() }).ToList();
        }

        public List<SelectListItem> GetVAssesments(long schoolId)
        {
            var input = _Entities.TbVAssesments.Where(x => x.SchoolId == schoolId && x.IsActive).ToList()
                        .OrderBy(x => x.AssesmentId)
                        .Select(x => new V_Assesment(x))
                        .ToList();

            return input.Select(x => new SelectListItem { Text = x.AssesmentName(), Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetVClassDivisinListsByUser(long schoolId, long userId)
        {
            var classFive = _Entities.TbClasses.FirstOrDefault(x => x.SchoolId == schoolId && x.Class.Trim().ToUpper() == "V" && x.IsActive && x.PublishStatus);
            if (classFive == null) return new List<SelectListItem>();

            var divisionList = _Entities.TbDivisions.Where(z => z.IsActive && z.ClassId == classFive.ClassId).OrderBy(z => z.Division).ToList();
            var allowed = _Entities.TbTeacherClassSubjects
                .Where(x => x.SchoolId == schoolId && x.ClassId == classFive.ClassId && x.Teacher.UserId == userId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive)
                .Select(x => x.DivisionId)
                .ToList();

            var input = from a in divisionList where allowed.Contains(a.DivisionId) select a;
            return input.Select(x => new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() }).ToList();
        }

        public List<SelectListItem> GetSpecialSubjectsByTeacher(long Id, long SchoolId, long userId, long divisionId)
        {
            var allowed = _Entities.TbTeacherClassSubjects
                .Where(x => x.SchoolId == SchoolId && x.DivisionId == divisionId && x.Teacher.UserId == userId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive)
                .Select(x => x.SubjectId)
                .ToList();

            IQueryable<TbSubject> input;
            if (Id == (int)VSpecialMarks.ReadingSkill || Id == (int)VSpecialMarks.WritingSkill || Id == (int)VSpecialMarks.SpeakingSkills || Id == (int)VSpecialMarks.ListeningSkill)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 0);
            }
            else if (Id == (int)VSpecialMarks.AspectsI)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 1);
            }
            else if (Id == (int)VSpecialMarks.AspectsII)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 2);
            }
            else if (Id == (int)VSpecialMarks.ArtAndCraft || Id == (int)VSpecialMarks.MusicAndDance)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 3);
            }
            else if (Id == (int)VSpecialMarks.AspectsIII)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 4);
            }
            else if (Id == (int)VSpecialMarks.AspectsIV)
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 5);
            }
            else
            {
                input = _Entities.TbSubjects.Where(x => x.SchoolI == SchoolId && x.IsActive && x.EnumTypeId == 1000);
            }

            var filtered = from a in input.ToList() where allowed.Contains(a.SubId) select a;
            return filtered.Select(x => new SelectListItem { Text = x.SubjectName.ToString(), Value = x.SubId.ToString() }).ToList();
        }

        public List<SelectListItem> GetAllScholasticAreasSpecial(long schoolid, long ClassId, long examId)
        {
            var regionEntry = _Entities.TbRegionsClasses.FirstOrDefault(x => x.ClassId == ClassId && x.Class.SchoolId == schoolid && x.IsActive);
            if (regionEntry == null) return new List<SelectListItem>();

            var RegionId = regionEntry.RegionId;
            var examTerm = new DeclaredExams(examId).TermId; 
            var input = _Entities.TbScholasticAreas.Where(x => x.RegionId == RegionId && x.SchoolId == schoolid && x.IsActive && (x.SpecificTerm == null || x.SpecificTerm == examTerm)).ToList();
            return input.Select(x => new SelectListItem { Text = x.ItemName, Value = x.Id.ToString() }).ToList();
        }

       
        public class RemarkClass
        {
            public long Id { get; set; }
            public long SchoolId { get; set; }
            public string Remark { get; set; }
        }

        public class Preschool
        {
            public string Preschools { get; set; }
            public string Id { get; set; }
        }

        public static List<Preschool> Preschool_Li = new List<Preschool>()
        {
           new Preschool { Preschools  = "Nursery" , Id = "1"},
           new Preschool { Preschools  = "LKG" , Id = "2"},
           new Preschool { Preschools  = "UKG" , Id = "3"}
        };

        public List<SelectListItem> GetClass_Preschool(long schoolId)
        {
            var input = _Entities.TbClasses.Where(z => z.IsActive && z.PublishStatus && z.SchoolId == schoolId && (z.Class == "NURSERY" || z.Class == "LKG" || z.Class == "UKG")).OrderBy(z => z.ClassOrder).ToList();
            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
        }


        private class DivissionModel{
             public long DivisionId { get; set; }
            public long ClassId { get; set; }
            public string Division { get; set; }
            public System.DateTime TimeStamp { get; set; }
            public System.Guid DivisionGuid { get; set; }
            public bool IsActive { get; set; }
        }

        public  List<SelectListItem> GetVClassDivisinLists_Others(long schoolId, string classType)
        {
            // var classOthers = _Entities.tb_Class.Where(x => x.SchoolId == schoolId &&( x.Class.Trim().ToUpper() == "NURSERY" || x.Class.Trim().ToUpper() == "LKG" || x.Class.Trim().ToUpper() == "UKG") && x.IsActive && x.PublishStatus).ToList();
            var classOthers = _Entities.TbClasses.Where(x => x.SchoolId == schoolId && x.Class.Trim().ToUpper() == classType && x.IsActive && x.PublishStatus).ToList();

            List<DivissionModel> li = new List<DivissionModel>();
            foreach (var a1 in classOthers)
            {
                var divissionDetails = _Entities.TbDivisions.Where(z => z.IsActive && z.ClassId == a1.ClassId).OrderBy(z => z.Division).ToList();
                foreach (var a2 in divissionDetails)
                {
                    DivissionModel di = new DivissionModel
                    {
                        Division = a2.Division,
                        DivisionId = a2.DivisionId
                    };
                    li.Add(di);
                }

                
            }
            return li.Select(x => new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() }).ToList();
        }


        private class TeacherClassSubjectModel
        {
            //public long Id { get; set; }
            //public long SchoolId { get; set; }
            //public long TeacherId { get; set; }
            //public long ClassId { get; set; }
            public long DivisionId { get; set; }
            //public long SubjectId { get; set; }
            //public bool IsActive { get; set; }
            //public System.DateTime TimeStamp { get; set; }
        }


        public  List<SelectListItem> GetVClassDivisinListsByUser__Others(long schoolId, long UserId)
        {
            var classOthers = _Entities.TbClasses.Where(x => x.SchoolId == schoolId && (x.Class.Trim().ToUpper() == "NURSERY" || x.Class.Trim().ToUpper() == "LKG" || x.Class.Trim().ToUpper() == "UKG") && x.IsActive && x.PublishStatus).ToList();

            List<DivissionModel> divisionList = new List<DivissionModel>();
            List<TeacherClassSubjectModel> allowded1 = new List<TeacherClassSubjectModel>();
            foreach (var a1 in classOthers)
            {
                var divissionDetails = _Entities.TbDivisions.Where(z => z.IsActive && z.ClassId == a1.ClassId).OrderBy(z => z.Division).ToList();
                var allowdedDetails = _Entities.TbTeacherClassSubjects.Where(x => x.SchoolId == schoolId && x.ClassId == a1.ClassId && x.Teacher.UserId == UserId && x.IsActive && x.Class.PublishStatus == true && x.Class.IsActive).ToList().Select(x => x.DivisionId).ToList();


                foreach (var a2 in divissionDetails)
                {
                    DivissionModel di = new DivissionModel
                    {
                        Division = a2.Division,
                        DivisionId = a2.DivisionId,
                        ClassId = a1.ClassId
                        
                    };
                    divisionList.Add(di);
                }

                foreach (var a3 in allowdedDetails)
                {
                    TeacherClassSubjectModel di = new TeacherClassSubjectModel
                    {
                        DivisionId = a3
                    };
                    allowded1.Add(di);
                }


            }

            var eeee = allowded1;


            // var input = from a in divisionList where (from o in eeee select o).Contains(a.DivisionId) select a;
            // return input.Select(x => new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() }).ToList();
            return null;
        }

        

        public  List<SelectListItem> GetSpecialSubjects_others()
        {
            var input = Preschool_Subjects_Li.ToList();
            //var input = _Entities.tb_Class.Where(z => z.IsActive && z.PublishStatus && z.SchoolId == schoolId && (z.Class == "NURSERY" || z.Class == "LKG" || z.Class == "UKG")).OrderBy(z => z.ClassOrder).ToList();
            return input.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

        }

        public  List<SelectListItem> LoadSpecialSubjects_others_CoColastic()
        {
            var input = LoadSpecialSubjects_others_CoColastic_Li.ToList();
            //var input = _Entities.tb_Class.Where(z => z.IsActive && z.PublishStatus && z.SchoolId == schoolId && (z.Class == "NURSERY" || z.Class == "LKG" || z.Class == "UKG")).OrderBy(z => z.ClassOrder).ToList();
            return input.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

        }



        private class Preschool_Subjects
        {
            public string Subjects { get; set; }
            public string Id { get; set; }
        }

        private  List<Preschool_Subjects> LoadSpecialSubjects_others_CoColastic_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Co-scholastic Areas" , Id = "5"},
           new Preschool_Subjects { Subjects  = "Personal Assessments" , Id = "6"},
           new Preschool_Subjects { Subjects  = "Health" , Id = "7"},
           new Preschool_Subjects { Subjects  = "Remark" , Id = "8"}

        };

        private  List<Preschool_Subjects> Preschool_Subjects_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "English" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Hindi" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Mathematics" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Environmental Studies" , Id = "4"}
        };

        private  List<Preschool_Subjects> English_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Writing Skills" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Reading Skills" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Speaking Skills" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Reciting Skills" , Id = "4"},
           new Preschool_Subjects { Subjects  = "Listening Skills" , Id = "5"},
           new Preschool_Subjects { Subjects  = "Oral" , Id = "6"}
        };

        private  List<Preschool_Subjects> Mathematics_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Logical and Reasoning" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Understanding of concepts" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Mental Ability" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Oral" , Id = "4"}
        };
        
        private  List<Preschool_Subjects> EnvironmentalStudies_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Adventure Studies" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Understanding of concepts" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Activity" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Oral" , Id = "4"}
        };
        private  List<Preschool_Subjects> Hindi_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Writing Skills" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Reading Skills" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Speaking Skills" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Reciting Skills" , Id = "4"},
           new Preschool_Subjects { Subjects  = "Listening Skills" , Id = "5"},
           new Preschool_Subjects { Subjects  = "Oral" , Id = "6"},
           
        };

        public  List<SelectListItem> Scholasticareas_others(long SchoolId,string id)
        {
            switch(id)
            {
                case "1":
                    var input1 = English_Li.ToList();
                    return input1.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();
                    
                case "2":
                    var input2 = Hindi_Li.ToList();
                    return input2.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

                case "3":
                    var input3 = Mathematics_Li.ToList();
                    return input3.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

                case "4":
                    var input4 = EnvironmentalStudies_Li.ToList();
                    return input4.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();
                    
            }
            return null;

            
        }

        private  List<Preschool_Subjects> Assesments_others_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Term I" , Id = "5"},
           new Preschool_Subjects { Subjects  = "Term II" , Id = "6"},
           new Preschool_Subjects { Subjects  = "Term III" , Id = "7"},
           

        };

        public  List<SelectListItem> GetVAssesments_others(long schoolId)
        {
            //var input = _Entities.tb_V_Assesment.Where(x => x.SchoolId == schoolId && x.IsActive).ToList().OrderBy(x => x.AssesmentId).Select(x => new V_Assesment(x)).ToList();
            var input = Assesments_others_Li.ToList();
            return input.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();
        }


        //////////////////////////////////////////////////////////////////////

        public  List<SelectListItem> GetClass_Preschool_(long schoolId)
        {
            var input = _Entities.TbClasses.Where(z => z.IsActive && z.PublishStatus && z.SchoolId == schoolId && (z.Class == "NURSERY" || z.Class == "LKG" || z.Class == "UKG")).OrderBy(z => z.ClassOrder).ToList();
            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
            //  return Preschool_Li.Select(x => new SelectListItem { Text = x.Preschools, Value = x.Id }).ToList();
        }


        ////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        //////////...........One To Four.................................///////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////

        public  List<SelectListItem> GetClass_Preschool_One_Four(long schoolId)
        {
            var input = _Entities.TbClasses.Where(z => z.IsActive && z.PublishStatus && z.SchoolId == schoolId && (z.Class == "I" || z.Class == "II" || z.Class == "III" || z.Class == "IV")).OrderBy(z => z.ClassOrder).ToList();
            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
            //  return Preschool_Li.Select(x => new SelectListItem { Text = x.Preschools, Value = x.Id }).ToList();
        }


        public  List<SelectListItem> GetVClassDivisinLists_ONE_FOUR(long schoolId, string classType)
        {
            // var classOthers = _Entities.tb_Class.Where(x => x.SchoolId == schoolId &&( x.Class.Trim().ToUpper() == "NURSERY" || x.Class.Trim().ToUpper() == "LKG" || x.Class.Trim().ToUpper() == "UKG") && x.IsActive && x.PublishStatus).ToList();
            var classOthers = _Entities.TbClasses.Where(x => x.SchoolId == schoolId && x.Class.Trim().ToUpper() == classType && x.IsActive && x.PublishStatus).ToList();

            List<DivissionModel> li = new List<DivissionModel>();
            foreach (var a1 in classOthers)
            {
                var divissionDetails = _Entities.TbDivisions.Where(z => z.IsActive && z.ClassId == a1.ClassId).OrderBy(z => z.Division).ToList();
                foreach (var a2 in divissionDetails)
                {
                    DivissionModel di = new DivissionModel
                    {
                        Division = a2.Division,
                        DivisionId = a2.DivisionId
                    };
                    li.Add(di);
                }


            }
            return li.Select(x => new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() }).ToList();
        }


        private  List<Preschool_Subjects> Assesments_ONE_FOUR_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Term I" , Id = "5"},
           new Preschool_Subjects { Subjects  = "Term II" , Id = "6"},
           new Preschool_Subjects { Subjects  = "Term III" , Id = "7"},


        };

        public  List<SelectListItem> GetVAssesments_ONE_FOUR(long schoolId)
        {
            //var input = _Entities.tb_V_Assesment.Where(x => x.SchoolId == schoolId && x.IsActive).ToList().OrderBy(x => x.AssesmentId).Select(x => new V_Assesment(x)).ToList();
            var input = Assesments_ONE_FOUR_Li.ToList();
            return input.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();
        }


        public  List<SelectListItem> GetSpecialSubjects_ONE_FOUR()
        {
            var input = Preschool_Subjects_ONE_FOUR_Li.ToList();
            //var input = _Entities.tb_Class.Where(z => z.IsActive && z.PublishStatus && z.SchoolId == schoolId && (z.Class == "NURSERY" || z.Class == "LKG" || z.Class == "UKG")).OrderBy(z => z.ClassOrder).ToList();
            return input.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

        }

        private  List<Preschool_Subjects> Preschool_Subjects_ONE_FOUR_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Hindi" , Id = "1"},
           new Preschool_Subjects { Subjects  = "English" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Punjabi" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Mathematics" , Id = "4"},
           new Preschool_Subjects { Subjects  = "EVS" , Id = "5"}
        };

        private  List<Preschool_Subjects> Hindi_ONE_FOUR_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Reading Skills" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Writing Skills" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Listening Skills" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Weekly Assessment" , Id = "4"},
           new Preschool_Subjects { Subjects  = "Speaking Skills" , Id = "5"},
           new Preschool_Subjects { Subjects  = "Terminal Assessment" , Id = "6"},
           new Preschool_Subjects { Subjects  = "Aggregate Grade" , Id = "7"}
        };
        private  List<Preschool_Subjects> English_ONE_FOUR_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Reading Skills" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Writing Skills" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Listening Skills" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Weekly Assessment" , Id = "4"},
           new Preschool_Subjects { Subjects  = "Speaking Skills" , Id = "5"},
           new Preschool_Subjects { Subjects  = "Terminal Assessment" , Id = "6"},
           new Preschool_Subjects { Subjects  = "Aggregate Grade" , Id = "7"}
        };
        private  List<Preschool_Subjects> Punjabi_ONE_FOUR_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Reading Skills" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Writing Skills" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Listening Skills" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Weekly Assessment" , Id = "4"},
           new Preschool_Subjects { Subjects  = "Speaking Skills" , Id = "5"},
           new Preschool_Subjects { Subjects  = "Terminal Assessment" , Id = "6"},
           new Preschool_Subjects { Subjects  = "Aggregate Grade" , Id = "7"}
        };
        private  List<Preschool_Subjects> Mathematics_ONE_FOUR_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Concepts/Activity" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Tables" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Mental-Ability" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Written-Work" , Id = "4"},
           new Preschool_Subjects { Subjects  = "Weekly Assessment" , Id = "5"},
           new Preschool_Subjects { Subjects  = "Terminal Assessment" , Id = "6"},
           new Preschool_Subjects { Subjects  = "Aggregate Grade" , Id = "7"}
        };
        private  List<Preschool_Subjects> EVS_ONE_FOUR_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Activity / Viva" , Id = "1"},
           new Preschool_Subjects { Subjects  = "Written-Work" , Id = "2"},
           new Preschool_Subjects { Subjects  = "Weekly Assessment" , Id = "3"},
           new Preschool_Subjects { Subjects  = "Terminal Assessment" , Id = "4"},
           new Preschool_Subjects { Subjects  = "Aggregate Grade" , Id = "5"}
           
        };

        public  List<SelectListItem> Scholasticareas_ONE_FOUR(long SchoolId, string id)
        {
            switch (id)
            {
                case "1":
                    var input1 = Hindi_ONE_FOUR_Li.ToList();
                    return input1.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

                case "2":
                    var input2 = English_ONE_FOUR_Li.ToList();
                    return input2.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

                case "3":
                    var input3 = Punjabi_ONE_FOUR_Li.ToList();
                    return input3.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

                case "4":
                    var input4 = Mathematics_ONE_FOUR_Li.ToList();
                    return input4.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();
                case "5":
                    var input5 = EVS_ONE_FOUR_Li.ToList();
                    return input5.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

            }
            return null;


        }


        public  List<SelectListItem> GetSpecialSubjects_CoScolastic_ONE_FOUR()
        {
            var input = Preschool_Subjects_CoScolastic_ONE_FOUR_Li.ToList();
            //var input = _Entities.tb_Class.Where(z => z.IsActive && z.PublishStatus && z.SchoolId == schoolId && (z.Class == "NURSERY" || z.Class == "LKG" || z.Class == "UKG")).OrderBy(z => z.ClassOrder).ToList();
            return input.Select(x => new SelectListItem { Text = x.Subjects, Value = x.Id.ToString() }).ToList();

        }

        private  List<Preschool_Subjects> Preschool_Subjects_CoScolastic_ONE_FOUR_Li = new List<Preschool_Subjects>()
        {
           new Preschool_Subjects { Subjects  = "Co-Scholastic" , Id = "6"},
           new Preschool_Subjects { Subjects  = "Personality Development" , Id = "7"},
           new Preschool_Subjects { Subjects  = "Health" , Id = "8"},
           new Preschool_Subjects { Subjects  = "Remarks" , Id = "9"},
           
        };


        //18-sep-2020 Jibin Start ..........................................

        //jibin 9/8/2020
        public  List<SelectListItem> GetSubject(long UserId)
        {

            var input = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == UserId).ToList();


            if (input.Count == 0)//jibin 9/16/2020
            {
                var inputnull = _Entities.TbSubjects.ToList();

                return inputnull.Select(x => new SelectListItem { Text = "No subjects Assigned", Value = "" }).ToList();



            }//jibin 9/16/2020

            return input.Select(x => new SelectListItem { Text = x.Subject.SubjectName, Value = x.SubjectId.ToString() }).ToList();
        }

        // jibin 9/17/2020
        public  List<SelectListItem> GetAssignedSubjects(long schoolId)
        {
            var input = _Entities.TbSubjects.Where(z => z.IsActive && z.SchoolI == schoolId).OrderBy(z => z.SubjectName).ToList();

            return input.Select(x => new SelectListItem { Text = x.SubjectName, Value = x.SubId.ToString() }).ToList();
        }
        // jibin 9/17/2020



        //18-sep-2020 Jibin End..........................................


        //Gayathri(24/05/2023) -Getprevious year report card
        public  List<SelectListItem> GetAllAcademicYear()
        {
            var year = _Entities.TbAcademicYears.ToList();
            //var notYearId = year.FirstOrDefault().YearId;
            var input = year.Where(z=> z.IsActive && z.CurrentYear != true && z.YearId>3).OrderByDescending(z => z.YearId).ToList();
            return input.Select(x => new SelectListItem { Text = x.AcademicYear, Value = x.YearId.ToString() }).ToList();
        }

        public  List<SelectListItem> GetClass_Previous_One_Four(long schoolId,Int32 AcademicId)
        {
            var input = _Entities.TbClasses.Where(z => z.IsActive && z.SchoolId == schoolId && z.AcademicYearId== AcademicId && (z.Class == "I" || z.Class == "II" || z.Class == "III" || z.Class == "IV")).OrderBy(z => z.ClassOrder).ToList();
            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
            //  return Preschool_Li.Select(x => new SelectListItem { Text = x.Preschools, Value = x.Id }).ToList();
        }

        public  List<SelectListItem> GetClass_Previous_Five_XII(long schoolId, Int32 AcademicId)
        {
            var input = _Entities.TbClasses.Where(z => z.IsActive  && z.SchoolId == schoolId && z.AcademicYearId == AcademicId && (z.Class != "I" && z.Class != "II" && z.Class != "III" && z.Class != "IV" && z.Class != "LKG" && z.Class !="UKG" && z.Class !="NURSERY")).OrderBy(z => z.ClassOrder).ToList();
            return input.Select(x => new SelectListItem { Text = x.Class, Value = x.ClassId.ToString() }).ToList();
            //  return Preschool_Li.Select(x => new SelectListItem { Text = x.Preschools, Value = x.Id }).ToList();
        }


    }
}

