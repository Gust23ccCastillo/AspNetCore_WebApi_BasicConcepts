using AutoMapper;
using BookingApplication.Dal;
using BookingApplication.Domain.Models;
using BookingApplication.Services.MiddlewareGlobal;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace BookingApplication.Services.Commands.CommandReservations
{
    public class CommandCreateNewReservationForHotel
    {
        
        public class CreateNewReservationInformation : IRequest<string>
        {
            
            public Guid hotelIdParameter { get; set; }
            public Guid roomIdParameter { get; set; }
            public DateTime checkInDateParameter { get; set; }
            public DateTime checkOutDateParameter { get; set; }
            public string NameForClient { get; set; }
        }

        public class FluentValidationData: AbstractValidator<CreateNewReservationInformation>
        {
            public FluentValidationData()
            {

                RuleFor(propertyEvaluated => propertyEvaluated.hotelIdParameter)
                   .NotEmpty().WithMessage("Debe Ingresar el 'Id' del hotel a reservar!!")
                   .Must(BeAValidGuid).WithMessage("El 'Id' del hotel proporcionado no es el dato esperado!!.");

                RuleFor(propertyEvaluated => propertyEvaluated.roomIdParameter)
                   .NotEmpty().WithMessage("Debe Ingresar el 'Id' de la habitacion a reservar!!")
                   .Must(BeAValidGuid).WithMessage("El 'Id' de la habitacion a reservar proporcionado no es el dato esperado!!.");

                RuleFor(propertyEvaluated => propertyEvaluated.checkInDateParameter)
                    .NotEmpty().WithMessage("Debe seleccionar una fecha de ingreso de la reservacion.");

                RuleFor(propertyEvaluated => propertyEvaluated.checkOutDateParameter)
                    .NotEmpty().WithMessage("Debe seleccionar una fecha de salida de la reservacion.");

                RuleFor(propertyEvaluated => propertyEvaluated.NameForClient)
                    .NotEmpty().WithMessage("Debe ingresar el nombre del cliente que realiza la reservacion.")
                    .MaximumLength(100).WithMessage("El nombre del cliente es demasido largo, Ingrese un nombre mas corto!!")
                    .MinimumLength(5).WithMessage("El nombre del cliente es demasido corto, Ingrese un nombre mas largo!!");
            }
            private bool BeAValidGuid(Guid id)
            {
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<CreateNewReservationInformation, string>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            private readonly IMapper _AutomapperInject;
            
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject,
                IMapper AutomapperInject)
            {
                this._DbContextProyectInject = DbContextProyectInject;
                this._AutomapperInject = AutomapperInject;
                
            }
            public async Task<string> Handle(CreateNewReservationInformation request, CancellationToken cancellationToken)
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                var linkedToken = linkedCts.Token;
                bool transactionCommitted = false;

                await using var transaction = await _DbContextProyectInject.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    linkedToken.ThrowIfCancellationRequested();
                    //STEP 1: TRAER EL HOTEL A RESERVAR
                    var hotelExists = await _DbContextProyectInject
                        ._TableHotels
                        .AnyAsync(searchHotelById => searchHotelById.HotelId == request.hotelIdParameter, cancellationToken);
                    if (hotelExists == false)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound, new
                        {
                            MessageInformation = "El hotel a realizar la 'Reservacion' no se encuentra 'Disponible'," +
                            "Porfavor intentelo nuevamente.."
                        });
                    }

                    linkedToken.ThrowIfCancellationRequested();
                    //STEP 2: TRAER LA HABITACION ESPECIFICA A RESERVAR
                    var roomExistsInformation = await this._DbContextProyectInject
                         ._TableRooms
                        .Where(searchRoomById => searchRoomById.RoomId == request.roomIdParameter)
                        .Select(selectInfoRoom => new Room
                        {
                            RoomId = selectInfoRoom.RoomId,
                            RoomNumber = selectInfoRoom.RoomNumber,
                            NeedRepair = selectInfoRoom.NeedRepair,
                            RoomSize = selectInfoRoom.RoomSize,
                            DateReservationForClient = selectInfoRoom.DateReservationForClient
                        })
                        .FirstOrDefaultAsync();
                    if (roomExistsInformation == null)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound, 
                            new { MessageInformation = "La habitacion del hotel a 'Reservar' no existe, Porfavor ingrese otra habitacion.." });
                    }

                    //STEP 4: VERIFICAR LAS FECHAS A RESERVAR
                    if (request.checkInDateParameter < request.checkOutDateParameter && request.checkOutDateParameter > request.checkInDateParameter)
                    {
                        //VERIFICAR SI LA HABITACION NECESITA REPARACION
                        if (roomExistsInformation.NeedRepair == true)
                        {
                            throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.BadRequest, 
                                new { MessageInformation = "La habitacion a 'Reservar' se encuentra en reparaciones, Porfavor seleccione otra habitacion.." });
                        }
                        //GENERERAMOS LA LISTA DE RESERVACION DE ACUERDO A LA FECHA DE ENTRADA Y SALIDA
                        List<DateTime> listDateToReservateForClient = GenerateTheListOfDatesToReserved(request.checkInDateParameter, request.checkOutDateParameter);

                        //Step 5: VERIFICAR SI EN LA TABLA DE ROOMS EN LA LISTA DE DIAS RESERVADOS CONCIDE ALGUNO
                        //DE LA LISTA NUEVA CREADA, Por qué HashSet?, Las búsquedas en un HashSet son más rápidas(O(1) en promedio)
                        //en comparación con listas(O(n)).
                        var dateSet = new HashSet<DateTime>(listDateToReservateForClient.Select(d => d.Date));
                        var conflictingDates = roomExistsInformation.DateReservationForClient
                                     .Where(reservationDate => dateSet.Contains(reservationDate.ReservationDate.Date))
                                     .Select(reservationDate => reservationDate.ReservationDate)
                                     .ToList();
                        if (!conflictingDates.Any())
                        {
                            linkedToken.ThrowIfCancellationRequested();

                            //STEP 6: COMPLETAR LA RESERVACION Y GUARDAR LOS DATOS, AQUI AGREGAMOS A LA LISTA DE DIAS DE RESERVACION PERO DE LA TABLA Rooms, RoomReservationDate
                            AgregateDayInTableRoomsAndRoomReservationDate(listDateToReservateForClient, this._DbContextProyectInject,
                                roomExistsInformation, request.hotelIdParameter);

                            //AQUI CREAMOS LA RESERVACION
                            var createReservation = CreateReservartion(request.hotelIdParameter, roomExistsInformation.RoomId, request.NameForClient);

                            linkedToken.ThrowIfCancellationRequested();

                            //AQUI AGREGAMOS A LA LISTA DE DIAS DE RESERVACION PERO DE LA TABLA RESERVACION, HotelReservationDate
                            AgregateDayInTableHotelReservationDate(listDateToReservateForClient, this._DbContextProyectInject,
                                createReservation);

                            this._DbContextProyectInject._TableReservations.Add(createReservation);
                            linkedToken.ThrowIfCancellationRequested();

                            var resultToCreateReservation = await this._DbContextProyectInject.SaveChangesAsync(cancellationToken);
                            if (resultToCreateReservation < 0)
                            {
                                throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.InternalServerError, 
                                    new { MessageInformation = "Error!, Problemas para crear la reservacion, Porfavor intentelo nuevamente.." });
                            }

                            linkedToken.ThrowIfCancellationRequested();
                            await transaction.CommitAsync(cancellationToken);
                            transactionCommitted = true;
                            return $"Se ha creado la reservacion con existo!!, Numero de reservacion con el 'ID': {createReservation.ReservationId}";
                        }
                        else
                        {
                            //AQUI ENVIAMOS UN MENSAJE AL CLIENTE EN CASO DE QUE ALGUN DIA YA ESTE RESERVADO SEGUN EL HOTEL Y SU HABITACION
                            var returnMessageInformation = string.Empty;
                            foreach (var item in conflictingDates)
                            {
                                returnMessageInformation = returnMessageInformation + " | " + item.ToString("dd/MM/yyyy");
                            }
                            throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.BadRequest, new
                            {
                                MessageInformation = $"Ya hay 'Reservaciones' en esas fechas!!, Las cuales son: {returnMessageInformation}. " +
                                $"Porfavor seleccione otras fechas de reservacion.."
                            });
                        }
                    }
                    else
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.BadRequest, 
                            new { MessageInformation = "Las fechas de la 'Reservacion' no son validas!!, Porfavor ingrese otras fechas de 'Reservacion'.." });
                    }
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
                        new { MessageInformation = "Error no se pudo agregar la informacion de la reservacion, Porfavor intentelo nuevamente." });
                }
                
            }

            //METODO PARA GENERAR LA LISTA DE FECHAS A RESERVAR DEL CLIENTE
            private List<DateTime> GenerateTheListOfDatesToReserved(DateTime reservationStartDate, DateTime reservationOutDate)
            {
                List<DateTime> listReservationDates = new List<DateTime>();
                for (DateTime Date = reservationStartDate; Date <= reservationOutDate; Date = Date.AddDays(1))
                {
                    listReservationDates.Add(Date);
                }
                return listReservationDates;
            }

            //METODO PARA AGREGAR LOS DIAS DE RESERVACION EN LA TABLA ROOMS Y ROOMRESERVATIONDATE
           private void AgregateDayInTableRoomsAndRoomReservationDate(List<DateTime> listDateToReservateForClient, 
               DbContextProyect dbContextProyect, Room roomExistsInformation,Guid hotelIdParameter)
            {
                foreach (var addListOfDaysReservationToRoomingList in listDateToReservateForClient)
                {
                    var addDayToList = new RoomReservationDate
                    {
                        ReservationDate = addListOfDaysReservationToRoomingList,
                        RoomId = roomExistsInformation.RoomId
                    };
                    roomExistsInformation.DateReservationForClient.Add(addDayToList);
                    this._DbContextProyectInject._TableRoomReservationDates.Add(addDayToList);
                }

                roomExistsInformation.HotelId = hotelIdParameter;
                this._DbContextProyectInject._TableRooms.Update(roomExistsInformation);
            }

            //METODO PARA AGREGAR LOS DIAS DE RESERVACION EN LA TABLA HOTELRESERVATIONDATE

            private void AgregateDayInTableHotelReservationDate(List<DateTime> listDateToReservateForClient,
               DbContextProyect dbContextProyect,Reservation newReservation)
            {
                foreach (var addDaysInReservation in listDateToReservateForClient)
                {
                    var agregatedDaysInList = new HotelReservationDate
                    {
                        ReservationId = newReservation.ReservationId,
                        ReservationDate = addDaysInReservation
                    };
                    newReservation.ListToDateReservatedInHotel.Add(agregatedDaysInList);
                    this._DbContextProyectInject._TableHotelReservationDates.Add(agregatedDaysInList);
                }
            }

            //METODO PARA CREAR EL MODELO DE RESERVACION
            private Reservation CreateReservartion(Guid hotelIdParameter,Guid roomId, string NameForCliente)
            {
                var createReservation = new Reservation()
                {
                    ReservationId = Guid.NewGuid(),
                    HotelId = hotelIdParameter,
                    RoomId = roomId,
                    Customer = NameForCliente
                };

                return createReservation;
            }
        }
    }
}
