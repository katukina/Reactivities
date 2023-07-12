import { Grid } from "semantic-ui-react";
import ActivityList from "./ActivityList";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";

export default observer(function ActivityDashboard() {

    const {activityStore} = useStore();
    const {loadActivities, activityMap} = activityStore;

    //what we do when app loads up
    useEffect(() => {
        if (activityMap.size <= 1) loadActivities();
    }, [activityMap.size, loadActivities])
  
    if (activityStore.loadingInitial) return <LoadingComponent content='Loading app...' />

    return (
        <Grid>
            <Grid.Column width={10}>
            <ActivityList />
            </Grid.Column>
            <Grid.Column width={6}>
                <h2>Activities filters</h2>
            </Grid.Column>
        </Grid>
    )
})