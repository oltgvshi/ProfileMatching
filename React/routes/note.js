const express = require('express');
const router = express.Router();

// note model
const note = require('../models/note');

// @route   GET /api/note/
// @desc    Get all notes
// @access  Public
router.get('/', async (req, res) => {
  try {
    const note = await note.find({});
    res.send({ note })
  } catch(err) {
    res.status(400).send({ error: err });
  }
});

// @route   GET /api/note/:id
// @desc    Get a specific note
// @access  Public
router.get('/:id', async (req, res) => {
  try {
    const note = await note.findById(req.params.id);
    res.send({ note });
  } catch (err) {
    res.status(404).send({ message: 'note not found!' });
  }
});

// @route   POST /api/note/
// @desc    Create a note
// @access  Public
router.post('/', async (req, res) => {
  try {
    const newnote = await note.create({ workspaceId: req.body.workspaceId, noteText: req.body.noteText, userId: req.body.userId });
     res.send({ newnote });
  } catch(err) {
    res.status(400).send({ error: err });
  }

});

//look for exisiting note
router.get('/forWorkspaceAndUser/:workspaceId/:userId', async (req, res) => {
  try {
    const notes = await note.find({
      workspaceId: req.params.workspaceId,
      userId: req.params.userId,
    });
    res.send({ notes });
  } catch (err) {
    res.status(404).send({ message: 'Notes not found!' });
  }
});


// @route   PUT /api/note/:id
// @desc    Update a note
// @access  Public
router.put('/:id', async (req, res) => {
  try {
    const updatednote = await note.findByIdAndUpdate(req.params.id, req.body);
     res.send({ message: 'The note was updated' });
  } catch(err) {
    res.status(400).send({ error: err });
  }
});

// @route   DELETE /api/note/:id
// @desc    Delete a note
// @access  Public
router.delete('/:id', async (req, res) => {
  try {
    const removenote = await note.findByIdAndRemove(req.params.id);
     res.send({ message: 'The note was removed' });
  } catch(err) {
    res.status(400).send({ error: err });
  }
});


module.exports = router;