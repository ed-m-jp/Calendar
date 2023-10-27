import type { CancelTokenSource } from "axios";

export type componentData = {
    username: string;
    password: string;
    errorMessage: string;
    submitted: boolean;
    cancelTokenSource: CancelTokenSource|null;
}