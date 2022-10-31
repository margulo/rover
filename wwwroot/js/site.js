let NasaControlPanel = (function () {
    function NasaControlPanel(config) {
        this.canvas = config.canvas;
        this.unitSize = config.unitSize;
        this.columns = config.columns;
        this.lines = config.lines;
        this.drawRate = config.drawRate;
        this.gridSize = config.gridSize;

        this.width = this.canvas.width = this.unitSize * this.columns;
        this.height = this.canvas.height = this.unitSize * this.lines;

        this.ctx = this.canvas.getContext('2d');
        this.infoPanel = config.infoPanel;
        this.infoPanel.style.width = this.width - 12 + 'px';
        this.infoGeneration = config.infoGeneration;
        this.infoStatus = config.infoStatus;
        this.roverImage = new Image();
        
        this.gameOn = false;
        this.gameBox = config.gameBox;

        this.gameBoxSize = { w: this.gameBox.clientWidth, h: this.gameBox.clientHeight };
    }

    NasaControlPanel.prototype.init = function () {        

    };

    //NasaControlPanel.prototype.drawGrid = function () {

    //    var hLines = this.height / this.unitSize / this.gridSize;
    //    var wLines = this.width / this.unitSize / this.gridSize;

    //    for (var i = 0; i < hLines; i++) {
    //        this.ctx.beginPath();
    //        this.ctx.moveTo(0, i * this.gridSize * this.unitSize - .5);
    //        this.ctx.lineTo(this.width, i * this.gridSize * this.unitSize - .5);

    //        if (i % 5) {
    //            this.ctx.strokeStyle = 'rgba(66,66,66,.2)';
    //        } else {
    //            this.ctx.strokeStyle = 'rgba(66,66,66,.7)';
    //        }
    //        this.ctx.stroke();
    //        this.ctx.closePath();
    //    }

    //    for (var i = 0; i < wLines; i++) {
    //        this.ctx.beginPath();
    //        this.ctx.moveTo(i * this.gridSize * this.unitSize - .5, 0);
    //        this.ctx.lineTo(i * this.gridSize * this.unitSize - .5, this.height);

    //        if (i % 5) {
    //            this.ctx.strokeStyle = 'rgba(66,66,66,.2)';
    //        } else {
    //            this.ctx.strokeStyle = 'rgba(66,66,66,.7)';
    //        }
    //        this.ctx.stroke();
    //        this.ctx.closePath();
    //    }
    //};

    NasaControlPanel.prototype.draw = function () {        
        this.ctx.clearRect(0, 0, this.width, this.height);

        //this.drawGrid();
        this.drawRovers();
    };

    NasaControlPanel.prototype.degToRad = function (d) {
        // Converts degrees to radians  
        return d * 0.01745;
    }

    NasaControlPanel.prototype.drawRovers = function () {
        let state = this.infoStatus.textContent;

        switch (state)
        {
            case 'DiscoveryInProgress':
                this.discoveryMission();
                break;
            case 'ScanningMissionInProgress':
                startMission();
                break;
        }
    }

    NasaControlPanel.prototype.discoveryMission = function () {
        console.log("discoveryMission");
                
        const roverH = 35;

        //TODO: temporarily hard-coded 'discovery' mission:
        //it just leads the rover to the upper-right corner
        let mission = new RoverMission(0, this.canvas.height - roverH / 2, 'N', "MMMMRMMMMMLMRMMM");
        let animation = new RoverAnimation(mission, this.ctx);
        animation.runRover();
    }        

    NasaControlPanel.prototype.start = function () {
        this.init();
        this.draw();            
    };

    return NasaControlPanel;

})();


var c = document.getElementById('world');

var infoPanel = document.getElementsByClassName('info-panel')[0];
var infoGeneration = document.getElementById('info_generation');
var infoStatus = document.getElementById('info_status');
    
var gameBox = document.getElementById('game');

var controlPanel = new NasaControlPanel({
    canvas: c,

    unitSize: 2,
    columns: 180,
    lines: 120,
    drawRate: 1000 / 16,
    gridSize: 4,

    infoPanel: infoPanel,
    infoGeneration: infoGeneration,
    infoStatus: infoStatus,
    gameBox: gameBox
});

controlPanel.start();
