import type { ActionContext } from 'vuex';

/**
 * User Store:
 * - Manages the user's state, including username and authentication token.
 * - Provides getters to determine if a user is logged in.
 * - Includes mutations to update or clear the user's information.
 * - Actions available for committing the respective mutations.
 */
export type UserState = {
    username: string | null;
    token: string | null;
};

const state = () => ({
    username: null,
    token: null,
});

const getters = {
    isLoggedIn: (state: UserState) => !!state.token
};

const mutations = {
    updateUserInfo(state: UserState, payload: { username: string | null; token: string | null; }) {
        state.username = payload.username;
        state.token = payload.token;
    },
    purgeUserData(state: UserState) {
        state.username = null;
        state.token = null;
    }
};

const actions = {
    updateUserInfo({ commit }: ActionContext<UserState, unknown>, payload: { username: string | null; token: string | null; }) {
        commit('updateUserInfo', payload);
    },
    purgeUserData({ commit }: ActionContext<UserState, unknown>) {
        commit('purgeUserData');
    }
};

export const userStore = {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
};
