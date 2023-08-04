import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { makeAutoObservable, runInAction } from "mobx";
import { ChatComment } from "../models/comment";
import { store } from "./store";

export default class CommentStore {
    comments: ChatComment[] = [];
    hubConnection: HubConnection | null = null;

    constructor() {
        makeAutoObservable(this);
    }

    createHubConnection = (activityId: string) => {
        if (store.activityStore.selectedActivity) {
            this.hubConnection = new HubConnectionBuilder()
            //Our chat endpoint, be carefull with (activityId) spelling as we are suing the string as the key and needs to match with the server
            //Next pass the token
                .withUrl(process.env.REACT_APP_CHAT_URL + '?activityId=' + activityId, {
                    accessTokenFactory: () => store.userStore.user?.token!
                })
                //Reconnect to client char hub if connection lose
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Information)
                // Create the connection
                .build();

            this.hubConnection.start().catch(error => console.log('Error establishing connection: ', error));
            //What was used in ChatHub.cs in the API, has to match and we get back list of comments
            this.hubConnection.on('LoadComments', (comments: ChatComment[]) => {
                //update our observable for that we need runInAction
                runInAction(() => {
                    comments.forEach(comment => {
                        //Make date Javascript date
                        comment.createdAt = new Date(comment.createdAt);
                    });
                    this.comments = comments;
                });
            });

            //New comment
            this.hubConnection.on('ReceiveComment', comment => {
                runInAction(() => {
                    comment.createdAt = new Date(comment.createdAt);
                    //Instead of push unshift which will place the comment at the start of the array
                    this.comments.unshift(comment);
                })
            })
        }
    }

    stopHubConnection = () => {
        this.hubConnection?.stop().catch(error => console.log('Error stopping connection: ', error));
    }

    //Call this when we move away from activity
    clearComments = () => {
        this.comments = [];
        this.stopHubConnection();
    }

    addComment = async (values: any) => {
        values.activityId = store.activityStore.selectedActivity?.id;
        try {
            //From API this need to match exactly the name of the method that we want to invoke on the server
            await this.hubConnection?.invoke('SendComment', values);
        } catch (error) {
            console.log(error);
        }
    }
}