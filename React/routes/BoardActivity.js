const express = require('express');
const router = express.Router();

// BoardActivity model
const BoardActivity = require('../models/BoardActivity'); 

// @route   GET /api/BoardActivity/
// @desc    Get all BoardActivities
// @access  Public
router.get('/', async (req, res) => {
  try {
    const BoardActivity = await BoardActivity.find({});
    res.send({ BoardActivity })
  } catch(err) {
    res.status(400).send({ error: err });
  }
});

// @route   GET /api/BoardActivity/:id
// @desc    Get a specific BoardActivity
// @access  Public
router.get('/:id', async (req, res) => {
  try {
    const BoardActivity = await BoardActivity.findById(req.params.id);
    res.send({ BoardActivity });
  } catch (err) {
    res.status(404).send({ message: 'BoardActivity not found!' });
  }
});

// @route   POST /api/BoardActivity/
// @desc    Create a BoardActivity
// @access  Public
router.post('/', async (req, res) => {
  try {
    const newBoardActivity = await BoardActivity.create({ name: req.body.name, email: req.body.email, enrollnumber: req.body.enrollnumber });
     res.send({ newBoardActivity });
  } catch(err) {
    res.status(400).send({ error: err });
  }

});

// @route   PUT /api/BoardActivity/:id
// @desc    Update a BoardActivity
// @access  Public
router.put('/:id', async (req, res) => {
  try {
    const updatedBoardActivity = await BoardActivity.findByIdAndUpdate(req.params.id, req.body);
     res.send({ message: 'The BoardActivity was updated' });
  } catch(err) {
    res.status(400).send({ error: err });
  }
});

// @route   DELETE /api/BoardActivity/:id
// @desc    Delete a BoardActivity
// @access  Public
router.delete('/:id', async (req, res) => {
  try {
    const removeBoardActivity = await BoardActivity.findByIdAndRemove(req.params.id);
     res.send({ message: 'The BoardActivity was removed' });
  } catch(err) {
    res.status(400).send({ error: err });
  }
});


module.exports = router;