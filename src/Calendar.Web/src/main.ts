import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

// vuetify
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import 'vuetify/dist/vuetify.min.css';

// stores
import store from './stores/Store';

const vuetify = createVuetify({
    components,
    directives,
})

// Create app instance
const app = createApp(App);

app.use(vuetify);

app.use(store);

// Mount the app
app.mount('#app');
