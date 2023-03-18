export interface SearchRepositoryItem{
	Name: string,
	UserName: string,
	ForkName?: string,
	Description ?: string,
	Language ?: string,
	Star ?: string,
	Fork ?: string,
	ModifiedDate ?: string,
}


export interface RepositoryLanguage {
	Name: string,
	Parcentage ?: string | null,
}


export interface TimelineRepositoryItem {
	Name: string,
	UserName: string,
	ForkName ?: string,
	Description ?: string,
	Star ?: string,
	Fork ?: string,
	ModifiedDate ?: string,
	CreatedDate ?: string,
	Languages ?: RepositoryLanguage[],
	MainBrachName ?: string,
	IsFristUsersRepos ?: boolean,
	IsSecondUsersRepos ?: boolean,
}