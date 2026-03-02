

namespace Satluj_Latest.Models
{
    public class SpecialClassModel
    {
        public long SchoolId { get; set; }
        public long PeriodId { get; set; }
        public Assesments AssesmentId { get; set; }
    }
    public class SubjectScoreModel
    {
        public long ClassId { get; set; }
        public long SchoolId { get; set; }
        public long AssesmentTableId { get; set; }
        public long DivisionId { get; set; }
        public long SubjectId { get; set; }
        public VSpecialMarks VSpecialMarksId { get; set; }
        public long UserId { get; set; }
        public int IsAdminCheck { get; set; }
        public string StartDateString { get; set; }
        public string Academicyear { get;set; }
    }
    public class StudentSkillMarks
    {
        public List<SkillModel> _SkillModel { get; set; }
        public List<LKG_UKG_NURSERY_SKILLS_Model> _Lkg_Ukg_Mo { get; set; }

        public List<ONE_FOUR_SKILLS_MODEL> _ONE_FOUR { get; set; }
        public long ClassId { get; set; }
        public long SchoolId { get; set; }
        public long DivisionId { get; set; }
        public long AssesmentId { get; set; }
        public long SubjectId { get; set; }
        public string Languages { get; set; }
    }
    public class SkillModel
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public int RolleNo { get; set; }
        public long ClassId { get; set; }
        public string Languages { get; set; }
        public string Reading_Skill { get; set; }
        public string Writing_Skill { get; set; }
        public string Speaking_Skill { get; set; }
        public string Listening_Skill { get; set; }
        public LKG_UKG_NURSERY_SKILLS_Model Lkg_Ukg_ner { get;set; }
        public ReadingSkill _ReadingSkill { get; set; }
        public WritingSkill _WritingSkill { get; set; }
        public SpeakingSkill _SpeakingSkill { get; set; }
        public ListenningSkill _ListenningSkill { get; set; }
        public AspectsMaths _AspectsMaths { get; set; }
        public AspectsNaturalScience _AspectsNaturalScience { get; set; }
        public ArtCraft _ArtCraft { get; set; }
        public MusicDance _MusicDance { get; set; }
        public AspectsSocialStudies _AspectsSocialStudies { get; set; }
        public AspectsRelativeAreas _AspectsRelativeAreas { get; set; }


        public ONE_FOUR_SKILLS_MODEL ONE_FOUR { get; set; }

    }
    public class ReadingSkill
    {
        public string Pronun { get; set; }
        public string Fluency { get; set; }
    }
    public class WritingSkill
    {
        public string CreativeThinking { get; set; }
        public string HandWriting { get; set; }
        public string Grammer { get; set; }
        public string Spelling { get; set; }
        public string Vocabulary { get; set; }
        public string UnitTest { get; set; }
        public string WorkSheet { get; set; }
    }
    public class SpeakingSkill
    {
        public string Conversation { get; set; }
        public string Recitation { get; set; }
    }
    public class ListenningSkill
    {
        public string Comprehension { get; set; }
    }
    public class AspectsMaths
    {
        public string Concepts { get; set; }
        public string Activity { get; set; }
        public string Tables { get; set; }
        public string MentalAbility { get; set; }
        public string WrittenWork { get; set; }
        public string WorkSheet { get; set; }
    }
    public class AspectsNaturalScience
    {
        public string ActivityProject { get; set; }
        public string GroupDiscussion { get; set; }
        public string WrittenWork { get; set; }
        public string WorkSheet { get; set; }
    }
    public class ArtCraft
    {
        public string Interest { get; set; }
        public string Creativity { get; set; }
    }
    public class MusicDance
    {
        public string Interest { get; set; }
        public string Rhythm { get; set; }
    }
    public class AspectsSocialStudies
    {
        public string ActivityProject { get; set; }
        public string Viva { get; set; }
        public string WrittenWork { get; set; }
        public string WorkSheet { get; set; }
    }
    public class AspectsRelativeAreas
    {
        public string ComputerScience { get; set; }
        public string GeneralStudies { get; set; }
        public string Valueeducation { get; set; }
        public string French { get; set; }
        public string SmartClass { get; set; }
    }
    //---------------------------------------------------------------------
    public class NonSubjectScoreModel
    {
        public long SchoolId { get; set; }
        public long AssesmentTableId { get; set; }
        public long DivisionId { get; set; }
        public VNonSubjectSkills VMarkTypeId { get; set; }
    }

    public class StudentNonSubjectSkillMarks
    {
        public List<NonSubjectSkillModel> _SkillModel { get; set; }
        public long SchoolId { get; set; }
        public long DivisionId { get; set; }
        public long AssesmentId { get; set; }
    }

    public class NonSubjectSkillModel
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public int RollNo { get; set; }
        public GamesScore _GamesScore { get; set; }
        public HealthScore _HealthScore { get; set; }
        public Personality_Development _Personality_Development { get; set; }
        public RemarkSchore _RemarkSchore { get; set; }
    }

    public class GamesScore
    {
        public string Enthusiasm { get; set; }
        public string Discipline { get; set; }
        public string TeamSpirit { get; set; }
    }
    public class HealthScore
    {
        public string Height { get; set; }
        public string Weight { get; set; }
        public int WorkingDays { get; set; }
        public int Present { get; set; }
    }
    public class Personality_Development
    {
        public string Courteousness { get; set; }
        public string Confident { get; set; }
        public string CareOfBelongings { get; set; }
        public string Neatness { get; set; }
        public string Punctuality { get; set; }
        public string Initiative { get; set; }
        public string SharingCaring { get; set; }
        public string Property { get; set; }
    }
    public class RemarkSchore
    {
        public string Remark { get; set; }
        public string AdditionalRemark { get; set; }
    }
    public class VProgressCardList
    {
        public long SchoolId { get; set; }
        public long DivisionId { get; set; }
        public string ProgressCardName { get; set; }
        public List<StudentModel> TempMo1 { get; set; }
    }
    //-------------------------------------------------------------------
    public class V_ProgressCard
    {
        public long StudentId { get; set; }
        public string ReportName { get; set; }
        public string SchoolName { get; set; }
        public string SchoolLogo { get; set; }
        public string AcademicYear { get; set; }
        public string StudentProfile { get; set; }
        public string StudentName { get; set; }
        public string ClassDivision { get; set; }
        public string RollNo { get; set; }
        public string AdmissionNo { get; set; }
        public string DOB { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string AffiliationNo { get; set; }
        public List<CaseI> _CaseI { get; set; }
        public List<CaseII> _CaseII { get; set; }
        public List<CaseIII> _CaseIII { get; set; }
        public List<CaseIV> _CaseIV { get; set; }
        public List<CaseV> _CaseV { get; set; }
        public List<CaseVI> _CaseVI { get; set; }
        public List<CaseVII> _CaseVII { get; set; }
        public List<CaseVIII> _CaseVIII { get; set; }
        public List<CaseIX> _CaseIX { get; set; }
        public List<CaseX> _CaseX { get; set; }
    }


    public class CaseI// Reading,Listening ...etc ie., language results
    {
        public string SubjectName { get; set; }
        public List<CaseIDetails> _CaseIDetails { get; set; }
    }
    public class CaseIDetails
    {
        public string Assesment { get; set; }
        //--------------------------------------
        public string Pronun { get; set; }
        public string Fluency { get; set; }
        //--------------------------------------
        public string Creative_Writing { get; set; }
        public string Hand_Writing { get; set; }
        public string Grammar { get; set; }
        public string Spelling { get; set; }
        public string Vocabulary { get; set; }
        public string Literature { get; set; }
        public string Weekly_Assesment { get; set; }
        //--------------------------------------
        public string Conversation { get; set; }
        public string Recitation { get; set; }
        //--------------------------------------
        public string Comprehension { get; set; }
        //--------------------------------------
        public string Aggregate { get; set; }
    }

    public class CaseII // Mathematics
    {
        public string Assesment { get; set; }
        public string Concept { get; set; }
        public string Activity { get; set; }
        public string Tables { get; set; }
        public string MentalAbility { get; set; }
        public string WrittenWork { get; set; }
        public string WeeklyAssesment { get; set; }
        public string Aggregate { get; set; }
    }

    public class CaseIII //Natural Science
    {
        public string Activity_Pproject { get; set; }
        public string Group_Discussion { get; set; }
        public string Written_Work { get; set; }
        public string Work_Sheet { get; set; }
        public string Aggregrate { get; set; }
        public string Assesment { get; set; }
    }

    public class CaseIV // Social Studies
    {
        public string Assesment { get; set; }
        public string Activity_Project { get; set; }
        public string Viva { get; set; }
        public string Written_Work { get; set; }
        public string WorkSheet { get; set; }
        public string Aggregrate { get; set; }
    }

    public class CaseV // Relative Areas
    {
        public string Assesment { get; set; }
        public string Computer_Science { get; set; }
        public string GeneralStudies { get; set; }
        public string ValueEducation { get; set; }
        public string French { get; set; }
        public string SmartClass { get; set; }
        public string Aggregate { get; set; }
    }
    public class CaseVI // ART / CRAFT & MUSIC / DANCE
    {
        public string assesment { get; set; }
        public string artInterest { get; set; }
        public string artCreativity { get; set; }
        public string musicInterest { get; set; }
        public string musicRhythm { get; set; }
    }

    public class CaseVII //Game and Health
    {
        public string Enthusiasm { get; set; }
        public string Discipline { get; set; }
        public string TeamSpirit { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Assesment { get; set; }

    }
    public class CaseVIII // Personality Development
    {
        public string Courteousness { get; set; }
        public string Confident { get; set; }
        public string CareOfBelongings { get; set; }
        public string Neatness { get; set; }
        public string Punctuality { get; set; }
        public string Initiative { get; set; }
        public string Sharing_Caring { get; set; }
        public string Property { get; set; }
        public string Assesment { get; set; }
    }

    public class CaseIX // Reamrk
    {
        public string Assesment { get; set; }
        public string Reamrk { get; set; }
    }

    public class CaseX// Attendance
    {
        public string Assesment { get; set; }
        public string TotalWorkingDays { get; set; }
        public string PresentDays { get; set; }
    }

    public class AggregateScore
    {
        public int assessmentId { get; set; }
        public double AggregaeScore { get; set; }
    }










}