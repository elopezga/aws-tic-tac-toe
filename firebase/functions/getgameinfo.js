const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db = admin.firestore();

exports.getgameinfo = () => functions.https.onRequest((request, response) => {
    const gameid = request.query.game;

    db.collection('games')
    .doc(gameid)
    .get()
    .then( snapshot => {
        return response.send(JSON.stringify(snapshot.data()));
    })
    .catch( error => {
        return response.send(error);
    });
});