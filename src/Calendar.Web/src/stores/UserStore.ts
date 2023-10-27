import type { ActionContext } from 'vuex';

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
