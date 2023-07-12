import NavBar from './NavBar';
import { Container } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';
import { Outlet, useLocation } from 'react-router-dom';
import HomePage from '../../features/home/HomePage';

function App() {
  const location = useLocation();

  //router outlets is used when we do load a routes, this get swapped with the actual component that we are loading
  return (
    <>
    {location.pathname === '/' ? <HomePage /> : (
      <>
        <NavBar />
        <Container style={{ marginTop: '7em' }}>
          <Outlet />
        </Container>
      </>
    )}
  </>
  );
}

export default observer(App);
