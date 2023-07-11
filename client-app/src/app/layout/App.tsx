import { useEffect } from 'react';
import NavBar from './NavBar';
import { Container } from 'semantic-ui-react';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import { useStore } from '../stores/store';
import { observer } from 'mobx-react-lite';
import LoadingComponent from './LoadingComponent';

function App() {
  const { activityStore } = useStore();

  //what we do when app loads up
  useEffect(() => {
    activityStore.loadActivities();
  }, [activityStore])

  if (activityStore.loadingInitial) return <LoadingComponent content='Loading app...' />

  return (
    <>
        <NavBar/>
        <Container style={{ marginTop: '7em' }}>
          <ActivityDashboard />
        </Container>     
    </>
  );
}

export default observer(App);
