using AutoMapper;
using BookingApplication.Dal;
using BookingApplication.Services.MiddlewareGlobal;
using BookingApplication.Services.Querys.RoomHotelQuery.QueryRoomHotelDto;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Querys.RoomHotelQuery
{
    public class QueryGetSpecificRoomForSpecificHotel
    {
        public class GetSpecificRoomForSpecificHotelInformation : IRequest<ModelDtoSpecificInfoRoom>
        {
           
            public Guid hotelIdParameter { get; set; }
            public Guid roomIdParameter { get; set; }
        }
        public class FluentValidationData : AbstractValidator<GetSpecificRoomForSpecificHotelInformation>
        {
            public FluentValidationData()
            {
                RuleFor(x => x.hotelIdParameter)
                       .NotEmpty().WithMessage("Debe ingresar un ID de Hotel para buscar la habitación específica.")
                       .Must(BeAValidGuid).WithMessage("El ID de Hotel debe ser un GUID válido.");

                RuleFor(x => x.roomIdParameter)
                    .NotEmpty().WithMessage("Debe ingresar el ID de la habitación.")
                    .Must(BeAValidGuid).WithMessage("El ID de la habitación debe ser un GUID válido.");
            }
            private bool BeAValidGuid(Guid id)
            {
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<GetSpecificRoomForSpecificHotelInformation, ModelDtoSpecificInfoRoom>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject)
            {
               this._DbContextProyectInject = DbContextProyectInject;
            }
            public async Task<ModelDtoSpecificInfoRoom> Handle(GetSpecificRoomForSpecificHotelInformation request, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var hotelExists = await _DbContextProyectInject._TableHotels
                   .AnyAsync(searhHotelById => searhHotelById.HotelId == request.hotelIdParameter, cancellationToken);

                if (hotelExists == false)
                {
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound,
                        new { MessageInformation = "El hotel especificado no existe en el sistema." });
                }
              
                cancellationToken.ThrowIfCancellationRequested();

                var getRoomInformationOfHotel = await this._DbContextProyectInject._TableRooms
                 .AsNoTracking()
                 .Where(conditions => conditions.HotelId == request.hotelIdParameter && conditions.RoomId == request.roomIdParameter)
                 .Select(room => new ModelDtoSpecificInfoRoom
                 {
                     roomNumber = room.RoomNumber ?? 0,
                     size = room.RoomSize ?? 0,
                     needsRepair = room.NeedRepair ?? false,
                     dateReservations = room.DateReservationForClient
                         .OrderByDescending(date => date.ReservationDate)
                         .Select(reservationDate => reservationDate.ReservationDate.ToString("yyyy-MM-dd"))// Formato de fecha
                         .ToList()
                 })
                 .FirstOrDefaultAsync(cancellationToken);
                if (getRoomInformationOfHotel == null)
                {
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound, new { MessageInformation = "La Habitacion a buscar del Hotel no existe en el sistema, Porfavor ingrese otro valor de habitacion" });
                }

                cancellationToken.ThrowIfCancellationRequested();
                return getRoomInformationOfHotel;
            }
        }

    }
}
