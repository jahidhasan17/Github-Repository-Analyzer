export interface SearchRepositoryItem{
	name: string,
	userName: string,
	forkName?: string,
	description ?: string,
	language ?: string,
	star ?: string,
	fork ?: string,
	modifiedDate ?: string,
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