using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Satluj_Latest.PostModel.TeacherAttendancePostModel;
using static System.Net.Mime.MediaTypeNames;

namespace Satluj_Latest.Data
{
    public class Admin : BaseReference
    {
        public Admin()
        {

        }

        public List<Login> GetSchoolList()
        {
            return _Entities.TbLogins.Where(x => x.IsActive == true && x.RoleId == (int)UserRole.School).ToList().Select(y => new Login(y)).OrderBy(c => c.Name).ToList();
        }

        public List<SP_GetPaymentGatewayList_Result> GetPaymentList(long schoolId,int month , int year, int opr)
        {
            return _Entities.PaymentGatewayListResult
                     .FromSqlRaw("EXEC SP_GetPaymentGatewayList {0}, {1}, {2},{3}",
                                 opr, schoolId, month, year)
                     .ToList();
        }
        public List<SP_GetMonthlyAttendance_Result> GeteMonthlyAttendance(long studentId, int month, int year)
        {
            return _Entities.MonthlyAttendanceResult
                    .FromSqlRaw("EXEC SP_GetMonthlyAttendance {0}, {1}, {2}",
                                  studentId, month, year)
                    .ToList();
           
        }

        public List<Division> GetAllAppDivision(long schoolId)
        {

            var xyyy = _Entities.TbBiometricDivisions.Where(x => x.SchoolId == schoolId).ToList();
            return xyyy.Select(y => new Division(y.Division.DivisionId)).ToList();



        }

        public List<SpMessageForParent> ParentTeacherConversation(string studentId, int start, int length)
        {
            long StudentId = Convert.ToInt64(studentId);
            //var data = _Entities.parentteacherResult(StudentId, start, length).ToList().Select(y => new SpMessageForParent(y)).ToList();
            var data = _Entities.ParentTeacherConversationFull
      .FromSqlRaw("EXEC sp_ParentTeacherConversationFull {0}, {1}, {2}", StudentId, start, length)
      .AsEnumerable()
      .Select(y => new SpMessageForParent(y)).ToList();

            return data;
        }

        public List<SpSmsPackage> GetSmsPackageBySchoolId(long schoolId)
        {

            //return _Entities.SP_GetSmsPackage(schoolId).ToList().Select(y => new SpSmsPackage(y)).ToList();
         var result= _Entities.GetSpSmsPackages
            .FromSqlRaw("EXEC SP_GetSmsPackage {0}",
                        schoolId)
            .ToList();
            return result;

        }

        //jibin 9/17/2020

        public List<Sp_GetAssessmentsByIDandDatesDiv_Result> GetAssessmentMonthlyByDiv(int ClassId, int month, int year, int divisionid)
        {
          var result =  _Entities.GetAssessmentsByIDandDatesDiv_Result
        .FromSqlRaw("EXEC Sp_GetAssessmentsByIDandDatesDiv {0}, {1}, {2},{3}",
                    ClassId, month, year, divisionid)
        .ToList();

            //return _Entities.Sp_GetAssessmentsByIDandDatesDiv(ClassId, month, year, divisionid).ToList();
            return result;
        }

        public List<Sp_GetAssessmentsByIDandDateMonth_Result> GetAssessmentMonthlyandYear(int ClassId, int month, int year)
        {
            var result = _Entities.GetAssessmentsByIDandDateMonth_Result
        .FromSqlRaw("EXEC Sp_GetAssessmentsByIDandDateMonth {0}, {1}, {2}",
                    ClassId, month, year)
        .ToList();
            return result;
        }



        //jibin 9/17/2020
        public List<Sp_GetAllSmsCount> GetAllSmsCount(DateTime start, DateTime end)
        {
            var startStr = start.ToString("yyyy-MM-dd");
            var endStr = end.ToString("yyyy-MM-dd");

            var results = _Entities.GetAllSmsCounts
                .FromSqlRaw("EXEC SP_GetAllSmsOnTwoDates {0}, {1}, {2}",
                            startStr, endStr, 0)
                .ToList();

            // Load school list for mapping
            var schools = _Entities.TbLogins
                .Where(s => s.RoleId == (int)UserRole.School)
                .ToList();

            foreach (var r in results)
            {
                var school = schools.FirstOrDefault(s => s.SchoolId == r.ScholId);

                if (school != null)
                {
                    r.SchoolName = school.Name;
                    r.Address = school.School.Address;
                }

                // SMS count per school
                r.Count = results.Count(x => x.ScholId == r.ScholId);
            }

            return results
                .GroupBy(x => x.ScholId)
                .Select(g => g.First())
                .ToList();
        }

    }
}
