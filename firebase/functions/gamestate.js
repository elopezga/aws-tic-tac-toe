const functions = require('firebase-functions');
const admin = require('firebase-admin');

var db  = admin.firestore();

exports.setgamestate = () => functions.https.onRequest((request, response) => {
    const gameid = request.query.gameuuid;
    const gamestate = {gameState: request.body};

    // Analyze win or draw condition
    let winner = whoWon(request.body);
    let cellsTaken = allCellsTaken(request.body);
    let draw = false;

    if (!winner && cellsTaken)
    {
        draw = true;
    }

    let nextPlayerTurn = "";

    db.collection('games')
    .doc(gameid)
    .get()
    .then(doc => {
        let data = doc.data();
        nextPlayerTurn = (data.currentTurnId === data.xOwner) ? data.oOwner : data.xOwner;

        console.log(nextPlayerTurn);
        console.log("Winner: " + winner);
        console.log("Draw: " + draw);

        return db.collection('games')
            .doc(gameid)
            .set({
                currentTurnId: nextPlayerTurn,
                winner: winner,
                draw: draw,
                gameState: request.body
            }, {merge: true});
    })
    .then(snapshot => {
        return response.send("yo");
    })
    .catch(error => {
        return response.send(error);
    });
});

function allCellsTaken(gameState)
{
    const bottomRow = gameState.bottomRow;
    const middleRow = gameState.middleRow;
    const topRow = gameState.topRow;

    for (var i = 0; i < 3; i += 1)
    {
        if (!bottomRow[i].ownerid)
        {
            return false;
        }

        if (!middleRow[i].ownerid)
        {
            return false;
        }

        if (!topRow[i].ownerid)
        {
            return false;
        }
    }

    return true;
}

function whoWon(gameState){
    console.log(gameState);

    const bottomRow = gameState.bottomRow;
    const middleRow = gameState.middleRow;
    const topRow = gameState.topRow;

    let whoWon = "";

    // Check forward diagonal
    if (topRow[0].ownerid === middleRow[1].ownerid && bottomRow[2].ownerid)
    {
        if ( (topRow[0].ownerid === middleRow[1].ownerid) && (middleRow[1].ownerid === bottomRow[2].ownerid) )
        {
            whoWon = topRow[0].ownerid;
            return whoWon;
        }
    }

    // Check backward diagonal
    if (bottomRow[0].ownerid && middleRow[1].ownerid && topRow[2].ownerid)
    {
        if ( (bottomRow[0].ownerid === middleRow[1].ownerid) && (middleRow[1].ownerid === topRow[2].ownerid) )
        {
            whoWon = bottomRow[0].ownerid;
            return whoWon;
        }
    }

    // Check left column
    if (topRow[0].ownerid && middleRow[0].ownerid && bottomRow[0].ownerid)
    {
        if ( (topRow[0].ownerid === middleRow[0].ownerid) && (middleRow[0].ownerid === bottomRow[0].ownerid) )
        {
            whoWon = topRow[0].ownerid;
            return whoWon;
        }
    }

    // Check middle column
    if (topRow[1].ownerid && middleRow[1].ownerid && bottomRow[1].ownerid)
    {
        if ( (topRow[1].ownerid === middleRow[1].ownerid) && (middleRow[1].ownerid === bottomRow[1].ownerid) )
        {
            whoWon = topRow[1].ownerid;
            return whoWon;
        }
    }

    // Check right column
    if (topRow[2].ownerid && middleRow[2].ownerid && bottomRow[2].ownerid)
    {
        if ( (topRow[2].ownerid === middleRow[2].ownerid) && (middleRow[2].ownerid === bottomRow[2].ownerid) )
        {
            whoWon = topRow[2].ownerid;
            return whoWon;
        }
    }

    // Check top row
    if (topRow[0].ownerid && topRow[1].ownerid && topRow[2].ownerid)
    {
        if ( (topRow[0].ownerid === topRow[1].ownerid) && (topRow[1].ownerid === topRow[2].ownerid) )
        {
            whoWon = topRow[0];
            return whoWon;
        }
    }

    // Check middle row
    if (middleRow[0].ownerid && middleRow[1].ownerid && middleRow[2].ownerid)
    {
        if ( (middleRow[0].ownerid === middleRow[1].ownerid) && (middleRow[1].ownerid === middleRow[2].ownerid) )
        {
            whoWon = middleRow[0];
            return whoWon;
        }
    }

    // Check bottom row
    if (bottomRow[0].ownerid && bottomRow[1].ownerid && bottomRow[2])
    {
        if ( (bottomRow[0].ownerid === bottomRow[1].ownerid) && (bottomRow[1].ownerid === bottomRow[2].ownerid))
        {
            whoWon = bottomRow[0];
            return whoWon;
        }
    }

    return whoWon;
}