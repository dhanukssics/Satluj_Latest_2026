using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Satluj_Latest
{
    public class Enum
    {

    }
    public enum UserRole
    {
        School = 1,
        Staff = 2,
        Teacher = 3,
        Parent = 4,
        Master = 5
    }

    public enum FeeType
    {
        CommonFee = 1, //Fees for all student
        SpecialFee = 2
    }
    public enum MessageType
    {
        ForStudent = 1,
        ForClass = 2,
        ForSchool = 3
    }
    public enum DataFromStatus
    {
        Cash = 0,
        Bill = 1,
        Bank = 2
    }
    public enum TravellingStatus
    {
        Start = 0,
        Ongoing = 1,
        Stop = 2,
        NotUpdated = 4
    }
    public enum SMSSendType
    {
        Student = 0,
        Staff = 1
    }
    public enum AccountType
    {
        Expense = 0,
        Income = 1,
        Reverse = 2
    }
    public enum BankType
    {
        Deposit = 0, // Expense // Debit
        Withdraw = 1// Income // Credit
    }
    public enum AssetsLiabilityType
    {
        Assets = 0,
        Liability = 1
    }
    public enum PaymentBillType
    {
        NormalBill = 0,
        BillDue = 1,
        FineBill = 2
    }
    public enum Days
    {
        Monday = 0,
        Tuesday = 1,
        Wednesday = 2,
        Thursday = 3,
        Friday = 4,
        Saturday = 5
    }
    public enum Periods
    {
        One = 0,
        two = 1,
        three = 2,
        four = 3,
        five = 4,
        six = 5,
        seven = 6,
        eight = 7
    }
    public enum AttendanceShift
    {
        Morning = 0,
        Evening = 1
    }
    public enum AttendanceStatus
    {
        NotTaken = 0,
        Present = 1,
        Absent = 2
    }
    public enum SchoolMsgFromApp
    {
        FullSchool = 0,
        ClassWise = 1
    }
    public enum StockStatus
    {
        Active = 0,
        InActive = 1,
        Broken = 2
    }
    public enum PaymentMode
    {// in Billing section, the payment mode is hard coded , the data is starting from 1 . But the can't be start with 1 , becouse here we must have the 0th data.
        // so here we take the value from the enum with add 2. ie.,
        //Cash = 0,//=1
        Cheque = 0,//=2
        Other = 1//=3
    }
    public enum Nationality
    {
        Indian = 0
    }
    public enum Country
    {
        India = 0
    }
    public enum StudentCategory
    {
        General = 0,
        OBC = 1,
        SCST = 2,
        BPL = 3,
        BC = 4,
        BCB = 5,
        MULWAL = 6,
        SC=2
    }
    public enum UserTypeModule
    {
        Staff = 0,
        Teacher = 1
    }
    public enum Assesments
    {
        [Display(Name = "Assesment I")]
        AssesmentI = 0,

        [Display(Name = "Assesment II")]
        AssesmentII = 1,

        [Display(Name = "Assesment III")]
        AssesmentIII = 2
    }
    public enum VSpecialMarks
    {
        [Display(Name = "READING SKILLS")]
        ReadingSkill = 10,
        [Display(Name = "WRITING SKILLS")]
        WritingSkill = 12, /*Same 0*/
        [Display(Name = "SPEAKING SKILLS")]
        SpeakingSkills = 11,/*Same 0*/
        [Display(Name = "LISTENING SKILLS")]
        ListeningSkill = 13,/*Same 0*/
        [Display(Name = "MATHEMATICS ASPECTS")]
        AspectsI = 15,
        [Display(Name = "NATURAL SCIENCE ASPECTS")]
        AspectsII = 16,
        [Display(Name = "ART / CRAFT")]
        ArtAndCraft = 6,
        [Display(Name = "MUSIC / DANCE")]
        MusicAndDance = 7,
        [Display(Name = "SOCIAL STUDIES ASPECTS")]
        AspectsIII = 17,
        [Display(Name = "RELATIVE AREAS ASPECTS")]
        AspectsIV = 9
    }
       
    public enum VNonSubjectSkills
    {
        [Display(Name = "PHYSICAL EDUCATION")]
        PhysicalEducation = 0,
        [Display(Name = "HEALTH")]
        Health = 1,
        [Display(Name = "PERSONALITY DEVELOPMENT")]
        Personality_Development = 2,
        [Display(Name = "REMARKS")]
        Remark = 3
    }
    public enum UsersDesignation
    {
        [Display(Name = "Link Officer")]
        Link_Officer = 0,
        HOD = 1,
        HOY = 2,
        [Display(Name = "Deputy HOY")]
        Deputy_HOY = 3,
        Others = 4
    }


    /////////////////////////////
    ///////////////////////////////////////
    public enum Others_Subjects
    {
        [Display(Name = "English")]
        English = 1,
        [Display(Name = "Hindi")]
        Hindi = 2, /*Same 0*/
        [Display(Name = "Mathematics")]
        Mathematics = 3,/*Same 0*/
        [Display(Name = "Environmental Studies")]
        EnvironmentalStudies = 4,/*Same 0*/
    }
    public enum enum_English
    {
        [Display(Name = "Writing Skills")]
        WritingSkills = 1,
        [Display(Name = "Reading Skills")]
        ReadingSkills = 2,  
        [Display(Name = "Speaking Skills")]
        SpeakingSkills = 3, 
        [Display(Name = "Reciting Skills")]
        RecitingSkills = 4,
        [Display(Name = "Listening Skills")]
        ListeningSkills = 5,
        [Display(Name = "Oral")]
        Oral = 6,
    }
   

}
