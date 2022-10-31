using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarsRover.Models;

namespace MarsRover.ViewModels
{
    public class RoverViewModel
    {
        public RoverViewModel()
        {
            X = 0;
            Y = 0;
            Direction = Direction.North;
            Program = new List<Movement>();
        }

        public RoverViewModel(int x, int y, Direction direction, IList<Movement> mission)
        {
            X = x;
            Y = y;
            Direction = direction;
            Program = mission;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public Direction Direction { get; set; }

        public char DirectionAsChar => (char)Direction;
        
        public IList<Movement> Program { get; set; }

        [Required, StringLength(20, MinimumLength = 1)]
        public string MissionAsText { get; set; }

        public void InitializeMission(string missionAsText)
        {
            Program.Clear();

            //expected only the allowed characters 'L', 'R', and 'M'
            foreach(var c in missionAsText)
            {
                Movement movement;
                switch(c)
                {
                    case 'L':
                        movement = Movement.SpinLeft;
                        break;
                    case 'R':
                        movement = Movement.SpinRight;
                        break;
                    case 'M':
                        movement = Movement.MoveForward;
                        break;
                    default:
                        continue;
                }

                Program.Add(movement);                
            }
        }

        public string Description => $"({X},{Y},{Direction})";
    }
}
