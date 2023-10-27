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
    import dayGridPlugin from '@fullcalendar/daygrid';
    import timeGridPlugin from '@fullcalendar/timegrid';
    import interactionPlugin from '@fullcalendar/interaction';
    import { INITIAL_EVENTS, createEventId } from '../scripts/EventHelper';
    import { mapState } from 'vuex';

    const CalendarComponent = defineComponent({
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
                    initialView: this.activeView,
                    initialEvents: INITIAL_EVENTS,
                    editable: true,
                    selectable: true,
                    selectMirror: true,
                    dayMaxEvents: true,
                    weekends: true,
                    select: this.handleDateSelect,
                    eventClick: this.handleEventClick,
                    eventsSet: this.handleEvents
                    /* you can update a remote database when these fire:
                    eventAdd:
                    eventChange:
                    eventRemove:
                    */
                },
                currentEvents: [],
            }
        },
        computed: {
            ...mapState('calendarView', ['activeView']),
        },
        methods: {
            handleDateSelect(selectInfo: any) {
                let title = prompt('Please enter a new title for your event')
                let calendarApi = selectInfo.view.calendar

                calendarApi.unselect()

                if (title) {
                    calendarApi.addEvent({
                        id: createEventId(),
                        title,
                        start: selectInfo.startStr,
                        end: selectInfo.endStr,
                        allDay: selectInfo.allDay
                    })
                }
            },
            handleEventClick(clickInfo: any) {
                if (confirm(`Are you sure you want to delete the event '${clickInfo.event.title}'`)) {
                    clickInfo.event.remove()
                }
            },
            handleEvents(events: any) {
                this.currentEvents = events
            },
        },
        watch: {
            activeView(newView) {
                (this.$refs.fullCalendar as typeof FullCalendar).getApi().changeView(newView);
            },
        },
    });

    export default CalendarComponent as any;
</script>


<style lang='css'>

    h2 {
        margin: 0;
        font-size: 16px;
    }

    ul {
        margin: 0;
        padding: 0 0 0 1.5em;
    }

    li {
        margin: 1.5em 0;
        padding: 0;
    }

    b { /* used for event dates/times */
        margin-right: 3px;
    }

    .demo-app {
        display: flex;
        min-height: 100%;
        font-family: Arial, Helvetica Neue, Helvetica, sans-serif;
        font-size: 14px;
    }

    .demo-app-sidebar {
        width: 300px;
        line-height: 1.5;
        background: #eaf9ff;
        border-right: 1px solid #d3e2e8;
    }

    .demo-app-sidebar-section {
        padding: 2em;
    }

    .demo-app-main {
        flex-grow: 1;
        padding: 3em;
    }

    .fc { /* the calendar root */
        max-width: 100%;
        margin: 0 auto;
    }
</style>
