import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

const Sidebar = ({ userId, userRole }) => {
    const [jobPosts, setJobPosts] = useState([]);
    const [selectedJobPost, setSelectedJobPost] = useState('');
    const [workspaces, setWorkspaces] = useState([]);
    const [fullName, setFullName] = useState('');

    useEffect(() => {
        const fetchJobPosts = async () => {
            try {
                if (userRole === 'Client') {
                    const response = await axios.get('https://localhost:7044/api/ApplicantsPerJobs/GetJobpostsByClientId', {
                        mode: 'cors',
                        credentials: 'include'
                    });
                    const jobPostsData = response.data;

                    const matchingWorkspaces = workspaces.filter((workspace) =>
                        jobPostsData.some((jobPost) => jobPost.jobPostId.toString() === workspace.jobPostId)
                    );
                    setWorkspaces(matchingWorkspaces);

                    const unmatchedJobPosts = jobPostsData.filter(
                        (jobPost) => !matchingWorkspaces.some((workspace) => workspace.jobPostId === jobPost.jobPostId.toString())
                    );
                    setJobPosts(unmatchedJobPosts);
                } else if (userRole === 'Freelancer') {
                    const response = await axios.get('https://localhost:7044/api/ApplicantsPerJobs/getmyhiredjobs', {
                        mode: 'cors',
                        credentials: 'include'
                    });
                    const hiredJobsData = response.data;

                    const matchingWorkspaces = workspaces.filter((workspace) =>
                        hiredJobsData.some((jobPost) => jobPost.jobPostId.toString() === workspace.jobPostId)
                    );
                    setWorkspaces(matchingWorkspaces);
                }
            } catch (error) {
                console.error('Error:', error);
            }
        };

        fetchJobPosts();
    }, [userRole]);

    useEffect(() => {
        const fetchWorkspaces = async () => {
            try {
                const response = await axios.get('http://localhost:5000/api/workspaces');
                const workspacesData = response.data;
                setWorkspaces(workspacesData.workspaces);
            } catch (err) {
                console.log('Error:', err);
            }
        };

        fetchWorkspaces();
    }, []);

    useEffect(() => {
        const fetchFullName = async () => {
            try {
                const response = await axios.get('https://localhost:7044/api/jobpostsapi/GetFullName', {
                    params: {
                        userId: userId
                    }
                });
                const fullNameData = response.data;
                setFullName(fullNameData);
            } catch (error) {
                console.error('Error:', error);
            }
        };

        fetchFullName();
    }, [userId]);

    const handleJobPostChange = (event) => {
        setSelectedJobPost(event.target.value);
    };

    const addWorkspace = async (e) => {
        e.preventDefault();
        try {
            const jpId = selectedJobPost;
            const selectedJobPostData = jobPosts.find((jobPost) => jobPost.jobPostId == jpId);
            const jobPostName = selectedJobPostData.jobPostName;

            await axios.post('http://localhost:5000/api/workspaces/create-workspace', {
                jobPostId: jpId,
                jobPostName: jobPostName,
                userId: userId
            });

            setSelectedJobPost('');

            // Refresh the page to fetch updated workspaces
            window.location.reload();
        } catch (err) {
            console.log('Error:', err);
        }
    };

    return (
        <div style={{maxWidth:'1300px', width:'100%'}}>
            <p>Welcome {fullName}!</p>
            {jobPosts.length > 0 && (
                <>
                    <p>Add Workspace:</p>
                    <div className="input-group mb-3">
                        <select className="form-select" value={selectedJobPost} onChange={handleJobPostChange}>
                            <option value="">Select Job Post</option>
                            {jobPosts.map((jobPost) => (
                                <option key={jobPost.jobPostId} value={jobPost.jobPostId}>
                                    {jobPost.jobPostName}
                                </option>
                            ))}
                        </select>
                        <button className="btn btn-primary" onClick={addWorkspace} disabled={!selectedJobPost}>
                            Add
                        </button>
                    </div>
                </>
            )}

            <h1>Your Workspaces:</h1>
            {workspaces.length > 0 ? (
                <div style={{display:'grid', gridTemplateColumns:'1fr 1fr 1fr', gap:'20px'}}>
                    {workspaces.map((workspace) => (
                        <Link to={`/workspace/${workspace._id}/${userId}`}>
                        <div style={{
                            textAlign:'center',
                            backgroundColor: '#ffffff',
                            border: '1px solid #e9e9e9',
                            borderRadius: '5px',
                            padding: '50px 15px',
                            margin: '5px 0',
                            boxShadow: 'rgba(0, 0, 0, 0.1) 0px 0px 5px 0px, rgba(0, 0, 0, 0.1) 0px 0px 1px 0px',
                        }}>
                        <h3 key={workspace._id}>
                            {workspace.jobPostName}
                        </h3>
                        
                        </div>
                        </Link>
                        
                    ))}
                </div>
            ) : (
                <p>Nothing to show here</p>
            )}
        </div>
    );
};

export default Sidebar;
