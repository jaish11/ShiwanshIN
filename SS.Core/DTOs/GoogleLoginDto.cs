
using SS.Core.Entities;

namespace SS.Core.DTOs
{
    public class GoogleLoginDto
    {
        public string IdToken { get; set; }
        public User User { get; set; }
    }
}
