// This file is part of the Harvest Management library for LANDIS-II.

using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A stand ranking method based on economic ranks
    /// </summary>
    public class FireRiskRank
        : StandRankingMethod
    {
        private FireRiskTable rankTable;

        //---------------------------------------------------------------------

        public FireRiskRank(FireRiskTable rankTable)
        {
            this.rankTable = rankTable;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        protected override double ComputeRank(Stand stand, int i)
        {

            SiteVars.GetExternalVars();
            //if (SiteVars.CFSFuelType == null)
            //    throw new System.ApplicationException("Error: CFS Fuel Type NOT Initialized.  Fuel extension MUST be active.");

            double standFireRisk = 0.0;
            foreach (ActiveSite site in stand) {

                int fuelType = SiteVars.CFSFuelType[site];
                //Model.Core.UI.WriteLine("Base Harvest: ComputeRank:  FuelType = {0}.", fuelType);
                FireRiskParameters rankingParameters = rankTable[fuelType];
                standFireRisk = (double)rankingParameters.Rank;

                //foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                //{
                //    FireRiskParameters rankingParameters = rankTable[speciesCohorts.Species];
                //    foreach (ICohort cohort in speciesCohorts) {
                //        if (rankingParameters.MinimumAge > 0 &&
                //            rankingParameters.MinimumAge <= cohort.Age)
                //            siteEconImportance += (double) rankingParameters.Rank / rankingParameters.MinimumAge * cohort.Age;
                //    }
                //}
                //standEconImportance += siteEconImportance;
            }
            standFireRisk /= stand.SiteCount;

            return standFireRisk;
        }
    }
}
