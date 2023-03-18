
export interface CurrentUserToken {
	AccessToken: string,
	RefreshToken: string,
}

export interface CurrentUserDecodedInfo {
	Email: string,
	UserName: string,
	exp: any,
}

export interface AuthContextValue {
	currentUser : CurrentUserToken | null,
	setCurrentUser : (val: CurrentUserToken | null) => void,
	isAuthenticate : () => boolean,
	getUserTokenDecodedInfo : () => CurrentUserDecodedInfo | null,
	isUserLoggedIn : () => boolean,
	setToCurrentUser : (val: CurrentUserToken) => boolean,
	tryRefreshAccessToken : () => boolean,
	logout: () => void
}