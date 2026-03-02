
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using Satluj_Latest.Utility;


namespace Satluj_Latest.Data
{
    public class Student : BaseReference
    {
        private TbStudent student;
        public Student(TbStudent obj) { student = obj; }
        public Student(long id) { student = _Entities.TbStudents.FirstOrDefault(z => z.StudentId == id); }
        public long StudentId { get { return student.StudentId; } }
        public long SchoolId { get { return student.SchoolId; } }
        public string StudentSpecialId { get { return student.StudentSpecialId; } }
        public string StundentName { get { return student.StundentName; } }
        public string ParentEmail { get { return student.ParentEmail; } }
        public string Gender { get { return student.Gender; } }
        public string BloodGroup { get { return student.BloodGroup; } }
        public string ParentName { get { return student.ParentName; } }
        public string Address { get { return student.Address; } }
        public string City { get { return student.City; } }
        public string ContactNumber { get { return student.ContactNumber; } }
        public string ClasssNumber { get { return student.ClasssNumber ?? "0"; } }
        public long ClassId { get { return student.ClassId; } }
        public long DivisionId { get { return student.DivisionId; } }
        public long BusId { get { return student.BusId; } }
        public string TripNo { get { return student.TripNo; } }
        public string BioNumber { get { return student.BioNumber; } }
        public string FilePath { get { return student.FilePath; } }
        public System.DateTime TimeStamp { get { return student.TimeStamp; } }
        public System.Guid? StudentGuid { get { return student.StudentGuid; } }
        public bool IsActive { get { return student.IsActive; } }
        public Nullable<long> ParentId { get { return student.ParentId; } }
        public string State { get { return student.State; } }
        public string MotherName { get { return student.MotherName; } }
        public string ClassName { get { return student.Class.Class; } }
        public Division Division { get { return new Data.Division(student.Division); } }
        public string DivisionName { get { return Division.DivisionName; } }
        public Bus Bus { get { return new Data.Bus(student.Bus); } }
        public string BusSpecialId { get { return student.Bus.BusSpecialId; } }
        public string SchoolName { get { return student.School.SchoolName; } }
        public string SchoolAddress { get { return student.School.Address; } }
        public string Latitude { get { return student.School.Latitude; } }
        public string Longitude { get { return student.School.Longitude; } }
        public int ClassOrder { get { return student.Class.ClassOrder; } }
        //--------------------
        public string MobileNo { get { return student.MobileNo; } }
        public string PlaceOfBirth { get { return student.PlaceOfBirth; } }
        public string MotherTongue { get { return student.MotherTongue; } }
        public Nullable<System.DateTime> DateOfJoining { get { return student.DateOfJoining; } }
        public Nullable<int> NationalityId { get { return student.NationalityId; } }
        public Nullable<int> CountryId { get { return student.CountryId; } }
        public Nullable<int> CategoryId { get { return student.CategoryId; } }
        public string PostalCode { get { return student.PostalCode; } }
        //------------------------------

        public DateTime? DOB { get { return student.Dob; } }
        public bool ReadStatus { get { return NewMessage(student); } }
        public School School { get { return new Data.School(student.School); } }
        public string DOBString()
        {
            string dobString = " ";
            var studentDateB = "";
            if (student.Dob != null)
            {
                dobString = student.Dob.ToString();
                studentDateB = Convert.ToDateTime(dobString).ToShortDateString();
            }
            return studentDateB;
        }
        private bool NewMessage(TbStudent student)
        {
            if (student.ParentId != null)
            {
                var msg = _Entities.TbParentMessages.Where(x => x.SenderId == student.ParentId && x.IsActive && x.ReadStatus == true).Select(x => x.ReadStatus).FirstOrDefault();
                return msg;
            }
            else
                return false;
        }

        public Teacher Teacher { get { return FetchTeacher(); } }
        public Teacher FetchTeacher()
        {
            var row = student.Class.TbTeacherClasses.FirstOrDefault(z => z.DivisionId == DivisionId);
            if (row != null)
            {
                return new Teacher(row.Teacher);
            }
            else
            {
                return null;
            }
        }
        public List<FeeDiscount> Discount { get { return student.TbFeeDiscounts.Where(z => z.IsActive).ToList().Select(z => new FeeDiscount(z)).ToList(); } }
        
        public async Task<List<SPFullFees>> GetStudentPaymentFees(long classId, long studentId)
        {
            var p1 = new SqlParameter("@classId", classId);
            var p2 = new SqlParameter("@studentId", studentId);

            return await _Entities.SPFullFees
                .FromSqlRaw("EXEC SP_FullFee @classId, @studentId", p1, p2)
                .ToListAsync();
        }

        public List<SPAdvanceFee> GetStudentAdvancePaymentFees()
        {
            return _Entities.SPAdvanceFees
                .FromSqlRaw("EXEC SP_AdvanceFee {0}, {1}", student.ClassId, student.StudentId)
                .ToList();
        }
        public Driver Driver
        {
            get
            {
                var currentDateTime = DateTime.UtcNow.Date;

                try
                {
                    var trip = _Entities.TbTrips
                        .Where(z =>
                            z.StartTime >= currentDateTime &&
                            z.StartTime < currentDateTime.AddDays(1) &&
                            z.TravellingStatus == 1 &&
                            z.IsActive &&
                            z.TripNo == student.TripNo &&
                            z.BusId == student.BusId)
                        .Select(z => new Driver(z.DriverId))
                        .FirstOrDefault();

                    return trip;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<SPEditFeeBilling> GetStudentFeeList(long classId, long studentId)
        {
            return _Entities.SPEditFeeBillings
                .FromSqlInterpolated($"EXEC SP_EditFullFee {classId}, {studentId}")
                .ToList();
        }

        public List<SPStudentBillPayment> GetStudentPaidFees()
        {
            var p1 = new SqlParameter("@StudentId", student.StudentId);

            return _Entities.SPStudentBillPayments
                .FromSqlRaw("EXEC SP_StudentBillPayment @studentId", p1)
                .ToList();
        }

        public List<Payment> GetStudentPaidFeesByBillNo(long billNo)
        {
            var g = _Entities.TbPayments.Where(z => z.StudentId == student.StudentId && z.BillNo == billNo).ToList().Select(z => new Payment(z)).ToList();
            return g;
        }
        public Student GetStudentData()
        {
            var data = _Entities.TbStudents.Where(x => x.StudentId == student.StudentId && x.IsActive).ToList().Select(x => new Student(x)).FirstOrDefault();
            return data;
        }
        public List<BillFeeDateHistory> GetHistoryGroupByStudentId()
        {
            long studentId = student.StudentId;

            return _Entities.BillFeeDateHistory
                .FromSqlRaw(@"SELECT StudentId,ClassId,SUM(Amount) AS Amount,CONVERT(date, TimeStamp) AS [Date] FROM tb_Payment WHERE StudentId = @p0 GROUP BY StudentId, CONVERT(date, TimeStamp), ClassId",studentId)
                .ToList();
        }

        public List<billNumberList> GetBillHistory()
        {
            //var s = _Entities.tb_Payment.Where(z => z.StudentId == student.StudentId && z.ClassId == student.ClassId && z.BillNo != null).ToList();
            ////var sp = s.Select(z => z.BillNo).Distinct().ToList().Select(z=> new List<billNumberList>()).ToList();
            //var sp = s.Select(z => z.BillNo).Distinct().Select(z=> new billNumberList()).ToList();
            //return sp;

            //var s = _Entities.tb_Payment.Where(z => z.StudentId == student.StudentId && z.ClassId == student.ClassId && z.BillNo != null).ToList();
            //var sp = s.Select(z => z.BillNo).Distinct().ToList();
            //List<billNumberList> list = new List<billNumberList>();
            //foreach (var item in sp)
            //{
            //    billNumberList one = new billNumberList();
            //    one.BillNo=item.b
            //}
            var s = _Entities.TbPayments.Where(z => z.StudentId == student.StudentId && z.ClassId == student.ClassId && z.BillNo != null && z.IsActive && z.IsPaid).ToList().Select(xz => new Payment(xz)).ToList();
            List<billNumberList> list = new List<billNumberList>();
            foreach (var item in s)
            {
                if (item.BillNo != null)
                {
                    billNumberList one = new billNumberList();
                    one.BillNo = item.BillNo ?? 00;
                    one.TimeStamp = item.TimeStamp;
                    list.Add(one);
                }
            }
            List<billNumberList> newList = new List<billNumberList>();
            newList = list.GroupBy(x => x.BillNo).Select(y => y.First()).ToList();
            return newList;
        }
        public List<Payment> GetHistoryTableByDate(DateTime TimeStamp)
        {
            string Maxdate = TimeStamp.Date.ToString("dd-MM-yyyy") + ' ' + "11:59:00 PM";
            DateTime maxDate = Convert.ToDateTime(Maxdate);
            return _Entities.TbPayments.Where(z => z.TimeStamp >= TimeStamp && z.TimeStamp <= maxDate && z.IsActive == true && z.IsPaid == true).ToList().Select(x => new Payment(x)).ToList();
        }
        public List<Payment> GetHistoryTableByBillNo(long BillNo)
        {
            return _Entities.TbPayments.Where(z => z.IsActive == true && z.IsPaid == true && z.StudentId == student.StudentId && z.BillNo == BillNo).ToList().Select(x => new Payment(x)).ToList();
        }
        public bool IsDueBill(Guid feeGuid)
        {
            return _Entities.TbFeeDues.Where(z => z.IsActive == true && z.FeeDuesGuid == feeGuid).FirstOrDefault() == null ? false : true;
        }
        public List<Student> StudentList(long ClassId, long DivisionId)
        {
            List<Student> listData = new List<Student>();
            listData = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == DivisionId && x.IsActive).ToList().Select(x => new Student(x)).ToList();
            return listData;
        }
        public decimal StudentBalance()
        {
            decimal bal = 0;
            var balance = student.TbStudentBalances.Where(z => z.IsActive == true).FirstOrDefault();
            if (balance != null)
            {
                bal = balance.Amount;
            }
            return bal;
        }
        public TbStudentPaidAmount StudentPaidAmountByBillNo(long BillNo)
        {
            return student.TbStudentPaidAmounts.Where(z => z.BillNo == BillNo).FirstOrDefault();

        }

        //public decimal GetCurrentAmountParial(long feeId)
        //{
        //    decimal partialPayment = 0;

        //}
        public List<AttendanceSummary> GetAcademicPeriods()
        {
            int termIndex = 0;
            var result = new List<AttendanceSummary>();

            var periods = _Entities.TbAcademicPeriods
                .Where(x => x.ClassId == student.ClassId && x.IsActive)
                .OrderBy(x => x.StartDate)
                .ToList();

            foreach (var p in periods)
            {
                var totalCount = _Entities.TbAttendances
                    .Count(x => x.StudentId == student.StudentId
                                && x.IsActive
                                && x.ShiftStatus == 0
                                && x.AttendanceDate >= p.StartDate
                                && x.AttendanceDate <= p.EndDate);

                var presentCount = _Entities.TbAttendances
                    .Count(x => x.StudentId == student.StudentId
                                && x.IsActive
                                && x.ShiftStatus == 0
                                && x.AttendanceData == true
                                && x.AttendanceDate >= p.StartDate
                                && x.AttendanceDate <= p.EndDate);

                result.Add(new AttendanceSummary
                {
                    Term = termIndex,
                    Total = totalCount,
                    Present = presentCount
                });

                termIndex++;
            }

            return result;
        }

        public List<HealthSummay> GetHealthDetails()
        {
            int i = 0;
            var healthData = student.TbHealths.Where(x => x.ClassId == student.ClassId && x.DivisionId == student.DivisionId && x.IsActive == true).OrderBy(x => x.Periods.StartDate).ToList();
           
            List<HealthSummay> _HealthSummay = new List<HealthSummay>();
                foreach (var item in healthData)
                {
                    HealthSummay one = new HealthSummay();
                    one.Term = i;
                    one.Weight = item.Weight;
                    one.Height = item.Height;
                    _HealthSummay.Add(one);
                    i = i + 1;
                }
            
           
            return _HealthSummay;
        }

        public List<HealthSummay> GetHealthDetails_Previous(long ClassId)
        {
            int i = 0;
            var healthData = student.TbHealths.Where(x => x.ClassId == ClassId && x.IsActive == true).OrderBy(x => x.Periods.StartDate).ToList();
            List<HealthSummay> _HealthSummay = new List<HealthSummay>();
            foreach (var item in healthData)
            {
                HealthSummay one = new HealthSummay();
                one.Term = i;
                one.Weight = item.Weight;
                one.Height = item.Height;
                _HealthSummay.Add(one);
                i = i + 1;
            }
            return _HealthSummay;
        }


        public HealthSummay GetHealthDetails_NewFinal(long studentId)
        {
           
            var healthData = student.TbHealths.Where(x => x.StudentId == studentId && x.ClassId == student.ClassId && x.DivisionId == student.DivisionId && x.IsActive == true).OrderByDescending(x=>x.Id).FirstOrDefault();
            
                HealthSummay one = new HealthSummay();

                    if (healthData != null)
                    {
                        one.Weight = healthData.Weight;
                        one.Height = healthData.Height;
                    }

                
               
            
            return one;
        }



        //Code changed by HGayathri Remakrs are not correctly dispayed
        public List<RemarksData> StudentRemarks()
        {
            List<RemarksData> _RemarksData = new List<RemarksData>();
            var remarks = student.TbStudentRemarks.Where(x => x.IsActive).OrderBy(x => x.Exam.TermId).ToList();
            foreach (var item in remarks)
            {
                RemarksData one = new RemarksData();
                one.Term = Convert.ToInt32(item.Exam.TermId);
                var r1 = item.Remark == null ? "" : item.Remark.Trim().ToUpper();
                var r2 = item.AdditionalRemarks == null ? "" : item.AdditionalRemarks.ToUpper().Trim();
                if (r1.Trim() != "" && r1.Trim() != "--Choose--")
                {
                    if (r2.Trim() != "")
                        one.Remark = r1 + ", " + r2;
                    else
                        one.Remark = r1;
                }
                else
                    one.Remark = r2;
                _RemarksData.Add(one);
            }
            return _RemarksData;
        }

        //Code changed by HGayathri Remakrs are not correctly dispayed
        public List<RemarksData> StudentRemarks_Middlewing(long ExamId,long StudentId)
        {
            List<RemarksData> _RemarksData = new List<RemarksData>();
            var remarks = student.TbStudentRemarks.Where(x => x.StudentId== StudentId && x.ExamId==ExamId && x.IsActive).OrderBy(x => x.Exam.TermId).ToList();
            foreach (var item in remarks)
            {
                RemarksData one = new RemarksData();
                one.Term = Convert.ToInt32(item.Exam.TermId);
                var r1 = item.Remark == null ? "" : item.Remark.Trim().ToUpper();
                var r2 = item.AdditionalRemarks == null ? "" : item.AdditionalRemarks.ToUpper().Trim();
                if (r1.Trim() != "" && r1.Trim() != "--Choose--")
                {
                    if (r2.Trim() != "")
                        one.Remark = r1 + ", " + r2;
                    else
                        one.Remark = r1;
                }
                else
                    one.Remark = r2;
                _RemarksData.Add(one);
            }
            return _RemarksData;
        }

        public List<RemarksData> StudentRemarksIX()
        {
            List<RemarksData> _RemarksData = new List<RemarksData>();
            var remarks = student.TbStudentRemarks.Where(x => x.IsActive).OrderBy(x => x.Exam.TermId).ToList();
            int TempInt = 0;
            foreach (var item in remarks)
            {
                RemarksData one = new RemarksData();
                one.Term = Convert.ToInt32(item.Exam.TermId);
                var r1 = item.Remark == null ? "" : item.Remark.Trim().ToUpper();
                var r2 = item.AdditionalRemarks == null ? "" : item.AdditionalRemarks.ToUpper().Trim();
                if (r1.Trim() != "" && r1.Trim() != "--Choose--")
                {
                    if (TempInt == 0)
                    {
                        one.Remark = r1;
                    }
                    else if(TempInt == 1)
                    {
                        one.Remark = r2;
                    }

                    TempInt = TempInt + 1;
                    //if (r2.Trim() != "")
                    //    one.Remark = r1 + ", " + r2;
                    //else
                    //    one.Remark = r1;
                }
                else
                    one.Remark = r2;
                _RemarksData.Add(one);
            }
            return _RemarksData;
        }

        public RemarksData StudentRemarksIX_New(long examid,long studentId)
        {
            List<RemarksData> _RemarksData = new List<RemarksData>();
            //var remarks = student.TbStudentsRemark.Where(x => x.IsActive).OrderBy(x => x.TbExamBooks.TermId).ToList();

            var remarks = student.TbStudentRemarks.Where(x => x.StudentId== studentId && x.ExamId == examid && x.IsActive == true).FirstOrDefault();
            RemarksData one = new RemarksData();
            if (remarks != null)
            {
                
                one.Remark = remarks.Remark;
            }
            
            return one;
        }



        public List<AttendanceSummary> GetStudentsAttendanceData()
        {
            List<AttendanceSummary> _AttendanceSummary = new List<AttendanceSummary>();
            int i = 0;
            var Periods = _Entities.TbAcademicPeriods.Where(x => x.ClassId == student.ClassId && x.IsActive).OrderBy(x => x.StartDate).ToList();
            foreach (var item in Periods)
            {
                AttendanceSummary one = new AttendanceSummary();
                one.Term = i;
                var data = _Entities.TbAttendancePeriods.Where(x => x.StudentId == student.StudentId && x.DivisionId == student.DivisionId && x.IsActive && x.PeriodId == item.Id).FirstOrDefault();
                if (data != null)
                {
                    one.Total = data.TotalDays;
                    one.Present = data.PresentDays;
                }
                else
                {
                    one.Total = 0;
                    one.Present = 0;
                }
                _AttendanceSummary.Add(one);
                i = i + 1;
            }
            return _AttendanceSummary;
        }

        public List<AttendanceSummary> GetStudentsAttendanceData_Previous(long ClassId)
        {
            List<AttendanceSummary> _AttendanceSummary = new List<AttendanceSummary>();
            int i = 0;
            var Periods = _Entities.TbAcademicPeriods.Where(x => x.ClassId == ClassId && x.IsActive).OrderBy(x => x.StartDate).ToList();
            foreach (var item in Periods)
            {
                AttendanceSummary one = new AttendanceSummary();
                one.Term = i;
                var data = _Entities.TbAttendancePeriods.Where(x => x.StudentId == student.StudentId  && x.IsActive && x.PeriodId == item.Id).FirstOrDefault();
                if (data != null)
                {
                    one.Total = data.TotalDays;
                    one.Present = data.PresentDays;
                }
                else
                {
                    one.Total = 0;
                    one.Present = 0;
                }
                _AttendanceSummary.Add(one);
                i = i + 1;
            }
            return _AttendanceSummary;
        }




        public List<StudentOutOfScores> GetAllOutOfData()
        {
            List<StudentOutOfScores> list = new List<StudentOutOfScores>();
            var data = _Entities.TbStudentTotalScores.Where(x => x.StudentId == student.StudentId && x.ClassId == student.ClassId && x.IsActive && x.Exam.IsActive == true).OrderBy(x => x.Exam.TermId).ToList();
            foreach (var item in data)
            {
                StudentOutOfScores one = new StudentOutOfScores();
                one.MarkObtained = item.TotalScore == null ? 0 : item.TotalScore;
                one.Percentage = item.Percentage == null ? 0 : item.Percentage;
                list.Add(one);
            }
            return list;
        }
        public List<V_AggregateScore> GetAggregateScoreData()
        {
            List<V_AggregateScore> list = new List<V_AggregateScore>();
            var assesments = _Entities.TbVAssesments.Where(x => x.IsActive && x.SchoolId == student.SchoolId).ToList();//10042: Punjabi Id avoiding For all Student by Satluj requirement in 09/27/2019
            var reading = student.TbReadingSkills.Where(x => x.IsActive && x.Assesment.IsActive && x.SubjectId!=10042).ToList();
            var writing = student.TbWritingSkills.Where(x => x.IsActive && x.Assesment.IsActive && x.SubjectId != 10042).ToList();
            var spaking = student.TbSpeakingSkills.Where(x => x.IsActive && x.Assesment.IsActive && x.SubjectId != 10042).ToList();
            var listening = student.TbListenningSkills.Where(x => x.IsActive && x.Assesment.IsActive && x.SubjectId != 10042).ToList();
            var maths = student.TbAspectsMathsSkills.Where(x => x.IsActive && x.Assesment.IsActive).ToList();
            var naturalScience = student.TbAspectsNaturalSciencesSkills.Where(x => x.IsActive && x.Assesment.IsActive).ToList();
            var socialStudies = student.TbAspectsSocialStudiesSkills.Where(x => x.IsActive && x.Assesment.IsActive).ToList();
            //var relativeAreas = student.tb_AspectsRelativeAreaSkills.Where(x => x.IsActive && x.tb_V_Assesment.IsActive).ToList();
            //var outOff = _Entities.tb_V_TotalScoreList.Where(x => x.SchoolId == student.SchoolId && x.IsActive && x.EnumTypeId!=9).Sum(x => x.Mark) == 0 ? 1 : _Entities.tb_V_TotalScoreList.Where(x => x.SchoolId == student.SchoolId && x.IsActive && x.EnumTypeId!=9).Sum(x => x.Mark);//Avoid the 9 , Relative Areas Satluj asked us to avoid the relative areas to Aggregate Score
            var languageBalancScore = 100; //  The Language Score have 3 sections, one will directly includes in outOff the balance 200 is this,but avoid Punjabi so its 100 By 27/09/2019 Requirement;
            var outOff = _Entities.TbVTotalScoreLists.Where(x => x.SchoolId == student.SchoolId && x.IsActive && (x.EnumTypeId != 9 &&
            x.EnumTypeId != 0 && x.EnumTypeId != 1 && x.EnumTypeId != 2 && x.EnumTypeId != 3 && x.EnumTypeId != 4 && x.EnumTypeId != 5 && x.EnumTypeId != 8)).Sum(x => x.Mark);

            outOff = outOff + languageBalancScore;
            for (int i = 0; i <= 2; i++)
            {
                V_AggregateScore one = new V_AggregateScore();
                one.AssId = i;
                one.AggregateScore = 0;
                if (assesments != null && assesments.Count > i)
                {
                    var x1 = assesments.Where(x => x.AssesmentId == i).FirstOrDefault();
                    if (x1 != null)
                    {
                        if (reading != null && reading.Count > 0)
                        {
                            var x2 = reading.Where(x => x.AssesmentId == x1.Id).ToList();
                            if (x2 != null && x2.Count>0)
                            {
                                //one.AggregateScore = one.AggregateScore + Convert.ToDecimal(x2.PRONUN == string.Empty ? "0" : x2.PRONUN) + Convert.ToDecimal(x2.FLUENCY == string.Empty ? "0" : x2.FLUENCY);
                                //decimal a = x2.Sum(x => Convert.ToDecimal(x.PRONUN==string.Empty?"0":x.PRONUN));
                                decimal a = x2.Sum(x => Convert.ToDecimal(x.ReadingSkill == string.Empty ? "0" : x.ReadingSkill));
                                //decimal b = x2.Sum(x => Convert.ToDecimal(x.FLUENCY == string.Empty ? "0" : x.FLUENCY));
                                one.AggregateScore = one.AggregateScore + a;
                            }
                        }
                        if (writing != null && writing.Count > 0)
                        {
                            var x3 = writing.Where(x => x.AssesmentId == x1.Id).ToList();
                            if (x3 != null && x3.Count>0)
                            {
                                //one.AggregateScore = one.AggregateScore + Convert.ToDecimal(x3.CREATIVE_WRITING == string.Empty ? "0" : x3.CREATIVE_WRITING) + Convert.ToDecimal(x3.HAND_WRITING == string.Empty ? "0" : x3.HAND_WRITING) + Convert.ToDecimal(x3.GRAMMAR == string.Empty ? "0" : x3.GRAMMAR) +Convert.ToDecimal(x3.SPELLINGS == string.Empty ? "0" : x3.SPELLINGS) + Convert.ToDecimal(x3.VOCABULARY == string.Empty ? "0" : x3.VOCABULARY) + Convert.ToDecimal(x3.UNIT_TEST == string.Empty ? "0" : x3.UNIT_TEST) + Convert.ToDecimal(x3.WORK_SHEET == string.Empty ? "0" : x3.WORK_SHEET);
                                //decimal a = x3.Sum(x => Convert.ToDecimal(x.CREATIVE_WRITING == string.Empty ? "0" : x.CREATIVE_WRITING));
                                //decimal b = x3.Sum(x => Convert.ToDecimal(x.HAND_WRITING == string.Empty ? "0" : x.HAND_WRITING));
                                //decimal c = x3.Sum(x => Convert.ToDecimal(x.GRAMMAR ==string.Empty?"0":x.GRAMMAR));
                                //decimal d = x3.Sum(x => Convert.ToDecimal(x.SPELLINGS == string.Empty ? "0" : x.SPELLINGS));
                                //decimal e = x3.Sum(x => Convert.ToDecimal(x.VOCABULARY == string.Empty ? "0" : x.VOCABULARY));
                                decimal a = x3.Sum(x => Convert.ToDecimal(x.WritingSkill == string.Empty ? "0" : x.WritingSkill));
                                decimal b = x3.Sum(x => Convert.ToDecimal(x.WorkSheet == string.Empty ? "0" : x.WorkSheet));
                                one.AggregateScore = one.AggregateScore + a + b ;
                            }
                            }
                        if (spaking != null && spaking.Count > 0)
                        {
                            var x3 = spaking.Where(x => x.AssesmentId == x1.Id).ToList();
                            if (x3 != null && x3.Count>0)
                            {
                                //one.AggregateScore = one.AggregateScore + Convert.ToDecimal(x3.CONVERSATION == string.Empty ? "0" : x3.CONVERSATION) + Convert.ToDecimal(x3.RECITATION == string.Empty ? "0" : x3.RECITATION);
                                decimal a = x3.Sum(x => Convert.ToDecimal(x.SpeakingSkill == string.Empty ? "0" : x.SpeakingSkill));
                                //decimal b = x3.Sum(x => Convert.ToDecimal(x.RECITATION == string.Empty ? "0" : x.RECITATION));
                                one.AggregateScore = one.AggregateScore + a  ;
                            }
                        }
                        if (listening != null && listening.Count > 0)
                        {
                            var x3 = listening.Where(x => x.AssesmentId == x1.Id).ToList();
                            if (x3 != null && x3.Count>0)
                            {
                                //one.AggregateScore = one.AggregateScore + Convert.ToDecimal(x3.COMPREHENSION == string.Empty ? "0" : x3.COMPREHENSION);
                                decimal a = x3.Sum(x => Convert.ToDecimal(x.ListeningSkill == string.Empty ? "0" : x.ListeningSkill));
                                one.AggregateScore = one.AggregateScore + a;
                            }
                        }
                        if (maths != null && maths.Count > 0)
                        {
                            var x3 = maths.Where(x => x.AssesmentId == x1.Id).ToList();
                            if (x3 != null && x3.Count>0)
                            {
                                //one.AggregateScore = one.AggregateScore + Convert.ToDecimal(x3.CONCEPTS == string.Empty ? "0" : x3.CONCEPTS) + Convert.ToDecimal(x3.ACTIVITY == string.Empty ? "0" : x3.ACTIVITY) + Convert.ToDecimal(x3.TABLES == string.Empty ? "0" : x3.TABLES) + Convert.ToDecimal(x3.MENTAL_ABILITY == string.Empty ? "0" : x3.MENTAL_ABILITY) + Convert.ToDecimal(x3.WRITTEN_WORK == string.Empty ? "0" : x3.WRITTEN_WORK) + Convert.ToDecimal(x3.WORK_SHEET == string.Empty ? "0" : x3.WORK_SHEET);
                                decimal a = x3.Sum(x => Convert.ToDecimal(x.Concepts == string.Empty ? "0" : x.Concepts));
                                //decimal b = x3.Sum(x => Convert.ToDecimal(x.ACTIVITY == string.Empty ? "0" : x.ACTIVITY));
                                //decimal c = x3.Sum(x => Convert.ToDecimal(x.TABLES == string.Empty ? "0" : x.TABLES));
                                //decimal d = x3.Sum(x => Convert.ToDecimal(x.MENTAL_ABILITY == string.Empty ? "0" : x.MENTAL_ABILITY));
                                decimal b = x3.Sum(x => Convert.ToDecimal(x.WrittenWork == string.Empty ? "0" : x.WrittenWork));
                                decimal c = x3.Sum(x => Convert.ToDecimal(x.WorkSheet == string.Empty ? "0" : x.WorkSheet));
                                one.AggregateScore = one.AggregateScore + a + b + c ;
                            }
                        }
                        if (naturalScience != null && naturalScience.Count > 0)
                        {
                            var x3 = naturalScience.Where(x => x.AssesmentId == x1.Id).ToList();
                            if (x3 != null && x3.Count > 0)
                            {
                                //one.AggregateScore = one.AggregateScore + Convert.ToDecimal(x3.ACTIVITY_PROJECT == string.Empty ? "0" : x3.ACTIVITY_PROJECT) + Convert.ToDecimal(x3.GROUP_DISCUSSION == string.Empty ? "0" : x3.GROUP_DISCUSSION) + Convert.ToDecimal(x3.WRITTEN_WORK == string.Empty ? "0" : x3.WRITTEN_WORK) + Convert.ToDecimal(x3.WORK_SHEET == string.Empty ? "0" : x3.WORK_SHEET);
                                decimal a = x3.Sum(x => Convert.ToDecimal(x.ActivityProject == string.Empty ? "0" : x.ActivityProject));
                                //decimal b = x3.Sum(x => Convert.ToDecimal(x.GROUP_DISCUSSION == string.Empty ? "0" : x.GROUP_DISCUSSION));
                                decimal c = x3.Sum(x => Convert.ToDecimal(x.WrittenWork == string.Empty ? "0" : x.WrittenWork));
                                decimal d = x3.Sum(x => Convert.ToDecimal(x.WorkSheet == string.Empty ? "0" : x.WorkSheet));
                                one.AggregateScore = one.AggregateScore + a  + c + d ;
                            }
                        }
                        if (socialStudies != null && socialStudies.Count > 0)
                        {
                            var x3 = socialStudies.Where(x => x.AssesmentId == x1.Id).ToList();
                            if (x3 != null && x3.Count>0)
                            {
                                //one.AggregateScore = one.AggregateScore + Convert.ToDecimal(x3.ACTIVITY_PROJECT == string.Empty ? "0" : x3.ACTIVITY_PROJECT) + Convert.ToDecimal(x3.VIVA == string.Empty ? "0" : x3.VIVA) + Convert.ToDecimal(x3.WRITTEN_WORK == string.Empty ? "0" : x3.WRITTEN_WORK) + Convert.ToDecimal(x3.WORK_SHEET == string.Empty ? "0" : x3.WORK_SHEET);
                                decimal a = x3.Sum(x => Convert.ToDecimal(x.ActivityProject == string.Empty ? "0" : x.ActivityProject));
                                //decimal b = x3.Sum(x => Convert.ToDecimal(x.VIVA == string.Empty ? "0" : x.VIVA));
                                decimal c = x3.Sum(x => Convert.ToDecimal(x.WrittenWork == string.Empty ? "0" : x.WrittenWork));
                                decimal d = x3.Sum(x => Convert.ToDecimal(x.WorkSheet == string.Empty ? "0" : x.WorkSheet));
                                one.AggregateScore = one.AggregateScore + a + c + d;
                            }
                        }
                        //if (relativeAreas != null && relativeAreas.Count > 0) Satluj don't want's to calculate the relative Areas mark for Aggregate in 27/09/2019  
                        //{
                        //    var x3 = relativeAreas.Where(x => x.AssesmentId == x1.Id).ToList();
                        //    if (x3 != null && x3.Count>0)
                        //    {
                        //        //one.AggregateScore = one.AggregateScore + Convert.ToDecimal(x3.COMPUTER_SCIENCE == string.Empty ? "0" : x3.COMPUTER_SCIENCE) + Convert.ToDecimal(x3.GENERAL_STUDIES == string.Empty ? "0" : x3.GENERAL_STUDIES) + Convert.ToDecimal(x3.VALUE_EDUCATION == string.Empty ? "0" : x3.VALUE_EDUCATION) + Convert.ToDecimal(x3.FRENCH == string.Empty ? "0" : x3.FRENCH);
                        //        decimal a = x3.Sum(x => Convert.ToDecimal(x.COMPUTER_SCIENCE == string.Empty ? "0" : x.COMPUTER_SCIENCE));
                        //        decimal b = x3.Sum(x => Convert.ToDecimal(x.GENERAL_STUDIES == string.Empty ? "0" : x.GENERAL_STUDIES));
                        //        decimal c = x3.Sum(x => Convert.ToDecimal(x.VALUE_EDUCATION == string.Empty ? "0" : x.VALUE_EDUCATION));
                        //        decimal d = x3.Sum(x => Convert.ToDecimal(x.FRENCH == string.Empty ? "0" : x.FRENCH));
                        //        one.AggregateScore = one.AggregateScore + a + b + c + d;
                        //    }
                        //}
                    }
                    else
                    {


                    }
                }
                one.Grade = CalculateGrade(one.AggregateScore, outOff);
                list.Add(one);
            }
            return list;
        }

        public string CalculateGrade(decimal score, decimal outoff)
        {
            string grade = "";
            decimal givenMark = Math.Round(((score / outoff) * 100), MidpointRounding.AwayFromZero);

            if (90 <= givenMark && givenMark <= 100)
            {
                grade = "A*";
            }
            else if (75 <= givenMark && givenMark <= 89)
            {
                grade = "A";
            }
            else if (56 <= givenMark && givenMark <= 74)
            {
                grade = "B";
            }
            else if (35 <= givenMark && givenMark <= 55)
            {
                grade = "C";
            }
            else
            {
                if (score == 0)
                {
                    grade = "";
                }
                else
                {
                    grade = "D";
                }
            }
            return grade;
        }
        public int GetCoScholasticNonHeadCount(long RegionId)
        {
            //Order No 10 is the health and physical fitness/educarion , So they needs the extra heading
            int count = 0;
                var data = _Entities.TbCoScholasticAreas.Where(x => x.SchoolId == student.SchoolId && x.RegionId == RegionId && x.IsActive && x.OrderNo != 10  && (x.ClassId==null||x.ClassId==student.ClassId)).ToList();
            if (data != null && data.Count > 0)
                count = data.Count();
            return count;
        }



    }
}
