import { Button, Item, Label, Segment } from "semantic-ui-react";
import { SyntheticEvent, useState } from "react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";

export default observer(function ActivityList() {

    const {activityStore} = useStore();
    const {deleteActivity, activitiesByDate, loading} = activityStore;

    const [target, setTarget] = useState('');

    function handleActivityDelete(clickEvent: SyntheticEvent<HTMLButtonElement>, id: string) {
        setTarget(clickEvent.currentTarget.name);
        deleteActivity(id);
    }  

    return (
        <Segment>
            <Item.Group divided>
                {activitiesByDate.map(activity => (
                    <Item key={activity.id}>
                        <Item.Content>
                            <Item.Header as='a'>{activity.title}</Item.Header>
                            <Item.Meta>{activity.date}</Item.Meta>
                            <Item.Description>
                                <div>{activity.description}</div>
                                <div>{activity.city}, {activity.venue}</div>
                            </Item.Description>
                            <Item.Extra>
                                <Button onClick={() => activityStore.selectActivity(activity.id)} floated='right' content='view' color="blue"></Button>
                                <Button 
                                    name={activity.id} //each button has as a name uniquename that is the id
                                    loading={loading && target === activity.id} //ensure that only the clicked button show the loading
                                    onClick={(clickEvent) => handleActivityDelete(clickEvent, activity.id)} 
                                    floated='right' content='delete' color="red"></Button>
                                <Label basic content={activity.category}></Label>
                            </Item.Extra>
                        </Item.Content>
                    </Item>
                    ))}
            </Item.Group>
        </Segment>
    )
})