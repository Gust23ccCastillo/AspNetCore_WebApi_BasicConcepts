using System.Net;

namespace BookingApplication.Services.MiddlewareGlobal
{
    public class ExecuteMiddlewareGlobalOfProyect:Exception
    {
        public HttpStatusCode _HttpStatusCode { get; }
        public object _ErrorsRequest { get; }
        public ExecuteMiddlewareGlobalOfProyect(HttpStatusCode httpStatusCodeParameterException, Object ErrorsRequestParameter = null)
        {
            this._ErrorsRequest = ErrorsRequestParameter;
            this._HttpStatusCode = httpStatusCodeParameterException;
        }
    }
}
