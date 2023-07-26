import { User } from "./user";

export interface Profile {
    username: string;
    displayName: string;
    image?: string;
    bio?: string;
}

//class used in activityStore to set the values
export class Profile implements Profile {
    constructor(user: User) {
        this.username = user.username;
        this.displayName = user.displayName;
        this.image = user.image
    }
}