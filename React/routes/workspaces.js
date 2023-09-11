const express = require('express');
const router = express.Router();

// Workspace model
const Workspace = require('../models/workspaces');

// @route   GET /api/workspaces/
// @desc    Get all workspaces
// @access  Public
router.get('/', async (req, res) => {
  try {
    const workspaces = await Workspace.find({});
    res.send({ workspaces });
  } catch (err) {
    res.status(400).send({ error: err });
  }
});

// @route   GET /api/workspaces/:id
// @desc    Get a specific workspace
// @access  Public
router.get('/:id', async (req, res) => {
  try {
    const workspace = await Workspace.findById(req.params.id);
    res.send({ workspace });
  } catch (err) {
    res.status(404).send({ message: 'Workspace not found!' });
  }
});

// @route   POST /api/workspaces/
// @desc    Create a workspace
// @access  Public
// Create workspace
// Create workspace
router.post('/create-workspace', async (req, res) => {
  try {
    const { jobPostId, jobPostName, userId } = req.body;
    const newWorkspace = await Workspace.create({ jobPostId, jobPostName, userId });
    res.json(newWorkspace);
  } catch (err) {
    res.status(400).json({ error: err });
  }
});



// @route   PUT /api/workspaces/:id
// @desc    Update a workspace
// @access  Public
router.put('/:id', async (req, res) => {
  try {
    const updatedWorkspace = await Workspace.findByIdAndUpdate(req.params.id, req.body);
    res.send({ message: 'The workspace was updated' });
  } catch (err) {
    res.status(400).send({ error: err });
  }
});

// @route   DELETE /api/workspaces/:id
// @desc    Delete a workspace
// @access  Public
router.delete('/:id', async (req, res) => {
  try {
    const removeWorkspace = await Workspace.findByIdAndRemove(req.params.id);
    res.send({ message: 'The workspace was removed' });
  } catch (err) {
    res.status(400).send({ error: err });
  }
});

module.exports = router;
