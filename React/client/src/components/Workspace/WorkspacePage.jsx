import React, { useState, useEffect } from 'react';
import axios from 'axios';
import AddCard from '../Card/AddCard';
import { Link } from 'react-router-dom';
import Note from './Note';

const WorkspacePage = ({ match }) => {
  const [workspace, setWorkspace] = useState(null);
  const [showCreateBoard, setShowCreateBoard] = useState(false);
  const [boardName, setBoardName] = useState('');
  const [matchingBoards, setMatchingBoards] = useState([]);
  const [addingCard, setAddingCard] = useState(null);
  const [cardsByBoard, setCardsByBoard] = useState({});
  const [clientName, setClientName] = useState('');
  const [memberNames, setMemberNames] = useState([]);
  const workspaceId = match.params.id;
  const userId = match.params.userId;
  const [showContent, setShowContent] = useState(false);

  const handleHover = () => {
    setShowContent(true);
  };

  const handleLeave = () => {
    setShowContent(false);
  };

  useEffect(() => {
    const draggables = document.querySelectorAll(".task");
    const droppables = document.querySelectorAll(".swim-lane");

    draggables.forEach((task) => {
      task.addEventListener("dragstart", () => {
        task.classList.add("is-dragging");
      });
      task.addEventListener("dragend", () => {
        task.classList.remove("is-dragging");
      });
    });

    droppables.forEach((zone) => {
      zone.addEventListener("dragover", (e) => {
        e.preventDefault();
    
        const bottomTask = insertAboveTask(zone, e.clientY);
        const curTask = document.querySelector(".is-dragging");
    
        if (!bottomTask) {
          zone.insertBefore(curTask, zone.lastChild);

        } else {
          zone.insertBefore(curTask, bottomTask);
        }
      });
    });

    const insertAboveTask = (zone, mouseY) => {
      const els = zone.querySelectorAll(".task:not(.is-dragging)");
    
      let closestTask = null;
      let closestOffset = Number.NEGATIVE_INFINITY;
    
      els.forEach((task) => {
        const { top } = task.getBoundingClientRect();
    
        const offset = mouseY - top;
    
        if (offset < 0 && offset > closestOffset) {
          closestOffset = offset;
          closestTask = task;
        }
      });
    
      return closestTask;
    };

  }, []);

  useEffect(() => {
    const fetchWorkspace = async () => {
      try {
        const response = await axios.get(`http://localhost:5000/api/workspaces/${workspaceId}`);
        const workspaceData = response.data;
        console.log('Workspace Data:', workspaceData);
        setWorkspace(workspaceData.workspace);
        const userId = workspaceData.workspace.userId;
        const jobPostId = workspaceData.workspace.jobPostId;
        const workspaceMembers = await axios.get(`https://localhost:7044/api/ApplicantsPerJobs/GetHiredApplicantsForJobPost/${jobPostId}`);
        const memberNames = workspaceMembers.data.map((member) => `${member.firstName} ${member.lastName}`);
        console.log(memberNames);
        setMemberNames(memberNames);
        const clientResponse = await axios.get(`https://localhost:7044/api/jobpostsapi/getjobclientname/${userId}`);
        const clientData = clientResponse.data;
        setClientName(clientData);
      } catch (error) {
        console.error('Error:', error);
      }
    };

    fetchWorkspace();
  }, [workspaceId]);

  useEffect(() => {
    const fetchMatchingBoards = async () => {
      try {
        const response = await axios.get(`http://localhost:5000/api/boards/forWorkspace/${workspaceId}`);
        const matchingBoardsData = response.data;
        console.log('Matching Boards Data:', matchingBoardsData);
        setMatchingBoards(matchingBoardsData.boards);

        const cardsData = await Promise.all(
          matchingBoardsData.boards.map(async (board) => {
            const cardResponse = await axios.get(`http://localhost:5000/api/cards/getByBoard/${board._id}`);
            return { boardId: board._id, cards: cardResponse.data.cards };
          })
        );

        const cardsByBoardData = {};
        cardsData.forEach((cardsObj) => {
          cardsByBoardData[cardsObj.boardId] = cardsObj.cards;
        });
        setCardsByBoard(cardsByBoardData);
      } catch (error) {
        console.error('Error:', error);
      }
    };

    fetchMatchingBoards();
  }, [workspaceId]);

  const handleBoardNameChange = (event) => {
    setBoardName(event.target.value);
  };

  const toggleCreateBoard = () => {
    setShowCreateBoard(!showCreateBoard);
  };

  const createBoard = async (e) => {
    e.preventDefault();
    try {
      await axios.post('http://localhost:5000/api/boards', {
        workspaceId: workspaceId,
        boardName: boardName,
      });
      setBoardName('');

      const matchingResponse = await axios.get(`http://localhost:5000/api/boards/forWorkspace/${workspaceId}`);
      const matchingBoardsData = matchingResponse.data;
      setMatchingBoards(matchingBoardsData.boards);
    } catch (err) {
      console.log('Error:', err);
    }
  };

  const addCard = async (boardId) => {
    setAddingCard(boardId);
  };

  const handleCardAdded = async () => {
    const matchingResponse = await axios.get(`http://localhost:5000/api/boards/forWorkspace/${workspaceId}`);
    const matchingBoardsData = matchingResponse.data;
    setMatchingBoards(matchingBoardsData.boards);
    setAddingCard(null);
  };

  if (!workspace) {
    return <p>Loading workspace...</p>;
  }

  return (
    <div>
      <div style={{ display:'flex',justifyContent:'space-between',alignItems:'center' }}>
        <h2 style={{ margin: '20px 20px'}}>{workspace.jobPostName} Workspace </h2>

        <div style={{ marginRight: '20px', position: 'relative' }}>
          <p
            style={{
              fontWeight: 'bold',
              cursor: 'pointer'
            }}
            onMouseEnter={handleHover}
            onMouseLeave={handleLeave}
          >
            Workspace Members
          </p>
          {showContent && (
            <div
              style={{
                position: 'absolute',
                top: '100%',
                left: 0,
                backgroundColor: '#fff',
                boxShadow: '0px 4px 8px rgba(0, 0, 0, 0.1)',
                padding: '10px',
                fontWeight: 'normal',
                zIndex: 1
              }}
            >
              <p>Job Client: {clientName}</p>
              <p>Members: {memberNames.join(', ')}</p>
            </div>
          )}
      </div>


      </div>

     <div style={{ display: 'flex'}}>
      <h3 style={{marginLeft:'20px',}}>Workspace Boards</h3>
      <h3 style={{ display: 'flex', alignItems: 'center', marginLeft:'20px', }}>
        <button
          onClick={toggleCreateBoard}
          style={{ cursor: 'pointer', backgroundColor: '#6200ea', border: 'none', color:'#FFFFFF', boxSizing:'border-box', padding:'12px 16px', borderRadius:'4px', marginTop: '-5px' }}
        >
          + Add Board
        </button>
        {showCreateBoard && (
          <form onSubmit={createBoard} style={{ display: 'flex', alignItems: 'center', marginLeft: '10px' }}>
            <input
              type="text"
              value={boardName}
              onChange={handleBoardNameChange}
              placeholder="Board Name"
              style={{ padding: '12px 16px', margin:'0px 5px 0px 0px', borderRadius:'4px' }}
            />
            <button type="submit" style={{cursor: 'pointer', backgroundColor: '#6200ea', border: 'none', color:'#FFFFFF', boxSizing:'border-box', padding:'12px 16px', borderRadius:'4px'}}>
              Create
            </button>
          </form>
        )}
      </h3>
      </div>

      <div className='bodyContainer' style={{ display: 'flex'}}>
        <div className='rightContainer'>
        <div className='lanes' style={{ display: 'flex', flexWrap: 'wrap', margin: '0px 20px' }}>
        {matchingBoards.map((board) => (
          <div className='swim-lane'
            key={board.id}
            style={{
              backgroundColor: '#f1f2f4',
              padding: '20px',
              margin: '10px',
              width: '250px',
              borderRadius: '10px',
            }}
          >
            <h3 style={{marginTop:'0px'}}>{board.boardName}</h3>
            {/* <div className='cardHandler'> */}
              {cardsByBoard[board._id] &&
                cardsByBoard[board._id].map((card) => (
                  <div className='task' draggable="true"
                    key={card._id}
                    style={{
                      backgroundColor: '#ffffff',
                      border: '1px solid #e9e9e9',
                      borderRadius: '5px',
                      padding: '5px 15px',
                      margin: '5px 0',
                      boxShadow: 'rgba(0, 0, 0, 0.1) 0px 0px 5px 0px, rgba(0, 0, 0, 0.1) 0px 0px 1px 0px',
                      cursor: 'move',
                    }}
                  >
                    <div style={{display:'flex',justifyContent:'space-between',alignItems:'center',marginTop:'0.5em',marginBottom:'1em'}}>
                    <h3 style={{ fontWeight: 'bold', margin:'0'}}>{card.cardName}</h3> 
                    
                    <p style={{backgroundColor:'#6200ea', marginTop:'0',marginBottom:'0',display:'inline-block',color:'#ffffff', width:'25px',height:'25px',display:'flex',justifyContent:'center',alignItems:'center',borderRadius:'100px'}}>{card.status ? '✔' : '✖'}</p>
                    </div>
                    <p>{card.cardDescription}</p>
                    
                    <hr style={{marginTop:'20px', marginBottom:'5px'}} />
                    <div style={{display:'flex', justifyContent:'space-between'}}>
                    <p style={{ fontWeight: 'bold', marginBottom: '5px' }}>Deadline:</p>
                    <p>{new Date(card.deadline).toLocaleDateString()}</p>
                    
                    </div>
                  </div>
                ))}
            {/* </div> */}
            {addingCard === board.id ? (
              <AddCard boardId={board._id} onCardAdded={handleCardAdded} />
            ) : (
              <Link to={`/addcard/${board._id}/${userId}`}
              style={{cursor: 'pointer', backgroundColor: 'transparent', border: '2px solid #6200ea', color:'#6200ea', boxSizing:'border-box', padding:'12px 16px', borderRadius:'4px'
                      , display:'inline-block', width:'100%', marginTop:'15px'}}>+ Add Card</Link>
            )}
          </div>
          
        ))}
      </div>
        </div>

        <div className='leftContainer'>
        <h3>Your Personal Notes: </h3>
        <Note workspaceId={workspaceId} userId={userId} />
        </div>

      </div>
      
    
    </div>
    
    
  );
};

export default WorkspacePage;