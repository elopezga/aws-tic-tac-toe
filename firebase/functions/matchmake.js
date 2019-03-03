const functions = require('firebase-functions');

exports.match = () => functions.https.onRequest((request, response) => {
    const useruid = request.query.uuid;
    response.send(useruid);
});