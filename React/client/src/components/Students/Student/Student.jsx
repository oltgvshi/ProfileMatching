import './Student.css';
import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import Avatar from 'react-avatar';


const Student = ({ _id, name, email, enrollnumber, removeStudent }) => {
  return(
    <tr>
      <td>{ name }</td>
      <td>{ email }</td>
      <td>{ enrollnumber }</td>
      <td>
        <button onClick={ () => removeStudent(_id) } className="Action-Button fa fa-trash"></button>
        <Link to={{ pathname: '/editStudent', search: _id }}>
         <button className="Action-Button fa fa-pencil"></button>
        </Link>
      </td>

    </tr>
  );
};

export default Student;
