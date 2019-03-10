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