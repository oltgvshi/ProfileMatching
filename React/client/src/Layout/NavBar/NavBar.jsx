import React from 'react';
import './NavBar.css';
import { Link } from 'react-router-dom';

const Home = () => {
  return (
   <nav className="NavBar-Wrapper">
     <div>
     <Link to="/" className="NavBar-Link">JobMatch Connect</Link>
     </div>
     <div className="NavBar-Links">
      <Link to="/" className="NavBar-Link">Workspaces</Link>
      <a href="https://localhost:7044" className="NavBar-Link">Go to JobMatch</a>
     </div>
   </nav>
  );
};

export default Home;
