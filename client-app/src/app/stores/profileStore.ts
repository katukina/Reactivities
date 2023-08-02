import {Photo, Profile, userActivity} from "../models/profile";
import {makeAutoObservable, reaction, runInAction} from "mobx";
import agent from "../api/agent";
import {toast} from "react-toastify";
import { store } from "./store";

export default class ProfileStore {
    profile: Profile | null = null;
    loadingProfile = false;
    uploading = false;
    loading = false;
    followings: Profile[] = [];
    loadingFollowings = false;
    activeTab: number = 0;    
    activityList: userActivity[] = [];
    loadingActivities = false;

    //To make observable and propoerties are automatically flagges as observables
    constructor() {
        makeAutoObservable(this);
        reaction(
            () => this.activeTab,
            activeTab => {
                if (activeTab === 3 || activeTab === 4) {
                    const predicate = activeTab === 3 ? 'followers' : 'following';
                    this.loadFollowings(predicate);
                } else {
                    this.followings = [];
                }
            }
        )
    }

    setActiveTab = (activeTab: any) => {
        this.activeTab = activeTab;        
    }

    get isCurrentUser() {
        if (store.userStore.user && this.profile) {
            return store.userStore.user.username === this.profile.username;
        }
        return false;
    }

    //Added a method to get the activities calling agent.ts
    loadUserActivities = async (username: string, predicate?: string) => {
        this.loadingActivities = true;
        try {            
            const activities = await agent.Profiles.getActivities(username, predicate!);
            runInAction(() => {
                this.activityList = activities;
                this.loadingActivities = false;
            })
        } catch (error) {
            toast.error('Problem loading user activities');
            runInAction(() => {
                this.loadingProfile = false;
            })            
        }
    }

    loadProfile = async (username: string) => {
        this.loadingProfile = true;
        try {
            //Get a profile back form request to API
            const profile = await agent.Profiles.get(username);
            runInAction(() => {
                this.profile = profile;
                this.loadingProfile = false;
            })
        } catch (error) {
            toast.error('Problem loading profile');
            runInAction(() => {
                this.loadingProfile = false;
            })
        }
    }

    editProfile = async (profile: Partial<Profile>) => {
        this.loading = true;
        try {
            await agent.Profiles.editProfile(profile);
            runInAction(() => {
                if (profile.displayName && profile.displayName !== store.userStore.user?.displayName){
                    store.userStore.setDisplayName(profile.displayName);
                }
                let updatedProfile = {...this.profile, ...profile }; 
                this.profile = updatedProfile as Profile;
                this.loading = false;
            })
            
        } catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }

    }

    uploadPhoto = async (file: any) => {
        this.uploading = true;
        try {
            const response = await agent.Profiles.uploadPhoto(file);
            const photo = response.data;
            runInAction(() => {
                if (this.profile) {
                    this.profile.photos?.push(photo);
                    if (photo.isMain && store.userStore.user) {
                        store.userStore.setImage(photo.url);
                        this.profile.image = photo.url;
                    }
                }
                this.uploading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => this.uploading = false);
        }
    }

    setMainPhoto = async (photo: Photo) => {
        this.loading = true;
        try {
            await agent.Profiles.setMainPhoto(photo.id);
            //update suer store
            store.userStore.setImage(photo.url);
            runInAction(() => {
                if (this.profile && this.profile.photos) {
                    //find from all the photos the one that is main and then set that property to false.
                    this.profile.photos.find(a => a.isMain)!.isMain = false;
                    //Find from all the photos the one who has the id and set is main to true
                    this.profile.photos.find(a => a.id === photo.id)!.isMain = true;
                    //update profile store
                    this.profile.image = photo.url;
                    this.loading = false;
                }
            })
        } catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }

    deletePhoto = async (photo: Photo) => {
        this.loading = true;
        try {
            await agent.Profiles.deletePhoto(photo.id);
            runInAction(() => {
                if (this.profile) {
                    this.profile.photos = this.profile.photos?.filter(a => a.id !== photo.id);
                    this.loading = false;
                }
            })
        } catch (error) {
            toast.error('Problem deleting photo');
            this.loading = false;
        }

        
    }

    //This method is to update properties of profile
    //following is what we are going to do when button clicked.
    updateFollowing = async (username: string, following: boolean) => {
        this.loading = true;
        try {
            await agent.Profiles.updateFollowing(username);
            store.activityStore.updateAttendeeFollowing(username);
            runInAction(() => {
                //This is the case of the other profiles
                if (this.profile 
                        && this.profile.username !== store.userStore.user?.username 
                        && this.profile.username === username) {
                    following ? this.profile.followersCount++ : this.profile.followersCount--;
                    this.profile.following = !this.profile.following;
                } 
                //THis is the case of logged profile
                if (this.profile && this.profile.username === store.userStore.user?.username) {
                    following ? this.profile.followingCount++ : this.profile.followingCount--;
                }
                this.followings.forEach(profile => {
                    if (profile.username === username) {
                        //Base on existing state not the proterty following passing here
                        profile.following ? profile.followersCount-- : profile.followersCount++
                        profile.following = !profile.following;
                    }
                })
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }

    //List of followings
    loadFollowings = async (predicate: string) => {
        this.loadingFollowings = true;
        try {
            const followings = await agent.Profiles.listFollowings(this.profile!.username, predicate);
            runInAction(() => {
                this.followings = followings;
                this.loadingFollowings = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => this.loadingFollowings = false);
        }
    }    

    
}