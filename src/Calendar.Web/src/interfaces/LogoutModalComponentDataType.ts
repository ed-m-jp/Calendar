import type { CancelTokenSource } from "axios";

export type componentData = {
    errorMessage: string;
    cancelTokenSource: CancelTokenSource|null;
}