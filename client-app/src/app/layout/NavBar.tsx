import { Button, Container, Menu } from "semantic-ui-react";
import { NavLink } from "react-router-dom";

export default function NavBar() {    

    return (
        <Menu inverted fixed="top">
            <Container>
                <Menu.Item as={NavLink} to='/' header>
                    <img src="/assets/logo.png" alt="logo" style={{margin :'10px'}}/>
                    Reactivities
                </Menu.Item >
                <Menu.Item as={NavLink} to='/activities' name="Activities"></Menu.Item>
                <Menu.Item as={NavLink} to='/errors' name='Errors' />
                <Menu.Item>
                    <Button as={NavLink} to='/CreateActivity' positive content="Create Activity"></Button>
                </Menu.Item>
            </Container>
        </Menu>
    )
}