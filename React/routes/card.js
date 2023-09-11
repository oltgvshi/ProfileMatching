const express = require('express');
const router = express.Router();

// card model
const card = require('../models/card');

// @route   GET /api/card/
// @desc    Get all cards
// @access  Public
router.get('/', async (req, res) => {
  try {
    const card = await card.find({});
    res.send({ card })
  } catch(err) {
    res.status(400).send({ error: err });
  }
});

// @route   GET /api/card/:id
// @desc    Get a specific card
// @access  Public
router.get('/:id', async (req, res) => {
  try {
    const card = await card.findById(req.params.id);
    res.send({ card });
  } catch (err) {
    res.status(404).send({ message: 'card not found!' });
  }
});

// @route   POST /api/card/
// @desc    Create a card
// @access  Public
router.post('/', async (req, res) => {
  try {
    const newcard = await card.create({ boardId: req.body.boardId, cardName: req.body.cardName, cardDescription: req.body.cardDescription, userId: req.body.userId, status: req.body.status, deadline: req.body.deadline });
     res.send({ newcard });
  } catch(err) {
    res.status(400).send({ error: err });
  }

});

router.get('/getByBoard/:boardId', async (req, res) => {
  try {
    const boardId = req.params.boardId;
    const cards = await card.find({ boardId: boardId });
    res.send({ cards });
  } catch (err) {
    res.status(400).send({ error: err });
  }
});


// @route   PUT /api/card/:id
// @desc    Update a card
// @access  Public
router.put('/:id', async (req, res) => {
  try {
    const updatedcard = await card.findByIdAndUpdate(req.params.id, req.body);
     res.send({ message: 'The card was updated' });
  } catch(err) {
    res.status(400).send({ error: err });
  }
});

// @route   DELETE /api/card/:id
// @desc    Delete a card
// @access  Public
router.delete('/:id', async (req, res) => {
  try {
    const removecard = await card.findByIdAndRemove(req.params.id);
     res.send({ message: 'The card was removed' });
  } catch(err) {
    res.status(400).send({ error: err });
  }
});


module.exports = router;