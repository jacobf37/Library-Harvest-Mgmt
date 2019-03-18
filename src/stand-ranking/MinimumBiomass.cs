// This file is part of the Harvest Management library for LANDIS-II.

using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;


namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A ranking requirement which marks a stand for harvest if it has met the time since last fire
    /// </summary>
    public class MinimumBiomass
        : IRequirement
    {
        private ushort minimumBiomass;

        //---------------------------------------------------------------------

        public MinimumBiomass(ushort agb)
        {
            minimumBiomass = agb;
        }

        //---------------------------------------------------------------------

        bool IRequirement.MetBy(Stand stand)
        {
            double avgAGB = 0.0;
            int num = 0;

                foreach(ActiveSite site in stand)
                {
                    avgAGB += CohortTotal(site);
                    num = num + 1;
                }

            avgAGB = avgAGB / (double) num;

            if(num == 0)
            {
                return false;
            }

            return avgAGB >= minimumBiomass;
        }

        double CohortTotal(ActiveSite site)
        {
            double agb = 0.0;
            foreach(ICohort cohort in SiteVars.Cohorts[site])
            {
                agb += cohort.Biomass;
            }
            return agb;
        }
    }
}
