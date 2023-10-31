export enum HttpStatus {
    // 2xx Success
    OK = 200,
    CREATED = 201,
    NO_CONTENT = 204,

    // 4xx Client Errors
    BAD_REQUEST = 400,
    UNAUTHORIZED = 401,
    NOT_FOUND = 404,
    METHOD_NOT_ALLOWED = 405,
    CONFLICT = 409,
    IM_A_TEAPOT = 418, // LOL
    UNPROCESSABLE_ENTITY = 422,

    // 5xx Server Errors
    INTERNAL_SERVER_ERROR = 500,
    BAD_GATEWAY = 502,
    SERVICE_UNAVAILABLE = 503,
    GATEWAY_TIMEOUT = 504,
}
