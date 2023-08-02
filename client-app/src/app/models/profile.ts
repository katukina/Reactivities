import { User } from "./user";

//Correspond with Application/Profiles/Profile.cs
export interface Profile {
    username: string;
    displayName: string;
    image?: string;
    bio?: string;
    photos?: Photo[]    
    followersCount: number;
    followingCount: number;
    following: boolean;
}

//class used in activityStore to set the values
export class Profile implements Profile {
    constructor(user: User) {
        this.username = user.username;
        this.displayName = user.displayName;
        this.image = user.image
    }
}

export interface Photo {
    id: string;
    url: string;
    isMain: boolean;
}

export interface userActivity {
    id: string;
    title: string;
    category: string;
    date: Date | null;
}