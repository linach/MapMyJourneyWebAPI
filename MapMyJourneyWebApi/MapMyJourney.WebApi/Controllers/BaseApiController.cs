using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MapMyJourney.WebApi.Controllers
{
    public class BaseApiController : ApiController
    {
        protected T PerformOperationAndHandleExceptions<T>(Func<T> operation)
        {
            try
            {
                return operation();
            }
            catch (ArgumentNullException ane)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ane.Message);
                throw new HttpResponseException(errResponse);
            }
            catch (ArgumentOutOfRangeException aor)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, aor.Message);
                throw new HttpResponseException(errResponse);
            }
            catch (ArgumentException ae)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ae.Message);
                throw new HttpResponseException(errResponse);
            }
            catch (InvalidOperationException io)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, io.Message);
                throw new HttpResponseException(errResponse);
            }
            catch (Exception ex)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                throw new HttpResponseException(errResponse);
            }
        }
    }
}
