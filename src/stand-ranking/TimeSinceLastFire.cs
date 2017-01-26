// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A ranking requirement which marks a stand for harvest if it has met the time since last fire
    /// </summary>
    public class TimeSinceLastFire
        : IRequirement
    {
        private ushort timeSinceFire;

        //---------------------------------------------------------------------

        public TimeSinceLastFire(ushort firetime)
        {
            timeSinceFire = firetime;
        }

        //---------------------------------------------------------------------

        bool IRequirement.MetBy(Stand stand)
        {
            double avgtime = 0;
            int num = 0;

            if(SiteVars.TimeOfLastFire != null)
            {
                foreach(ActiveSite site in stand)
                {
                    avgtime = avgtime + SiteVars.TimeOfLastFire[(Site)site];
                    num = num + 1;
                }
            }

            avgtime = avgtime / num;

            if(num == 0)
            {
                return false;
            }

            return avgtime >= timeSinceFire;
        }
    }
}
