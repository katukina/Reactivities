import React, { useEffect, useState } from 'react';
import logo from './logo.svg';
import './App.css';
import axios from 'axios';
import { Header, List } from 'semantic-ui-react';

function App() {
  //KP variable to store the acitvities and fucntion to set the activities
  const [activities, setActivities] = useState([]); // start with empty array

  //what we do when app loads up
  useEffect(() => {
    //get data from API
    axios.get('http://localhost:5000/api/activities')
    .then(response =>{      
      setActivities(response.data);
    })
  }, []) // KP the [] there is to do that once

  return (
    <div>
        <Header as = 'h2' icon='users' content='Reactivities'/>        
        <List>
            {activities.map((activity: any) => (
            <List.Item key={activity.id}>
            {activity.title}
            </List.Item>
          ))}
        </List>
    </div>
  );
}

export default App;
