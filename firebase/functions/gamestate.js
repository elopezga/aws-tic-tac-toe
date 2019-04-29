const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db  = admin.firestore();

exports.setgamestate = () => functions.https.onRequest((request, response) => {
    const gameid = request.query.gameuuid;
    const gamestate = {gameState: request.body};

    db.collection('games')
    .doc(gameid)
    .set(gamestate, {merge: true})
    .then(snapshot => {
        return response.send("yo");
    })
    .catch(error => {
        return response.send(error);
    });
});