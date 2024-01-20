using System.ComponentModel.DataAnnotations;

namespace backend_pastebook_capstone.Models
{
    public class Verification
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string VerificationCode { get; set; } = null!;
    }

    public class VerificationDTO
    {
        public string Email { get; set; } = null!;
        public string VerificationCode { get; set; } = null!;
    }
}
