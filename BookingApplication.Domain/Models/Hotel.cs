using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Domain.Models
{
    public class Hotel
    {
        [Required]
        [Key]
        public Guid HotelId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage ="Debe Ingresar un Nombre al Hotel.")]
        [DataType("nvarchar(100)")]
        [MaxLength(100,ErrorMessage ="Debe Ingresar un Nombre de Hotel mas Corto!!.")]
        [MinLength(5,ErrorMessage ="Debe Ingresar un Nombre de Hotel mas Largo!!.")]

        public string? HotelName { get; set;}

        [Required(ErrorMessage ="Debe Ingresar la Calificacion de Estrellas del Hotel, (1) como Malo a (5) como Muy Buena Calidad!!")]
        [Range(1, 6,ErrorMessage ="El Numero de Estrellas debe ser entre (1) como Minimo a (5) como Maximo.")]
        public int StarsAssigned { get; set; }

        [Required(ErrorMessage = "Debe Ingresar una Dirreccion del Hotel.")]
        [DataType("nvarchar(500)")]
        [MaxLength(500, ErrorMessage = "La Dirrecion del Hotel Sobrepasa el Limite de Caracteres!!")]
        [MinLength(5, ErrorMessage = "La Dirrecion del Hotel es Demasiado Corto, Debe Ingresar una Dirrecion mas Larga!!.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Debe Ingresar La Ciudad donde se Encuentra el Hotel.")]
        [DataType("nvarchar(100)")]
        [MaxLength(100, ErrorMessage = "La Ciudad donde se Encuentra el Hotel Sobrepasa el Limite de Caracteres!!")]
        [MinLength(5, ErrorMessage = "La Ciudad donde se Encuentra el Hotel es Demasiado Corto!!")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Debe Ingresar El Pais donde se Encuentra el Hotel.")]
        [DataType("nvarchar(100)")]
        [MaxLength(100, ErrorMessage = "El Pais donde se Encuentra el Hotel Sobrepasa el Limite de Caracteres!!")]
        [MinLength(5, ErrorMessage = "El Pais donde se Encuentra el Hotel es Demasiado Corto!!")]
        public string? Country { get; set; }


        public List<Room>? ListOfRooms { get; set; }  
    }
}
