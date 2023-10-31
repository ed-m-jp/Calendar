export type EventApiResponse = {
    id: number;
    title: string;
    description: string|null;
    allDay: boolean;
    startTime: string;
    endTime: string;
}