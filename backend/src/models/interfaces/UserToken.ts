
export interface UserToken {
	id: string,
	UserId: string,
	RefreshToken: string,
	GithubAccessToken: string,
	RefreshTokenExpiryTime: Date,
}