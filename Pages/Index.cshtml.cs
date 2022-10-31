using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarsRover.Models;
using MarsRover.ViewModels;

namespace MarsRover.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IRoverManager _roverManager;
                
        [BindProperty]
        public RoverViewModel RoverToDeploy { get; set; }
                
        public IList<RoverViewModel> Rovers => _roverManager?.Rovers;

        public State State => _roverManager.State;

        public string Error => _roverManager.Error;

        public RoverViewModel CurrentRoverMission => _roverManager.CurrentRoverMission;
        
        public double FinalX { get; set; }
        public double FinalY { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IRoverManager roverManager)
        {
            _logger = logger;
            _roverManager = roverManager;            
        }        
                
        public void OnPostDeploy()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Cannot deply extra Rover - the data is invalid");
                return;
            }

            RoverToDeploy.InitializeMission(RoverToDeploy.MissionAsText);
            _roverManager.DeployRover(RoverToDeploy);            
        }

        public void OnGetStartFirstMission()
        {
            _roverManager.StartDiscoveryMission();
        }

        public void OnGetMissionFinished(double x, double y)
        {
            switch(this.State)
            {
                case State.DiscoveryInProgress:
                    _roverManager.HandlePlateauDiscoveryAccomplished(x, y);
                    break;
                case State.ScanningMissionInProgress:
                    _roverManager.HandleScanningMissionAccomplished();
                    break;
                default:
                    _logger.LogWarning($"OnGetMissionFinished called in unexpected State: {this.State}");
                    break;
            }            
        }

        public void OnGetDeployRovers()
        {
            _roverManager.StartScanningMission();
        }        
    }
}
