import React, { useEffect, useState } from 'react';

const UserCredentials = ({ setUserCredentials }) => {
  const [userId, setUserId] = useState('');

  useEffect(() => {
    const fetchUserCredentials = async () => {
      try {
        const response = await fetch('https://localhost:7044/api/jobpostsapi/getUserCredentials', {
          mode: 'cors',
          credentials: 'include'
        });

        if (response.ok) {
          const data = await response.json();
          const userId = data[0].userId;
          const userRole = data[0].userRole;
          console.log('User:', userId);
          console.log('Role:', userRole);
          setUserCredentials(userId, userRole);
        } else if (response.status === 404) {
          window.location.href = 'https://localhost:7044/Identity/Account/Login';
        } else {
          throw new Error('Error fetching user credentials');
        }
      } catch (error) {
        console.error('Error:', error);
      }
    };

    fetchUserCredentials();
  }, [setUserCredentials]);

  return null;
};

export default UserCredentials;