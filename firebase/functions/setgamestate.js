const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db  = admin.firestore();

exports.setgamestate = () => functions.https.onRequest((request, response) => {
    const gameid = request.query.gameuuid;
    const gamestate = request.body;
    console.log(gameid);
    console.log(gamestate);
    /*
        {

        }
    */
   response.send("yo");
});