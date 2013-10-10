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
using System.Web.Http.ValueProviders;

namespace MapMyJourney.WebApi.Controllers
{
    public class PlacesController : BaseApiController
    {
        [ActionName("all")]
        public IEnumerable<VisitedPlaceModel> GetAllForJourney(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string authToken, int journeyId)
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

                    var places = from allPlaces in context.VisitedPlaces
                                 where allPlaces.Journey.Id == journeyId
                                 select new VisitedPlaceModel
                                 {
                                     Id = allPlaces.Id,
                                     Name = allPlaces.Name,
                                     Latitude = allPlaces.Latitude,
                                     Longitude = allPlaces.Longitude,
                                     Picture = allPlaces.Picture,
                                     Comment = allPlaces.Comment
                                 };

                    return places;
                });

            return responseMsg;
        }

        [ActionName("new")]
        public HttpResponseMessage PostNewPlace([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string authToken,
            int journeyId, [FromBody]VisitedPlaceModel placeModel)
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

                    var journey = context.Journeys.Where(x => x.Id == journeyId).FirstOrDefault();
                    var newPlace = new VisitedPlace
                    {
                        Name = placeModel.Name,
                        Latitude = placeModel.Latitude,
                        Longitude = placeModel.Longitude,
                        Picture = placeModel.Picture,
                        Comment = placeModel.Comment
                    };

                    context.VisitedPlaces.Add(newPlace);
                    context.SaveChanges();
                    journey.VisitedPlaces.Add(newPlace);
                    context.SaveChanges();

                    var response = this.Request.CreateResponse(HttpStatusCode.Created);
                    return response;
                });

            return responseMsg;
        }
    }
}
