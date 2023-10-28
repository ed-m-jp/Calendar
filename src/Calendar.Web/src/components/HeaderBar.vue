<template>
    <header class="header-container">
        <div class="left-actions">
            <v-btn-toggle v-model="activeView" mandatory>
                <v-btn :value="calendarViews.MONTH">Month</v-btn>
                <v-btn :value="calendarViews.WEEK">Week</v-btn>
                <v-btn :value="calendarViews.DAY">Day</v-btn>
            </v-btn-toggle>
        </div>
        <div class="right-actions">
            <template v-if="isUserLoggedIn">
                <span>{{ username }}</span>
                <v-btn color="secondary" @click="openLogoutModal">Logout</v-btn>
                <LogoutModal :show-modal="showLogoutModal" @close-modal="showLogoutModal = false" />
            </template>
            <template v-else>
                <v-btn color="primary" @click="openLoginModal">Login</v-btn>
                <LoginModal :show-modal="showLoginModal" @close-modal="showLoginModal = false" />
            </template>
        </div>
    </header>
</template>

<script lang="ts">
    import { defineComponent } from 'vue';
    import LoginModal from './LoginModal.vue';
    import LogoutModal from './LogoutModal.vue';
    import { CALENDAR_VIEW } from '../stores/CalendarViewStore';
    import { mapState } from 'vuex';
    import store from '../stores/Store';

    export default defineComponent({
        name: 'HeaderBar',
        components: {
            LoginModal,
            LogoutModal,
        },
        data() {
            return {
                showLoginModal: false,
                showLogoutModal: false,
                activeView: store.state.calendarView.activeView,
                calendarViews: CALENDAR_VIEW
            };
        },
        computed: {
            ...mapState('user', ['username']),
            isUserLoggedIn(): boolean {
                return store.getters['user/isLoggedIn'];
            },
        },
        methods: {
            openLoginModal() {
                this.showLoginModal = true;
            },
            openLogoutModal() {
                this.showLogoutModal = true;
            }
        },
        watch: {
            activeView(newView) {
                store.dispatch('calendarView/changeActiveView', newView);
            }
        }
    });
</script>

<style scoped src="../assets/HeaderBar.css"></style>
