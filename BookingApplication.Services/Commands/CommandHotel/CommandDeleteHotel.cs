using BookingApplication.Dal;
using BookingApplication.Services.MiddlewareGlobal;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Commands.CommandHotel
{
    public class CommandDeleteHotel
    {
        public class DeleteSpecificHotelInformation : IRequest<Unit>
        {
            public Guid idHotelParameter { get; set; }
        }

        public class FluentValidationData : AbstractValidator<DeleteSpecificHotelInformation>
        {
            public FluentValidationData()
            {
                RuleFor(xHotelIdParameter => xHotelIdParameter.idHotelParameter)
                    .NotEmpty().WithMessage("Debe Ingresar el 'Id' del Hotel!!")
                    .Must(BeAValidGuid).WithMessage("El 'Id' de Hotel proporcionado no es el dato esperado!!.");
            }
            private bool BeAValidGuid(Guid id)
            {
                // Si el tipo es Guid, esta validación se garantiza.
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<DeleteSpecificHotelInformation, Unit>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject)
            {
                _DbContextProyectInject = DbContextProyectInject;
            }
            public async Task<Unit> Handle(DeleteSpecificHotelInformation request, CancellationToken cancellationToken)
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                var linkedToken = linkedCts.Token;
                bool transactionCommitted = false;


                await using var transaction = await _DbContextProyectInject.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    linkedToken.ThrowIfCancellationRequested();

                    var existingHotelInSystem = await _DbContextProyectInject._TableHotels
                           .AsNoTrackingWithIdentityResolution() // Mejor rendimiento para relaciones complejas
                           .Where(conditions => conditions.HotelId == request.idHotelParameter)
                           .Include(includeRoomsInfo => includeRoomsInfo.ListOfRooms)
                                     .ThenInclude(listDateReservate => listDateReservate.DateReservationForClient) 
                   .FirstOrDefaultAsync(cancellationToken);

                    if (existingHotelInSystem == null)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound, 
                            new { MessageInformation = "El Hotel a 'Eliminar su Informacion', No existe en el sistema, Porfavor intentelo mas tarde.." });
                    }

                    if (existingHotelInSystem.ListOfRooms.Count > 0)
                    {
                        if (existingHotelInSystem.ListOfRooms.Any(verifyBoolean => verifyBoolean.DateReservationForClient.Count > 0))
                        {
                            throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.Conflict, 
                                new { MessageInformation = "Conflicto!!, No se puede eliminar el hotel debido a que todavia existen fechas de reservacion.." });
                        }
                        else
                        {
                            // Eliminar las habitaciones asociadas
                            this._DbContextProyectInject._TableRooms.RemoveRange(existingHotelInSystem.ListOfRooms);
                        }
                    }
                    // Eliminar el hotel
                    this._DbContextProyectInject._TableHotels.Remove(existingHotelInSystem);

                    linkedToken.ThrowIfCancellationRequested();
                    var resultToOperation = await _DbContextProyectInject.SaveChangesAsync(cancellationToken);

                    if (resultToOperation <= 0)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.InternalServerError, 
                            new { MessageInformation = "Error!!, Problemas con el servidor encargado de 'Eliminar la Informacion' del Hotel, Porfavor intentelo mas tarde.." });
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
                        MessageInformation = "¡Tiempo de espera agotado! La eliminacion del hotel tomó más de 5 minutos y fue cancelada."
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
                        new { MessageInformation = "Error al eliminar la información del hotel, Inténtalo nuevamente." });
                }
            }
        }
    }
}
