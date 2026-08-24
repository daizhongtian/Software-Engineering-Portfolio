using System.ComponentModel.DataAnnotations;

namespace CinemaMvc.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int ScreeningId { get; set; }
        public Screening Screening { get; set; } = null!;

        public int RowNumber { get; set; }

        public int SeatNumber { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    }
}
