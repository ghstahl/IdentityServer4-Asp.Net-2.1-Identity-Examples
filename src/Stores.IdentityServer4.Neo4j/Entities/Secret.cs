using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using IdentityServer4;

namespace StoresIdentityServer4.Neo4j.Entities
{
    public abstract class Secret
    {
        private string _description;

        public string Description
        {
            get
            {
                return string.IsNullOrEmpty(_description) ? "" : _description;
            }
            set { _description = value; }
        }

        public string Value { get; set; }
        private DateTime _expiration;

        public DateTime? Expiration
        {
            get
            {
                if (_expiration == null)
                {
                    _expiration = DateTime.ParseExact("01/01/2100", "d", CultureInfo.InvariantCulture);
                }

                return _expiration;
            }
            set { value = _expiration; }
        }

        public string Type { get; set; } = IdentityServerConstants.SecretTypes.SharedSecret;
    }
}
