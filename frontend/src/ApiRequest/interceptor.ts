import { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse  } from "axios";

const onRequest = (config: AxiosRequestConfig): any => {
    //console.info(`[request] [${JSON.stringify(config)}]`);
    return config;
}

const onRequestError = (error: AxiosError) : any => {
    //console.error(`[request error] [${JSON.stringify(error)}]`);
   //return Promise.reject(error);
}

const onResponse = (response: AxiosResponse): any => {
    // console.info(`[response] [${JSON.stringify(response)}]`);
    return response;
}

const onResponseError = (error: AxiosError): any => {
    // console.error(`[response error] [${JSON.stringify(error)}]`);
    // return Promise.reject(error);
}

export function setupInterceptorsTo(axiosInstance: AxiosInstance): AxiosInstance {
    axiosInstance.interceptors.request.use(onRequest, onRequestError);
    axiosInstance.interceptors.response.use(onResponse, onResponseError);
    return axiosInstance;
}