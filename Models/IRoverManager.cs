using System.Collections.Generic;
using MarsRover.ViewModels;

namespace MarsRover.Models
{
    public interface IRoverManager
    {
        IList<RoverViewModel> Rovers { get; }

        State State { get; }

        string Error { get; }

        RoverViewModel CurrentRoverMission { get; }

        void DeployRover(RoverViewModel mission);

        void StartDiscoveryMission();
        void HandlePlateauDiscoveryAccomplished(double x, double y);

        bool StartScanningMission();
        void HandleScanningMissionAccomplished();
        bool StartNextMission();
    }
}
