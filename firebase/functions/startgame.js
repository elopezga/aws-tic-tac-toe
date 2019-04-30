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

exports.deleteGameHttps = () => functions.https.onRequest((request, response) => {
    /* const gameId = request.query.game;

    db.collection('games')
    .doc(gameId)
    .delete()
    .then( docRef => {
        db.collection('players')
        .where('games', 'array-contains', gameId)
        .get()
        .then(snapshot => {
            snapshot.forEach(doc => {
                doc.get("games")
            });
        })

        return response.send("Deleted game " + gameId);
    })
    .catch( error => {
        response.send(error);
    })
 */
});

exports.startGame = function(owner, joiner)
{
    
    return db.collection('games')
        .add({
            players: [owner, joiner],
            currentTurnId: owner,
            xOwner: owner,
            oOwner: joiner,
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
                    games: admin.firestore.FieldValue.arrayUnion({gameid: docRef.id, turn: owner})
                });
            db.collection('players')
                .doc(joiner)
                .update({
                    games: admin.firestore.FieldValue.arrayUnion({gameid: docRef.id, turn: owner})
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