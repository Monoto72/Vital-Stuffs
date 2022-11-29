let currentCheckpoint;
let currentCheckpointBlip;
let sessionTimer;
let returnTimer;

mp.keys.bind(0x59, false, function() {
    mp.events.callRemote('DrivingSchool:Start');
});

mp.events.add('DrivingSchool:Test:StartTimer', () => { 
    if (sessionTimer) {
        clearInterval(sessionTimer);
        sessionTimer = null;
    }

    sessionTimer = setInterval(() => {
        if (mp.players.local.vehicle) {
            mp.events.callRemote('DrivingSchool:SpeedCheck', mp.players.local.vehicle.getSpeed() * 3.6);
        }
    }, 5000);
});

mp.events.add('DrivingSchool:Test:StopTimer', () => { 
    if (sessionTimer) {
        clearInterval(sessionTimer);
        sessionTimer = null;;
    }
    
});


mp.events.add('DrivingSchool:RenderCheckpoint', (x, y, z, x2, y2, z2) => {
    currentCheckpoint = mp.checkpoints.new(1, new mp.Vector3(x, y, z), 10,{
        direction: new mp.Vector3(x2, y2, z2),
        color: [245, 188, 66, 255],
        visible: true,
        dimension: 0
    });

    currentCheckpointBlip = mp.blips.new(1, new mp.Vector3(x, y, z), {
        name: 'DMV Checkpoint',
        color: 1,
        shortRange: false,
        dimension: 0
    });

    currentCheckpointBlip.setRoute(true);
});

mp.events.add('playerEnterCheckpoint', (checkpoint) => {
    if (!mp.players.local.vehicle) return;
    mp.events.callRemote('DrivingSchool:ReachedCheckpoint');
    currentCheckpoint.destroy();
    currentCheckpointBlip.destroy();
});