const mongoose = require('mongoose');

const noteSchema = new mongoose.Schema({
  workspaceId: {
    type: String,
  },
  noteText: {
    type: String,
  },
  userId: {
    type: String,
  },
});

module.exports = mongoose.model('Note', noteSchema);
