<template id="app">
    <HeaderBar />

    <CalendarComponent />
</template>

<script setup lang="ts">
    import CalendarComponent from './components/CalendarComponent.vue';
    import HeaderBar from './components/HeaderBar.vue';

    // For components
    const components = { CalendarComponent, HeaderBar };
</script>

<style scoped>
    
</style>