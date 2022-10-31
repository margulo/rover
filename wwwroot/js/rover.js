class RoverMission {
    constructor(startX, startY, initialDirection, mission) {
        this.startX = startX;
        this.startY = startY;
        this.initialDirection = initialDirection;
        this.mission = mission;
    }
}

//this class controls the rover animation and draws a mission
class RoverAnimation {
    constructor(mission, ctx) {
        this.roverImage = new Image();
        this.x = mission.startX;
        this.y = mission.startY;
        this.direction = mission.initialDirection;
        this.mission = mission.mission;
        this.ctx = ctx;
        this.intervalId = 0;
        this.speed = 700; //delay in millisec. between iterations
        this.missionIx = 0; //current index in the mission movements array
    }

    static roverW = 20;
    static roverH = 35;
    static gridSizeX = 50;
    static gridSizeY = 40;
       

    runRover() {
        this.roverImage.addEventListener('load', () => {            
            this.missionIx = 0;
            this.intervalId = setInterval(this.#drawStep.bind(this), this.speed);

        }, false);

        this.roverImage.src = "../img/rover.jpg";
    }

    #drawStep() {
        if (this.missionIx >= this.mission.length - 1) {
            console.log("stop animation");
            clearInterval(this.intervalId);

            onMissionFinished(this.x, this.y);
            return;
        }

        console.log(`Clear: [${this.x},${this.y}]`);
        this.ctx.setTransform(1, 0, 0, 1, 0, 0); // Reset transformation matrix to the identity matrix

        // clear the last drawn frame
        switch (this.direction) {
            case 'N':
            case 'S':
                this.ctx.clearRect(this.x, this.y, RoverAnimation.roverW, RoverAnimation.roverH * 2);
                break;
            default:
                this.ctx.clearRect(this.x - RoverAnimation.roverH, this.y - RoverAnimation.roverW * 0.5, RoverAnimation.roverH * 2, RoverAnimation.roverW * 2);
                //ctx.rect(x, y, roverH * 2, roverW);
                //ctx.fill();
                break;
        }

        this.ctx.save();

        let command = this.mission[this.missionIx];
        switch (command) {
            case 'M':
                let { nextX, nextY } = this.#calcNextLocation(this.x, this.y, this.direction);
                this.x = nextX;
                this.y = nextY;
                break;
            case 'L':
            case 'R':
                this.#setNextDirection(command);
                break;
        }

        this.#drawRover();

        this.missionIx++;
    }

    #calcNextLocation(currX, currY, direction) {
        let nextX, nextY;

        switch (direction) {
            case 'N':
                nextX = currX;
                nextY = currY - RoverAnimation.gridSizeY;
                break;
            case 'S':
                nextX = currX;
                nextY = currY + RoverAnimation.gridSizeY;
                break;
            case 'W':
                nextX = currX - RoverAnimation.gridSizeX;
                nextY = currY;
                break;
            case 'E':
                nextX = currX + RoverAnimation.gridSizeX;
                nextY = currY;
                break;
            default:
                nextX = currX;
                nextY = currY;
        }

        return { nextX, nextY };
    }

    #directionToAngle(direction) {
        switch (direction) {
            case 'N':
                return 0.0;
            case 'W':
                return -90.0;
            case 'S':
                return 180.0;
            case 'E':
                return 90.0;
            default:
                return 0.0;
        }
    }

    #setNextDirection(command) {
        switch (command) {
            case 'L': //turn 90 deg. left
                switch (this.direction) {
                    case 'N':
                        this.direction = 'W';
                        return;
                    case 'W':
                        this.direction = 'S';
                        return;
                    case 'S':
                        this.direction = 'E';
                        return;
                    case 'E':
                        this.direction = 'N';
                        return;
                }
                break;
            case 'R': //turn 90 deg. right
                switch (this.direction) {
                    case 'N':
                        this.direction = 'E';
                        return;
                    case 'E':
                        this.direction = 'S';
                        return;
                    case 'S':
                        this.direction = 'W';
                        return;
                    case 'W':
                        this.direction = 'N';
                        return;
                }
        }
    }

    #drawRover() {
        console.log(`drawRover [${this.x},${this.y}]`);

        this.ctx.translate(this.x, this.y);

        let rotationDeg = this.#directionToAngle(this.direction);
        console.info("Rotate: " + rotationDeg);
        this.ctx.rotate(degToRad(rotationDeg));
        this.ctx.drawImage(this.roverImage, 0, 0, RoverAnimation.roverW, RoverAnimation.roverH);
    }
}