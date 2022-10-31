  Mars Rover Project
User Guide:

Main menu command buttons:
 DISCOVERY MISSION - start Discovery mission  - run a rover to the right-upper corner of the plateau and report coordinates
                     (coordinates are reported in the logs only, currently always reports [0,0] - known bug)
                     Discovery can run only oncem after it's completed the Status will change from 'Undiscovered' to 'ReadyToScan'

 DEPLOY SCANNER ROVERS - load rover missions from the movement file which must be in the Bin folder:
                         in development environment it's: bin\Debug\net6.0\MovementFiles\missions1.csv
                         (the file path is hard-coded and relative to the working folder the app is running from)
                         
                         If there is an issue with loading the missions, errors will be printed in the logs
                         Any mission successfully loaded will appear in the list of ROVERS on the right showing the initial coordinates
                         and direction.

                         The missions will run sequentially until the last one is done
                         The status bar should display the status: 'ScanningMissionInProgress' and MISSION: section should display the mission (movements) currently in progress
                         Note: some animations are buggy: previous rover location is not removed properly - known bug.
                         After all the missions are completes the State will return to 'ReadyToScan'

 DEPLOY EXTRA ROVER  - deploys an additional scanning rover, the details of the mission must be filled in first (under the main menu)
                       the UI (status bar and the ROVERS section) will behave the same way as with the pre-loaded rover missions (see above)

_________________________________
Assumptions:
   - no failed missions: for simplicity
   - only one Discovery Mission is allowed (the first mission to check the upper-right coordinates)
   - Nasa cannot deploy more than one scanning mission (of multiple rovers defined in the CSV file) at a time
   - Cannot deploy any rovers before the Discovery Mission is aacomplished
   - To deploy an extra rover:
       a) if there's no scanning in progress - start the mission immediately
       b) if there's a scanning mission in progress (including any extra rovers) - schedule the extra rover after all the currently deployed rovers are finished their programs
   
   - There's no limit on amount of rovers
   - We're not checking for repeated/partially repeated missions (it would be a good optimization feature)
   - Movements CSV file: assume the coordinates are all integer numbers in range (0,0) to (5,5) i.i inside the plateau,
      any numbers outside of this range will be considered illegal and the mission will not load (other missions should load if they're valid)
  
   - The rovers are actually deployed (placed) on the plateau in sequence, only one rover can be ona plateau at any given time.
     This means we're not dealing with edge cases where a scanning path of one rover can cross location of another one.

___________________________________

* Simple state machine pattern implemented to avoid invalid user commands:
  - Discovery Mission has to be completed first (no scanning missions can run prior to that)
  - Only one mission can run at a time

* When user (NASA) want to start a new Scanning mission (i.e. deploy a sequence of rovers):
  1) If there is a 

________________________________
Known Bugs I didn't fix

* The Discovery mission doesn't pass the final coordinates to the server  - it always passes (0,0) instead

* Multiple animation bugs: rovers are not redrawn correctly - probably bad coordinate calculations around the ctx.clearRect()

* Better looking status bar expeciall when there're errors

* UI: originally I've borrowed a logic to draw a grid, but the rovers animation deletes the underlying grid,
      for now I've commented the grid out.

__________________________________
Wish List (features that would be implemented if I had more time)

* Missions are pre-validated on the server side before launch to make sure the rover is not going beyond the plateau boundaries

* The Discovery mission is 'fake' - i.e. I'm not actually testing the plateau borders, but just pre-programming it to run to the 
right-upper corner, ideally I would generate a randon plateau and implement the real discovery searching for the boumdary

* It's worth assigning IDs and friendly names to Rovers and missions, but this is out of scope.

* There can be multiple synchronisation issues with user(s) scheduling multiple missions, but I didn't implement any synchronisation
 due to lack of time.

* For smoother more 'real-time' front-end operation I would implement a SignalR connection: the server would report mission commands and errors
and there will be less page reloads.

* I could have done much smoother animation with nice 90 degree turns left and right

* Making the animation speed configurable
