// This file is part of the Harvest Management library for LANDIS-II.

using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    public class ShortPixel : SingleBandPixel<short>
    {
        public ShortPixel()
            : base()
        {
        }

        //---------------------------------------------------------------------

        public ShortPixel(short band0)
            : base(band0)
        {
        }
    }
}
