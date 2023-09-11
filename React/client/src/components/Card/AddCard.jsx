import React, { useState } from 'react';
import axios from 'axios';

const AddCard = ({ match }) => {
  const [cardName, setCardName] = useState('');
  const [cardDescription, setCardDescription] = useState('');
  const [status, setStatus] = useState(false);
  const [deadline, setDeadline] = useState('');

  const boardId = match.params.boardId;
  const userId = match.params.userId;

  const handleCardNameChange = (event) => {
    setCardName(event.target.value);
  };

  const handleCardDescriptionChange = (event) => {
    setCardDescription(event.target.value);
  };

  const handleStatusChange = (event) => {
    setStatus(event.target.checked);
  };

  const handleDeadlineChange = (event) => {
    setDeadline(event.target.value);
  };

  const createCard = async (e) => {
    e.preventDefault();
    try {
      await axios.post('http://localhost:5000/api/cards', {
        boardId: boardId,
        cardName: cardName,
        cardDescription: cardDescription,
        userId: userId,
        status: status,
        deadline: deadline,
      });

      // Clear form inputs
      setCardName('');
      setCardDescription('');
      setStatus(false);
      setDeadline('');

      // Redirect to the workspace or any other appropriate page
      // Replace the below line with the desired redirection logic
      window.history.back();
    } catch (err) {
      console.log('Error:', err);
    }
  };


  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
    <div style={{ width: '400px', margin: 'auto', padding: '20px', borderRadius: '8px', boxShadow: '0 0px 10px rgba(0, 0, 0, 0.2)' }}>
        <h2 style={{ marginBottom: '20px', textAlign: 'center' }}>Add Card</h2>
        <form onSubmit={createCard}>
          <div style={{ marginBottom: '10px' }}>
            <label htmlFor="cardName" style={{ display: 'block', marginBottom: '5px' }}>
              Card Name:
            </label>
            <input
              type="text"
              id="cardName"
              value={cardName}
              onChange={handleCardNameChange}
              required
              style={{ padding: '5px', width: '100%', margin: 0 }}
            />
          </div>
          <div style={{ marginBottom: '10px' }}>
            <label htmlFor="cardDescription" style={{ display: 'block', marginBottom: '5px' }}>
              Card Description:
            </label>
            <textarea
              id="cardDescription"
              value={cardDescription}
              onChange={handleCardDescriptionChange}
              style={{ padding: '5px', minHeight: '100px', width: '100%' }}
            />
          </div>
          <div style={{ marginBottom: '10px' }}>
            <label htmlFor="status" style={{ marginRight: '10px' }}>
              Status:
            </label>
            <input
              style={{ display: 'inline'}}
              type="checkbox"
              id="status"
              checked={status}
              onChange={handleStatusChange}
            />
          </div>
          <div style={{ marginBottom: '10px' }}>
            <label htmlFor="deadline" style={{ marginRight: '10px' }}>
              Deadline:
            </label>
            <input
             style={{ display: 'inline-block'}}
              type="date"
              id="deadline"
              value={deadline}
              onChange={handleDeadlineChange}
            />
          </div>
          <button
            type="submit"
            style={{
              padding: '10px',
              backgroundColor: '#4CAF50',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer',
              width: '102%'
            }}
          >
            Create Card
          </button>
        </form>
      </div>
    </div>
  );
};

export default AddCard;