import axios from 'axios';
import { setupSearchReposInterceptorsTo } from './SearchReposInterceptor';

const axiosInstance = setupSearchReposInterceptorsTo(axios);


export function SearchRepos(body: any, config: any) {
	return axiosInstance.post("http://localhost:8080/" + "api/search/repositories", body, config);
}

export function GetSearchResults(searchId: any) {
	return axiosInstance.get("http://localhost:8080/" + "api/get/repositories?searchId=" + searchId);
}


export function SearchTimeline(body: any, config: any) {
	return axiosInstance.post(process.env.REACT_APP_BASE_URL + "api/github/timeline/repository", body, config);
}
