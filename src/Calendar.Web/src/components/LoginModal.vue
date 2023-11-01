<template>
    <v-dialog :model-value="showModal" persistent max-width="500px">
        <v-card>
            <v-card-title>
                <v-row class="fill-height no-gutters">
                    <v-col class="align-self-center">
                        Login
                    </v-col>
                    <v-col class="text-end align-self-center">
                        <v-btn icon @click="closeModal">
                            <v-icon>mdi-close</v-icon>
                        </v-btn>
                    </v-col>
                </v-row>
            </v-card-title>

            <v-alert v-if="errorMessage" type="error">
                {{ errorMessage }}
            </v-alert>

            <v-card-text class="text-start">
                <v-text-field label="Username" v-model="username" :error-messages="submitted && !isUsernameValid ? modalMessages.userNameTooShort : ''"></v-text-field>
                <v-text-field type="password" label="Password" v-model="password" :error-messages="submitted && !isPasswordValid ? modalMessages.passwordTooWeak : ''"></v-text-field>
            </v-card-text>

            <v-card-actions class="text-start">
                <v-btn color="primary" @click="login">Sign In</v-btn>
                <v-btn color="secondary" @click="register">Register</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    // Vue Components
    import { defineComponent } from 'vue';
    // Utils & Helpers
    import httpHelper from '../scripts/HttpHelper';
    // Store and Vuex related
    import store from '../stores/Store';
    // Types & Interfaces
    import type { componentData } from '../interfaces/LoginModalComponentDataType';
    import type { loginResponse } from '../interfaces/LoginResponse';

    export default defineComponent({
        props: {
            showModal: {
                type: Boolean,
                required: true
            }
        },
        emits: ['close-modal'],
        data(): componentData {
            return {
                username: '',
                password: '',
                errorMessage: '',
                submitted: false,
                cancelTokenSource: null,
            };
        },
        mounted() {
            this.cancelTokenSource = httpHelper.getCancelToken();
        },
        computed: {
            // Checks if the username length meets a minimum requirement.
            isUsernameValid(): boolean {
                return this.username.length >= 6;
            },
            // Checks if the password meets the required criteria for strength.
            // Need at least 1 uppercase, 1 lowercase, 1 number and minimum 6 characters.
            isPasswordValid(): boolean {
                const hasDigit = /[0-9]/.test(this.password);
                const hasLowercase = /[a-z]/.test(this.password);
                const hasUppercase = /[A-Z]/.test(this.password);
                return this.password.length >= 6 && hasDigit && hasLowercase && hasUppercase;
            },
            // Ensure username and password are valid.
            isValid() {
                return this.isUsernameValid && this.isPasswordValid;
            },
            // Contains messages to be shown for various input scenarios.
            modalMessages() {
                return {
                    userNameTooShort: 'Username must be at least 6 characters.',
                    passwordTooWeak: 'Password must have at least 6 characters, one digit, one lowercase and one uppercase letter.',
                };
            },
            // Preparing login/register request for API calls.
            userRequest() {
                return {
                    Username: this.username,
                    Password: this.password,
                };
            }
        },
        methods: {
            closeModal() {
                this.errorMessage = '';
                this.submitted = false;
                this.$emit('close-modal');
            },
            login() {
                this.errorMessage = '';
                this.submitted = true;
                if (!this.isValid) {
                    this.errorMessage = 'Invalid login details.';
                } else {
                    // Making API call to try to log in based on provided credentials.
                    httpHelper.doPostHttpCall<loginResponse>(
                        '/api/account/login',           // Endpoint url.
                        this.userRequest,               // Request body.
                        {},                             // Request header.
                        this.cancelTokenSource!.token!  // Cancellation token.
                    ).then(async (resp) => {
                        // If successful, update user information in the store (JWT token + username).
                        store.dispatch('user/updateUserInfo', resp);
                        this.closeModal();
                    }).catch(async (error) => {
                        // Handle errors based on status code.
                        if (error.status === 400) {
                            this.errorMessage = 'Invalid login details.';
                        } else {
                            this.errorMessage = 'An error happened. Please try again later or contact an administrator.';
                        }
                        console.log(error);
                    }).finally(() => {
                        this.submitted = false;
                    })
                }
            },
            register() {
                this.errorMessage = '';
                this.submitted = true;
                if (!this.isValid) {
                    this.errorMessage = 'Invalid registration details.';
                } else {
                    // Making API call to try to register a new user based on provided credentials.
                    httpHelper.doPostHttpCall<loginResponse>(
                        '/api/account/register',        // Endpoint url.
                        this.userRequest,               // Request body.
                        {},                             // Request header.
                        this.cancelTokenSource!.token!  // Cancellation token.
                    ).then(async (resp) => {
                        // If successful, update information for newly created user in the store (JWT token + username).
                        store.dispatch('user/updateUserInfo', resp);
                        this.closeModal();
                    }).catch(async (error) => {
                        // Handle errors based on status code.
                        if (error.status === 400) {
                            this.errorMessage = 'Invalid login details.';
                        } else {
                            this.errorMessage = 'An error happened. Please try again later or contact an administrator.';
                        }
                        console.log(error);
                    }).finally(() => {
                        this.submitted = false;
                    })
                }
            },
        },
    });
</script>
