// This file is part of the Harvest Management library for LANDIS-II.

using Landis.Utilities;
using Landis.SpatialModeling;
using System;
using System.Collections.Generic;
using Landis.Core;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// The application of a repeat-harvest to a management area.
    /// </summary>
    public class AppliedRepeatHarvest
        : AppliedPrescription
    {
        private delegate void SetAsideMethod(Stand stand);

        //---------------------------------------------------------------------

        private RepeatHarvest repeatHarvest;
        private bool isMultipleRepeatHarvest;
        private SetAsideMethod setAside;
        // tjs 2009.01.09
        private bool hasBeenHarvested;
        //  The queue is in the chronological order.
        public Queue<ReservedStand> reservedStands;
        private int damagedSites;
        private int cohortsDamaged;
        private int cohortsKilled;
        double biomassRemoved;
        IDictionary<ISpecies, double> totalBiomassBySpecies = new Dictionary<ISpecies, double>();

        //---------------------------------------------------------------------

        public AppliedRepeatHarvest(RepeatHarvest  repeatHarvest,
                                    Percentage percentageToHarvest,
                                    Percentage percentStandsToHarvest,
                                    int            beginTime,
                                    int            endTime)
            : base(repeatHarvest,
                   percentageToHarvest,
                   percentStandsToHarvest,
                   beginTime,
                   endTime)
        {
            this.repeatHarvest = repeatHarvest;
            // tjs 2009.01.09
            hasBeenHarvested = false;
            if (repeatHarvest is SingleRepeatHarvest) {
                isMultipleRepeatHarvest = false;
                setAside = SetAsideForSingleHarvest;
            }
            else {
                isMultipleRepeatHarvest = true;
                setAside = SetAsideForMultipleHarvests;
            }
            this.reservedStands = new Queue<ReservedStand>();
            this.damagedSites = 0;
            this.cohortsDamaged = 0;
            this.cohortsKilled = 0;
            this.biomassRemoved = 0;
        }

        //---------------------------------------------------------------------

        // <summary>
        // Has this ever been harvested - tjs 2009.01.09
        // </summary>
        public bool HasBeenHarvested
        {
            get
            {
                return hasBeenHarvested;
            }
            set
            {
                hasBeenHarvested = value;
            }
        }
        // <summary>
        // Time interval for repeat harvest - tjs 2008.12.17
        // </summary>
        public int Interval
        {
            get
            {
                return repeatHarvest.Interval;
            }
        }
        // <summary>
        // Whether the prescription is a mutliple repeat harvest.
        // </summary>
        public bool IsMultipleRepeatHarvest
        {
            get {
                return isMultipleRepeatHarvest;
            }
        }

        // <summary>
        // Resets the values tracked for logging the repeat harvests
        // </summary>
        public void ResetLogValues()
        {
            this.damagedSites = 0;
            this.cohortsDamaged = 0;
            this.cohortsKilled = 0;
            this.biomassRemoved = 0;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sets a stand aside for a single additional harvest.
        /// </summary>
        public void SetAsideForSingleHarvest(Stand stand)
        {
            stand.SetAsideUntil(Model.Core.CurrentTime + repeatHarvest.Interval);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sets a stand aside for multiple additional harvests.
        /// </summary>
        public void SetAsideForMultipleHarvests(Stand stand)
        {
            stand.SetAsideUntil(EndTime);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests the highest-ranked stand which hasn't been harvested yet
        /// during the current timestep.
        /// </summary>
        public override void HarvestHighestRankedStand()
        {        
            base.HarvestHighestRankedStand();

            foreach (Stand stand in repeatHarvest.HarvestedStands)
            {
                if (!stand.IsSetAside)
                {
                    setAside(stand);
                    ScheduleNextHarvest(stand);
                }
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Schedules the next harvest for a stand that's been set aside
        /// (reserved).
        /// </summary>
        protected void ScheduleNextHarvest(Stand stand)
        {
            int nextTimeToHarvest = Model.Core.CurrentTime + repeatHarvest.Interval;
            if (nextTimeToHarvest <= EndTime)
                reservedStands.Enqueue(new ReservedStand(stand, nextTimeToHarvest));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests the stands that have repeat harvests scheduled for the
        /// current time step.
        /// </summary>
        public void HarvestReservedStands()
        {
            this.repeatHarvest.IncrementRepeat();

            while (reservedStands.Count > 0 &&
                   reservedStands.Peek().NextTimeToHarvest <= Model.Core.CurrentTime)
            {
                //Stand stand = reservedStands.Dequeue().Stand;
                Stand stand = reservedStands.Peek().Stand;

                repeatHarvest.Harvest(stand);

                stand = reservedStands.Dequeue().Stand;

                // Calculate some values for the eventlog
                foreach (ActiveSite site in stand)
                {
                    if (Landis.Library.BiomassHarvest.SiteVars.CohortsPartiallyDamaged[site] > 0 || SiteVars.CohortsDamaged[site] > 0)
                    {
                        damagedSites++;

                        //Conversion from [g m-2] to [Mg ha-1] to [Mg]
                        biomassRemoved += SiteVars.BiomassRemoved[site] / 100.0 * modelCore.CellArea;
                        IDictionary<ISpecies, int> siteBiomassBySpecies = SiteVars.BiomassBySpecies[site];
                        if (siteBiomassBySpecies != null)
                        {
                            // Sum up total biomass for each species
                            foreach (ISpecies species in modelCore.Species)
                            {
                                int addValue = 0;
                                siteBiomassBySpecies.TryGetValue(species, out addValue);
                                double oldValue;
                                if (totalBiomassBySpecies.TryGetValue(species, out oldValue))
                                {
                                    totalBiomassBySpecies[species] += addValue / 100.0 * modelCore.CellArea;
                                }
                                else
                                {
                                    totalBiomassBySpecies.Add(species, addValue / 100.0 * modelCore.CellArea);
                                }
                            }
                        }
                    }
                    EventsLog el = new EventsLog();
                    el.Time = Model.Core.CurrentTime;
                    el.ManagementArea = stand.ManagementArea.MapCode;
                    el.Prescription = stand.PrescriptionName + "(" + this.repeatHarvest.RepeatNumber + ")";
                    el.Stand = stand.MapCode;
                    el.EventID = stand.EventId;
                    el.StandAge = stand.Age;
                    el.StandRank = Convert.ToInt32(stand.HarvestedRank);
                    el.NumberOfSites = stand.SiteCount;
                    el.HarvestedSites = damagedSites;
                    el.MgBiomassRemoved = biomassRemoved;
                    el.MgBioRemovedPerDamagedHa = biomassRemovedPerHa;
                    el.TotalCohortsPartialHarvest = cohortsDamaged;
                    el.TotalCohortsCompleteHarvest = cohortsKilled;
                    el.CohortsHarvested_ = species_cohorts;
                    el.BiomassHarvestedMg_ = species_biomass;

                    Log.WriteEvents(el);
                }

                if (isMultipleRepeatHarvest)
                    ScheduleNextHarvest(stand);
            }
        }
    }
}