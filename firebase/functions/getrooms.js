const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db = admin.firestore();

exports.getrooms = () => functions.https.onRequest((request, response) => {
    const uuid = request.query.uuid;

    db.collection('rooms')
        .where('players', 'array-contains', uuid)
        .get()
        .then( snapshot => {
            let rooms = [];
            snapshot.forEach( doc => {
                let roomData = {
                    id: doc.id,
                    players: doc.get("players"),
                    isFull: doc.get("isFull"),
                    created: doc.get("created")
                };
                rooms.push(roomData);
            });

            return response.send(JSON.stringify(rooms));
        })
        .catch( error => {
            console.log(error);
        });
});