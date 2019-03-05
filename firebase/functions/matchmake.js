const functions = require('firebase-functions');
const admin = require('firebase-admin');

admin.initializeApp(functions.config().firebase);

var db = admin.firestore();

exports.match = () => functions.https.onRequest((request, response) => {

    const uuid = request.query.uuid;

    // 1. Check the oldest room
    //      if no room, create room
    //      else, join that room

    getRoomsByOldestFirst()
        .then( snapshot => {
            if( snapshot.empty)
            {
                // No room, create one
                db.collection('rooms')
                    .add({
                        players: [uuid],
                        created: admin.firestore.FieldValue.serverTimestamp(),
                        isFull: false
                    });
                console.log("Created new room for " + uuid);
                return "Created new room for " + uuid;
            }
            else
            {
                // We got a room, lets join
                // Todo: Check that the same player is not joining a room that s/he created
                let joinedRoom = false;

                for(var doc of snapshot.docs)
                {
                    let room = doc.data();

                    // Prevent player from joining a game they started
                    // Or don't join if room is full
                    if (room.players.includes(uuid) || room.isFull)
                    {
                        continue;
                    }

                    // Add player to room
                    // This should be a transaction to prevent more than 2 people joining
                    db.collection('rooms').doc(doc.id)
                        .update({
                            players: admin.firestore.FieldValue.arrayUnion(uuid),
                            isFull: true
                        });
                    joinedRoom = true;
                    break;
                }

                // If was not able to join a room, create a room
                if(!joinedRoom)
                {
                    db.collection('rooms')
                        .add({
                            players: [uuid],
                            created: admin.firestore.FieldValue.serverTimestamp(),
                            isFull: false
                        });
                    console.log("Created new room for " + uuid);
                    return "Created new room for " + uuid;
                }
  
                console.log("Joining room " + snapshot.docs[0].id);
                return "Joining room " + snapshot.docs[0].id;
            }
        })
        .then( message => {
            return response.send(message);
        })
        .catch( error => {
            response.send(error);
        });
});

function getRoomsByOldestFirst()
{
    return db.collection('rooms')
        .orderBy('created', 'asc')
        .get();
}