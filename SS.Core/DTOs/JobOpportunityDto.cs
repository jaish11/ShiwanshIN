
namespace SS.Core.DTOs
{
    public class JobOpportunityDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // e.g., Full-time, Part-time, Contract, Internship
        public string Experience { get; set; }
        public string Salary { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public string Duration { get; set; }
        public string Location { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Responsibilities { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
