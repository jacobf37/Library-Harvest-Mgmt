// This file is part of the Harvest Management library for LANDIS-II.

using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// Represents when a prescription has finished a repeat harvest step
    /// </summary>
    public class RepeatHarvestPrescriptionFinishedEvent
    {
        /// <summary>
        /// Information about an event that's passed to event handlers.
        /// </summary>
        public class Args : System.EventArgs
        {
            /// <summary>
            /// The prescription that just finished its repeat harvest
            /// </summary>
            public Prescription Prescription { get; protected set; }

            /// <summary>
            /// The management area the prescription belongs to
            /// </summary>
            public ManagementArea MgmtArea { get; protected set; }

            /// <summary>
            /// Creates a new instance.
            /// </summary>
            /// <param name="site">The prescription that just finished its repeat harvest</param>
            public Args(Prescription prescription, ManagementArea mgmtArea)
            {
                Prescription = prescription;
                MgmtArea = mgmtArea;
            }
        }

        /// <summary>
        /// Handler for this type of event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="eventArgs">Details about the event.</param>
        public delegate void Handler(object sender,
                                     Args eventArgs);
    }
}