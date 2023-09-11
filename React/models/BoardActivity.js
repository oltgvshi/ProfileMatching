const mongoose = require('mongoose');

const BoardActivitySchema = new mongoose.Schema({
    Id: {
    type: String,
    required: true,
  },
    UserId: {
    type: String,
  },
    Date: {
    type: String,
  },
  bActivity:{
    type:String,
  }
});

module.exports = mongoose.model('BoardActivity', BoardActivitySchema);
