using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using Satluj_Latest.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web.WebPages.Html;

namespace Satluj_Latest.Controllers
{
    public class SpecialClassController : BaseController
    {
        private readonly DropdownData _dropdown;
        public SpecialClassController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
        }

        public IActionResult AssesmentHome()
        {
            SpecialClassModel model = new SpecialClassModel();
            model.SchoolId = _user.SchoolId;
            var dropdown = new DropdownData();
            ViewBag.PeriodList = dropdown.GetPeriodsLists(model.SchoolId);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAssesment(SpecialClassModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, msg = "Invalid data" });

            bool status = false;
            string msg = "Failed";

            
            bool existsPeriod = await _Entities.TbVAssesments
                .AnyAsync(x => x.SchoolId == _user.SchoolId && x.PeriodId == model.PeriodId && x.IsActive);

            if (existsPeriod)
            {
                msg = "Already exists this period";
                return Json(new { status, msg });
            }

            bool existsAsses = await _Entities.TbVAssesments
                .AnyAsync(x => x.SchoolId == _user.SchoolId && x.AssesmentId == (int)model.AssesmentId && x.IsActive);

            if (existsAsses)
            {
                msg = "Already exists this Assesments";
                return Json(new { status, msg });
            }

            var newData = new TbVAssesment
            {
                SchoolId = _user.SchoolId,
                PeriodId = model.PeriodId,
                AssesmentId = (int)model.AssesmentId,
                IsActive = true,
                TimeStamp = CurrentTime
            };

            _Entities.TbVAssesments.Add(newData);
            status = (await _Entities.SaveChangesAsync()) > 0;
            msg = status ? "Successful" : "Failed";

            return Json(new { status, msg });
        }

        [HttpGet]
        public IActionResult AssesmentListPartialView()
        {
            var model = new SpecialClassModel
            {
                SchoolId = _user.SchoolId
            };

            return PartialView("~/Views/SpecialClass/_pv_AssesmentList.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAssesments(string id)
        {
            if (!long.TryParse(id, out long assId))
                return Json(new { status = false, msg = "Invalid ID" });

            var oldData = await _Entities.TbVAssesments
                                        .FirstOrDefaultAsync(x => x.Id == assId && x.IsActive);

            if (oldData == null)
            {
                return Json(new { status = false, msg = "No such Assessment!" });
            }

            oldData.IsActive = false;

            bool status = (await _Entities.SaveChangesAsync()) > 0;

            return Json(new { status = status, msg = status ? "Successful" : "Failed" });
        }

        public IActionResult ReadingSkillsHome()
        {
            var model = new SubjectScoreModel
            {
                SchoolId = _user.SchoolId,
                UserId = _user.UserId,
                IsAdminCheck = HttpContext.Session.GetInt32("isAdmin") ?? 0
            };
            ViewBag.Vassessments = _dropdown.GetVAssesments(model.SchoolId);
            ViewBag.VClassDivisinLists = _dropdown.GetVClassDivisinLists(model.SchoolId);
            ViewBag.Vdivisionlistuser = _dropdown.GetVClassDivisinListsByUser(model.SchoolId, model.UserId);
            return View(model);
        }

        public PartialViewResult LoadAllStudentsWithSkillsMarks(string id)
        {
            string[] data = id.Split('~');
            long divisionId = Convert.ToInt64(data[0]);
            long subjectId = Convert.ToInt64(data[1]);
            long skillEnum = Convert.ToInt32(data[2]);
            long AssesmentId = Convert.ToInt64(data[3]);
            StudentSkillMarks model = new StudentSkillMarks();
            model.SchoolId = _user.SchoolId;
            model.DivisionId = divisionId;
            model.AssesmentId = AssesmentId;
            model.SubjectId = subjectId;
            model._SkillModel = new List<SkillModel>();
            var studentList = _Entities.TbStudents.Where(x => x.DivisionId == divisionId && x.SchoolId == _user.SchoolId && x.IsActive).OrderBy(x => x.StundentName).ToList();
            if (skillEnum == 10) //READING SKILLS
            {
                #region Read
                var mark = _Entities.TbReadingSkills.Where(x => x.AssesmentId == AssesmentId && x.SubjectId == subjectId && x.DivisionId == divisionId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)// Have data
                {
                    if (mark.Count == studentList.Count)// Have full students marks
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._ReadingSkill = new ReadingSkill();
                            //Commented by :gayathri (03/08/2023)--Vmarksheet changed based on new requirement
                            //one._ReadingSkill.Pronun = item.PRONUN;
                            //one._ReadingSkill.Fluency = item.FLUENCY;
                            one._ReadingSkill.Pronun = item.ReadingSkill;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._ReadingSkill = new ReadingSkill();
                            //one._ReadingSkill.Pronun = item.PRONUN;
                            //one._ReadingSkill.Fluency = item.FLUENCY;
                            one._ReadingSkill.Pronun = item.ReadingSkill;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        //var balanceStudents = _Entities.tb_Student.Where(x => !mark.Any(y => y.StudentId != x.StudentId) && x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.IsActive).ToList();
                        var balanceStudents = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._ReadingSkill = new ReadingSkill();
                            //one._ReadingSkill.Pronun = "";
                            //one._ReadingSkill.Fluency = "";
                            one._ReadingSkill.Pronun = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else// New entry 
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._ReadingSkill = new ReadingSkill();
                        //one._ReadingSkill.Pronun = "";
                        //one._ReadingSkill.Fluency = "";
                        one._ReadingSkill.Pronun = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentReadingSkillEntry.cshtml", model);
                #endregion
            }
            else if (skillEnum == 12)//WRITING SKILLS
            {
                #region Writing Skills
                var marks = _Entities.TbWritingSkills.Where(x => x.DivisionId == divisionId && x.SchoolId == _user.SchoolId && x.AssesmentId == AssesmentId && x.SubjectId == subjectId && x.IsActive).ToList();
                if (marks != null && marks.Count > 0)
                {
                    if (marks.Count == studentList.Count)
                    {
                        foreach (var item in marks)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._WritingSkill = new WritingSkill();
                            //one._WritingSkill.CreativeThinking = item.CREATIVE_WRITING;
                            //one._WritingSkill.HandWriting = item.HAND_WRITING;
                            //one._WritingSkill.Grammer = item.GRAMMAR;
                            //one._WritingSkill.Spelling = item.SPELLINGS;
                            //one._WritingSkill.Vocabulary = item.VOCABULARY;
                            //one._WritingSkill.UnitTest = item.UNIT_TEST;
                            one._WritingSkill.UnitTest = item.WritingSkill;
                            one._WritingSkill.WorkSheet = item.WorkSheet;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in marks)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._WritingSkill = new WritingSkill();
                            //one._WritingSkill.CreativeThinking = item.CREATIVE_WRITING;
                            //one._WritingSkill.HandWriting = item.HAND_WRITING;
                            //one._WritingSkill.Grammer = item.GRAMMAR;
                            //one._WritingSkill.Spelling = item.SPELLINGS;
                            //one._WritingSkill.Vocabulary = item.VOCABULARY;
                            //one._WritingSkill.UnitTest = item.UNIT_TEST;
                            one._WritingSkill.UnitTest = item.WritingSkill;
                            one._WritingSkill.WorkSheet = item.WorkSheet;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        //var balanceStudents = _Entities.tb_Student.Where(x => !marks.Any(y => y.StudentId != x.StudentId) && x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.IsActive).ToList();
                        var balanceStudents = studentList.Where(x => !marks.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._WritingSkill = new WritingSkill();
                            //one._WritingSkill.CreativeThinking = "";
                            //one._WritingSkill.HandWriting = "";
                            //one._WritingSkill.Grammer = "";
                            //one._WritingSkill.Spelling = "";
                            //one._WritingSkill.Vocabulary = "";
                            //one._WritingSkill.UnitTest = "";
                            one._WritingSkill.WorkSheet = "";
                            one._WritingSkill.UnitTest = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._WritingSkill = new WritingSkill();
                        //one._WritingSkill.CreativeThinking = "";
                        //one._WritingSkill.HandWriting = "";
                        //one._WritingSkill.Grammer = "";
                        //one._WritingSkill.Spelling = "";
                        //one._WritingSkill.Vocabulary = "";
                        //one._WritingSkill.UnitTest = "";
                        one._WritingSkill.WorkSheet = "";
                        one._WritingSkill.UnitTest = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentWritingSkillEntry.cshtml", model);
                #endregion
            }
            else if (skillEnum == 11)//SPEAKING SKILLS
            {
                #region Speaking
                var mark = _Entities.TbSpeakingSkills.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.AssesmentId == AssesmentId && x.SubjectId == subjectId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._SpeakingSkill = new SpeakingSkill();
                            //one._SpeakingSkill.Conversation = item.CONVERSATION;
                            //one._SpeakingSkill.Recitation = item.RECITATION;
                            one._SpeakingSkill.Conversation = item.SpeakingSkill;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._SpeakingSkill = new SpeakingSkill();
                            //one._SpeakingSkill.Conversation = item.CONVERSATION;
                            //one._SpeakingSkill.Recitation = item.RECITATION;
                            one._SpeakingSkill.Conversation = item.SpeakingSkill;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        //var balanceStudents = _Entities.tb_Student.Where(x => !mark.Any(y => y.StudentId != x.StudentId) && x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.IsActive).ToList();
                        var balanceStudents = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._SpeakingSkill = new SpeakingSkill();
                            //one._SpeakingSkill.Conversation = "";
                            //one._SpeakingSkill.Recitation = "";
                            one._SpeakingSkill.Conversation = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._SpeakingSkill = new SpeakingSkill();
                        //one._SpeakingSkill.Conversation = "";
                        //one._SpeakingSkill.Recitation = "";
                        one._SpeakingSkill.Conversation = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentSpeakingSkillEntry.cshtml", model);
                #endregion
            }
            else if (skillEnum == 13)//LISTENING SKILLS
            {
                #region Listening
                var mark = _Entities.TbListenningSkills.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.SubjectId == subjectId && x.AssesmentId == AssesmentId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._ListenningSkill = new ListenningSkill();
                            //one._ListenningSkill.Comprehension = item.COMPREHENSION;
                            one._ListenningSkill.Comprehension = item.ListeningSkill;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._ListenningSkill = new ListenningSkill();
                            //one._ListenningSkill.Comprehension = item.COMPREHENSION;
                            one._ListenningSkill.Comprehension = item.ListeningSkill;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        //var balanceStudents = _Entities.tb_Student.Where(x => !mark.Any(y => y.StudentId != x.StudentId) && x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.IsActive).ToList();
                        var balanceStudents = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._ListenningSkill = new ListenningSkill();
                            //one._ListenningSkill.Comprehension = "";
                            one._ListenningSkill.Comprehension = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._ListenningSkill = new ListenningSkill();
                        //one._ListenningSkill.Comprehension = "";
                        one._ListenningSkill.Comprehension = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentListeningSkillEntry.cshtml", model);
                #endregion
            }
            else if (skillEnum == 15)//MATHEMATICS ASPECTS
            {
                #region Mathematics Aspects
                var mark = _Entities.TbAspectsMathsSkills.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.SubjectId == subjectId && x.AssesmentId == AssesmentId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._AspectsMaths = new AspectsMaths();
                            one._AspectsMaths.Concepts = item.Concepts;
                            //one._AspectsMaths.Activity = item.ACTIVITY;
                            //one._AspectsMaths.Tables = item.TABLES;
                            //one._AspectsMaths.MentalAbility = item.MENTAL_ABILITY;
                            one._AspectsMaths.WrittenWork = item.WrittenWork;
                            one._AspectsMaths.WorkSheet = item.WorkSheet;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._AspectsMaths = new AspectsMaths();
                            one._AspectsMaths.Concepts = item.Concepts;
                            //one._AspectsMaths.Activity = item.ACTIVITY;
                            //one._AspectsMaths.Tables = item.TABLES;
                            //one._AspectsMaths.MentalAbility = item.MENTAL_ABILITY;
                            one._AspectsMaths.WrittenWork = item.WrittenWork;
                            one._AspectsMaths.WorkSheet = item.WorkSheet;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        //var balanceStudents = _Entities.tb_Student.Where(x => !mark.Any(y => y.StudentId != x.StudentId) && x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.IsActive).ToList();
                        var balanceStudents = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._AspectsMaths = new AspectsMaths();
                            one._AspectsMaths.Concepts = "";
                            //one._AspectsMaths.Activity = "";
                            //one._AspectsMaths.Tables = "";
                            //one._AspectsMaths.MentalAbility = "";
                            one._AspectsMaths.WrittenWork = "";
                            one._AspectsMaths.WorkSheet = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._AspectsMaths = new AspectsMaths();
                        one._AspectsMaths.Concepts = "";
                        //one._AspectsMaths.Activity = "";
                        //one._AspectsMaths.Tables = "";
                        //one._AspectsMaths.MentalAbility = "";
                        one._AspectsMaths.WrittenWork = "";
                        one._AspectsMaths.WorkSheet = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentAspectsMathsSkillEntry.cshtml", model);
                #endregion
            }
            else if (skillEnum == 16)//NATURAL SCIENCE ASPECTS
            {
                #region Natural Science 
                var mark = _Entities.TbAspectsNaturalSciencesSkills.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.SubjectId == subjectId && x.AssesmentId == AssesmentId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._AspectsNaturalScience = new AspectsNaturalScience();
                            one._AspectsNaturalScience.ActivityProject = item.ActivityProject;
                            //one._AspectsNaturalScience.GroupDiscussion = item.GROUP_DISCUSSION;
                            one._AspectsNaturalScience.WrittenWork = item.WrittenWork;
                            one._AspectsNaturalScience.WorkSheet = item.WorkSheet;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._AspectsNaturalScience = new AspectsNaturalScience();
                            one._AspectsNaturalScience.ActivityProject = item.ActivityProject;
                            //one._AspectsNaturalScience.GroupDiscussion = item.GROUP_DISCUSSION;
                            one._AspectsNaturalScience.WrittenWork = item.WrittenWork;
                            one._AspectsNaturalScience.WorkSheet = item.WorkSheet;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        //var balanceStudents = _Entities.tb_Student.Where(x => !mark.Any(y => y.StudentId != x.StudentId) && x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.IsActive).ToList();
                        var balanceStudents = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._AspectsNaturalScience = new AspectsNaturalScience();
                            one._AspectsNaturalScience.ActivityProject = "";
                            //one._AspectsNaturalScience.GroupDiscussion = "";
                            one._AspectsNaturalScience.WrittenWork = "";
                            one._AspectsNaturalScience.WorkSheet = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._AspectsNaturalScience = new AspectsNaturalScience();
                        one._AspectsNaturalScience.ActivityProject = "";
                        //one._AspectsNaturalScience.GroupDiscussion = "";
                        one._AspectsNaturalScience.WrittenWork = "";
                        one._AspectsNaturalScience.WorkSheet = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentAspectNaturalScienceSkillEntry.cshtml", model);
                #endregion
            }
            else if (skillEnum == 6)//ART / CRAFT
            {
                #region Art / Craft
                var mark = _Entities.TbArtCraftSkills.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.AssesmentId == AssesmentId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._ArtCraft = new ArtCraft();
                            one._ArtCraft.Interest = item.Interest;
                            one._ArtCraft.Creativity = item.Creativity;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._ArtCraft = new ArtCraft();
                            one._ArtCraft.Interest = item.Interest;
                            one._ArtCraft.Creativity = item.Creativity;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        //var balanceStudents = _Entities.tb_Student.Where(x => !mark.Any(y => y.StudentId != x.StudentId) && x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.IsActive).ToList();
                        var balanceStudents = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._ArtCraft = new ArtCraft();
                            one._ArtCraft.Interest = "";
                            one._ArtCraft.Creativity = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._ArtCraft = new ArtCraft();
                        one._ArtCraft.Interest = "";
                        one._ArtCraft.Creativity = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentArtCraftSkillEntry.cshtml", model);
                #endregion
            }
            else if (skillEnum == 7)//MUSIC / DANCE
            {
                #region Music / Dance
                var mark = _Entities.TbMusicDanceSkills.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.SubjectId == subjectId && x.AssesmentId == AssesmentId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._MusicDance = new MusicDance();
                            one._MusicDance.Interest = item.Interest;
                            one._MusicDance.Rhythm = item.Rhythm;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._MusicDance = new MusicDance();
                            one._MusicDance.Interest = item.Interest;
                            one._MusicDance.Rhythm = item.Rhythm;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        //var balanceStudents = _Entities.tb_Student.Where(x => !mark.Any(y => y.StudentId != x.StudentId) && x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.IsActive).ToList();
                        var balanceStudents = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._MusicDance = new MusicDance();
                            one._MusicDance.Interest = "";
                            one._MusicDance.Rhythm = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._MusicDance = new MusicDance();
                        one._MusicDance.Interest = "";
                        one._MusicDance.Rhythm = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentMusicDanceSkillEntry.cshtml", model);
                #endregion
            }
            else if (skillEnum == 17)// SOCIAL STUDIES ASPECTS
            {
                #region SOCIAL STUDIES ASPECTS
                var mark = _Entities.TbAspectsSocialStudiesSkills.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.SubjectId == subjectId && x.AssesmentId == AssesmentId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._AspectsSocialStudies = new AspectsSocialStudies();
                            one._AspectsSocialStudies.ActivityProject = item.ActivityProject;
                            //one._AspectsSocialStudies.Viva = item.VIVA;
                            one._AspectsSocialStudies.WrittenWork = item.WrittenWork;
                            one._AspectsSocialStudies.WorkSheet = item.WorkSheet;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._AspectsSocialStudies = new AspectsSocialStudies();
                            one._AspectsSocialStudies.ActivityProject = item.ActivityProject;
                            //one._AspectsSocialStudies.Viva = item.VIVA;
                            one._AspectsSocialStudies.WrittenWork = item.WrittenWork;
                            one._AspectsSocialStudies.WorkSheet = item.WorkSheet;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        var balanceStudents = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._AspectsSocialStudies = new AspectsSocialStudies();
                            one._AspectsSocialStudies.ActivityProject = "";
                            //one._AspectsSocialStudies.Viva = "";
                            one._AspectsSocialStudies.WrittenWork = "";
                            one._AspectsSocialStudies.WorkSheet = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._AspectsSocialStudies = new AspectsSocialStudies();
                        one._AspectsSocialStudies.ActivityProject = "";
                        //one._AspectsSocialStudies.Viva = "";
                        one._AspectsSocialStudies.WrittenWork = "";
                        one._AspectsSocialStudies.WorkSheet = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentAspectSocialStudiesSkillEntry.cshtml", model);
                #endregion
            }
            else if (skillEnum == 9)// RELATIVE AREAS ASPECTS
            {
                #region RELATIVE AREAS ASPECTS
                var mark = _Entities.TbAspectsRelativeAreaSkills.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == divisionId && x.SubjectId == subjectId && x.AssesmentId == AssesmentId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._AspectsRelativeAreas = new AspectsRelativeAreas();
                            one._AspectsRelativeAreas.ComputerScience = item.ComputerScience;
                            one._AspectsRelativeAreas.GeneralStudies = item.GeneralStudies;
                            one._AspectsRelativeAreas.Valueeducation = item.ValueEducation;
                            one._AspectsRelativeAreas.French = item.French;
                            one._AspectsRelativeAreas.SmartClass = item.SmartClass;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._AspectsRelativeAreas = new AspectsRelativeAreas();
                            one._AspectsRelativeAreas.ComputerScience = item.ComputerScience;
                            one._AspectsRelativeAreas.GeneralStudies = item.GeneralStudies;
                            one._AspectsRelativeAreas.Valueeducation = item.ValueEducation;
                            one._AspectsRelativeAreas.French = item.French;
                            one._AspectsRelativeAreas.SmartClass = item.SmartClass;
                            one.RolleNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        var balanceStudents = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balanceStudents)
                        {
                            SkillModel one = new SkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._AspectsRelativeAreas = new AspectsRelativeAreas();
                            one._AspectsRelativeAreas.ComputerScience = "";
                            one._AspectsRelativeAreas.GeneralStudies = "";
                            one._AspectsRelativeAreas.Valueeducation = "";
                            one._AspectsRelativeAreas.French = "";
                            one._AspectsRelativeAreas.SmartClass = "";
                            one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        SkillModel one = new SkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._AspectsRelativeAreas = new AspectsRelativeAreas();
                        one._AspectsRelativeAreas.ComputerScience = "";
                        one._AspectsRelativeAreas.GeneralStudies = "";
                        one._AspectsRelativeAreas.Valueeducation = "";
                        one._AspectsRelativeAreas.French = "";
                        one._AspectsRelativeAreas.SmartClass = "";
                        one.RolleNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RolleNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentAspectRelativeStudiesSkillEntry.cshtml", model);
                #endregion
            }
            return PartialView("~/Views/SpecialClass/_pv_StudentReadingSkillEntry.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentReadingSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var newRecords = new List<TbReadingSkill>();

            foreach (var item in model._SkillModel)
            {
                var existing = await _Entities.TbReadingSkills
                    .FirstOrDefaultAsync(x =>
                        x.StudentId == item.StudentId &&
                        x.DivisionId == model.DivisionId &&
                        x.SubjectId == model.SubjectId &&
                        x.AssesmentId == model.AssesmentId &&
                        x.IsActive);

                
                if (existing == null)
                {
                    if (!string.IsNullOrEmpty(item._ReadingSkill?.Pronun) ||
                        !string.IsNullOrEmpty(item._ReadingSkill?.Fluency))
                    {
                        var newSkill = new TbReadingSkill
                        {
                            AssesmentId = model.AssesmentId,
                            DivisionId = model.DivisionId,
                            SubjectId = model.SubjectId,
                            StudentId = item.StudentId,
                            SchoolId = model.SchoolId,
                            ReadingSkill = item._ReadingSkill.Pronun ?? "",
                            IsActive = true,
                            TimeStamp = CurrentTime,
                            UserId = _user.UserId
                        };

                        newRecords.Add(newSkill);
                    }
                }
                else
                {
                    
                    existing.ReadingSkill = item._ReadingSkill.Pronun ?? "";
                }
            }

           
            if (newRecords.Count > 0)
                await _Entities.TbReadingSkills.AddRangeAsync(newRecords);

          
            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentWritingSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var newRecords = new List<TbWritingSkill>();

            foreach (var item in model._SkillModel)
            {
                var existing = await _Entities.TbWritingSkills
                    .FirstOrDefaultAsync(x =>
                        x.StudentId == item.StudentId &&
                        x.DivisionId == model.DivisionId &&
                        x.SubjectId == model.SubjectId &&
                        x.AssesmentId == model.AssesmentId &&
                        x.IsActive);

                
                if (existing == null)
                {
                    bool hasAnyValue =
                        !string.IsNullOrEmpty(item._WritingSkill?.CreativeThinking) ||
                        !string.IsNullOrEmpty(item._WritingSkill?.HandWriting) ||
                        !string.IsNullOrEmpty(item._WritingSkill?.Grammer) ||
                        !string.IsNullOrEmpty(item._WritingSkill?.Spelling) ||
                        !string.IsNullOrEmpty(item._WritingSkill?.Vocabulary) ||
                        !string.IsNullOrEmpty(item._WritingSkill?.UnitTest) ||
                        !string.IsNullOrEmpty(item._WritingSkill?.WorkSheet);

                    if (hasAnyValue)
                    {
                        var newSkill = new TbWritingSkill
                        {
                            AssesmentId = model.AssesmentId,
                            DivisionId = model.DivisionId,
                            SubjectId = model.SubjectId,
                            StudentId = item.StudentId,
                            SchoolId = model.SchoolId,
                            IsActive = true,
                            TimeStamp = CurrentTime,
                            UserId = _user.UserId,
                            WritingSkill = item._WritingSkill?.UnitTest,
                            WorkSheet = item._WritingSkill?.WorkSheet ?? ""
                        };

                        newRecords.Add(newSkill);
                    }
                }
                else
                {
                   
                    existing.WritingSkill = item._WritingSkill?.UnitTest;
                    existing.WorkSheet = item._WritingSkill?.WorkSheet ?? "";
                }
            }

            
            if (newRecords.Count > 0)
                await _Entities.TbWritingSkills.AddRangeAsync(newRecords);

       
            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentSpeakingSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            
            var existingList = await _Entities.TbSpeakingSkills
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.SubjectId == model.SubjectId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            
            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbSpeakingSkill>();

            foreach (var item in model._SkillModel)
            {
                var skillDto = item._SpeakingSkill;
                bool hasAnyValue = !string.IsNullOrEmpty(skillDto?.Conversation) ||
                                   !string.IsNullOrEmpty(skillDto?.Recitation);

                if (!hasAnyValue)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    
                    existing.SpeakingSkill = skillDto?.Conversation ?? "";
                    
                }
                else
                {
                  
                    var add = new TbSpeakingSkill
                    {
                        AssesmentId = model.AssesmentId,
                        DivisionId = model.DivisionId,
                        SubjectId = model.SubjectId,
                        StudentId = item.StudentId,
                        SchoolId = model.SchoolId,
                        SpeakingSkill = skillDto?.Conversation ?? "",
                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

            if (newRecords.Count > 0)
                await _Entities.TbSpeakingSkills.AddRangeAsync(newRecords);

            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }


        [HttpPost]
        public async Task<IActionResult> SubmitStudentListeningSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var existingList = await _Entities.TbListenningSkills
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.SubjectId == model.SubjectId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbListenningSkill>();

            foreach (var item in model._SkillModel)
            {
                string comprehension = item._ListenningSkill?.Comprehension;

                if (string.IsNullOrEmpty(comprehension))
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    
                    existing.ListeningSkill = comprehension;
                }
                else
                {
                  
                    var add = new TbListenningSkill
                    {
                        AssesmentId = model.AssesmentId,
                        DivisionId = model.DivisionId,
                        SubjectId = model.SubjectId,
                        StudentId = item.StudentId,
                        SchoolId = model.SchoolId,
                        ListeningSkill = comprehension,
                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

           
            if (newRecords.Count > 0)
                await _Entities.TbListenningSkills.AddRangeAsync(newRecords);

            
            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentAspectsMathsSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            // Load all existing records for this assessment in ONE query
            var existingList = await _Entities.TbAspectsMathsSkills
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.SubjectId == model.SubjectId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            // Fast lookup dictionary
            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbAspectsMathsSkill>();

            foreach (var item in model._SkillModel)
            {
                var skill = item._AspectsMaths;

                bool hasValues =
                    !string.IsNullOrEmpty(skill?.Concepts) ||
                    !string.IsNullOrEmpty(skill?.WrittenWork) ||
                    !string.IsNullOrEmpty(skill?.WorkSheet);

            
                if (!hasValues)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                  
                    existing.Concepts = skill?.Concepts ?? "";
                    existing.WrittenWork = skill?.WrittenWork ?? "";
                    existing.WorkSheet = skill?.WorkSheet ?? "";
                }
                else
                {
                
                    var add = new TbAspectsMathsSkill
                    {
                        AssesmentId = model.AssesmentId,
                        DivisionId = model.DivisionId,
                        SubjectId = model.SubjectId,
                        StudentId = item.StudentId,
                        SchoolId = model.SchoolId,
                        Concepts = skill?.Concepts ?? "",
                        WrittenWork = skill?.WrittenWork ?? "",
                        WorkSheet = skill?.WorkSheet ?? "",
                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

        
            if (newRecords.Count > 0)
                await _Entities.TbAspectsMathsSkills.AddRangeAsync(newRecords);

       
            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentAspectsNaturalScienceSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            
            var existingList = await _Entities.TbAspectsNaturalSciencesSkills
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.SubjectId == model.SubjectId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbAspectsNaturalSciencesSkill>();

            foreach (var item in model._SkillModel)
            {
                var skill = item._AspectsNaturalScience;

                bool hasValues =
                    !string.IsNullOrEmpty(skill?.ActivityProject) ||
                    !string.IsNullOrEmpty(skill?.WrittenWork) ||
                    !string.IsNullOrEmpty(skill?.WorkSheet);

                if (!hasValues)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    
                    existing.ActivityProject = skill?.ActivityProject ?? "";
                    existing.WrittenWork = skill?.WrittenWork ?? "";
                    existing.WorkSheet = skill?.WorkSheet ?? "";
                }
                else
                {
                    var add = new TbAspectsNaturalSciencesSkill
                    {
                        AssesmentId = model.AssesmentId,
                        DivisionId = model.DivisionId,
                        SubjectId = model.SubjectId,
                        StudentId = item.StudentId,
                        SchoolId = model.SchoolId,

                        ActivityProject = skill?.ActivityProject ?? "",
                        WrittenWork = skill?.WrittenWork ?? "",
                        WorkSheet = skill?.WorkSheet ?? "",

                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

            
            if (newRecords.Count > 0)
                await _Entities.TbAspectsNaturalSciencesSkills.AddRangeAsync(newRecords);

           
            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }


        [HttpPost]
        public async Task<IActionResult> SubmitStudentArtCraftSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var existingList = await _Entities.TbArtCraftSkills
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.SubjectId == model.SubjectId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbArtCraftSkill>();

            foreach (var item in model._SkillModel)
            {
                var skill = item._ArtCraft;

                bool hasValues =
                    !string.IsNullOrEmpty(skill?.Interest);


                if (!hasValues)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    
                    existing.Interest = skill?.Interest ?? "";
                }
                else
                {
                    
                    var add = new TbArtCraftSkill
                    {
                        AssesmentId = model.AssesmentId,
                        DivisionId = model.DivisionId,
                        SubjectId = model.SubjectId,
                        StudentId = item.StudentId,
                        SchoolId = model.SchoolId,

                        Interest = skill?.Interest ?? "",
                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

            if (newRecords.Count > 0)
                await _Entities.TbArtCraftSkills.AddRangeAsync(newRecords);

           
            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentMusicDanceSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var existingList = await _Entities.TbMusicDanceSkills
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.SubjectId == model.SubjectId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbMusicDanceSkill>();

            foreach (var item in model._SkillModel)
            {
                var skill = item._MusicDance;

                bool hasValue =
                    !string.IsNullOrEmpty(skill?.Interest) ||
                    !string.IsNullOrEmpty(skill?.Rhythm);

                
                if (!hasValue)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                   
                    existing.Interest = skill?.Interest ?? "";
                    existing.Rhythm = skill?.Rhythm ?? "";
                }
                else
                {
                  
                    var add = new TbMusicDanceSkill
                    {
                        AssesmentId = model.AssesmentId,
                        DivisionId = model.DivisionId,
                        SubjectId = model.SubjectId,
                        StudentId = item.StudentId,
                        SchoolId = model.SchoolId,

                        Interest = skill?.Interest ?? "",
                        Rhythm = skill?.Rhythm ?? "",

                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

            
            if (newRecords.Count > 0)
                await _Entities.TbMusicDanceSkills.AddRangeAsync(newRecords);

            
            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentAspectSocialStudiesSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var existingList = await _Entities.TbAspectsSocialStudiesSkills
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.SubjectId == model.SubjectId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbAspectsSocialStudiesSkill>();

            foreach (var item in model._SkillModel)
            {
                var skill = item._AspectsSocialStudies;

                bool hasValues =
                    !string.IsNullOrEmpty(skill?.ActivityProject) ||
                    !string.IsNullOrEmpty(skill?.Viva) ||
                    !string.IsNullOrEmpty(skill?.WrittenWork) ||
                    !string.IsNullOrEmpty(skill?.WorkSheet);

                
                if (!hasValues)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    
                    existing.ActivityProject = skill?.ActivityProject ?? "";
                   
                    existing.WrittenWork = skill?.WrittenWork ?? "";
                    existing.WorkSheet = skill?.WorkSheet ?? "";
                }
                else
                {
                   
                    var add = new TbAspectsSocialStudiesSkill
                    {
                        AssesmentId = model.AssesmentId,
                        DivisionId = model.DivisionId,
                        SubjectId = model.SubjectId,
                        StudentId = item.StudentId,
                        SchoolId = model.SchoolId,

                        ActivityProject = skill?.ActivityProject ?? "",
                        //VIVA = skill?.Viva ?? "",
                        WrittenWork = skill?.WrittenWork ?? "",
                        WorkSheet = skill?.WorkSheet ?? "",

                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

           
            if (newRecords.Count > 0)
                await _Entities.TbAspectsSocialStudiesSkills.AddRangeAsync(newRecords);

            
            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentAspectRelativeStudiesSkill(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var existingList = await _Entities.TbAspectsRelativeAreaSkills
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.SubjectId == model.SubjectId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbAspectsRelativeAreaSkill>();

            foreach (var item in model._SkillModel)
            {
                var skill = item._AspectsRelativeAreas;

                bool hasValues =
                    !string.IsNullOrEmpty(skill?.ComputerScience) ||
                    !string.IsNullOrEmpty(skill?.GeneralStudies) ||
                    !string.IsNullOrEmpty(skill?.Valueeducation) ||
                    !string.IsNullOrEmpty(skill?.French) ||
                    !string.IsNullOrEmpty(skill?.SmartClass);

            
                if (!hasValues)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    
                    existing.ComputerScience = skill?.ComputerScience ?? "";
                    existing.GeneralStudies = skill?.GeneralStudies ?? "";
                    existing.ValueEducation = skill?.Valueeducation ?? "";
                    existing.French = skill?.French ?? "";
                    existing.SmartClass = skill?.SmartClass ?? "";
                }
                else
                {
                   
                    var add = new TbAspectsRelativeAreaSkill
                    {
                        AssesmentId = model.AssesmentId,
                        DivisionId = model.DivisionId,
                        SubjectId = model.SubjectId,
                        StudentId = item.StudentId,
                        SchoolId = model.SchoolId,

                        ComputerScience = skill?.ComputerScience ?? "",
                        GeneralStudies = skill?.GeneralStudies ?? "",
                        ValueEducation = skill?.Valueeducation ?? "",
                        French = skill?.French ?? "",
                        SmartClass = skill?.SmartClass ?? "",

                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

            if (newRecords.Count > 0)
                await _Entities.TbAspectsRelativeAreaSkills.AddRangeAsync(newRecords);

            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }

        //--------------------------------------------------------------------------------------------------
        public IActionResult NonEducationalHome()
        {
            NonSubjectScoreModel model = new NonSubjectScoreModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.Vassessments = _dropdown.GetVAssesments(model.SchoolId);
            ViewBag.VClassDivisinLists = _dropdown.GetVClassDivisinLists(model.SchoolId);
            return View(model);
        }

        public PartialViewResult LoadAllStudentsWithNonSubjectSkillsMarks(string id)
        {
            string[] splitData = id.Split('~');
            long DivisionId = Convert.ToInt64(splitData[0]);
            long SkillId = Convert.ToInt64(splitData[1]);
            long AssesmentId = Convert.ToInt64(splitData[2]);
            StudentNonSubjectSkillMarks model = new StudentNonSubjectSkillMarks();
            model.SchoolId = _user.SchoolId;
            model.DivisionId = DivisionId;
            model.AssesmentId = AssesmentId;
            model._SkillModel = new List<NonSubjectSkillModel>();
            var studentList = _Entities.TbStudents.Where(x => x.SchoolId == model.SchoolId && x.DivisionId == model.DivisionId && x.IsActive).ToList();
            if (SkillId == 0)//GAMES Converted into physical education Gayathri(07/08/2023): Satluj V progresscard modification
            {
                #region GAMES
                var mark = _Entities.TbVGames.Where(x => x.DivisionId == model.DivisionId && x.IsActive && x.AssesmentId == model.AssesmentId && x.SchoolId == model.SchoolId).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._GamesScore = new GamesScore();
                            one._GamesScore.Enthusiasm = item.Enthusiasm;
                            //one._GamesScore.Discipline = item.Discipline;
                            //one._GamesScore.TeamSpirit = item.TeamSpirit;
                            one.RollNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._GamesScore = new GamesScore();
                            one._GamesScore.Enthusiasm = item.Enthusiasm;
                            //one._GamesScore.Discipline = item.Discipline;
                            //one._GamesScore.TeamSpirit = item.TeamSpirit;
                            one.RollNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        var balance = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balance)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._GamesScore = new GamesScore();
                            one._GamesScore.Enthusiasm = "";
                            //one._GamesScore.Discipline = "";
                            //one._GamesScore.TeamSpirit = "";
                            one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        NonSubjectSkillModel one = new NonSubjectSkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._GamesScore = new GamesScore();
                        one._GamesScore.Enthusiasm = "";
                        //one._GamesScore.Discipline = "";
                        //one._GamesScore.TeamSpirit = "";
                        one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RollNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentGamesScoreEntry.cshtml", model);
                #endregion
            }
            else if (SkillId == 1)//HEALTH
            {
                #region HEALTH
                var mark = _Entities.TbVHealths.Where(x => x.SchoolId == model.SchoolId && x.DivisionId == DivisionId && x.AssesmentId == model.AssesmentId && x.IsActive).ToList();
                var ass = _Entities.TbVAssesments.Where(x => x.Id == model.AssesmentId && x.IsActive && x.SchoolId == _user.SchoolId).FirstOrDefault();// Assesment and Period data
                var att = _Entities.TbAttendancePeriods.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.PeriodId == ass.PeriodId && x.IsActive).ToList();// Attendance Details 
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._HealthScore = new HealthScore();
                            one._HealthScore.Height = item.Height;
                            one._HealthScore.Weight = item.Weight;
                            one.RollNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            if (att != null && att.Count > 0)
                            {
                                var oneStudent = att.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                                if (oneStudent != null)
                                {
                                    one._HealthScore.WorkingDays = oneStudent.TotalDays == null ? 0 : oneStudent.TotalDays;
                                    one._HealthScore.Present = oneStudent.PresentDays == null ? 0 : oneStudent.PresentDays;
                                }
                                else
                                {
                                    one._HealthScore.WorkingDays = 0;
                                    one._HealthScore.Present = 0;
                                }
                            }
                            else
                            {
                                one._HealthScore.WorkingDays = 0;
                                one._HealthScore.Present = 0;
                            }
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._HealthScore = new HealthScore();
                            one._HealthScore.Height = item.Height;
                            one._HealthScore.Weight = item.Weight;
                            one.RollNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            if (att != null && att.Count > 0)
                            {
                                var oneStudent = att.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                                if (oneStudent != null)
                                {
                                    one._HealthScore.WorkingDays = oneStudent.TotalDays == null ? 0 : oneStudent.TotalDays;
                                    one._HealthScore.Present = oneStudent.PresentDays == null ? 0 : oneStudent.PresentDays;
                                }
                                else
                                {
                                    one._HealthScore.WorkingDays = 0;
                                    one._HealthScore.Present = 0;
                                }
                            }
                            else
                            {
                                one._HealthScore.WorkingDays = 0;
                                one._HealthScore.Present = 0;
                            }
                            model._SkillModel.Add(one);
                        }
                        var balance = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balance)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._HealthScore = new HealthScore();
                            one._HealthScore.Height = "";
                            one._HealthScore.Weight = "";
                            one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            if (att != null && att.Count > 0)
                            {
                                var oneStudent = att.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                                if (oneStudent != null)
                                {
                                    one._HealthScore.WorkingDays = oneStudent.TotalDays == null ? 0 : oneStudent.TotalDays;
                                    one._HealthScore.Present = oneStudent.PresentDays == null ? 0 : oneStudent.PresentDays;
                                }
                                else
                                {
                                    one._HealthScore.WorkingDays = 0;
                                    one._HealthScore.Present = 0;
                                }
                            }
                            else
                            {
                                one._HealthScore.WorkingDays = 0;
                                one._HealthScore.Present = 0;
                            }
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        NonSubjectSkillModel one = new NonSubjectSkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._HealthScore = new HealthScore();
                        one._HealthScore.Height = "";
                        one._HealthScore.Weight = "";
                        one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        if (att != null && att.Count > 0)
                        {
                            var oneStudent = att.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                            if (oneStudent != null)
                            {
                                one._HealthScore.WorkingDays = oneStudent.TotalDays == null ? 0 : oneStudent.TotalDays;
                                one._HealthScore.Present = oneStudent.PresentDays == null ? 0 : oneStudent.PresentDays;
                            }
                            else
                            {
                                one._HealthScore.WorkingDays = 0;
                                one._HealthScore.Present = 0;
                            }
                        }
                        else
                        {
                            one._HealthScore.WorkingDays = 0;
                            one._HealthScore.Present = 0;
                        }
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RollNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentHealthScoreEntry.cshtml", model);
                #endregion
            }
            else if (SkillId == 2)//PERSONALITY DEVELOPMENT
            {
                #region PERSONALITY DEVELOPMENT
                var mark = _Entities.TbVPersonalityDevelopments.Where(x => x.SchoolId == model.SchoolId && x.DivisionId == model.DivisionId && x.AssesmentId == model.AssesmentId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._Personality_Development = new Personality_Development();
                            one._Personality_Development.Courteousness = item.Courteousness;
                            one._Personality_Development.Confident = item.Confident;
                            one._Personality_Development.CareOfBelongings = item.CareOfBelongings;
                            one._Personality_Development.Neatness = item.Neatness;
                            one._Personality_Development.Punctuality = item.Punctuality;
                            one._Personality_Development.Initiative = item.Initiative;
                            one._Personality_Development.SharingCaring = item.SharingCaring;
                            one._Personality_Development.Property = item.Property;
                            one.RollNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._Personality_Development = new Personality_Development();
                            one._Personality_Development.Courteousness = item.Courteousness;
                            one._Personality_Development.Confident = item.Confident;
                            one._Personality_Development.CareOfBelongings = item.CareOfBelongings;
                            one._Personality_Development.Neatness = item.Neatness;
                            one._Personality_Development.Punctuality = item.Punctuality;
                            one._Personality_Development.Initiative = item.Initiative;
                            one._Personality_Development.SharingCaring = item.SharingCaring;
                            one._Personality_Development.Property = item.Property;
                            one.RollNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        var balance = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balance)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._Personality_Development = new Personality_Development();
                            one._Personality_Development.Courteousness = "";
                            one._Personality_Development.Confident = "";
                            one._Personality_Development.CareOfBelongings = "";
                            one._Personality_Development.Neatness = "";
                            one._Personality_Development.Punctuality = "";
                            one._Personality_Development.Initiative = "";
                            one._Personality_Development.SharingCaring = "";
                            one._Personality_Development.Property = "";
                            one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        NonSubjectSkillModel one = new NonSubjectSkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._Personality_Development = new Personality_Development();
                        one._Personality_Development.Courteousness = "";
                        one._Personality_Development.Confident = "";
                        one._Personality_Development.CareOfBelongings = "";
                        one._Personality_Development.Neatness = "";
                        one._Personality_Development.Punctuality = "";
                        one._Personality_Development.Initiative = "";
                        one._Personality_Development.SharingCaring = "";
                        one._Personality_Development.Property = "";
                        one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RollNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentPersonalityDevelopmentScoreEntry.cshtml", model);
                #endregion
            }
            else if (SkillId == 3)//REMARKS
            {
                #region REMARKS
                var mark = _Entities.TbVRemarks.Where(x => x.SchoolId == model.SchoolId && x.DivisionId == model.DivisionId && x.AssesmentId == model.AssesmentId && x.IsActive).ToList();
                if (mark.Count > 0 && mark != null)
                {
                    if (mark.Count == studentList.Count)
                    {
                        foreach (var item in mark)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._RemarkSchore = new RemarkSchore();
                            one._RemarkSchore.Remark = item.Remark == null ? "" : item.Remark;
                            one._RemarkSchore.AdditionalRemark = item.AdditionalRemarks == null ? "" : item.AdditionalRemarks;
                            one.RollNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in mark)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one._RemarkSchore = new RemarkSchore();
                            one._RemarkSchore.Remark = item.Remark == null ? "" : item.Remark;
                            one._RemarkSchore.AdditionalRemark = item.AdditionalRemarks == null ? "" : item.AdditionalRemarks;
                            one.RollNo = item.Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.Student.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                        var balance = studentList.Where(x => !mark.Any(y => y.StudentId == x.StudentId)).ToList();
                        foreach (var item in balance)
                        {
                            NonSubjectSkillModel one = new NonSubjectSkillModel();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one._RemarkSchore = new RemarkSchore();
                            one._RemarkSchore.Remark = "";
                            one._RemarkSchore.AdditionalRemark = "";
                            one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            model._SkillModel.Add(one);
                        }
                    }
                }
                else
                {
                    foreach (var item in studentList)
                    {
                        NonSubjectSkillModel one = new NonSubjectSkillModel();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one._RemarkSchore = new RemarkSchore();
                        one._RemarkSchore.Remark = "";
                        one._RemarkSchore.AdditionalRemark = "";
                        one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        model._SkillModel.Add(one);
                    }
                }
                model._SkillModel = model._SkillModel.OrderBy(x => x.RollNo).ThenBy(x => x.StudentName).ToList();
                return PartialView("~/Views/SpecialClass/_pv_StudentRemarkScoreEntry.cshtml", model);
                #endregion
            }
            return PartialView("~/Views/SpecialClass/_pv_StudentGamesScoreEntry.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentGameSkill(StudentNonSubjectSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var existingList = await _Entities.TbVGames
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbVGame>();

            foreach (var item in model._SkillModel)
            {
                var skill = item._GamesScore;

                bool hasValues =
                    !string.IsNullOrEmpty(skill?.Enthusiasm) ||
                    !string.IsNullOrEmpty(skill?.Discipline) ||
                    !string.IsNullOrEmpty(skill?.TeamSpirit);

                if (!hasValues)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    
                    existing.Enthusiasm = skill?.Enthusiasm ?? "";
                    //existing.Discipline = skill?.Discipline ?? "";
                    //existing.TeamSpirit = skill?.TeamSpirit ?? "";
                }
                else
                {
                    
                    var add = new TbVGame
                    {
                        SchoolId = _user.SchoolId,
                        StudentId = item.StudentId,
                        DivisionId = model.DivisionId,
                        AssesmentId = model.AssesmentId,

                        Enthusiasm = skill?.Enthusiasm ?? "",
                        //Discipline = skill?.Discipline ?? "",
                        //TeamSpirit = skill?.TeamSpirit ?? "",

                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

            
            if (newRecords.Count > 0)
                await _Entities.TbVGames.AddRangeAsync(newRecords);

        
            status = await _Entities.SaveChangesAsync() > 0;

            if (status)
                msg = "Successful";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentHealthSkill(StudentNonSubjectSkillMarks model)
        {
            string msg = "Failed";
            bool status = false;

         
            var ass = await _Entities.TbVAssesments
                .FirstOrDefaultAsync(x =>
                    x.Id == model.AssesmentId &&
                    x.IsActive &&
                    x.SchoolId == _user.SchoolId);

            if (ass == null)
                return Json(new { status = false, msg = "Invalid Assessment" });

            int periodId = (int)ass.PeriodId;

           
            var existingHealth = await _Entities.TbVHealths
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var healthByStudent = existingHealth.ToDictionary(x => x.StudentId, x => x);

            
            var existingAttendance = await _Entities.TbAttendancePeriods
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.PeriodId == periodId &&
                    x.IsActive)
                .ToListAsync();

            var attendanceByStudent = existingAttendance.ToDictionary(x => x.StudentId, x => x);

          
            var newHealthRecords = new List<TbVHealth>();
            var newAttendanceRecords = new List<TbAttendancePeriod>();

            foreach (var item in model._SkillModel)
            {
                var health = item._HealthScore;

                bool hasHealthValues =
                    !string.IsNullOrEmpty(health?.Height) ||
                    !string.IsNullOrEmpty(health?.Weight);

              
                if (hasHealthValues)
                {
                    if (healthByStudent.TryGetValue(item.StudentId, out var existingH))
                    {
                     
                        existingH.Height = health?.Height ?? "";
                        existingH.Weight = health?.Weight ?? "";
                    }
                    else
                    {
                    
                        newHealthRecords.Add(new TbVHealth
                        {
                            SchoolId = _user.SchoolId,
                            StudentId = item.StudentId,
                            DivisionId = model.DivisionId,
                            AssesmentId = model.AssesmentId,
                            Height = health?.Height ?? "",
                            Weight = health?.Weight ?? "",
                            IsActive = true,
                            TimeStamp = CurrentTime,
                            UserId = _user.UserId
                        });
                    }
                }

                bool hasAttendanceValues =
                    (health?.WorkingDays ?? 0) > 0 ||
                    (health?.Present ?? 0) > 0;

                if (attendanceByStudent.TryGetValue(item.StudentId, out var existingA))
                {
                    
                    existingA.TotalDays = health?.WorkingDays ?? 0;
                    existingA.PresentDays = health?.Present ?? 0;
                }
                else if (hasAttendanceValues)
                {
                    
                    newAttendanceRecords.Add(new TbAttendancePeriod
                    {
                        SchoolId = _user.SchoolId,
                        StudentId = item.StudentId,
                        DivisionId = model.DivisionId,
                        PeriodId = periodId,
                        UserId = _user.UserId,
                        TotalDays = health?.WorkingDays ?? 0,
                        PresentDays = health?.Present ?? 0,
                        IsActive = true,
                        TimeStampp = CurrentTime
                    });
                }
            }

            
            if (newHealthRecords.Count > 0)
                await _Entities.TbVHealths.AddRangeAsync(newHealthRecords);

            if (newAttendanceRecords.Count > 0)
                await _Entities.TbAttendancePeriods.AddRangeAsync(newAttendanceRecords);

           
            status = await _Entities.SaveChangesAsync() > 0;

            msg = status ? "Successful" : "Failed";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentPersonalSkill(StudentNonSubjectSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var existingList = await _Entities.TbVPersonalityDevelopments
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbVPersonalityDevelopment>();

            foreach (var item in model._SkillModel)
            {
                var skill = item._Personality_Development;

                bool hasValues =
                    !string.IsNullOrEmpty(skill?.Courteousness) ||
                    !string.IsNullOrEmpty(skill?.Confident) ||
                    !string.IsNullOrEmpty(skill?.CareOfBelongings) ||
                    !string.IsNullOrEmpty(skill?.Neatness) ||
                    !string.IsNullOrEmpty(skill?.Punctuality) ||
                    !string.IsNullOrEmpty(skill?.Initiative) ||
                    !string.IsNullOrEmpty(skill?.SharingCaring) ||
                    !string.IsNullOrEmpty(skill?.Property);

                if (!hasValues)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    existing.Courteousness = skill?.Courteousness ?? "";
                    existing.Confident = skill?.Confident ?? "";
                    existing.CareOfBelongings = skill?.CareOfBelongings ?? "";
                    existing.Neatness = skill?.Neatness ?? "";
                    existing.Punctuality = skill?.Punctuality ?? "";  
                    existing.Initiative = skill?.Initiative ?? "";
                    existing.SharingCaring = skill?.SharingCaring ?? "";
                    existing.Property = skill?.Property ?? "";
                }
                else
                {
                    var add = new TbVPersonalityDevelopment
                    {
                        SchoolId = _user.SchoolId,
                        StudentId = item.StudentId,
                        DivisionId = model.DivisionId,
                        AssesmentId = model.AssesmentId,

                        Courteousness = skill?.Courteousness ?? "",
                        Confident = skill?.Confident ?? "",
                        CareOfBelongings = skill?.CareOfBelongings ?? "",
                        Neatness = skill?.Neatness ?? "",
                        Punctuality = skill?.Punctuality ?? "",
                        Initiative = skill?.Initiative ?? "",
                        SharingCaring = skill?.SharingCaring ?? "",
                        Property = skill?.Property ?? "",

                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

            if (newRecords.Count > 0)
                await _Entities.TbVPersonalityDevelopments.AddRangeAsync(newRecords);

            status = await _Entities.SaveChangesAsync() > 0;

            msg = status ? "Successful" : "Failed";

            return Json(new { status, msg });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentRemarkSkill(StudentNonSubjectSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";

            var existingList = await _Entities.TbVRemarks
                .Where(x =>
                    x.DivisionId == model.DivisionId &&
                    x.AssesmentId == model.AssesmentId &&
                    x.IsActive)
                .ToListAsync();

            var existingByStudent = existingList.ToDictionary(x => x.StudentId, x => x);

            var newRecords = new List<TbVRemark>();

            foreach (var item in model._SkillModel)
            {
                string remark = item._RemarkSchore?.Remark?.Trim();
                string additional = item._RemarkSchore?.AdditionalRemark?.Trim();

                if (string.Equals(remark, "--Choose--", StringComparison.OrdinalIgnoreCase))
                    remark = "";

                bool hasAnyValue =
                    !string.IsNullOrEmpty(remark) ||
                    !string.IsNullOrEmpty(additional);

                if (!hasAnyValue)
                    continue;

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    existing.Remark = remark ?? "";
                    existing.AdditionalRemarks = additional?.ToUpper() ?? "";
                }
                else
                {
                    var add = new TbVRemark
                    {
                        SchoolId = _user.SchoolId,
                        StudentId = item.StudentId,
                        DivisionId = model.DivisionId,
                        AssesmentId = model.AssesmentId,

                        Remark = remark ?? "",
                        AdditionalRemarks = additional?.ToUpper() ?? "",

                        IsActive = true,
                        TimeStamp = CurrentTime,
                        UserId = _user.UserId
                    };

                    newRecords.Add(add);
                }
            }

            if (newRecords.Count > 0)
                await _Entities.TbVRemarks.AddRangeAsync(newRecords);

         
            status = await _Entities.SaveChangesAsync() > 0;

            msg = status ? "Successful" : "Failed";

            return Json(new { status, msg });
        }

        public IActionResult VProgressCardHome()
        {
            VProgressCardList model = new VProgressCardList();
            model.SchoolId = _user.SchoolId;
            model.ProgressCardName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            ViewBag.Divisionlist = _dropdown.GetVClassDivisinLists(model.SchoolId);
            return View(model);
        }
        public PartialViewResult ViewAll_V_ClassStudents(string id)
        {
            VProgressCardList model = new VProgressCardList();
            model.SchoolId = _user.SchoolId;
            model.DivisionId = Convert.ToInt64(id);
            return PartialView("~/Views/SpecialClass/_pv_StudentListForProgressCard.cshtml", model);
        }
        //-----------------------------------------------------------------------------
        public IActionResult VProgressCard(string id)
        {
            V_ProgressCard model = new V_ProgressCard();
            model.StudentId = Convert.ToInt64(id);
            var studentDetails = _Entities.TbStudents.Where(x => x.StudentId == model.StudentId).FirstOrDefault();
            model.ReportName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            model.SchoolName = studentDetails.School.SchoolName;
            model.SchoolLogo = "http://satluj.schoolman.in" + studentDetails.School.FilePath;
            model.AcademicYear = studentDetails.Class.AcademicYear.AcademicYear;
            //model.StudentProfile = "http://satluj.schoolman.in" + studentDetails.FilePath;
            model.StudentProfile = studentDetails.FilePath;
            //model.StudentProfile = "http://localhost:16135" + studentDetails.FilePath;
            model.StudentName = studentDetails.StundentName;
            model.ClassDivision = studentDetails.Class.Class + " " + studentDetails.Division.Division;
            model.RollNo = studentDetails.ClasssNumber;
            model.AdmissionNo = studentDetails.StudentSpecialId;
            try
            {
                model.DOB = Convert.ToDateTime(studentDetails.Dob).ToString("dd/MM/yyyy") ?? "";
            }
            catch
            {
                model.DOB = Convert.ToDateTime(studentDetails.Dob).ToShortDateString() ?? "";
            }
            model.FatherName = studentDetails.ParentName;
            model.MotherName = studentDetails.MotherName ?? "";
            var assesmentList = _Entities.TbVAssesments.Where(x => x.SchoolId == studentDetails.SchoolId && x.IsActive).OrderBy(x => x.Period.StartDate).ToList();

            //-------------CASE I---------------------
            #region Case I
            model._CaseI = new List<CaseI>();
            var subjects = _Entities.TbSubjects.Where(x => x.EnumTypeId == 0 && x.IsActive && x.SchoolI == studentDetails.SchoolId).ToList();
            if (subjects != null && subjects.Count > 0 && assesmentList != null && assesmentList.Count > 0)
            {
                int i = 0;
                foreach (var item in subjects)
                {
                    i = i + 1;
                    if (i < 4)
                    {
                        CaseI one = new CaseI();
                        one.SubjectName = item.SubjectName;
                        one._CaseIDetails = new List<CaseIDetails>();
                        int y = 0;
                        foreach (var item2 in assesmentList)
                        {
                            decimal aggregate = 0;
                            int aggregateCount = 0;
                            CaseIDetails two = new CaseIDetails();
                            if (y == 0)
                                two.Assesment = "I";
                            else if (y == 1)
                                two.Assesment = "II";
                            else
                                two.Assesment = "III";
                            var reading = _Entities.TbReadingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (reading != null)
                            {
                                two.Pronun = GradeCreation(reading.Pronun, studentDetails.SchoolId, VSpecialMarks.ReadingSkill, "Pronun");
                                two.Fluency = GradeCreation(reading.Fluency, studentDetails.SchoolId, VSpecialMarks.ReadingSkill, "Fluency");
                                if (reading.Pronun != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(reading.Pronun);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (reading.Fluency != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(reading.Fluency);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Pronun = "";
                                two.Fluency = "";
                            }
                            var writing = _Entities.TbWritingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (writing != null)
                            {
                                two.Creative_Writing = GradeCreation(writing.CreativeWriting, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Creative_Writing");
                                two.Hand_Writing = GradeCreation(writing.HandWriting, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Hand_Writing");
                                two.Grammar = GradeCreation(writing.Grammar, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Grammar");
                                two.Spelling = GradeCreation(writing.Spellings, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Spelling");
                                two.Vocabulary = GradeCreation(writing.Vocabulary, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Vocabulary");
                                two.Literature = GradeCreation(writing.UnitTest, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Literature");
                                two.Weekly_Assesment = GradeCreation(writing.WorkSheet, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Weekly_Assesment");
                                if (writing.CreativeWriting != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.CreativeWriting);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.HandWriting != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.HandWriting);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.Grammar != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.Grammar);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.Spellings != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.Spellings);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.Vocabulary != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.Vocabulary);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.UnitTest != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.UnitTest);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.WorkSheet != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.WorkSheet);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Creative_Writing = "";
                                two.Hand_Writing = "";
                                two.Grammar = "";
                                two.Spelling = "";
                                two.Vocabulary = "";
                                two.Literature = "";
                                two.Weekly_Assesment = "";
                            }
                            var speak = _Entities.TbSpeakingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (speak != null)
                            {
                                two.Conversation = GradeCreation(speak.Conversation, studentDetails.SchoolId, VSpecialMarks.SpeakingSkills, "Conversation");
                                two.Recitation = GradeCreation(speak.Recitation, studentDetails.SchoolId, VSpecialMarks.SpeakingSkills, "Recitation");
                                if (speak.Conversation != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(speak.Conversation);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (speak.Recitation != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(speak.Recitation);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Conversation = "";
                                two.Recitation = "";
                            }
                            var listen = _Entities.TbListenningSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (listen != null)
                            {
                                two.Comprehension = GradeCreation(listen.Comprehension, studentDetails.SchoolId, VSpecialMarks.ListeningSkill, "Comprehension");
                                if (listen.Comprehension != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(listen.Comprehension);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Comprehension = "";
                            }
                            if (aggregateCount != 0)
                            {
                                //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                                two.Aggregate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 1);
                            }
                            else
                            {
                                two.Aggregate = "";
                            }

                            one._CaseIDetails.Add(two);
                            y = y + 1;
                        }
                        model._CaseI.Add(one);
                    }
                }
            }
            #endregion

            //-------------CASE II--------------------
            #region 
            model._CaseII = new List<CaseII>();
            var subjectsI = _Entities.TbSubjects.Where(x => x.EnumTypeId == 1 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int p = 0;
            if (subjectsI != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    var data = _Entities.TbAspectsMathsSkills.Where(x => x.SubjectId == subjectsI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseII one = new CaseII();
                    if (p == 0)
                        one.Assesment = "I";
                    else if (p == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Concept = GradeCreation(data.Concepts, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Concept");
                        one.Activity = GradeCreation(data.Activity, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Activity");
                        one.Tables = GradeCreation(data.Tables, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Tables");
                        one.MentalAbility = GradeCreation(data.MentalAbility, studentDetails.SchoolId, VSpecialMarks.AspectsI, "MentalAbility");
                        one.WrittenWork = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsI, "WrittenWork");
                        one.WeeklyAssesment = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsI, "WeeklyAssesment");
                        if (data.Concepts != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Concepts);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.Activity != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Activity);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.Tables != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Tables);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.MentalAbility != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.MentalAbility);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Concept = "";
                        one.Activity = "";
                        one.Tables = "";
                        one.MentalAbility = "";
                        one.WrittenWork = "";
                        one.WeeklyAssesment = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 2);
                    }
                    else
                    {
                        one.Aggregate = "";
                    }
                    model._CaseII.Add(one);
                    p = p + 1;
                }
            }

            #endregion

            //-------------CASE III-------------------
            #region 
            model._CaseIII = new List<CaseIII>();
            var subjectsII = _Entities.TbSubjects.Where(x => x.EnumTypeId == 2 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int q = 0;
            if (subjectsII != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsNaturalSciencesSkills.Where(x => x.SubjectId == subjectsII.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseIII one = new CaseIII();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (q == 0)
                        one.Assesment = "I";
                    else if (q == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Activity_Pproject = GradeCreation(data.ActivityProject, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Activity_Pproject");
                        one.Group_Discussion = GradeCreation(data.GroupDiscussion, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Group_Discussion");
                        one.Written_Work = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Written_Work");
                        one.Work_Sheet = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Work_Sheet");
                        if (data.ActivityProject != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.ActivityProject);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.GroupDiscussion != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.GroupDiscussion);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Activity_Pproject = "";
                        one.Group_Discussion = "";
                        one.Written_Work = "";
                        one.Work_Sheet = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregrate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 3);
                    }
                    else
                    {
                        one.Aggregrate = "";
                    }
                    model._CaseIII.Add(one);
                    q = q + 1;
                }
            }

            #endregion

            //-------------CASE IV--------------------
            #region 
            model._CaseIV = new List<CaseIV>();
            var subjectsIII = _Entities.TbSubjects.Where(x => x.EnumTypeId == 4 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int ab = 0;
            if (subjectsIII != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsSocialStudiesSkills.Where(x => x.SubjectId == subjectsIII.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseIV one = new CaseIV();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (ab == 0)
                        one.Assesment = "I";
                    else if (ab == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Activity_Project = GradeCreation(data.ActivityProject, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Activity_Project");
                        one.Viva = GradeCreation(data.Viva, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Viva");
                        one.Written_Work = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Written_Work");
                        one.WorkSheet = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "WorkSheet");
                        if (data.ActivityProject != "")
                        {
                            aggregate = Convert.ToDecimal(data.ActivityProject);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.Viva != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Viva);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Activity_Project = "";
                        one.Viva = "";
                        one.Written_Work = "";
                        one.WorkSheet = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregrate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 4);
                    }
                    else
                    {
                        one.Aggregrate = "";
                    }
                    model._CaseIV.Add(one);
                    ab = ab + 1;
                }
            }
            #endregion

            //-------------CASE V---------------------
            #region 
            model._CaseV = new List<CaseV>();
            var subjectsIV = _Entities.TbSubjects.Where(x => x.EnumTypeId == 5 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int abc = 0;
            if (subjectsIV != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsRelativeAreaSkills.Where(x => x.SubjectId == subjectsIV.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseV one = new CaseV();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (abc == 0)
                        one.Assesment = "I";
                    else if (abc == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Computer_Science = GradeCreation(data.ComputerScience, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "Computer_Science");
                        one.GeneralStudies = GradeCreation(data.GeneralStudies, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "GeneralStudies");
                        one.ValueEducation = GradeCreation(data.ValueEducation, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "ValueEducation");
                        one.French = GradeCreation(data.French, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "French");
                        one.SmartClass = data.SmartClass; // Samrt Class Result is Grade, so not wants to calculate the score 
                    }
                    else
                    {
                        one.Computer_Science = "";
                        one.GeneralStudies = "";
                        one.ValueEducation = "";
                        one.French = "";
                        one.SmartClass = "";
                    }
                    #region Aggregate 
                    //if(one.Computer_Science != "")
                    //{
                    //    aggregate = Convert.ToDecimal(data.COMPUTER_SCIENCE);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.GeneralStudies!="")
                    //{
                    //    aggregate = aggregate+ Convert.ToDecimal(data.GENERAL_STUDIES);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.ValueEducation != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.VALUE_EDUCATION);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.French != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.FRENCH);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.SmartClass != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.SMART_CLASS);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (aggregateCount != 0)
                    //{
                    //    string grandAggregate =  Convert.ToString(aggregate / aggregateCount);
                    //    one.Aggregate = GradeCreation(grandAggregate);
                    //}
                    //else
                    //{
                    //    one.Aggregate = "";
                    //}
                    #endregion
                    model._CaseV.Add(one);
                    abc = abc + 1;
                }
            }
            #endregion

            //-------------CASE V---------------------
            #region 
            model._CaseVI = new List<CaseVI>();
            var subjectsVI = _Entities.TbSubjects.Where(x => x.EnumTypeId == 3 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int abcd = 0;
            if (subjectsVI != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbMusicDanceSkills.Where(x => x.SubjectId == subjectsVI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    var data1 = _Entities.TbArtCraftSkills.Where(x => x.SubjectId == subjectsVI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseVI one = new CaseVI();
                    if (abcd == 0)
                        one.assesment = "I";
                    else if (abcd == 1)
                        one.assesment = "II";
                    else
                    {
                        one.assesment = "III";
                    }
                    if (data1 != null)
                    {
                        one.artInterest = data1.Interest;
                        one.artCreativity = data1.Creativity;
                    }
                    else
                    {
                        one.artInterest = "";
                        one.artCreativity = "";
                    }
                    if (data != null)
                    {
                        one.musicInterest = data.Interest;
                        one.musicRhythm = data.Rhythm;
                    }
                    else
                    {
                        one.musicInterest = "";
                        one.musicRhythm = "";
                    }
                    model._CaseVI.Add(one);
                    abcd = abcd + 1;
                }
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseVII = new List<CaseVII>();
            int abcde = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVGames.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                var data1 = _Entities.TbVHealths.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseVII one = new CaseVII();
                if (abcde == 0)
                    one.Assesment = "I";
                else if (abcde == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data1 != null)
                {
                    one.Height = data1.Height;
                    one.Weight = data1.Weight;
                }
                else
                {
                    one.Height = "";
                    one.Weight = "";
                }
                if (data != null)
                {
                    one.Enthusiasm = data.Enthusiasm;
                    one.TeamSpirit = data.TeamSpirit;
                    one.Discipline = data.Discipline;
                }
                else
                {
                    one.Enthusiasm = "";
                    one.TeamSpirit = "";
                    one.Discipline = "";
                }
                model._CaseVII.Add(one);
                abcde = abcde + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseVIII = new List<CaseVIII>();
            int abcdef = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVPersonalityDevelopments.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseVIII one = new CaseVIII();
                if (abcdef == 0)
                    one.Assesment = "I";
                else if (abcdef == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data != null)
                {
                    one.Courteousness = data.Courteousness;
                    one.Confident = data.Confident;
                    one.CareOfBelongings = data.CareOfBelongings;
                    one.Neatness = data.Neatness;
                    one.Punctuality = data.Punctuality;
                    one.Initiative = data.Initiative;
                    one.Sharing_Caring = data.SharingCaring;
                    one.Property = data.Property;
                }
                else
                {
                    one.Courteousness = "";
                    one.Confident = "";
                    one.CareOfBelongings = "";
                    one.Neatness = "";
                    one.Punctuality = "";
                    one.Initiative = "";
                    one.Sharing_Caring = "";
                    one.Property = "";
                }
                model._CaseVIII.Add(one);
                abcdef = abcdef + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseIX = new List<CaseIX>();
            int abcdefg = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVRemarks.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseIX one = new CaseIX();
                if (abcdefg == 0)
                    one.Assesment = "I";
                else if (abcdefg == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data != null)
                {
                    var r1 = data.Remark == null ? "" : data.Remark;
                    var r2 = data.AdditionalRemarks == null ? "" : data.AdditionalRemarks.ToUpper();
                    if (r1.Trim() != "")
                    {
                        if (r2.Trim() != string.Empty)
                            one.Reamrk = r1 + " , " + r2;
                        else
                            one.Reamrk = r1;
                    }
                    else
                        one.Reamrk = r2;
                }
                else
                {
                    one.Reamrk = "";
                }
                model._CaseIX.Add(one);
                abcdefg = abcdefg + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseX = new List<CaseX>();
            int abcdefgh = 0;
            foreach (var item in assesmentList)
            {
                DateTime startDate = item.Period.StartDate;
                DateTime endDate = item.Period.EndDate;
                //var attendance = _Entities.tb_Attendance.Where(x => x.StudentId == studentDetails.StudentId && x.IsActive == true && x.ShiftStatus == 0 && System.Data.Entity.DbFunctions.TruncateTime(x.AttendanceDate) >= startDate.Date && System.Data.Entity.DbFunctions.TruncateTime(x.AttendanceDate) <= endDate.Date).ToList();// Data From the Attendnace Table 
                var attendance = _Entities.TbAttendancePeriods.Where(x => x.StudentId == studentDetails.StudentId && x.IsActive == true && x.PeriodId == item.Period.Id).FirstOrDefault();
                CaseX one = new CaseX();
                if (abcdefgh == 0)
                    one.Assesment = "I";
                else if (abcdefgh == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                //if (attendance != null && attendance.Count > 0) // DON'T DELETE 
                //{
                //    one.TotalWorkingDays = Convert.ToString(attendance.Count);
                //    one.PresentDays = Convert.ToString(attendance.Where(x => x.AttendanceData == true).ToList().Count());
                //}
                if (attendance != null)
                {
                    one.TotalWorkingDays = Convert.ToString(attendance.TotalDays);
                    one.PresentDays = Convert.ToString(attendance.PresentDays);
                }
                else
                {
                    one.TotalWorkingDays = "";
                    one.PresentDays = "";
                }
                model._CaseX.Add(one);
                abcdefgh = abcdefgh + 1;
            }
            #endregion

            return View(model);
        }

        public IActionResult VProgressCard_new(string id)
        {
            V_ProgressCard model = new V_ProgressCard();
            model.StudentId = Convert.ToInt64(id);
            var studentDetails = _Entities.TbStudents.Where(x => x.StudentId == model.StudentId).FirstOrDefault();
            model.ReportName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            model.SchoolName = studentDetails.School.SchoolName;
            model.SchoolLogo = "http://satluj.schoolman.in" + studentDetails.School.FilePath;
            model.AcademicYear = studentDetails.Class.AcademicYear.AcademicYear;
            //model.StudentProfile = "http://satluj.schoolman.in" + studentDetails.FilePath;
            model.StudentProfile = studentDetails.FilePath;
            //model.StudentProfile = "http://localhost:16135" + studentDetails.FilePath;
            model.StudentName = studentDetails.StundentName;
            model.ClassDivision = studentDetails.Class.Class + " " + studentDetails.Division.Division;
            model.RollNo = studentDetails.ClasssNumber;
            model.AdmissionNo = studentDetails.StudentSpecialId;
            try
            {
                model.DOB = Convert.ToDateTime(studentDetails.Dob).ToString("dd/MM/yyyy") ?? "";
            }
            catch
            {
                model.DOB = Convert.ToDateTime(studentDetails.Dob).ToShortDateString() ?? "";
            }
            model.FatherName = studentDetails.ParentName;
            model.MotherName = studentDetails.MotherName ?? "";
            var assesmentList = _Entities.TbVAssesments.Where(x => x.SchoolId == studentDetails.SchoolId && x.IsActive).OrderBy(x => x.Period.StartDate).ToList();

            //-------------CASE I---------------------
            #region Case I
            model._CaseI = new List<CaseI>();
            var subjects = _Entities.TbSubjects.Where(x => x.EnumTypeId == 0 && x.IsActive && x.SchoolI == studentDetails.SchoolId).ToList();
            if (subjects != null && subjects.Count > 0 && assesmentList != null && assesmentList.Count > 0)
            {
                int i = 0;
                foreach (var item in subjects)
                {
                    i = i + 1;
                    if (i < 4)
                    {
                        CaseI one = new CaseI();
                        one.SubjectName = item.SubjectName;
                        one._CaseIDetails = new List<CaseIDetails>();
                        int y = 0;
                        foreach (var item2 in assesmentList)
                        {
                            decimal aggregate = 0;
                            int aggregateCount = 0;
                            CaseIDetails two = new CaseIDetails();
                            if (y == 0)
                                two.Assesment = "I";
                            else if (y == 1)
                                two.Assesment = "II";
                            else
                                two.Assesment = "III";
                            var reading = _Entities.TbReadingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (reading != null)
                            {
                                two.Pronun = GradeCreation(reading.Pronun, studentDetails.SchoolId, VSpecialMarks.ReadingSkill, "Pronun");
                                two.Fluency = GradeCreation(reading.Fluency, studentDetails.SchoolId, VSpecialMarks.ReadingSkill, "Fluency");
                                if (reading.Pronun != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(reading.Pronun);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (reading.Fluency != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(reading.Fluency);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Pronun = "";
                                two.Fluency = "";
                            }
                            var writing = _Entities.TbWritingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (writing != null)
                            {
                                two.Creative_Writing = GradeCreation(writing.CreativeWriting, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Creative_Writing");
                                two.Hand_Writing = GradeCreation(writing.HandWriting, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Hand_Writing");
                                two.Grammar = GradeCreation(writing.Grammar, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Grammar");
                                two.Spelling = GradeCreation(writing.Spellings, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Spelling");
                                two.Vocabulary = GradeCreation(writing.Vocabulary, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Vocabulary");
                                two.Literature = GradeCreation(writing.UnitTest, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Literature");
                                two.Weekly_Assesment = GradeCreation(writing.WorkSheet, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Weekly_Assesment");
                                if (writing.CreativeWriting != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.CreativeWriting);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.HandWriting != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.HandWriting);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.Grammar != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.Grammar);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.Spellings != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.Spellings);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.Vocabulary != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.Vocabulary);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.UnitTest != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.UnitTest);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.WorkSheet != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.WorkSheet);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Creative_Writing = "";
                                two.Hand_Writing = "";
                                two.Grammar = "";
                                two.Spelling = "";
                                two.Vocabulary = "";
                                two.Literature = "";
                                two.Weekly_Assesment = "";
                            }
                            var speak = _Entities.TbSpeakingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (speak != null)
                            {
                                two.Conversation = GradeCreation(speak.Conversation, studentDetails.SchoolId, VSpecialMarks.SpeakingSkills, "Conversation");
                                two.Recitation = GradeCreation(speak.Recitation, studentDetails.SchoolId, VSpecialMarks.SpeakingSkills, "Recitation");
                                if (speak.Conversation != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(speak.Conversation);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (speak.Recitation != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(speak.Recitation);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Conversation = "";
                                two.Recitation = "";
                            }
                            var listen = _Entities.TbListenningSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (listen != null)
                            {
                                two.Comprehension = GradeCreation(listen.Comprehension, studentDetails.SchoolId, VSpecialMarks.ListeningSkill, "Comprehension");
                                if (listen.Comprehension != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(listen.Comprehension);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Comprehension = "";
                            }
                            if (aggregateCount != 0)
                            {
                                //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                                two.Aggregate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 1);
                            }
                            else
                            {
                                two.Aggregate = "";
                            }

                            one._CaseIDetails.Add(two);
                            y = y + 1;
                        }
                        model._CaseI.Add(one);
                    }
                }
            }
            #endregion

            //-------------CASE II--------------------
            #region 
            model._CaseII = new List<CaseII>();
            var subjectsI = _Entities.TbSubjects.Where(x => x.EnumTypeId == 1 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int p = 0;
            if (subjectsI != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    var data = _Entities.TbAspectsMathsSkills.Where(x => x.SubjectId == subjectsI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseII one = new CaseII();
                    if (p == 0)
                        one.Assesment = "I";
                    else if (p == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Concept = GradeCreation(data.Concepts, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Concept");
                        one.Activity = GradeCreation(data.Activity, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Activity");
                        one.Tables = GradeCreation(data.Tables, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Tables");
                        one.MentalAbility = GradeCreation(data.MentalAbility, studentDetails.SchoolId, VSpecialMarks.AspectsI, "MentalAbility");
                        one.WrittenWork = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsI, "WrittenWork");
                        one.WeeklyAssesment = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsI, "WeeklyAssesment");
                        if (data.Concepts != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Concepts);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.Activity != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Activity);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.Tables != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Tables);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.MentalAbility != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.MentalAbility);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Concept = "";
                        one.Activity = "";
                        one.Tables = "";
                        one.MentalAbility = "";
                        one.WrittenWork = "";
                        one.WeeklyAssesment = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 2);
                    }
                    else
                    {
                        one.Aggregate = "";
                    }
                    model._CaseII.Add(one);
                    p = p + 1;
                }
            }

            #endregion

            //-------------CASE III-------------------
            #region 
            model._CaseIII = new List<CaseIII>();
            var subjectsII = _Entities.TbSubjects.Where(x => x.EnumTypeId == 2 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int q = 0;
            if (subjectsII != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsNaturalSciencesSkills.Where(x => x.SubjectId == subjectsII.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseIII one = new CaseIII();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (q == 0)
                        one.Assesment = "I";
                    else if (q == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Activity_Pproject = GradeCreation(data.ActivityProject, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Activity_Pproject");
                        one.Group_Discussion = GradeCreation(data.GroupDiscussion, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Group_Discussion");
                        one.Written_Work = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Written_Work");
                        one.Work_Sheet = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Work_Sheet");
                        if (data.ActivityProject != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.ActivityProject);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.GroupDiscussion != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.GroupDiscussion);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Activity_Pproject = "";
                        one.Group_Discussion = "";
                        one.Written_Work = "";
                        one.Work_Sheet = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregrate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 3);
                    }
                    else
                    {
                        one.Aggregrate = "";
                    }
                    model._CaseIII.Add(one);
                    q = q + 1;
                }
            }

            #endregion

            //-------------CASE IV--------------------
            #region 
            model._CaseIV = new List<CaseIV>();
            var subjectsIII = _Entities.TbSubjects.Where(x => x.EnumTypeId == 4 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int ab = 0;
            if (subjectsIII != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsSocialStudiesSkills.Where(x => x.SubjectId == subjectsIII.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseIV one = new CaseIV();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (ab == 0)
                        one.Assesment = "I";
                    else if (ab == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Activity_Project = GradeCreation(data.ActivityProject, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Activity_Project");
                        one.Viva = GradeCreation(data.Viva, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Viva");
                        one.Written_Work = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Written_Work");
                        one.WorkSheet = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "WorkSheet");
                        if (data.ActivityProject != "")
                        {
                            aggregate = Convert.ToDecimal(data.ActivityProject);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.Viva != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Viva);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Activity_Project = "";
                        one.Viva = "";
                        one.Written_Work = "";
                        one.WorkSheet = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregrate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 4);
                    }
                    else
                    {
                        one.Aggregrate = "";
                    }
                    model._CaseIV.Add(one);
                    ab = ab + 1;
                }
            }
            #endregion

            //-------------CASE V---------------------
            #region 
            model._CaseV = new List<CaseV>();
            var subjectsIV = _Entities.TbSubjects.Where(x => x.EnumTypeId == 5 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int abc = 0;
            if (subjectsIV != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsRelativeAreaSkills.Where(x => x.SubjectId == subjectsIV.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseV one = new CaseV();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (abc == 0)
                        one.Assesment = "I";
                    else if (abc == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Computer_Science = GradeCreation(data.ComputerScience, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "Computer_Science");
                        one.GeneralStudies = GradeCreation(data.GeneralStudies, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "GeneralStudies");
                        one.ValueEducation = GradeCreation(data.ValueEducation, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "ValueEducation");
                        one.French = GradeCreation(data.French, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "French");
                        one.SmartClass = data.SmartClass; // Samrt Class Result is Grade, so not wants to calculate the score 
                    }
                    else
                    {
                        one.Computer_Science = "";
                        one.GeneralStudies = "";
                        one.ValueEducation = "";
                        one.French = "";
                        one.SmartClass = "";
                    }
                    #region Aggregate 
                    //if(one.Computer_Science != "")
                    //{
                    //    aggregate = Convert.ToDecimal(data.COMPUTER_SCIENCE);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.GeneralStudies!="")
                    //{
                    //    aggregate = aggregate+ Convert.ToDecimal(data.GENERAL_STUDIES);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.ValueEducation != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.VALUE_EDUCATION);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.French != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.FRENCH);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.SmartClass != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.SMART_CLASS);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (aggregateCount != 0)
                    //{
                    //    string grandAggregate =  Convert.ToString(aggregate / aggregateCount);
                    //    one.Aggregate = GradeCreation(grandAggregate);
                    //}
                    //else
                    //{
                    //    one.Aggregate = "";
                    //}
                    #endregion
                    model._CaseV.Add(one);
                    abc = abc + 1;
                }
            }
            #endregion

            //-------------CASE V---------------------
            #region 
            model._CaseVI = new List<CaseVI>();
            var subjectsVI = _Entities.TbSubjects.Where(x => x.EnumTypeId == 3 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int abcd = 0;
            if (subjectsVI != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbMusicDanceSkills.Where(x => x.SubjectId == subjectsVI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    var data1 = _Entities.TbArtCraftSkills.Where(x => x.SubjectId == subjectsVI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseVI one = new CaseVI();
                    if (abcd == 0)
                        one.assesment = "I";
                    else if (abcd == 1)
                        one.assesment = "II";
                    else
                    {
                        one.assesment = "III";
                    }
                    if (data1 != null)
                    {
                        one.artInterest = data1.Interest;
                        one.artCreativity = data1.Creativity;
                    }
                    else
                    {
                        one.artInterest = "";
                        one.artCreativity = "";
                    }
                    if (data != null)
                    {
                        one.musicInterest = data.Interest;
                        one.musicRhythm = data.Rhythm;
                    }
                    else
                    {
                        one.musicInterest = "";
                        one.musicRhythm = "";
                    }
                    model._CaseVI.Add(one);
                    abcd = abcd + 1;
                }
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseVII = new List<CaseVII>();
            int abcde = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVGames.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                var data1 = _Entities.TbVHealths.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseVII one = new CaseVII();
                if (abcde == 0)
                    one.Assesment = "I";
                else if (abcde == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data1 != null)
                {
                    one.Height = data1.Height;
                    one.Weight = data1.Weight;
                }
                else
                {
                    one.Height = "";
                    one.Weight = "";
                }
                if (data != null)
                {
                    one.Enthusiasm = data.Enthusiasm;
                    one.TeamSpirit = data.TeamSpirit;
                    one.Discipline = data.Discipline;
                }
                else
                {
                    one.Enthusiasm = "";
                    one.TeamSpirit = "";
                    one.Discipline = "";
                }
                model._CaseVII.Add(one);
                abcde = abcde + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseVIII = new List<CaseVIII>();
            int abcdef = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVPersonalityDevelopments.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseVIII one = new CaseVIII();
                if (abcdef == 0)
                    one.Assesment = "I";
                else if (abcdef == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data != null)
                {
                    one.Courteousness = data.Courteousness;
                    one.Confident = data.Confident;
                    one.CareOfBelongings = data.CareOfBelongings;
                    one.Neatness = data.Neatness;
                    one.Punctuality = data.Punctuality;
                    one.Initiative = data.Initiative;
                    one.Sharing_Caring = data.SharingCaring;
                    one.Property = data.Property;
                }
                else
                {
                    one.Courteousness = "";
                    one.Confident = "";
                    one.CareOfBelongings = "";
                    one.Neatness = "";
                    one.Punctuality = "";
                    one.Initiative = "";
                    one.Sharing_Caring = "";
                    one.Property = "";
                }
                model._CaseVIII.Add(one);
                abcdef = abcdef + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseIX = new List<CaseIX>();
            int abcdefg = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVRemarks.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseIX one = new CaseIX();
                if (abcdefg == 0)
                    one.Assesment = "I";
                else if (abcdefg == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data != null)
                {
                    var r1 = data.Remark == null ? "" : data.Remark;
                    var r2 = data.AdditionalRemarks == null ? "" : data.AdditionalRemarks.ToUpper();
                    if (r1.Trim() != "")
                    {
                        if (r2.Trim() != string.Empty)
                            one.Reamrk = r1 + " , " + r2;
                        else
                            one.Reamrk = r1;
                    }
                    else
                        one.Reamrk = r2;
                }
                else
                {
                    one.Reamrk = "";
                }
                model._CaseIX.Add(one);
                abcdefg = abcdefg + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseX = new List<CaseX>();
            int abcdefgh = 0;
            foreach (var item in assesmentList)
            {
                DateTime startDate = item.Period.StartDate;
                DateTime endDate = item.Period.EndDate;
                //var attendance = _Entities.tb_Attendance.Where(x => x.StudentId == studentDetails.StudentId && x.IsActive == true && x.ShiftStatus == 0 && System.Data.Entity.DbFunctions.TruncateTime(x.AttendanceDate) >= startDate.Date && System.Data.Entity.DbFunctions.TruncateTime(x.AttendanceDate) <= endDate.Date).ToList();// Data From the Attendnace Table 
                var attendance = _Entities.TbAttendancePeriods.Where(x => x.StudentId == studentDetails.StudentId && x.IsActive == true && x.PeriodId == item.Period.Id).FirstOrDefault();
                CaseX one = new CaseX();
                if (abcdefgh == 0)
                    one.Assesment = "I";
                else if (abcdefgh == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                //if (attendance != null && attendance.Count > 0) // DON'T DELETE 
                //{
                //    one.TotalWorkingDays = Convert.ToString(attendance.Count);
                //    one.PresentDays = Convert.ToString(attendance.Where(x => x.AttendanceData == true).ToList().Count());
                //}
                if (attendance != null)
                {
                    one.TotalWorkingDays = Convert.ToString(attendance.TotalDays);
                    one.PresentDays = Convert.ToString(attendance.PresentDays);
                }
                else
                {
                    one.TotalWorkingDays = "";
                    one.PresentDays = "";
                }
                model._CaseX.Add(one);
                abcdefgh = abcdefgh + 1;
            }
            #endregion

            return View(model);
        }

        public IActionResult VprogressCard_Newone(string id)
        {
            V_ProgressCard model = new V_ProgressCard();
            model.StudentId = Convert.ToInt64(id);
            var studentDetails = _Entities.TbStudents.Where(x => x.StudentId == model.StudentId).FirstOrDefault();
            model.ReportName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            model.SchoolName = studentDetails.School.SchoolName;
            model.SchoolLogo = "http://satluj.schoolman.in" + studentDetails.School.FilePath;
            model.AcademicYear = studentDetails.Class.AcademicYear.AcademicYear;
            //model.StudentProfile = "http://satluj.schoolman.in" + studentDetails.FilePath;
            model.StudentProfile = studentDetails.FilePath;
            //model.StudentProfile = "http://localhost:16135" + studentDetails.FilePath;
            model.StudentName = studentDetails.StundentName;
            model.ClassDivision = studentDetails.Class.Class + " " + studentDetails.Division.Division;
            model.RollNo = studentDetails.ClasssNumber;
            model.AdmissionNo = studentDetails.StudentSpecialId;
            try
            {
                model.DOB = Convert.ToDateTime(studentDetails.Dob).ToString("dd/MM/yyyy") ?? "";
            }
            catch
            {
                model.DOB = Convert.ToDateTime(studentDetails.Dob).ToShortDateString() ?? "";
            }
            model.FatherName = studentDetails.ParentName;
            model.MotherName = studentDetails.MotherName ?? "";
            var assesmentList = _Entities.TbVAssesments.Where(x => x.SchoolId == studentDetails.SchoolId && x.IsActive).OrderBy(x => x.Period.StartDate).ToList();

            //-------------CASE I---------------------
            #region Case I
            model._CaseI = new List<CaseI>();
            var subjects = _Entities.TbSubjects.Where(x => x.EnumTypeId == 0 && x.IsActive && x.SchoolI == studentDetails.SchoolId).ToList();
            if (subjects != null && subjects.Count > 0 && assesmentList != null && assesmentList.Count > 0)
            {
                int i = 0;
                foreach (var item in subjects)
                {
                    i = i + 1;
                    if (i < 4)
                    {
                        CaseI one = new CaseI();
                        one.SubjectName = item.SubjectName;
                        one._CaseIDetails = new List<CaseIDetails>();
                        int y = 0;
                        foreach (var item2 in assesmentList)
                        {
                            decimal aggregate = 0;
                            int aggregateCount = 0;
                            CaseIDetails two = new CaseIDetails();
                            if (y == 0)
                                two.Assesment = "I";
                            else if (y == 1)
                                two.Assesment = "II";
                            else
                                two.Assesment = "III";
                            var reading = _Entities.TbReadingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (reading != null)
                            {
                                two.Pronun = GradeCreation(reading.ReadingSkill, studentDetails.SchoolId, VSpecialMarks.ReadingSkill, "ReadingSkill");
                                //two.Fluency = GradeCreation(reading.FLUENCY, studentDetails.SchoolId, VSpecialMarks.ReadingSkill, "Fluency");
                                if (reading.ReadingSkill != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(reading.ReadingSkill);
                                    aggregateCount = aggregateCount + 1;
                                }
                                //if (reading.FLUENCY != "")
                                //{
                                //    aggregate = aggregate + Convert.ToDecimal(reading.FLUENCY);
                                //    aggregateCount = aggregateCount + 1;
                                //}
                            }
                            else
                            {
                                two.Pronun = "";
                                //two.Fluency = "";
                            }
                            var writing = _Entities.TbWritingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (writing != null)
                            {
                                two.Literature = GradeCreation(writing.WritingSkill, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "WritingSkill");
                                two.Weekly_Assesment = GradeCreation(writing.WorkSheet, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "WeeklyAssessment");
                               
                                if (writing.WritingSkill != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.WritingSkill);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.WorkSheet != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.WorkSheet);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                               
                                two.Literature = "";
                                two.Weekly_Assesment = "";
                            }
                            var speak = _Entities.TbSpeakingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (speak != null)
                            {
                                two.Conversation = GradeCreation(speak.SpeakingSkill, studentDetails.SchoolId, VSpecialMarks.SpeakingSkills, "Speaking Skills");
                                if (speak.SpeakingSkill != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(speak.SpeakingSkill);
                                    aggregateCount = aggregateCount + 1;
                                }
                                
                            }
                            else
                            {
                                two.Conversation = "";
                                //two.Recitation = "";
                            }
                            var listen = _Entities.TbListenningSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == studentDetails.DivisionId && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (listen != null)
                            {
                                two.Comprehension = GradeCreation(listen.ListeningSkill, studentDetails.SchoolId, VSpecialMarks.ListeningSkill, "ListeningSkill");
                                if (listen.ListeningSkill != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(listen.ListeningSkill);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Comprehension = "";
                            }
                            if (aggregateCount != 0)
                            {
                                //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                                two.Aggregate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 1);
                            }
                            else
                            {
                                two.Aggregate = "";
                            }

                            one._CaseIDetails.Add(two);
                            y = y + 1;
                        }
                        model._CaseI.Add(one);
                    }
                }
            }
            #endregion

            //-------------CASE II--------------------
            #region 
            model._CaseII = new List<CaseII>();
            var subjectsI = _Entities.TbSubjects.Where(x => x.EnumTypeId == 1 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int p = 0;
            if (subjectsI != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    var data = _Entities.TbAspectsMathsSkills.Where(x => x.SubjectId == subjectsI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseII one = new CaseII();
                    if (p == 0)
                        one.Assesment = "I";
                    else if (p == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Concept = GradeCreation(data.Concepts, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Concept Clarity/Activity");
                        one.WrittenWork = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Written Work");
                        one.WeeklyAssesment = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Weekly Assessment");
                        if (data.Concepts != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Concepts);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Concept = "";
                        one.WrittenWork = "";
                        one.WeeklyAssesment = "";
                    }
                    if (aggregateCount != 0)
                    {
                        one.Aggregate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 2);
                    }
                    else
                    {
                        one.Aggregate = "";
                    }
                    model._CaseII.Add(one);
                    p = p + 1;
                }
            }

            #endregion

            //-------------CASE III-------------------
            #region 
            model._CaseIII = new List<CaseIII>();
            var subjectsII = _Entities.TbSubjects.Where(x => x.EnumTypeId == 2 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int q = 0;
            if (subjectsII != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsNaturalSciencesSkills.Where(x => x.SubjectId == subjectsII.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseIII one = new CaseIII();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (q == 0)
                        one.Assesment = "I";
                    else if (q == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Activity_Pproject = GradeCreation(data.ActivityProject, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Activity/VIVA");
                        one.Written_Work = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Written Work");
                        one.Work_Sheet = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Weekly Assessment");
                        if (data.ActivityProject != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.ActivityProject);
                            aggregateCount = aggregateCount + 1;
                        }
                        
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Activity_Pproject = "";
                        one.Written_Work = "";
                        one.Work_Sheet = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregrate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 3);
                    }
                    else
                    {
                        one.Aggregrate = "";
                    }
                    model._CaseIII.Add(one);
                    q = q + 1;
                }
            }

            #endregion

            //-------------CASE IV--------------------
            #region 
            model._CaseIV = new List<CaseIV>();
            var subjectsIII = _Entities.TbSubjects.Where(x => x.EnumTypeId == 4 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int ab = 0;
            if (subjectsIII != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsSocialStudiesSkills.Where(x => x.SubjectId == subjectsIII.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseIV one = new CaseIV();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (ab == 0)
                        one.Assesment = "I";
                    else if (ab == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Activity_Project = GradeCreation(data.ActivityProject, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Activity/VIVA");
                        one.Written_Work = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Written Work");
                        one.WorkSheet = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Weekly Assessment");
                        if (data.ActivityProject != "")
                        {
                            aggregate = Convert.ToDecimal(data.ActivityProject);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Activity_Project = "";
                        one.Written_Work = "";
                        one.WorkSheet = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregrate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 4);
                    }
                    else
                    {
                        one.Aggregrate = "";
                    }
                    model._CaseIV.Add(one);
                    ab = ab + 1;
                }
            }
            #endregion

            //-------------CASE V---------------------
            #region 
            model._CaseV = new List<CaseV>();
            var subjectsIV = _Entities.TbSubjects.Where(x => x.EnumTypeId == 5 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int abc = 0;
            if (subjectsIV != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsRelativeAreaSkills.Where(x => x.SubjectId == subjectsIV.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseV one = new CaseV();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (abc == 0)
                        one.Assesment = "I";
                    else if (abc == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Computer_Science = GradeCreation(data.ComputerScience, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "IT");
                        one.GeneralStudies = GradeCreation(data.GeneralStudies, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "GeneralStudies");
                        one.ValueEducation = GradeCreation(data.ValueEducation, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "ValueEducation");
                        one.French = GradeCreation(data.French, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "French");
                        one.SmartClass = data.SmartClass; // Samrt Class Result is Grade, so not wants to calculate the score 
                    }
                    else
                    {
                        one.Computer_Science = "";
                        one.GeneralStudies = "";
                        one.ValueEducation = "";
                        one.French = "";
                        one.SmartClass = "";
                    }
                    #region Aggregate 
                    //if (one.Computer_Science != "")
                    //{
                    //    aggregate = Convert.ToDecimal(data.COMPUTER_SCIENCE);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.GeneralStudies != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.GENERAL_STUDIES);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.ValueEducation != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.VALUE_EDUCATION);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.French != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.FRENCH);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.SmartClass != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.SMART_CLASS);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (aggregateCount != 0)
                    //{
                    //    string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                    //    one.Aggregate = GradeCreation(grandAggregate);
                    //}
                    //else
                    //{
                    //    one.Aggregate = "";
                    //}
                    #endregion
                    model._CaseV.Add(one);
                    abc = abc + 1;
                }
            }
            #endregion

            //-------------CASE V---------------------
            #region 
            model._CaseVI = new List<CaseVI>();
            var subjectsVI = _Entities.TbSubjects.Where(x => x.EnumTypeId == 3 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int abcd = 0;
            if (subjectsVI != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbMusicDanceSkills.Where(x => x.SubjectId == subjectsVI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    var data1 = _Entities.TbArtCraftSkills.Where(x => x.SubjectId == subjectsVI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    var physical_Edu = _Entities.TbVGames.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();

                    CaseVI one = new CaseVI();
                    if (abcd == 0)
                        one.assesment = "I";
                    else if (abcd == 1)
                        one.assesment = "II";
                    else
                    {
                        one.assesment = "III";
                    }
                    if (data1 != null)
                    {
                        one.artInterest = data1.Interest;
                        //one.artCreativity = data1.CREATIVITY;
                    }
                    else
                    {
                       one.artInterest = "";
                        //one.artCreativity = "";
                    }
                    if (data != null)
                    {
                        one.musicInterest = data.Interest;
                        one.musicRhythm = data.Rhythm;
                    }
                    else
                    {
                        one.musicInterest = "";
                        one.musicRhythm = "";
                    }
                    if(physical_Edu !=null)
                    {
                        one.artCreativity = physical_Edu.Enthusiasm;
                    }
                    else
                    {
                        one.artCreativity = "";

                    }
                    model._CaseVI.Add(one);
                    abcd = abcd + 1;
                }
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseVII = new List<CaseVII>();
            int abcde = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVGames.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                var data1 = _Entities.TbVHealths.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseVII one = new CaseVII();
                if (abcde == 0)
                    one.Assesment = "I";
                else if (abcde == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data1 != null)
                {
                    one.Height = data1.Height;
                    one.Weight = data1.Weight;
                }
                else
                {
                    one.Height = "";
                    one.Weight = "";
                }
                if (data != null)
                {
                    one.Enthusiasm = data.Enthusiasm;
                    one.TeamSpirit = data.TeamSpirit;
                    one.Discipline = data.Discipline;
                }
                else
                {
                    one.Enthusiasm = "";
                    one.TeamSpirit = "";
                    one.Discipline = "";
                }
                model._CaseVII.Add(one);
                abcde = abcde + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseVIII = new List<CaseVIII>();
            int abcdef = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVPersonalityDevelopments.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseVIII one = new CaseVIII();
                if (abcdef == 0)
                    one.Assesment = "I";
                else if (abcdef == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data != null)
                {
                    one.Courteousness = data.Courteousness;
                    one.Confident = data.Confident;
                    one.CareOfBelongings = data.CareOfBelongings;
                    one.Neatness = data.Neatness;
                    one.Punctuality = data.Punctuality;
                    one.Initiative = data.Initiative;
                    one.Sharing_Caring = data.SharingCaring;
                    one.Property = data.Property;
                }
                else
                {
                    one.Courteousness = "";
                    one.Confident = "";
                    one.CareOfBelongings = "";
                    one.Neatness = "";
                    one.Punctuality = "";
                    one.Initiative = "";
                    one.Sharing_Caring = "";
                    one.Property = "";
                }
                model._CaseVIII.Add(one);
                abcdef = abcdef + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseIX = new List<CaseIX>();
            int abcdefg = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVRemarks.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseIX one = new CaseIX();
                if (abcdefg == 0)
                    one.Assesment = "I";
                else if (abcdefg == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data != null)
                {
                    var r1 = data.Remark == null ? "" : data.Remark;
                    var r2 = data.AdditionalRemarks == null ? "" : data.AdditionalRemarks.ToUpper();
                    if (r1.Trim() != "")
                    {
                        if (r2.Trim() != string.Empty)
                            one.Reamrk = r1 + " , " + r2;
                        else
                            one.Reamrk = r1;
                    }
                    else
                        one.Reamrk = r2;
                }
                else
                {
                    one.Reamrk = "";
                }
                model._CaseIX.Add(one);
                abcdefg = abcdefg + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseX = new List<CaseX>();
            int abcdefgh = 0;
            foreach (var item in assesmentList)
            {
                DateTime startDate = item.Period.StartDate;
                DateTime endDate = item.Period.EndDate;
                //var attendance = _Entities.tb_Attendance.Where(x => x.StudentId == studentDetails.StudentId && x.IsActive == true && x.ShiftStatus == 0 && System.Data.Entity.DbFunctions.TruncateTime(x.AttendanceDate) >= startDate.Date && System.Data.Entity.DbFunctions.TruncateTime(x.AttendanceDate) <= endDate.Date).ToList();// Data From the Attendnace Table 
                var attendance = _Entities.TbAttendancePeriods.Where(x => x.StudentId == studentDetails.StudentId && x.IsActive == true && x.PeriodId == item.Period.Id).FirstOrDefault();
                CaseX one = new CaseX();
                if (abcdefgh == 0)
                    one.Assesment = "I";
                else if (abcdefgh == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                //if (attendance != null && attendance.Count > 0) // DON'T DELETE 
                //{
                //    one.TotalWorkingDays = Convert.ToString(attendance.Count);
                //    one.PresentDays = Convert.ToString(attendance.Where(x => x.AttendanceData == true).ToList().Count());
                //}
                if (attendance != null)
                {
                    one.TotalWorkingDays = Convert.ToString(attendance.TotalDays);
                    one.PresentDays = Convert.ToString(attendance.PresentDays);
                }
                else
                {
                    one.TotalWorkingDays = "";
                    one.PresentDays = "";
                }
                model._CaseX.Add(one);
                abcdefgh = abcdefgh + 1;
            }
            #endregion

            return View(model);
        }

        public string GradeCreation(string mark, long schoolId, VSpecialMarks enumValue, string title)
        {
            string grade = "";
            int enumno = Convert.ToInt32(enumValue);
            decimal defautTotal = 100;
            var data = _Entities.TbVTotalScoreLists.Where(x => x.SchoolId == schoolId && x.EnumTypeId == enumno && x.IsActive && x.SubTitle == title).FirstOrDefault();
            if (data != null)
            {
                defautTotal = data.Mark;
            }
            if(mark==null || mark =="")
            {

            }
            else
            {
                decimal realScore = Convert.ToDecimal(mark);
                decimal givenMark = Math.Round(((realScore / defautTotal) * 100), MidpointRounding.AwayFromZero);

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
                    grade = "D";
                }
            }
            return grade;
        }

        public string GradeCreationCase(string mark, long schoolId, int caseNo)
        {
            string grade = "";
            decimal totalScore = 100;
            int count = 1;
            if (caseNo == 1) // Reading , Writing, Speaking, Listening
            {
                //var enumList = new int[] { 0, 1, 2, 3 }.ToList();//Modified by Gayathri A Class V Progresscard recreation
                var enumList = new int[] { 10, 11, 12, 13 }.ToList();
                var totalScoreData = _Entities.TbVTotalScoreLists.Where(x => enumList.Contains(x.EnumTypeId) && x.SchoolId == schoolId && x.IsActive == true).ToList();
                if (totalScoreData != null && totalScoreData.Count > 0)
                {
                    totalScore = totalScoreData.Sum(x => x.Mark);
                    count = totalScoreData.Count();
                }
            }
            else if (caseNo == 2) // Mathematics
            {
                var enumValue = Convert.ToInt32(VSpecialMarks.AspectsI);
                var totalScoreData = _Entities.TbVTotalScoreLists.Where(x => x.EnumTypeId == enumValue && x.SchoolId == schoolId && x.IsActive == true).ToList();
                if (totalScoreData != null && totalScoreData.Count > 0)
                {
                    totalScore = totalScoreData.Sum(x => x.Mark);
                    count = totalScoreData.Count();
                }
            }
            else if (caseNo == 3) // Natural Science
            {
                var enumValue = Convert.ToInt32(VSpecialMarks.AspectsII);
                var totalScoreData = _Entities.TbVTotalScoreLists.Where(x => x.EnumTypeId == enumValue && x.SchoolId == schoolId && x.IsActive == true).ToList();
                if (totalScoreData != null && totalScoreData.Count > 0)
                {
                    totalScore = totalScoreData.Sum(x => x.Mark);
                    count = totalScoreData.Count();
                }
            }
            else if (caseNo == 4) // Social Science
            {
                var enumValue = Convert.ToInt32(VSpecialMarks.AspectsIII);
                var totalScoreData = _Entities.TbVTotalScoreLists.Where(x => x.EnumTypeId == enumValue && x.SchoolId == schoolId && x.IsActive == true).ToList();
                if (totalScoreData != null && totalScoreData.Count > 0)
                {
                    totalScore = totalScoreData.Sum(x => x.Mark);
                    count = totalScoreData.Count();
                }
            }
            if (mark.Trim() != "")
            {
                decimal realScore = Convert.ToDecimal(mark);
                //Razi changed to roundof realscore on 11/10/22
                decimal givenMark = Math.Round(((realScore / totalScore) * 100), MidpointRounding.AwayFromZero);
                //decimal givenMark = ((realScore / totalScore) * 100);

                if (givenMark > 100)
                {
                    grade = "A*";
                }
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
                    grade = "D";
                }
            }
            return grade;
        }

        /// <summary>
        /// 999999
        /// </summary>
        /// <returns></returns>
        #region SCeo Compleet
        public IActionResult AssesmentHome_Others()
        {
            SpecialClassModel model = new SpecialClassModel();
            model.SchoolId = _user.SchoolId;
            DropdownData dropdown = new DropdownData();
            ViewBag.PeriodList = dropdown.GetPeriodsLists(model.SchoolId);

            return View(model);
        }

        #endregion

        public IActionResult ReadingSkillsHome_Others()
        {
            var model = new SubjectScoreModel
            {
                SchoolId = _user.SchoolId,
                UserId = _user.UserId,
                IsAdminCheck = HttpContext.Session.GetInt32("isAdmin") ?? 0
            };
            ViewBag.ClassList = _dropdown
    .GetClass_Preschool(model.SchoolId)
    .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
    {
        Text = x.Text,
        Value = x.Value
    })
    .ToList();


            return View(model);
        }

        public PartialViewResult LoadAllStudentsWithSkillsMarks_others(string id)
        {
            string[] data = id.Split('~');
            long ClassId = Convert.ToInt64(data[0]);
            long DivisionId = Convert.ToInt64(data[1]);
            long AssesmentId = Convert.ToInt32(data[2]);
            long SubjectId = Convert.ToInt64(data[3]);
             

            StudentSkillMarks model = new StudentSkillMarks();
            model.ClassId = ClassId;
            model.SchoolId = _user.SchoolId;
            model.DivisionId = DivisionId;
            model.AssesmentId = AssesmentId;
            model.SubjectId = SubjectId;
            
            model._SkillModel = new List<SkillModel>();
            //var studentList = TempMo1.Where(x => x.classId == ClassId && x.divisionId == DivisionId && x.schoolId == _user.SchoolId).OrderBy(x => x.studentName).ToList();

             var studentList = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == DivisionId && x.SchoolId == _user.SchoolId && x.IsActive).OrderBy(x => x.StundentName).ToList();

            if (studentList.Count > 0)
            {
                //List<SkillModel> sk_li = new List<SkillModel>();
                List<LKG_UKG_NURSERY_SKILLS_Model> sk_li = new List<LKG_UKG_NURSERY_SKILLS_Model>();
                string currentYear = _Entities.TbAcademicYears.Where(X => X.CurrentYear == true).Select(X => X.AcademicYear).FirstOrDefault();

                foreach (var a1 in studentList)
                {
                    //SkillModel mo = new SkillModel();
                    LKG_UKG_NURSERY_SKILLS_Model mo = new LKG_UKG_NURSERY_SKILLS_Model();

                    mo.studentName = a1.StundentName;
                    mo.StudentId = a1.StudentId;
                    mo.ClassId = a1.ClassId;
                    var one = _Entities.TbLkgUkgNurserySkills.Where(x => x.Year == currentYear && x.SchoolId == model.SchoolId
                                                                      && x.StudentId == a1.StudentId
                                                                      && x.ClassId == a1.ClassId && x.DivisionId == model.DivisionId
                                                                      && x.SubjectId == model.SubjectId && x.AssesmentId == model.AssesmentId
                                                                      && x.IsActive == true).FirstOrDefault();

                    //English..........

                    if (one != null)
                    {

                        if (model.SubjectId == 1)
                        {
                            if (one.WritingSkills != null)
                            {
                                mo.WritingSkills = one.WritingSkills;
                            }
                            else
                            {
                                mo.WritingSkills = "";
                            }
                            if (one.ReadingSkills != null)
                            {
                                mo.ReadingSkills = one.ReadingSkills;
                            }
                            else
                            {
                                mo.ReadingSkills = "";
                            }
                            if (one.SpeakingSkills != null)
                            {
                                mo.SpeakingSkills = one.SpeakingSkills;
                            }
                            else
                            {
                                mo.SpeakingSkills = "";
                            }
                            if (one.RecitingSkills != null)
                            {
                                mo.RecitingSkills = one.RecitingSkills;
                            }
                            else
                            {
                                mo.RecitingSkills = "";
                            }
                            if (one.ListeningSkills != null)
                            {
                                mo.ListeningSkills = one.ListeningSkills;
                            }
                            else
                            {
                                mo.ListeningSkills = "";
                            }
                            if (one.Oral != null)
                            {
                                mo.Oral = one.Oral;
                            }
                            else
                            {
                                mo.Oral = "";
                            }
                            //

                        }

                        else if (model.SubjectId == 2)//Hindi
                        {
                            if (one.WritingSkills != null)
                            {
                                mo.WritingSkills = one.WritingSkills;
                            }
                            else
                            {
                                mo.WritingSkills = "";
                            }
                            if (one.ReadingSkills != null)
                            {
                                mo.ReadingSkills = one.ReadingSkills;
                            }
                            else
                            {
                                mo.ReadingSkills = "";
                            }
                            if (one.SpeakingSkills != null)
                            {
                                mo.SpeakingSkills = one.SpeakingSkills;
                            }
                            else
                            {
                                mo.SpeakingSkills = "";
                            }
                            if (one.RecitingSkills != null)
                            {
                                mo.RecitingSkills = one.RecitingSkills;
                            }
                            else
                            {
                                mo.RecitingSkills = "";
                            }
                            if (one.ListeningSkills != null)
                            {
                                mo.ListeningSkills = one.ListeningSkills;
                            }
                            else
                            {
                                mo.ListeningSkills = "";
                            }
                            if (one.Oral != null)
                            {
                                mo.Oral = one.Oral;
                            }
                            else
                            {
                                mo.Oral = "";
                            }
                        }
                        else if (model.SubjectId == 3)
                        {
                            if (one.LogicalandReasoning != null)
                            {
                                mo.LogicalandReasoning = one.LogicalandReasoning;
                            }
                            else
                            {
                                mo.LogicalandReasoning = "";
                            }
                            if (one.Understandingofconcepts != null)
                            {
                                mo.Understandingofconcepts = one.Understandingofconcepts;
                            }
                            else
                            {
                                mo.Understandingofconcepts = "";
                            }
                            if (one.MentalAbility != null)
                            {
                                mo.MentalAbility = one.MentalAbility;
                            }
                            else
                            {
                                mo.MentalAbility = "";
                            }
                            if (one.Oral != null)
                            {
                                mo.Oral = one.Oral;
                            }
                            else
                            {
                                mo.Oral = "";
                            }
                        }
                        else if (model.SubjectId == 4)
                        {
                            if (one.AdventureStudies != null)
                            {
                                mo.AdventureStudies = one.AdventureStudies;
                            }
                            else
                            {
                                mo.AdventureStudies = "";
                            }
                            if (one.Understandingofconcepts != null)
                            {
                                mo.Understandingofconcepts = one.Understandingofconcepts;
                            }
                            else
                            {
                                mo.Understandingofconcepts = "";
                            }
                            if (one.Activity != null)
                            {
                                mo.Activity = one.Activity;
                            }
                            else
                            {
                                mo.Activity = "";
                            }

                            if (one.Oral != null)
                            {
                                mo.Oral = one.Oral;
                            }
                            else
                            {
                                mo.Oral = "";
                            }
                        }

                        else if (model.SubjectId == 5)
                        {
                            if (one.Rhymes != null)
                            {
                                mo.Rhymes = one.Rhymes;
                            }
                            else
                            {
                                mo.Rhymes = "";
                            }
                            if (one.ComputerSkills != null)
                            {
                                mo.ComputerSkills = one.ComputerSkills;
                            }
                            else
                            {
                                mo.ComputerSkills = "";
                            }
                            if (one.GeneralStudies != null)
                            {
                                mo.GeneralStudies = one.GeneralStudies;
                            }
                            else
                            {
                                mo.GeneralStudies = "";
                            }
                            if (one.StoryTelling != null)
                            {
                                mo.StoryTelling = one.StoryTelling;
                            }
                            else
                            {
                                mo.StoryTelling = "";
                            }
                            if (one.PatternWriting != null)
                            {
                                mo.PatternWriting = one.PatternWriting;
                            }
                            else
                            {
                                mo.PatternWriting = "";
                            }
                            if (one.SmartClassPerformance != null)
                            {
                                mo.SmartClassPerformance = one.SmartClassPerformance;
                            }
                            else
                            {
                                mo.SmartClassPerformance = "";
                            }
                            if (one.ArtandCraft != null)
                            {
                                mo.ArtandCraft = one.ArtandCraft;
                            }
                            else
                            {
                                mo.ArtandCraft = "";
                            }
                            if (one.Handwriting != null)
                            {
                                mo.Handwriting = one.Handwriting;
                            }
                            else
                            {
                                mo.Handwriting = "";
                            }
                            if (one.PhysicalEducation != null)
                            {
                                mo.PhysicalEducation = one.PhysicalEducation;
                            }
                            else
                            {
                                mo.PhysicalEducation = "";
                            }
                            if (one.MusicandDance != null)
                            {
                                mo.MusicandDance = one.MusicandDance;
                            }
                            else
                            {
                                mo.MusicandDance = "";
                            }
                            if (one.CommunicationSkills != null)
                            {
                                mo.CommunicationSkills = one.CommunicationSkills;
                            }
                            else
                            {
                                mo.CommunicationSkills = "";
                            }
                            if (one.Worksheets != null)
                            {
                                mo.Worksheets = one.Worksheets;
                            }
                            else
                            {
                                mo.Worksheets = "";
                            }
                            if (one.InformationSheets != null)
                            {
                                mo.InformationSheets = one.InformationSheets;
                            }
                            else
                            {
                                mo.InformationSheets = "";
                            }
                        }
                        else if (model.SubjectId == 6)
                        {
                            if (one.Courteousness != null)
                            {
                                mo.Courteousness = one.Courteousness;
                            }
                            else
                            {
                                mo.Courteousness = "";
                            }
                            if (one.Relationship != null)
                            {
                                mo.Relationship = one.Relationship;
                            }
                            else
                            {
                                mo.Relationship = "";
                            }
                            if (one.Confidence != null)
                            {
                                mo.Confidence = one.Confidence;
                            }
                            else
                            {
                                mo.Confidence = "";
                            }
                            if (one.CareOfBelongings != null)
                            {
                                mo.CareOfBelongings = one.CareOfBelongings;
                            }
                            else
                            {
                                mo.CareOfBelongings = "";
                            }
                            if (one.Neatness != null)
                            {
                                mo.Neatness = one.Neatness;
                            }
                            else
                            {
                                mo.Neatness = "";
                            }
                            if (one.Regularity != null)
                            {
                                mo.Regularity = one.Regularity;
                            }
                            else
                            {
                                mo.Regularity = "";
                            }
                            if (one.Punctuality != null)
                            {
                                mo.Punctuality = one.Punctuality;
                            }
                            else
                            {
                                mo.Punctuality = "";
                            }
                            if (one.Initiative != null)
                            {
                                mo.Initiative = one.Initiative;
                            }
                            else
                            {
                                mo.Initiative = "";
                            }
                            if (one.SharingandCaring != null)
                            {
                                mo.SharingandCaring = one.SharingandCaring;
                            }
                            else
                            {
                                mo.SharingandCaring = "";
                            }
                            if (one.RespectForOthersProperty != null)
                            {
                                mo.RespectForOthersProperty = one.RespectForOthersProperty;
                            }
                            else
                            {
                                mo.RespectForOthersProperty = "";
                            }
                            if (one.Concentration != null)
                            {
                                mo.Concentration = one.Concentration;
                            }
                            else
                            {
                                mo.Concentration = "";
                            }
                            if (one.Cleanliness != null)
                            {
                                mo.Cleanliness = one.Cleanliness;
                            }
                            else
                            {
                                mo.Cleanliness = "";
                            }
                            if (one.GeneralConduct != null)
                            {
                                mo.GeneralConduct = one.GeneralConduct;
                            }
                            else
                            {
                                mo.GeneralConduct = "";
                            }
                            if (one.LeadershipSkills != null)
                            {
                                mo.LeadershipSkills = one.LeadershipSkills;
                            }
                            else
                            {
                                mo.LeadershipSkills = "";
                            }
                        }
                        else if (model.SubjectId == 7)
                        {
                            if (one.Height != null)
                            {
                                mo.Height = one.Height;
                            }
                            else
                            {
                                mo.Height = "";
                            }
                            if (one.Weight != null)
                            {
                                mo.Weight = one.Weight;
                            }
                            else
                            {
                                mo.Weight = "";
                            }
                            if (one.NoOfWorkingDays != null)
                            {
                                mo.NoOfWorkingDays = one.NoOfWorkingDays;
                            }
                            else
                            {
                                mo.NoOfWorkingDays = "";
                            }
                            if (one.DaysAttended != null)
                            {
                                mo.DaysAttended = one.DaysAttended;
                            }
                            else
                            {
                                mo.DaysAttended = "";
                            }
                        }
                        else if (model.SubjectId == 8)
                        {
                            if (one.Remark != null)
                            {
                                mo.Remark = one.Remark;
                            }
                            else
                            {
                                mo.Remark = "";
                            }
                        }

                    }
                    else
                    {

                        mo.Rhymes = "";
                        mo.ComputerSkills = "";
                        mo.GeneralStudies = "";
                        mo.StoryTelling = "";
                        mo.PatternWriting = "";
                        mo.SmartClassPerformance = "";
                        mo.ArtandCraft = "";
                        mo.Handwriting = "";
                        mo.PhysicalEducation = "";
                        mo.MusicandDance = "";
                        mo.CommunicationSkills = "";
                        mo.Worksheets = "";
                        mo.InformationSheets = "";

                        mo.WritingSkills = "";
                        mo.ReadingSkills = "";
                        mo.SpeakingSkills = "";
                        mo.RecitingSkills = "";
                        mo.ListeningSkills = "";
                        mo.Oral = "";

                        //math
                        mo.LogicalandReasoning = "";
                        mo.Understandingofconcepts = "";
                        mo.MentalAbility = "";

                        //Environmental Studies

                        mo.AdventureStudies = "";
                        mo.Understandingofconcepts = "";
                        mo.Activity = "";

                    }
                    
                    // mo.StudentName = a1.StundentName;
                    sk_li.Add(mo);
                }
                model._Lkg_Ukg_Mo = sk_li;
            }


            return PartialView("~/Views/SpecialClass/_pv_StudentReadingSkillEntry_Others.cshtml", model);
        }
        
        [HttpPost]
        public object SubmitStudentSkill_Others(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";            
            try
            {
                foreach (var item in model._SkillModel)
                {

                    string currentYear = _Entities.TbAcademicYears.Where(X => X.CurrentYear == true).Select(X => X.AcademicYear).FirstOrDefault();



                    var one = _Entities.TbLkgUkgNurserySkills.Where(x => x.Year == currentYear && x.SchoolId == model.SchoolId  
                                                                       && x.StudentId == item.StudentId
                                                                       && x.ClassId == item.ClassId && x.DivisionId == model.DivisionId 
                                                                       && x.SubjectId == model.SubjectId && x.AssesmentId == model.AssesmentId 
                                                                       && x.IsActive == true).FirstOrDefault();
                    if (one == null)
                    {
                        var read = new TbLkgUkgNurserySkill();  

                        read.Year = currentYear;
                        read.SchoolId = model.SchoolId;
                        read.StudentId = item.StudentId;
                        read.ClassId = item.ClassId;
                        read.DivisionId = model.DivisionId;
                        read.AssesmentId = model.AssesmentId;
                        read.SubjectId = model.SubjectId;
                        read.Languages = item.Languages;
                        read.UserId = _user.UserId;

                        //English..........


                        if (item.Lkg_Ukg_ner.WritingSkills != null)
                        {                            
                          read.WritingSkills = item.Lkg_Ukg_ner.WritingSkills;                           
                        }


                        if (item.Lkg_Ukg_ner.ReadingSkills != null)
                        {                            
                          read.ReadingSkills = item.Lkg_Ukg_ner.ReadingSkills;                          
                        }

                        if (item.Lkg_Ukg_ner.SpeakingSkills != null)
                        {                           
                          read.SpeakingSkills = item.Lkg_Ukg_ner.SpeakingSkills;
                        }
                        if (item.Lkg_Ukg_ner.RecitingSkills != null)
                        {
                            read.RecitingSkills = item.Lkg_Ukg_ner.RecitingSkills;
                        }
                        if (item.Lkg_Ukg_ner.ListeningSkills != null)
                        {
                            read.ListeningSkills = item.Lkg_Ukg_ner.ListeningSkills;
                        }

                        // mathes... English

                        if (item.Lkg_Ukg_ner.Oral != null)
                        {
                            read.Oral = item.Lkg_Ukg_ner.Oral;
                        }

                        //

                        //Mathematics..........

                        if (item.Lkg_Ukg_ner.LogicalandReasoning != null)
                        {
                            read.LogicalandReasoning = item.Lkg_Ukg_ner.LogicalandReasoning;
                        }
                        if (item.Lkg_Ukg_ner.Understandingofconcepts != null)
                        {
                            read.Understandingofconcepts = item.Lkg_Ukg_ner.Understandingofconcepts;
                        }
                        if (item.Lkg_Ukg_ner.MentalAbility != null)
                        {
                            read.MentalAbility = item.Lkg_Ukg_ner.MentalAbility;
                        }

                        //Environmental Studies

                        if (item.Lkg_Ukg_ner.AdventureStudies != null)
                        {
                            read.AdventureStudies = item.Lkg_Ukg_ner.AdventureStudies;
                        }
                        if (item.Lkg_Ukg_ner.Activity != null)
                        {
                            read.Activity = item.Lkg_Ukg_ner.Activity;
                        }

                        //Co-scholastic Areas 
                        if (item.Lkg_Ukg_ner.Rhymes != null)
                        {
                            read.Rhymes = item.Lkg_Ukg_ner.Rhymes;
                        }
                        if (item.Lkg_Ukg_ner.ComputerSkills != null)
                        {
                            read.ComputerSkills = item.Lkg_Ukg_ner.ComputerSkills;
                        }
                        if (item.Lkg_Ukg_ner.GeneralStudies != null)
                        {
                            read.GeneralStudies = item.Lkg_Ukg_ner.GeneralStudies;
                        }
                        if (item.Lkg_Ukg_ner.StoryTelling != null)
                        {
                            read.StoryTelling = item.Lkg_Ukg_ner.StoryTelling;
                        }
                        if (item.Lkg_Ukg_ner.PatternWriting != null)
                        {
                            read.PatternWriting = item.Lkg_Ukg_ner.PatternWriting;
                        }
                        if (item.Lkg_Ukg_ner.SmartClassPerformance != null)
                        {
                            read.SmartClassPerformance = item.Lkg_Ukg_ner.SmartClassPerformance;
                        }
                        if (item.Lkg_Ukg_ner.ArtandCraft != null)
                        {
                            read.ArtandCraft = item.Lkg_Ukg_ner.ArtandCraft;
                        }
                        if (item.Lkg_Ukg_ner.Handwriting != null)
                        {
                            read.Handwriting = item.Lkg_Ukg_ner.Handwriting;
                        }
                        if (item.Lkg_Ukg_ner.PhysicalEducation != null)
                        {
                            read.PhysicalEducation = item.Lkg_Ukg_ner.PhysicalEducation;
                        }
                        if (item.Lkg_Ukg_ner.MusicandDance != null)
                        {
                            read.MusicandDance = item.Lkg_Ukg_ner.MusicandDance;
                        }
                        if (item.Lkg_Ukg_ner.CommunicationSkills != null)
                        {
                            read.CommunicationSkills = item.Lkg_Ukg_ner.CommunicationSkills;
                        }
                        if (item.Lkg_Ukg_ner.Worksheets != null)
                        {
                            read.Worksheets = item.Lkg_Ukg_ner.Worksheets;
                        }
                        if (item.Lkg_Ukg_ner.InformationSheets != null)
                        {
                            read.InformationSheets = item.Lkg_Ukg_ner.InformationSheets;
                        }


                        //PERSONAL ASSESSMENTS 
                        if (item.Lkg_Ukg_ner.Courteousness != null)
                        {
                            read.Courteousness = item.Lkg_Ukg_ner.Courteousness;
                        }
                        if (item.Lkg_Ukg_ner.Relationship != null)
                        {
                            read.Relationship = item.Lkg_Ukg_ner.Relationship;
                        }
                        if (item.Lkg_Ukg_ner.Confidence != null)
                        {
                            read.Confidence = item.Lkg_Ukg_ner.Confidence;
                        }
                        if (item.Lkg_Ukg_ner.CareOfBelongings != null)
                        {
                            read.CareOfBelongings = item.Lkg_Ukg_ner.CareOfBelongings;
                        }
                        if (item.Lkg_Ukg_ner.Neatness != null)
                        {
                            read.Neatness = item.Lkg_Ukg_ner.Neatness;
                        }
                        if (item.Lkg_Ukg_ner.Regularity != null)
                        {
                            read.Regularity = item.Lkg_Ukg_ner.Regularity;
                        }
                        if (item.Lkg_Ukg_ner.Punctuality != null)
                        {
                            read.Punctuality = item.Lkg_Ukg_ner.Punctuality;
                        }
                        if (item.Lkg_Ukg_ner.Initiative != null)
                        {
                            read.Initiative = item.Lkg_Ukg_ner.Initiative;
                        }
                        if (item.Lkg_Ukg_ner.SharingandCaring != null)
                        {
                            read.SharingandCaring = item.Lkg_Ukg_ner.SharingandCaring;
                        }
                        if (item.Lkg_Ukg_ner.RespectForOthersProperty != null)
                        {
                            read.RespectForOthersProperty = item.Lkg_Ukg_ner.RespectForOthersProperty;
                        }
                        if (item.Lkg_Ukg_ner.Concentration != null)
                        {
                            read.Concentration = item.Lkg_Ukg_ner.Concentration;
                        }
                        if (item.Lkg_Ukg_ner.Cleanliness != null)
                        {
                            read.Cleanliness = item.Lkg_Ukg_ner.Cleanliness;
                        }
                        if (item.Lkg_Ukg_ner.GeneralConduct != null)
                        {
                            read.GeneralConduct = item.Lkg_Ukg_ner.GeneralConduct;
                        }
                        if (item.Lkg_Ukg_ner.LeadershipSkills != null)
                        {
                            read.LeadershipSkills = item.Lkg_Ukg_ner.LeadershipSkills;
                        }

                        //Health
                        
                            if (item.Lkg_Ukg_ner.Height != null)
                            {
                            read.Height = item.Lkg_Ukg_ner.Height;
                            }
                            
                            if (item.Lkg_Ukg_ner.Weight != null)
                            {
                            read.Weight = item.Lkg_Ukg_ner.Weight;
                            }
                            
                            if (item.Lkg_Ukg_ner.NoOfWorkingDays != null)
                            {
                            read.NoOfWorkingDays = item.Lkg_Ukg_ner.NoOfWorkingDays;
                            }
                            
                            if (item.Lkg_Ukg_ner.DaysAttended != null)
                            {
                            read.DaysAttended = item.Lkg_Ukg_ner.DaysAttended;
                            }
                            if (item.Lkg_Ukg_ner.Remark != null)
                            {
                                read.Remark = item.Lkg_Ukg_ner.Remark;
                            }


                        read.IsActive = true;
                        read.TimeStamp = CurrentTime;

                        _Entities.TbLkgUkgNurserySkills.Add(read);
                        status = _Entities.SaveChanges() > 0;



                    }

                    else
                    {
                        one.UserId = _user.UserId;


                        //English

                        if (item.Lkg_Ukg_ner.WritingSkills != null)
                        {
                            if (item.Lkg_Ukg_ner.WritingSkills == "0")
                            {
                                one.WritingSkills = "";
                            }
                            else
                            {
                                one.WritingSkills = item.Lkg_Ukg_ner.WritingSkills;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.ReadingSkills != null)
                        {
                            if (item.Lkg_Ukg_ner.ReadingSkills == "0")
                            {
                                one.ReadingSkills = "";
                            }
                            else
                            {
                                one.ReadingSkills = item.Lkg_Ukg_ner.ReadingSkills;
                            }
                           
                        }
                        if (item.Lkg_Ukg_ner.SpeakingSkills != null)
                        {
                            if (item.Lkg_Ukg_ner.SpeakingSkills == "0")
                            {
                                one.SpeakingSkills = "";
                            }
                            else
                            {
                                one.SpeakingSkills = item.Lkg_Ukg_ner.SpeakingSkills;
                            }
                           
                        }
                        if (item.Lkg_Ukg_ner.RecitingSkills != null)
                        {
                            if (item.Lkg_Ukg_ner.RecitingSkills == "0")
                            {
                                one.RecitingSkills = "";
                            }
                            else
                            {
                                one.RecitingSkills = item.Lkg_Ukg_ner.RecitingSkills;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.ListeningSkills != null)
                        {
                            if (item.Lkg_Ukg_ner.ListeningSkills == "0")
                            {
                                one.ListeningSkills = "";
                            }
                            else
                            {
                                one.ListeningSkills = item.Lkg_Ukg_ner.ListeningSkills;
                            }
                            
                        }

                        ///English, Malayalam

                        if (item.Lkg_Ukg_ner.Oral != null)
                        {
                            if (item.Lkg_Ukg_ner.Oral == "0")
                            {
                                one.Oral = "";
                            }
                            else
                            {
                                one.Oral = item.Lkg_Ukg_ner.Oral;
                            }
                            
                        }

                        //Mathematics..........

                        if (item.Lkg_Ukg_ner.LogicalandReasoning != null)
                        {
                            if (item.Lkg_Ukg_ner.LogicalandReasoning == "0")
                            {
                                one.LogicalandReasoning = "";
                            }
                            else
                            {
                                one.LogicalandReasoning = item.Lkg_Ukg_ner.LogicalandReasoning;
                            }
                            

                        }
                        if (item.Lkg_Ukg_ner.Understandingofconcepts != null)
                        {
                            if (item.Lkg_Ukg_ner.Understandingofconcepts == "0")
                            {
                                one.Understandingofconcepts = "";
                            }
                            else
                            {
                                one.Understandingofconcepts = item.Lkg_Ukg_ner.Understandingofconcepts;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.MentalAbility != null)
                        {
                            if (item.Lkg_Ukg_ner.MentalAbility == "0")
                            {
                                one.MentalAbility = "";
                            }
                            else
                            {
                                one.MentalAbility = item.Lkg_Ukg_ner.MentalAbility;
                            }
                            
                        }

                        //Environmental Studies

                        if (item.Lkg_Ukg_ner.AdventureStudies != null)
                        {
                            if (item.Lkg_Ukg_ner.AdventureStudies == "0")
                            {
                                one.AdventureStudies = "";
                            }
                            else
                            {
                                one.AdventureStudies = item.Lkg_Ukg_ner.AdventureStudies;
                            }
                            
                        }                        
                        if (item.Lkg_Ukg_ner.Activity != null)
                        {
                            if (item.Lkg_Ukg_ner.Activity == "0")
                            {
                                one.Activity = "";
                            }
                            else
                            {
                                one.Activity = item.Lkg_Ukg_ner.Activity;
                            }
                            
                        }

                        //Co-scholastic Areas 
                        if (item.Lkg_Ukg_ner.Rhymes != null)
                        {
                            if (item.Lkg_Ukg_ner.Rhymes == "0")
                            {
                                one.Rhymes = "";
                            }
                            else
                            {
                                one.Rhymes = item.Lkg_Ukg_ner.Rhymes;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.ComputerSkills != null)
                        {
                            if (item.Lkg_Ukg_ner.ComputerSkills == "0")
                            {
                                one.ComputerSkills = "";
                            }
                            else
                            {
                                one.ComputerSkills = item.Lkg_Ukg_ner.ComputerSkills;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.GeneralStudies != null)
                        {
                            if (item.Lkg_Ukg_ner.GeneralStudies == "0")
                            {
                                one.GeneralStudies = "";
                            }
                            else
                            {
                                one.GeneralStudies = item.Lkg_Ukg_ner.GeneralStudies;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.StoryTelling != null)
                        {
                            if (item.Lkg_Ukg_ner.StoryTelling == "0")
                            {
                                one.StoryTelling = "";
                            }
                            else
                            {
                                one.StoryTelling = item.Lkg_Ukg_ner.StoryTelling;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.PatternWriting != null)
                        {
                            if (item.Lkg_Ukg_ner.PatternWriting == "0")
                            {
                                one.PatternWriting = "";
                            }
                            else
                            {
                                one.PatternWriting = item.Lkg_Ukg_ner.PatternWriting;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.SmartClassPerformance != null)
                        {
                            if (item.Lkg_Ukg_ner.SmartClassPerformance == "0")
                            {
                                one.SmartClassPerformance = "";
                            }
                            else
                            {
                                one.SmartClassPerformance = item.Lkg_Ukg_ner.SmartClassPerformance;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.ArtandCraft != null)
                        {
                            if (item.Lkg_Ukg_ner.ArtandCraft == "0")
                            {
                                one.ArtandCraft = "";
                            }
                            else
                            {
                                one.ArtandCraft = item.Lkg_Ukg_ner.ArtandCraft;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Handwriting != null)
                        {
                            if (item.Lkg_Ukg_ner.Handwriting == "0")
                            {
                                one.Handwriting = "";
                            }
                            else
                            {
                                one.Handwriting = item.Lkg_Ukg_ner.Handwriting;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.PhysicalEducation != null)
                        {
                            if (item.Lkg_Ukg_ner.PhysicalEducation == "0")
                            {
                                one.PhysicalEducation = "";
                            }
                            else
                            {
                                 
                                one.PhysicalEducation = item.Lkg_Ukg_ner.PhysicalEducation;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.MusicandDance != null)
                        {
                            if (item.Lkg_Ukg_ner.MusicandDance == "0")
                            {
                                one.MusicandDance = "";
                            }
                            else
                            {

                                one.MusicandDance = item.Lkg_Ukg_ner.MusicandDance;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.CommunicationSkills != null)
                        {
                            if (item.Lkg_Ukg_ner.CommunicationSkills == "0")
                            {
                                one.CommunicationSkills = "";
                            }
                            else
                            {

                                one.CommunicationSkills = item.Lkg_Ukg_ner.CommunicationSkills;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Worksheets != null)
                        {
                            if (item.Lkg_Ukg_ner.Worksheets == "0")
                            {
                                one.Worksheets = "";
                            }
                            else
                            {

                                one.Worksheets = item.Lkg_Ukg_ner.Worksheets;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.InformationSheets != null)
                        {
                            if (item.Lkg_Ukg_ner.InformationSheets == "0")
                            {
                                one.InformationSheets = "";
                            }
                            else
                            {

                                one.InformationSheets = item.Lkg_Ukg_ner.InformationSheets;
                            }
                            
                        }


                        //PERSONAL ASSESSMENTS 
                        if (item.Lkg_Ukg_ner.Courteousness != null)
                        {
                            if (item.Lkg_Ukg_ner.Courteousness == "0")
                            {
                                one.Courteousness = "";
                            }
                            else
                            {

                                one.Courteousness = item.Lkg_Ukg_ner.Courteousness;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Relationship != null)
                        {
                            if (item.Lkg_Ukg_ner.Relationship == "0")
                            {
                                one.Relationship = "";
                            }
                            else
                            {

                                one.Relationship = item.Lkg_Ukg_ner.Relationship;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Confidence != null)
                        {
                            if (item.Lkg_Ukg_ner.Confidence == "0")
                            {
                                one.Confidence = "";
                            }
                            else
                            {

                                one.Confidence = item.Lkg_Ukg_ner.Confidence;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.CareOfBelongings != null)
                        {
                            if (item.Lkg_Ukg_ner.CareOfBelongings == "0")
                            {
                                one.CareOfBelongings = "";
                            }
                            else
                            {

                                one.CareOfBelongings = item.Lkg_Ukg_ner.CareOfBelongings;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Neatness != null)
                        {
                            if (item.Lkg_Ukg_ner.Neatness == "0")
                            {
                                one.Neatness = "";
                            }
                            else
                            {

                                one.Neatness = item.Lkg_Ukg_ner.Neatness;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Regularity != null)
                        {
                            if (item.Lkg_Ukg_ner.Regularity == "0")
                            {
                                one.Regularity = "";
                            }
                            else
                            {

                                one.Regularity = item.Lkg_Ukg_ner.Regularity;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Punctuality != null)
                        {
                            if (item.Lkg_Ukg_ner.Punctuality == "0")
                            {
                                one.Punctuality = "";
                            }
                            else
                            {

                                one.Punctuality = item.Lkg_Ukg_ner.Punctuality;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Initiative != null)
                        {
                            if (item.Lkg_Ukg_ner.Initiative == "0")
                            {
                                one.Initiative = "";
                            }
                            else
                            {

                                one.Initiative = item.Lkg_Ukg_ner.Initiative;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.SharingandCaring != null)
                        {
                            if (item.Lkg_Ukg_ner.SharingandCaring == "0")
                            {
                                one.SharingandCaring = "";
                            }
                            else
                            {

                                one.SharingandCaring = item.Lkg_Ukg_ner.SharingandCaring;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.RespectForOthersProperty != null)
                        {
                            if (item.Lkg_Ukg_ner.RespectForOthersProperty == "0")
                            {
                                one.RespectForOthersProperty = "";
                            }
                            else
                            {

                                one.RespectForOthersProperty = item.Lkg_Ukg_ner.RespectForOthersProperty;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Concentration != null)
                        {
                            if (item.Lkg_Ukg_ner.Concentration == "0")
                            {
                                one.Concentration = "";
                            }
                            else
                            {

                                one.Concentration = item.Lkg_Ukg_ner.Concentration;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.Cleanliness != null)
                        {
                            if (item.Lkg_Ukg_ner.Cleanliness == "0")
                            {
                                one.Cleanliness = "";
                            }
                            else
                            {

                                one.Cleanliness = item.Lkg_Ukg_ner.Cleanliness;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.GeneralConduct != null)
                        {
                            if (item.Lkg_Ukg_ner.GeneralConduct == "0")
                            {
                                one.GeneralConduct = "";
                            }
                            else
                            {

                                one.GeneralConduct = item.Lkg_Ukg_ner.GeneralConduct;
                            }
                            
                        }
                        if (item.Lkg_Ukg_ner.LeadershipSkills != null)
                        {
                            if (item.Lkg_Ukg_ner.LeadershipSkills == "0")
                            {
                                one.LeadershipSkills = "";
                            }
                            else
                            {

                                one.LeadershipSkills = item.Lkg_Ukg_ner.LeadershipSkills;
                            }
                            
                        }

                        //health

                        if (item.Lkg_Ukg_ner.Height != null)
                        {
                            if (item.Lkg_Ukg_ner.Height == "0")
                            {
                                one.Height = "";
                            }
                            else
                            {

                                one.Height = item.Lkg_Ukg_ner.Height;
                            }
                            
                        }

                        if (item.Lkg_Ukg_ner.Weight != null)
                        {
                            if (item.Lkg_Ukg_ner.Weight == "0")
                            {
                                one.Weight = "";
                            }
                            else
                            {

                                one.Weight = item.Lkg_Ukg_ner.Weight;
                            }
                            
                        }

                        if (item.Lkg_Ukg_ner.NoOfWorkingDays != null)
                        {
                            if (item.Lkg_Ukg_ner.NoOfWorkingDays == "0")
                            {
                                one.NoOfWorkingDays = "";
                            }
                            else
                            {

                                one.NoOfWorkingDays = item.Lkg_Ukg_ner.NoOfWorkingDays;
                            }
                            
                        }

                        if (item.Lkg_Ukg_ner.DaysAttended != null)
                        {
                            if (item.Lkg_Ukg_ner.DaysAttended == "0")
                            {
                                one.DaysAttended = "";
                            }
                            else
                            {

                                one.DaysAttended = item.Lkg_Ukg_ner.DaysAttended;
                            }
                            
                        }

                        //Remarks

                        if (item.Lkg_Ukg_ner.Remark != null)
                        {
                            if (item.Lkg_Ukg_ner.Remark == "0")
                            {
                                one.Remark = "";
                            }
                            else
                            {

                                one.Remark = item.Lkg_Ukg_ner.Remark;
                            }
                            
                        }

                        status = _Entities.SaveChanges() > 0;

                    }
                    

                }
                _Entities.SaveChanges();
                return Json(new { status = true, msg = "Successful" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = ex.Message });
            }
        }

        public IActionResult NonEducationalHome_Others()
        {
            var model = new SubjectScoreModel
            {
                SchoolId = _user.SchoolId,
                UserId = _user.UserId,
                IsAdminCheck = Convert.ToInt32(HttpContext.Session.GetInt32("isAdmin"))
            };
            ViewBag.classpreschool = _dropdown.GetClass_Preschool(model.SchoolId);
            return View(model);
        }


        public IActionResult VProgressCardHome_Others()
        {
            var model = new SubjectScoreModel
            {
                SchoolId = _user.SchoolId,
                UserId = _user.UserId,
                IsAdminCheck = Convert.ToInt32(HttpContext.Session.GetInt32("isAdmin"))
            };
            ViewBag.Classlist = _dropdown.GetClass_Preschool(model.SchoolId);

            return View(model);
        }

        public PartialViewResult ViewAll_V_ClassStudents_Others(string id)
        {
            VProgressCardList model = new VProgressCardList();
           
            string[] splitData = id.Split('~');
            long ClassId = Convert.ToInt64(splitData[0]);
            long DivissionId = Convert.ToInt64(splitData[1]);

            model.SchoolId = _user.SchoolId;

            //model.TempMo1 = TempMo1.Where(x => x.classId == ClassId && x.divisionId == DivissionId && x.schoolId == _user.SchoolId).OrderBy(x => x.studentName).ToList();

            var results = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == DivissionId && x.SchoolId == _user.SchoolId && x.IsActive == true).OrderBy(x => x.StundentName).ToList();
            List<StudentModel> li = new List<StudentModel>(); 
            foreach (var a1 in results)
            {
                StudentModel st = new StudentModel();
                st.studentId = a1.StudentId;
                st.studentName = a1.StundentName;
                li.Add(st);
            }
            
            model.TempMo1 = li;

            return PartialView("~/Views/SpecialClass/_pv_StudentListForProgressCard_Others.cshtml", model);
        }

        public IActionResult VProgressCard_others(string ids)
        {
            string[] conte = ids.Split('~');
            long id = Convert.ToInt64(conte[0]);
            string static_Date = conte[1];

            string currentYear = _Entities.TbAcademicYears.Where(X => X.CurrentYear == true).Select(X => X.AcademicYear).FirstOrDefault();

            long studentId = Convert.ToInt64(id);
            long schoolId = _user.SchoolId;
            var data = _Entities.TbLkgUkgNurserySkills.Where(x => x.SchoolId == schoolId && x.StudentId == studentId && x.Year == currentYear && x.IsActive == true).ToList();


            /// this to starting.............

            

            Lkg_Ukg_Nu_ReportModel mo = new Lkg_Ukg_Nu_ReportModel();


            Title_Report Titl = new Title_Report();

            English_Report Eng = new English_Report();
            Hindi_Report Hin = new Hindi_Report();
            Mathematics_Report Mat = new Mathematics_Report();
            EnvironmentalStudies_Report Env = new EnvironmentalStudies_Report();
            SCHOLASTICAREAS_Report SchoL = new SCHOLASTICAREAS_Report();
            PERSONALASSESSMENTS_Report Pers = new PERSONALASSESSMENTS_Report();
            HealthStatus_Report Heal = new HealthStatus_Report();
            Attendance_Report Att = new Attendance_Report();

            Term_Remarks_Report Tem = new Term_Remarks_Report();

            //model.StudentId = Convert.ToInt64(id);
            var studentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId).FirstOrDefault();
            Titl.ReportName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            Titl.SchoolName = studentDetails.School.SchoolName;
            Titl.SchoolLogo = "http://satluj.schoolman.in" + studentDetails.School.FilePath;
            //model.AcademicYear = studentDetails.tb_Class.tb_AcademicYear.AcademicYear;
            Titl.StudentProfile = "http://satluj.schoolman.in" + studentDetails.FilePath;
            //model.StudentName = studentDetails.StundentName;
            //model.ClassDivision = studentDetails.tb_Class.Class + " " + studentDetails.tb_Division.Division;
            //model.RollNo = studentDetails.ClasssNumber;
            //model.AdmissionNo = studentDetails.StudentSpecialId;
            Titl.Static_Date = static_Date;




            var data_Student_Table = _Entities.TbStudents.Where(x => x.SchoolId == schoolId && x.StudentId == studentId).FirstOrDefault();

            if (data_Student_Table != null)
            {
                Titl.Name = data_Student_Table.StundentName;
                Titl.FatherName = _Entities.TbParents.Where(x => x.ParentId == data_Student_Table.ParentId).Select(x => x.ParentName).FirstOrDefault();
                Titl.MotherName = _Entities.TbParents.Where(x => x.ParentId == data_Student_Table.ParentId).Select(x => x.MotherName).FirstOrDefault();
                Titl.AdmissionNo = data_Student_Table.StudentSpecialId;
                Titl.Class = _Entities.TbClasses.Where(x => x.ClassId == data_Student_Table.ClassId).Select(x => x.Class).FirstOrDefault();
                Titl.Division = _Entities.TbDivisions.Where(x => x.DivisionId == data_Student_Table.DivisionId).Select(x => x.Division).FirstOrDefault();
                DateTime Dt = Convert.ToDateTime(data_Student_Table.Dob);
                string DD = Dt.Day.ToString();
                string MM = Dt.Month.ToString();
                string YYYY = Dt.Year.ToString();
                if (Convert.ToInt32(DD) < 10 )
                {
                    DD = "0" + DD;
                }
                if (Convert.ToInt32(MM) < 10)
                {
                    MM = "0" + MM;
                }

                Titl.DateOfBirth = DD + "-" + MM + "-" + YYYY;
            }
            else
            {
                Titl.Name = "xxxx";
                Titl.FatherName = "xxxx";
                Titl.MotherName = "xxxx";
                Titl.AdmissionNo = "xxxx";
                Titl.Class = "xxxx";
                Titl.DateOfBirth = "xxxx";

            }

            //2019-03-26 00:00:00.000 11-08-2000
            //DateTime Dt = Convert.ToDateTime("2019-03-26 00:00:00.000");
           

            Titl.Session = currentYear;
           
            //...................

            //English Report
            Eng.WritingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.WritingSkills).FirstOrDefault();
            Eng.ReadingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.ReadingSkills).FirstOrDefault();
            Eng.SpeakingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.SpeakingSkills).FirstOrDefault();
            Eng.RecitingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.RecitingSkills).FirstOrDefault();
            Eng.ListeningSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.ListeningSkills).FirstOrDefault();
            Eng.Oral_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.Oral).FirstOrDefault();

            Eng.WritingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.WritingSkills).FirstOrDefault();
            Eng.ReadingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.ReadingSkills).FirstOrDefault();
            Eng.SpeakingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.SpeakingSkills).FirstOrDefault();
            Eng.RecitingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.RecitingSkills).FirstOrDefault();
            Eng.ListeningSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.ListeningSkills).FirstOrDefault();
            Eng.Oral_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.Oral).FirstOrDefault();

            Eng.WritingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.WritingSkills).FirstOrDefault();
            Eng.ReadingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.ReadingSkills).FirstOrDefault();
            Eng.SpeakingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.SpeakingSkills).FirstOrDefault();
            Eng.RecitingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.RecitingSkills).FirstOrDefault();
            Eng.ListeningSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.ListeningSkills).FirstOrDefault();
            Eng.Oral_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.Oral).FirstOrDefault();


            //Hindi Report
            Hin.WritingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.WritingSkills).FirstOrDefault();
            Hin.ReadingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.ReadingSkills).FirstOrDefault();
            Hin.SpeakingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.SpeakingSkills).FirstOrDefault();
            Hin.RecitingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.RecitingSkills).FirstOrDefault();
            Hin.ListeningSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.ListeningSkills).FirstOrDefault();
            Hin.Oral_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.Oral).FirstOrDefault();

            Hin.WritingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.WritingSkills).FirstOrDefault();
            Hin.ReadingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.ReadingSkills).FirstOrDefault();
            Hin.SpeakingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.SpeakingSkills).FirstOrDefault();
            Hin.RecitingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.RecitingSkills).FirstOrDefault();
            Hin.ListeningSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.ListeningSkills).FirstOrDefault();
            Hin.Oral_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.Oral).FirstOrDefault();

            Hin.WritingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.WritingSkills).FirstOrDefault();
            Hin.ReadingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.ReadingSkills).FirstOrDefault();
            Hin.SpeakingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.SpeakingSkills).FirstOrDefault();
            Hin.RecitingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.RecitingSkills).FirstOrDefault();
            Hin.ListeningSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.ListeningSkills).FirstOrDefault();
            Hin.Oral_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.Oral).FirstOrDefault();

            //Maths.......

            Mat.LogicalandReasoning_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.LogicalandReasoning).FirstOrDefault();
            Mat.Understandingofconcepts_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.Understandingofconcepts).FirstOrDefault();
            Mat.MentalAbility_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.MentalAbility).FirstOrDefault();
            Mat.Oral_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.Oral).FirstOrDefault();

            Mat.LogicalandReasoning_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.LogicalandReasoning).FirstOrDefault();
            Mat.Understandingofconcepts_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.Understandingofconcepts).FirstOrDefault();
            Mat.MentalAbility_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.MentalAbility).FirstOrDefault();
            Mat.Oral_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.Oral).FirstOrDefault();

            Mat.LogicalandReasoning_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.LogicalandReasoning).FirstOrDefault();
            Mat.Understandingofconcepts_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.Understandingofconcepts).FirstOrDefault();
            Mat.MentalAbility_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.MentalAbility).FirstOrDefault();
            Mat.Oral_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.Oral).FirstOrDefault();

            //EnvironmentalStudies
            Env.AdventureStudies_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.AdventureStudies).FirstOrDefault();
            Env.Understandingofconcepts_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.Understandingofconcepts).FirstOrDefault();
            Env.Activity_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.Activity).FirstOrDefault();
            Env.Oral_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.Oral).FirstOrDefault();


            Env.AdventureStudies_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.AdventureStudies).FirstOrDefault();
            Env.Understandingofconcepts_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.Understandingofconcepts).FirstOrDefault();
            Env.Activity_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.Activity).FirstOrDefault();
            Env.Oral_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.Oral).FirstOrDefault();


            Env.AdventureStudies_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.AdventureStudies).FirstOrDefault();
            Env.Understandingofconcepts_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.Understandingofconcepts).FirstOrDefault();
            Env.Activity_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.Activity).FirstOrDefault();
            Env.Oral_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.Oral).FirstOrDefault();

            ///SCHOLASTICAREAS_Report

            SchoL.Rhymes_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.Rhymes).FirstOrDefault();
            SchoL.ComputerSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.ComputerSkills).FirstOrDefault();
            SchoL.GeneralStudies_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.GeneralStudies).FirstOrDefault();
            SchoL.StoryTelling_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.StoryTelling).FirstOrDefault();
            SchoL.PatternWriting_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.PatternWriting).FirstOrDefault();
            SchoL.SmartClassPerformance_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.SmartClassPerformance).FirstOrDefault();
            SchoL.ArtandCraft_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.ArtandCraft).FirstOrDefault();
            SchoL.Handwriting_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.Handwriting).FirstOrDefault();
            SchoL.PhysicalEducation_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.PhysicalEducation).FirstOrDefault();
            SchoL.MusicandDance_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.MusicandDance).FirstOrDefault();
            SchoL.CommunicationSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.CommunicationSkills).FirstOrDefault();
            SchoL.Worksheets_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.Worksheets).FirstOrDefault();
            SchoL.InformationSheets_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.InformationSheets).FirstOrDefault();


            SchoL.Rhymes_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.Rhymes).FirstOrDefault();
            SchoL.ComputerSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.ComputerSkills).FirstOrDefault();
            SchoL.GeneralStudies_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.GeneralStudies).FirstOrDefault();
            SchoL.StoryTelling_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.StoryTelling).FirstOrDefault();
            SchoL.PatternWriting_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.PatternWriting).FirstOrDefault();
            SchoL.SmartClassPerformance_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.SmartClassPerformance).FirstOrDefault();
            SchoL.ArtandCraft_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.ArtandCraft).FirstOrDefault();
            SchoL.Handwriting_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.Handwriting).FirstOrDefault();
            SchoL.PhysicalEducation_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.PhysicalEducation).FirstOrDefault();
            SchoL.MusicandDance_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.MusicandDance).FirstOrDefault();
            SchoL.CommunicationSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.CommunicationSkills).FirstOrDefault();
            SchoL.Worksheets_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.Worksheets).FirstOrDefault();
            SchoL.InformationSheets_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.InformationSheets).FirstOrDefault();


            SchoL.Rhymes_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.Rhymes).FirstOrDefault();
            SchoL.ComputerSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.ComputerSkills).FirstOrDefault();
            SchoL.GeneralStudies_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.GeneralStudies).FirstOrDefault();
            SchoL.StoryTelling_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.StoryTelling).FirstOrDefault();
            SchoL.PatternWriting_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.PatternWriting).FirstOrDefault();
            SchoL.SmartClassPerformance_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.SmartClassPerformance).FirstOrDefault();
            SchoL.ArtandCraft_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.ArtandCraft).FirstOrDefault();
            SchoL.Handwriting_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.Handwriting).FirstOrDefault();
            SchoL.PhysicalEducation_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.PhysicalEducation).FirstOrDefault();
            SchoL.MusicandDance_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.MusicandDance).FirstOrDefault();
            SchoL.CommunicationSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.CommunicationSkills).FirstOrDefault();
            SchoL.Worksheets_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.Worksheets).FirstOrDefault();
            SchoL.InformationSheets_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.InformationSheets).FirstOrDefault();

            //PERSONALASSESSMENTS_Report...

            Pers.Courteousness_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Courteousness).FirstOrDefault();
            Pers.Relationship_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Relationship).FirstOrDefault();
            Pers.Confidence_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Confidence).FirstOrDefault();
            Pers.CareOfBelongings_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.CareOfBelongings).FirstOrDefault();
            Pers.Neatness_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Neatness).FirstOrDefault();
            Pers.Regularity_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Regularity).FirstOrDefault();
            Pers.Punctuality_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Punctuality).FirstOrDefault();
            Pers.Initiative_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Initiative).FirstOrDefault();
            Pers.SharingandCaring_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.SharingandCaring).FirstOrDefault();
            Pers.RespectForOthersProperty_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.RespectForOthersProperty).FirstOrDefault();
            Pers.Concentration_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Concentration).FirstOrDefault();
            Pers.Cleanliness_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Cleanliness).FirstOrDefault();
            Pers.GeneralConduct_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.GeneralConduct).FirstOrDefault();
            Pers.LeadershipSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.LeadershipSkills).FirstOrDefault();


            Pers.Courteousness_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Courteousness).FirstOrDefault();
            Pers.Relationship_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Relationship).FirstOrDefault();
            Pers.Confidence_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Confidence).FirstOrDefault();
            Pers.CareOfBelongings_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.CareOfBelongings).FirstOrDefault();
            Pers.Neatness_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Neatness).FirstOrDefault();
            Pers.Regularity_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Regularity).FirstOrDefault();
            Pers.Punctuality_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Punctuality).FirstOrDefault();
            Pers.Initiative_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Initiative).FirstOrDefault();
            Pers.SharingandCaring_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.SharingandCaring).FirstOrDefault();
            Pers.RespectForOthersProperty_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.RespectForOthersProperty).FirstOrDefault();
            Pers.Concentration_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Concentration).FirstOrDefault();
            Pers.Cleanliness_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Cleanliness).FirstOrDefault();
            Pers.GeneralConduct_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.GeneralConduct).FirstOrDefault();
            Pers.LeadershipSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.LeadershipSkills).FirstOrDefault();


            Pers.Courteousness_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Courteousness).FirstOrDefault();
            Pers.Relationship_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Relationship).FirstOrDefault();
            Pers.Confidence_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Confidence).FirstOrDefault();
            Pers.CareOfBelongings_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.CareOfBelongings).FirstOrDefault();
            Pers.Neatness_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Neatness).FirstOrDefault();
            Pers.Regularity_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Regularity).FirstOrDefault();
            Pers.Punctuality_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Punctuality).FirstOrDefault();
            Pers.Initiative_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Initiative).FirstOrDefault();
            Pers.SharingandCaring_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.SharingandCaring).FirstOrDefault();
            Pers.RespectForOthersProperty_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.RespectForOthersProperty).FirstOrDefault();
            Pers.Concentration_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Concentration).FirstOrDefault();
            Pers.Cleanliness_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Cleanliness).FirstOrDefault();
            Pers.GeneralConduct_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.GeneralConduct).FirstOrDefault();
            Pers.LeadershipSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.LeadershipSkills).FirstOrDefault();

            //health
            Heal.Height_Cms_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Height).FirstOrDefault();
            Heal.Weight_Kg_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Weight).FirstOrDefault();

            Heal.Height_Cms_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Height).FirstOrDefault();
            Heal.Weight_Kg_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Weight).FirstOrDefault();

            Heal.Height_Cms_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Height).FirstOrDefault();
            Heal.Weight_Kg_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Weight).FirstOrDefault();

            //attentence

            Att.No_of_Working_Days_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.NoOfWorkingDays).FirstOrDefault();
            Att.Days_Attended_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.DaysAttended).FirstOrDefault();

            Att.No_of_Working_Days_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.NoOfWorkingDays).FirstOrDefault();
            Att.Days_Attended_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.DaysAttended).FirstOrDefault();

            Att.No_of_Working_Days_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.NoOfWorkingDays).FirstOrDefault();
            Att.Days_Attended_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.DaysAttended).FirstOrDefault();

            //Remarks

            Tem.Term_Remarks_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.Remark).FirstOrDefault();
            Tem.Term_Remarks_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.Remark).FirstOrDefault();
            Tem.Term_Remarks_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.Remark).FirstOrDefault();



            mo.English_Report = Eng;
            mo.Hindi_Report = Hin;
            mo.Mathematics_Report = Mat;
            mo.EnvironmentalStudies_Report = Env;
            mo.SCHOLASTICAREAS_Report = SchoL;
            mo.PERSONALASSESSMENTS_Report = Pers;
            mo.Title_Report = Titl;
            mo.HealthStatus_Report = Heal;
            mo.Attendance_Report = Att;
            mo.Term_Remarks_Report = Tem;

            return View(mo);
        }
        

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////
        ///////////ONE_FOUR---------------------------------------

        #region Comp
        public IActionResult AssesmentHome_ONE_FOUR()
        {
            SpecialClassModel model = new SpecialClassModel();
            model.SchoolId = _user.SchoolId;
            DropdownData dropdown = new DropdownData();
            ViewBag.PeriodList= dropdown.GetPeriodsLists(model.SchoolId);
            return View(model);
        }
        #endregion
        public IActionResult ReadingSkillsHome_ONE_FOUR()
        {
            var model = new SubjectScoreModel
            {
                SchoolId = _user.SchoolId,
                UserId = _user.UserId,
                IsAdminCheck = Convert.ToInt32(HttpContext.Session.GetInt32("isAdmin"))
            };
            ViewBag.ClassList = _dropdown
                    .GetClass_Preschool_One_Four(model.SchoolId)
                    .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList();

            return View(model);
        }


        [HttpGet]
        public PartialViewResult LoadAllStudentsWithSkillsMarks_ONE_FOUR(string id)
        {
            string[] data = id.Split('~');
            long ClassId = Convert.ToInt64(data[0]);
            long DivisionId = Convert.ToInt64(data[1]);
            long AssesmentId = Convert.ToInt32(data[2]);
            long SubjectId = Convert.ToInt64(data[3]);


            StudentSkillMarks model = new StudentSkillMarks();
            model.ClassId = ClassId;
            model.SchoolId = _user.SchoolId;
            model.DivisionId = DivisionId;
            model.AssesmentId = AssesmentId;
            model.SubjectId = SubjectId;

            model._SkillModel = new List<SkillModel>();
            //var studentList = TempMo1.Where(x => x.classId == ClassId && x.divisionId == DivisionId && x.schoolId == _user.SchoolId).OrderBy(x => x.studentName).ToList();

            var studentList = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == DivisionId && x.SchoolId == _user.SchoolId && x.IsActive).OrderBy(x => x.StundentName).ToList();

            if (studentList.Count > 0)
            {
                //List<SkillModel> sk_li = new List<SkillModel>();
                List<ONE_FOUR_SKILLS_MODEL> sk_li = new List<ONE_FOUR_SKILLS_MODEL>();
                string currentYear = _Entities.TbAcademicYears.Where(X => X.CurrentYear == true).Select(X => X.AcademicYear).FirstOrDefault();

                foreach (var a1 in studentList)
                {
                    //SkillModel mo = new SkillModel();
                    ONE_FOUR_SKILLS_MODEL mo = new ONE_FOUR_SKILLS_MODEL();

                    mo.StundentName = a1.StundentName;
                    mo.StudentId = a1.StudentId;
                    mo.ClassId = a1.ClassId;
                    var one = _Entities.TbOneFourSkills.Where(x => x.Year == currentYear && x.SchoolId == model.SchoolId
                                                                      && x.StudentId == a1.StudentId
                                                                      && x.ClassId == a1.ClassId && x.DivisionId == model.DivisionId
                                                                      && x.SubjectId == model.SubjectId && x.AssesmentId == model.AssesmentId
                                                                      && x.IsActive == true).FirstOrDefault();

                    //English..........

                    if (one != null)
                    {

                        if (model.SubjectId == 1) //Hindi
                        {
                            if (one.WritingSkills != null) ///////
                            {
                                mo.WritingSkills = one.WritingSkills;
                            }
                            else
                            {
                                mo.WritingSkills = "";
                            }
                            if (one.ReadingSkills != null)/////
                            {
                                mo.ReadingSkills = one.ReadingSkills;
                            }
                            else
                            {
                                mo.ReadingSkills = "";
                            }
                            if (one.ListeningSkills != null)//////
                            {
                                mo.ListeningSkills = one.ListeningSkills;
                            }
                            else
                            {
                                mo.ListeningSkills = "";
                            }
                            if (one.WeeklyAssessment != null)
                            {
                                mo.WeeklyAssessment = one.WeeklyAssessment;
                            }
                            else
                            {
                                mo.WeeklyAssessment = "";
                            }


                            if (one.SpeakingSkills != null)/////
                            {
                                mo.SpeakingSkills = one.SpeakingSkills;
                            }
                            else
                            {
                                mo.SpeakingSkills = "";
                            }
                           
                            
                            if (one.TerminalAssessment != null)
                            {
                                mo.TerminalAssessment = one.TerminalAssessment;
                            }
                            else
                            {
                                mo.TerminalAssessment = "";
                            }
                            if (one.AggregateGrade != null)
                            {
                                mo.AggregateGrade = one.AggregateGrade;
                            }
                            else
                            {
                                mo.AggregateGrade = "";
                            }
                            //

                        }

                        else if (model.SubjectId == 2)//English
                        {
                            if (one.WritingSkills != null) ///////
                            {
                                mo.WritingSkills = one.WritingSkills;
                            }
                            else
                            {
                                mo.WritingSkills = "";
                            }
                            if (one.ReadingSkills != null)/////
                            {
                                mo.ReadingSkills = one.ReadingSkills;
                            }
                            else
                            {
                                mo.ReadingSkills = "";
                            }
                            if (one.ListeningSkills != null)//////
                            {
                                mo.ListeningSkills = one.ListeningSkills;
                            }
                            else
                            {
                                mo.ListeningSkills = "";
                            }
                            if (one.WeeklyAssessment != null)
                            {
                                mo.WeeklyAssessment = one.WeeklyAssessment;
                            }
                            else
                            {
                                mo.WeeklyAssessment = "";
                            }


                            if (one.SpeakingSkills != null)/////
                            {
                                mo.SpeakingSkills = one.SpeakingSkills;
                            }
                            else
                            {
                                mo.SpeakingSkills = "";
                            }


                            if (one.TerminalAssessment != null)
                            {
                                mo.TerminalAssessment = one.TerminalAssessment;
                            }
                            else
                            {
                                mo.TerminalAssessment = "";
                            }
                            if (one.AggregateGrade != null)
                            {
                                mo.AggregateGrade = one.AggregateGrade;
                            }
                            else
                            {
                                mo.AggregateGrade = "";
                            }
                            //
                        }
                        else if (model.SubjectId == 3)//Punjabi
                        {
                            if (one.WritingSkills != null) ///////
                            {
                                mo.WritingSkills = one.WritingSkills;
                            }
                            else
                            {
                                mo.WritingSkills = "";
                            }
                            if (one.ReadingSkills != null)/////
                            {
                                mo.ReadingSkills = one.ReadingSkills;
                            }
                            else
                            {
                                mo.ReadingSkills = "";
                            }
                            if (one.ListeningSkills != null)//////
                            {
                                mo.ListeningSkills = one.ListeningSkills;
                            }
                            else
                            {
                                mo.ListeningSkills = "";
                            }
                            if (one.WeeklyAssessment != null)
                            {
                                mo.WeeklyAssessment = one.WeeklyAssessment;
                            }
                            else
                            {
                                mo.WeeklyAssessment = "";
                            }


                            if (one.SpeakingSkills != null)/////
                            {
                                mo.SpeakingSkills = one.SpeakingSkills;
                            }
                            else
                            {
                                mo.SpeakingSkills = "";
                            }


                            if (one.TerminalAssessment != null)
                            {
                                mo.TerminalAssessment = one.TerminalAssessment;
                            }
                            else
                            {
                                mo.TerminalAssessment = "";
                            }
                            if (one.AggregateGrade != null)
                            {
                                mo.AggregateGrade = one.AggregateGrade;
                            }
                            else
                            {
                                mo.AggregateGrade = "";
                            }
                            //
                        }
                        else if (model.SubjectId == 4)//mathes
                        {
                            if (one.ConceptsActivity != null)
                            {
                                mo.ConceptsActivity = one.ConceptsActivity;
                            }
                            else
                            {
                                mo.ConceptsActivity = "";
                            }

                            if (one.Tables != null)
                            {
                                mo.Tables = one.Tables;
                            }
                            else
                            {
                                mo.Tables = "";
                            }

                            if (one.MentelAbility != null)
                            {
                                mo.MentelAbility = one.MentelAbility;
                            }
                            else
                            {
                                mo.MentelAbility = "";
                            }

                            if (one.WrittenWork != null)
                            {
                                mo.WrittenWork = one.WrittenWork;
                            }
                            else
                            {
                                mo.WrittenWork = "";
                            }

                            if (one.WeeklyAssessment != null)
                            {
                                mo.WeeklyAssessment = one.WeeklyAssessment;
                            }
                            else
                            {
                                mo.WeeklyAssessment = "";
                            }

                            if (one.TerminalAssessment != null)
                            {
                                mo.TerminalAssessment = one.TerminalAssessment;
                            }
                            else
                            {
                                mo.TerminalAssessment = "";
                            }

                            if (one.AggregateGrade != null)
                            {
                                mo.AggregateGrade = one.AggregateGrade;
                            }
                            else
                            {
                                mo.AggregateGrade = "";
                            }

                        }

                        else if (model.SubjectId == 5) //EVS
                        {
                            if (one.ActivityViva != null)
                            {
                                mo.ActivityViva = one.ActivityViva;
                            }
                            else
                            {
                                mo.ActivityViva = "";
                            }

                            if (one.WrittenWork != null)
                            {
                                mo.WrittenWork = one.WrittenWork;
                            }
                            else
                            {
                                mo.WrittenWork = "";
                            }

                            if (one.WeeklyAssessment != null)
                            {
                                mo.WeeklyAssessment = one.WeeklyAssessment;
                            }
                            else
                            {
                                mo.WeeklyAssessment = "";
                            }

                            if (one.TerminalAssessment != null)
                            {
                                mo.TerminalAssessment = one.TerminalAssessment;
                            }
                            else
                            {
                                mo.TerminalAssessment = "";
                            }

                            if (one.AggregateGrade != null)
                            {
                                mo.AggregateGrade = one.AggregateGrade;
                            }
                            else
                            {
                                mo.AggregateGrade = "";
                            }

                        }
                        else if (model.SubjectId == 6) //Co-Scholastic
                        {
                            if (one.ArtInterest != null)
                            {
                                mo.ArtInterest = one.ArtInterest;
                            }
                            else
                            {
                                mo.ArtInterest = "";
                            }

                            if (one.MusicInterest != null)
                            {
                                mo.MusicInterest = one.MusicInterest;
                            }
                            else
                            {
                                mo.MusicInterest = "";
                            }

                            if (one.ArtCreativity != null)
                            {
                                mo.ArtCreativity = one.ArtCreativity;
                            }
                            else
                            {
                                mo.ArtCreativity = "";
                            }

                            if (one.MusicRhythm != null)
                            {
                                mo.MusicRhythm = one.MusicRhythm;
                            }
                            else
                            {
                                mo.MusicRhythm = "";
                            }

                            if (one.Enthusiasm != null)
                            {
                                mo.Enthusiasm = one.Enthusiasm;
                            }
                            else
                            {
                                mo.Enthusiasm = "";
                            }

                            if (one.Discipline != null)
                            {
                                mo.Discipline = one.Discipline;
                            }
                            else
                            {
                                mo.Discipline = "";
                            }

                            if (one.TeamSpirit != null)
                            {
                                mo.TeamSpirit = one.TeamSpirit;
                            }
                            else
                            {
                                mo.TeamSpirit = "";
                            }

                            if (one.Computerscience != null)
                            {
                                mo.ComputerScience = one.Computerscience;
                            }
                            else
                            {
                                mo.ComputerScience = "";
                            }

                            if (one.GeneralStudies != null)
                            {
                                mo.GeneralStudies = one.GeneralStudies;
                            }
                            else
                            {
                                mo.GeneralStudies = "";
                            }

                            if (one.Valueeducation != null)
                            {
                                mo.Valueeducation = one.Valueeducation;
                            }
                            else
                            {
                                mo.Valueeducation = "";
                            }

                            if (one.Yle != null)
                            {
                                mo.YLE = one.Yle;
                            }
                            else
                            {
                                mo.YLE = "";
                            }

                            if (one.French != null)
                            {
                                mo.French = one.French;
                            }
                            else
                            {
                                mo.French = "";
                            }



                        }
                        else if (model.SubjectId == 7)//Personality Development
                        {
                            if (one.Courteousness != null)
                            {
                                mo.Courteousness = one.Courteousness;
                            }
                            else
                            {
                                mo.Courteousness = "";
                            }
                            if (one.Confidence != null)
                            {
                                mo.Confidence = one.Confidence;
                            }
                            else
                            {
                                mo.Confidence = "";
                            }
                            if (one.Careofbelongings != null)
                            {
                                mo.Careofbelongings = one.Careofbelongings;
                            }
                            else
                            {
                                mo.Careofbelongings = "";
                            }
                            if (one.Neatness != null)
                            {
                                mo.Neatness = one.Neatness;
                            }
                            else
                            {
                                mo.Neatness = "";
                            }
                            if (one.Punctuality != null)
                            {
                                mo.Punctuality = one.Punctuality;
                            }
                            else
                            {
                                mo.Punctuality = "";
                            }
                            if (one.Initiative != null)
                            {
                                mo.Initiative = one.Initiative;
                            }
                            else
                            {
                                mo.Initiative = "";
                            }
                            if (one.SharingCaring != null)
                            {
                                mo.SharingCaring = one.SharingCaring;
                            }
                            else
                            {
                                mo.SharingCaring = "";
                            }
                        }
                        else if (model.SubjectId == 8)//hEALTH
                        {
                            if (one.Height != null)
                            {
                                mo.Height = one.Height;
                            }
                            else
                            {
                                mo.Height = "";
                            }

                            if (one.Weight != null)
                            {
                                mo.Weight = one.Weight;
                            }
                            else
                            {
                                mo.Weight = "";
                            }

                            if (one.NoOfWorkingdays != null)
                            {
                                mo.NoOfWorkingdays = one.NoOfWorkingdays;
                            }
                            else
                            {
                                mo.NoOfWorkingdays = "";
                            }
                            if (one.DaysPresent != null)
                            {
                                mo.DaysPresent = one.DaysPresent;
                            }
                            else
                            {
                                mo.DaysPresent = "";
                            }

                            if (one.AggregateGrade != null)
                            {
                                mo.AggregateGrade = one.AggregateGrade;
                            }
                            else
                            {
                                mo.AggregateGrade = "";
                            }
                        }
                        else if (model.SubjectId == 9)
                        {
                            if (one.Remarks != null)
                            {
                                mo.Remarks = one.Remarks;
                            }
                            else
                            {
                                mo.Remarks = "";
                            }
                        }

                    }
                    else
                    {
                                //mo.WritingSkills = "";                             
                                //mo.ReadingSkills = "";                             
                                //mo.ListeningSkills = "";
                                //mo.WeeklyAssessment = ""; 
                                //mo.SpeakingSkills = "";
                                //mo.TerminalAssessment = "";
                                //mo.AggregateGrade = "";
                                //mo.ConceptsActivity = "";
                                //mo.Tables = "";
                                //mo.MentelAbility = "";
                                //mo.WrittenWork = "";
                                //mo.ActivityViva = "";
                            
                        }

                    // mo.StudentName = a1.StundentName;
                    sk_li.Add(mo);
                }
                model._ONE_FOUR = sk_li;
            }

             //return PartialView("~/Views/SpecialClass/_pv_StudentReadingSkillEntry_ONE_FOUR_NEW.cshtml", model);

             return PartialView("~/Views/SpecialClass/_pv_StudentReadingSkillEntry_ONE_FOUR.cshtml", model);
        }

        [HttpPost]
        public object SubmitStudentSkill_ONE_FOUR(StudentSkillMarks model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                foreach (var item in model._SkillModel)
                {

                    string currentYear = _Entities.TbAcademicYears.Where(X => X.CurrentYear == true).Select(X => X.AcademicYear).FirstOrDefault();



                    var one = _Entities.TbOneFourSkills.Where(x => x.Year == currentYear && x.SchoolId == model.SchoolId
                                                                       && x.StudentId == item.StudentId
                                                                       && x.ClassId == item.ClassId && x.DivisionId == model.DivisionId
                                                                       && x.SubjectId == model.SubjectId && x.AssesmentId == model.AssesmentId
                                                                       && x.IsActive == true).FirstOrDefault();
                    if (one == null)
                    {
                        one = new TbOneFourSkill
                        {
                            Year = currentYear,
                            SchoolId = model.SchoolId,
                            StudentId = item.StudentId,
                            ClassId = item.ClassId,
                            DivisionId = model.DivisionId,
                            AssesmentId = model.AssesmentId,
                            SubjectId = model.SubjectId,
                            Languages = item.Languages,
                            UserId = _user.UserId,
                            IsActive = true,
                            TimeStamp = CurrentTime
                        };


                        //Hindi..........English......Panjabi......
                        if (item.ONE_FOUR.ReadingSkills != null)
                        {
                            one.ReadingSkills = item.ONE_FOUR.ReadingSkills;
                        }
                        if (item.ONE_FOUR.WritingSkills != null)
                        {
                            one.WritingSkills = item.ONE_FOUR.WritingSkills;
                        }
                        if (item.ONE_FOUR.ListeningSkills != null)
                        {
                            one.ListeningSkills = item.ONE_FOUR.ListeningSkills;
                        }
                        if (item.ONE_FOUR.WeeklyAssessment != null)
                        {
                            one.WeeklyAssessment = item.ONE_FOUR.WeeklyAssessment;
                        }

                        if (item.ONE_FOUR.SpeakingSkills != null)
                        {
                            one.SpeakingSkills = item.ONE_FOUR.SpeakingSkills;
                        }

                        if (item.ONE_FOUR.TerminalAssessment != null)
                        {
                            one.TerminalAssessment = item.ONE_FOUR.TerminalAssessment;
                        }
                        if (item.ONE_FOUR.AggregateGrade != null)
                        {
                            one.AggregateGrade = item.ONE_FOUR.AggregateGrade;
                        }


                        // mathes... EV

                        if (item.ONE_FOUR.ConceptsActivity != null)
                        {
                            one.ConceptsActivity = item.ONE_FOUR.ConceptsActivity;
                        }
                        if (item.ONE_FOUR.Tables != null)
                        {
                            one.Tables = item.ONE_FOUR.Tables;
                        }
                        if (item.ONE_FOUR.MentelAbility != null)
                        {
                            one.MentelAbility = item.ONE_FOUR.MentelAbility;
                        }

                        if (item.ONE_FOUR.WrittenWork != null)
                        {
                            one.WrittenWork = item.ONE_FOUR.WrittenWork;
                        }

                        if (item.ONE_FOUR.ActivityViva != null)
                        {
                            one.ActivityViva = item.ONE_FOUR.ActivityViva;
                        }

                        ////////////////////

                        if (item.ONE_FOUR.Courteousness != null)
                        {
                            one.Courteousness = item.ONE_FOUR.Courteousness;
                        }

                        if (item.ONE_FOUR.Confidence != null)
                        {
                            one.Confidence = item.ONE_FOUR.Confidence;
                        }
                        if (item.ONE_FOUR.Careofbelongings != null)
                        {
                            one.Careofbelongings = item.ONE_FOUR.Careofbelongings;
                        }
                        if (item.ONE_FOUR.Neatness != null)
                        {
                            one.Neatness = item.ONE_FOUR.Neatness;
                        }
                        if (item.ONE_FOUR.Punctuality != null)
                        {
                            one.Punctuality = item.ONE_FOUR.Punctuality;
                        }
                        if (item.ONE_FOUR.Initiative != null)
                        {
                            one.Initiative = item.ONE_FOUR.Initiative;
                        }
                        if (item.ONE_FOUR.SharingCaring != null)
                        {
                            one.SharingCaring = item.ONE_FOUR.SharingCaring;
                        }

                         

                        ///////////////////////////////..............

                        if (item.ONE_FOUR.ArtInterest != null)
                        {
                            one.ArtInterest = item.ONE_FOUR.ArtInterest;
                        }
                        if (item.ONE_FOUR.MusicInterest != null)
                        {
                            one.MusicInterest = item.ONE_FOUR.MusicInterest;
                        }
                        if (item.ONE_FOUR.ArtCreativity != null)
                        {
                            one.ArtCreativity = item.ONE_FOUR.ArtCreativity;
                        }
                        if (item.ONE_FOUR.MusicRhythm != null)
                        {
                            one.MusicRhythm = item.ONE_FOUR.MusicRhythm;
                        }

                        if (item.ONE_FOUR.Enthusiasm != null)
                        {
                            one.Enthusiasm = item.ONE_FOUR.Enthusiasm;
                        }
                        if (item.ONE_FOUR.Discipline != null)
                        {
                            one.Discipline = item.ONE_FOUR.Discipline;
                        }
                        if (item.ONE_FOUR.TeamSpirit != null)
                        {
                            one.TeamSpirit = item.ONE_FOUR.TeamSpirit;
                        }

                        //......................TeamSpirit

                        if (item.ONE_FOUR.ComputerScience != null)
                        {
                            one.Computerscience = item.ONE_FOUR.ComputerScience;
                        }
                        if (item.ONE_FOUR.GeneralStudies != null)
                        {
                            one.GeneralStudies = item.ONE_FOUR.GeneralStudies;
                        }
                        if (item.ONE_FOUR.Valueeducation != null)
                        {
                            one.Valueeducation = item.ONE_FOUR.Valueeducation;
                        }
                        if (item.ONE_FOUR.YLE != null)
                        {
                            one.Yle = item.ONE_FOUR.YLE;
                        }
                        if (item.ONE_FOUR.French != null)
                        {
                            one.French = item.ONE_FOUR.French;
                        }


                        ///////hEALTH
                                                

                        if (item.ONE_FOUR.Height != null)
                        {
                            one.Height = item.ONE_FOUR.Height;
                        }
                        if (item.ONE_FOUR.Weight != null)
                        {
                            one.Weight = item.ONE_FOUR.Weight;
                        }
                        if (item.ONE_FOUR.NoOfWorkingdays != null)
                        {
                            one.NoOfWorkingdays = item.ONE_FOUR.NoOfWorkingdays;
                        }
                        if (item.ONE_FOUR.DaysPresent != null)
                        {
                            one.DaysPresent = item.ONE_FOUR.DaysPresent;
                        }
                        if (item.ONE_FOUR.AggregateGrade != null)
                        {
                            one.AggregateGrade = item.ONE_FOUR.AggregateGrade;
                        }

                        //////////////////////
                        if (item.ONE_FOUR.Remarks != null)
                        {
                            one.Remarks = item.ONE_FOUR.Remarks;
                        }

                        /////////////////////////////

                        one.IsActive = true;
                        one.TimeStamp = CurrentTime;

                        _Entities.TbOneFourSkills.Add(one);
                        status = _Entities.SaveChanges() > 0;



                    }

                    else
                    {
                        one.UserId = _user.UserId;


                        //English ..... this to start

                        if (item.ONE_FOUR.ReadingSkills != null)
                        {
                            if (item.ONE_FOUR.ReadingSkills == "0")
                            {
                                one.ReadingSkills = "";
                            }
                            else
                            {
                                one.ReadingSkills = item.ONE_FOUR.ReadingSkills;
                            }
                            
                        }
                        if (item.ONE_FOUR.WritingSkills != null)
                        {
                            if (item.ONE_FOUR.WritingSkills == "0")
                            {
                                one.WritingSkills = "";
                            }
                            else
                            {
                                one.WritingSkills = item.ONE_FOUR.WritingSkills;
                            }
                            
                        }
                        if (item.ONE_FOUR.ListeningSkills != null)
                        {
                            if (item.ONE_FOUR.ListeningSkills == "0")
                            {
                                one.ListeningSkills = "";
                            }
                            else
                            {
                                one.ListeningSkills = item.ONE_FOUR.ListeningSkills;
                            }
                            
                        }
                        if (item.ONE_FOUR.WeeklyAssessment != null)
                        {
                            if (item.ONE_FOUR.WeeklyAssessment == "0")
                            {
                                one.WeeklyAssessment = "";
                            }
                            else
                            {
                                one.WeeklyAssessment = item.ONE_FOUR.WeeklyAssessment;
                            }
                            
                        }
                        if (item.ONE_FOUR.SpeakingSkills != null)
                        {
                            if (item.ONE_FOUR.SpeakingSkills == "0")
                            {
                                one.SpeakingSkills = "";
                            }
                            else
                            {
                                one.SpeakingSkills = item.ONE_FOUR.SpeakingSkills;
                            }
                            
                        }
                        if (item.ONE_FOUR.TerminalAssessment != null)
                        {
                            if (item.ONE_FOUR.TerminalAssessment == "0")
                            {
                                one.TerminalAssessment = "";
                            }
                            else
                            {
                                one.TerminalAssessment = item.ONE_FOUR.TerminalAssessment;
                            }
                            
                        }
                        if (item.ONE_FOUR.AggregateGrade != null)
                        {
                            if (item.ONE_FOUR.AggregateGrade == "0")
                            {
                                one.AggregateGrade = "";
                            }
                            else
                            {
                                one.AggregateGrade = item.ONE_FOUR.AggregateGrade;
                            }
                            
                        }
                        if (item.ONE_FOUR.ConceptsActivity != null)
                        {
                            if (item.ONE_FOUR.ConceptsActivity == "0")
                            {
                                one.ConceptsActivity = "";
                            }
                            else
                            {
                                one.ConceptsActivity = item.ONE_FOUR.ConceptsActivity;
                            }
                            
                        }
                        if (item.ONE_FOUR.Tables != null)
                        {
                            if (item.ONE_FOUR.Tables == "0")
                            {
                                one.Tables = "";
                            }
                            else
                            {
                                one.Tables = item.ONE_FOUR.Tables;
                            }
                            
                        }
                        if (item.ONE_FOUR.MentelAbility != null)
                        {
                            if (item.ONE_FOUR.MentelAbility == "0")
                            {
                                one.MentelAbility = "";
                            }
                            else
                            {
                                one.MentelAbility = item.ONE_FOUR.MentelAbility;
                            }
                            
                        }

                        if (item.ONE_FOUR.WrittenWork != null)
                        {
                            if (item.ONE_FOUR.WrittenWork == "0")
                            {
                                one.WrittenWork = "";
                            }
                            else
                            {
                                one.WrittenWork = item.ONE_FOUR.WrittenWork;
                            }
                            
                        }

                        if (item.ONE_FOUR.ActivityViva != null)
                        {
                            if (item.ONE_FOUR.ActivityViva == "0")
                            {
                                one.ActivityViva = "";
                            }
                            else
                            {
                                one.ActivityViva = item.ONE_FOUR.ActivityViva;
                            }
                            
                        }

                        ////////////////////

                        if (item.ONE_FOUR.Courteousness != null)
                        {
                            if (item.ONE_FOUR.Courteousness == "0")
                            {
                                one.Courteousness = "";
                            }
                            else
                            {
                                one.Courteousness = item.ONE_FOUR.Courteousness;
                            }
                            
                        }

                        if (item.ONE_FOUR.Confidence != null)
                        {
                            if (item.ONE_FOUR.Confidence == "0")
                            {
                                one.Confidence = "";
                            }
                            else
                            {
                                one.Confidence = item.ONE_FOUR.Confidence;
                            }
                            
                        }
                        if (item.ONE_FOUR.Careofbelongings != null)
                        {
                            if (item.ONE_FOUR.Careofbelongings == "0")
                            {
                                one.Careofbelongings = "";
                            }
                            else
                            {
                                one.Careofbelongings = item.ONE_FOUR.Careofbelongings;
                            }
                            
                        }
                        if (item.ONE_FOUR.Neatness != null)
                        {
                            if (item.ONE_FOUR.Neatness == "0")
                            {
                                one.Neatness = "";
                            }
                            else
                            {
                                one.Neatness = item.ONE_FOUR.Neatness;
                            }
                            
                        }
                        if (item.ONE_FOUR.Punctuality != null)
                        {
                            if (item.ONE_FOUR.Punctuality == "0")
                            {
                                one.Punctuality = "";
                            }
                            else
                            {
                                one.Punctuality = item.ONE_FOUR.Punctuality;
                            }
                            
                        }
                        if (item.ONE_FOUR.Initiative != null)
                        {
                            if (item.ONE_FOUR.Initiative == "0")
                            {
                                one.Initiative = "";
                            }
                            else
                            {
                                one.Initiative = item.ONE_FOUR.Initiative;
                            }
                            
                        }
                        if (item.ONE_FOUR.SharingCaring != null)
                        {
                            if (item.ONE_FOUR.SharingCaring == "0")
                            {
                                one.SharingCaring = "";
                            }
                            else
                            {
                                one.SharingCaring = item.ONE_FOUR.SharingCaring;
                            }
                            
                        }


                        /////////////////////////////

                        //...............................
                        if (item.ONE_FOUR.ArtInterest != null)
                        {
                            if (item.ONE_FOUR.ArtInterest == "0")
                            {
                                one.ArtInterest = "";
                            }
                            else
                            {
                                one.ArtInterest = item.ONE_FOUR.ArtInterest;
                            }
                            
                        }
                        if (item.ONE_FOUR.MusicInterest != null)
                        {
                            if (item.ONE_FOUR.MusicInterest == "0")
                            {
                                one.MusicInterest = "";
                            }
                            else
                            {
                                one.MusicInterest = item.ONE_FOUR.MusicInterest;
                            }
                            
                        }
                        if (item.ONE_FOUR.ArtCreativity != null)
                        {
                            if (item.ONE_FOUR.ArtCreativity == "0")
                            {
                                one.ArtCreativity = "";
                            }
                            else
                            {
                                one.ArtCreativity = item.ONE_FOUR.ArtCreativity;
                            }
                            
                        }
                        if (item.ONE_FOUR.MusicRhythm != null)
                        {
                            if (item.ONE_FOUR.MusicRhythm == "0")
                            {
                                one.MusicRhythm = "";
                            }
                            else
                            {
                                one.MusicRhythm = item.ONE_FOUR.MusicRhythm;
                            }
                            
                        }

                        if (item.ONE_FOUR.Enthusiasm != null)
                        {
                            if (item.ONE_FOUR.Enthusiasm == "0")
                            {
                                one.Enthusiasm = "";
                            }
                            else
                            {
                                one.Enthusiasm = item.ONE_FOUR.Enthusiasm;
                            }
                            
                        }
                        if (item.ONE_FOUR.Discipline != null)
                        {
                            if (item.ONE_FOUR.Discipline == "0")
                            {
                                one.Discipline = "";
                            }
                            else
                            {
                                one.Discipline = item.ONE_FOUR.Discipline;
                            }
                            
                        }
                        if (item.ONE_FOUR.TeamSpirit != null)
                        {
                            if (item.ONE_FOUR.TeamSpirit == "0")
                            {
                                one.TeamSpirit = "";
                            }
                            else
                            {
                                one.TeamSpirit = item.ONE_FOUR.TeamSpirit;
                            }
                            
                        }



                        ////////////////////



                        if (item.ONE_FOUR.ComputerScience != null)
                        {
                            if (item.ONE_FOUR.ComputerScience == "0")
                            {
                                one.Computerscience = "";
                            }
                            else
                            {
                                one.Computerscience = item.ONE_FOUR.ComputerScience;
                            }
                            
                        }
                        if (item.ONE_FOUR.GeneralStudies != null)
                        {
                            if (item.ONE_FOUR.GeneralStudies == "0")
                            {
                                one.GeneralStudies = "";
                            }
                            else
                            {
                                one.GeneralStudies = item.ONE_FOUR.GeneralStudies;
                            }
                            
                        }
                        if (item.ONE_FOUR.Valueeducation != null)
                        {
                            if (item.ONE_FOUR.Valueeducation == "0")
                            {
                                one.Valueeducation = "";
                            }
                            else
                            {
                                one.Valueeducation = item.ONE_FOUR.Valueeducation;
                            }
                            
                        }
                        if (item.ONE_FOUR.YLE != null)
                        {
                            if (item.ONE_FOUR.YLE == "0")
                            {
                                one.Yle = "";
                            }
                            else
                            {
                                one.Yle = item.ONE_FOUR.YLE;
                            }
                            
                        }
                        if (item.ONE_FOUR.French != null)
                        {
                            if (item.ONE_FOUR.French == "0")
                            {
                                one.French = "";
                            }
                            else
                            {
                                one.French = item.ONE_FOUR.French;
                            }
                            
                        }
                        //..............................

                        if (item.ONE_FOUR.Height != null)
                        {
                            if (item.ONE_FOUR.Height == "0")
                            {
                                one.Height = "";
                            }
                            else
                            {
                                one.Height = item.ONE_FOUR.Height;
                            }
                            
                        }
                        if (item.ONE_FOUR.Weight != null)
                        {
                            if (item.ONE_FOUR.Weight == "0")
                            {
                                one.Weight = "";
                            }
                            else
                            {
                                one.Weight = item.ONE_FOUR.Weight;
                            }
                            
                        }
                        if (item.ONE_FOUR.NoOfWorkingdays != null)
                        {
                            if (item.ONE_FOUR.NoOfWorkingdays == "0")
                            {
                                one.NoOfWorkingdays = "";
                            }
                            else
                            {
                                one.NoOfWorkingdays = item.ONE_FOUR.NoOfWorkingdays;
                            }
                            
                        }
                        if (item.ONE_FOUR.DaysPresent != null)
                        {
                            if (item.ONE_FOUR.DaysPresent == "0")
                            {
                                one.DaysPresent = "";
                            }
                            else
                            {
                                one.DaysPresent = item.ONE_FOUR.DaysPresent;
                            }
                            
                        }
                        if (item.ONE_FOUR.AggregateGrade != null)
                        {
                            if (item.ONE_FOUR.AggregateGrade == "0")
                            {
                                one.AggregateGrade = "";
                            }
                            else
                            {
                                one.AggregateGrade = item.ONE_FOUR.AggregateGrade;
                            }
                            
                        }

                        if (item.ONE_FOUR.Remarks != null)
                        {
                            if (item.ONE_FOUR.Remarks == "0")
                            {
                                one.Remarks = "";
                            }
                            else
                            {
                                one.Remarks = item.ONE_FOUR.Remarks;
                            }
                            
                        }


                        status = _Entities.SaveChanges() > 0;

                    }


                }
                msg = "successful";
                return Json(new { status = 1, msg = msg });
            }
            catch (Exception ex)
            {
                return Json(new { status = status, msg = msg });
            }

        }

        public IActionResult NonEducationalHome_ONE_FOUR()
        {
            SubjectScoreModel model = new SubjectScoreModel
            {
                SchoolId = _user.SchoolId,
                UserId = _user.UserId,
                IsAdminCheck = HttpContext.Session.GetInt32("isAdmin") ?? 0,
            };
            ViewBag.classpreschoo = _dropdown.GetClass_Preschool_One_Four(model.SchoolId);


            return View(model);
        }


        public IActionResult VProgressCardHome_ONE_FOUR()
        {
            SubjectScoreModel model = new SubjectScoreModel
            {
                SchoolId = _user.SchoolId,
                UserId = _user.UserId,
                IsAdminCheck = HttpContext.Session.GetInt32("isAdmin") ?? 0
            };
            ViewBag.Classlist = _dropdown.GetClass_Preschool_One_Four(model.SchoolId);

            return View(model);
        }

        public PartialViewResult ViewAll_V_ClassStudents_ONE_FOUR(string id)
        {
            VProgressCardList model = new VProgressCardList();

            string[] splitData = id.Split('~');
            long ClassId = Convert.ToInt64(splitData[0]);
            long DivissionId = Convert.ToInt64(splitData[1]);

            model.SchoolId = _user.SchoolId;

            //model.TempMo1 = TempMo1.Where(x => x.classId == ClassId && x.divisionId == DivissionId && x.schoolId == _user.SchoolId).OrderBy(x => x.studentName).ToList();

            var results = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == DivissionId && x.SchoolId == _user.SchoolId && x.IsActive == true).OrderBy(x => x.StundentName).ToList();
            List<StudentModel> li = new List<StudentModel>();
            foreach (var a1 in results)
            {
                StudentModel st = new StudentModel();
                st.studentId = a1.StudentId;
                st.studentName = a1.StundentName;
                li.Add(st);
            }

            model.TempMo1 = li;

            return PartialView("~/Views/SpecialClass/_pv_StudentListForProgressCard_ONE_FOUR.cshtml", model);
        }

        public IActionResult VProgressCard_ONE_FOUR(string ids)
        {
            string[] conte = ids.Split('~');
            long id = Convert.ToInt64(conte[0]);
            string static_Date = conte[1];

            string currentYear = _Entities.TbAcademicYears.Where(X => X.CurrentYear == true).Select(X => X.AcademicYear).FirstOrDefault();

            long studentId = Convert.ToInt64(id);
            long schoolId = _user.SchoolId;
            var data = _Entities.TbOneFourSkills.Where(x => x.SchoolId == schoolId && x.StudentId == studentId && x.Year == currentYear).ToList();


            /// this to starting.............



            ONE_FOUR_ReportModel mo = new ONE_FOUR_ReportModel();


            Title_Report Titl = new Title_Report();

            COMMEN_ONE_FOUR__Report English = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Hindi = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Punjabi = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Mathematics = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report EV = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Art_and_Music = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Physical_Health = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Personality = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Aspects = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Attendence_Aggre = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Assessment = new COMMEN_ONE_FOUR__Report();
                        

            var studentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId).FirstOrDefault();
            Titl.ReportName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            Titl.SchoolName = studentDetails.School.SchoolName;
            Titl.SchoolLogo = "http://satluj.schoolman.in" + studentDetails.School.FilePath;
            
            Titl.StudentProfile = "http://satluj.schoolman.in" + studentDetails.FilePath;
             
            Titl.Static_Date = static_Date;




            var data_Student_Table = _Entities.TbStudents.Where(x => x.SchoolId == schoolId && x.StudentId == studentId).FirstOrDefault();

            if (data_Student_Table != null)
            {
                Titl.Name = data_Student_Table.StundentName;
                Titl.FatherName = _Entities.TbParents.Where(x => x.ParentId == data_Student_Table.ParentId).Select(x => x.ParentName).FirstOrDefault();
                Titl.MotherName = _Entities.TbParents.Where(x => x.ParentId == data_Student_Table.ParentId).Select(x => x.MotherName).FirstOrDefault();
                Titl.AdmissionNo = data_Student_Table.StudentSpecialId;
                Titl.Class = _Entities.TbClasses.Where(x => x.ClassId == data_Student_Table.ClassId).Select(x => x.Class).FirstOrDefault();
                Titl.Division = _Entities.TbDivisions.Where(x => x.DivisionId == data_Student_Table.DivisionId).Select(x => x.Division).FirstOrDefault();

                DateTime Dt = Convert.ToDateTime(data_Student_Table.Dob);
                string DD = Dt.Day.ToString();
                string MM = Dt.Month.ToString();
                string YYYY = Dt.Year.ToString();
                if (Convert.ToInt32(DD) < 10)
                {
                    DD = "0" + DD;
                }
                if (Convert.ToInt32(MM) < 10)
                {
                    MM = "0" + MM;
                }

                Titl.DateOfBirth = DD + "-" + MM + "-" + YYYY;
            }
            else
            {
                Titl.Name = "xxxx";
                Titl.FatherName = "xxxx";
                Titl.MotherName = "xxxx";
                Titl.AdmissionNo = "xxxx";
                Titl.Class = "xxxx";
                Titl.DateOfBirth = "xxxx";

            }

            //2019-03-26 00:00:00.000 11-08-2000
            //DateTime Dt = Convert.ToDateTime("2019-03-26 00:00:00.000");


            Titl.Session = currentYear;

            //...................

            //Hindi Report
            Hindi.ReadingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.ReadingSkills).FirstOrDefault();
            Hindi.WritingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.WritingSkills).FirstOrDefault();
            Hindi.ListeningSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.ListeningSkills).FirstOrDefault();
            Hindi.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Hindi.SpeakingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.SpeakingSkills).FirstOrDefault();
            Hindi.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.TerminalAssessment).FirstOrDefault();
            Hindi.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.AggregateGrade).FirstOrDefault();

            Hindi.ReadingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.ReadingSkills).FirstOrDefault();
            Hindi.WritingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.WritingSkills).FirstOrDefault();
            Hindi.ListeningSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.ListeningSkills).FirstOrDefault();
            Hindi.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Hindi.SpeakingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.SpeakingSkills).FirstOrDefault();
            Hindi.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.TerminalAssessment).FirstOrDefault();
            Hindi.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.AggregateGrade).FirstOrDefault();

            Hindi.ReadingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.ReadingSkills).FirstOrDefault();
            Hindi.WritingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.WritingSkills).FirstOrDefault();
            Hindi.ListeningSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.ListeningSkills).FirstOrDefault();
            Hindi.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Hindi.SpeakingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.SpeakingSkills).FirstOrDefault();
            Hindi.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.TerminalAssessment).FirstOrDefault();
            Hindi.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.AggregateGrade).FirstOrDefault();



            //English Report
            English.ReadingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.ReadingSkills).FirstOrDefault();
            English.WritingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.WritingSkills).FirstOrDefault();
            English.ListeningSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.ListeningSkills).FirstOrDefault();
            English.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.WeeklyAssessment).FirstOrDefault();
            English.SpeakingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.SpeakingSkills).FirstOrDefault();
            English.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.TerminalAssessment).FirstOrDefault();
            English.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.AggregateGrade).FirstOrDefault();

            English.ReadingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.ReadingSkills).FirstOrDefault();
            English.WritingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.WritingSkills).FirstOrDefault();
            English.ListeningSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.ListeningSkills).FirstOrDefault();
            English.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.WeeklyAssessment).FirstOrDefault();
            English.SpeakingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.SpeakingSkills).FirstOrDefault();
            English.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.TerminalAssessment).FirstOrDefault();
            English.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.AggregateGrade).FirstOrDefault();

            English.ReadingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.ReadingSkills).FirstOrDefault();
            English.WritingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.WritingSkills).FirstOrDefault();
            English.ListeningSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.ListeningSkills).FirstOrDefault();
            English.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.WeeklyAssessment).FirstOrDefault();
            English.SpeakingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.SpeakingSkills).FirstOrDefault();
            English.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.TerminalAssessment).FirstOrDefault();
            English.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.AggregateGrade).FirstOrDefault();

            //Punjabi.......

            Punjabi.ReadingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.ReadingSkills).FirstOrDefault();
            Punjabi.WritingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.WritingSkills).FirstOrDefault();
            Punjabi.ListeningSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.ListeningSkills).FirstOrDefault();
            Punjabi.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Punjabi.SpeakingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.SpeakingSkills).FirstOrDefault();
            Punjabi.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.TerminalAssessment).FirstOrDefault();
            Punjabi.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.AggregateGrade).FirstOrDefault();

            Punjabi.ReadingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.ReadingSkills).FirstOrDefault();
            Punjabi.WritingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.WritingSkills).FirstOrDefault();
            Punjabi.ListeningSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.ListeningSkills).FirstOrDefault();
            Punjabi.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Punjabi.SpeakingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.SpeakingSkills).FirstOrDefault();
            Punjabi.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.TerminalAssessment).FirstOrDefault();
            Punjabi.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.AggregateGrade).FirstOrDefault();

            Punjabi.ReadingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.ReadingSkills).FirstOrDefault();
            Punjabi.WritingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.WritingSkills).FirstOrDefault();
            Punjabi.ListeningSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.ListeningSkills).FirstOrDefault();
            Punjabi.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Punjabi.SpeakingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.SpeakingSkills).FirstOrDefault();
            Punjabi.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.TerminalAssessment).FirstOrDefault();
            Punjabi.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.AggregateGrade).FirstOrDefault();

            //Mathematics
            Mathematics.ConceptsActivity_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.ConceptsActivity).FirstOrDefault();
            Mathematics.Tables_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.Tables).FirstOrDefault();
            Mathematics.MentelAbility_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.MentelAbility).FirstOrDefault();
            Mathematics.WrittenWork_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.WrittenWork).FirstOrDefault();
            Mathematics.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Mathematics.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.TerminalAssessment).FirstOrDefault();
            Mathematics.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.AggregateGrade).FirstOrDefault();


            Mathematics.ConceptsActivity_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.ConceptsActivity).FirstOrDefault();
            Mathematics.Tables_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.Tables).FirstOrDefault();
            Mathematics.MentelAbility_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.MentelAbility).FirstOrDefault();
            Mathematics.WrittenWork_II= data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.WrittenWork).FirstOrDefault();
            Mathematics.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Mathematics.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.TerminalAssessment).FirstOrDefault();
            Mathematics.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.AggregateGrade).FirstOrDefault();


            Mathematics.ConceptsActivity_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.ConceptsActivity).FirstOrDefault();
            Mathematics.Tables_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.Tables).FirstOrDefault();
            Mathematics.MentelAbility_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.MentelAbility).FirstOrDefault();
            Mathematics.WrittenWork_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.WrittenWork).FirstOrDefault();
            Mathematics.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Mathematics.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.TerminalAssessment).FirstOrDefault();
            Mathematics.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.AggregateGrade).FirstOrDefault();

            ///Environmental Studies

            EV.ActivityViva_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.ActivityViva).FirstOrDefault();
            EV.WrittenWork_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.WrittenWork).FirstOrDefault();
            EV.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.WeeklyAssessment).FirstOrDefault();
            EV.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.TerminalAssessment).FirstOrDefault();
            EV.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.AggregateGrade).FirstOrDefault();

            EV.ActivityViva_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.ActivityViva).FirstOrDefault();
            EV.WrittenWork_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.WrittenWork).FirstOrDefault();
            EV.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.WeeklyAssessment).FirstOrDefault();
            EV.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.TerminalAssessment).FirstOrDefault();
            EV.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.AggregateGrade).FirstOrDefault();

            EV.ActivityViva_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.ActivityViva).FirstOrDefault();
            EV.WrittenWork_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.WrittenWork).FirstOrDefault();
            EV.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.WeeklyAssessment).FirstOrDefault();
            EV.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.TerminalAssessment).FirstOrDefault();
            EV.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.AggregateGrade).FirstOrDefault();

            ////////...........COSCOLASTIC............SUBJECT ID
            //Art_and_Music... 
            Art_and_Music.ArtInterest_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.ArtInterest).FirstOrDefault();
            Art_and_Music.ArtCreativity_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.ArtCreativity).FirstOrDefault();
            Art_and_Music.MusicInterest_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.MusicInterest).FirstOrDefault();
            Art_and_Music.MusicRhythm_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.MusicRhythm).FirstOrDefault();

            Art_and_Music.ArtInterest_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.ArtInterest).FirstOrDefault();
            Art_and_Music.ArtCreativity_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.ArtCreativity).FirstOrDefault();
            Art_and_Music.MusicInterest_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.MusicInterest).FirstOrDefault();
            Art_and_Music.MusicRhythm_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.MusicRhythm).FirstOrDefault();

            Art_and_Music.ArtInterest_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.ArtInterest).FirstOrDefault();
            Art_and_Music.ArtCreativity_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.ArtCreativity).FirstOrDefault();
            Art_and_Music.MusicInterest_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.MusicInterest).FirstOrDefault();
            Art_and_Music.MusicRhythm_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.MusicRhythm).FirstOrDefault();


            //Physical Education and health
            Physical_Health.Enthusiasm_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Enthusiasm).FirstOrDefault();
            Physical_Health.Discipline_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Discipline).FirstOrDefault();
            Physical_Health.TeamSpirit_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.TeamSpirit).FirstOrDefault();

            Physical_Health.Enthusiasm_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Enthusiasm).FirstOrDefault();
            Physical_Health.Discipline_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Discipline).FirstOrDefault();
            Physical_Health.TeamSpirit_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.TeamSpirit).FirstOrDefault();

            Physical_Health.Enthusiasm_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Enthusiasm).FirstOrDefault();
            Physical_Health.Discipline_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Discipline).FirstOrDefault();
            Physical_Health.TeamSpirit_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.TeamSpirit).FirstOrDefault();


            //Aspects

            Aspects.Computerscience_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Computerscience).FirstOrDefault();
            Aspects.GeneralStudies_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.GeneralStudies).FirstOrDefault();
            Aspects.Valueeducation_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Valueeducation).FirstOrDefault();
            Aspects.YLE_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Yle).FirstOrDefault();
            Aspects.French_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.French).FirstOrDefault();

            Aspects.Computerscience_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Computerscience).FirstOrDefault();
            Aspects.GeneralStudies_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.GeneralStudies).FirstOrDefault();
            Aspects.Valueeducation_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Valueeducation).FirstOrDefault();
            Aspects.YLE_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Yle).FirstOrDefault();
            Aspects.French_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.French).FirstOrDefault();

            Aspects.Computerscience_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Computerscience).FirstOrDefault();
            Aspects.GeneralStudies_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.GeneralStudies).FirstOrDefault();
            Aspects.Valueeducation_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Valueeducation).FirstOrDefault();
            Aspects.YLE_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Yle).FirstOrDefault();
            Aspects.French_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.French).FirstOrDefault();

            //Personality Development

            Personality.Courteousness_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Courteousness).FirstOrDefault();
            Personality.Confidence_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Confidence).FirstOrDefault();
            Personality.Careofbelongings_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Careofbelongings).FirstOrDefault();
            Personality.Neatness_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Neatness).FirstOrDefault();
            Personality.Punctuality_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Punctuality).FirstOrDefault();
            Personality.Initiative_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Initiative).FirstOrDefault();
            Personality.SharingCaring_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.SharingCaring).FirstOrDefault();

            Personality.Courteousness_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Courteousness).FirstOrDefault();
            Personality.Confidence_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Confidence).FirstOrDefault();
            Personality.Careofbelongings_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Careofbelongings).FirstOrDefault();
            Personality.Neatness_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Neatness).FirstOrDefault();
            Personality.Punctuality_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Punctuality).FirstOrDefault();
            Personality.Initiative_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Initiative).FirstOrDefault();
            Personality.SharingCaring_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.SharingCaring).FirstOrDefault();

            Personality.Courteousness_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Courteousness).FirstOrDefault();
            Personality.Confidence_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Confidence).FirstOrDefault();
            Personality.Careofbelongings_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Careofbelongings).FirstOrDefault();
            Personality.Neatness_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Neatness).FirstOrDefault();
            Personality.Punctuality_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Punctuality).FirstOrDefault();
            Personality.Initiative_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Initiative).FirstOrDefault();
            Personality.SharingCaring_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.SharingCaring).FirstOrDefault();


            ///hEALTH

            Physical_Health.Height_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.Height).FirstOrDefault();
            Physical_Health.Weight_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.Weight).FirstOrDefault();

            
            Physical_Health.Height_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.Height).FirstOrDefault();
            Physical_Health.Weight_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.Weight).FirstOrDefault();

              Physical_Health.Height_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.Height).FirstOrDefault();
            Physical_Health.Weight_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.Weight).FirstOrDefault();

            
            
            //Attendence_Aggre

            Attendence_Aggre.NoOfWorkingdays_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.NoOfWorkingdays).FirstOrDefault();
            Attendence_Aggre.DaysPresent_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.DaysPresent).FirstOrDefault();
            Attendence_Aggre.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.AggregateGrade).FirstOrDefault();

            Attendence_Aggre.NoOfWorkingdays_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.NoOfWorkingdays).FirstOrDefault();
            Attendence_Aggre.DaysPresent_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.DaysPresent).FirstOrDefault();
            Attendence_Aggre.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.AggregateGrade).FirstOrDefault();

            Attendence_Aggre.NoOfWorkingdays_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.NoOfWorkingdays).FirstOrDefault();
            Attendence_Aggre.DaysPresent_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.DaysPresent).FirstOrDefault();
            Attendence_Aggre.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.AggregateGrade).FirstOrDefault();

            //Assessment
            Assessment.Assessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 9).Select(x => x.Remarks).FirstOrDefault();
            Assessment.Assessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 9).Select(x => x.Remarks).FirstOrDefault();
            Assessment.Assessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 9).Select(x => x.Remarks).FirstOrDefault();




            mo.English_Report = English;
            mo.Hindi_Report = Hindi;
            mo.Punjabi_Report = Punjabi;
            mo.Mathematics_Report = Mathematics;
            mo.EV_Report = EV;
            mo.Art_and_Music_Report = Art_and_Music;
            mo.Physical_Education_Health_Report = Physical_Health;
            mo.Personality_Report = Personality;
            mo.Aspects_Report = Aspects;
            mo.Attendence_Report = Attendence_Aggre;
            mo.Assessment_Report = Assessment;
            mo.Title_Report = Titl;
            return View(mo);
        }
        public IActionResult PreviousYear_Progresscard()
        {
            SubjectScoreModel model = new SubjectScoreModel
            {
                SchoolId = _user.SchoolId,
                UserId = _user.UserId,
                IsAdminCheck = HttpContext.Session.GetInt32("isAdmin") ?? 0
            };
            ViewBag.Academicyear = _dropdown.GetAllAcademicYear();

            return View(model);
        }

        public PartialViewResult ViewAll_Previous_ClassStudents_ONE_FOUR(string id)
        {
            VProgressCardList model = new VProgressCardList();

            string[] splitData = id.Split('~');
            long ClassId = Convert.ToInt64(splitData[0]);
            //long DivissionId = Convert.ToInt64(splitData[1]);
            string year = Convert.ToString(splitData[1]);
            model.SchoolId = _user.SchoolId;

            //model.TempMo1 = TempMo1.Where(x => x.classId == ClassId && x.divisionId == DivissionId && x.schoolId == _user.SchoolId).OrderBy(x => x.studentName).ToList();
            //var Previousyear = _Entities.tb_ONE_FOUR_SKILLS.Where(z => z.Year == year && z.IsActive == true).ToList();
            var query = _Entities.SPPreviousYearProgressReport
                .FromSqlRaw("EXEC SP_PreviousYear_Progressreport @p0, @p1, @p2",
                             _user.SchoolId, year, ClassId)
                .ToList();

            var results = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.SchoolId == _user.SchoolId && x.IsActive == true).OrderBy(x => x.StundentName).ToList();
            List<StudentModel> li = new List<StudentModel>();
            foreach (var a1 in query)
            {
                StudentModel st = new StudentModel();
                st.studentId = a1.StudentId;
                st.studentName = a1.StundentName;
                st.AcademinYear = a1.Year;
                li.Add(st);
            }

            model.TempMo1 = li;

            return PartialView("~/Views/SpecialClass/_pv_previous_yearcard.cshtml", model);
        }

        public IActionResult VPreviousYearCard_ONE_FOUR(string ids)
        {
            string[] conte = ids.Split('~');
            long id = Convert.ToInt64(conte[0]);
            string static_Date =Convert.ToString(DateTime.Now);
            string Previousyear = conte[1];
            //string currentYear = _Entities.tb_AcademicYear.Where(X => X.CurrentYear == true).Select(X => X.AcademicYear).FirstOrDefault();

            long studentId = Convert.ToInt64(id);
            long schoolId = _user.SchoolId;
            var data = _Entities.TbOneFourSkills.Where(x => x.SchoolId == schoolId && x.StudentId == studentId && x.Year == Previousyear).ToList();

            var data_One = _Entities.TbOneFourSkills.Where(x => x.SchoolId == schoolId && x.StudentId == studentId && x.Year == Previousyear).FirstOrDefault();

            /// this to starting.............



            ONE_FOUR_ReportModel mo = new ONE_FOUR_ReportModel();


            Title_Report Titl = new Title_Report();

            COMMEN_ONE_FOUR__Report English = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Hindi = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Punjabi = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Mathematics = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report EV = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Art_and_Music = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Physical_Health = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Personality = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Aspects = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Attendence_Aggre = new COMMEN_ONE_FOUR__Report();
            COMMEN_ONE_FOUR__Report Assessment = new COMMEN_ONE_FOUR__Report();


            var studentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId).FirstOrDefault();
            Titl.ReportName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            Titl.SchoolName = studentDetails.School.SchoolName;
            Titl.SchoolLogo = "http://satluj.schoolman.in" + studentDetails.School.FilePath;

            Titl.StudentProfile = "http://satluj.schoolman.in" + studentDetails.FilePath;

            Titl.Static_Date = static_Date;




            var data_Student_Table = _Entities.TbStudents.Where(x => x.SchoolId == schoolId && x.StudentId == studentId).FirstOrDefault();

            if (data_Student_Table != null)
            {
                Titl.Name = data_Student_Table.StundentName;
                Titl.FatherName = _Entities.TbParents.Where(x => x.ParentId == data_Student_Table.ParentId).Select(x => x.ParentName).FirstOrDefault();
                Titl.MotherName = _Entities.TbParents.Where(x => x.ParentId == data_Student_Table.ParentId).Select(x => x.MotherName).FirstOrDefault();
                Titl.AdmissionNo = data_Student_Table.StudentSpecialId;
                //Titl.Class = _Entities.tb_Class.Where(x => x.ClassId == data_Student_Table.ClassId).Select(x => x.Class).FirstOrDefault();
                //Titl.Division = _Entities.tb_Division.Where(x => x.DivisionId == data_Student_Table.DivisionId).Select(x => x.Division).FirstOrDefault();
                Titl.Class = _Entities.TbClasses.Where(x => x.ClassId == data_One.ClassId).Select(x => x.Class).FirstOrDefault();
                Titl.Division = _Entities.TbDivisions.Where(x => x.DivisionId == data_One.DivisionId).Select(x => x.Division).FirstOrDefault();

                DateTime Dt = Convert.ToDateTime(data_Student_Table.Dob);
                string DD = Dt.Day.ToString();
                string MM = Dt.Month.ToString();
                string YYYY = Dt.Year.ToString();
                if (Convert.ToInt32(DD) < 10)
                {
                    DD = "0" + DD;
                }
                if (Convert.ToInt32(MM) < 10)
                {
                    MM = "0" + MM;
                }

                Titl.DateOfBirth = DD + "-" + MM + "-" + YYYY;
            }
            else
            {
                Titl.Name = "xxxx";
                Titl.FatherName = "xxxx";
                Titl.MotherName = "xxxx";
                Titl.AdmissionNo = "xxxx";
                Titl.Class = "xxxx";
                Titl.DateOfBirth = "xxxx";

            }

            //2019-03-26 00:00:00.000 11-08-2000
            //DateTime Dt = Convert.ToDateTime("2019-03-26 00:00:00.000");


            Titl.Session = Previousyear;

            //...................

            //Hindi Report
            Hindi.ReadingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.ReadingSkills).FirstOrDefault();
            Hindi.WritingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.WritingSkills).FirstOrDefault();
            Hindi.ListeningSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.ListeningSkills).FirstOrDefault();
            Hindi.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Hindi.SpeakingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.SpeakingSkills).FirstOrDefault();
            Hindi.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.TerminalAssessment).FirstOrDefault();
            Hindi.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 1).Select(x => x.AggregateGrade).FirstOrDefault();

            Hindi.ReadingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.ReadingSkills).FirstOrDefault();
            Hindi.WritingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.WritingSkills).FirstOrDefault();
            Hindi.ListeningSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.ListeningSkills).FirstOrDefault();
            Hindi.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Hindi.SpeakingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.SpeakingSkills).FirstOrDefault();
            Hindi.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.TerminalAssessment).FirstOrDefault();
            Hindi.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 1).Select(x => x.AggregateGrade).FirstOrDefault();

            Hindi.ReadingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.ReadingSkills).FirstOrDefault();
            Hindi.WritingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.WritingSkills).FirstOrDefault();
            Hindi.ListeningSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.ListeningSkills).FirstOrDefault();
            Hindi.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Hindi.SpeakingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.SpeakingSkills).FirstOrDefault();
            Hindi.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.TerminalAssessment).FirstOrDefault();
            Hindi.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 1).Select(x => x.AggregateGrade).FirstOrDefault();



            //English Report
            English.ReadingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.ReadingSkills).FirstOrDefault();
            English.WritingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.WritingSkills).FirstOrDefault();
            English.ListeningSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.ListeningSkills).FirstOrDefault();
            English.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.WeeklyAssessment).FirstOrDefault();
            English.SpeakingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.SpeakingSkills).FirstOrDefault();
            English.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.TerminalAssessment).FirstOrDefault();
            English.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 2).Select(x => x.AggregateGrade).FirstOrDefault();

            English.ReadingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.ReadingSkills).FirstOrDefault();
            English.WritingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.WritingSkills).FirstOrDefault();
            English.ListeningSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.ListeningSkills).FirstOrDefault();
            English.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.WeeklyAssessment).FirstOrDefault();
            English.SpeakingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.SpeakingSkills).FirstOrDefault();
            English.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.TerminalAssessment).FirstOrDefault();
            English.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 2).Select(x => x.AggregateGrade).FirstOrDefault();

            English.ReadingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.ReadingSkills).FirstOrDefault();
            English.WritingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.WritingSkills).FirstOrDefault();
            English.ListeningSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.ListeningSkills).FirstOrDefault();
            English.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.WeeklyAssessment).FirstOrDefault();
            English.SpeakingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.SpeakingSkills).FirstOrDefault();
            English.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.TerminalAssessment).FirstOrDefault();
            English.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 2).Select(x => x.AggregateGrade).FirstOrDefault();

            //Punjabi.......

            Punjabi.ReadingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.ReadingSkills).FirstOrDefault();
            Punjabi.WritingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.WritingSkills).FirstOrDefault();
            Punjabi.ListeningSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.ListeningSkills).FirstOrDefault();
            Punjabi.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Punjabi.SpeakingSkills_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.SpeakingSkills).FirstOrDefault();
            Punjabi.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.TerminalAssessment).FirstOrDefault();
            Punjabi.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 3).Select(x => x.AggregateGrade).FirstOrDefault();

            Punjabi.ReadingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.ReadingSkills).FirstOrDefault();
            Punjabi.WritingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.WritingSkills).FirstOrDefault();
            Punjabi.ListeningSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.ListeningSkills).FirstOrDefault();
            Punjabi.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Punjabi.SpeakingSkills_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.SpeakingSkills).FirstOrDefault();
            Punjabi.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.TerminalAssessment).FirstOrDefault();
            Punjabi.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 3).Select(x => x.AggregateGrade).FirstOrDefault();

            Punjabi.ReadingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.ReadingSkills).FirstOrDefault();
            Punjabi.WritingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.WritingSkills).FirstOrDefault();
            Punjabi.ListeningSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.ListeningSkills).FirstOrDefault();
            Punjabi.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Punjabi.SpeakingSkills_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.SpeakingSkills).FirstOrDefault();
            Punjabi.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.TerminalAssessment).FirstOrDefault();
            Punjabi.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 3).Select(x => x.AggregateGrade).FirstOrDefault();

            //Mathematics
            Mathematics.ConceptsActivity_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.ConceptsActivity).FirstOrDefault();
            Mathematics.Tables_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.Tables).FirstOrDefault();
            Mathematics.MentelAbility_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.MentelAbility).FirstOrDefault();
            Mathematics.WrittenWork_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.WrittenWork).FirstOrDefault();
            Mathematics.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Mathematics.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.TerminalAssessment).FirstOrDefault();
            Mathematics.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 4).Select(x => x.AggregateGrade).FirstOrDefault();


            Mathematics.ConceptsActivity_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.ConceptsActivity).FirstOrDefault();
            Mathematics.Tables_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.Tables).FirstOrDefault();
            Mathematics.MentelAbility_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.MentelAbility).FirstOrDefault();
            Mathematics.WrittenWork_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.WrittenWork).FirstOrDefault();
            Mathematics.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Mathematics.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.TerminalAssessment).FirstOrDefault();
            Mathematics.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 4).Select(x => x.AggregateGrade).FirstOrDefault();


            Mathematics.ConceptsActivity_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.ConceptsActivity).FirstOrDefault();
            Mathematics.Tables_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.Tables).FirstOrDefault();
            Mathematics.MentelAbility_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.MentelAbility).FirstOrDefault();
            Mathematics.WrittenWork_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.WrittenWork).FirstOrDefault();
            Mathematics.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.WeeklyAssessment).FirstOrDefault();
            Mathematics.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.TerminalAssessment).FirstOrDefault();
            Mathematics.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 4).Select(x => x.AggregateGrade).FirstOrDefault();

            ///Environmental Studies

            EV.ActivityViva_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.ActivityViva).FirstOrDefault();
            EV.WrittenWork_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.WrittenWork).FirstOrDefault();
            EV.WeeklyAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.WeeklyAssessment).FirstOrDefault();
            EV.TerminalAssessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.TerminalAssessment).FirstOrDefault();
            EV.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 5).Select(x => x.AggregateGrade).FirstOrDefault();

            EV.ActivityViva_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.ActivityViva).FirstOrDefault();
            EV.WrittenWork_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.WrittenWork).FirstOrDefault();
            EV.WeeklyAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.WeeklyAssessment).FirstOrDefault();
            EV.TerminalAssessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.TerminalAssessment).FirstOrDefault();
            EV.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 5).Select(x => x.AggregateGrade).FirstOrDefault();

            EV.ActivityViva_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.ActivityViva).FirstOrDefault();
            EV.WrittenWork_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.WrittenWork).FirstOrDefault();
            EV.WeeklyAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.WeeklyAssessment).FirstOrDefault();
            EV.TerminalAssessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.TerminalAssessment).FirstOrDefault();
            EV.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 5).Select(x => x.AggregateGrade).FirstOrDefault();

            ////////...........COSCOLASTIC............SUBJECT ID
            //Art_and_Music... 
            Art_and_Music.ArtInterest_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.ArtInterest).FirstOrDefault();
            Art_and_Music.ArtCreativity_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.ArtCreativity).FirstOrDefault();
            Art_and_Music.MusicInterest_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.MusicInterest).FirstOrDefault();
            Art_and_Music.MusicRhythm_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.MusicRhythm).FirstOrDefault();

            Art_and_Music.ArtInterest_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.ArtInterest).FirstOrDefault();
            Art_and_Music.ArtCreativity_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.ArtCreativity).FirstOrDefault();
            Art_and_Music.MusicInterest_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.MusicInterest).FirstOrDefault();
            Art_and_Music.MusicRhythm_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.MusicRhythm).FirstOrDefault();

            Art_and_Music.ArtInterest_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.ArtInterest).FirstOrDefault();
            Art_and_Music.ArtCreativity_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.ArtCreativity).FirstOrDefault();
            Art_and_Music.MusicInterest_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.MusicInterest).FirstOrDefault();
            Art_and_Music.MusicRhythm_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.MusicRhythm).FirstOrDefault();


            //Physical Education and health
            Physical_Health.Enthusiasm_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Enthusiasm).FirstOrDefault();
            Physical_Health.Discipline_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Discipline).FirstOrDefault();
            Physical_Health.TeamSpirit_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.TeamSpirit).FirstOrDefault();

            Physical_Health.Enthusiasm_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Enthusiasm).FirstOrDefault();
            Physical_Health.Discipline_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Discipline).FirstOrDefault();
            Physical_Health.TeamSpirit_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.TeamSpirit).FirstOrDefault();

            Physical_Health.Enthusiasm_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Enthusiasm).FirstOrDefault();
            Physical_Health.Discipline_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Discipline).FirstOrDefault();
            Physical_Health.TeamSpirit_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.TeamSpirit).FirstOrDefault();


            //Aspects

            Aspects.Computerscience_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Computerscience).FirstOrDefault();
            Aspects.GeneralStudies_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.GeneralStudies).FirstOrDefault();
            Aspects.Valueeducation_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Valueeducation).FirstOrDefault();
            Aspects.YLE_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.Yle).FirstOrDefault();
            Aspects.French_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 6).Select(x => x.French).FirstOrDefault();

            Aspects.Computerscience_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Computerscience).FirstOrDefault();
            Aspects.GeneralStudies_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.GeneralStudies).FirstOrDefault();
            Aspects.Valueeducation_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Valueeducation).FirstOrDefault();
            Aspects.YLE_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.Yle).FirstOrDefault();
            Aspects.French_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 6).Select(x => x.French).FirstOrDefault();

            Aspects.Computerscience_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Computerscience).FirstOrDefault();
            Aspects.GeneralStudies_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.GeneralStudies).FirstOrDefault();
            Aspects.Valueeducation_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Valueeducation).FirstOrDefault();
            Aspects.YLE_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.Yle
            ).FirstOrDefault();
            Aspects.French_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 6).Select(x => x.French).FirstOrDefault();

            //Personality Development

            Personality.Courteousness_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Courteousness).FirstOrDefault();
            Personality.Confidence_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Confidence).FirstOrDefault();
            Personality.Careofbelongings_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Careofbelongings).FirstOrDefault();
            Personality.Neatness_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Neatness).FirstOrDefault();
            Personality.Punctuality_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Punctuality).FirstOrDefault();
            Personality.Initiative_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.Initiative).FirstOrDefault();
            Personality.SharingCaring_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 7).Select(x => x.SharingCaring).FirstOrDefault();

            Personality.Courteousness_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Courteousness).FirstOrDefault();
            Personality.Confidence_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Confidence).FirstOrDefault();
            Personality.Careofbelongings_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Careofbelongings).FirstOrDefault();
            Personality.Neatness_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Neatness).FirstOrDefault();
            Personality.Punctuality_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Punctuality).FirstOrDefault();
            Personality.Initiative_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.Initiative).FirstOrDefault();
            Personality.SharingCaring_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 7).Select(x => x.SharingCaring).FirstOrDefault();

            Personality.Courteousness_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Courteousness).FirstOrDefault();
            Personality.Confidence_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Confidence).FirstOrDefault();
            Personality.Careofbelongings_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Careofbelongings).FirstOrDefault();
            Personality.Neatness_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Neatness).FirstOrDefault();
            Personality.Punctuality_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Punctuality).FirstOrDefault();
            Personality.Initiative_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.Initiative).FirstOrDefault();
            Personality.SharingCaring_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 7).Select(x => x.SharingCaring).FirstOrDefault();


            ///hEALTH

            Physical_Health.Height_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.Height).FirstOrDefault();
            Physical_Health.Weight_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.Weight).FirstOrDefault();


            Physical_Health.Height_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.Height).FirstOrDefault();
            Physical_Health.Weight_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.Weight).FirstOrDefault();

            Physical_Health.Height_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.Height).FirstOrDefault();
            Physical_Health.Weight_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.Weight).FirstOrDefault();



            //Attendence_Aggre

            Attendence_Aggre.NoOfWorkingdays_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.NoOfWorkingdays).FirstOrDefault();
            Attendence_Aggre.DaysPresent_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.DaysPresent).FirstOrDefault();
            Attendence_Aggre.AggregateGrade_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 8).Select(x => x.AggregateGrade).FirstOrDefault();

            Attendence_Aggre.NoOfWorkingdays_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.NoOfWorkingdays).FirstOrDefault();
            Attendence_Aggre.DaysPresent_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.DaysPresent).FirstOrDefault();
            Attendence_Aggre.AggregateGrade_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 8).Select(x => x.AggregateGrade).FirstOrDefault();

            Attendence_Aggre.NoOfWorkingdays_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.NoOfWorkingdays).FirstOrDefault();
            Attendence_Aggre.DaysPresent_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.DaysPresent).FirstOrDefault();
            Attendence_Aggre.AggregateGrade_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 8).Select(x => x.AggregateGrade).FirstOrDefault();

            //Assessment
            Assessment.Assessment_I = data.Where(x => x.AssesmentId == 5 && x.SubjectId == 9).Select(x => x.Remarks).FirstOrDefault();
            Assessment.Assessment_II = data.Where(x => x.AssesmentId == 6 && x.SubjectId == 9).Select(x => x.Remarks).FirstOrDefault();
            Assessment.Assessment_III = data.Where(x => x.AssesmentId == 7 && x.SubjectId == 9).Select(x => x.Remarks).FirstOrDefault();




            mo.English_Report = English;
            mo.Hindi_Report = Hindi;
            mo.Punjabi_Report = Punjabi;
            mo.Mathematics_Report = Mathematics;
            mo.EV_Report = EV;
            mo.Art_and_Music_Report = Art_and_Music;
            mo.Physical_Education_Health_Report = Physical_Health;
            mo.Personality_Report = Personality;
            mo.Aspects_Report = Aspects;
            mo.Attendence_Report = Attendence_Aggre;
            mo.Assessment_Report = Assessment;
            mo.Title_Report = Titl;
            return View(mo);
        }
        public IActionResult VProgressCard_Previous(string id)
        {
            V_ProgressCard model = new V_ProgressCard();
            string[] splitData = id.Split('~');
            //model.StudentId = Convert.ToInt64(id);
            model.StudentId = Convert.ToInt64(splitData[0]);
            long examDeclaredId = Convert.ToInt64(splitData[1]);
            long classId = Convert.ToInt64(splitData[2]);
            long divisionid = Convert.ToInt64(splitData[3]);
            var studentDetails = _Entities.TbStudents.Where(x => x.StudentId == model.StudentId).FirstOrDefault();
            model.ReportName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            model.SchoolName = studentDetails.School.SchoolName;
            model.SchoolLogo = "http://satluj.schoolman.in" + studentDetails.School.FilePath;
            Int64 Academicyear = _Entities.TbClasses.Where(x => x.ClassId == classId).Select(x => x.AcademicYearId).FirstOrDefault();
            model.AcademicYear = _Entities.TbAcademicYears.Where(x => x.YearId == Academicyear).Select(x=>x.AcademicYear).FirstOrDefault();
            //model.StudentProfile = "http://satluj.schoolman.in" + studentDetails.FilePath;
            model.StudentProfile = studentDetails.FilePath;
            //model.StudentProfile = "http://localhost:16135" + studentDetails.FilePath;
            model.StudentName = studentDetails.StundentName;
            string classname= _Entities.TbClasses.Where(x => x.ClassId == classId).Select(x => x.Class).FirstOrDefault();
            string Divisionname = _Entities.TbDivisions.Where(x => x.DivisionId == divisionid).Select(x => x.Division).FirstOrDefault();
            model.ClassDivision = classname + " " + Divisionname;
            model.RollNo = studentDetails.ClasssNumber;
            model.AdmissionNo = studentDetails.StudentSpecialId;
            try
            {
                model.DOB = Convert.ToDateTime(studentDetails.Dob).ToString("dd/MM/yyyy") ?? "";
            }
            catch
            {
                model.DOB = Convert.ToDateTime(studentDetails.Dob).ToShortDateString() ?? "";
            }
            model.FatherName = studentDetails.ParentName;
            model.MotherName = studentDetails.MotherName ?? "";
            var assesmentList = _Entities.TbVAssesments.Where(x => x.SchoolId == studentDetails.SchoolId && x.IsActive).OrderBy(x => x.Period.StartDate).ToList();

            //-------------CASE I---------------------
            #region Case I
            model._CaseI = new List<CaseI>();
            var subjects = _Entities.TbSubjects.Where(x => x.EnumTypeId == 0 && x.IsActive && x.SchoolI == studentDetails.SchoolId).ToList();
            if (subjects != null && subjects.Count > 0 && assesmentList != null && assesmentList.Count > 0)
            {
                int i = 0;
                foreach (var item in subjects)
                {
                    i = i + 1;
                    if (i < 4)
                    {
                        CaseI one = new CaseI();
                        one.SubjectName = item.SubjectName;
                        one._CaseIDetails = new List<CaseIDetails>();
                        int y = 0;
                        foreach (var item2 in assesmentList)
                        {
                            decimal aggregate = 0;
                            int aggregateCount = 0;
                            CaseIDetails two = new CaseIDetails();
                            if (y == 0)
                                two.Assesment = "I";
                            else if (y == 1)
                                two.Assesment = "II";
                            else
                                two.Assesment = "III";
                            var reading = _Entities.TbReadingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId==divisionid && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (reading != null)
                            {
                                two.Pronun = GradeCreation(reading.Pronun, studentDetails.SchoolId, VSpecialMarks.ReadingSkill, "Pronun");
                                two.Fluency = GradeCreation(reading.Fluency, studentDetails.SchoolId, VSpecialMarks.ReadingSkill, "Fluency");
                                if (reading.Pronun != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(reading.Pronun);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (reading.Fluency != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(reading.Fluency);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Pronun = "";
                                two.Fluency = "";
                            }
                            var writing = _Entities.TbWritingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == divisionid && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (writing != null)
                            {
                                two.Creative_Writing = GradeCreation(writing.CreativeWriting, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Creative_Writing");
                                two.Hand_Writing = GradeCreation(writing.HandWriting, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Hand_Writing");
                                two.Grammar = GradeCreation(writing.Grammar, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Grammar");
                                two.Spelling = GradeCreation(writing.Spellings, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Spelling");
                                two.Vocabulary = GradeCreation(writing.Vocabulary, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Vocabulary");
                                two.Literature = GradeCreation(writing.UnitTest, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Literature");
                                two.Weekly_Assesment = GradeCreation(writing.WorkSheet, studentDetails.SchoolId, VSpecialMarks.WritingSkill, "Weekly_Assesment");
                                if (writing.CreativeWriting != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.CreativeWriting);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.HandWriting != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.HandWriting);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.Grammar != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.Grammar);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.Spellings != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.Spellings);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.Vocabulary != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.Vocabulary);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.UnitTest != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.UnitTest);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (writing.WorkSheet != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(writing.WorkSheet);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Creative_Writing = "";
                                two.Hand_Writing = "";
                                two.Grammar = "";
                                two.Spelling = "";
                                two.Vocabulary = "";
                                two.Literature = "";
                                two.Weekly_Assesment = "";
                            }
                            var speak = _Entities.TbSpeakingSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == divisionid && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (speak != null)
                            {
                                two.Conversation = GradeCreation(speak.Conversation, studentDetails.SchoolId, VSpecialMarks.SpeakingSkills, "Conversation");
                                two.Recitation = GradeCreation(speak.Recitation, studentDetails.SchoolId, VSpecialMarks.SpeakingSkills, "Recitation");
                                if (speak.Conversation != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(speak.Conversation);
                                    aggregateCount = aggregateCount + 1;
                                }
                                if (speak.Recitation != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(speak.Recitation);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Conversation = "";
                                two.Recitation = "";
                            }
                            var listen = _Entities.TbListenningSkills.Where(x => x.StudentId == studentDetails.StudentId && x.DivisionId == divisionid && x.SubjectId == item.SubId && x.AssesmentId == item2.Id && x.IsActive).FirstOrDefault();
                            if (listen != null)
                            {
                                two.Comprehension = GradeCreation(listen.Comprehension, studentDetails.SchoolId, VSpecialMarks.ListeningSkill, "Comprehension");
                                if (listen.Comprehension != "")
                                {
                                    aggregate = aggregate + Convert.ToDecimal(listen.Comprehension);
                                    aggregateCount = aggregateCount + 1;
                                }
                            }
                            else
                            {
                                two.Comprehension = "";
                            }
                            if (aggregateCount != 0)
                            {
                                //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                                two.Aggregate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 1);
                            }
                            else
                            {
                                two.Aggregate = "";
                            }

                            one._CaseIDetails.Add(two);
                            y = y + 1;
                        }
                        model._CaseI.Add(one);
                    }
                }
            }
            #endregion

            //-------------CASE II--------------------
            #region 
            model._CaseII = new List<CaseII>();
            var subjectsI = _Entities.TbSubjects.Where(x => x.EnumTypeId == 1 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int p = 0;
            if (subjectsI != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    var data = _Entities.TbAspectsMathsSkills.Where(x => x.SubjectId == subjectsI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseII one = new CaseII();
                    if (p == 0)
                        one.Assesment = "I";
                    else if (p == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Concept = GradeCreation(data.Concepts, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Concept");
                        one.Activity = GradeCreation(data.Activity, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Activity");
                        one.Tables = GradeCreation(data.Tables, studentDetails.SchoolId, VSpecialMarks.AspectsI, "Tables");
                        one.MentalAbility = GradeCreation(data.MentalAbility, studentDetails.SchoolId, VSpecialMarks.AspectsI, "MentalAbility");
                        one.WrittenWork = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsI, "WrittenWork");
                        one.WeeklyAssesment = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsI, "WeeklyAssesment");
                        if (data.Concepts != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Concepts);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.Activity != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Activity);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.Tables != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Tables);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.MentalAbility != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.MentalAbility);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Concept = "";
                        one.Activity = "";
                        one.Tables = "";
                        one.MentalAbility = "";
                        one.WrittenWork = "";
                        one.WeeklyAssesment = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 2);
                    }
                    else
                    {
                        one.Aggregate = "";
                    }
                    model._CaseII.Add(one);
                    p = p + 1;
                }
            }

            #endregion

            //-------------CASE III-------------------
            #region 
            model._CaseIII = new List<CaseIII>();
            var subjectsII = _Entities.TbSubjects.Where(x => x.EnumTypeId == 2 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int q = 0;
            if (subjectsII != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsNaturalSciencesSkills.Where(x => x.SubjectId == subjectsII.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseIII one = new CaseIII();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (q == 0)
                        one.Assesment = "I";
                    else if (q == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Activity_Pproject = GradeCreation(data.ActivityProject, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Activity_Pproject");
                        one.Group_Discussion = GradeCreation(data.GroupDiscussion, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Group_Discussion");
                        one.Written_Work = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Written_Work");
                        one.Work_Sheet = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsII, "Work_Sheet");
                        if (data.ActivityProject != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.ActivityProject);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.GroupDiscussion != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.GroupDiscussion);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Activity_Pproject = "";
                        one.Group_Discussion = "";
                        one.Written_Work = "";
                        one.Work_Sheet = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregrate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 3);
                    }
                    else
                    {
                        one.Aggregrate = "";
                    }
                    model._CaseIII.Add(one);
                    q = q + 1;
                }
            }

            #endregion

            //-------------CASE IV--------------------
            #region 
            model._CaseIV = new List<CaseIV>();
            var subjectsIII = _Entities.TbSubjects.Where(x => x.EnumTypeId == 4 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int ab = 0;
            if (subjectsIII != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsSocialStudiesSkills.Where(x => x.SubjectId == subjectsIII.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseIV one = new CaseIV();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (ab == 0)
                        one.Assesment = "I";
                    else if (ab == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Activity_Project = GradeCreation(data.ActivityProject, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Activity_Project");
                        one.Viva = GradeCreation(data.Viva, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Viva");
                        one.Written_Work = GradeCreation(data.WrittenWork, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "Written_Work");
                        one.WorkSheet = GradeCreation(data.WorkSheet, studentDetails.SchoolId, VSpecialMarks.AspectsIII, "WorkSheet");
                        if (data.ActivityProject != "")
                        {
                            aggregate = Convert.ToDecimal(data.ActivityProject);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.Viva != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.Viva);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WrittenWork != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WrittenWork);
                            aggregateCount = aggregateCount + 1;
                        }
                        if (data.WorkSheet != "")
                        {
                            aggregate = aggregate + Convert.ToDecimal(data.WorkSheet);
                            aggregateCount = aggregateCount + 1;
                        }
                    }
                    else
                    {
                        one.Activity_Project = "";
                        one.Viva = "";
                        one.Written_Work = "";
                        one.WorkSheet = "";
                    }
                    if (aggregateCount != 0)
                    {
                        //string grandAggregate = Convert.ToString(aggregate / aggregateCount);
                        one.Aggregrate = GradeCreationCase(aggregate.ToString(), studentDetails.SchoolId, 4);
                    }
                    else
                    {
                        one.Aggregrate = "";
                    }
                    model._CaseIV.Add(one);
                    ab = ab + 1;
                }
            }
            #endregion

            //-------------CASE V---------------------
            #region 
            model._CaseV = new List<CaseV>();
            var subjectsIV = _Entities.TbSubjects.Where(x => x.EnumTypeId == 5 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int abc = 0;
            if (subjectsIV != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbAspectsRelativeAreaSkills.Where(x => x.SubjectId == subjectsIV.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseV one = new CaseV();
                    decimal aggregate = 0;
                    int aggregateCount = 0;
                    if (abc == 0)
                        one.Assesment = "I";
                    else if (abc == 1)
                        one.Assesment = "II";
                    else
                    {
                        one.Assesment = "III";
                    }
                    if (data != null)
                    {
                        one.Computer_Science = GradeCreation(data.ComputerScience, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "Computer_Science");
                        one.GeneralStudies = GradeCreation(data.GeneralStudies, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "GeneralStudies");
                        one.ValueEducation = GradeCreation(data.ValueEducation, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "ValueEducation");
                        one.French = GradeCreation(data.French, studentDetails.SchoolId, VSpecialMarks.AspectsIV, "French");
                        one.SmartClass = data.SmartClass; // Samrt Class Result is Grade, so not wants to calculate the score 
                    }
                    else
                    {
                        one.Computer_Science = "";
                        one.GeneralStudies = "";
                        one.ValueEducation = "";
                        one.French = "";
                        one.SmartClass = "";
                    }
                    #region Aggregate 
                    //if(one.Computer_Science != "")
                    //{
                    //    aggregate = Convert.ToDecimal(data.COMPUTER_SCIENCE);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.GeneralStudies!="")
                    //{
                    //    aggregate = aggregate+ Convert.ToDecimal(data.GENERAL_STUDIES);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.ValueEducation != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.VALUE_EDUCATION);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.French != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.FRENCH);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (one.SmartClass != "")
                    //{
                    //    aggregate = aggregate + Convert.ToDecimal(data.SMART_CLASS);
                    //    aggregateCount = aggregateCount + 1;
                    //}
                    //if (aggregateCount != 0)
                    //{
                    //    string grandAggregate =  Convert.ToString(aggregate / aggregateCount);
                    //    one.Aggregate = GradeCreation(grandAggregate);
                    //}
                    //else
                    //{
                    //    one.Aggregate = "";
                    //}
                    #endregion
                    model._CaseV.Add(one);
                    abc = abc + 1;
                }
            }
            #endregion

            //-------------CASE V---------------------
            #region 
            model._CaseVI = new List<CaseVI>();
            var subjectsVI = _Entities.TbSubjects.Where(x => x.EnumTypeId == 3 && x.IsActive && x.SchoolI == studentDetails.SchoolId).FirstOrDefault();
            int abcd = 0;
            if (subjectsVI != null && assesmentList != null && assesmentList.Count > 0)
            {
                foreach (var item in assesmentList)
                {
                    var data = _Entities.TbMusicDanceSkills.Where(x => x.SubjectId == subjectsVI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    var data1 = _Entities.TbArtCraftSkills.Where(x => x.SubjectId == subjectsVI.SubId && x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                    CaseVI one = new CaseVI();
                    if (abcd == 0)
                        one.assesment = "I";
                    else if (abcd == 1)
                        one.assesment = "II";
                    else
                    {
                        one.assesment = "III";
                    }
                    if (data1 != null)
                    {
                        one.artInterest = data1.Interest;
                        one.artCreativity = data1.Creativity;
                    }
                    else
                    {
                        one.artInterest = "";
                        one.artCreativity = "";
                    }
                    if (data != null)
                    {
                        one.musicInterest = data.Interest;
                        one.musicRhythm = data.Rhythm;
                    }
                    else
                    {
                        one.musicInterest = "";
                        one.musicRhythm = "";
                    }
                    model._CaseVI.Add(one);
                    abcd = abcd + 1;
                }
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseVII = new List<CaseVII>();
            int abcde = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVGames.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                var data1 = _Entities.TbVHealths.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseVII one = new CaseVII();
                if (abcde == 0)
                    one.Assesment = "I";
                else if (abcde == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data1 != null)
                {
                    one.Height = data1.Height;
                    one.Weight = data1.Weight;
                }
                else
                {
                    one.Height = "";
                    one.Weight = "";
                }
                if (data != null)
                {
                    one.Enthusiasm = data.Enthusiasm;
                    one.TeamSpirit = data.TeamSpirit;
                    one.Discipline = data.Discipline;
                }
                else
                {
                    one.Enthusiasm = "";
                    one.TeamSpirit = "";
                    one.Discipline = "";
                }
                model._CaseVII.Add(one);
                abcde = abcde + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseVIII = new List<CaseVIII>();
            int abcdef = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVPersonalityDevelopments.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseVIII one = new CaseVIII();
                if (abcdef == 0)
                    one.Assesment = "I";
                else if (abcdef == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data != null)
                {
                    one.Courteousness = data.Courteousness;
                    one.Confident = data.Confident;
                    one.CareOfBelongings = data.CareOfBelongings;
                    one.Neatness = data.Neatness;
                    one.Punctuality = data.Punctuality;
                    one.Initiative = data.Initiative;
                    one.Sharing_Caring = data.SharingCaring;
                    one.Property = data.Property;
                }
                else
                {
                    one.Courteousness = "";
                    one.Confident = "";
                    one.CareOfBelongings = "";
                    one.Neatness = "";
                    one.Punctuality = "";
                    one.Initiative = "";
                    one.Sharing_Caring = "";
                    one.Property = "";
                }
                model._CaseVIII.Add(one);
                abcdef = abcdef + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseIX = new List<CaseIX>();
            int abcdefg = 0;
            foreach (var item in assesmentList)
            {
                var data = _Entities.TbVRemarks.Where(x => x.StudentId == studentDetails.StudentId && x.AssesmentId == item.Id && x.IsActive).FirstOrDefault();
                CaseIX one = new CaseIX();
                if (abcdefg == 0)
                    one.Assesment = "I";
                else if (abcdefg == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                if (data != null)
                {
                    var r1 = data.Remark == null ? "" : data.Remark;
                    var r2 = data.AdditionalRemarks == null ? "" : data.AdditionalRemarks.ToUpper();
                    if (r1.Trim() != "")
                    {
                        if (r2.Trim() != string.Empty)
                            one.Reamrk = r1 + " , " + r2;
                        else
                            one.Reamrk = r1;
                    }
                    else
                        one.Reamrk = r2;
                }
                else
                {
                    one.Reamrk = "";
                }
                model._CaseIX.Add(one);
                abcdefg = abcdefg + 1;
            }
            #endregion

            //-------------CASE VII---------------------
            #region 
            model._CaseX = new List<CaseX>();
            int abcdefgh = 0;
            foreach (var item in assesmentList)
            {
                DateTime startDate = item.Period.StartDate;
                DateTime endDate = item.Period.EndDate;
                //var attendance = _Entities.tb_Attendance.Where(x => x.StudentId == studentDetails.StudentId && x.IsActive == true && x.ShiftStatus == 0 && System.Data.Entity.DbFunctions.TruncateTime(x.AttendanceDate) >= startDate.Date && System.Data.Entity.DbFunctions.TruncateTime(x.AttendanceDate) <= endDate.Date).ToList();// Data From the Attendnace Table 
                var attendance = _Entities.TbAttendancePeriods.Where(x => x.StudentId == studentDetails.StudentId && x.IsActive == true && x.PeriodId == item.Period.Id).FirstOrDefault();
                CaseX one = new CaseX();
                if (abcdefgh == 0)
                    one.Assesment = "I";
                else if (abcdefgh == 1)
                    one.Assesment = "II";
                else
                {
                    one.Assesment = "III";
                }
                //if (attendance != null && attendance.Count > 0) // DON'T DELETE 
                //{
                //    one.TotalWorkingDays = Convert.ToString(attendance.Count);
                //    one.PresentDays = Convert.ToString(attendance.Where(x => x.AttendanceData == true).ToList().Count());
                //}
                if (attendance != null)
                {
                    one.TotalWorkingDays = Convert.ToString(attendance.TotalDays);
                    one.PresentDays = Convert.ToString(attendance.PresentDays);
                }
                else
                {
                    one.TotalWorkingDays = "";
                    one.PresentDays = "";
                }
                model._CaseX.Add(one);
                abcdefgh = abcdefgh + 1;
            }
            #endregion

            return View(model);
        }
    }
}

















   
