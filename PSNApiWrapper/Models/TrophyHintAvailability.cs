using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public class HintAvailabilityRoot
    {
        public HintAvailabilityData data { get; set; }
    }

    public class HintAvailabilityData
    {
        public HintAvailability hintAvailabilityRetrieve { get; set; }
    }

    public class HintAvailability
    {
        public string __typename { get; set; }
        /// <summary>
        /// Contains a list of trophies which support Game Help. 
        /// Will return an empty array if no trophies support Game Help.
        /// </summary>
        public IList<TrophyInfoWithHintAvailable> trophies { get; set; }
    }

    public class TrophyInfoWithHintAvailable
    {
        public string __typename { get; set; }
        /// <summary>
        /// Type of Game Help.
        /// </summary>
        public string helpType { get; set; }
        /// <summary>
        /// Combination of the title ID and individual trophy ID (e.g. "NPWR20188_00::18")
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// ID of the trophy
        /// </summary>
        public string trophyId { get; set; }
        /// <summary>
        /// Game Help ID
        /// </summary>
        public string udsObjectId { get; set; }
    }
}
