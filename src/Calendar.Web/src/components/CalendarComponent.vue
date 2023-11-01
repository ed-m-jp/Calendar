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
    <EventCreateModal :dialog="showEventCreateDialog" :event="eventToCreate" @update:dialog="updateShowCreateEventDialog" @save="createEvent" />
    <EventDetailsModal :dialog="showEventDetailsDialog" :eventId="eventId" @update:dialog="updateShowEventDetailsDialog" />
</template>

<script lang="ts">
    // Vue Components
    import { defineComponent } from 'vue';
    import FullCalendar from '@fullcalendar/vue3';
    // Dialog component
    import EventCreateModal from './events/EventCreateModal.vue';
    import EventDetailsModal from './events/EventDetailsModal.vue';
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
            EventDetailsModal,
        },
        data() {
            return {
                // Configuration options for FullCalendar.
                calendarOptions: {
                    plugins: [
                        dayGridPlugin,
                        timeGridPlugin,
                        interactionPlugin // needed for dateClick.
                    ],
                    headerToolbar: {
                        left: 'prev today',
                        center: 'title',
                        right: 'next'
                    },
                    initialView: store.state.calendarView.activeView,
                    editable: false, // TODO: allow drag and drop and rezise.
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
                showEventCreateDialog: false as boolean,
                showEventDetailsDialog: false as boolean,
                eventToCreate: {} as object,
                eventId: 0 as number,
            }
        },
        mounted() {
            this.cancelTokenSource = httpHelper.getCancelToken();
        },
        computed: {
            // Mapping state of activeView from store to keep track of the selected active view (monthly, weekly or daily)
            ...mapState('calendarView', ['activeView']),
            isUserLoggedIn(): boolean {
                return store.getters['user/isLoggedIn'];
            },
            // Access the FullCalendar's API
            getCalendarApi(): Calendar {
                return (this.$refs.fullCalendar as typeof FullCalendar).getApi();
            },
        },
        methods: {
            // Handler for when a date range is selected in the calendar,
            // update eventToCreate to pass the selected date to create modal.
            handleDateSelect(selectInfo: DateSelectArg) {
                selectInfo.view.calendar.unselect()
                this.eventToCreate = {
                    allDay: selectInfo.allDay,
                    startTime: selectInfo.startStr,
                    endTime: selectInfo.endStr,
                };
                this.showEventCreateDialog = true;
            },
            // Handler for when an existing event is clicked in the calendar,
            // get the eventId to pass to the event details modal.
            handleEventClick(clickInfo: EventClickArg) {
                this.eventId = Number(clickInfo.event.id);
                this.showEventDetailsDialog = true;
            },
            handleEvents(events: EventApi[]) {
                this.currentEvents = events
            },
            // Transforms API response to a format that FullCalendar understands.
            transformToEventInput(apiResponseEvents: EventPartialApiResponse[]): EventInput[] {
                return apiResponseEvents.map(event => ({
                    id: event.id.toString(),
                    title: event.title,
                    start: event.startTime,
                    end: event.endTime,
                    allDay: event.allDay,
                }));
            },
            // Fetch events from the server for current logged in user and load them into the calendar.
            async fetchAndLoadEvents() {
                httpHelper.doGetHttpCall<EventPartialApiResponse[]>(
                    '/api/event/events/range',
                    httpHelper.getRequestHeader(),
                    this.cancelTokenSource!.token,
                    { 'startDate': '2023-11-1', 'endDate': '2023-12-1' } // todo: fetch current display
                ).then((resp) => {
                    this.getCalendarApi.addEventSource(this.transformToEventInput(resp));
                }).catch((error) => {
                    // todo display proper error message.
                    console.log(error);
                });
            },
            updateShowCreateEventDialog(val: boolean) {
                this.showEventCreateDialog = val;
            },
            updateShowEventDetailsDialog(val: boolean) {
                this.showEventDetailsDialog = val;
            },
            // Add a new event to the calendar based on the value passed from the create modal.
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
            // Change the calendar view based on the activeView from state. (related to mapState line 86)
            activeView(newView) {
                this.getCalendarApi.changeView(newView);
            },
            // Load events when a user logs in, remove them when they log out
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
