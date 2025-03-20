using BookingApplication.Dal;
using BookingApplication.Services.MiddlewareGlobal;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Commands.CommandRooms
{
    public class CommandDeleteRoomForSpecificHotel
    {
        public class DeleteRoomForSpecificHotelInformation : IRequest<Unit>
        {
            public Guid hotelIdParameter { get; set; }
            public Guid roomIdParameter { get; set; }
        }

        public class FluentValidationData: AbstractValidator<DeleteRoomForSpecificHotelInformation>
        {
            public FluentValidationData()
            {
                RuleFor(xHotelIdParameter => xHotelIdParameter.hotelIdParameter)
                    .NotEmpty().WithMessage("Debe Ingresar el 'Id' del Hotel!!")
                    .Must(BeAValidGuid).WithMessage("El 'Id' de Hotel proporcionado no es el dato esperado!!.");

                RuleFor(xHotelIdParameter => xHotelIdParameter.roomIdParameter)
                    .NotEmpty().WithMessage("Debe Ingresar el 'Id' de la habitacion!!")
                    .Must(BeAValidGuid).WithMessage("El 'Id' de la habitacion proporcionado no es el dato esperado!!.");
            }
            private bool BeAValidGuid(Guid id)
            {
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<DeleteRoomForSpecificHotelInformation, Unit>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject)
            {
                this._DbContextProyectInject = DbContextProyectInject;      
            }
            public async Task<Unit> Handle(DeleteRoomForSpecificHotelInformation request, CancellationToken cancellationToken)
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
                        .Include(includeDayReservarted => includeDayReservarted.DateReservationForClient)
                        .FirstOrDefaultAsync(searhRoom => searhRoom.RoomId == request.roomIdParameter &&
                                             searhRoom.HotelId == request.hotelIdParameter, cancellationToken);

                    if (existingRoom == null)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound,
                            new { MessageInformation = "La habitación especificada no existe en el hotel." });
                    }

                    if (existingRoom.DateReservationForClient.Count > 0)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.Conflict, new
                        {
                            MessageInformation = "Error de Conflicto!!, No se puede eliminar la habitacion debido a que contiene fechas reservadas.."
                        });
                    }

                    this._DbContextProyectInject._TableRooms.Remove(existingRoom);

                    linkedToken.ThrowIfCancellationRequested();
                    var resultToOperation = await _DbContextProyectInject.SaveChangesAsync(cancellationToken);

                    if (resultToOperation <= 0)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.InternalServerError,
                       new { MessageInformation = "Hubo un problema al eliminar la habitación. Por favor, inténtelo más tarde." });
                    }

                    linkedToken.ThrowIfCancellationRequested();
                    await transaction.CommitAsync(cancellationToken);
                    transactionCommitted = true;
                    return Unit.Value;
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.RequestTimeout, new
                    {
                        MessageInformation = "¡Tiempo de espera agotado! La eliminacion de la habitacion tomó más de 5 minutos y fue cancelada."
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
                        new { MessageInformation = "Error no se pudo eliminar la informacion de la habitacion, Porfavor intentelo nuevamente." });
                }
            }
        }
    }
}
