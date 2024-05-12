using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public abstract class TrophyTitleBase
    {
        /// <summary>
        /// The current version of the trophy set
        /// </summary>
        public string trophySetVersion { get; set; }
        /// <summary>
        /// Title name
        /// </summary>
        public string trophyTitleName { get; set; }
        /// <summary>
        /// Title description. PS3, PS4 and PS Vita titles only.
        /// </summary>
        public string trophyTitleDetail { get; set; }
        /// <summary>
        /// URL of the icon for the title
        /// </summary>
        public string trophyTitleIconUrl { get; set; }
        /// <summary>
        /// The platform this title belongs to. 
        /// Some games have trophy sets which are shared between multiple platforms (ie. PS4,PSVITA). 
        /// The platforms will be comma separated.
        /// </summary>
        public string trophyTitlePlatform { get; set; }
    }
}
