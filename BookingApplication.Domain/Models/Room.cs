using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Domain.Models
{
    public class Room
    {
        [Required]
        public Guid RoomId { get; set; } 

        [Required(ErrorMessage ="Debe Ingresar un Numero de Habitacion!!")]
        public int? RoomNumber { get; set; }

        [Required(ErrorMessage = "Debe Ingresar en Metros Cuadrados el Tamaño de la Habitacion!!")]
        public double? RoomSize {  get; set; }

        [Required(ErrorMessage ="Debe Ingresar si la Habitacion Necesita alguna Reparacion!!")]
        public bool? NeedRepair { get; set; } = false;

        public List<RoomReservationDate> DateReservationForClient { get; set; } = new List<RoomReservationDate>();    

        [Required]
        public Guid? HotelId { get; set; }

        [Required]
        public Hotel? Hotel { get; set; }

    }
}
