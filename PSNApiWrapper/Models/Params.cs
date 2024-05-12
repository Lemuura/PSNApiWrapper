using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public class HelpTrophiesParams
    {
        /// <summary>
        /// ID of the trophy
        /// </summary>
        public string trophyId { get; set; }
        /// <summary>
        /// ID of the Game Help
        /// </summary>
        public string udsObjectId { get; set; }
        /// <summary>
        /// Type of Game Help
        /// Usually "HINT" or "HINTGROUP"
        /// </summary>
        public string helpType { get; set; }

        public HelpTrophiesParams(string trophyId, string udsObjectId, string helpType = "HINT")
        {
            this.trophyId = trophyId;
            this.udsObjectId = udsObjectId;
            this.helpType = helpType;
        }
    }
}
