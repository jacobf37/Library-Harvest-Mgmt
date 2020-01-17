// This file is part of the Harvest Management library for LANDIS-II.

using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// Utility class for prescription maps.
    /// </summary>
    public class PrescriptionMaps
    {
        private string nameTemplate;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="nameTemplate">
        /// The template for the pathnames to the maps.
        /// </param>
        public PrescriptionMaps(string nameTemplate)
        {
            this.nameTemplate = nameTemplate;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Writes an output map of prescriptions that harvested each active
        /// site.
        /// </summary>
        /// <param name="timestep">
        /// Timestep to use in the map's name.
        /// </param>
        public void WriteMap(int timestep)
        {
            string path = MapNames.ReplaceTemplateVars(nameTemplate, timestep);
            using (IOutputRaster<ShortPixel> outputRaster = Model.Core.CreateRaster<ShortPixel>(path, Model.Core.Landscape.Dimensions, 
                Model.Core.LandscapeMapMetadata))
            {
                foreach (Site site in Model.Core.Landscape.AllSites)
                {
                    if (site.IsActive) {
                        Prescription prescription = SiteVars.Prescription[site];
                        if (prescription == null)
                            outputRaster.WritePixel(new ShortPixel((short)1));
                        else
                            outputRaster.WritePixel(new ShortPixel((short)(prescription.Number + 1)));
                    }
                    else {
                        //  Inactive site
                        outputRaster.WritePixel(new ShortPixel((short)0));
                    }
                }
            }
        }

    }
}
