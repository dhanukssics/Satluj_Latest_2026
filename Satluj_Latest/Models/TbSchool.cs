using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSchool
{
    public long SchoolId { get; set; }

    public string SchoolName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? City { get; set; }

    public string? Website { get; set; }

    public string Contact { get; set; } = null!;

    public DateTime TimeStamp { get; set; }

    public Guid SchoolGuidId { get; set; }

    public bool IsActive { get; set; }

    public string? State { get; set; }

    public string? FilePath { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? BillingFooterMessage { get; set; }

    public int? LibraryDueDays { get; set; }

    public bool? PaymentOption { get; set; }

    public bool SmsActive { get; set; }

    public string? SpecialFilePath { get; set; }

    public virtual ICollection<TbAcademicPeriod> TbAcademicPeriods { get; set; } = new List<TbAcademicPeriod>();

    public virtual ICollection<TbAccountHead> TbAccountHeads { get; set; } = new List<TbAccountHead>();

    public virtual ICollection<TbArtCraftSkill> TbArtCraftSkills { get; set; } = new List<TbArtCraftSkill>();

    public virtual ICollection<TbAspectsMathsSkill> TbAspectsMathsSkills { get; set; } = new List<TbAspectsMathsSkill>();

    public virtual ICollection<TbAspectsNaturalSciencesSkill> TbAspectsNaturalSciencesSkills { get; set; } = new List<TbAspectsNaturalSciencesSkill>();

    public virtual ICollection<TbAspectsRelativeAreaSkill> TbAspectsRelativeAreaSkills { get; set; } = new List<TbAspectsRelativeAreaSkill>();

    public virtual ICollection<TbAspectsSocialStudiesSkill> TbAspectsSocialStudiesSkills { get; set; } = new List<TbAspectsSocialStudiesSkill>();

    public virtual ICollection<TbAssetsLiabilityDatum> TbAssetsLiabilityData { get; set; } = new List<TbAssetsLiabilityDatum>();

    public virtual ICollection<TbAssetsLiabilityId> TbAssetsLiabilityIds { get; set; } = new List<TbAssetsLiabilityId>();

    public virtual ICollection<TbAttendancePeriod> TbAttendancePeriods { get; set; } = new List<TbAttendancePeriod>();

    public virtual ICollection<TbBalance> TbBalances { get; set; } = new List<TbBalance>();

    public virtual ICollection<TbBankBookDatum> TbBankBookData { get; set; } = new List<TbBankBookDatum>();

    public virtual ICollection<TbBankBookId> TbBankBookIds { get; set; } = new List<TbBankBookId>();

    public virtual ICollection<TbBankEntry> TbBankEntries { get; set; } = new List<TbBankEntry>();

    public virtual ICollection<TbBank> TbBanks { get; set; } = new List<TbBank>();

    public virtual ICollection<TbBillCancelAccount> TbBillCancelAccounts { get; set; } = new List<TbBillCancelAccount>();

    public virtual ICollection<TbBiometricDivision> TbBiometricDivisions { get; set; } = new List<TbBiometricDivision>();

    public virtual ICollection<TbBookCategory> TbBookCategories { get; set; } = new List<TbBookCategory>();

    public virtual ICollection<TbBu> TbBus { get; set; } = new List<TbBu>();

    public virtual ICollection<TbCalenderEvent> TbCalenderEvents { get; set; } = new List<TbCalenderEvent>();

    public virtual ICollection<TbCashEntry> TbCashEntries { get; set; } = new List<TbCashEntry>();

    public virtual ICollection<TbCcavenueCourseResponse> TbCcavenueCourseResponses { get; set; } = new List<TbCcavenueCourseResponse>();

    public virtual ICollection<TbCertificateName> TbCertificateNames { get; set; } = new List<TbCertificateName>();

    public virtual ICollection<TbCircular> TbCirculars { get; set; } = new List<TbCircular>();

    public virtual ICollection<TbClass> TbClasses { get; set; } = new List<TbClass>();

    public virtual ICollection<TbCoScholasticArea> TbCoScholasticAreas { get; set; } = new List<TbCoScholasticArea>();

    public virtual ICollection<TbCoScholasticResultmain> TbCoScholasticResultmains { get; set; } = new List<TbCoScholasticResultmain>();

    public virtual ICollection<TbDayBookDatum> TbDayBookData { get; set; } = new List<TbDayBookDatum>();

    public virtual ICollection<TbDayBookIdBank> TbDayBookIdBanks { get; set; } = new List<TbDayBookIdBank>();

    public virtual ICollection<TbDayBookId> TbDayBookIds { get; set; } = new List<TbDayBookId>();

    public virtual ICollection<TbDeclaredExam> TbDeclaredExams { get; set; } = new List<TbDeclaredExam>();

    public virtual ICollection<TbDepartment> TbDepartments { get; set; } = new List<TbDepartment>();

    public virtual ICollection<TbDesignation> TbDesignations { get; set; } = new List<TbDesignation>();

    public virtual ICollection<TbDriver> TbDrivers { get; set; } = new List<TbDriver>();

    public virtual ICollection<TbExamBook> TbExamBooks { get; set; } = new List<TbExamBook>();

    public virtual ICollection<TbExamResult> TbExamResults { get; set; } = new List<TbExamResult>();

    public virtual ICollection<TbExam> TbExams { get; set; } = new List<TbExam>();

    public virtual ICollection<TbExpense> TbExpenses { get; set; } = new List<TbExpense>();

    public virtual ICollection<TbFeeAlertDatum> TbFeeAlertData { get; set; } = new List<TbFeeAlertDatum>();

    public virtual ICollection<TbFee> TbFees { get; set; } = new List<TbFee>();

    public virtual ICollection<TbFile> TbFiles { get; set; } = new List<TbFile>();

    public virtual ICollection<TbHomeworkSm> TbHomeworkSms { get; set; } = new List<TbHomeworkSm>();

    public virtual ICollection<TbIncome> TbIncomes { get; set; } = new List<TbIncome>();

    public virtual ICollection<TbLaboratoryCategory> TbLaboratoryCategories { get; set; } = new List<TbLaboratoryCategory>();

    public virtual ICollection<TbLibraryBookSerialNumber> TbLibraryBookSerialNumbers { get; set; } = new List<TbLibraryBookSerialNumber>();

    public virtual ICollection<TbLibraryFine> TbLibraryFines { get; set; } = new List<TbLibraryFine>();

    public virtual ICollection<TbListenningSkill> TbListenningSkills { get; set; } = new List<TbListenningSkill>();

    public virtual ICollection<TbLogin> TbLogins { get; set; } = new List<TbLogin>();

    public virtual ICollection<TbMaster> TbMasters { get; set; } = new List<TbMaster>();

    public virtual ICollection<TbMusicDanceSkill> TbMusicDanceSkills { get; set; } = new List<TbMusicDanceSkill>();

    public virtual ICollection<TbOptionalSubjectStudent> TbOptionalSubjectStudents { get; set; } = new List<TbOptionalSubjectStudent>();

    public virtual ICollection<TbPaymentBillNo> TbPaymentBillNos { get; set; } = new List<TbPaymentBillNo>();

    public virtual ICollection<TbPayment> TbPayments { get; set; } = new List<TbPayment>();

    public virtual ICollection<TbReadingSkill> TbReadingSkills { get; set; } = new List<TbReadingSkill>();

    public virtual ICollection<TbRegions> TbRegionss { get; set; } = new List<TbRegions>();

    public virtual ICollection<TbRemark> TbRemarks { get; set; } = new List<TbRemark>();

    public virtual ICollection<TbRoleDetail> TbRoleDetails { get; set; } = new List<TbRoleDetail>();

    public virtual ICollection<TbScholasticArea> TbScholasticAreas { get; set; } = new List<TbScholasticArea>();

    public virtual ICollection<TbScholasticResultMain> TbScholasticResultMains { get; set; } = new List<TbScholasticResultMain>();

    public virtual ICollection<TbSchoolSenderId> TbSchoolSenderIds { get; set; } = new List<TbSchoolSenderId>();

    public virtual ICollection<TbSetting> TbSettings { get; set; } = new List<TbSetting>();

    public virtual ICollection<TbSmsHead> TbSmsHeads { get; set; } = new List<TbSmsHead>();

    public virtual ICollection<TbSmsHistory> TbSmsHistories { get; set; } = new List<TbSmsHistory>();

    public virtual ICollection<TbSmsPackage> TbSmsPackages { get; set; } = new List<TbSmsPackage>();

    public virtual ICollection<TbSmtpdetail> TbSmtpdetails { get; set; } = new List<TbSmtpdetail>();

    public virtual ICollection<TbSpeakingSkill> TbSpeakingSkills { get; set; } = new List<TbSpeakingSkill>();

    public virtual ICollection<TbStaffSmshistory> TbStaffSmshistories { get; set; } = new List<TbStaffSmshistory>();

    public virtual ICollection<TbStockUpdate> TbStockUpdates { get; set; } = new List<TbStockUpdate>();

    public virtual ICollection<TbStudentPremotion> TbStudentPremotions { get; set; } = new List<TbStudentPremotion>();

    public virtual ICollection<TbStudentTotalScore> TbStudentTotalScores { get; set; } = new List<TbStudentTotalScore>();

    public virtual ICollection<TbStudent> TbStudents { get; set; } = new List<TbStudent>();

    public virtual ICollection<TbSubject> TbSubjects { get; set; } = new List<TbSubject>();

    public virtual ICollection<TbTeacherClassSubject> TbTeacherClassSubjects { get; set; } = new List<TbTeacherClassSubject>();

    public virtual ICollection<TbTeacher> TbTeachers { get; set; } = new List<TbTeacher>();

    public virtual ICollection<TbTimeTable> TbTimeTables { get; set; } = new List<TbTimeTable>();

    public virtual ICollection<TbTrip> TbTrips { get; set; } = new List<TbTrip>();

    public virtual ICollection<TbUserModuleMain> TbUserModuleMains { get; set; } = new List<TbUserModuleMain>();

    public virtual ICollection<TbVAssesment> TbVAssesments { get; set; } = new List<TbVAssesment>();

    public virtual ICollection<TbVGame> TbVGames { get; set; } = new List<TbVGame>();

    public virtual ICollection<TbVHealth> TbVHealths { get; set; } = new List<TbVHealth>();

    public virtual ICollection<TbVPersonalityDevelopment> TbVPersonalityDevelopments { get; set; } = new List<TbVPersonalityDevelopment>();

    public virtual ICollection<TbVRemark> TbVRemarks { get; set; } = new List<TbVRemark>();

    public virtual ICollection<TbVTotalScoreList> TbVTotalScoreLists { get; set; } = new List<TbVTotalScoreList>();

    public virtual ICollection<TbVoucherNumber> TbVoucherNumbers { get; set; } = new List<TbVoucherNumber>();

    public virtual ICollection<TbWritingSkill> TbWritingSkills { get; set; } = new List<TbWritingSkill>();
}
