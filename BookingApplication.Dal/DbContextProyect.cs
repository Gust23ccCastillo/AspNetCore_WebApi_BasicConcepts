using BookingApplication.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApplication.Dal
{
    public class DbContextProyect:DbContext
    {
        public DbContextProyect(DbContextOptions<DbContextProyect> options):base(options)
        {
                
        }

        public DbSet<Hotel> _TableHotels { get; set; }
        public DbSet<Room> _TableRooms { get; set; }
        public DbSet<Reservation> _TableReservations { get; set; }
        public DbSet<RoomReservationDate>   _TableRoomReservationDates { get; set; }
        public DbSet<HotelReservationDate> _TableHotelReservationDates { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación Hotel → Room (Un hotel tiene muchas habitaciones)
            modelBuilder.Entity<Room>()
                .HasOne(entity => entity.Hotel)
                .WithMany(property => property.ListOfRooms)
                .HasForeignKey(associated_key => associated_key.HotelId)
                .OnDelete(DeleteBehavior.Cascade); // si permite eliminar un hotel si tiene habitaciones

          
            // Relación Room → RoomReservationDate (Una habitación tiene muchas fechas de reservacion)
            modelBuilder.Entity<RoomReservationDate>()
                .HasOne(entity => entity.Room)
                .WithMany(property => property.DateReservationForClient)
                .HasForeignKey(associated_key => associated_key.RoomId)
                .OnDelete(DeleteBehavior.Cascade); // permite eliminar una habitación si tiene reservas


            // Relación Reservation → HotelReservationDate (Una reserva tiene muchas fechas)
            modelBuilder.Entity<HotelReservationDate>()
                .HasOne(entity => entity.Reservation)
                .WithMany(property => property.ListToDateReservatedInHotel)
                .HasForeignKey(associated_key => associated_key.ReservationId)
                .OnDelete(DeleteBehavior.Restrict); // no permite eliminar la fechas de reservacion



            //A ESTOS DOS MODELOS SI ELIMINO LA RESERVACION, ELIMINARA LA INFORMACION DE LA RESERVACION PERO
            //NO ELIMINARA INFORMACION DE LA TABLA ROOM NI HOTEL

            // Relación Reservation → Room (Una reserva tiene una habitación)
            modelBuilder.Entity<Reservation>()
                .HasOne(entity => entity.RoomReservated)
                .WithMany()
                .HasForeignKey(associated_key => associated_key.RoomId)
                .OnDelete(DeleteBehavior.Restrict); // no permite eliminar una habitación si tiene reservas

            // Relación Reservation → Hotel (Una reserva tiene un hotel)
            modelBuilder.Entity<Reservation>()
                .HasOne(entity => entity.HotelReservated)
                .WithMany()
                .HasForeignKey(associated_key => associated_key.HotelId)
                .OnDelete(DeleteBehavior.Restrict); // no permite eliminar un hotel si tiene reservas


        }

        
    }
}
