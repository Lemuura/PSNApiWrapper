using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public class AuthenticationData
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public double Expires_In { get; set; }
        public string Scope { get; set; }
        public string Id_Token { get; set; }
    }
}
