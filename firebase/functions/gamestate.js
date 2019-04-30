const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db  = admin.firestore();

exports.setgamestate = () => functions.https.onRequest((request, response) => {
    const gameid = request.query.gameuuid;
    const gamestate = {gameState: request.body};

    let nextPlayerTurn = "";

    db.collection('games')
    .doc(gameid)
    .get()
    .then(doc => {
        let data = doc.data();
        nextPlayerTurn = (data.currentTurnId === data.xOwner) ? data.oOwner : data.xOwner;

        console.log(nextPlayerTurn);

        return db.collection('games')
            .doc(gameid)
            .set({
                currentTurnId: nextPlayerTurn,
                gameState: request.body
            }, {merge: true});
    })
    .then(snapshot => {
        return response.send("yo");
    })
    .catch(error => {
        return response.send(error);
    });
});