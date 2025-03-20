
using AutoMapper;
using BookingApplication.Dal;
using BookingApplication.Domain.Models;
using BookingApplication.Services.Commands.CommandRooms.CommandModelDto;
using BookingApplication.Services.MiddlewareGlobal;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Commands.CommandRooms
{
    public class CommandUpdatedRoomForSpecificHotel
    {
        public class UpdatedRoomForSpecficHotelInformation : IRequest<ModelDtoReturnUpdateRooms>
        {
            public Guid hotelIdParameter { get; set; }
            public Guid roomIdParameter { get; set; }
            public int? roomNumberParameter { get; set; }
            public double? sizeRoomParameter { get; set; }
            public bool? needsRepairParameter { get; set; } = false;
        }

        public class FluentValidationData : AbstractValidator<UpdatedRoomForSpecficHotelInformation>
        {
            public FluentValidationData()
            {
                RuleFor(xHotelIdParameter => xHotelIdParameter.hotelIdParameter)
                    .NotEmpty().WithMessage("Debe Ingresar el 'Id' del Hotel!!")
                    .Must(BeAValidGuid).WithMessage("El 'Id' de Hotel proporcionado no es el dato esperado!!.");

                RuleFor(xHotelIdParameter => xHotelIdParameter.roomIdParameter)
                    .NotEmpty().WithMessage("Debe Ingresar el 'Id' de la habitacion!!")
                   .Must(BeAValidGuid).WithMessage("El 'Id' de la habitacion proporcionado no es el dato esperado!!.");

                RuleFor(xroomNumberParameter => xroomNumberParameter.roomNumberParameter)
                    .NotNull().WithMessage("Debe ingresar un numero de habitacion a crear!!")
                     .InclusiveBetween(1, 150).WithMessage("El número de habitacion a crear debe ser entre (1) como Mínimo a (150) como Máximo.");

                RuleFor(x => x.sizeRoomParameter)
                     .NotNull().WithMessage("Debe ingresar el tamaño de la habitación en metros cuadrados.")
                     .GreaterThan(0).WithMessage("El tamaño de la habitación debe ser mayor a 0.");

                RuleFor(x => x.needsRepairParameter)
                      .NotNull().WithMessage("Debe especificar si la reparación es necesaria.")
                      .Must(value => value == true || value == false)
                      .WithMessage("El valor debe ser verdadero o falso.");
            }
            private bool BeAValidGuid(Guid id)
            {
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<UpdatedRoomForSpecficHotelInformation, ModelDtoReturnUpdateRooms>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            private readonly IMapper _AutomapperInject;
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject, IMapper AutomapperInject)
            {
                this._DbContextProyectInject = DbContextProyectInject;
                this._AutomapperInject = AutomapperInject;
            }

            public async Task<ModelDtoReturnUpdateRooms> Handle(UpdatedRoomForSpecficHotelInformation request, CancellationToken cancellationToken)
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                var linkedToken = linkedCts.Token;
                bool transactionCommitted = false;

                await using var transaction = await _DbContextProyectInject.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    linkedToken.ThrowIfCancellationRequested();

                    var hotelExists = await _DbContextProyectInject._TableHotels
                             .AnyAsync(conditions => conditions.HotelId == request.hotelIdParameter, cancellationToken);

                    if (hotelExists == false)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound,
                            new { MessageInformation = "El hotel especificado no existe en el sistema." });
                    }
                   
                    linkedToken.ThrowIfCancellationRequested();
                    var existingRoom = await _DbContextProyectInject
                                          ._TableRooms
                                          .AsNoTracking()
                                          .Include(includeInfo => includeInfo.Hotel)
                                          .Where(conditions =>
                                             conditions.RoomId == request.roomIdParameter)
                                          .Select(selectPropertyRoom => new Room 
                                            {        RoomNumber = selectPropertyRoom.RoomNumber,
                                                    NeedRepair = selectPropertyRoom.NeedRepair,
                                                    RoomSize  = selectPropertyRoom.RoomSize
                                             }).FirstOrDefaultAsync(cancellationToken);
                    if (existingRoom == null)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound,
                            new { MessageInformation = "La habitación especificada no existe en el hotel." });
                    }
                    ApplyUpdateRoomInfo(existingRoom, request);
                    existingRoom.RoomId = request.roomIdParameter;
                    existingRoom.HotelId = request.hotelIdParameter;
                    _DbContextProyectInject._TableRooms.Update(existingRoom);

                    linkedToken.ThrowIfCancellationRequested();
                    var resultToOperation = await _DbContextProyectInject.SaveChangesAsync(cancellationToken);

                    if (resultToOperation <= 0)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.InternalServerError,
                        new { MessageInformation = "Hubo un problema al actualizar la información de la habitación. Por favor, inténtelo más tarde." });
                    }

                    linkedToken.ThrowIfCancellationRequested();
                    await transaction.CommitAsync(cancellationToken);
                    transactionCommitted = true;
                    return _AutomapperInject.Map<ModelDtoReturnUpdateRooms>(existingRoom);
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.RequestTimeout, new
                    {
                        MessageInformation = "¡Tiempo de espera agotado! La actualizacion de la habitacion tomó más de 5 minutos y fue cancelada."
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
                        new { MessageInformation = "Error no se pudo actualizar la informacion de la nueva habitacion, Porfavor intentelo nuevamente." });
                }
            }

            // Actualización de la habitación
            private void ApplyUpdateRoomInfo(Room existingRoom, UpdatedRoomForSpecficHotelInformation newRoomInfoUpdate)
            {
                existingRoom.RoomNumber = newRoomInfoUpdate.roomNumberParameter ?? existingRoom.RoomNumber;
                existingRoom.RoomSize = newRoomInfoUpdate.sizeRoomParameter ?? existingRoom.RoomSize;
                existingRoom.NeedRepair = newRoomInfoUpdate.needsRepairParameter ?? existingRoom.NeedRepair;
            }
        }
    }
}
