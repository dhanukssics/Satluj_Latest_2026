using Microsoft.AspNetCore.Mvc;
using Satluj_Latest.Data;


namespace Satluj_Latest.Controllers
{
    public class DataController : Controller
    {
        private readonly DropdownData _dropdown;
        public DataController(DropdownData dropdown)
        {
            _dropdown = dropdown;
        }

        public object LoadDivision(long id)
        {
       
            var result = _dropdown.GetDivision(id);
            return Json(new { status = result.Count > 0, list = result });
        }

        public object LoadClassesByAcademicYear(string id)
        {
            string[] splitData = id.Split('~');
            long schoolId = Convert.ToInt64(splitData[0]);
            long acYearId = Convert.ToInt64(splitData[1]);

            var result =_dropdown.GetAllClasses(schoolId, acYearId);
            return Json(new { status = result.Count > 0, list = result } );
        }


        public object LoadFreeDivision(long id)
        {
            var result = _dropdown.GetDivision(id);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadFreeSchoolDivision(long id, long schoolid)
        {
            var result = _dropdown.GetFreeSchoolDivision(id, schoolid);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadTeacherDivision(long id, long userid)
        {
            long classId = id;
            long teacherId = userid;
            var result = _dropdown.GetTeacherDivision(id, userid);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadExams(long classId, long schoolId)
        {
            var result = _dropdown.GetAllExams(classId, schoolId);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadSubjects(long examId)
        {
            var result = _dropdown.GetAllSubjects(examId);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadAllSchoolDivision(long id, long schoolid)
        {
            var result = _dropdown.GetAllSchoolDivision(id, schoolid);
            return Json(new { status = result.Count > 0, list = result } );
        }
        //---------------- For get subedger corresponding to the account head 
        public object LoadSubLedgerList(long id)
        {
            var result = _dropdown.GetSubLedgerList(id);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadAllSchoolClasses(long id, long schoolid)
        {
            var result = _dropdown.GetAllSchoolClassesByRegion(id, schoolid);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadAllExams(long id, long schoolid, long classId)
        {
            var result = _dropdown.GetAllExamsDeclared(id, schoolid, classId);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadTermsWithoutExamDeclaration(string id)
        {
            string[] splitData = id.Split('~');
            long ClassId = Convert.ToInt64(splitData[0]);
            long SchoolId = Convert.ToInt64(splitData[1]);
            var result = _dropdown.LoadTermsWithoutExamDeclaration(ClassId, SchoolId);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadAllSubjects(string id)
        {
            string[] splitData = id.Split('~');
            long examId = Convert.ToInt64(splitData[0]);
            long schoolid = Convert.ToInt64(splitData[1]);
            long classId = Convert.ToInt64(splitData[2]);
            var result = _dropdown.GetAllExamsSubjects(examId, schoolid, classId);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadAllScholasticAreas(string id)
        {
            string[] splitData = id.Split('~');
            long schoolid = Convert.ToInt64(splitData[0]);
            long classId = Convert.ToInt64(splitData[1]);
            long examId = Convert.ToInt64(splitData[2]);
            var result = _dropdown.GetAllScholasticAreas(schoolid, classId);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadAllCoScholasticAreas(string id)
        {
            string[] splitData = id.Split('~');
            long schoolid = Convert.ToInt64(splitData[0]);
            long classId = Convert.ToInt64(splitData[1]);
            var result = _dropdown.GetAllCoScholasticAreas(schoolid, classId);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadRoleDetails(string id)
        {
            var result = _dropdown.GetAllCoScholasticAreas(1, 1);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadAllClass(long id, long schoolid)
        {
            var result = _dropdown.GetAllSchoolClassesByRegion(id, schoolid);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadPeriods(long id)
        {
            var result = _dropdown.GetPeriods(id);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadAllSchoolDivisionUserWise(long id, long schoolid, long UserId)
        {
            var result = _dropdown.GetAllSchoolDivisionUserWise(id, schoolid, UserId);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadSubjectsUserWise(long examId, long UserId, long schoolid)
        {
            var result = _dropdown.GetAllSubjectsUserWise(examId, UserId, schoolid);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadAllClassesTeacherWise(long id, long schoolid)
        {
            var result = _dropdown.GetAllClassesTeacherWise(id, schoolid);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadAllDivisionTeacherWise(long id, long schoolid, long TeacherId)
        {
            var result = _dropdown.GetAllDivisionTeacherWise(id, schoolid, TeacherId);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object GetAllSubjectsOfExamInTeacherWise(long ClassId, long SchoolId, long TeacherId, long ExamId, long divisionId)
        {
            var result = _dropdown.GetAllSubjectsOfExamInTeacherWise(ClassId, SchoolId, TeacherId, ExamId, divisionId);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadAllowdedAllSubjects(string id)
        {
            string[] splitData = id.Split('~');
            long examId = Convert.ToInt64(splitData[0]);
            long schoolid = Convert.ToInt64(splitData[1]);
            long classId = Convert.ToInt64(splitData[2]);
            long userId = Convert.ToInt64(splitData[3]);
            long divisionId = Convert.ToInt64(splitData[4]);
            var result = _dropdown.GetAllAllowdedExamsSubjects(examId, schoolid, classId, userId, divisionId);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadAllowdedSubjectsFromPerformance(string id)
        {
            string[] splitData = id.Split('~');
            long examId = Convert.ToInt64(splitData[0]);
            long schoolid = Convert.ToInt64(splitData[1]);
            long classId = Convert.ToInt64(splitData[2]);
            long userId = Convert.ToInt64(splitData[3]);
            long divisionId = Convert.ToInt64(splitData[4]);
            var result = _dropdown.GetAllSubjectsTeacherWise(examId, schoolid, classId, userId, divisionId);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadSpecialSubjects(string id)
        {
            string[] splitData = id.Split('~');
            long Id = Convert.ToInt64(splitData[0]);
            long SchoolId = Convert.ToInt64(splitData[1]);
            var result = _dropdown.GetSpecialSubjects(Id, SchoolId);
            return Json(new { status = result.Count > 0, list = result });
        }
        public object LoadSpecialSubjectsByTeacher(string id)
        {
            string[] splitData = id.Split('~');
            long Id = Convert.ToInt64(splitData[0]);
            long SchoolId = Convert.ToInt64(splitData[1]);
            long UserId = Convert.ToInt64(splitData[2]);
            long DivisionId = Convert.ToInt64(splitData[3]);
            var result = _dropdown.GetSpecialSubjectsByTeacher(Id, SchoolId, UserId, DivisionId);
            return Json(new { status = result.Count > 0, list = result } );
        }
        public object LoadAllScholasticAreasSpecial(string id)
        {
            string[] splitData = id.Split('~');
            long schoolid = Convert.ToInt64(splitData[0]);
            long classId = Convert.ToInt64(splitData[1]);
            long examId = Convert.ToInt64(splitData[2]);
            var result = _dropdown.GetAllScholasticAreasSpecial(schoolid, classId, examId);
            return Json(new { status = result.Count > 0, list = result });
        }

        ////////////
        //////////////////////////////

        public object LoadSpecialAssesments_others(long SchoolId)
        {
            var result = _dropdown.GetVAssesments_others(SchoolId);
            return Json(new { status = result.Count > 0, list = result } );
        }

        public object LoadSpecialDivission_others(string id)
        {
            string[] splitData = id.Split('~');            
            string classType = splitData[0];
            long SchoolId = Convert.ToInt64(splitData[1]);
            var result = _dropdown.GetVClassDivisinLists_Others(SchoolId, classType);
            return Json(new { status = result.Count > 0, list = result });
        }

        public object LoadSpecialSubjects_others(string id)
        {
            string[] splitData = id.Split('~');
            long Id = Convert.ToInt64(splitData[0]);            
            var result = _dropdown.GetSpecialSubjects_others();
            return Json(new { status = result.Count > 0, list = result });
        }

        public object LoadSpecialSubjects_others_CoColastic(string id)
        {
            string[] splitData = id.Split('~');
            long Id = Convert.ToInt64(splitData[0]);
            var result = _dropdown.LoadSpecialSubjects_others_CoColastic();
            return Json(new { status = result.Count > 0, list = result });
        }

        public object Scholasticareas_others(string id)
        {
            string[] splitData = id.Split('~');
            string colPre = splitData[0];
            long SchoolId = Convert.ToInt64(splitData[1]);
            var result = _dropdown.Scholasticareas_others(SchoolId, colPre);
            return Json(new { status = result.Count > 0, list = result } );
        }



        ///////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////...................ONE to FOUR ....................///////////////////////////////

        public object LoadSpecialDivission_ONE_FOUR(string id)
        {
            string[] splitData = id.Split('~');
            string classType = splitData[0];
            long SchoolId = Convert.ToInt64(splitData[1]);
            var result = _dropdown.GetVClassDivisinLists_ONE_FOUR(SchoolId, classType);
            return Json(new { status = result.Count > 0, list = result } );
        }

        public object LoadSpecialAssesments_ONE_FOUR(long SchoolId)
        {
            var result = _dropdown.GetVAssesments_ONE_FOUR(SchoolId);
            return Json(new { status = result.Count > 0, list = result });
        }

        public object LoadSpecialSubjects_ONE_FOUR(string id)
        {
            string[] splitData = id.Split('~');
            long Id = Convert.ToInt64(splitData[0]);
            var result = _dropdown.GetSpecialSubjects_ONE_FOUR();
            return Json(new { status = result.Count > 0, list = result });
        }

        public object Scholasticareas_ONE_FOUR(string id)
        {
            string[] splitData = id.Split('~');
            string colPre = splitData[0];
            long SchoolId = Convert.ToInt64(splitData[1]);
            var result = _dropdown.Scholasticareas_ONE_FOUR(SchoolId, colPre);
            return Json(new { status = result.Count > 0, list = result } );
        }

        public object LoadSpecialSubjects_CoScolastic_ONE_FOUR(string id)
        {
            string[] splitData = id.Split('~');
            long Id = Convert.ToInt64(splitData[0]);
            var result = _dropdown.GetSpecialSubjects_CoScolastic_ONE_FOUR();
            return Json(new { status = result.Count > 0, list = result });
        }


        //18-sep-2020 Jibin Start ..........................................

        //jibin 9/9/2020
        public object Assessment_LoadSubject(long id)
        {
            var result = _dropdown.GetSubject(id);
            return Json(new { status = result.Count > 0, list = result } );
        }

        //jibin 9/9/2020


        //18-sep-2020 Jibin End..........................................

        public object LoadSpecialClass_Previous(string id)
        {
            string[] splitData = id.Split('~');
            Int32 AcademicId = Convert.ToInt32(splitData[0]);
            long SchoolId = Convert.ToInt64(splitData[1]);
            var result = _dropdown.GetClass_Previous_One_Four(SchoolId, AcademicId);
            return Json(new { status = result.Count > 0, list = result });
        }

        public object LoadClass_Previous_Year(string id)
        {
            string[] splitData = id.Split('~');
            Int32 AcademicId = Convert.ToInt32(splitData[0]);
            long SchoolId = Convert.ToInt64(splitData[1]);
            var result = _dropdown.GetClass_Previous_Five_XII(SchoolId, AcademicId);
            return Json(new { status = result.Count > 0, list = result });
        }


    }
}
