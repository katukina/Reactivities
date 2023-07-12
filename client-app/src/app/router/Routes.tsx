import { RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import ActivityDashboard from "../../features/activities/dashboard/ActivityDashboard";
import ActivityDetails from "../../features/activities/details/ActivityDetails";
import AvtivityForm from "../../features/activities/form/AvtivityForm";
import HomePage from "../../features/home/HomePage";

//When same component is used with key then those components are not identical
export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            {path: 'activities', element: <ActivityDashboard />},
            {path: 'activities/:id', element: <ActivityDetails />},
            {path: 'createActivity', element: <AvtivityForm key='create' />},
            {path: 'manage/:id', element: <AvtivityForm key='manage' />},
        ]
    }
]

export const router =createBrowserRouter(routes);