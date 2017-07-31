using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace PurinaQnA.Controllers
{

    public class ContactUsController : ApiController
    {
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] string userName)
        {
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}