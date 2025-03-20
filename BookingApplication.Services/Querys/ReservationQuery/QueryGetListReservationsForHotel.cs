using BookingApplication.Dal;
using BookingApplication.Services.MiddlewareGlobal;
using BookingApplication.Services.Querys.ReservationQuery.QueryReservationDtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Querys.ReservationQuery
{

    public class QueryGetListReservationsForHotel
    {
        public class GetListAllReservationForSpecificHotel : IRequest<List<ModelDto_Reservation_Information>> { }


        public class ModelServiceAndInformationLogic : IRequestHandler<GetListAllReservationForSpecificHotel, List<ModelDto_Reservation_Information>>
        {
            private readonly DbContextProyect _DbContextProyectInject;
           
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject)
            {
               this._DbContextProyectInject = DbContextProyectInject;
            }
            public async Task<List<ModelDto_Reservation_Information>> Handle(GetListAllReservationForSpecificHotel request, CancellationToken cancellationToken)
            {
               
                cancellationToken.ThrowIfCancellationRequested();
                var obtainTheReservationList = await this._DbContextProyectInject._TableReservations
                 .AsNoTracking()
                 .Include(hotelInformation => hotelInformation.HotelReservated)
                 .Include(roomReservation => roomReservation.RoomReservated)
                 .Select(reservation => new ModelDto_Reservation_Information
                 {
                      reservationId = reservation.ReservationId,
                      nameClient = reservation.Customer,
                      hotel_Information =  new ModelDto_Hotel_Information 
                      {
                         Name = reservation.HotelReservated.HotelName,
                         city = reservation.HotelReservated.City,
                         country = reservation.HotelReservated.Country
                      },
                      room_Information = new ModelDto_Room_Information
                      {
                        roomNumber =  reservation.RoomReservated.RoomNumber.Value,
                        size = reservation.RoomReservated.RoomSize.Value
                      },
                     // Ordenar fechas de manera descendente y formatear en "dd/MM/yyyy"
                     daysOfReservation = reservation.ListToDateReservatedInHotel
                        .OrderByDescending(date => date.ReservationDate)
                        .Select(date => new ModelDto_InfoDaysReservated
                        {
                            DayReservate = date.ReservationDate.ToString("dd/MM/yyyy")
                        })
                        .ToList()
                 })
                 .ToListAsync(cancellationToken);

                if (!obtainTheReservationList.Any())
                {
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound, 
                        new { MessageInformation = "No se encontraron reservaciones en ningun hotel, Porfavor intentelo mas tarde.." });
                }

                cancellationToken.ThrowIfCancellationRequested();
                return obtainTheReservationList;
            }
        }
    }
}
