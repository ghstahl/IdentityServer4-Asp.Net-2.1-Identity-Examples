using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using IdentityServer4;
using Newtonsoft.Json;

namespace StoresIdentityServer4.Neo4j.Entities
{
    public abstract class Secret
    {
        private string _description;

        public string Description { get; set; }

        public string Value { get; set; }
       
     
        public string ExpirationJson { get; set; }
        /*
        {
            get
            {
                if (Expiration != null)
                {
                    var dateTime = ((DateTime) Expiration).ToString("MM'/'dd'/'yyyy HH':'mm");
                    return dateTime;
                }

                return null;
            }
            set
            {
                Expiration =  DateTime.ParseExact(value, "MM'/'dd'/'yyyy HH':'mm", CultureInfo.InstalledUICulture);

            }
        }
*/
        public string Type { get; set; } = IdentityServerConstants.SecretTypes.SharedSecret;
    }
}
