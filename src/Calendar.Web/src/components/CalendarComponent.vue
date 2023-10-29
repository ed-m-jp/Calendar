<template>
    <div class='demo-app'>
        <div class='demo-app-main'>
            <FullCalendar ref="fullCalendar" class='demo-app-calendar' :options='calendarOptions'>
                <template #eventContent="arg">
                    <b>{{ arg.timeText }}</b>
                    <i>{{ arg.event.title }}</i>
                </template>
            </FullCalendar>
        </div>
    </div>
</template>

<script lang="ts">
    import { defineComponent } from 'vue';
    import FullCalendar from '@fullcalendar/vue3';
    import type { CalendarOptions, EventApi, DateSelectArg, EventClickArg, EventInput } from '@fullcalendar/core'
    import dayGridPlugin from '@fullcalendar/daygrid';
    import timeGridPlugin from '@fullcalendar/timegrid';
    import interactionPlugin from '@fullcalendar/interaction';
    import type { Calendar } from '@fullcalendar/core';
    import { mapState } from 'vuex';
    import store from '../stores/Store';
    import httpHelper from '../scripts/HttpHelper';
    import type { CancelTokenSource } from 'axios';

    export default defineComponent({
        components: {
            FullCalendar,
        },
        data() {
            return {
                calendarOptions: {
                    plugins: [
                        dayGridPlugin,
                        timeGridPlugin,
                        interactionPlugin // needed for dateClick
                    ],
                    headerToolbar: {
                        left: 'prev today',
                        center: 'title',
                        right: 'next'
                    },
                    initialView: store.state.calendarView.activeView,
                    editable: true,
                    selectable: true,
                    selectMirror: true,
                    dayMaxEvents: true,
                    weekends: true,
                    select: this.handleDateSelect,
                    eventClick: this.handleEventClick,
                    eventsSet: this.handleEvents
                } as CalendarOptions,
                currentEvents: [] as EventApi[],
                cancelTokenSource: null as CancelTokenSource|null,
            }
        },
        mounted() {
            this.cancelTokenSource = httpHelper.getCancelToken();
        },
        computed: {
            ...mapState('calendarView', ['activeView']),
            isUserLoggedIn(): boolean {
                return store.getters['user/isLoggedIn'];
            },
            getCalendarApi(): Calendar {
                return (this.$refs.fullCalendar as typeof FullCalendar).getApi();
            },
        },
        methods: {
            handleDateSelect(selectInfo: DateSelectArg) {
                let title = prompt('Please enter a new title for your event')
                let calendarApi = selectInfo.view.calendar

                calendarApi.unselect()

                if (title) {
                    const eventToAdd = {
                        title: title,
                        description: 'tmp description', // todo add this after creating the modal to add events
                        allDay: selectInfo.allDay,
                        startTime: selectInfo.startStr,
                        endTime: selectInfo.endStr
                    }
                    httpHelper.doPostHttpCall<EventPartialApiResponse>(
                        '/api/event',
                        eventToAdd,
                        httpHelper.getRequestHeader(),
                        this.cancelTokenSource!.token
                    ).then((resp) => {
                        calendarApi.addEvent({
                            id: resp.id.toString(),
                            title: resp.title,
                            start: resp.startTime,
                            end: resp.endTime,
                            allDay: resp.allDay,
                        })
                    }).catch((error) => {
                        // todo display proper error message.
                        console.log(error);
                    })
                }
            },
            handleEventClick(clickInfo: EventClickArg) {
                if (confirm(`Are you sure you want to delete the event '${clickInfo.event.title}'`)) {
                    httpHelper.doDeleteHttpCall<any>(
                        `/api/event/${clickInfo.event.id}`,
                        httpHelper.getRequestHeader(),
                        this.cancelTokenSource!.token,
                    ).then(() => {
                        clickInfo.event.remove();
                    }).catch((error) => {
                        // todo display proper error message.
                        console.log(error);
                    })
                }
            },
            handleEvents(events: EventApi[]) {
                this.currentEvents = events
            },
            transformToEventInput(apiResponseEvents: EventPartialApiResponse[]): EventInput[] {
                return apiResponseEvents.map(event => ({
                    id: event.id.toString(),
                    title: event.title,
                    start: event.startTime,
                    end: event.endTime,
                    allDay: event.allDay,
                }));
            },
            async fetchAndLoadEvents() {
                httpHelper.doGetHttpCall<EventPartialApiResponse[]>(
                    '/api/event/events/range',
                    httpHelper.getRequestHeader(),
                    this.cancelTokenSource!.token,
                    { 'startDate': '2023-10-1', 'endDate': '2023-11-1' }
                ).then((resp) => {
                    this.getCalendarApi.addEventSource(this.transformToEventInput(resp));
                }).catch((error) => {
                    // todo display proper error message.
                    console.log(error);
                });
            }
        },
        watch: {
            activeView(newView) {
                this.getCalendarApi.changeView(newView);
            },
            isUserLoggedIn(newValue) {
                if (newValue === true) {
                    this.fetchAndLoadEvents();
                } else {
                    this.getCalendarApi.removeAllEventSources();
                }
            },
        },
    });
</script>

<style scoped src='../assets/Calendar.css'></style>
