using MarsRover.ViewModels;
using System.Collections.Generic;

namespace MarsRover.Models
{
    public interface IMissionLoader
    {
        IList<RoverViewModel> LoadMissions(string filePath);
    }
}
