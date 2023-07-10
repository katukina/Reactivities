import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Activity } from '../models/activity';
import NavBar from './NavBar';
import { Container } from 'semantic-ui-react';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import {v4 as uuid} from 'uuid'

function App() {
  //KP variable to store the acitvities and fucntion to set the activities
  //in useStape we can put a type, in here Activity interface array added in model
  const [activities, setActivities] = useState<Activity[]>([]); // start with empty array
  const [selectedActivity, setselectedActivity] = useState<Activity | undefined>(undefined);
  const [editMode, setEditMode] = useState(false);

  function handleSelectActivity(id: string){
    setselectedActivity(activities.find(x => x.id === id));
  }

  function handleCancelSelectActivity(){
    setselectedActivity(undefined);
  }

  function handleFormOpen(id?: string){
    id ? handleSelectActivity(id) : handleCancelSelectActivity();
    setEditMode(true);
   }

  function handleFormClose(){
    setEditMode(false);
  }

  //Firt check if we actually have an activity ID to know if are creating ot editing
  //...(spread operator)to loop over our existing activities 
  //in the alternative case don't have id then it is new activity by adding our new activity to the array of activities
  //Updating locally not in API
  function handleCreateOrEditActivity(activity: Activity) {
    activity.id ? 
    setActivities([...activities.filter(x => x.id !== activity.id), activity])
    : setActivities([...activities, {...activity, id: uuid()}]);
    setEditMode(false);
    setselectedActivity(activity);
  }

  function handleDeleteActivity(id: string) {
    setActivities([...activities.filter(x => x.id !== id)]);
  }

  //what we do when app loads up
  useEffect(() => {
    //get data from API
    //Here also we can speacify the type of we are going to receive
    axios.get<Activity[]>('http://localhost:5000/api/activities')
    .then(response =>{      
      setActivities(response.data);
    })
  }, []) // KP the [] there is to do that once

  return (
    <>
        <NavBar openForm={handleFormOpen}/>
        <Container style={{ marginTop: '7em' }}>
          <ActivityDashboard activities={activities} selectedActivity={selectedActivity} 
          selectActivity={handleSelectActivity} cancelActivity={handleCancelSelectActivity} 
          editMode={editMode} openForm={handleFormOpen} closeForm={handleFormClose}
          createOrEditActivity={handleCreateOrEditActivity} deleteActivity={handleDeleteActivity}
          />
        </Container>     
    </>
  );
}

export default App;
