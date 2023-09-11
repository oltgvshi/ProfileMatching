import React, { Component } from 'react';
import './Home.css';
import axios from 'axios';
import { PropagateLoader } from 'react-spinners';
// Components
import Student from '../../components/Students/Student/Student';
import SearchStudents from '../../components/Students/SearchStudent/SearchStudents';
import UserCredentials from './UserCredentials';
import Sidebar from './Sidebar';

class Home extends Component {
  state = {
    data: null,
    allStudents: null,
    error: '',
    userId: '', // Add userId state
    userRole: '' // Add userRole state
  };

  async componentDidMount() {
    try {
      const students = await axios('/api/students/');
      this.setState({ data: students.data });
    } catch (err) {
      this.setState({ error: err.message });
    }
  }

  // removeStudent = async (id) => {
  //   try {
  //     const studentRemoved = await axios.delete(`/api/students/${id}`);
  //     const students = await axios('/api/students/');
  //     this.setState({ data: students.data });
  //   } catch (err) {
  //     this.setState({ error: err.message });
  //   }
  // };

  // searchStudents = async (username) => {
  //   let allStudents = [...this.state.data.students];
  //   if (this.state.allStudents === null) this.setState({ allStudents });

  //   let students = this.state.data.students.filter(
  //     ({ name }) => name.toLo`we`rCase().includes(username.toLowerCase())
  //   );
  //   if (students.length > 0) this.setState({ data: { students } });

  //   if (username.trim() === '')
  //     this.setState({ data: { students: this.state.allStudents } });
  // };

  setUserCredentials = (userId, userRole) => {
    this.setState({ userId, userRole });
  };

  render() {
    let students;

    if (this.state.data)
      students =
        this.state.data.students &&
        this.state.data.students.map((student) => (
          <Student key={student._id} {...student} removeStudent={this.removeStudent} />
        ));
    else
      return (
        <div className="Spinner-Wrapper">
          {' '}
          <PropagateLoader color={'#333'} />{' '}
        </div>
      );

    if (this.state.error) return <h1>{this.state.error}</h1>;
    if (this.state.data !== null)
      if (!this.state.data.students.length) return <h1 className="No-Students">No students!</h1>;

    return (
      <div className="Table-Wrapper">
        <UserCredentials setUserCredentials={this.setUserCredentials} /> {/* Pass setUserCredentials prop */}
        <Sidebar userId={this.state.userId} userRole={this.state.userRole} /> {/* Pass userId and userRole props */}
        
      </div>
    );
  }
}
export default Home;
