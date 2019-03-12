const functions = require('firebase-functions');
const admin = require('firebase-admin');

admin.initializeApp(functions.config().firebase);

// Other functions
const matchmake = require('./matchmake');
const getrooms = require('./getrooms');
const startgame = require('./startgame');
const getPlayerInfo = require('./getPlayerInfo');
const getgameinfo = require('./getgameinfo');
const createuser = require('./createuser');

// // Create and Deploy Your First Cloud Functions
// // https://firebase.google.com/docs/functions/write-firebase-functions
//
exports.helloWorld = functions.https.onRequest((request, response) => {
  response.send("Hello from Firebase!");
});

exports.matchmake = matchmake.match();
exports.getrooms = getrooms.getrooms();
exports.startgame = startgame.startGameHttps();
exports.deletegame = startgame.deleteGameHttps();
exports.getPlayerInfo = getPlayerInfo.getPlayerInfo();
exports.getturns = getPlayerInfo.getTurns();
exports.getPlayerMenuInfo = getPlayerInfo.getPlayerMenuInfo();
exports.getgameinfo = getgameinfo.getgameinfo();
exports.createuser = createuser.createuser();