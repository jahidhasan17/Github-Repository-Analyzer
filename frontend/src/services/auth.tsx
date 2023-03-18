import jwtDecode from "jwt-decode";
import { CurrentUserDecodedInfo, CurrentUserToken } from "../models/auth";



// function isCurrentUserExpired(currentUser: CurrentUserToken) {
// 	if(isInvalidCurrentUser()) return true;

// 	const decodedInfo = tokenDecodedInfo(currentUser);
// 	return isExpired(decodedInfo);
// }

export function tokenDecodedInfo(currentUser: CurrentUserToken | null) {
	if(!currentUser) return null;
	try{
		const decodedInfo: CurrentUserDecodedInfo = jwtDecode(currentUser?.AccessToken);
		return decodedInfo;
	}catch(error) {
		console.log("Error Occur When decoded token");
		return null;
	}
}

export function isCurrentUserAuthenticate(currentUser: CurrentUserToken | null) {
	if(isInvalidCurrentUser(currentUser)) return false;

	const decodedInfo = tokenDecodedInfo(currentUser);
	if(isInvalidDecodedInfo(decodedInfo)) return false;

	if(isExpired(decodedInfo)) return false;
	
	return true;
}

export function isLoggedIn(currentUser: CurrentUserToken | null) {
	return !isInvalidCurrentUser(currentUser);
}


/* All Private Methods */
function isExpired(decodedInfo: CurrentUserDecodedInfo | null) {
	if(!decodedInfo || !decodedInfo.exp) return true;
	const date = new Date();
	return decodedInfo?.exp < date.getTime() / 1000;
}

function isInvalidCurrentUser(currentUser: CurrentUserToken | null) {
	if(!currentUser || !currentUser.AccessToken || !currentUser.RefreshToken) return true;
	return false;
}

function isInvalidDecodedInfo(decodedInfo: CurrentUserDecodedInfo | null) {
	if(!decodedInfo || !decodedInfo.UserName) return true;
	return false;
}

/* Private Method ends */