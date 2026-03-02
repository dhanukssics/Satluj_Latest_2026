using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbAssesmentUpload
{
    public int ClassId { get; set; }

    public string Filename { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? FileUploadedDate { get; set; }

    public int FileId { get; set; }

    public int? SubjectId { get; set; }

    public string? TeacherName { get; set; }

    public string? SubjectName { get; set; }

    public string? DivisionName { get; set; }

    public int? Division { get; set; }

    public virtual ICollection<TbStudentAssesmentUpload> TbStudentAssesmentUploads { get; set; } = new List<TbStudentAssesmentUpload>();
}
