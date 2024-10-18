<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';

// Define the Tabs interface
interface Tabs {
  id: number;
  name: string;
  description: string;
  url: string;
}

const route = useRoute();
const tabsUrl = ref(route.params.tabs ? String(route.params.tabs) : ''); // Set tabsUrl based on route params
const tabsData = ref<Tabs | null>(null);
const loading = ref(true);
const error = ref('');

// Function to fetch tabs data
const fetchTabsData = async () => {
  if (!tabsUrl.value) {
    // Prevent fetching if URL is empty
    error.value = 'No valid URL provided.';
    loading.value = false;
    return;
  }

  loading.value = true;
  try {
    const response = await fetch(`http://192.168.1.90:5176/api/tabs/Url?url=${tabsUrl.value}`);

    if (!response.ok) {
      throw new Error('Failed to fetch tabs data');
    }
    tabsData.value = await response.json();
    loading.value = false;
  } catch (err: any) {
    error.value = err.message;
    loading.value = false;
  }
};

// Fetch data on component mount
onMounted(() => {
  fetchTabsData();
});

// Watch for changes in the route parameter
watch(() => route.params.tabs, (newTabs) => {
  tabsUrl.value = newTabs ? String(newTabs) : ''; // Update tabsUrl
  fetchTabsData(); // Refetch data if the URL changes
});
</script>

<template>
  <main>
    <div v-if="loading">Loading...</div>
    <div v-if="error" class="text-red-500">{{ error }}</div>
    <div v-if="tabsData">
      <h1 class="text-white text-3xl">{{ tabsData.name }}</h1>
      <p class="text-gray-300">{{ tabsData.description }}</p>
    </div>
  </main>
</template>
