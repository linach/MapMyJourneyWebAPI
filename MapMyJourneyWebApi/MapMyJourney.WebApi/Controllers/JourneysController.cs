using MapMyJourney.Data;
using MapMyJourney.Models;
using MapMyJourney.WebApi.Attributes;
using MapMyJourney.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ValueProviders;

namespace MapMyJourney.WebApi.Controllers
{
    [EnableCors(origins: "http://mapmyjourneywebapi.apphb.com/", headers: "*", methods: "*")]
    public class JourneysController : BaseApiController
    {
        [ActionName("all")]
        public IEnumerable<JourneyModel> GetAllForUser(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string authToken)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = new JourneysContext();
                    var journeys = (from allJourneys in context.Journeys
                                   where allJourneys.User.AuthToken == authToken
                                   select new JourneyModel
                                   {
                                       Id = allJourneys.Id,
                                       Name = allJourneys.Name,
                                       StartDate = allJourneys.StartDate,
                                       EndDate = allJourneys.EndDate
                                   }).OrderByDescending(x=>x.StartDate);

                    return journeys;
                });

            return responseMsg;
        }

        [ActionName("new")]
        public HttpResponseMessage PostNewJourney([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string authToken,
            [FromBody]JourneyModel journeyModel)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = new JourneysContext();
                    var user = context.Users.Where(x => x.AuthToken == authToken).FirstOrDefault();
                    if (user == null)
                    {
                        throw new ArgumentNullException("User is not logged in or does not exist!");
                    }

                    context.Journeys.Add(new Journey
                    {
                        Name = journeyModel.Name,
                        StartDate = journeyModel.StartDate,
                        EndDate = journeyModel.EndDate,
                        User = user
                    });

                    context.SaveChanges();
                    var response = this.Request.CreateResponse(HttpStatusCode.Created);
                    return response;
                });

            return responseMsg;
        }

        [ActionName("finish")]
        public HttpResponseMessage PutFinishJourney([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string authToken,
            [FromBody]JourneyModel journeyModel)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = new JourneysContext();
                    var user = context.Users.Where(x => x.AuthToken == authToken).FirstOrDefault();

                    if (user == null)
                    {
                        throw new ArgumentNullException("User is not logged in or does not exist!");
                    }

                    var journeyToFinish = context.Journeys.Where(x => x.Id == journeyModel.Id)
                        .FirstOrDefault();
                    journeyToFinish.EndDate = journeyModel.EndDate;

                    context.SaveChanges();
                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                });

            return responseMsg;
        }
    }
}
