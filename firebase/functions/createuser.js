const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db = admin.firestore();

exports.createuser = () => functions.auth.user().onCreate(user => {
    // What about if user already exists?
    return db.collection('players')
    .doc(user.uid)
    .set({
        games: [],
        rooms: []
    })
    .catch( error => {
        response.send(error);
    })
});

exports.createGoogleUser = () => functions.https.onRequest((request, response) => {
    const uuid = request.query.uuid;

    var addedUser = false;

    db.collection('players')
    .doc(uuid)
    .get()
    .then( doc => {
        return new Promise((resolve, reject) => {
            let addUser = false;

            if (!doc.exists)
            {
                addUser = true;
            }
            resolve(addUser);
        });
    })
    .then( addUser => {

        if (addUser)
        {
            db.collection('players').doc(uuid).set({
                games: [],
                rooms: []
            });
            
            return response.send(true);
        }
        else
        {
            return response.send(false);
        }

    })
    .catch( error => {
        return response.send(error);
    });
});