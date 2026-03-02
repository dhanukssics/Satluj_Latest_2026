using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStudentAssesmentUpload
{
    public int? StudentId { get; set; }

    public string? FileName { get; set; }

    public DateTime? UploadedDate { get; set; }

    public int FileId { get; set; }

    public int? ClassId { get; set; }

    public string? StudentName { get; set; }

    public string? Class { get; set; }

    public int? DivisionId { get; set; }

    public string? Division { get; set; }

    public int? SubjectId { get; set; }

    public string? SubjectName { get; set; }

    public string? Description { get; set; }

    public int? TeacherFileId { get; set; }

    public virtual TbAssesmentUpload? TeacherFile { get; set; }
}
