
namespace SS.Core.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string? Bio { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public string? LinkedIn { get; set; }
        public string? GitHub { get; set; }
        public string? LeetCode { get; set; }

        public string? University { get; set; }
        public string? Degree { get; set; }
        public string? Department { get; set; }
        public int? GraduationYear { get; set; }
        public string? GPA { get; set; }

        public string? CompanyName { get; set; }
        public string? Position { get; set; }
        public DateTime? ExperienceFrom { get; set; }
        public DateTime? ExperienceTo { get; set; }
        public string? TotalExperience { get; set; }
        public string? NoticePeriod { get; set; }

        public string? ProfileImage { get; set; }
        public string? ResumeFile { get; set; }
        public string? ResumeUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
