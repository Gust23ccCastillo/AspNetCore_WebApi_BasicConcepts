using AutoMapper;
using BookingApplication.Dal;
using BookingApplication.Domain.Models;
using BookingApplication.Services.Commands.CommandReservations.CommandReservationDto;
using BookingApplication.Services.MiddlewareGlobal;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Commands.CommandReservations
{
    public class CommandDeleteSpecificReservationForHotel
    {
        public class DeleteSpecificReservationInformation : IRequest<string>
        {
           
            public Guid reservationId { get; set; }
        }

        public class FluentValidationData : AbstractValidator<DeleteSpecificReservationInformation>
        {
            public FluentValidationData()
            {
               
                RuleFor(xHotelIdParameter => xHotelIdParameter.reservationId).NotEmpty().WithMessage("Debe Ingresar el 'Id' de la reservacion!!")
                   .Must(BeAValidGuid).WithMessage("El 'Id' de la reservacion proporcionado no es el dato esperado!!.");
            }
            private bool BeAValidGuid(Guid id)
            {
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<DeleteSpecificReservationInformation, string>
        {
            private readonly DbContextProyect _DbContextProyectInject;
           
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject)
            {
                this._DbContextProyectInject = DbContextProyectInject; 
            }

          
            public async Task<string> Handle(DeleteSpecificReservationInformation request, CancellationToken cancellationToken)
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(15));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                var linkedToken = linkedCts.Token;
                bool transactionCommitted = false;

                await using var transaction = await _DbContextProyectInject.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    linkedToken.ThrowIfCancellationRequested();

                    var reservationInfo = await GetReservationInfoAsync(request.reservationId, cancellationToken);
                    if (reservationInfo == null)
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound,
                            new { MessageInformation = "No se pudo encontrar la información relacionada a la reservación. ¡Inténtalo nuevamente!" });

                    linkedToken.ThrowIfCancellationRequested();
                    var filterDates = FilterDuplicateReservationDates(
                        reservationInfo.roomReservated.infoDaysReservate,
                        reservationInfo.daysReservated,
                        reservationInfo.roomReservated,
                        reservationInfo.reservationId,
                        reservationInfo.hotelReservated.hotelId,
                        this._DbContextProyectInject                 
                    );

                    linkedToken.ThrowIfCancellationRequested();
                    ApplyReservationUpdates(reservationInfo, filterDates);

                    linkedToken.ThrowIfCancellationRequested();
                    await _DbContextProyectInject.SaveChangesAsync(cancellationToken);

                    linkedToken.ThrowIfCancellationRequested();
                    await transaction.CommitAsync(cancellationToken);
                    transactionCommitted = true;

                    return $"Se eliminó con éxito la reservación a nombre de: {reservationInfo.customerName}.";
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.RequestTimeout, new
                    {
                        MessageInformation = "¡Tiempo de espera agotado! La creacion de la reservacion tomó más de 5 minutos y fue cancelada."
                    });
                }
                catch (ExecuteMiddlewareGlobalOfProyect)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
                catch (Exception)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.Conflict,
                        new { MessageInformation = "Error al eliminar la información. Inténtalo nuevamente." });
                }
            }

            private async Task<ModelReservationDto_Delete?> GetReservationInfoAsync(Guid reservationId, 
                CancellationToken cancellationToken)
            {
                return await _DbContextProyectInject._TableReservations
                    .AsNoTracking()
                    .Where(conditions => conditions.ReservationId == reservationId)
                    .Include(hotelInfo => hotelInfo.HotelReservated)
                    .Include(roomInfo => roomInfo.RoomReservated)
                        .ThenInclude(daysReservateInRoom => daysReservateInRoom.DateReservationForClient)
                    .Select(selectInfo => new ModelReservationDto_Delete
                    {
                        reservationId = selectInfo.ReservationId,
                        customerName = selectInfo.Customer,
                        hotelReservated = new ModelReservationHotelDto_Delete
                        {
                            hotelId = selectInfo.HotelReservated.HotelId
                        },
                        roomReservated = new ModelReservsationRoomDto_Delete
                        {
                            idRoom = selectInfo.RoomReservated.RoomId,
                            roomNumber = selectInfo.RoomReservated.RoomNumber.Value,
                            size = selectInfo.RoomReservated.RoomSize.Value,
                            needsRepair = selectInfo.RoomReservated.NeedRepair.Value,
                            infoDaysReservate = selectInfo.RoomReservated.DateReservationForClient
                                .Where(conditions => conditions.RoomId == selectInfo.RoomId)
                                .Select(select => new ModelDaysReservationsRoomInfo_Dto
                                {
                                    Id = select.RoomReservationDateId,
                                    DayReservated = select.ReservationDate
                                })
                                .ToList()
                        },
                        daysReservated = selectInfo.ListToDateReservatedInHotel
                            .Where(conditions => conditions.ReservationId == selectInfo.ReservationId)
                            .Select(propertySelect => new ModelReservationListDaysReservatedDto_Delete
                            {
                                Id = propertySelect.HotelReservationDateId,
                                DayReservate = propertySelect.ReservationDate
                            }).ToList()
                    })
                    .FirstOrDefaultAsync(cancellationToken);
            }

            //METODO QUE REALIZA ELIMINAR DIA IGUALES EN LA TABALAS DE ROOMRESERVATIONDATE Y HOTELRESERVATIONDATE
            private static FilterDuplicateDateModelDto FilterDuplicateReservationDates(
                  List<ModelDaysReservationsRoomInfo_Dto> roomReservationDates,
                  List<ModelReservationListDaysReservatedDto_Delete> hotelReservationDates,
                  ModelReservsationRoomDto_Delete roomInformation,
                  Guid reservationId,
                  Guid hotelId,
                  DbContextProyect dbContextProyect)
            {
                var reservedDatesSet = new HashSet<DateTime>(hotelReservationDates.Select(selectProperty => 
                selectProperty.DayReservate));

                var filteredRoomReservationDates = roomReservationDates
                    .Where(date => !reservedDatesSet.Contains(date.DayReservated))
                    .Select(date => new RoomReservationDate
                    {
                        RoomReservationDateId = date.Id,
                        ReservationDate = date.DayReservated,
                        RoomId = roomInformation.idRoom
                    })
                    .ToList();

                var hotelReservationDatesToDelete = hotelReservationDates
                    .Select(date => new HotelReservationDate
                    {
                        HotelReservationDateId = date.Id,
                        ReservationDate = date.DayReservate,
                        ReservationId = reservationId
                    })
                    .ToList();

                dbContextProyect._TableRoomReservationDates.UpdateRange(filteredRoomReservationDates);
                dbContextProyect._TableHotelReservationDates.RemoveRange(hotelReservationDatesToDelete);

                return new FilterDuplicateDateModelDto
                {
                    ListRoomReservationDates = filteredRoomReservationDates,
                    ListHotelReservationDates = hotelReservationDatesToDelete
                };
            }

            //METODO QUE ACTUALIZA LA LISTA DE DIAS RESERVADIOS EN ROOM Y ELIMINAR RESERVACION
            private void ApplyReservationUpdates(ModelReservationDto_Delete reservationInfo, 
                FilterDuplicateDateModelDto filterDates)
            {
                // Recuperamos la entidad Room de la base de datos para asegurarnos de que está siendo rastreada por EF
                var updateRoomInformation = this._DbContextProyectInject._TableRooms
                    .Include(includeDaysReservated => includeDaysReservated.DateReservationForClient)
                    .FirstOrDefault(r => r.RoomId == reservationInfo.roomReservated.idRoom);

                if (updateRoomInformation != null)
                {
                    // Filtramos las fechas de reserva a eliminar
                    var resultOfEliminationReservationDays = updateRoomInformation.DateReservationForClient
                        .Where(d => filterDates.ListRoomReservationDates.Any(fd => fd.ReservationDate.Date == d.ReservationDate.Date))
                        .ToList(); // Convertimos a lista para evitar modificar la colección mientras iteramos

                    updateRoomInformation.DateReservationForClient = resultOfEliminationReservationDays;
                    this._DbContextProyectInject._TableRooms.Update(updateRoomInformation);
                }

                var reservationToRemove = new Reservation
                {
                    ReservationId = reservationInfo.reservationId,
                    Customer = reservationInfo.customerName,
                    RoomId = reservationInfo.roomReservated.idRoom,
                    HotelId = reservationInfo.hotelReservated.hotelId,
                    ListToDateReservatedInHotel = filterDates.ListHotelReservationDates
                };

                _DbContextProyectInject._TableReservations.Remove(reservationToRemove);
            }
        }
    }
}
