//To avoid in client side use some routes that are going to give not authorized from API side
//Here is user type http://localhost:3000/activities and it is not logged will come back to home page where login! button exist
import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useStore } from "../stores/store";

export default function RequireAuth() {
    const {userStore: {isLoggedIn}} = useStore();
    const location = useLocation();

    if (!isLoggedIn) {
        // Navigate to home page where user is logged in
        return <Navigate to='/' state={{from: location}} />
    }

    return <Outlet />
}