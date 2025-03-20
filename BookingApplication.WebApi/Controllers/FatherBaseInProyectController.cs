using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingApplication.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FatherBaseInProyectController : Controller
    {
        private IMediator _MediatorInjectInProyect;
        protected IMediator MediatorInjectInProyect => _MediatorInjectInProyect ??
                  (_MediatorInjectInProyect = HttpContext.RequestServices.GetRequiredService<IMediator>());

    }
}
