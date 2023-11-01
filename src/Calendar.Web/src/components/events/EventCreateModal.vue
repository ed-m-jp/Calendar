<template>
    <v-dialog v-model="showModal" max-width="500px">
        <v-card>
            <v-card-title>
                {{ 'Create new calendar Event' }}
            </v-card-title>
            <v-form ref="form" @submit.prevent="onSubmit">
                <v-card-text>
                    <v-text-field v-model="newEvent.title"
                                  :rules="titleRules"
                                  label="Event Title"
                                  required>
                    </v-text-field>
                    <v-text-field v-model="newEvent.description"
                                  :rules="descriptionRules"
                                  label="Event Description"
                                  required>
                    </v-text-field>
                    <v-checkbox v-model="newEvent.allDay" label="All Day Event"></v-checkbox>
                    <v-text-field v-model="newEvent.startTime"
                                  :rules="startTimeRules"
                                  label="Start Time"
                                  :type="dateInputType"
                                  required>
                    </v-text-field>
                    <v-text-field v-model="newEvent.endTime"
                                  :rules="endTimeRules"
                                  label="End Time"
                                  :type="dateInputType"
                                  required>
                    </v-text-field>
                </v-card-text>
                <v-card-actions>
                    <v-btn color="primary" type="submit">Save</v-btn>
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
    import type { EventPartialApiResponse } from '../../interfaces/EventPartialApiResponse';

    export default defineComponent({
        name: 'EventCreateModal',
        emits: ['update:dialog', 'save'],
        props: {
            event: {
                type: Object,
                required: true
            },
            dialog: {
                type: Boolean,
                required: true,
            },
        },
        data() {
            return {
                newEvent: {
                    title: '' as string,
                    description: '' as string,
                    allDay: this.event.allDay as boolean,
                    startTime: '' as string,
                    endTime: '' as string,
                },
                currentTimezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
                showModal: this.dialog,
                cancelTokenSource: null as CancelTokenSource | null,
                titleRules: [(value: string) => !!value || 'Title is required'],
                descriptionRules: [(value: string) => !!value || 'Description is required'],
                startTimeRules: [(value: string) => !!value || 'Start Time is required'],
                endTimeRules: [(value: string) => !!value || 'End Time is required'],
            };
        },
        mounted() {
            this.formatEventDates();
            this.cancelTokenSource = httpHelper.getCancelToken();
        },
        computed: {
            // Determines type of date input (full date or date-time) based on whether the event is all day.
            dateInputType() {
                return this.newEvent.allDay ? 'date' : 'datetime-local';
            },
        },
        methods: {
            closeModal() {
                this.$emit('update:dialog', false);
            },
            async onSubmit() {
                // Validate the form.
                const validationResult = await (this.$refs.form as any).validate();
                if (validationResult.valid) {
                    // Convert the form data for API request.
                    const apiRequest = {
                        title: this.newEvent.title,
                        description: this.newEvent.description,
                        allDay: this.newEvent.allDay,
                        startTime: moment.tz(this.newEvent.startTime, this.currentTimezone).toISOString(),
                        endTime: moment.tz(this.newEvent.endTime, this.currentTimezone).toISOString(),
                    }
                    // API request to create an event.
                    httpHelper.doPostHttpCall<EventPartialApiResponse>(
                        '/api/event',
                        apiRequest,
                        httpHelper.getRequestHeader(),
                        this.cancelTokenSource!.token
                    ).then((resp) => {
                        // On success, emit the saved event and close modal.
                        this.$emit('save', resp);
                        this.closeModal();
                    }).catch((error) => {
                        // Handle error properly.
                        console.log(error);
                    });
                }
            },
            // Format date based on whether it's an all-day event or specific time event.
            // TODO: check to move this to an helper to avoid code repeat
            formatDate(value: string, allDay: boolean) {
                const format = allDay ? 'YYYY-MM-DD' : 'YYYY-MM-DDTHH:mm';
                return moment.tz(value, this.currentTimezone).format(format);
            },
            formatEventDates() {
                this.newEvent.startTime = this.formatDate(this.event.startTime, this.newEvent.allDay);
                this.newEvent.endTime = this.formatDate(this.event.endTime, this.newEvent.allDay);
            },
        },
        watch: {
            // Handle when the event property changes.
            event: {
                deep: true,
                immediate: true,
                handler(newVal) {
                    this.newEvent = { ...newVal };
                }
            },
            dialog(newVal: boolean) {
                this.showModal = newVal;
            },
            showModal(newVal: boolean) {
                if (newVal === true) {
                    this.formatEventDates();
                }
                // This is here to handle the case when we click outside of the modal to close it.
                this.$emit('update:dialog', newVal);
            },
            // When the allDay property of the event changes update the date format.
            'newEvent.allDay'() {
                this.formatEventDates();
            },
        },
    });
</script>
