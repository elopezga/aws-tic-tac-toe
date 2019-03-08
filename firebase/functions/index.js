const functions = require('firebase-functions');

// Other functions
const matchmake = require('./matchmake');
const getrooms = require('./getrooms');

// // Create and Deploy Your First Cloud Functions
// // https://firebase.google.com/docs/functions/write-firebase-functions
//
exports.helloWorld = functions.https.onRequest((request, response) => {
  response.send("Hello from Firebase!");
});

exports.matchmake = matchmake.match();
exports.getrooms = getrooms.getrooms();