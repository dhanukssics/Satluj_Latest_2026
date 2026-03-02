using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbClass
{
    private TbClass @class;
    public TbClass() { }

    public TbClass(TbClass @class)
    {
        this.@class = @class;
    }

    public long ClassId { get; set; }

    public long SchoolId { get; set; }

    public string Class { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public Guid ClassGuild { get; set; }

    public bool IsActive { get; set; }

    public int ClassOrder { get; set; }

    public bool PublishStatus { get; set; }

    public long AcademicYearId { get; set; }

    public virtual TbAcademicYear AcademicYear { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbAcademicPeriod> TbAcademicPeriods { get; set; } = new List<TbAcademicPeriod>();

    public virtual ICollection<TbAttendance> TbAttendances { get; set; } = new List<TbAttendance>();

    public virtual ICollection<TbCoScholasticArea> TbCoScholasticAreas { get; set; } = new List<TbCoScholasticArea>();

    public virtual ICollection<TbDeclaredExam> TbDeclaredExams { get; set; } = new List<TbDeclaredExam>();

    public virtual ICollection<TbDivision> TbDivisions { get; set; } = new List<TbDivision>();

    public virtual ICollection<TbExam> TbExams { get; set; } = new List<TbExam>();

    public virtual ICollection<TbFeeClass> TbFeeClasses { get; set; } = new List<TbFeeClass>();

    public virtual ICollection<TbHealth> TbHealths { get; set; } = new List<TbHealth>();

    public virtual ICollection<TbPayment> TbPayments { get; set; } = new List<TbPayment>();

    public virtual ICollection<TbRegionsClass> TbRegionsClasses { get; set; } = new List<TbRegionsClass>();

    public virtual ICollection<TbStudentPremotion> TbStudentPremotions { get; set; } = new List<TbStudentPremotion>();

    public virtual ICollection<TbStudentTotalScore> TbStudentTotalScores { get; set; } = new List<TbStudentTotalScore>();

    public virtual ICollection<TbStudent> TbStudents { get; set; } = new List<TbStudent>();

    public virtual ICollection<TbTeacherClassSubject> TbTeacherClassSubjects { get; set; } = new List<TbTeacherClassSubject>();

    public virtual ICollection<TbTeacherClass> TbTeacherClasses { get; set; } = new List<TbTeacherClass>();

    public virtual ICollection<TbTimeTable> TbTimeTables { get; set; } = new List<TbTimeTable>();
}
