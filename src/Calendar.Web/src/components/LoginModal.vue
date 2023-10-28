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
    import { defineComponent } from 'vue';
    import httpHelper from '../scripts/HttpHelper';
    import type { componentData } from '../interfaces/LoginModalComponentDataType';
    import store from '../stores/Store';

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
            isUsernameValid(): boolean {
                return this.username.length >= 6;
            },
            isPasswordValid(): boolean {
                const hasDigit = /[0-9]/.test(this.password);
                const hasLowercase = /[a-z]/.test(this.password);
                const hasUppercase = /[A-Z]/.test(this.password);
                return this.password.length >= 6 && hasDigit && hasLowercase && hasUppercase;
            },
            isValid() {
                return this.isUsernameValid && this.isPasswordValid;
            },
            modalMessages() {
                return {
                    userNameTooShort: 'Username must be at least 6 characters.',
                    passwordTooWeak: 'Password must have at least 6 characters, one digit, one lowercase and one uppercase letter.',
                };
            },
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
                    console.log('Login with:', this.username, this.password);

                    httpHelper.doPostHttpCall<loginResponse>('/api/account/login', this.userRequest, {}, this.cancelTokenSource!.token!)
                        .then(async (resp) => {
                            store.dispatch('user/updateUserInfo', resp);
                            this.closeModal();
                            this.submitted = false;
                        })
                        .catch(async (error) => {
                            if (error.status === 400) {
                                this.errorMessage = 'Invalid login details.';
                            } else {
                                this.errorMessage = 'An error happened. Please try again later or contact an administrator.';
                            }
                            console.log(error);

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
                    httpHelper.doPostHttpCall<loginResponse>('/api/account/register', this.userRequest, {}, this.cancelTokenSource!.token!)
                        .then(async (resp) => {
                            console.log(resp);
                        })
                        .catch(async (error) => {
                            console.log(error);
                        })
                    //this.closeModal();
                }
            },
        },
    });
</script>
