const mongoose = require('mongoose');

const cardSchema = new mongoose.Schema({
    boardId: {
    type: String,
    required: true,
  },
   cardName: {
    type: String,
  },
   cardDescription: {
    type: String,
  },
    userId: {
    type: String,
  },
    status: {
    type: Boolean,
  },
    deadline: {
    type: Date,
  },
});

module.exports = mongoose.model('Card', cardSchema);