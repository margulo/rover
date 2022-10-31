using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using MarsRover.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace MarsRover.Models
{
    public class MissionLoader : IMissionLoader
    {
        public class MissionRow
        {
            [Index(0)]
            public string StartPosition { get; set; }

            [Index(1)]
            public string Movements { get; set; }
        }

        private readonly ILogger<MissionLoader> _logger;

        public MissionLoader(ILogger<MissionLoader> logger)
        {
            _logger = logger;
        }

        public IList<RoverViewModel> LoadMissions(string filePath)
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "|", HasHeaderRecord = false };
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, config))
                {
                    const int MinX = 0;
                    const int MaxX = 5;
                    const int MinY = 0;
                    const int MaxY = 5;

                    var missionRecords = csv.GetRecords<MissionRow>();
                    var missions = new List<RoverViewModel>();

                    foreach(var rawMission in missionRecords)
                    {
                        try
                        {
                            var segments = rawMission.StartPosition.Split(' ');
                            if(segments.Length != 3)
                            {
                                _logger.LogError($"Invalid start position: '{rawMission.StartPosition}'");
                                continue;
                            }

                            var startX = int.Parse(segments[0]);
                            if(startX > MaxX || startX < MinX)
                            {
                                _logger.LogError($"Invalid start X position: '{startX}'");
                                continue;
                            }

                            var startY = int.Parse(segments[1]);
                            if (startY > MaxY || startY < MinY)
                            {
                                _logger.LogError($"Invalid start Y position: '{startY}'");
                                continue;
                            }

                            var direction = (Direction)segments[2][0];
                            if(!Enum.IsDefined(typeof(Direction), direction))
                            {
                                _logger.LogError($"Invalid direction: '{segments[2]}'");
                                continue;
                            }

                            //validate the mission
                            if(!Regex.IsMatch(rawMission.Movements, "^[LRM]+$"))
                            {
                                _logger.LogError($"Invalid movements: '{rawMission.Movements}'");
                                continue;
                            }

                            //TODO: implement mission validation here i.e. make sure the rover will never receive a command
                            //      to move outside the plateau limits (0,0) - (5,5)

                            var mission = new RoverViewModel();
                            mission.X = startX;
                            mission.Y = startY;
                            mission.Direction = direction;
                            foreach(var c in rawMission.Movements)
                            {
                                mission.Program.Add((Movement)c);
                            }
                            mission.MissionAsText = rawMission.Movements;

                            missions.Add(mission);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError($"Failed to patse mission: '{ex.Message}'", ex);
                        }
                    }

                    return missions;
                }                
            }
            catch(Exception e)
            {
                _logger.LogError($"Failed to load missions: {e.Message}", e);
                return new List<RoverViewModel>();
            }            
        }
    }
}
