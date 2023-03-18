import { Route } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { Navigate } from "react-router-dom";
import Cookies from 'js-cookie';

import { useEffect } from "react";

interface Props {
	component: React.ComponentType,
}

export function GeneralRoute({ component: Component } : Props) : JSX.Element{
	const { isUserLoggedIn, isAuthenticate, tryRefreshAccessToken, setToCurrentUser } = useAuth();

	const AccessToken = Cookies.get("AccessToken");
	const RefreshToken =  Cookies.get("RefreshToken");

	console.log("Rendered GeneralRoute");


	useEffect(() => {
		if(!isUserLoggedIn() && AccessToken && RefreshToken) {
			console.log("Getting value from cookie");
			console.log(AccessToken);
			console.log(RefreshToken);
			setToCurrentUser({
				"AccessToken": AccessToken,
				"RefreshToken": RefreshToken,
			});
			Cookies.remove("AccessToken");
			Cookies.remove("RefreshToken");
		}
	}, [isUserLoggedIn, AccessToken, RefreshToken, setToCurrentUser]);


	
	console.log("isUserLoggedIn()", isUserLoggedIn());
	console.log("isAuthenticate()", isAuthenticate());
	if(isUserLoggedIn() && !isAuthenticate()) {
		tryRefreshAccessToken();
	}


	return (
		<Component />
	);
}