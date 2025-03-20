using BookingApplication.Services.Querys.HotelQuery.QueryHotelDtos;
using BookingApplication.Services.Querys.HotelQuery;
using BookingApplication.WebApi.Controllers.V1;
using MediatR;
using Moq;
using System.Net;
using BookingApplication.Services.MiddlewareGlobal;

namespace BookingApplicationTesting.TestingForControllers
{
    public class TestingHotelActionController
    {  
      
        //[Fact]
        //public async Task GetAllHotels_InApplication_For_SuccessInformation()
        //{

        //    // Arrange

        //    var mediatorMock = new Mock<IMediator>();

        //    var controller = new HotelActionsController(mediatorMock.Object);
        
         

        //    mediatorMock.Setup(m => m.Send(It.IsAny<QueryGetHotels.GetListHotelInformation>(), default))
        //                .ReturnsAsync(new List<ModelDto_Hotel_List_Information>());

        //    // Act
        //    var _ResultToGetHotels = await controller.GetAllHotelsInApplication();

        //    // Assert
           
        //     Assert.IsType<List<ModelDto_Hotel_List_Information>>(_ResultToGetHotels.Value);
        //     Assert.NotNull(_ResultToGetHotels.Value);
            

        //}

        //[Theory]
        //[InlineData("37bd73a4-a7fb-4e0c-9110-21b67eeed35c", "ab2bd817-98cd-4cf3-a80a-53ea0cd9c111")]
        //public async Task GetSpecificHotel_ById_InApplication_VerifySuccess_Information(string GuidOneParameter, string GuidTwoParameter)
        //{
            
        //    //Arrange
        //    var _ValidGuidParameter = new Guid(GuidOneParameter);
        //    var _InvalidGuidParameter = new Guid(GuidTwoParameter);

        //    var mediatorMock = new Mock<IMediator>();
        //    var _Controller_For_Hotel = new HotelActionsController(mediatorMock.Object);

              
        //      mediatorMock.Setup(m => m.Send(It.IsAny<QueryGetSpecificHotel.GetSpecificHotelInformation>(), default))
        //              .ReturnsAsync(new ModelDto_Specific_Hotel_Information());


        //    //Act
        //    var _NotFoundResult_For_GetSpecificHotel_ById = await _Controller_For_Hotel
        //        .GetSpecificHotelByIdInApplication(new QueryGetSpecificHotel.GetSpecificHotelInformation { HotelIdParameter = _InvalidGuidParameter});

        //    var OkResultt_For_GetSpecificHotel_ById = await _Controller_For_Hotel
        //        .GetSpecificHotelByIdInApplication(new QueryGetSpecificHotel.GetSpecificHotelInformation { HotelIdParameter = _ValidGuidParameter });
        //    //Assert

        //    ////VERIFY STATUS CODE FOR SEND GUID PARAMETER INVALID
        //    var _ResultNotFount = Assert.IsType<ExecuteMiddlewareGlobalOfProyect>(_NotFoundResult_For_GetSpecificHotel_ById);
        //    Assert.Equal(HttpStatusCode.NotFound, _ResultNotFount._HttpStatusCode);

        //    ////VERIFY DATA RETUNR FOR METHOD AND VARIFY DATA IS CORRECTLY
        //    Assert.IsType<ModelDto_Specific_Hotel_Information>(OkResultt_For_GetSpecificHotel_ById.Value);
        //    Assert.NotNull(OkResultt_For_GetSpecificHotel_ById.Value);


        //    ////Now, let us check the value itself.
        //    //Assert.Equal("The Maybourne Riviera", OkResultt_For_GetSpecificHotel_ById.Value.HotelName);
        //    //Assert.Equal("France", _HotelSpecific.Country);



        //}

    }
}

