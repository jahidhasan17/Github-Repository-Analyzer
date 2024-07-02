import { Provider } from "../../const/User";

export interface User {
	id: string,
	UserName: string,
	Email: string,
	Password: string,
	Provider: Provider,
	AvatarUrl: string,
	Roles: string[],
}