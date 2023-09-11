const mongoose = require('mongoose');

const boardSchema = new mongoose.Schema({
    workspaceId: {
        type: String,
    },
    boardName: {
        type: String,
    },
});

module.exports = mongoose.model('Board', boardSchema);