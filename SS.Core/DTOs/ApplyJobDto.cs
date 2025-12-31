
namespace SS.Core.DTOs
{
    public class ApplyJobDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int UserId { get; set; }
        public string JobTitle { get; set; }
        public string JobType { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string? ResumeUrl { get; set; }
        public string? ResumeFile { get; set; }

        public string University { get; set; }
        public string Degree { get; set; }
        public string Department { get; set; }
        public string GPA { get; set; }
        public int GraduationYear { get; set; }

        public string? CompanyName { get; set; }
        public string? Position { get; set; }
        public string? TotalExperience { get; set; }
        public string? NoticePeriod { get; set; }
        public DateTime? ExperienceFrom { get; set; }
        public DateTime? ExperienceTo { get; set; }

        public string? LinkedIn { get; set; }
        public string? GitHub { get; set; }

        public DateTime AppliedDate { get; set; }
    }
}
