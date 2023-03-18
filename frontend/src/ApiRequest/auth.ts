import axios from 'axios';
import { setupInterceptorsTo } from "./interceptor";

const axiosInstance = setupInterceptorsTo(axios);


export function signup(body: any) {
	return axiosInstance.post(process.env.REACT_APP_BASE_URL + "api/identity/signup", body);
}

export function login(body: any) {
	return axiosInstance.post(process.env.REACT_APP_BASE_URL + "api/identity/login", body);
}

export function refreshToken(body: any) {
	return axiosInstance.post(process.env.REACT_APP_BASE_URL + "api/identity/refresh", body);
}