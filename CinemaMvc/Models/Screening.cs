using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaMvc.Models
{
    public class Screening
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string FilmTitle { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public int CinemaId { get; set; }

        [ForeignKey(nameof(CinemaId))]
        public Cinema? Cinema { get; set; }

            public ICollection<Reservation>Reservations{get;set;}=new List<Reservation>();
  
    }

}