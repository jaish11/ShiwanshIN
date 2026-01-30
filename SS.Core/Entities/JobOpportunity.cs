
namespace SS.Core.Entities
{
    public class JobOpportunity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // e.g., Full-time, Part-time, Contract, Internship
        public string Experience { get; set; }
        public string Salary { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }

        //this is for company name and created by user
        public string CompanyName { get; set; }   
        public int CreatedByUserId { get; set; }
        public bool IsActive { get; set; }

        public string Image { get; set; }
        public string? Duration { get; set; }
        public string Location { get; set; }
        public string SkillsJson { get; set; }
        public string ResponsibilitiesJson { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Responsibilities { get; set; }
        public DateTime CreatedDate { get; set; }

    }

}
