const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db = admin.firestore();

exports.startgame = functions.https.onRequest((request, response) => {
    const owneruuid = request.query.owner;
    const joineruuid = request.query.joiner;

    db.collection('games')
        .add({
            players: [owneruuid, joineruuid],
            currentTurn: owneruuid,
            gameState: {
                topRow: createEmptyRow(),
                middleRow: createEmptyRow(),
                bottomRow: createEmptyRow()
            }
        })

        // Fire off to messaging that game created
});

function createEmptyRow(){
    return [
        createEmptyCell(),
        createEmptyCell(),
        createEmptyCell()
    ];
}

function createEmptyCell(){
    return {
        ownerid: '',
        piece: ''
    };
}