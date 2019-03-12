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
    });
});