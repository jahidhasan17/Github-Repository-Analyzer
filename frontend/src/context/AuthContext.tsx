import React, { useContext } from "react";
import { refreshToken } from "../ApiRequest/auth";
import { useLocalStorage } from "../hooks/useLocalStorage";
import { AuthContextValue, CurrentUserDecodedInfo, CurrentUserToken } from "../models/auth";
import { isCurrentUserAuthenticate, isLoggedIn, tokenDecodedInfo } from "../services/auth";
import { useNavigate } from 'react-router-dom';

const AuthContext = React.createContext<AuthContextValue | null>(null);

export function useAuth() {
	return useContext(AuthContext) as AuthContextValue;
}


interface Props {
	children: React.ReactNode
}


export function AuthProvider({ children }: Props) : JSX.Element {

	console.log("AuthProvider Rendered");

	const [currentUser, setCurrentUser] = useLocalStorage<CurrentUserToken | null>("CurrentUser", null);
	let navigate = useNavigate();

	function isAuthenticate() : boolean {
		return isCurrentUserAuthenticate(currentUser);
	}

	function getUserTokenDecodedInfo() : CurrentUserDecodedInfo | null {
		return tokenDecodedInfo(currentUser);
	}

	function isUserLoggedIn() : boolean {
		return isLoggedIn(currentUser);
	}

	function setToCurrentUser(user : CurrentUserToken) : boolean {
		setCurrentUser(user);
		return true;
	}

	function tryRefreshAccessToken() {
		console.log("Call RefreshToken => ", isLoggedIn(currentUser));
		if(!isLoggedIn(currentUser)) return false;

		refreshToken({
			"AccessToken" : currentUser?.AccessToken,
			"RefreshToken" : currentUser?.RefreshToken,
		})
		.then((data) => {
			console.log("Token Refresh Successfully");
			console.log(data);
			setCurrentUser({
				"AccessToken": data.data.AccessToken,
				"RefreshToken": data.data.RefreshToken,
			});
			return true;
		}).catch((error) => {
			console.log("Error Occur When token refreshing");
			console.log(error);
			return false;
		});
		return false;
	}

	function logout() {
		setCurrentUser(null);
		navigate('/');
	}


	const value = {
		currentUser,
		setCurrentUser,
		isAuthenticate,
		getUserTokenDecodedInfo,
		isUserLoggedIn,
		setToCurrentUser,
		tryRefreshAccessToken,
		logout
	};

	return (
		<AuthContext.Provider value={value}>
			{children}
		</AuthContext.Provider>
	);
}