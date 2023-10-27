import { createStore } from 'vuex';
import { userStore } from './UserStore';
import { calendarViewStore } from './CalendarViewStore';
import type { UserState } from './UserStore';
import type { CalendarViewState } from './CalendarViewStore';

export type RootState = {
    calendarView: CalendarViewState;
    user: UserState;
};

const store = createStore<RootState>({
    modules: {
        calendarView: calendarViewStore,
        user: userStore
    }
});

export default store;