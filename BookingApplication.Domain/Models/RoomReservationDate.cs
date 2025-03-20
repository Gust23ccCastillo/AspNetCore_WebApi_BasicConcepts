using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Domain.Models
{
    public class RoomReservationDate
    {
        [Key]
        public Guid RoomReservationDateId { get; set; } = Guid.NewGuid();
        
        public DateTime ReservationDate { get; set; }

        [Required]
        public Guid RoomId { get; set; }

        public Room Room { get; set; }
    }
}
