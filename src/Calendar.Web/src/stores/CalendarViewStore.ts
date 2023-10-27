import type { ActionContext } from 'vuex';

export const CALENDAR_VIEW = {
    MONTH: 'dayGridMonth',
    WEEK: 'timeGridWeek',
    DAY: 'timeGridDay',
} as const;

type CalendarViewType = typeof CALENDAR_VIEW[keyof typeof CALENDAR_VIEW];

export type CalendarViewState = {
    activeView: CalendarViewType;
};

const state = () => ({
    activeView: CALENDAR_VIEW.MONTH
});

const mutations = {
    changeCalendarView(state: CalendarViewState, view: CalendarViewType) {
        state.activeView = view;
    }
};

const actions = {
    changeActiveView({ commit }: ActionContext<CalendarViewState, unknown>, view: CalendarViewType) {
        commit('changeCalendarView', view);
    }
};

export const calendarViewStore = {
    namespaced: true,
    state,
    mutations,
    actions
};
