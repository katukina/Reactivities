import { Button, ButtonGroup, Card, Image } from 'semantic-ui-react'
import { useStore } from "../../../app/stores/store";
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { observer } from 'mobx-react-lite';
import { Link, useParams } from 'react-router-dom';
import { useEffect } from 'react';

//because we want to observe loadActivity & loadingInitial this component needs to observe
export default observer(function ActivityDetails() {
    const {activityStore} = useStore();
    const {selectedActivity: activity, loadActivity, loadingInitial} = activityStore;
    const {id} = useParams();

    //When the component is loaded show the activity by calling loadActivity
    useEffect(() => {
        if (id) loadActivity(id);
    }, [id, loadActivity])

    if (loadingInitial || !activity) return <LoadingComponent />;

    return (
        <Card fluid>
            <Image src={`/assets/categoryImages/${activity.category}.jpg`}/>
            <Card.Content>
                <Card.Header>{activity.title}</Card.Header>
                <Card.Meta>
                    <span>{activity.date}</span>
                </Card.Meta>
                <Card.Description>
                    {activity.description}
                </Card.Description>
            </Card.Content>
            <Card.Content extra>
                <ButtonGroup widths={2}></ButtonGroup>
                    <Button as={Link} to={`/manage/${activity.id}`} basic color='blue' content='Edit'></Button>
                    <Button as={Link} to='/activities' basic color='grey' content='Cancel'></Button>
            </Card.Content>
        </Card>
    )
})