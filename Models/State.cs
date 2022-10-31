namespace MarsRover.Models
{
    public enum State
    {
        /// <summary>
        /// Need to finish discovery mission
        /// </summary>
        Undiscovered,

        /// <summary>
        /// Discovery mission in progress
        /// </summary>
        DiscoveryInProgress,

        /// <summary>
        ///  All pending missions have been finished, can deploy scanners
        /// </summary>
        ReadyToScan,

        /// <summary>
        /// Scanning rover mission in progress (can schedule additional rovers if needed)
        /// </summary>
        ScanningMissionInProgress,
    }
}
