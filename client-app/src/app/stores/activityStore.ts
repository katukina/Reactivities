import { makeAutoObservable, runInAction } from "mobx";
import { Activity, ActivityFormValues  } from "../models/activity";
import agent from "../api/agent";
import { v4 as uuid } from 'uuid';
import { format } from 'date-fns';
import { store } from "./store";
import { Profile } from "../models/profile";

export default class ActivityStore {
    activityMap = new Map<string, Activity>();
    selectedActivity?: Activity | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = false;

    constructor() {
        makeAutoObservable(this);
    }

    //This was similar that it was App.tsx in useEffect to reduce code there
    loadActivities = async () => {
        this.setLoadingInitial(true);
        try {
            const activities = await agent.Activities.list();            
            activities.forEach(activity => {
                this.setActivity(activity);
            })
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    }

    private setActivity = (activity: Activity) => {
        const user = store.userStore.user;
        if (user) {
            activity.isGoing = activity.attendees!.some(
                a => a.username === user.username
            );
            activity.isHost = activity.hostUsername === user.username;
            activity.host = activity.attendees?.find(x => x.username === activity.hostUsername);
        }        
        activity.date = new Date(activity.date!);
        this.activityMap.set(activity.id, activity);    }

    private getActivity = (id: string) => {
        return this.activityMap.get(id);
    }

    //If we have in memory we used it else from API using agent
    loadActivity = async (id: string) => {
        let activity = this.getActivity(id);
        if (activity) {
            this.selectedActivity = activity;
            return activity;
        }
        else {
            this.setLoadingInitial(true);
            try {
                activity = await agent.Activities.details(id);
                this.setActivity(activity);
                this.setLoadingInitial(false);
                return activity;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    //methods including select activity not needed(deleted) and also because there is routing available
    createActivity = async (activity: ActivityFormValues) => {
        const user = store.userStore!.user;
        const profile = new Profile(user!)
        try {
            await agent.Activities.create(activity);
            //set the other properties that are also needed
            const newActivity = new Activity(activity);
            newActivity.hostUsername = user!.username;
            newActivity.attendees = [profile];
            this.setActivity(newActivity);
            runInAction(() => this.selectedActivity = newActivity);
        } catch (error) {
            console.log(error);
        }
    }

    updateActivity = async (activity: ActivityFormValues) => {
        try {
            await agent.Activities.update(activity);
            runInAction(() => {
                if (activity.id) {
                    //combine updatedActivity, the ActivityFormValues that has been updated but also get the original info for activity with missing properties
                    //spread operator permite combinar varios objetos o arrays en uno solo. Es especialmente útil para combinar las propiedades de varios objetos en uno nuevo.
                    //first is the original and second is activity of ActivityFormValues
                    let updatedActivity = {...this.getActivity(activity.id), ...activity };
                    this.activityMap.set(activity.id, updatedActivity as Activity);
                    this.selectedActivity = updatedActivity as Activity;
                }
            })
        } catch (error) {
            console.log(error);
        }
    }

    deleteActivity = async (id: string) => {
        this.loading = true;
        try {
            await agent.Activities.delete(id);
            runInAction(() => {
                this.activityMap.delete(id);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }

        
    }

    //Is made in same button cancel/Join activity in ActivityDetailHeader.tsx
    updateAttendeance = async () => {
        const user = store.userStore.user;
        this.loading = true;
        try {
            //Update API call attend methond in agents
            await agent.Activities.attend(this.selectedActivity!.id);
            runInAction(() => {
                //remove if already is the activity if not create a new object of profile type and add to the list
                if (this.selectedActivity?.isGoing) {
                    this.selectedActivity.attendees = this.selectedActivity.attendees?.filter(a => a.username !== user?.username);
                    this.selectedActivity.isGoing = false;
                } else {
                    const attendee = new Profile(user!);
                    this.selectedActivity?.attendees?.push(attendee);
                    this.selectedActivity!.isGoing = true;
                }
                this.activityMap.set(this.selectedActivity!.id, this.selectedActivity!);
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }

    //Is made in same button 'Re-activate activity' : 'Cancel Activity' button in ActivityDetailHeader.tsx
    cancelActivityToggle = async () => {
        this.loading = true;
        try {
            //attend methond in agents
            await agent.Activities.attend(this.selectedActivity!.id);
            runInAction(() => {
                //Set the oposite of what is already set
                this.selectedActivity!.isCancelled = !this.selectedActivity!.isCancelled;
                this.activityMap.set(this.selectedActivity!.id, this.selectedActivity!);
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }    
    
    get activitiesByDate() {
        return Array.from(this.activityMap.values()).sort((a, b) =>
            a.date!.getTime() - b.date!.getTime())
    }

    get groupedActivities() {
        return Object.entries(
            this.activitiesByDate.reduce((activities, activity) => {
                const date = format(activity.date!, 'dd MMM yyyy');
                activities[date] = activities[date] ? [...activities[date], activity] : [activity];
                return activities;
            }, {} as {[key: string]: Activity[]})
        )
    }    

    clearSelectedActivity = () => {
        this.selectedActivity = undefined;
    }

    updateAttendeeFollowing = (username: string) => {
        this.activityMap.forEach(activity => {
            activity.attendees.forEach((attendee: Profile) => {
                if (attendee.username === username) {
                    attendee.following ? attendee.followersCount-- : attendee.followersCount++;
                    attendee.following = !attendee.following;
                }
            })
        })
    }      
}

