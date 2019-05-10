using Landis.Library.Metadata;

namespace Landis.Library.HarvestManagement
{
    public class SummaryLog
    {

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time { set; get; }

        [DataFieldAttribute(Desc = "Management Area")]
        public uint ManagementArea { set; get; }

        [DataFieldAttribute(Desc = "Prescription Name")]
        public string Prescription { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Harvested Sites")]
        public int HarvestedSites { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Cohorts Complete Harvest")]
        public int TotalCohortsCompleteHarvest { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Cohorts Partial Harvest")]
        public int TotalCohortsPartialHarvest { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.None, Desc = "Total Biomass Harvested (Mg)")]
        public double TotalBiomassHarvested { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Cohorts Harvested by Species", SppList = true)]
        public double[] CohortsHarvested_ { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.None, Desc = "Biomass Harvested by Species (Mg)", SppList = true)]
        public double[] BiomassHarvestedMg_ { set; get; }
    }
}
