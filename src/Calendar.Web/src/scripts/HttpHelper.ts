import axios from 'axios';
import type { CancelToken, CancelTokenSource } from 'axios';
import store from '../stores/Store';

/**
 * HttpHelper Utility:
 * - Provides methods for HTTP requests using Axios (GET, POST, PUT, PATCH, DELETE).
 * - Handles request cancellation and generate header (including token from the store).
 * - Simplifies error handling for cancelled requests and other errors.
 */
export default {
    getCancelToken(): CancelTokenSource {
        return axios.CancelToken.source();
    },
    getRequestHeader() {
        return {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + store.state.user.token
        };
    },
    async doGetHttpCall<T>(
        url: string,
        headers: object,
        cancelToken: CancelToken,
        params: object = {}
    ): Promise<T> {
        return axios.get<T>(url, { headers, cancelToken, params })
            .then(response => {
                return response.data;
            })
            .catch((error) => {
                if (axios.isCancel(error)) {
                    throw {
                        message: error.message,
                        cancelled: true,
                    };
                } else {
                    throw {
                        status: error.response?.status,
                        message: error.message,
                        cancelled: false,
                    };
                }
            });
    },
    async doPostHttpCall<T>(
        url: string,
        data: any,
        headers: object,
        cancelToken: CancelToken,
    ): Promise<T> {
        return axios.post<T>(url, data, { headers, cancelToken })
            .then(response => {
                return response.data;
            })
            .catch((error) => {
                if (axios.isCancel(error)) {
                    throw {
                        message: error.message,
                        cancelled: true,
                    };
                } else {
                    throw {
                        status: error.response?.status,
                        message: error.message,
                        cancelled: false,
                    };
                }
            });
    },
    async doPatchHttpCall<T>(
        url: string,
        data: any,
        headers: object,
        cancelToken: CancelToken
    ): Promise<T> {
        return axios.patch<T>(url, data, { headers, cancelToken })
            .then(response => {
                return response.data;
            })
            .catch((error) => {
                if (axios.isCancel(error)) {
                    throw {
                        message: error.message,
                        cancelled: true,
                    };
                } else {
                    throw {
                        status: error.response?.status,
                        message: error.message,
                        cancelled: false,
                    };
                }
            });
    },
    async doPutHttpCall<T>(
        url: string,
        data: any,
        headers: object,
        cancelToken: CancelToken,
    ): Promise<T> {
        return axios.put<T>(url, data, { headers, cancelToken })
            .then(response => {
                return response.data;
            })
            .catch((error) => {
                if (axios.isCancel(error)) {
                    throw {
                        message: error.message,
                        cancelled: true,
                    };
                } else {
                    throw {
                        status: error.response?.status,
                        message: error.message,
                        cancelled: false,
                    };
                }
            });
    },
    async doDeleteHttpCall<T>(
        url: string,
        headers: object,
        cancelToken: CancelToken,
    ): Promise<T> {
        return axios.delete<T>(url, { headers, cancelToken })
            .then(response => {
                return response.data;
            })
            .catch((error) => {
                if (axios.isCancel(error)) {
                    throw {
                        message: error.message,
                        cancelled: true,
                    };
                } else {
                    throw {
                        status: error.response?.status,
                        message: error.message,
                        cancelled: false,
                    };
                }
            });
    }
}
