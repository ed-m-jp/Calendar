<template>
    <v-dialog v-model="showModal" max-width="500px">
        <v-card>
            <v-card-title>
                {{ 'Calendar Event Details' }}
            </v-card-title>
            <v-alert v-if="errorMessage" type="error">
                {{ errorMessage }}
            </v-alert>
            <v-form ref="form">
                <v-card-text>
                    <v-text-field v-model="event.title"
                                  label="Event Title"
                                  readonly>
                    </v-text-field>
                    <v-text-field v-model="event.description"
                                  label="Event Description"
                                  readonly>
                    </v-text-field>
                    <v-checkbox v-model="event.allDay"
                                label="All Day Event"
                                readonly>
                    </v-checkbox>
                    <v-text-field v-model="event.startTime"
                                  label="Start Time"
                                  :type="dateInputType"
                                  readonly>
                    </v-text-field>
                    <v-text-field v-model="event.endTime"
                                  label="End Time"
                                  :type="dateInputType"
                                  readonly>
                    </v-text-field>
                </v-card-text>
                <v-card-actions>
                    <v-btn @click="closeModal">Cancel</v-btn>
                </v-card-actions>
            </v-form>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    // Vue Components
    import { defineComponent } from 'vue';
    // Moment
    import moment from 'moment-timezone';
    // Axios & HTTP Helper
    import httpHelper from '../../scripts/HttpHelper';
    import type { CancelTokenSource } from 'axios';
    import type { EventApiResponse } from '../../interfaces/EventApiResponse';
    import { HttpStatus } from '../../enums/HttpStatus';

    export default defineComponent({
        name: 'EventDetailsModal',
        emits: ['update:dialog', 'save'],
        props: {
            eventId: {
                type: Number,
                required: true
            },
            dialog: {
                type: Boolean,
                required: true,
            },
        },
        data() {
            return {
                event: {
                    title: '' as string,
                    description: '' as string,
                    allDay: false as boolean,
                    startTime: '' as string,
                    endTime: '' as string,
                },
                currentTimezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
                showModal: this.dialog,
                cancelTokenSource: null as CancelTokenSource | null,
                errorMessage: null as string | null,
            };
        },
        mounted() {
            this.cancelTokenSource = httpHelper.getCancelToken();
        },
        computed: {
            dateInputType() {
                return this.event.allDay ? 'date' : 'datetime-local';
            },
        },
        methods: {
            closeModal() {
                this.$emit('update:dialog', false);
            },
            formatDate(value: string, allDay: boolean) {
                const format = allDay ? 'YYYY-MM-DD' : 'YYYY-MM-DDTHH:mm';
                return moment.tz(value, this.currentTimezone).format(format);
            },
            formatEventDates() {
                this.event.startTime = this.formatDate(this.event.startTime, this.event.allDay);
                this.event.endTime = this.formatDate(this.event.endTime, this.event.allDay);
            },
        },
        watch: {
            eventId(newId: number) {
                this.errorMessage = null;
                httpHelper.doGetHttpCall<EventApiResponse>(
                    `/api/event/${newId}`,
                    httpHelper.getRequestHeader(),
                    this.cancelTokenSource!.token
                ).then((resp) => {
                    this.event = {
                        title: resp.title,
                        description: resp.description ?? '',
                        allDay: resp.allDay,
                        startTime: resp.startTime,
                        endTime: resp.endTime,
                    };
                    this.formatEventDates();
                }).catch((error) => {
                    if (error.status === HttpStatus.NOT_FOUND) {
                        // TODO decide for a better message
                        this.errorMessage = 'This event does not exist in our database.';
                    }
                    // Handle error properly
                    this.errorMessage = 'An error happened please try again later.';
                });
            },
            dialog(newVal: boolean) {
                this.showModal = newVal;
            },
            showModal(newVal: boolean) {
                if (newVal) {
                    this.formatEventDates();
                }
                this.$emit('update:dialog', newVal);
            },
            'newEvent.allDay'() {
                this.formatEventDates();
            },
        },
    });
</script>
