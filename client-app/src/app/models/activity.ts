import { Profile } from "./profile";

export interface Activity {
    id: string;
    title: string;
    date: Date | null;
    description: string;
    category: string;
    city: string;
    venue: string;
    hostUsername?: string;
    isCancelled?: boolean;
    isGoing?: boolean;
    isHost?: boolean
    attendees?: Profile[]
    host?: Profile;
  }

  export class Activity implements Activity {
    constructor(init?: ActivityFormValues) {
      Object.assign(this, init);
    }
  }
   
//Create this class to use a constructor to initializa certains values when pass activity object from API into this constructor.
export class ActivityFormValues
{
  id?: string = undefined;
  title: string = '';
  category: string = '';
  description: string = '';
  date: Date | null = null;
  city: string = '';
  venue: string = '';
  //the activity is passed here have more properties that in the API then we just set those that are there.
  constructor(activity?: ActivityFormValues) {
    if (activity) {
      this.id = activity.id;
      this.title = activity.title;
      this.category = activity.category;
      this.description = activity.description;
      this.date = activity.date;
      this.venue = activity.venue;
      this.city = activity.city;
    }
  }
}