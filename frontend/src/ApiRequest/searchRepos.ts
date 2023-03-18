import axios from 'axios';
import { setupSearchReposInterceptorsTo } from './SearchReposInterceptor';

const axiosInstance = setupSearchReposInterceptorsTo(axios);


export function SearchRepos(body: any, config: any) {
	return axiosInstance.post(process.env.REACT_APP_BASE_URL + "api/github/search/repository", body, config);
}

export function SearchTimeline(body: any, config: any) {
	return axiosInstance.post(process.env.REACT_APP_BASE_URL + "api/github/timeline/repository", body, config);
}
