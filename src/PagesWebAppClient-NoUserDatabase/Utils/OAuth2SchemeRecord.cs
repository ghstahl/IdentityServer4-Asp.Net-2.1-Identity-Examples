using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagesWebAppClient.Utils
{
    /*
      "oauth2": [
         
         {
           "scheme": "google",
           "authority": "https://accounts.google.com",
           "callbackPath": "/signin-google",
           "additionalEndpointBaseAddresses": []
         }
       ]
    */

    public class OAuth2SchemeRecord
    {
        public string Scheme { get; set; }
        public string ClientId { get; set; }
        public string Authority { get; set; }
        public string CallbackPath { get; set; }
        public List<string> AdditionalEndpointBaseAddresses { get; set; }
    }
}
