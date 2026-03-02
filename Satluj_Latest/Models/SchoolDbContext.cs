using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Data;
using Satluj_Latest.DataLibrary;
using Satluj_Latest.MapModel;
using Satluj_Latest.Utility;

namespace Satluj_Latest.Models;

public partial class SchoolDbContext : DbContext
{
    public SchoolDbContext()
    {
    }

    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CloverStu3> CloverStu3s { get; set; }
    //Created by gayathri (19/11/2025)
    #region stored procedure registered
    public DbSet<SPFullFees> SPFullFees { get; set; }
    public DbSet<Sp_SmsTotalCount_Result> Sp_SmsTotalCount_Result { get; set; }

    public DbSet<SP_PreviousYear_VtoXII_Result> SP_PreviousYear_VtoXII_Results { get; set; }
    public DbSet<SP_PreviousYear_V_Result> SP_PreviousYear_V_Result { get; set; }
    public DbSet<SP_PreviousYear_XItoXII_Result> SP_PreviousYear_XItoXII_Result { get; set; }

    public DbSet<SP_PreviousYear_X_Result> SP_PreviousYear_X_Result { get; set; }

    //Stored procedures are registed into the dbcontext
    public DbSet<sp_ClassWithRegion_Result> sp_ClassWithRegion { get; set; }

    public DbSet<Sptravelhistroy> sptravelhistyoryresult { get; set; }
    public DbSet<sp_UnDeclaredExamSubjects_Result> sp_UnDeclaredExamSubjects { get; set; }

    public DbSet<SP_CircularNotification_Result> SP_CircularNotification { get; set; }

    public DbSet<CircularNotificationListMapModel> Circularresult { get; set; }
    public DbSet<sp_StudentMarkList_Result> sp_StudentMarkListResult { get; set; }
    public DbSet<sp_FeeAlertDetails> sp_FeeAlertDetailsResult { get; set; }

    public DbSet<SP_GetAllSmsOnTwoDates_Result> SP_GetAllSmsOnTwoDates { get; set; }

    public DbSet<SPdeleteoldAttendence> sPdeleteoldAttendences { get; set; }
    public DbSet<Sp_BalanceBDIncoExp> Sp_BalanceBDIncoExp { get; set; }
    public DbSet<Sp_BusTripHistoryHead_Result> Sp_BusTripHistoryHead_Result { get; set; }

    public DbSet<TbAttendance> spAttendances { get; set; }
    public DbSet<SPUnassignedTeachers> UnassignedTeachersResults { get; set; }
    public DbSet<SP_UnassignedTeachers_Result> SP_UnassignedTeachers_Results { get; set; }

    public DbSet<sp_ParentTeacherConversation_Result> sp_ParentTeacherConversation_Results { get; set; }
    public DbSet<sp_ParentTeacherConversationFull_Result> sp_ParentTeacherConversationFull_Results { get; set; }

    public DbSet<sp_TeacherConversation_Result> sp_TeacherConversation_Results { get; set; }

    public DbSet<SP_GetPaymentGatewayList_Result> PaymentGatewayListResult { get; set; }
    public DbSet<SP_GetMonthlyAttendance_Result> MonthlyAttendanceResult { get; set; }
    public DbSet<sp_ParentTeacherConversationFull> parentteacherResult { get; set; }
    public DbSet<SpParentTeacherConversationFullResult> ParentTeacherConversationFull { get; set; }
    //added by dhanu 22/11/2025
    public DbSet<SP_UnassignedDivisionResult> UnassignedDivisionResults { get; set; }
   
    public DbSet<LedgerFilterResult> LedgerFilterResults { get; set; }
    public DbSet<WithoutDeclaredExamTermResult> WithoutDeclaredExamTermResults { get; set; }
    public DbSet<SpSmsPackage> GetSpSmsPackages { get; set; }
    public DbSet<Sp_GetAssessmentsByIDandDatesDiv_Result> GetAssessmentsByIDandDatesDiv_Result { get; set; }

    public DbSet<Sp_GetAssessmentsByIDandDateMonth_Result> GetAssessmentsByIDandDateMonth_Result { get; set; }
    public DbSet<SPAdvanceFee>SPAdvanceFees { get; set; }
    public DbSet<SPEditFeeBilling> SPEditFeeBillings { get; set; }
    public DbSet<sp_CashBookDailyReport_Result> CashBookDailyReportResult { get; set; }
    public DbSet<SPStudentBillPayment> SPStudentBillPayments { get; set; }
    public DbSet<SPGetDailyReports> DailyReports { get; set; }
    public DbSet<SPCashBookReportData> SPCashBookReportData { get; set; }
    public DbSet<sp_CashBookReportSummary_Result> SPCashBookReportSummary { get; set; }
    public DbSet<SP_LEDGERDATAREPORT_Result> LedgerReportResult { get; set; }
    public DbSet<BillFeeDateHistory> BillFeeDateHistory { get; set; }
    public DbSet<SPDayBookStatus> SPDayBookStatuses { get; set; }
    public  DbSet<sp_StudentMarkPercentage_Result> spStudentMarkPercentage_Result { get; set; }

    public DbSet<sp_TrialBalance_Result>SpTrialBalanceResults { get; set; }
    public DbSet<SPBalanceSheet> SPBalanceSheetResults { get; set; }
    public DbSet<sp_BankStatement_Result> SPBankStatementResults { get; set; }
    public DbSet<SPBankYesterdayBalances> SPBankYesterdayBalances { get; set; }
    public DbSet<sp_ReceiptAndPaymentData_Result> SPReceiptAndPaymentData { get; set; }
    public DbSet<sp_DetailedCollectionReport_Result> SPDetailedCollectionReportResults { get; set; }
    public DbSet<SP_GetLibraryDueBook_Result> SPGetLibraryDueBookResult { get; set; }
    public DbSet<SP_GetDailyFeeCollection_Home_Result> SPGetDailyFeeCollectionHomeResult { get; set; }
    public DbSet<SP_GetLibraryBookCount_Home_Result>SPGetLibraryBookCountHomeResults { get; set; }
    public DbSet<sp_MonthlyAttendance_Result> SPMonthlyAttendanceResults { get; set; }
    public DbSet<SP_OutstandingReportNew_Result> SPOutstandingReportNew { get; set; }
    public DbSet<Sp_BilledReport_Result> SPBilledReport { get; set; }
    public DbSet<sp_OpeningBalanceAmountInCashBook_Result>SPOpeningBalanceAmountInCashBook { get; set; }
    public DbSet<sp_CashEntryReport_Result> SPCashEntryReport { get; set; }
    public DbSet<sp_ClosingBalanceAmount_Result> SPClosingBalanceAmountResult { get; set; }
    public DbSet<sp_BankEntryReport_Result> SPBankEntryReport { get; set; }
    public DbSet<sp_ReceiptPayment_Result> SPReceiptAndPaymentResult { get; set; }
    public DbSet<sp_DayBookReport_Result>spDayBookReportResults { get; set; }
    public DbSet<sp_ClassWithRegion_Result> SPClassWithRegion { get; set; }
    public DbSet<SP_PreviousYear_Progressreport_Result> SPPreviousYearProgressReport { get; set; }
    public DbSet<SPPushBusStart> PushBusStartResults { get; set; }
    public DbSet<Sp_GetAllSmsCount> GetAllSmsCounts { get; set; }


    #endregion
    public virtual DbSet<LupinParent3> LupinParent3s { get; set; }

    public virtual DbSet<LupinStu3> LupinStu3s { get; set; }

    public virtual DbSet<Parent2> Parent2s { get; set; }

    public virtual DbSet<ParentCandy> ParentCandies { get; set; }

    public virtual DbSet<ParentLilac> ParentLilacs { get; set; }

    public virtual DbSet<ParentNur> ParentNurs { get; set; }

    public virtual DbSet<ParentUkg> ParentUkgs { get; set; }

    public virtual DbSet<ParentXi> ParentXis { get; set; }
    public virtual DbSet<PetuniaParent3> PetuniaParent3s { get; set; }

    public virtual DbSet<PetuniaStu3> PetuniaStu3s { get; set; }

    public virtual DbSet<SalviaParent3> SalviaParent3s { get; set; }

    public virtual DbSet<SalviaStu3> SalviaStu3s { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Student2> Student2s { get; set; }

    public virtual DbSet<Student7> Student7s { get; set; }

    public virtual DbSet<StudentCandy> StudentCandies { get; set; }

    public virtual DbSet<StudentLilac> StudentLilacs { get; set; }

    public virtual DbSet<StudentNur> StudentNurs { get; set; }

    public virtual DbSet<StudentUkg> StudentUkgs { get; set; }

    public virtual DbSet<StudentXi> StudentXis { get; set; }

    public virtual DbSet<TbAcademicPeriod> TbAcademicPeriods { get; set; }

    public virtual DbSet<TbAcademicYear> TbAcademicYears { get; set; }

    public virtual DbSet<TbAccountHead> TbAccountHeads { get; set; }

    public virtual DbSet<TbAllMessage> TbAllMessages { get; set; }

    public virtual DbSet<TbArtCraftSkill> TbArtCraftSkills { get; set; }

    public virtual DbSet<TbAspectsMathsSkill> TbAspectsMathsSkills { get; set; }

    public virtual DbSet<TbAspectsNaturalSciencesSkill> TbAspectsNaturalSciencesSkills { get; set; }

    public virtual DbSet<TbAspectsRelativeAreaSkill> TbAspectsRelativeAreaSkills { get; set; }

    public virtual DbSet<TbAspectsSocialStudiesSkill> TbAspectsSocialStudiesSkills { get; set; }

    public virtual DbSet<TbAssesmentUpload> TbAssesmentUploads { get; set; }

    public virtual DbSet<TbAssetsLiabilityDatum> TbAssetsLiabilityData { get; set; }

    public virtual DbSet<TbAssetsLiabilityId> TbAssetsLiabilityIds { get; set; }

    public virtual DbSet<TbAttendance> TbAttendances { get; set; }

    public virtual DbSet<TbAttendancePeriod> TbAttendancePeriods { get; set; }

    public virtual DbSet<TbBalance> TbBalances { get; set; }

    public virtual DbSet<TbBank> TbBanks { get; set; }

    public virtual DbSet<TbBankBookDatum> TbBankBookData { get; set; }

    public virtual DbSet<TbBankBookId> TbBankBookIds { get; set; }

    public virtual DbSet<TbBankEntry> TbBankEntries { get; set; }

    public virtual DbSet<TbBankEntryTest> TbBankEntryTests { get; set; }

    public virtual DbSet<TbBillCancelAccount> TbBillCancelAccounts { get; set; }

    public virtual DbSet<TbBiometricDivision> TbBiometricDivisions { get; set; }

    public virtual DbSet<TbBookCategory> TbBookCategories { get; set; }

    public virtual DbSet<TbBu> TbBus { get; set; }

    public virtual DbSet<TbCalenderEvent> TbCalenderEvents { get; set; }

    public virtual DbSet<TbCashEntry> TbCashEntries { get; set; }

    public virtual DbSet<TbCashEntryTest> TbCashEntryTests { get; set; }

    public virtual DbSet<TbCcavenueCourseResponse> TbCcavenueCourseResponses { get; set; }

    public virtual DbSet<TbCertificateName> TbCertificateNames { get; set; }

    public virtual DbSet<TbCircular> TbCirculars { get; set; }

    public virtual DbSet<TbClass> TbClasses { get; set; }

    public virtual DbSet<TbClassList> TbClassLists { get; set; }

    public virtual DbSet<TbCoScholasticArea> TbCoScholasticAreas { get; set; }

    public virtual DbSet<TbCoScholasticResultmain> TbCoScholasticResultmains { get; set; }

    public virtual DbSet<TbDayBookDatum> TbDayBookData { get; set; }

    public virtual DbSet<TbDayBookId> TbDayBookIds { get; set; }

    public virtual DbSet<TbDayBookIdBank> TbDayBookIdBanks { get; set; }

    public virtual DbSet<TbDeclaredExam> TbDeclaredExams { get; set; }
    public virtual DbSet<sp_DeleteResultEnteredScholasticArearesult> deleteresultentered { get; set; }

    public virtual DbSet<sp_StudentsWithNoOptionalSubjects_Result> spstudentwithnoopt { get; set; }

    public virtual DbSet<TbDeclaredExamSubject> TbDeclaredExamSubjects { get; set; }

    public virtual DbSet<TbDeletedFeeStudent> TbDeletedFeeStudents { get; set; }

    public virtual DbSet<TbDepartment> TbDepartments { get; set; }

    public virtual DbSet<TbDesignation> TbDesignations { get; set; }

    public virtual DbSet<TbDeviceToken> TbDeviceTokens { get; set; }

    public virtual DbSet<TbDivision> TbDivisions { get; set; }

    public virtual DbSet<TbDriver> TbDrivers { get; set; }

    public virtual DbSet<TbExam> TbExams { get; set; }

    public virtual DbSet<TbExamBook> TbExamBooks { get; set; }

    public virtual DbSet<TbExamResult> TbExamResults { get; set; }

    public virtual DbSet<TbExamSubject> TbExamSubjects { get; set; }

    public virtual DbSet<TbExamTerm> TbExamTerms { get; set; }

    public virtual DbSet<TbExpense> TbExpenses { get; set; }

    public virtual DbSet<TbFee> TbFees { get; set; }

    public virtual DbSet<TbFeeAlertDatum> TbFeeAlertData { get; set; }

    public virtual DbSet<TbFeeClass> TbFeeClasses { get; set; }

    public virtual DbSet<TbFeeDiscount> TbFeeDiscounts { get; set; }

    public virtual DbSet<TbFeeDue> TbFeeDues { get; set; }

    public virtual DbSet<TbFeeStudent> TbFeeStudents { get; set; }

    public virtual DbSet<TbFile> TbFiles { get; set; }

    public virtual DbSet<TbHealth> TbHealths { get; set; }

    public virtual DbSet<TbHoliday> TbHolidays { get; set; }

    public virtual DbSet<TbHomeworkSm> TbHomeworkSms { get; set; }

    public virtual DbSet<TbIncome> TbIncomes { get; set; }

    public virtual DbSet<TbLaboratoryCategory> TbLaboratoryCategories { get; set; }

    public virtual DbSet<TbLibraryBook> TbLibraryBooks { get; set; }

    public virtual DbSet<TbLibraryBookSerialNumber> TbLibraryBookSerialNumbers { get; set; }

    public virtual DbSet<TbLibraryBookStudent> TbLibraryBookStudents { get; set; }

    public virtual DbSet<TbLibraryFine> TbLibraryFines { get; set; }

    public virtual DbSet<TbListenningSkill> TbListenningSkills { get; set; }

    public virtual DbSet<TbLkgUkgNurserySkill> TbLkgUkgNurserySkills { get; set; }

    public virtual DbSet<TbLogin> TbLogins { get; set; }

    public virtual DbSet<TbLoginAdmin> TbLoginAdmins { get; set; }

    public virtual DbSet<TbMaster> TbMasters { get; set; }

    public virtual DbSet<TbMenuList> TbMenuLists { get; set; }

    public virtual DbSet<TbMessage> TbMessages { get; set; }

    public virtual DbSet<TbModuleHome> TbModuleHomes { get; set; }

    public virtual DbSet<TbMusicDanceSkill> TbMusicDanceSkills { get; set; }

    public virtual DbSet<TbNotification> TbNotifications { get; set; }

    public virtual DbSet<TbOneFourSkill> TbOneFourSkills { get; set; }

    public virtual DbSet<TbOptionalSubjectStudent> TbOptionalSubjectStudents { get; set; }

    public virtual DbSet<TbOtpmessage> TbOtpmessages { get; set; }

    public virtual DbSet<TbParent> TbParents { get; set; }

    public virtual DbSet<TbParentMessage> TbParentMessages { get; set; }

    public virtual DbSet<TbPayment> TbPayments { get; set; }

    public virtual DbSet<TbPaymentBillNo> TbPaymentBillNos { get; set; }

    public virtual DbSet<TbPaymentTest> TbPaymentTests { get; set; }

    public virtual DbSet<TbPushDatum> TbPushData { get; set; }

    public virtual DbSet<TbReadingSkill> TbReadingSkills { get; set; }

    public virtual DbSet<TbRegions> TbRegionss { get; set; }

    public virtual DbSet<TbRegionsClass> TbRegionsClasses { get; set; }

    public virtual DbSet<TbRemark> TbRemarks { get; set; }

    public virtual DbSet<TbResetPassword> TbResetPasswords { get; set; }

    public virtual DbSet<TbRoleDetail> TbRoleDetails { get; set; }

    public virtual DbSet<TbScholasticArea> TbScholasticAreas { get; set; }

    public virtual DbSet<TbScholasticResultMain> TbScholasticResultMains { get; set; }

    public virtual DbSet<TbSchool> TbSchools { get; set; }

    public virtual DbSet<TbSchoolSenderId> TbSchoolSenderIds { get; set; }

    public virtual DbSet<TbScolasticAreaResultDetail> TbScolasticAreaResultDetails { get; set; }

    public virtual DbSet<TbSetting> TbSettings { get; set; }

    public virtual DbSet<TbSmsHead> TbSmsHeads { get; set; }

    public virtual DbSet<TbSmsHistory> TbSmsHistories { get; set; }

    public virtual DbSet<TbSmsPackage> TbSmsPackages { get; set; }

    public virtual DbSet<TbSmtpdetail> TbSmtpdetails { get; set; }

    public virtual DbSet<TbSpeakingSkill> TbSpeakingSkills { get; set; }

    public virtual DbSet<TbStaff> TbStaffs { get; set; }

    public virtual DbSet<TbStaffSmshistory> TbStaffSmshistories { get; set; }

    public virtual DbSet<TbState> TbStates { get; set; }

    public virtual DbSet<TbStockUpdate> TbStockUpdates { get; set; }

    public virtual DbSet<TbStudent> TbStudents { get; set; }

    public virtual DbSet<TbStudentAssesmentUpload> TbStudentAssesmentUploads { get; set; }

    public virtual DbSet<TbStudentBalance> TbStudentBalances { get; set; }

    public virtual DbSet<TbStudentMark> TbStudentMarks { get; set; }

    public virtual DbSet<TbStudentPaidAmount> TbStudentPaidAmounts { get; set; }

    public virtual DbSet<TbStudentPremotion> TbStudentPremotions { get; set; }

    public virtual DbSet<TbStudentRemark> TbStudentRemarks { get; set; }

    public virtual DbSet<TbStudentTotalScore> TbStudentTotalScores { get; set; }

    public virtual DbSet<TbSubLedgerDatum> TbSubLedgerData { get; set; }

    public virtual DbSet<TbSubModule> TbSubModules { get; set; }

    public virtual DbSet<TbSubject> TbSubjects { get; set; }

    public virtual DbSet<TbTeacher> TbTeachers { get; set; }

    public virtual DbSet<TbTeacherClass> TbTeacherClasses { get; set; }

    public virtual DbSet<TbTeacherClassSubject> TbTeacherClassSubjects { get; set; }

    public virtual DbSet<TbTimeTable> TbTimeTables { get; set; }

    public virtual DbSet<TbTravel> TbTravels { get; set; }

    public virtual DbSet<TbTrip> TbTrips { get; set; }

    public virtual DbSet<TbUserAllotedMenu> TbUserAllotedMenus { get; set; }

    public virtual DbSet<TbUserModuleDetail> TbUserModuleDetails { get; set; }

    public virtual DbSet<TbUserModuleMain> TbUserModuleMains { get; set; }

    public virtual DbSet<TbUserRole> TbUserRoles { get; set; }

    public virtual DbSet<TbVAssesment> TbVAssesments { get; set; }

    public virtual DbSet<TbVGame> TbVGames { get; set; }

    public virtual DbSet<TbVHealth> TbVHealths { get; set; }

    public virtual DbSet<TbVPersonalityDevelopment> TbVPersonalityDevelopments { get; set; }

    public virtual DbSet<TbVRemark> TbVRemarks { get; set; }

    public virtual DbSet<TbVTotalScoreList> TbVTotalScoreLists { get; set; }

    public virtual DbSet<TbVoucherNumber> TbVoucherNumbers { get; set; }

    public virtual DbSet<TbWritingSkill> TbWritingSkills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=50.63.166.171;Database=db_SchoolMan_satlujtest2025;User Id=sa;Password=WKO#$@@12345JK;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CloverStu3>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("'CLOVER-STU_3'");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });
        modelBuilder.Entity<SP_UnassignedDivisionResult>().HasNoKey(); //added by dhanu
        modelBuilder.Entity<SPAdvanceFee>().HasNoKey();
        modelBuilder.Entity<SPEditFeeBilling>().HasNoKey();
        modelBuilder.Entity<SPStudentBillPayment>().HasNoKey();
        modelBuilder.Entity<SPGetDailyReports>().HasNoKey();
        modelBuilder.Entity<SPCashBookReportData>().HasNoKey();
        modelBuilder.Entity<LedgerFilterResult>().HasNoKey();
        modelBuilder.Entity<WithoutDeclaredExamTermResult>().HasNoKey();
        modelBuilder.Entity<sp_CashBookReportSummary_Result>().HasNoKey();
        modelBuilder.Entity<sp_CashBookDailyReport_Result>().HasNoKey();
        modelBuilder.Entity<SP_LEDGERDATAREPORT_Result>().HasNoKey();
        modelBuilder.Entity<SPDayBookStatus>().HasNoKey();
        modelBuilder.Entity<sp_TrialBalance_Result>().HasNoKey();
        modelBuilder.Entity<SPBalanceSheet>().HasNoKey();
        modelBuilder.Entity<sp_BankStatement_Result>().HasNoKey();
        modelBuilder.Entity<SPBankYesterdayBalances>().HasNoKey();
        modelBuilder.Entity<sp_ReceiptAndPaymentData_Result>().HasNoKey();
        modelBuilder.Entity<sp_DetailedCollectionReport_Result>().HasNoKey();
        modelBuilder.Entity<SP_GetLibraryDueBook_Result>().HasNoKey();
        modelBuilder.Entity<SP_GetDailyFeeCollection_Home_Result>().HasNoKey();
        modelBuilder.Entity<SP_OutstandingReportNew_Result>().HasNoKey();
        modelBuilder.Entity<Sp_BilledReport_Result>().HasNoKey();
        modelBuilder.Entity<sp_OpeningBalanceAmountInCashBook_Result>().HasNoKey();
        modelBuilder.Entity<sp_CashEntryReport_Result>().HasNoKey();
        modelBuilder.Entity<sp_ClosingBalanceAmount_Result>().HasNoKey();
        modelBuilder.Entity<sp_BankEntryReport_Result>().HasNoKey();
        modelBuilder.Entity<sp_DayBookReport_Result>().HasNoKey();
        modelBuilder.Entity<sp_ReceiptPayment_Result>().HasNoKey();
        modelBuilder.Entity<sp_ClassWithRegion_Result>().HasNoKey();
        modelBuilder.Entity<SP_PreviousYear_Progressreport_Result>().HasNoKey();
        modelBuilder.Entity<CircularNotificationListMapModel>().HasNoKey();
        modelBuilder.Entity<SPPushBusStart>().HasNoKey();
        modelBuilder.Entity<SPFullFees>().HasNoKey();
        modelBuilder.Entity<SPUnassignedTeachers>().HasNoKey();
        modelBuilder.Entity<SP_UnassignedTeachers_Result>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
        });

        modelBuilder.Entity<sp_ParentTeacherConversation_Result>().HasNoKey();
        modelBuilder.Entity<sp_ParentTeacherConversationFull_Result>().HasNoKey();
        modelBuilder.Entity<sp_TeacherConversation_Result>().HasNoKey();
        modelBuilder.Entity<SP_GetPaymentGatewayList_Result>().HasNoKey();
        modelBuilder.Entity<SP_GetMonthlyAttendance_Result>().HasNoKey();
        modelBuilder.Entity<sp_ParentTeacherConversationFull>().HasNoKey();
        modelBuilder.Entity<SpParentTeacherConversationFullResult>().HasNoKey();
        modelBuilder.Entity<SP_CircularNotification_Result>().HasNoKey();
        modelBuilder.Entity<SP_GetLibraryBookCount_Home_Result>().HasNoKey();
#region Added By Gayathri(1/27/2026)
        modelBuilder.Entity<Sp_BalanceBDIncoExp>().HasNoKey();
        modelBuilder.Entity<Sp_BusTripHistoryHead_Result>().HasNoKey();
        modelBuilder.Entity<Sp_GetAssessmentsByIDandDatesDiv_Result>().HasNoKey();
        modelBuilder.Entity<Sp_GetAssessmentsByIDandDateMonth_Result>().HasNoKey();
        modelBuilder.Entity<Sp_SmsTotalCount_Result>().HasNoKey();
        modelBuilder.Entity<sp_DeleteResultEnteredScholasticArearesult>().HasNoKey();
        modelBuilder.Entity<sp_FeeAlertDetails>().HasNoKey();
        modelBuilder.Entity<Sptravelhistroy>().HasNoKey();
        modelBuilder.Entity<sp_MonthlyAttendance_Result>().HasNoKey();
        modelBuilder.Entity<sp_StudentMarkList_Result>().HasNoKey();
        modelBuilder.Entity<sp_StudentMarkPercentage_Result>().HasNoKey();
        modelBuilder.Entity<sp_StudentMarkPercentage_Result>().HasNoKey();
        modelBuilder.Entity<sp_StudentsWithNoOptionalSubjects_Result>().HasNoKey();
        modelBuilder.Entity<sp_UnDeclaredExamSubjects_Result>().HasNoKey();
        modelBuilder.Entity<BillFeeDateHistory>().HasNoKey();


        #endregion

        modelBuilder.Entity<SP_PreviousYear_V_Result>().HasNoKey();
        modelBuilder.Entity<SP_PreviousYear_VtoXII_Result>().HasNoKey();
        modelBuilder.Entity<SP_PreviousYear_XItoXII_Result>().HasNoKey();
        modelBuilder.Entity<SP_PreviousYear_X_Result>().HasNoKey();
        modelBuilder.Entity<SPdeleteoldAttendence>().HasNoKey();
        modelBuilder.Entity<SpSmsPackage>().HasNoKey();
        
        modelBuilder.Entity<LupinParent3>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("'LUPIN-PARENT_3'");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.FatherCountry)
                .HasMaxLength(255)
                .HasColumnName("Father Country");
            entity.Property(e => e.FatherEducation)
                .HasMaxLength(255)
                .HasColumnName("Father Education ");
            entity.Property(e => e.FatherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Father Occupation");
            entity.Property(e => e.MotherAddress)
                .HasMaxLength(255)
                .HasColumnName("Mother Address");
            entity.Property(e => e.MotherCity)
                .HasMaxLength(255)
                .HasColumnName("Mother City");
            entity.Property(e => e.MotherContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Mother Contact Number");
            entity.Property(e => e.MotherCountry)
                .HasMaxLength(255)
                .HasColumnName("Mother Country");
            entity.Property(e => e.MotherEducation)
                .HasMaxLength(255)
                .HasColumnName("Mother Education");
            entity.Property(e => e.MotherEmail)
                .HasMaxLength(255)
                .HasColumnName("Mother Email");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Mother Occupation");
            entity.Property(e => e.MotherPincode).HasColumnName("Mother Pincode");
            entity.Property(e => e.MotherState)
                .HasMaxLength(255)
                .HasColumnName("Mother State");
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<LupinStu3>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("'LUPIN-STU_3'");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<Parent2>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Parent_2");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.F23).HasMaxLength(255);
            entity.Property(e => e.F24).HasMaxLength(255);
            entity.Property(e => e.F25).HasMaxLength(255);
            entity.Property(e => e.F26).HasMaxLength(255);
            entity.Property(e => e.FatherCountry)
                .HasMaxLength(255)
                .HasColumnName("Father Country");
            entity.Property(e => e.FatherEducation)
                .HasMaxLength(255)
                .HasColumnName("Father Education ");
            entity.Property(e => e.FatherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Father Occupation");
            entity.Property(e => e.MotherAddress)
                .HasMaxLength(255)
                .HasColumnName("Mother Address");
            entity.Property(e => e.MotherCity)
                .HasMaxLength(255)
                .HasColumnName("Mother City");
            entity.Property(e => e.MotherContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Mother Contact Number");
            entity.Property(e => e.MotherCountry)
                .HasMaxLength(255)
                .HasColumnName("Mother Country");
            entity.Property(e => e.MotherEducation)
                .HasMaxLength(255)
                .HasColumnName("Mother Education");
            entity.Property(e => e.MotherEmail)
                .HasMaxLength(255)
                .HasColumnName("Mother Email");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Mother Occupation");
            entity.Property(e => e.MotherPincode).HasColumnName("Mother Pincode");
            entity.Property(e => e.MotherState)
                .HasMaxLength(255)
                .HasColumnName("Mother State");
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<ParentCandy>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Parent_Candy");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.F23).HasMaxLength(255);
            entity.Property(e => e.F24).HasMaxLength(255);
            entity.Property(e => e.F25).HasMaxLength(255);
            entity.Property(e => e.F26).HasMaxLength(255);
            entity.Property(e => e.FatherCountry)
                .HasMaxLength(255)
                .HasColumnName("Father Country");
            entity.Property(e => e.FatherEducation)
                .HasMaxLength(255)
                .HasColumnName("Father Education ");
            entity.Property(e => e.FatherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Father Occupation");
            entity.Property(e => e.MotherAddress)
                .HasMaxLength(255)
                .HasColumnName("Mother Address");
            entity.Property(e => e.MotherCity)
                .HasMaxLength(255)
                .HasColumnName("Mother City");
            entity.Property(e => e.MotherContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Mother Contact Number");
            entity.Property(e => e.MotherCountry)
                .HasMaxLength(255)
                .HasColumnName("Mother Country");
            entity.Property(e => e.MotherEducation)
                .HasMaxLength(255)
                .HasColumnName("Mother Education");
            entity.Property(e => e.MotherEmail)
                .HasMaxLength(255)
                .HasColumnName("Mother Email");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Mother Occupation");
            entity.Property(e => e.MotherPincode).HasColumnName("Mother Pincode");
            entity.Property(e => e.MotherState)
                .HasMaxLength(255)
                .HasColumnName("Mother State");
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<ParentLilac>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Parent_lilac");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.FatherCountry)
                .HasMaxLength(255)
                .HasColumnName("Father Country");
            entity.Property(e => e.FatherEducation)
                .HasMaxLength(255)
                .HasColumnName("Father Education ");
            entity.Property(e => e.FatherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Father Occupation");
            entity.Property(e => e.MotherAddress)
                .HasMaxLength(255)
                .HasColumnName("Mother Address");
            entity.Property(e => e.MotherCity)
                .HasMaxLength(255)
                .HasColumnName("Mother City");
            entity.Property(e => e.MotherContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Mother Contact Number");
            entity.Property(e => e.MotherCountry)
                .HasMaxLength(255)
                .HasColumnName("Mother Country");
            entity.Property(e => e.MotherEducation)
                .HasMaxLength(255)
                .HasColumnName("Mother Education");
            entity.Property(e => e.MotherEmail)
                .HasMaxLength(255)
                .HasColumnName("Mother Email");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Mother Occupation");
            entity.Property(e => e.MotherPincode).HasColumnName("Mother Pincode");
            entity.Property(e => e.MotherState)
                .HasMaxLength(255)
                .HasColumnName("Mother State");
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<ParentNur>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Parent_nur");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.F23).HasMaxLength(255);
            entity.Property(e => e.FatherCountry)
                .HasMaxLength(255)
                .HasColumnName("Father Country");
            entity.Property(e => e.FatherEducation)
                .HasMaxLength(255)
                .HasColumnName("Father Education ");
            entity.Property(e => e.FatherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Father Occupation");
            entity.Property(e => e.MotherAddress)
                .HasMaxLength(255)
                .HasColumnName("Mother Address");
            entity.Property(e => e.MotherCity)
                .HasMaxLength(255)
                .HasColumnName("Mother City");
            entity.Property(e => e.MotherContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Mother Contact Number");
            entity.Property(e => e.MotherCountry)
                .HasMaxLength(255)
                .HasColumnName("Mother Country");
            entity.Property(e => e.MotherEducation)
                .HasMaxLength(255)
                .HasColumnName("Mother Education");
            entity.Property(e => e.MotherEmail)
                .HasMaxLength(255)
                .HasColumnName("Mother Email");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Mother Occupation");
            entity.Property(e => e.MotherPincode).HasColumnName("Mother Pincode");
            entity.Property(e => e.MotherState)
                .HasMaxLength(255)
                .HasColumnName("Mother State");
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<ParentUkg>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Parent_UKG");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.F23).HasMaxLength(255);
            entity.Property(e => e.F24).HasMaxLength(255);
            entity.Property(e => e.F25).HasMaxLength(255);
            entity.Property(e => e.F26).HasMaxLength(255);
            entity.Property(e => e.FatherCountry)
                .HasMaxLength(255)
                .HasColumnName("Father Country");
            entity.Property(e => e.FatherEducation)
                .HasMaxLength(255)
                .HasColumnName("Father Education ");
            entity.Property(e => e.FatherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Father Occupation");
            entity.Property(e => e.MotherAddress)
                .HasMaxLength(255)
                .HasColumnName("Mother Address");
            entity.Property(e => e.MotherCity)
                .HasMaxLength(255)
                .HasColumnName("Mother City");
            entity.Property(e => e.MotherContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Mother Contact Number");
            entity.Property(e => e.MotherCountry)
                .HasMaxLength(255)
                .HasColumnName("Mother Country");
            entity.Property(e => e.MotherEducation)
                .HasMaxLength(255)
                .HasColumnName("Mother Education");
            entity.Property(e => e.MotherEmail)
                .HasMaxLength(255)
                .HasColumnName("Mother Email");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Mother Occupation");
            entity.Property(e => e.MotherPincode).HasColumnName("Mother Pincode");
            entity.Property(e => e.MotherState)
                .HasMaxLength(255)
                .HasColumnName("Mother State");
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SNo).HasColumnName("S#No#");
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<ParentXi>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Parent_XI");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.F100).HasMaxLength(255);
            entity.Property(e => e.F101).HasMaxLength(255);
            entity.Property(e => e.F102).HasMaxLength(255);
            entity.Property(e => e.F103).HasMaxLength(255);
            entity.Property(e => e.F104).HasMaxLength(255);
            entity.Property(e => e.F105).HasMaxLength(255);
            entity.Property(e => e.F106).HasMaxLength(255);
            entity.Property(e => e.F107).HasMaxLength(255);
            entity.Property(e => e.F108).HasMaxLength(255);
            entity.Property(e => e.F109).HasMaxLength(255);
            entity.Property(e => e.F110).HasMaxLength(255);
            entity.Property(e => e.F111).HasMaxLength(255);
            entity.Property(e => e.F112).HasMaxLength(255);
            entity.Property(e => e.F113).HasMaxLength(255);
            entity.Property(e => e.F114).HasMaxLength(255);
            entity.Property(e => e.F115).HasMaxLength(255);
            entity.Property(e => e.F116).HasMaxLength(255);
            entity.Property(e => e.F117).HasMaxLength(255);
            entity.Property(e => e.F118).HasMaxLength(255);
            entity.Property(e => e.F119).HasMaxLength(255);
            entity.Property(e => e.F120).HasMaxLength(255);
            entity.Property(e => e.F121).HasMaxLength(255);
            entity.Property(e => e.F122).HasMaxLength(255);
            entity.Property(e => e.F123).HasMaxLength(255);
            entity.Property(e => e.F124).HasMaxLength(255);
            entity.Property(e => e.F125).HasMaxLength(255);
            entity.Property(e => e.F126).HasMaxLength(255);
            entity.Property(e => e.F127).HasMaxLength(255);
            entity.Property(e => e.F128).HasMaxLength(255);
            entity.Property(e => e.F129).HasMaxLength(255);
            entity.Property(e => e.F130).HasMaxLength(255);
            entity.Property(e => e.F131).HasMaxLength(255);
            entity.Property(e => e.F132).HasMaxLength(255);
            entity.Property(e => e.F133).HasMaxLength(255);
            entity.Property(e => e.F134).HasMaxLength(255);
            entity.Property(e => e.F135).HasMaxLength(255);
            entity.Property(e => e.F136).HasMaxLength(255);
            entity.Property(e => e.F137).HasMaxLength(255);
            entity.Property(e => e.F138).HasMaxLength(255);
            entity.Property(e => e.F139).HasMaxLength(255);
            entity.Property(e => e.F140).HasMaxLength(255);
            entity.Property(e => e.F141).HasMaxLength(255);
            entity.Property(e => e.F142).HasMaxLength(255);
            entity.Property(e => e.F143).HasMaxLength(255);
            entity.Property(e => e.F144).HasMaxLength(255);
            entity.Property(e => e.F145).HasMaxLength(255);
            entity.Property(e => e.F146).HasMaxLength(255);
            entity.Property(e => e.F147).HasMaxLength(255);
            entity.Property(e => e.F148).HasMaxLength(255);
            entity.Property(e => e.F149).HasMaxLength(255);
            entity.Property(e => e.F150).HasMaxLength(255);
            entity.Property(e => e.F151).HasMaxLength(255);
            entity.Property(e => e.F152).HasMaxLength(255);
            entity.Property(e => e.F153).HasMaxLength(255);
            entity.Property(e => e.F154).HasMaxLength(255);
            entity.Property(e => e.F155).HasMaxLength(255);
            entity.Property(e => e.F156).HasMaxLength(255);
            entity.Property(e => e.F157).HasMaxLength(255);
            entity.Property(e => e.F158).HasMaxLength(255);
            entity.Property(e => e.F159).HasMaxLength(255);
            entity.Property(e => e.F160).HasMaxLength(255);
            entity.Property(e => e.F161).HasMaxLength(255);
            entity.Property(e => e.F162).HasMaxLength(255);
            entity.Property(e => e.F163).HasMaxLength(255);
            entity.Property(e => e.F164).HasMaxLength(255);
            entity.Property(e => e.F165).HasMaxLength(255);
            entity.Property(e => e.F166).HasMaxLength(255);
            entity.Property(e => e.F167).HasMaxLength(255);
            entity.Property(e => e.F168).HasMaxLength(255);
            entity.Property(e => e.F169).HasMaxLength(255);
            entity.Property(e => e.F170).HasMaxLength(255);
            entity.Property(e => e.F171).HasMaxLength(255);
            entity.Property(e => e.F172).HasMaxLength(255);
            entity.Property(e => e.F173).HasMaxLength(255);
            entity.Property(e => e.F174).HasMaxLength(255);
            entity.Property(e => e.F175).HasMaxLength(255);
            entity.Property(e => e.F176).HasMaxLength(255);
            entity.Property(e => e.F177).HasMaxLength(255);
            entity.Property(e => e.F178).HasMaxLength(255);
            entity.Property(e => e.F179).HasMaxLength(255);
            entity.Property(e => e.F180).HasMaxLength(255);
            entity.Property(e => e.F181).HasMaxLength(255);
            entity.Property(e => e.F182).HasMaxLength(255);
            entity.Property(e => e.F183).HasMaxLength(255);
            entity.Property(e => e.F184).HasMaxLength(255);
            entity.Property(e => e.F185).HasMaxLength(255);
            entity.Property(e => e.F186).HasMaxLength(255);
            entity.Property(e => e.F187).HasMaxLength(255);
            entity.Property(e => e.F188).HasMaxLength(255);
            entity.Property(e => e.F189).HasMaxLength(255);
            entity.Property(e => e.F190).HasMaxLength(255);
            entity.Property(e => e.F191).HasMaxLength(255);
            entity.Property(e => e.F192).HasMaxLength(255);
            entity.Property(e => e.F193).HasMaxLength(255);
            entity.Property(e => e.F194).HasMaxLength(255);
            entity.Property(e => e.F195).HasMaxLength(255);
            entity.Property(e => e.F196).HasMaxLength(255);
            entity.Property(e => e.F197).HasMaxLength(255);
            entity.Property(e => e.F198).HasMaxLength(255);
            entity.Property(e => e.F199).HasMaxLength(255);
            entity.Property(e => e.F200).HasMaxLength(255);
            entity.Property(e => e.F201).HasMaxLength(255);
            entity.Property(e => e.F202).HasMaxLength(255);
            entity.Property(e => e.F203).HasMaxLength(255);
            entity.Property(e => e.F204).HasMaxLength(255);
            entity.Property(e => e.F205).HasMaxLength(255);
            entity.Property(e => e.F206).HasMaxLength(255);
            entity.Property(e => e.F207).HasMaxLength(255);
            entity.Property(e => e.F208).HasMaxLength(255);
            entity.Property(e => e.F209).HasMaxLength(255);
            entity.Property(e => e.F210).HasMaxLength(255);
            entity.Property(e => e.F211).HasMaxLength(255);
            entity.Property(e => e.F212).HasMaxLength(255);
            entity.Property(e => e.F213).HasMaxLength(255);
            entity.Property(e => e.F214).HasMaxLength(255);
            entity.Property(e => e.F215).HasMaxLength(255);
            entity.Property(e => e.F216).HasMaxLength(255);
            entity.Property(e => e.F217).HasMaxLength(255);
            entity.Property(e => e.F218).HasMaxLength(255);
            entity.Property(e => e.F219).HasMaxLength(255);
            entity.Property(e => e.F220).HasMaxLength(255);
            entity.Property(e => e.F221).HasMaxLength(255);
            entity.Property(e => e.F222).HasMaxLength(255);
            entity.Property(e => e.F223).HasMaxLength(255);
            entity.Property(e => e.F224).HasMaxLength(255);
            entity.Property(e => e.F225).HasMaxLength(255);
            entity.Property(e => e.F226).HasMaxLength(255);
            entity.Property(e => e.F227).HasMaxLength(255);
            entity.Property(e => e.F228).HasMaxLength(255);
            entity.Property(e => e.F229).HasMaxLength(255);
            entity.Property(e => e.F230).HasMaxLength(255);
            entity.Property(e => e.F231).HasMaxLength(255);
            entity.Property(e => e.F232).HasMaxLength(255);
            entity.Property(e => e.F233).HasMaxLength(255);
            entity.Property(e => e.F234).HasMaxLength(255);
            entity.Property(e => e.F235).HasMaxLength(255);
            entity.Property(e => e.F236).HasMaxLength(255);
            entity.Property(e => e.F237).HasMaxLength(255);
            entity.Property(e => e.F238).HasMaxLength(255);
            entity.Property(e => e.F239).HasMaxLength(255);
            entity.Property(e => e.F240).HasMaxLength(255);
            entity.Property(e => e.F241).HasMaxLength(255);
            entity.Property(e => e.F242).HasMaxLength(255);
            entity.Property(e => e.F243).HasMaxLength(255);
            entity.Property(e => e.F244).HasMaxLength(255);
            entity.Property(e => e.F245).HasMaxLength(255);
            entity.Property(e => e.F246).HasMaxLength(255);
            entity.Property(e => e.F247).HasMaxLength(255);
            entity.Property(e => e.F248).HasMaxLength(255);
            entity.Property(e => e.F249).HasMaxLength(255);
            entity.Property(e => e.F250).HasMaxLength(255);
            entity.Property(e => e.F251).HasMaxLength(255);
            entity.Property(e => e.F252).HasMaxLength(255);
            entity.Property(e => e.F253).HasMaxLength(255);
            entity.Property(e => e.F254).HasMaxLength(255);
            entity.Property(e => e.F255).HasMaxLength(255);
            entity.Property(e => e.F39).HasMaxLength(255);
            entity.Property(e => e.F40).HasMaxLength(255);
            entity.Property(e => e.F41).HasMaxLength(255);
            entity.Property(e => e.F42).HasMaxLength(255);
            entity.Property(e => e.F43).HasMaxLength(255);
            entity.Property(e => e.F44).HasMaxLength(255);
            entity.Property(e => e.F45).HasMaxLength(255);
            entity.Property(e => e.F46).HasMaxLength(255);
            entity.Property(e => e.F47).HasMaxLength(255);
            entity.Property(e => e.F48).HasMaxLength(255);
            entity.Property(e => e.F49).HasMaxLength(255);
            entity.Property(e => e.F50).HasMaxLength(255);
            entity.Property(e => e.F51).HasMaxLength(255);
            entity.Property(e => e.F52).HasMaxLength(255);
            entity.Property(e => e.F53).HasMaxLength(255);
            entity.Property(e => e.F54).HasMaxLength(255);
            entity.Property(e => e.F55).HasMaxLength(255);
            entity.Property(e => e.F56).HasMaxLength(255);
            entity.Property(e => e.F57).HasMaxLength(255);
            entity.Property(e => e.F58).HasMaxLength(255);
            entity.Property(e => e.F59).HasMaxLength(255);
            entity.Property(e => e.F60).HasMaxLength(255);
            entity.Property(e => e.F61).HasMaxLength(255);
            entity.Property(e => e.F62).HasMaxLength(255);
            entity.Property(e => e.F63).HasMaxLength(255);
            entity.Property(e => e.F64).HasMaxLength(255);
            entity.Property(e => e.F65).HasMaxLength(255);
            entity.Property(e => e.F66).HasMaxLength(255);
            entity.Property(e => e.F67).HasMaxLength(255);
            entity.Property(e => e.F68).HasMaxLength(255);
            entity.Property(e => e.F69).HasMaxLength(255);
            entity.Property(e => e.F70).HasMaxLength(255);
            entity.Property(e => e.F71).HasMaxLength(255);
            entity.Property(e => e.F72).HasMaxLength(255);
            entity.Property(e => e.F73).HasMaxLength(255);
            entity.Property(e => e.F74).HasMaxLength(255);
            entity.Property(e => e.F75).HasMaxLength(255);
            entity.Property(e => e.F76).HasMaxLength(255);
            entity.Property(e => e.F77).HasMaxLength(255);
            entity.Property(e => e.F78).HasMaxLength(255);
            entity.Property(e => e.F79).HasMaxLength(255);
            entity.Property(e => e.F80).HasMaxLength(255);
            entity.Property(e => e.F81).HasMaxLength(255);
            entity.Property(e => e.F82).HasMaxLength(255);
            entity.Property(e => e.F83).HasMaxLength(255);
            entity.Property(e => e.F84).HasMaxLength(255);
            entity.Property(e => e.F85).HasMaxLength(255);
            entity.Property(e => e.F86).HasMaxLength(255);
            entity.Property(e => e.F87).HasMaxLength(255);
            entity.Property(e => e.F88).HasMaxLength(255);
            entity.Property(e => e.F89).HasMaxLength(255);
            entity.Property(e => e.F90).HasMaxLength(255);
            entity.Property(e => e.F91).HasMaxLength(255);
            entity.Property(e => e.F92).HasMaxLength(255);
            entity.Property(e => e.F93).HasMaxLength(255);
            entity.Property(e => e.F94).HasMaxLength(255);
            entity.Property(e => e.F95).HasMaxLength(255);
            entity.Property(e => e.F96).HasMaxLength(255);
            entity.Property(e => e.F97).HasMaxLength(255);
            entity.Property(e => e.F98).HasMaxLength(255);
            entity.Property(e => e.F99).HasMaxLength(255);
            entity.Property(e => e.FatherCountry)
                .HasMaxLength(255)
                .HasColumnName("Father Country");
            entity.Property(e => e.FatherEducation)
                .HasMaxLength(255)
                .HasColumnName("Father Education ");
            entity.Property(e => e.FatherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Father Occupation");
            entity.Property(e => e.GuardianAddress)
                .HasMaxLength(255)
                .HasColumnName("Guardian Address");
            entity.Property(e => e.GuardianCity)
                .HasMaxLength(255)
                .HasColumnName("Guardian City");
            entity.Property(e => e.GuardianContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Guardian Contact Number");
            entity.Property(e => e.GuardianCountry)
                .HasMaxLength(255)
                .HasColumnName("Guardian Country");
            entity.Property(e => e.GuardianEducation)
                .HasMaxLength(255)
                .HasColumnName("Guardian Education");
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(255)
                .HasColumnName("Guardian Email Id");
            entity.Property(e => e.GuardianName)
                .HasMaxLength(255)
                .HasColumnName("Guardian Name");
            entity.Property(e => e.GuardianOccupation)
                .HasMaxLength(255)
                .HasColumnName("Guardian occupation");
            entity.Property(e => e.GuardianPincode)
                .HasMaxLength(255)
                .HasColumnName("Guardian Pincode");
            entity.Property(e => e.GuardianState)
                .HasMaxLength(255)
                .HasColumnName("Guardian State");
            entity.Property(e => e.Kid1)
                .HasMaxLength(255)
                .HasColumnName("Kid 1");
            entity.Property(e => e.Kid1ClassDivision)
                .HasMaxLength(255)
                .HasColumnName("Kid 1 Class - Division");
            entity.Property(e => e.Kid2)
                .HasMaxLength(255)
                .HasColumnName("Kid 2");
            entity.Property(e => e.Kid2ClassDivision)
                .HasMaxLength(255)
                .HasColumnName("Kid 2 Class- Division ");
            entity.Property(e => e.Kid3)
                .HasMaxLength(255)
                .HasColumnName("Kid 3");
            entity.Property(e => e.Kid3ClassDivision)
                .HasMaxLength(255)
                .HasColumnName("Kid 3 Class- Division ");
            entity.Property(e => e.MotherAddress)
                .HasMaxLength(255)
                .HasColumnName("Mother Address");
            entity.Property(e => e.MotherCity)
                .HasMaxLength(255)
                .HasColumnName("Mother City");
            entity.Property(e => e.MotherContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Mother Contact Number");
            entity.Property(e => e.MotherCountry)
                .HasMaxLength(255)
                .HasColumnName("Mother Country");
            entity.Property(e => e.MotherEducation)
                .HasMaxLength(255)
                .HasColumnName("Mother Education");
            entity.Property(e => e.MotherEmail)
                .HasMaxLength(255)
                .HasColumnName("Mother Email");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Mother Occupation");
            entity.Property(e => e.MotherPincode).HasColumnName("Mother Pincode");
            entity.Property(e => e.MotherState)
                .HasMaxLength(255)
                .HasColumnName("Mother State");
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<PetuniaParent3>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("'PETUNIA-PARENT_3'");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.FatherCountry)
                .HasMaxLength(255)
                .HasColumnName("Father Country");
            entity.Property(e => e.FatherEducation)
                .HasMaxLength(255)
                .HasColumnName("Father Education ");
            entity.Property(e => e.FatherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Father Occupation");
            entity.Property(e => e.MotherAddress)
                .HasMaxLength(255)
                .HasColumnName("Mother Address");
            entity.Property(e => e.MotherCity)
                .HasMaxLength(255)
                .HasColumnName("Mother City");
            entity.Property(e => e.MotherContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Mother Contact Number");
            entity.Property(e => e.MotherCountry)
                .HasMaxLength(255)
                .HasColumnName("Mother Country");
            entity.Property(e => e.MotherEducation)
                .HasMaxLength(255)
                .HasColumnName("Mother Education");
            entity.Property(e => e.MotherEmail)
                .HasMaxLength(255)
                .HasColumnName("Mother Email");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Mother Occupation");
            entity.Property(e => e.MotherPincode).HasColumnName("Mother Pincode");
            entity.Property(e => e.MotherState)
                .HasMaxLength(255)
                .HasColumnName("Mother State");
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<PetuniaStu3>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("'PETUNIA-STU_3'");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(255)
                .HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<SalviaParent3>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("'SALVIA-PARENT_3'");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.FatherCountry)
                .HasMaxLength(255)
                .HasColumnName("Father Country");
            entity.Property(e => e.FatherEducation)
                .HasMaxLength(255)
                .HasColumnName("Father Education ");
            entity.Property(e => e.FatherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Father Occupation");
            entity.Property(e => e.MotherAddress)
                .HasMaxLength(255)
                .HasColumnName("Mother Address");
            entity.Property(e => e.MotherCity)
                .HasMaxLength(255)
                .HasColumnName("Mother City");
            entity.Property(e => e.MotherContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Mother Contact Number");
            entity.Property(e => e.MotherCountry)
                .HasMaxLength(255)
                .HasColumnName("Mother Country");
            entity.Property(e => e.MotherEducation)
                .HasMaxLength(255)
                .HasColumnName("Mother Education");
            entity.Property(e => e.MotherEmail)
                .HasMaxLength(255)
                .HasColumnName("Mother Email");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherOccupation)
                .HasMaxLength(255)
                .HasColumnName("Mother Occupation");
            entity.Property(e => e.MotherPincode).HasColumnName("Mother Pincode");
            entity.Property(e => e.MotherState)
                .HasMaxLength(255)
                .HasColumnName("Mother State");
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<SalviaStu3>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("'SALVIA-STU_3'");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo).HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Student");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.Contact).HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.Dob)
                .HasMaxLength(255)
                .HasColumnName("DOB");
            entity.Property(e => e.F26).HasMaxLength(255);
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<Student2>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Student_2");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.F26).HasMaxLength(255);
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<Student7>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Student_7");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.ClassNumber1).HasColumnName("Class number1");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.Division1)
                .HasMaxLength(255)
                .HasColumnName("Division 1");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<StudentCandy>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Student_Candy");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.ClassNumber)
                .HasMaxLength(255)
                .HasColumnName("Class number");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division)
                .HasMaxLength(255)
                .HasColumnName("Division ");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.F26).HasMaxLength(255);
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<StudentLilac>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Student_lilac");

            entity.Property(e => e.Aadhar).HasMaxLength(255);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<StudentNur>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Student_nur");

            entity.Property(e => e.Aadhar).HasMaxLength(255);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city ");
            entity.Property(e => e.Class)
                .HasMaxLength(255)
                .HasColumnName("Class ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.DivisionName)
                .HasMaxLength(255)
                .HasColumnName("Division name ");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<StudentUkg>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Student_UKG");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("City ");
            entity.Property(e => e.ClassNumber).HasColumnName("Class number");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("Contact ");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Division).HasColumnName("Division ");
            entity.Property(e => e.DivisionName)
                .HasMaxLength(255)
                .HasColumnName("Division name");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.F27).HasMaxLength(255);
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("Gender ");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SNo).HasColumnName("S#No#");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId).HasColumnName("Student Special ID");
        });

        modelBuilder.Entity<StudentXi>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Student_XI");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(255)
                .HasColumnName("Blood Group");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("Contact Number");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.DateOfJoining)
                .HasColumnType("datetime")
                .HasColumnName("Date Of Joining");
            entity.Property(e => e.Divisionname).HasMaxLength(255);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.F27).HasMaxLength(255);
            entity.Property(e => e.F28).HasMaxLength(255);
            entity.Property(e => e.F29).HasMaxLength(255);
            entity.Property(e => e.F30).HasMaxLength(255);
            entity.Property(e => e.F31).HasMaxLength(255);
            entity.Property(e => e.F32).HasMaxLength(255);
            entity.Property(e => e.F33).HasMaxLength(255);
            entity.Property(e => e.F34).HasMaxLength(255);
            entity.Property(e => e.F35).HasMaxLength(255);
            entity.Property(e => e.F36).HasMaxLength(255);
            entity.Property(e => e.F37).HasMaxLength(255);
            entity.Property(e => e.F38).HasMaxLength(255);
            entity.Property(e => e.F39).HasMaxLength(255);
            entity.Property(e => e.F40).HasMaxLength(255);
            entity.Property(e => e.F41).HasMaxLength(255);
            entity.Property(e => e.F42).HasMaxLength(255);
            entity.Property(e => e.F43).HasMaxLength(255);
            entity.Property(e => e.F44).HasMaxLength(255);
            entity.Property(e => e.F45).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(255);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("Mobile No");
            entity.Property(e => e.MotherName)
                .HasMaxLength(255)
                .HasColumnName("Mother Name");
            entity.Property(e => e.MotherTongue)
                .HasMaxLength(255)
                .HasColumnName("Mother Tongue");
            entity.Property(e => e.Nationality).HasMaxLength(255);
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(255)
                .HasColumnName("Parent Email");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("Parent Name");
            entity.Property(e => e.PlaceOfBirth)
                .HasMaxLength(255)
                .HasColumnName("Place Of Birth");
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.SlNo).HasColumnName("Sl No");
            entity.Property(e => e.State).HasMaxLength(255);
            entity.Property(e => e.StudentName)
                .HasMaxLength(255)
                .HasColumnName("Student Name");
            entity.Property(e => e.StudentSpecialId)
                .HasMaxLength(255)
                .HasColumnName("Student Special Id");
        });

        modelBuilder.Entity<TbAcademicPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Acade__3214EC07ECD2422F");

            entity.ToTable("tb_AcademicPeriods");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbAcademicPeriods)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Academ__Class__61BB7BD9");

            entity.HasOne(d => d.Region).WithMany(p => p.TbAcademicPeriods)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Academ__Regio__63A3C44B");

            entity.HasOne(d => d.School).WithMany(p => p.TbAcademicPeriods)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Academ__Schoo__62AFA012");
        });

        modelBuilder.Entity<TbAcademicYear>(entity =>
        {
            entity.HasKey(e => e.YearId).HasName("PK__tb_Acade__C33A18CD56E368E0");

            entity.ToTable("tb_AcademicYear");
        });

        modelBuilder.Entity<TbAccountHead>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__tb_Accou__349DA5A6D595FAB6");

            entity.ToTable("TbAccountHead");

            entity.Property(e => e.ForBill).HasDefaultValue(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbAccountHeads)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Accoun__Schoo__37703C52");
        });

        modelBuilder.Entity<TbAllMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.ToTable("tb_AllMessage");

            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TbAllMessages)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_AllMessage_TbTeachers");
        });

        modelBuilder.Entity<TbArtCraftSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_ArtCr__3214EC07033F4D2B");

            entity.ToTable("tb_ArtCraftSkills");

            entity.Property(e => e.Creativity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CREATIVITY");
            entity.Property(e => e.Interest)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("INTEREST");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbArtCraftSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ArtCra__Asses__43F60EC8");

            entity.HasOne(d => d.Division).WithMany(p => p.TbArtCraftSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ArtCra__Divis__39788055");

            entity.HasOne(d => d.School).WithMany(p => p.TbArtCraftSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ArtCra__Schoo__379037E3");

            entity.HasOne(d => d.Student).WithMany(p => p.TbArtCraftSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ArtCra__Stude__38845C1C");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbArtCraftSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ArtCra__Subje__3A6CA48E");

            entity.HasOne(d => d.User).WithMany(p => p.TbArtCraftSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ArtCra__UserI__3B60C8C7");
        });

        modelBuilder.Entity<TbAspectsMathsSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Aspec__3214EC072CBBA799");

            entity.ToTable("tb_AspectsMathsSkills");

            entity.Property(e => e.Activity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACTIVITY");
            entity.Property(e => e.Concepts)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CONCEPTS");
            entity.Property(e => e.MentalAbility)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MENTAL_ABILITY");
            entity.Property(e => e.Tables)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TABLES");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.WorkSheet)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WORK_SHEET");
            entity.Property(e => e.WrittenWork)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WRITTEN_WORK");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbAspectsMathsSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Asses__33BFA6FF");

            entity.HasOne(d => d.Division).WithMany(p => p.TbAspectsMathsSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Divis__2665ABE1");

            entity.HasOne(d => d.School).WithMany(p => p.TbAspectsMathsSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Schoo__247D636F");

            entity.HasOne(d => d.Student).WithMany(p => p.TbAspectsMathsSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Stude__257187A8");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbAspectsMathsSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Subje__2759D01A");

            entity.HasOne(d => d.User).WithMany(p => p.TbAspectsMathsSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__UserI__284DF453");
        });

        modelBuilder.Entity<TbAspectsNaturalSciencesSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Aspec__3214EC071D1926D6");

            entity.ToTable("tb_AspectsNaturalSciencesSkills");

            entity.Property(e => e.ActivityProject)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACTIVITY_PROJECT");
            entity.Property(e => e.GroupDiscussion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GROUP_DISCUSSION");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.WorkSheet)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WORK_SHEET");
            entity.Property(e => e.WrittenWork)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WRITTEN_WORK");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbAspectsNaturalSciencesSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Asses__34B3CB38");

            entity.HasOne(d => d.Division).WithMany(p => p.TbAspectsNaturalSciencesSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Divis__2D12A970");

            entity.HasOne(d => d.School).WithMany(p => p.TbAspectsNaturalSciencesSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Schoo__2B2A60FE");

            entity.HasOne(d => d.Student).WithMany(p => p.TbAspectsNaturalSciencesSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Stude__2C1E8537");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbAspectsNaturalSciencesSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Subje__2E06CDA9");

            entity.HasOne(d => d.User).WithMany(p => p.TbAspectsNaturalSciencesSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__UserI__2EFAF1E2");
        });

        modelBuilder.Entity<TbAspectsRelativeAreaSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Aspec__3214EC078F1E45BF");

            entity.ToTable("tb_AspectsRelativeAreaSkills");

            entity.Property(e => e.ComputerScience)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COMPUTER_SCIENCE");
            entity.Property(e => e.French)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FRENCH");
            entity.Property(e => e.GeneralStudies)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GENERAL_STUDIES");
            entity.Property(e => e.SmartClass)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SMART_CLASS");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.ValueEducation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("VALUE_EDUCATION");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbAspectsRelativeAreaSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Asses__7869D707");

            entity.HasOne(d => d.Division).WithMany(p => p.TbAspectsRelativeAreaSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Divis__795DFB40");

            entity.HasOne(d => d.School).WithMany(p => p.TbAspectsRelativeAreaSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Schoo__7A521F79");

            entity.HasOne(d => d.Student).WithMany(p => p.TbAspectsRelativeAreaSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Stude__7B4643B2");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbAspectsRelativeAreaSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Subje__7C3A67EB");

            entity.HasOne(d => d.User).WithMany(p => p.TbAspectsRelativeAreaSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__UserI__7D2E8C24");
        });

        modelBuilder.Entity<TbAspectsSocialStudiesSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Aspec__3214EC0792D1C9FF");

            entity.ToTable("tb_AspectsSocialStudiesSkills");

            entity.Property(e => e.ActivityProject)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACTIVITY_PROJECT");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.Viva)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("VIVA");
            entity.Property(e => e.WorkSheet)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WORK_SHEET");
            entity.Property(e => e.WrittenWork)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WRITTEN_WORK");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbAspectsSocialStudiesSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Asses__69279377");

            entity.HasOne(d => d.Division).WithMany(p => p.TbAspectsSocialStudiesSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Divis__6A1BB7B0");

            entity.HasOne(d => d.School).WithMany(p => p.TbAspectsSocialStudiesSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Schoo__6B0FDBE9");

            entity.HasOne(d => d.Student).WithMany(p => p.TbAspectsSocialStudiesSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Stude__6C040022");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbAspectsSocialStudiesSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__Subje__6CF8245B");

            entity.HasOne(d => d.User).WithMany(p => p.TbAspectsSocialStudiesSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Aspect__UserI__6DEC4894");
        });

        modelBuilder.Entity<TbAssesmentUpload>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK_tb_AssesmentUpload_1");

            entity.ToTable("tb_AssesmentUpload");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.FileUploadedDate)
                .HasColumnType("datetime")
                .HasColumnName("File_Uploaded_Date");
            entity.Property(e => e.Filename)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbAssetsLiabilityDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Asset__3214EC0746603EA1");

            entity.ToTable("tb_AssetsLiabilityData");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Head).WithMany(p => p.TbAssetsLiabilityData)
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Assets__HeadI__395884C4");

            entity.HasOne(d => d.School).WithMany(p => p.TbAssetsLiabilityData)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Assets__Schoo__3A4CA8FD");

            entity.HasOne(d => d.User).WithMany(p => p.TbAssetsLiabilityData)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Assets__UserI__3B40CD36");
        });

        modelBuilder.Entity<TbAssetsLiabilityId>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Asset__3214EC07BB8D8EAC");

            entity.ToTable("tb_AssetsLiabilityId");

            entity.HasOne(d => d.School).WithMany(p => p.TbAssetsLiabilityIds)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Assets__Schoo__3C34F16F");
        });

        modelBuilder.Entity<TbAttendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__tb_Atten__8B69261C622E3927");

            entity.ToTable("tb_Attendance");

            entity.Property(e => e.AttendanceDate).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbAttendances)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Attend__Class__3D2915A8");

            entity.HasOne(d => d.Division).WithMany(p => p.TbAttendances)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Attend__Divis__3E1D39E1");

            entity.HasOne(d => d.Staff).WithMany(p => p.TbAttendances)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Attend__Staff__3F115E1A");

            entity.HasOne(d => d.Student).WithMany(p => p.TbAttendances)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Attend__Stude__40058253");
        });

        modelBuilder.Entity<TbAttendancePeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Atten__3214EC07074A76CD");

            entity.ToTable("tb_AttendancePeriod");

            entity.Property(e => e.TimeStampp).HasColumnType("datetime");

            entity.HasOne(d => d.Division).WithMany(p => p.TbAttendancePeriods)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Attend__Divis__2077C861");

            entity.HasOne(d => d.Period).WithMany(p => p.TbAttendancePeriods)
                .HasForeignKey(d => d.PeriodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Attend__Perio__216BEC9A");

            entity.HasOne(d => d.School).WithMany(p => p.TbAttendancePeriods)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Attend__Schoo__1E8F7FEF");

            entity.HasOne(d => d.Student).WithMany(p => p.TbAttendancePeriods)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Attend__Stude__1F83A428");

            entity.HasOne(d => d.User).WithMany(p => p.TbAttendancePeriods)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Attend__UserI__226010D3");
        });

        modelBuilder.Entity<TbBalance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Balan__3214EC0750069C04");

            entity.ToTable("tb_Balance");

            entity.Property(e => e.Closing).HasColumnType("money");
            entity.Property(e => e.CurrentDate).HasColumnType("datetime");
            entity.Property(e => e.Opening).HasColumnType("money");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbBalances)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Balanc__Schoo__40F9A68C");
        });

        modelBuilder.Entity<TbBank>(entity =>
        {
            entity.HasKey(e => e.BankId).HasName("PK__tb_Banks__AA08CB13DFF48306");

            entity.ToTable("tb_Banks");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbBanks)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Banks__School__4B7734FF");
        });

        modelBuilder.Entity<TbBankBookDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_BankB__3214EC071D063143");

            entity.ToTable("tb_BankBookData");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ChequeDate).HasColumnType("datetime");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Bank).WithMany(p => p.TbBankBookData)
                .HasForeignKey(d => d.BankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankBo__BankI__41EDCAC5");

            entity.HasOne(d => d.Head).WithMany(p => p.TbBankBookData)
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankBo__HeadI__42E1EEFE");

            entity.HasOne(d => d.School).WithMany(p => p.TbBankBookData)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankBo__Schoo__43D61337");

            entity.HasOne(d => d.Subledger).WithMany(p => p.TbBankBookData)
                .HasForeignKey(d => d.SubledgerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankBo__Suble__44CA3770");

            entity.HasOne(d => d.User).WithMany(p => p.TbBankBookData)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankBo__UserI__45BE5BA9");
        });

        modelBuilder.Entity<TbBankBookId>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_BankB__3214EC07436EA7F3");

            entity.ToTable("TbBankBookIds");

            entity.HasOne(d => d.School).WithMany(p => p.TbBankBookIds)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankBo__Schoo__46B27FE2");
        });

        modelBuilder.Entity<TbBankEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_BankE__3214EC07081C59CB");

            entity.ToTable("tb_BankEntry");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.ChequeDate).HasColumnType("datetime");
            entity.Property(e => e.EnterDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Bank).WithMany(p => p.TbBankEntries)
                .HasForeignKey(d => d.BankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankEn__BankI__47A6A41B");

            entity.HasOne(d => d.Head).WithMany(p => p.TbBankEntries)
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankEn__HeadI__489AC854");

            entity.HasOne(d => d.School).WithMany(p => p.TbBankEntries)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankEn__Schoo__498EEC8D");

            entity.HasOne(d => d.User).WithMany(p => p.TbBankEntries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BankEn__UserI__4A8310C6");
        });

        modelBuilder.Entity<TbBankEntryTest>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tb_BankEntryTest");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BankId).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CancelStatus).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ChequeDate).HasColumnType("datetime");
            entity.Property(e => e.ChequeNumber).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DataFromStatus).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EnterDate).HasColumnType("datetime");
            entity.Property(e => e.HeadId).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Id).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Migration).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ModeType).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SchoolId).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubId).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.VoucherNumber).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<TbBillCancelAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_BillC__3214EC07A6A88A90");

            entity.ToTable("tb_BillCancelAccounts");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CancelDate).HasColumnType("datetime");

            entity.HasOne(d => d.Item).WithMany(p => p.TbBillCancelAccounts)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BillCa__ItemI__4C6B5938");

            entity.HasOne(d => d.School).WithMany(p => p.TbBillCancelAccounts)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BillCa__Schoo__4D5F7D71");
        });

        modelBuilder.Entity<TbBiometricDivision>(entity =>
        {
            entity.HasKey(e => e.BioDivId).HasName("PK__tb_Biome__96471CF2AF77402F");

            entity.ToTable("tb_BiometricDivision");

            entity.HasOne(d => d.Division).WithMany(p => p.TbBiometricDivisions)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Biomet__Divis__4E53A1AA");

            entity.HasOne(d => d.School).WithMany(p => p.TbBiometricDivisions)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Biomet__Schoo__4F47C5E3");
        });

        modelBuilder.Entity<TbBookCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__tb_BookC__19093A0B65FE66D5");

            entity.ToTable("tb_BookCategory");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbBookCategories)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_BookCa__Schoo__503BEA1C");
        });

        modelBuilder.Entity<TbBu>(entity =>
        {
            entity.HasKey(e => e.BusId).HasName("PK__TbBus__6A0F60B54A54E2F5");

            entity.ToTable("TbBus");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbBus)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TbBus__SchoolId__4E88ABD4");
        });

        modelBuilder.Entity<TbCalenderEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__tb_Calen__7944C810BFA93AAC");

            entity.ToTable("tb_CalenderEvent");

            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbCalenderEvents)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Calend__Schoo__5224328E");
        });

        modelBuilder.Entity<TbCashEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_CashE__3214EC0704098C01");

            entity.ToTable("tb_CashEntry");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.EnterDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Head).WithMany(p => p.TbCashEntries)
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_CashEn__HeadI__531856C7");

            entity.HasOne(d => d.School).WithMany(p => p.TbCashEntries)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_CashEn__Schoo__56E8E7AB");

            entity.HasOne(d => d.User).WithMany(p => p.TbCashEntries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_CashEn__UserI__5AB9788F");
        });

        modelBuilder.Entity<TbCashEntryTest>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tb_CashEntryTest");

            entity.Property(e => e.AdvanceStatus).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillNo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CancelStatus).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DataFromStatus).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EnterDate).HasColumnType("datetime");
            entity.Property(e => e.HeadId).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Id).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Migration).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ReverseStatus).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SchoolId).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubId).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.VoucherNumber).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<TbCcavenueCourseResponse>(entity =>
        {
            entity.HasKey(e => e.ResponseId).HasName("PK__tb_Ccave__1AAA646CF1645D49");

            entity.ToTable("tb_CcavenueCourseResponse");

            entity.Property(e => e.Amount).HasColumnType("money");

            entity.HasOne(d => d.Parent).WithMany(p => p.TbCcavenueCourseResponses)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__tb_Ccaven__Paren__5E8A0973");

            entity.HasOne(d => d.School).WithMany(p => p.TbCcavenueCourseResponses)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK_tb_CcavenueCourseResponse_tb_School");
        });

        modelBuilder.Entity<TbCertificateName>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Certi__3214EC07AB90932C");

            entity.ToTable("TbCertificateNames");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbCertificateNames)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Certif__Schoo__02925FBF");
        });

        modelBuilder.Entity<TbCircular>(entity =>
        {
            entity.HasKey(e => e.CircularId).HasName("PK__tb_Circu__C019C86EAE64C995");

            entity.ToTable("tb_Circular");

            entity.Property(e => e.CircularDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("USerId");

            entity.HasOne(d => d.School).WithMany(p => p.TbCirculars)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Circul__Schoo__607251E5");

            entity.HasOne(d => d.User).WithMany(p => p.TbCirculars)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Circul__USerI__6166761E");
        });

        modelBuilder.Entity<TbClass>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__tb_Class__CB1927C053877ED6");

            entity.ToTable("tb_Class");

            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.TbClasses)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Class__Academ__625A9A57");

            entity.HasOne(d => d.School).WithMany(p => p.TbClasses)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Class__School__2A4B4B5E");
        });

        modelBuilder.Entity<TbClassList>(entity =>
        {
            entity.HasKey(e => e.ClassNameId).HasName("PK__tb_Class__71AFB78402900EF8");

            entity.ToTable("tb_ClassList");
        });

        modelBuilder.Entity<TbCoScholasticArea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Co_Sc__3214EC07356C644C");

            entity.ToTable("TbCoScholasticAreas");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbCoScholasticAreas)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__tb_Co_Sch__Class__09946309");

            entity.HasOne(d => d.Region).WithMany(p => p.TbCoScholasticAreas)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Co_Sch__Regio__0A338187");

            entity.HasOne(d => d.School).WithMany(p => p.TbCoScholasticAreas)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Co_Sch__Schoo__093F5D4E");
        });

        modelBuilder.Entity<TbCoScholasticResultmain>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Co_Sc__3214EC07278B87DB");

            entity.ToTable("tb_Co_ScholasticResultmain");

            entity.Property(e => e.CoScholasticId).HasColumnName("Co_ScholasticId");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.CoScholastic).WithMany(p => p.TbCoScholasticResultmains)
                .HasForeignKey(d => d.CoScholasticId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Co_Sch__Co_Sc__27C3E46E");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbCoScholasticResultmains)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Co_Sch__ExamI__26CFC035");

            entity.HasOne(d => d.School).WithMany(p => p.TbCoScholasticResultmains)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Co_Sch__Schoo__24E777C3");

            entity.HasOne(d => d.Student).WithMany(p => p.TbCoScholasticResultmains)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Co_Sch__Stude__25DB9BFC");
        });

        modelBuilder.Entity<TbDayBookDatum>(entity =>
        {
            entity.HasKey(e => e.DayBookId).HasName("PK__tb_DayBo__91C4FAA74E793BD8");

            entity.ToTable("tb_DayBookData");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Head).WithMany(p => p.TbDayBookData)
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_DayBoo__HeadI__6442E2C9");

            entity.HasOne(d => d.School).WithMany(p => p.TbDayBookData)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_DayBoo__Schoo__65370702");

            entity.HasOne(d => d.SubLedger).WithMany(p => p.TbDayBookData)
                .HasForeignKey(d => d.SubLedgerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_DayBoo__SubLe__662B2B3B");

            entity.HasOne(d => d.User).WithMany(p => p.TbDayBookData)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_DayBoo__UserI__671F4F74");
        });

        modelBuilder.Entity<TbDayBookId>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_DayBo__3214EC07C8EE2E42");

            entity.ToTable("TbDayBookData");

            entity.Property(e => e.ExpenseId).HasDefaultValue(1L);
            entity.Property(e => e.IncomeId).HasDefaultValue(1L);

            entity.HasOne(d => d.School).WithMany(p => p.TbDayBookIds)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_DayBoo__Schoo__681373AD");
        });

        modelBuilder.Entity<TbDayBookIdBank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_DayBo__3214EC07030FF1EB");

            entity.ToTable("TbDayBookDataBank");

            entity.HasOne(d => d.School).WithMany(p => p.TbDayBookIdBanks)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_DayBoo__Schoo__690797E6");
        });

        modelBuilder.Entity<TbDeclaredExam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Decla__3214EC07E504B20E");

            entity.ToTable("TbDeclaredExams");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbDeclaredExams)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Declar__Class__0EF836A4");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbDeclaredExams)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Declar__ExamI__0E04126B");

            entity.HasOne(d => d.School).WithMany(p => p.TbDeclaredExams)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Declar__Schoo__0D0FEE32");

            entity.HasOne(d => d.TbExamTerm).WithMany(p => p.TbDeclaredExams)
                .HasForeignKey(d => d.TermId)
                .HasConstraintName("FK__tb_Declar__TermI__46486B8E");
        });

        modelBuilder.Entity<TbDeclaredExamSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Decla__3214EC07B117C3AE");

            entity.ToTable("TbDeclaredExamsubjects");

            entity.Property(e => e.ExamDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.TotalScore).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.DeclaredExam).WithMany(p => p.TbDeclaredExamSubjects)
                .HasForeignKey(d => d.DeclaredExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Declar__Decla__11D4A34F");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbDeclaredExamSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Declar__Subje__12C8C788");
        });

        modelBuilder.Entity<TbDeletedFeeStudent>(entity =>
        {
            entity.HasKey(e => e.DletedFeeId).HasName("PK__tb_Delet__267CB18FA9573D34");

            entity.ToTable("tb_DeletedFeeStudent");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.FeeClass).WithMany(p => p.TbDeletedFeeStudents)
                .HasForeignKey(d => d.FeeClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Delete__FeeCl__69FBBC1F");

            entity.HasOne(d => d.Student).WithMany(p => p.TbDeletedFeeStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Delete__Stude__6AEFE058");
        });

        modelBuilder.Entity<TbDepartment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Depar__3214EC0769064FB4");

            entity.ToTable("tb_Department");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbDepartments)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Depart__Schoo__60083D91");
        });

        modelBuilder.Entity<TbDesignation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Desig__3214EC0756E13F9C");

            entity.ToTable("tb_Designation");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbDesignations)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Design__Schoo__65C116E7");
        });

        modelBuilder.Entity<TbDeviceToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__tb_Devic__658FEEEABC6C1D6C");

            entity.ToTable("tb_DeviceToken");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbDivision>(entity =>
        {
            entity.HasKey(e => e.DivisionId).HasName("PK__tb_Divis__20EFC6A80678BEF9");

            entity.ToTable("tb_Division");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbDivisions)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Divisi__Class__2B3F6F97");
        });

        modelBuilder.Entity<TbDriver>(entity =>
        {
            entity.HasKey(e => e.DriverId).HasName("PK__tb_Drive__F1B1CD04D1B2B440");

            entity.ToTable("tb_Driver");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbDrivers)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Driver__Schoo__6CD828CA");
        });

        modelBuilder.Entity<TbExam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__tb_Exams__297521C7125C59F8");

            entity.ToTable("tb_Exams");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbExams)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Exams__ClassI__6DCC4D03");

            entity.HasOne(d => d.School).WithMany(p => p.TbExams)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Exams__School__6FB49575");

            entity.HasOne(d => d.User).WithMany(p => p.TbExams)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Exams__UserId__70A8B9AE");
        });

        modelBuilder.Entity<TbExamBook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_ExamB__3214EC072226BD25");

            entity.ToTable("TbExamBooks");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbExamBooks)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ExamBo__Schoo__7BE56230");

            entity.HasOne(d => d.Term).WithMany(p => p.TbExamBooks)
                .HasForeignKey(d => d.TermId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ExamBo__TermI__7AF13DF7");
        });

        modelBuilder.Entity<TbExamResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_ExamR__3214EC07BE005E3F");

            entity.ToTable("tb_ExamResult");

            entity.Property(e => e.PracticalScore).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.StudentScore).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbExamResults)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ExamRe__ExamI__3429BB53");

            entity.HasOne(d => d.School).WithMany(p => p.TbExamResults)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ExamRe__Schoo__324172E1");

            entity.HasOne(d => d.Student).WithMany(p => p.TbExamResults)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ExamRe__Stude__3335971A");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbExamResults)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ExamRe__Subje__351DDF8C");
        });

        modelBuilder.Entity<TbExamSubject>(entity =>
        {
            entity.HasKey(e => e.SubId).HasName("PK__tb_ExamS__4D9BB84ADAFB2C8A");

            entity.ToTable("tb_ExamSubjects");

            entity.Property(e => e.ExamDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExternalMark).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.InternalMarks).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Mark).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubjectId).HasDefaultValue(1L);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbExamSubjects)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ExamSu__ExamI__719CDDE7");

            entity.HasOne(d => d.SubjectNavigation).WithMany(p => p.TbExamSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ExamSu__Subje__72910220");
        });

        modelBuilder.Entity<TbExamTerm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_ExamT__3214EC07E792F941");

            entity.ToTable("tb_ExamTerms");
        });

        modelBuilder.Entity<TbExpense>(entity =>
        {
            entity.ToTable("tb_Expense");

            entity.Property(e => e.AccountHead).IsUnicode(false);
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Particular).IsUnicode(false);

            entity.HasOne(d => d.School).WithMany(p => p.TbExpenses)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_Expense_tb_School");
        });

        modelBuilder.Entity<TbFee>(entity =>
        {
            entity.HasKey(e => e.FeeId).HasName("PK__tb_Fee__B387B229816B936E");

            entity.ToTable("tb_Fee");

            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.FeeStartDate).HasColumnType("datetime");
            entity.Property(e => e.FineAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbFees)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Fee__SchoolId__74794A92");
        });

        modelBuilder.Entity<TbFeeAlertDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_FeeAl__3214EC07B693CEB9");

            entity.ToTable("tb_FeeAlertData");

            entity.Property(e => e.AlertDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbFeeAlertData)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_FeeAle__Schoo__756D6ECB");
        });

        modelBuilder.Entity<TbFeeClass>(entity =>
        {
            entity.HasKey(e => e.FeeClassId).HasName("PK__tb_FeeCl__7BDF497F7C4150D2");

            entity.ToTable("tb_FeeClass");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbFeeClasses)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_FeeCla__Class__76619304");

            entity.HasOne(d => d.Fee).WithMany(p => p.TbFeeClasses)
                .HasForeignKey(d => d.FeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_FeeCla__FeeId__7755B73D");
        });

        modelBuilder.Entity<TbFeeDiscount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__tb_FeeDi__E43F6D967E4C5864");

            entity.ToTable("TbFeeDiscounts");

            entity.Property(e => e.DiscountAmount).HasColumnType("money");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Fee).WithMany(p => p.TbFeeDiscounts)
                .HasForeignKey(d => d.FeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_FeeDis__FeeId__7849DB76");

            entity.HasOne(d => d.Student).WithMany(p => p.TbFeeDiscounts)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_FeeDis__Stude__793DFFAF");
        });

        modelBuilder.Entity<TbFeeDue>(entity =>
        {
            entity.HasKey(e => e.FeeDuesId).HasName("PK__tb_FeeDu__AA032D34848DE3A9");

            entity.ToTable("tb_FeeDues");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Fee).WithMany(p => p.TbFeeDues)
                .HasForeignKey(d => d.FeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_FeeDue__FeeId__7A3223E8");

            entity.HasOne(d => d.Student).WithMany(p => p.TbFeeDues)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_FeeDue__Stude__7B264821");
        });

        modelBuilder.Entity<TbFeeStudent>(entity =>
        {
            entity.HasKey(e => e.FeeStudentId).HasName("PK__tb_FeeSt__FC46064E615CE626");

            entity.ToTable("TbFeeStudents");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Fee).WithMany(p => p.TbFeeStudents)
                .HasForeignKey(d => d.FeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_FeeStu__FeeId__7C1A6C5A");

            entity.HasOne(d => d.Student).WithMany(p => p.TbFeeStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_FeeStu__Stude__7D0E9093");
        });

        modelBuilder.Entity<TbFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__tb_File__6F0F98BF285A9F2A");

            entity.ToTable("tb_File");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbFiles)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK__tb_File__SchoolI__7E02B4CC");
        });

        modelBuilder.Entity<TbHealth>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Healt__3214EC07A163538C");

            entity.ToTable("tb_Health");

            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Class).WithMany(p => p.TbHealths)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Health__Class__695C9DA1");

            entity.HasOne(d => d.Division).WithMany(p => p.TbHealths)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Health__Divis__6A50C1DA");

            entity.HasOne(d => d.Periods).WithMany(p => p.TbHealths)
                .HasForeignKey(d => d.PeriodsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Health__Perio__6B44E613");

            entity.HasOne(d => d.Student).WithMany(p => p.TbHealths)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Health__Stude__6C390A4C");
        });

        modelBuilder.Entity<TbHoliday>(entity =>
        {
            entity.HasKey(e => e.HolidayId).HasName("PK__tb_Holid__2D35D57A2D98666D");

            entity.ToTable("tb_Holidays");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HolidayName).HasMaxLength(100);
        });

        modelBuilder.Entity<TbHomeworkSm>(entity =>
        {
            entity.HasKey(e => e.HomeworkSmsId).HasName("PK__tb_Homew__E3C4B463BA1255AF");

            entity.ToTable("tb_HomeworkSms");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Head).WithMany(p => p.TbHomeworkSms)
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Homewo__HeadI__7EF6D905");

            entity.HasOne(d => d.School).WithMany(p => p.TbHomeworkSms)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Homewo__Schoo__7FEAFD3E");
        });

        modelBuilder.Entity<TbIncome>(entity =>
        {
            entity.ToTable("tb_Income");

            entity.Property(e => e.AccountHead).IsUnicode(false);
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Particular).IsUnicode(false);

            entity.HasOne(d => d.School).WithMany(p => p.TbIncomes)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_Income_tb_School");
        });

        modelBuilder.Entity<TbLaboratoryCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__tb_Labor__19093A0B94CB1489");

            entity.ToTable("tb_LaboratoryCategory");

            entity.HasOne(d => d.School).WithMany(p => p.TbLaboratoryCategories)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Labora__Schoo__01D345B0");
        });

        modelBuilder.Entity<TbLibraryBook>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__tb_Libra__3DE0C2072C0EFF2D");

            entity.ToTable("tb_LibraryBook");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.TbLibraryBooks)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Librar__Categ__02C769E9");
        });

        modelBuilder.Entity<TbLibraryBookSerialNumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Libra__3214EC0701D6CF4F");

            entity.ToTable("tb_LibraryBookSerialNumber");

            entity.HasOne(d => d.School).WithMany(p => p.TbLibraryBookSerialNumbers)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Librar__Schoo__03BB8E22");
        });

        modelBuilder.Entity<TbLibraryBookStudent>(entity =>
        {
            entity.HasKey(e => e.StudentBookId).HasName("PK__tb_Libra__F916A6E4433E2778");

            entity.ToTable("tb_LibraryBookStudent");

            entity.Property(e => e.AcceptDateTime).HasColumnType("datetime");
            entity.Property(e => e.IssueDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.TbLibraryBookStudents)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Librar__BookI__04AFB25B");

            entity.HasOne(d => d.Student).WithMany(p => p.TbLibraryBookStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Librar__Stude__05A3D694");
        });

        modelBuilder.Entity<TbLibraryFine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Libra__3214EC07E529623C");

            entity.ToTable("tb_LibraryFine");

            entity.Property(e => e.FineAmount).HasColumnType("money");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Fee).WithMany(p => p.TbLibraryFines)
                .HasForeignKey(d => d.FeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Librar__FeeId__0697FACD");

            entity.HasOne(d => d.School).WithMany(p => p.TbLibraryFines)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Librar__Schoo__078C1F06");
        });

        modelBuilder.Entity<TbListenningSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Liste__3214EC0717E9FADF");

            entity.ToTable("tb_ListenningSkills");

            entity.Property(e => e.Comprehension)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COMPREHENSION");
            entity.Property(e => e.ListeningSkill)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Listening_Skill");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbListenningSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Listen__Asses__32CB82C6");

            entity.HasOne(d => d.Division).WithMany(p => p.TbListenningSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Listen__Divis__1FB8AE52");

            entity.HasOne(d => d.School).WithMany(p => p.TbListenningSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Listen__Schoo__1DD065E0");

            entity.HasOne(d => d.Student).WithMany(p => p.TbListenningSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Listen__Stude__1EC48A19");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbListenningSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Listen__Subje__20ACD28B");

            entity.HasOne(d => d.User).WithMany(p => p.TbListenningSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Listen__UserI__21A0F6C4");
        });

        modelBuilder.Entity<TbLkgUkgNurserySkill>(entity =>
        {
            entity.HasKey(e => e.SkillsId);

            entity.ToTable("tb_LKG_UKG_NURSERY_SKILLS");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbLogin>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__TbLogins__1788CC4C72DFAEB7");

            entity.ToTable("tb_Login");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbLogins)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TbLogins__School__06CD04F7");
        });

        modelBuilder.Entity<TbLoginAdmin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__TbLogins__719FE4889A4BFA4D");

            entity.ToTable("TbLoginsAdmin");
        });

        modelBuilder.Entity<TbMaster>(entity =>
        {
            entity.HasKey(e => e.MasterId).HasName("PK__tb_Maste__F6B78224F8134CB6");

            entity.ToTable("tb_Master");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbMasters)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Master__Schoo__5C02A283");

            entity.HasOne(d => d.User).WithMany(p => p.TbMasters)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Master__UserI__5CF6C6BC");
        });

        modelBuilder.Entity<TbMenuList>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__tb_MenuL__C99ED230B59A09FE");

            entity.ToTable("tb_MenuList");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__tb_Messa__C87C0C9CF328DF15");

            entity.ToTable("tb_Message");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Student).WithMany(p => p.TbMessages)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Messag__Stude__09746778");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TbMessages)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Messag__Teach__0A688BB1");
        });

        modelBuilder.Entity<TbModuleHome>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Modul__3214EC0742B9850A");

            entity.ToTable("tb_ModuleHome");

            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbMusicDanceSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Music__3214EC07137AA172");

            entity.ToTable("tb_MusicDanceSkills");

            entity.Property(e => e.Interest)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("INTEREST");
            entity.Property(e => e.Rhythm)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RHYTHM");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbMusicDanceSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_MusicD__Asses__4301EA8F");

            entity.HasOne(d => d.Division).WithMany(p => p.TbMusicDanceSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_MusicD__Divis__40257DE4");

            entity.HasOne(d => d.School).WithMany(p => p.TbMusicDanceSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_MusicD__Schoo__3E3D3572");

            entity.HasOne(d => d.Student).WithMany(p => p.TbMusicDanceSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_MusicD__Stude__3F3159AB");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbMusicDanceSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_MusicD__Subje__4119A21D");

            entity.HasOne(d => d.User).WithMany(p => p.TbMusicDanceSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_MusicD__UserI__420DC656");
        });

        modelBuilder.Entity<TbNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Notif__3214EC07356F7D63");

            entity.ToTable("tb_Notification");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.NotificationMessage).IsUnicode(false);
            entity.Property(e => e.Source)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbOneFourSkill>(entity =>
        {
            entity.HasKey(e => e.SkillsId).HasName("PK_ONE_FOUR_SKILLS");

            entity.ToTable("tb_ONE_FOUR_SKILLS");

            entity.Property(e => e.Evs).HasColumnName("EVS");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.Yle).HasColumnName("YLE");
            entity.Property(e => e.YleyoungLearnerEnglish).HasColumnName("YLEYoungLearnerEnglish");
        });

        modelBuilder.Entity<TbOptionalSubjectStudent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Optio__3214EC072E346D21");

            entity.ToTable("tb_OptionalSubjectStudents");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbOptionalSubjectStudents)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Option__Schoo__4A18FC72");

            entity.HasOne(d => d.Student).WithMany(p => p.TbOptionalSubjectStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Option__Stude__4B0D20AB");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbOptionalSubjectStudents)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Option__Subje__4C0144E4");
        });

        modelBuilder.Entity<TbOtpmessage>(entity =>
        {
            entity.HasKey(e => e.OtpId).HasName("PK__tb_OTPMe__3143C4A39851E744");

            entity.ToTable("tb_OTPMessage");

            entity.Property(e => e.ExpTimeStamp).HasColumnType("datetime");
            entity.Property(e => e.Otp).HasColumnName("OTP");
            entity.Property(e => e.Otptype).HasColumnName("OTPType");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Student).WithMany(p => p.TbOtpmessages)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_OTPMes__Stude__0B5CAFEA");
        });

        modelBuilder.Entity<TbParent>(entity =>
        {
            entity.HasKey(e => e.ParentId).HasName("PK__tb_Paren__D339516FF0816AF5");

            entity.ToTable("tb_Parent");

            entity.Property(e => e.ContactNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbParentMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__tb_Paren__C87C0C9CE381945B");

            entity.ToTable("tb_ParentMessage");

            entity.Property(e => e.ReadStatus).HasDefaultValue(true);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Sender).WithMany(p => p.TbParentMessages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Parent__Sende__0C50D423");

            entity.HasOne(d => d.Student).WithMany(p => p.TbParentMessages)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Parent__Stude__0D44F85C");
        });

        modelBuilder.Entity<TbPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__tb_Payme__9B556A38A5249213");

            entity.ToTable("tb_Payment");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.ChequeDate).HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("money");
            entity.Property(e => e.MaxAmount).HasColumnType("money");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbPayments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Paymen__Class__0E391C95");

            entity.HasOne(d => d.Fee).WithMany(p => p.TbPayments)
                .HasForeignKey(d => d.FeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Paymen__FeeId__0F2D40CE");

            entity.HasOne(d => d.School).WithMany(p => p.TbPayments)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Paymen__Schoo__10216507");

            entity.HasOne(d => d.Student).WithMany(p => p.TbPayments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Paymen__Stude__11158940");
        });

        modelBuilder.Entity<TbPaymentBillNo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Payme__3214EC0741023032");

            entity.ToTable("tb_PaymentBillNo");

            entity.HasOne(d => d.School).WithMany(p => p.TbPaymentBillNos)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Paymen__Schoo__1209AD79");
        });

        modelBuilder.Entity<TbPaymentTest>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__tb_Payme__9B556A383494FC19");

            entity.ToTable("tb_PaymentTest");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.Discount).HasColumnType("money");
            entity.Property(e => e.MaxAmount).HasColumnType("money");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbPushDatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tb_PushData");

            entity.HasOne(d => d.School).WithMany()
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK__tb_PushDa__Schoo__12FDD1B2");
        });

        modelBuilder.Entity<TbReadingSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Readi__3214EC07E0F9BA1D");

            entity.ToTable("tb_ReadingSkills");

            entity.Property(e => e.Fluency)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FLUENCY");
            entity.Property(e => e.Pronun)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PRONUN");
            entity.Property(e => e.ReadingSkill)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Reading_Skill");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbReadingSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Readin__Asses__2FEF161B");

            entity.HasOne(d => d.Division).WithMany(p => p.TbReadingSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Readin__Divis__0BB1B5A5");

            entity.HasOne(d => d.School).WithMany(p => p.TbReadingSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Readin__Schoo__09C96D33");

            entity.HasOne(d => d.Student).WithMany(p => p.TbReadingSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Readin__Stude__0ABD916C");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbReadingSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Readin__Subje__0CA5D9DE");

            entity.HasOne(d => d.User).WithMany(p => p.TbReadingSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Readin__UserI__0D99FE17");
        });

        modelBuilder.Entity<TbRegions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Regio__3214EC073735FD2C");

            entity.ToTable("tb_Region");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbRegionss)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Region__Schoo__7EC1CEDB");
        });

        modelBuilder.Entity<TbRegionsClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Regio__3214EC07FEF3B9E0");

            entity.ToTable("tb_RegionClass");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbRegionsClasses)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Region__Class__2F650636");

            entity.HasOne(d => d.Region).WithMany(p => p.TbRegionsClasses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Region__Regio__2E70E1FD");
        });

        modelBuilder.Entity<TbRemark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Remar__3214EC07EE06056B");

            entity.ToTable("tb_Remark");

            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbRemarks)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Remark__Schoo__72E607DB");
        });

        modelBuilder.Entity<TbResetPassword>(entity =>
        {
            entity.HasKey(e => e.ResetPasswordId).HasName("PK__tb_Reset__805BA2629610B551");

            entity.ToTable("tb_ResetPassword");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbRoleDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_RoleD__3214EC07A21046F2");

            entity.ToTable("tb_RoleDetails");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbRoleDetails)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_RoleDe__Schoo__689D8392");
        });

        modelBuilder.Entity<TbScholasticArea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Schol__3214EC073CBD522D");

            entity.ToTable("TbScholasticAreas");

            entity.Property(e => e.DividedBy).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.TotalScore).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Region).WithMany(p => p.TbScholasticAreas)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Schola__Regio__0662F0A3");

            entity.HasOne(d => d.School).WithMany(p => p.TbScholasticAreas)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Schola__Schoo__056ECC6A");
        });

        modelBuilder.Entity<TbScholasticResultMain>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Schol__3214EC07FEFEB9D5");

            entity.ToTable("tb_ScholasticResultMain");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbScholasticResultMains)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Schola__ExamI__1D4655FB");

            entity.HasOne(d => d.Scholastic).WithMany(p => p.TbScholasticResultMains)
                .HasForeignKey(d => d.ScholasticId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Schola__Schol__1E3A7A34");

            entity.HasOne(d => d.School).WithMany(p => p.TbScholasticResultMains)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Schola__Schoo__1B5E0D89");

            entity.HasOne(d => d.Student).WithMany(p => p.TbScholasticResultMains)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Schola__Stude__1C5231C2");
        });

        modelBuilder.Entity<TbSchool>(entity =>
        {
            entity.HasKey(e => e.SchoolId).HasName("PK__tb_Schoo__3DA4675BCD88A38D");

            entity.ToTable("tb_School");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbSchoolSenderId>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Schoo__3214EC074C26D3D0");

            entity.ToTable("tb_SchoolSenderId");

            entity.HasOne(d => d.School).WithMany(p => p.TbSchoolSenderIds)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_School__Schoo__13F1F5EB");
        });

        modelBuilder.Entity<TbScolasticAreaResultDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Scola__3214EC07E55AE8B1");

            entity.ToTable("tb_ScolasticAreaResultDetails");

            entity.Property(e => e.Score).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Main).WithMany(p => p.TbScolasticAreaResultDetails)
                .HasForeignKey(d => d.MainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Scolas__MainI__2116E6DF");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbScolasticAreaResultDetails)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Scolas__Subje__220B0B18");
        });

        modelBuilder.Entity<TbSetting>(entity =>
        {
            entity.HasKey(e => e.SettingsId).HasName("PK__tb_Setti__991B19FCF2C1C7BE");

            entity.ToTable("tb_Settings");

            entity.Property(e => e.FeeStartDate).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbSettings)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK__tb_Settin__Schoo__14E61A24");
        });

        modelBuilder.Entity<TbSmsHead>(entity =>
        {
            entity.HasKey(e => e.HeadId).HasName("PK__tb_SmsHe__EB3F2510A600EEFB");

            entity.ToTable("tb_SmsHead");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbSmsHeads)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_SmsHea__Schoo__15DA3E5D");
        });

        modelBuilder.Entity<TbSmsHistory>(entity =>
        {
            entity.ToTable("tb_SmsHistory");

            entity.Property(e => e.DelivaryStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MessageDate).HasColumnType("datetime");
            entity.Property(e => e.SendStatus).IsUnicode(false);

            entity.HasOne(d => d.Head).WithMany(p => p.TbSmsHistories)
                .HasForeignKey(d => d.HeadId)
                .HasConstraintName("FK__tb_SmsHis__HeadI__16CE6296");

            entity.HasOne(d => d.Schol).WithMany(p => p.TbSmsHistories)
                .HasForeignKey(d => d.ScholId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_SmsHistory_tb_School");

            entity.HasOne(d => d.Stuent).WithMany(p => p.TbSmsHistories)
                .HasForeignKey(d => d.StuentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_SmsHistory_TbStudents");
        });

        modelBuilder.Entity<TbSmsPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__tb_SmsPa__322035CC84624286");

            entity.ToTable("tb_SmsPackage");

            entity.Property(e => e.FromDate).HasColumnType("datetime");
            entity.Property(e => e.SmsRate).HasColumnType("money");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.ToDate).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbSmsPackages)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_SmsPac__Schoo__19AACF41");
        });

        modelBuilder.Entity<TbSmtpdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_SMTPD__3214EC072E827D80");

            entity.ToTable("tb_SMTPDetail");

            entity.HasOne(d => d.School).WithMany(p => p.TbSmtpdetails)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_SMTPDe__Schoo__1A9EF37A");
        });

        modelBuilder.Entity<TbSpeakingSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Speak__3214EC07D3713271");

            entity.ToTable("tb_SpeakingSkills");

            entity.Property(e => e.Conversation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CONVERSATION");
            entity.Property(e => e.Recitation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RECITATION");
            entity.Property(e => e.SpeakingSkill)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Speaking_Skill");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbSpeakingSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Speaki__Asses__31D75E8D");

            entity.HasOne(d => d.Division).WithMany(p => p.TbSpeakingSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Speaki__Divis__190BB0C3");

            entity.HasOne(d => d.School).WithMany(p => p.TbSpeakingSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Speaki__Schoo__17236851");

            entity.HasOne(d => d.Student).WithMany(p => p.TbSpeakingSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Speaki__Stude__18178C8A");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbSpeakingSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Speaki__Subje__19FFD4FC");

            entity.HasOne(d => d.User).WithMany(p => p.TbSpeakingSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Speaki__UserI__1AF3F935");
        });

        modelBuilder.Entity<TbStaff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__tb_Staff__96D4AB17C89DC25F");

            entity.ToTable("tb_Staff");

            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Department).WithMany(p => p.TbStaffs)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__tb_Staff__Depart__703EA55A");

            entity.HasOne(d => d.Designation).WithMany(p => p.TbStaffs)
                .HasForeignKey(d => d.DesignationId)
                .HasConstraintName("FK__tb_Staff__Design__6F4A8121");

            entity.HasOne(d => d.User).WithMany(p => p.TbStaffs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Staff__UserId__1B9317B3");

            entity.HasOne(d => d.UserTypeNavigation).WithMany(p => p.TbStaffs)
                .HasForeignKey(d => d.UserType)
                .HasConstraintName("FK__tb_Staff__UserTy__1A34DF26");
        });

        modelBuilder.Entity<TbStaffSmshistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Staff__3214EC078147E7D8");

            entity.ToTable("tb_StaffSMSHistory");

            entity.Property(e => e.DelivaryStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MessageDate).HasColumnType("datetime");
            entity.Property(e => e.SendStatus).IsUnicode(false);

            entity.HasOne(d => d.Head).WithMany(p => p.TbStaffSmshistories)
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_StaffS__HeadI__1C873BEC");

            entity.HasOne(d => d.Schol).WithMany(p => p.TbStaffSmshistories)
                .HasForeignKey(d => d.ScholId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_StaffS__Schol__1D7B6025");

            entity.HasOne(d => d.Staff).WithMany(p => p.TbStaffSmshistories)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_StaffS__Staff__1E6F845E");
        });

        modelBuilder.Entity<TbState>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("PK__tb_State__C3BA3B3A690D7C4B");

            entity.ToTable("tb_State");
        });

        modelBuilder.Entity<TbStockUpdate>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("PK__tb_Stock__2C83A9C2DF5C645F");

            entity.ToTable("tb_StockUpdate");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.TbStockUpdates)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_StockU__Categ__1F63A897");

            entity.HasOne(d => d.School).WithMany(p => p.TbStockUpdates)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_StockU__Schoo__2057CCD0");

            entity.HasOne(d => d.User).WithMany(p => p.TbStockUpdates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_StockU__UserI__214BF109");
        });

        modelBuilder.Entity<TbStudent>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__tb_Stude__32C52B9917E029DB");

            entity.ToTable("tb_Student");

            entity.Property(e => e.BloodGroup).HasMaxLength(50);
            entity.Property(e => e.DateOfJoining).HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Bus).WithMany(p => p.TbStudents)
                .HasForeignKey(d => d.BusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__BusId__2F10007B");

            entity.HasOne(d => d.Class).WithMany(p => p.TbStudents)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Class__300424B4");

            entity.HasOne(d => d.Division).WithMany(p => p.TbStudents)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Divis__30F848ED");

            entity.HasOne(d => d.Parent).WithMany(p => p.TbStudents)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__tb_Studen__Paren__31EC6D26");

            entity.HasOne(d => d.School).WithMany(p => p.TbStudents)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Schoo__32E0915F");
        });

        modelBuilder.Entity<TbStudentAssesmentUpload>(entity =>
        {
            entity.HasKey(e => e.FileId);

            entity.ToTable("TbStudentsAssesmentUpload");

            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.TeacherFile).WithMany(p => p.TbStudentAssesmentUploads)
                .HasForeignKey(d => d.TeacherFileId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_StudentUpload_TeacherUpload");
        });

        modelBuilder.Entity<TbStudentBalance>(entity =>
        {
            entity.HasKey(e => e.BalanceId).HasName("PK__tb_Stude__A760D5BE4DAE235D");

            entity.ToTable("TbStudentsBalance");

            entity.Property(e => e.Amount).HasColumnType("money");

            entity.HasOne(d => d.Student).WithMany(p => p.TbStudentBalances)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Stude__2704CA5F");
        });

        modelBuilder.Entity<TbStudentMark>(entity =>
        {
            entity.HasKey(e => e.MarkId).HasName("PK__tb_Stude__4E30D366BCA50490");

            entity.ToTable("TbStudentsMarks");

            entity.Property(e => e.ExternalMark).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.InternalMark).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbStudentMarks)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__ExamI__27F8EE98");

            entity.HasOne(d => d.Student).WithMany(p => p.TbStudentMarks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Stude__28ED12D1");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbStudentMarks)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Subje__29E1370A");
        });

        modelBuilder.Entity<TbStudentPaidAmount>(entity =>
        {
            entity.HasKey(e => e.PaidId).HasName("PK__tb_Stude__B6E768CD331F3577");

            entity.ToTable("TbStudentsPaidAmount");

            entity.Property(e => e.BalanceAmount).HasColumnType("money");
            entity.Property(e => e.PaidAmount).HasColumnType("money");
            entity.Property(e => e.PreviousBalance).HasColumnType("money");

            entity.HasOne(d => d.Student).WithMany(p => p.TbStudentPaidAmounts)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Stude__2AD55B43");
        });

        modelBuilder.Entity<TbStudentPremotion>(entity =>
        {
            entity.HasKey(e => e.PremotionId).HasName("PK__tb_Stude__AA1632F010D61433");

            entity.ToTable("TbStudentsPremotion");

            entity.Property(e => e.LastUpdate).HasDefaultValue(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.FromDivisionNavigation).WithMany(p => p.TbStudentPremotionFromDivisionNavigations)
                .HasForeignKey(d => d.FromDivision)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbStudentsPremotion_tb_Division");

            entity.HasOne(d => d.OldClassNavigation).WithMany(p => p.TbStudentPremotions)
                .HasForeignKey(d => d.OldClass)
                .HasConstraintName("FK_TbStudentsPremotion_tb_Class");

            entity.HasOne(d => d.School).WithMany(p => p.TbStudentPremotions)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK_TbStudentsPremotion_tb_School");

            entity.HasOne(d => d.Student).WithMany(p => p.TbStudentPremotions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Stude__5F3414E9");

            entity.HasOne(d => d.ToDivisionNavigation).WithMany(p => p.TbStudentPremotionToDivisionNavigations)
                .HasForeignKey(d => d.ToDivision)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__ToDiv__60283922");
        });

        modelBuilder.Entity<TbStudentRemark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Stude__3214EC078ACFBD14");

            entity.ToTable("TbStudentsRemark");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbStudentRemarks)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__ExamI__5C37ACAD");

            entity.HasOne(d => d.Student).WithMany(p => p.TbStudentRemarks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Stude__5D2BD0E6");
        });

        modelBuilder.Entity<TbStudentTotalScore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Stude__3214EC07FADB5859");

            entity.ToTable("TbStudentsTotalScore");

            entity.Property(e => e.Percentage).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.TotalScore).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Class).WithMany(p => p.TbStudentTotalScores)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Class__2AF556D4");

            entity.HasOne(d => d.Division).WithMany(p => p.TbStudentTotalScores)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Divis__2BE97B0D");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbStudentTotalScores)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__ExamI__2DD1C37F");

            entity.HasOne(d => d.School).WithMany(p => p.TbStudentTotalScores)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Schoo__2CDD9F46");

            entity.HasOne(d => d.Student).WithMany(p => p.TbStudentTotalScores)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Studen__Stude__2A01329B");
        });

        modelBuilder.Entity<TbSubLedgerDatum>(entity =>
        {
            entity.HasKey(e => e.LedgerId).HasName("PK__tb_SubLe__AE70E0CF7A89E067");

            entity.ToTable("TbSubLedgerData");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.AccHead).WithMany(p => p.TbSubLedgerData)
                .HasForeignKey(d => d.AccHeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_SubLed__AccHe__2F9A1060");
        });

        modelBuilder.Entity<TbSubModule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_SubMo__3214EC07A193FBA3");

            entity.ToTable("tb_SubModule");

            entity.Property(e => e.SubModule).HasColumnName("SUbModule");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Main).WithMany(p => p.TbSubModules)
                .HasForeignKey(d => d.MainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_SubMod__MainI__025D5595");
        });

        modelBuilder.Entity<TbSubject>(entity =>
        {
            entity.HasKey(e => e.SubId).HasName("PK__tb_Subje__4D9BB84A32A62E5C");

            entity.ToTable("tb_Subjects");

            entity.Property(e => e.TmeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.SchoolINavigation).WithMany(p => p.TbSubjects)
                .HasForeignKey(d => d.SchoolI)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Subjec__Schoo__2EA5EC27");
        });

        modelBuilder.Entity<TbTeacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__tb_Teach__EDF25964C5CDEE63");

            entity.ToTable("tb_Teacher");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasDefaultValue(1L);

            entity.HasOne(d => d.Department).WithMany(p => p.TbTeachers)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__tb_Teache__Depar__6A85CC04");

            entity.HasOne(d => d.Designation).WithMany(p => p.TbTeachers)
                .HasForeignKey(d => d.DesignationId)
                .HasConstraintName("FK__tb_Teache__Desig__6991A7CB");

            entity.HasOne(d => d.School).WithMany(p => p.TbTeachers)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Teache__Schoo__36B12243");

            entity.HasOne(d => d.User).WithMany(p => p.TbTeachers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbLogins_UserID");

            entity.HasOne(d => d.UserTypeNavigation).WithMany(p => p.TbTeachers)
                .HasForeignKey(d => d.UserType)
                .HasConstraintName("FK__tb_Teache__UserT__1940BAED");
        });

        modelBuilder.Entity<TbTeacherClass>(entity =>
        {
            entity.HasKey(e => e.TeacherClassId).HasName("PK__tb_Teach__8FE4FE127FCD8DC0");

            entity.ToTable("tb_TeacherClass");

            entity.HasOne(d => d.Class).WithMany(p => p.TbTeacherClasses)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Teache__Class__49C3F6B7");

            entity.HasOne(d => d.Division).WithMany(p => p.TbTeacherClasses)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Teache__Divis__336AA144");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TbTeacherClasses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Teache__Teach__345EC57D");
        });

        modelBuilder.Entity<TbTeacherClassSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Teach__3214EC0752577D77");

            entity.ToTable("tb_TeachersClassSubject");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbTeacherClassSubjects)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Teache__Class__33008CF0");

            entity.HasOne(d => d.Division).WithMany(p => p.TbTeacherClassSubjects)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Teache__Divis__33F4B129");

            entity.HasOne(d => d.School).WithMany(p => p.TbTeacherClassSubjects)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Teache__Schoo__3118447E");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbTeacherClassSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Teache__Subje__34E8D562");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TbTeacherClassSubjects)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Teache__Teach__320C68B7");
        });

        modelBuilder.Entity<TbTimeTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_TimeT__3214EC078B40FF1B");

            entity.ToTable("tb_TimeTable");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TbTimeTables)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_TimeTa__Class__3552E9B6");

            entity.HasOne(d => d.Division).WithMany(p => p.TbTimeTables)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_TimeTa__Divis__36470DEF");

            entity.HasOne(d => d.School).WithMany(p => p.TbTimeTables)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_TimeTa__Schoo__373B3228");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbTimeTables)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_TimeTa__Subje__382F5661");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TbTimeTables)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_TimeTa__Teach__39237A9A");
        });

        modelBuilder.Entity<TbTravel>(entity =>
        {
            entity.HasKey(e => e.TravelId).HasName("PK__tb_Trave__E931523540D61962");

            entity.ToTable("tb_Travel");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Trip).WithMany(p => p.TbTravels)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Travel__TripI__0C85DE4D");
        });

        modelBuilder.Entity<TbTrip>(entity =>
        {
            entity.HasKey(e => e.TripId).HasName("PK__tb_Trip__51DC713E0E59E4ED");

            entity.ToTable("tb_Trip");

            entity.Property(e => e.ReachTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.TripDate).HasColumnType("datetime");

            entity.HasOne(d => d.Bus).WithMany(p => p.TbTrips)
                .HasForeignKey(d => d.BusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Trip__BusId__6E01572D");

            entity.HasOne(d => d.Driver).WithMany(p => p.TbTrips)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Trip__DriverI__34C8D9D1");

            entity.HasOne(d => d.School).WithMany(p => p.TbTrips)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Trip__SchoolI__35BCFE0A");
        });

        modelBuilder.Entity<TbUserAllotedMenu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_UserA__3214EC0725097231");

            entity.ToTable("tb_UserAllotedMenu");

            entity.HasOne(d => d.Menu).WithMany(p => p.TbUserAllotedMenus)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("FK__tb_UserAl__MenuI__3DE82FB7");

            entity.HasOne(d => d.User).WithMany(p => p.TbUserAllotedMenus)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__tb_UserAl__UserI__3EDC53F0");
        });

        modelBuilder.Entity<TbUserModuleDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_UserM__3214EC071A0D0A8A");

            entity.ToTable("tb_UserModuleDetails");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Main).WithMany(p => p.TbUserModuleDetails)
                .HasForeignKey(d => d.MainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_UserMo__MainI__1758727B");

            entity.HasOne(d => d.SubModule).WithMany(p => p.TbUserModuleDetails)
                .HasForeignKey(d => d.SubModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_UserMo__SubMo__184C96B4");

            entity.HasOne(d => d.UserModule).WithMany(p => p.TbUserModuleDetails)
                .HasForeignKey(d => d.UserModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_UserMo__UserM__16644E42");
        });

        modelBuilder.Entity<TbUserModuleMain>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_UserM__3214EC07ECA696EE");

            entity.ToTable("tb_UserModuleMain");

            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbUserModuleMains)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_UserMo__Schoo__1387E197");
        });

        modelBuilder.Entity<TbUserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_UserR__3214EC07D46BDDE1");

            entity.ToTable("tb_UserRole");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.TbUserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_UserRo__RoleI__6E565CE8");

            entity.HasOne(d => d.User).WithMany(p => p.TbUserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_UserRo__UserI__6D6238AF");
        });

        modelBuilder.Entity<TbVAssesment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_V_Ass__3214EC076E4494CD");

            entity.ToTable("tb_V_Assesment");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Period).WithMany(p => p.TbVAssesments)
                .HasForeignKey(d => d.PeriodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Asse__Perio__05F8DC4F");

            entity.HasOne(d => d.School).WithMany(p => p.TbVAssesments)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Asse__Schoo__06ED0088");
        });

        modelBuilder.Entity<TbVGame>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_V_Gam__3214EC0750AB27CF");

            entity.ToTable("tb_V_Games");

            entity.Property(e => e.Discipline)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Enthusiasm)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TeamSpirit)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbVGames)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Game__Asses__48BAC3E5");

            entity.HasOne(d => d.Division).WithMany(p => p.TbVGames)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Game__Divis__47C69FAC");

            entity.HasOne(d => d.School).WithMany(p => p.TbVGames)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Game__Schoo__46D27B73");

            entity.HasOne(d => d.Student).WithMany(p => p.TbVGames)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Game__Stude__5BCD9859");

            entity.HasOne(d => d.User).WithMany(p => p.TbVGames)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Game__UserI__49AEE81E");
        });

        modelBuilder.Entity<TbVHealth>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_V_Hea__3214EC070EF9DB95");

            entity.ToTable("tb_V_Health");

            entity.Property(e => e.Height)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.Weight)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbVHealths)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Heal__Asses__4E739D3B");

            entity.HasOne(d => d.Division).WithMany(p => p.TbVHealths)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Heal__Divis__4D7F7902");

            entity.HasOne(d => d.School).WithMany(p => p.TbVHealths)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Heal__Schoo__4C8B54C9");

            entity.HasOne(d => d.Student).WithMany(p => p.TbVHealths)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Heal__Stude__5CC1BC92");

            entity.HasOne(d => d.User).WithMany(p => p.TbVHealths)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Heal__UserI__4F67C174");
        });

        modelBuilder.Entity<TbVPersonalityDevelopment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_V_Per__3214EC07A59A4243");

            entity.ToTable("tb_V_Personality_Development");

            entity.Property(e => e.CareOfBelongings)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Confident)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Courteousness)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Initiative)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Neatness)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Property)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Punctuality)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SharingCaring)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbVPersonalityDevelopments)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Pers__Asses__59E54FE7");

            entity.HasOne(d => d.Division).WithMany(p => p.TbVPersonalityDevelopments)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Pers__Divis__58F12BAE");

            entity.HasOne(d => d.School).WithMany(p => p.TbVPersonalityDevelopments)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Pers__Schoo__57FD0775");

            entity.HasOne(d => d.Student).WithMany(p => p.TbVPersonalityDevelopments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Pers__Stude__5DB5E0CB");

            entity.HasOne(d => d.User).WithMany(p => p.TbVPersonalityDevelopments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Pers__UserI__5AD97420");
        });

        modelBuilder.Entity<TbVRemark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_V_Rem__3214EC073265F112");

            entity.ToTable("tb_V_Remark");

            entity.Property(e => e.Remark).IsUnicode(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbVRemarks)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Rema__Asses__542C7691");

            entity.HasOne(d => d.Division).WithMany(p => p.TbVRemarks)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Rema__Divis__53385258");

            entity.HasOne(d => d.School).WithMany(p => p.TbVRemarks)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Rema__Schoo__52442E1F");

            entity.HasOne(d => d.Student).WithMany(p => p.TbVRemarks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Rema__Stude__5EAA0504");

            entity.HasOne(d => d.User).WithMany(p => p.TbVRemarks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Rema__UserI__55209ACA");
        });

        modelBuilder.Entity<TbVTotalScoreList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_V_Tot__3214EC075A6A9C21");

            entity.ToTable("tb_V_TotalScoreList");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbVTotalScoreLists)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_V_Tota__Schoo__03DB89B3");
        });

        modelBuilder.Entity<TbVoucherNumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Vouch__3214EC07B6E3BB9B");

            entity.ToTable("tb_VoucherNumber");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.School).WithMany(p => p.TbVoucherNumbers)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Vouche__Schoo__3FD07829");
        });

        modelBuilder.Entity<TbWritingSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_Writi__3214EC075BFEB318");

            entity.ToTable("tb_WritingSkills");

            entity.Property(e => e.CreativeWriting)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CREATIVE_WRITING");
            entity.Property(e => e.Grammar)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GRAMMAR");
            entity.Property(e => e.HandWriting)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("HAND_WRITING");
            entity.Property(e => e.Spellings)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SPELLINGS");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.UnitTest)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UNIT_TEST");
            entity.Property(e => e.Vocabulary)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("VOCABULARY");
            entity.Property(e => e.WorkSheet)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WORK_SHEET");
            entity.Property(e => e.WritingSkill)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Writing_Skill");

            entity.HasOne(d => d.Assesment).WithMany(p => p.TbWritingSkills)
                .HasForeignKey(d => d.AssesmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Writin__Asses__30E33A54");

            entity.HasOne(d => d.Division).WithMany(p => p.TbWritingSkills)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Writin__Divis__125EB334");

            entity.HasOne(d => d.School).WithMany(p => p.TbWritingSkills)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Writin__Schoo__10766AC2");

            entity.HasOne(d => d.Student).WithMany(p => p.TbWritingSkills)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Writin__Stude__116A8EFB");

            entity.HasOne(d => d.Subject).WithMany(p => p.TbWritingSkills)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Writin__Subje__1352D76D");

            entity.HasOne(d => d.User).WithMany(p => p.TbWritingSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Writin__UserI__1446FBA6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
