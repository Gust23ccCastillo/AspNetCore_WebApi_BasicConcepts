using BookingApplication.Dal;
using BookingApplication.Services.MiddlewareGlobal;
using BookingApplication.Services.Querys.ReservationQuery.QueryReservationDtos;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Querys.ReservationQuery
{
    public class QueryGetSpecificReservationForHotel
    {
        public class GetSpecificReservationInformation : IRequest<ModelDto_Reservation_Information>
        {
            public Guid reservationId { get; set; }
        }

        public class FluentValidationData : AbstractValidator<GetSpecificReservationInformation>
        {
            public FluentValidationData()
            {
                RuleFor(x => x.reservationId)
                       .NotEmpty().WithMessage("Debe ingresar un ID de reservacion para buscar la informacion específica.")
                       .Must(BeAValidGuid).WithMessage("El ID de la reservacion debe ser un GUID válido.");
            }
            private bool BeAValidGuid(Guid id)
            {
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic: IRequestHandler<GetSpecificReservationInformation, ModelDto_Reservation_Information>
        {
            private readonly DbContextProyect _DbContextProyectInject;
           
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject)
            {
                this._DbContextProyectInject = DbContextProyectInject;
            }

            public async Task<ModelDto_Reservation_Information> Handle(GetSpecificReservationInformation request, CancellationToken cancellationToken)
            {
               
                cancellationToken.ThrowIfCancellationRequested();

                var obtainReservation = await this._DbContextProyectInject._TableReservations
                 .AsNoTracking()
                 .Where(reservation => reservation.ReservationId == request.reservationId)
                 .Select(reservation => new ModelDto_Reservation_Information
                 {
                     reservationId = reservation.ReservationId,
                     nameClient = reservation.Customer,

                     hotel_Information = new ModelDto_Hotel_Information
                     {
                        Name = reservation.HotelReservated.HotelName,
                        country = reservation.HotelReservated.Country,
                        city = reservation.HotelReservated.City
                     },

                     room_Information = new ModelDto_Room_Information
                     {
                        roomNumber = reservation.RoomReservated.RoomNumber.Value,
                        size = reservation.RoomReservated.RoomSize.Value
                     },

                     daysOfReservation = reservation.ListToDateReservatedInHotel
                        .Where(date => date.ReservationId == reservation.ReservationId)
                        .OrderByDescending(date => date.ReservationDate)
                        .Select(date => new ModelDto_InfoDaysReservated
                        {
                            DayReservate = date.ReservationDate.ToString("dd/MM/yyyy")
                        })
                        .ToList()
                 })
                 .FirstOrDefaultAsync(cancellationToken);
                if (obtainReservation == null)
                {
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound, new { MessageInformation = "No se encontro la reservacion a buscar!!, Porfavor intentelo mas tarde.." });
                }

                
                cancellationToken.ThrowIfCancellationRequested();
                return obtainReservation;
                     
            }
        }
    }
}
