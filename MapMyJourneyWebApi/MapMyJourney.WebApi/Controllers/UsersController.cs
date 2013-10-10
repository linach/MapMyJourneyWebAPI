using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MapMyJourney.Data;
using MapMyJourney.WebApi.Models;
using MapMyJourney.Models;
using System.Text;
using MapMyJourney.WebApi.Attributes;
using System.Web.Http.ValueProviders;

namespace MapMyJourney.WebApi.Controllers
{
    public class UsersController : BaseApiController
    {

        private const int MinUsernameAndDisplaynameLength = 6;
        private const int MaxUsernameAndDisplaynameLength = 30;
        private const string ValidUsernameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.";

        private const string ValidDisplaynameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_. -";

        private const string AuthTokenChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";

        private static readonly Random rand = new Random();

        private const int AuthTokenLength = 50;

        private const int AuthCodeLength = 40;

        public UsersController()
        {
        }

        [ActionName("register")]
        public HttpResponseMessage PostRegisterUser(UserModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = new JourneysContext();
                    using (context)
                    {
                        this.ValidateUsername(model.Username);
                        this.ValidateDisplayname(model.Displayname);
                        this.ValidateAuthCode(model.AuthCode);
                        
                        var usernameToLower = model.Username.ToLower();
                        var nicknameToLower = model.Displayname.ToLower();
                        var user = context.Users.FirstOrDefault(
                            usr => usr.Username == usernameToLower
                            || usr.DisplayName.ToLower() == nicknameToLower);

                        if (user != null)
                        {
                            throw new InvalidOperationException("Users exists");
                        }

                        user = new User()
                        {
                            Username = usernameToLower,
                            DisplayName = model.Displayname,
                            AuthCode = model.AuthCode
                        };

                        context.Users.Add(user);
                        context.SaveChanges();

                        user.AuthToken = this.GenerateAuthToken(user.Id);
                        context.SaveChanges();

                        var loggedModel = new LoggedUserModel()
                        {
                            Displayname = user.DisplayName,
                            SessionKey = user.AuthToken
                        };

                        var response =
                            this.Request.CreateResponse(HttpStatusCode.Created,
                                            loggedModel);
                        return response;
                    }
                });

            return responseMsg;
        }

        [ActionName("login")]
        public HttpResponseMessage PostLoginUser(UserModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var context = new JourneysContext();
                  using (context)
                  {
                      this.ValidateUsername(model.Username);
                      this.ValidateAuthCode(model.AuthCode);

                      var usernameToLower = model.Username.ToLower();
                      var user = context.Users.FirstOrDefault(
                          usr => usr.Username == usernameToLower
                          && usr.AuthCode == model.AuthCode);

                      if (user == null)
                      {
                          throw new InvalidOperationException("Invalid username or password");
                      }
                      if (user.AuthToken == null)
                      {
                          user.AuthToken = this.GenerateAuthToken(user.Id);
                          context.SaveChanges();
                      }

                      var loggedModel = new LoggedUserModel()
                      {
                          Displayname = user.DisplayName,
                          SessionKey = user.AuthToken
                      };

                      var response =
                          this.Request.CreateResponse(HttpStatusCode.Created,
                                          loggedModel);
                      return response;
                  }
              });

            return responseMsg;
        }

        [ActionName("logout")]
        public HttpResponseMessage PutLogoutUser(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   if (sessionKey==null)
	                {
                       throw new ArgumentNullException("User can logout only if he/she is logged in.");
	                }
                   var context = new JourneysContext();
                   using (context)
                   {
                       var user = context.Users.Where(u => u.AuthToken == sessionKey).FirstOrDefault();
                       if (user==null)
                       {
                           throw new ArgumentException("Session key is not valid");
                       }

                       user.AuthToken = null;
                       context.SaveChanges();

                       var response =
                         this.Request.CreateResponse(HttpStatusCode.OK);
                       return response;
                   }
               });

            return responseMsg;
        }

        private string GenerateAuthToken(int userId)
        {
            StringBuilder skeyBuilder = new StringBuilder(AuthTokenLength);
            skeyBuilder.Append(userId);
            while (skeyBuilder.Length < AuthTokenLength)
            {
                var index = rand.Next(AuthTokenChars.Length);
                skeyBuilder.Append(AuthTokenChars[index]);
            }
            return skeyBuilder.ToString();
        }

        private void ValidateAuthCode(string authCode)
        {
            if (authCode == null)
            {
                throw new ArgumentNullException("Password cannot be null");
            }
            if (authCode.Length != AuthCodeLength)
            {
                throw new ArgumentOutOfRangeException("Password should be encrypted");
            }
        }

        private void ValidateDisplayname(string displayname)
        {
            if (displayname==null)
            {
                throw new ArgumentException("Displayname cannot be null");
            }
            if (displayname.Length<MinUsernameAndDisplaynameLength 
                || displayname.Length>MaxUsernameAndDisplaynameLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Displayname must be between {0} and {1} characters long",
                    MinUsernameAndDisplaynameLength, MaxUsernameAndDisplaynameLength));
            }
            if (displayname.Any(ch=>!ValidDisplaynameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(
                    "Displayname must contain only Latin letters, digits '.','_',(space),'-'");
            }
        }

        private void ValidateUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("Username cannot be null");
            }
            if (username.Length < MinUsernameAndDisplaynameLength 
                ||username.Length>MaxUsernameAndDisplaynameLength)
            {
                throw new ArgumentOutOfRangeException(
                   string.Format("Username must be between {0} and {1} characters long",
                   MinUsernameAndDisplaynameLength, MaxUsernameAndDisplaynameLength));
            }
            if (username.Any(ch => !ValidUsernameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(
                    "Username must contain only Latin letters, digits '.','_'");
            }
        }
    }
}
