<template>
    <v-dialog v-model="showModal" max-width="300">
        <v-card>
            <v-card-title class="headline">{{ modalMessages.title }}</v-card-title>
            <v-alert v-if="errorMessage" type="error">
                {{ errorMessage }}
            </v-alert>
            <v-card-text>{{ modalMessages.message }}</v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="green darken-1" text @click="logout">Yes</v-btn>
                <v-btn color="red darken-1" text @click="closeModal">No</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import { defineComponent } from 'vue';
    import httpHelper from '../scripts/HttpHelper';
    import type { componentData } from '../interfaces/LogoutModalComponentDataType';
    import store from '../stores/Store';

    const LogoutConfirmDialog = defineComponent({
        props: {
            showModal: {
                type: Boolean,
                required: true
            }
        },
        emits: ['close-modal'],
        data(): componentData {
            return {
                errorMessage: '',
                cancelTokenSource: null,
            };
        },
        mounted() {
             this.cancelTokenSource = httpHelper.getCancelToken();
        },
        computed: {
            modalMessages() {
                return {
                    title: 'Logout Confirmation.',
                    message: 'Are you sure you want to log out?',
                };
            },
        },
        methods: {
            closeModal() {
                this.$emit('close-modal');
            },
            logout() {
                this.errorMessage = '';

                httpHelper.doPostHttpCall<loginResponse>('/api/account/logout', {}, {}, this.cancelTokenSource!.token!)
                    .then(async () => {
                        store.dispatch('user/purgeUserData');
                        this.closeModal();
                    })
                    .catch(async (error) => {
                        this.errorMessage = 'An error happened. Please try again later or contact an administrator.';
                        console.log(error);
                    })
            },
        },
    });

    export default LogoutConfirmDialog;
</script>
