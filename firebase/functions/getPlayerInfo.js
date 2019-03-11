const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db = admin.firestore();

exports.getPlayerInfo = () => functions.https.onRequest((request, response) => {
    const uuid = request.query.uuid;

    db.collection('players')
        .doc(uuid)
        .get()
        .then( snapshot => {
            console.log(snapshot.data());
            return response.send(JSON.stringify(snapshot.data()))
        })
        .catch( error => {
            console.log(error);
            return response.send(error);
        })
});

exports.getTurns = () => functions.https.onRequest((request, response) => {
    const uuid = request.query.uuid;

    db.collection('players')
    .doc(uuid)
    .get()
    .then( snapshot => {
        var games = snapshot.data().games;

        var payload = [];
        games.forEach( (gameid, index) => {
            db.collection('games')
            .doc(gameid)
            .get()
            .then( gamesnapshot => {
                var gamedata = gamesnapshot.data();
                payload.push({
                    gameid: gamesnapshot.id,
                    turn: gamedata.currentTurn
                });

                if (index === games.length-1)
                {
                    return response.send(JSON.stringify(payload));
                }

                return;
            })
            .catch( error => {
                console.log(error);
            });
        });

        return;
    })
    .catch( error => {
        response.send(error);
    });
})