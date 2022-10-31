using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarsRover.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace MarsRover.Models
{
    public class RoverManager : IRoverManager
    {        
        private readonly ILogger<RoverManager> _logger;
        private readonly IMissionLoader _missionLoader;
        private readonly Queue<RoverViewModel> _missions;
        
        public RoverManager(IMissionLoader missionLoader, ILogger<RoverManager> logger)
        {
            _missionLoader = missionLoader;
            _logger = logger;
            Rovers = new List<RoverViewModel>();
            State = State.Undiscovered;
            _missions = new Queue<RoverViewModel>();                    
        }
                
        public bool ScanningInProgress { get; }

        public IList<RoverViewModel> Rovers { get; }

        public RoverViewModel CurrentRoverMission { get; private set; }

        public State State { get; private set; }

        public string Error { get; private set; }

        public void DeployRover(RoverViewModel mission)
        {
            if (State != State.ScanningMissionInProgress && !ValidateNextState(State.ScanningMissionInProgress))
            {
                _logger.LogError($"Cannot deploy extra Rover: invalid state: {State}");
                Error = "Invalid state";
                return;
            }

            Rovers.Add(mission);
            _missions.Enqueue(mission);
        
            _logger.LogInformation("Scheduled new mission");
            Error = string.Empty;
            State = State.ScanningMissionInProgress;

            StartNextMission();
        }

        public void StartDiscoveryMission()
        {
            if(!ValidateNextState(State.DiscoveryInProgress))
            {
                _logger.LogError($"Invalid state transition from {State} to State.DiscoveryInProgress");
                Error = "Invalid state";
                return;
            }

            Error = string.Empty;
            CurrentRoverMission = new RoverViewModel();
            CurrentRoverMission.MissionAsText = "Discovery";
            State = State.DiscoveryInProgress;
        }

        public void HandlePlateauDiscoveryAccomplished(double x, double y)
        {
            if (!ValidateNextState(State.ReadyToScan))
            {
                _logger.LogError($"Invalid state transition from {State} to State.ReadyToScan");
                Error = "Invalid state";
                return;
            }

            _logger.LogInformation($"Discovery mission accomplished: Upper-right corner is at ({x}, {y})");
            CurrentRoverMission = null;
            Error = string.Empty;
            State = State.ReadyToScan;
        }

        public bool StartScanningMission()
        {
            if (!ValidateNextState(State.ScanningMissionInProgress))
            {
                _logger.LogError($"Invalid state transition from {State} to State.ScanningMissionInProgress");
                Error = "Invalid state";
                return false;
            }

            var currPath = PlatformServices.Default.Application.ApplicationBasePath;
            var missions = _missionLoader.LoadMissions(Path.Combine(currPath, @"MovementFiles\missions1.csv"));
            if(!missions.Any())
            {
                _logger.LogError($"Cannot start scanning mission - invalid mission details, check the mission CSV file");
                return false;
            }

            ((List<RoverViewModel>)Rovers).AddRange(missions);

            foreach (var mission in missions)
            {
                _missions.Enqueue(mission);
            }

            _logger.LogInformation($"Scheduled {missions.Count} scanning missions");
            Error = string.Empty;
            State = State.ScanningMissionInProgress;

            return StartNextMission();
        }

        public bool StartNextMission()
        {
            if (State != State.ScanningMissionInProgress)
            {
                _logger.LogError($"Cannot start new mission: current state: {State}");
                Error = "Invalid state";
                return false;
            }

            _logger.LogDebug("StartNextMission");

            if (CurrentRoverMission != null)
            {
                _logger.LogWarning($"Cannot start new mission: another mission in progress: {CurrentRoverMission.MissionAsText}");
                return false;
            }

            if(!_missions.TryDequeue(out var mission))
            {
                _logger.LogInformation("Cannot start new mission: no scheduled missions found");
                State = State.ReadyToScan;
                Rovers.Clear();
                return false;
            }

            _logger.LogInformation($"Next mission to run: '{mission.MissionAsText}'");
            CurrentRoverMission = mission;
            return true;
        }

        public void HandleScanningMissionAccomplished()
        {
            if (State != State.ScanningMissionInProgress)
            {
                _logger.LogWarning($"Unexpected Mission Accomplished trigger in the State: {State}");
                return;
            }

            CurrentRoverMission = null;
            StartNextMission();
        }

        private bool ValidateNextState(State nextState)
        {
            switch(State)
            {
                case State.Undiscovered:
                    return nextState == State.DiscoveryInProgress;
                case State.DiscoveryInProgress:
                    return nextState == State.ReadyToScan;
                case State.ReadyToScan:
                    return nextState == State.ScanningMissionInProgress;
                case State.ScanningMissionInProgress:
                    return nextState == State.ReadyToScan;
            }

            return false;
        }        
    }
}
