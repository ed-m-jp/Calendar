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
    <EventCreateModal :dialog="addEventDialog" @update:dialog="updateDialog" :event="eventToCreate" @save="createEvent" />
</template>

<script lang="ts">
    // Vue Components
    import { defineComponent } from 'vue';
    import FullCalendar from '@fullcalendar/vue3';
    // Dialog component
    import EventCreateModal from './events/EventCreateModal.vue';
    // FullCalendar Types & Plugins
    import type {
        CalendarOptions,
        EventApi,
        DateSelectArg,
        EventClickArg,
        EventInput,
        Calendar
    } from '@fullcalendar/core';
    import dayGridPlugin from '@fullcalendar/daygrid';
    import timeGridPlugin from '@fullcalendar/timegrid';
    import interactionPlugin from '@fullcalendar/interaction';
    // Store and Vuex related
    import { mapState } from 'vuex';
    import store from '../stores/Store';
    // Axios & HTTP Helper
    import httpHelper from '../scripts/HttpHelper';
    import type { CancelTokenSource } from 'axios';
    import type { EventPartialApiResponse } from '../interfaces/EventPartialApiResponse'

    export default defineComponent({
        components: {
            FullCalendar,
            EventCreateModal,
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
                cancelTokenSource: null as CancelTokenSource | null,
                addEventDialog: false as boolean,
                eventToCreate: {} as object,
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
                selectInfo.view.calendar.unselect()
                this.eventToCreate = {
                    allDay: selectInfo.allDay,
                    startTime: selectInfo.startStr,
                    endTime: selectInfo.endStr,
                };
                this.addEventDialog = true;
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
            },
            updateDialog(val: boolean) {
                this.addEventDialog = val;
            },
            createEvent(newEvent: EventPartialApiResponse) {
                this.getCalendarApi.addEvent({
                    id: newEvent.id.toString(),
                    title: newEvent.title,
                    start: newEvent.startTime,
                    end: newEvent.endTime,
                    allDay: newEvent.allDay,
                });
            },
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
