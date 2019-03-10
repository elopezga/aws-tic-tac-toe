const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db = admin.firestore();

exports.startGameHttps = () => functions.https.onRequest((request, response) => {
    const owneruuid = request.query.owner;
    const joineruuid = request.query.joiner;

    startGame(owneruuid, joineruuid)
        .then( () => {
            return response.send("Created game");
        })
        .catch( error => {
            response.send(error);
        });

        // Fire off to messaging that game created

});

exports.startGame = function(owner, joiner)
{
    
    return db.collection('games')
        .add({
            players: [owner, joiner],
            currentTurn: owner,
            gameState: {
                topRow: createEmptyRow(),
                middleRow: createEmptyRow(),
                bottomRow: createEmptyRow()
            }
        })
        .then( docRef => {
            db.collection('players')
                .doc(owner)
                .update({
                    games: admin.firestore.FieldValue.arrayUnion(docRef.id)
                });
            db.collection('players')
                .doc(joiner)
                .update({
                    games: admin.firestore.FieldValue.arrayUnion(docRef.id)
                });
            return;
        });
    
}

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