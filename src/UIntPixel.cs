// This file is part of the Harvest Management library for LANDIS-II.
using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    public class UIntPixel : SingleBandPixel<uint>
    {
        public UIntPixel()
            : base()
        {
        }

        //---------------------------------------------------------------------

        public UIntPixel(ushort band0)
            : base(band0)
        {
        }
    }
}
