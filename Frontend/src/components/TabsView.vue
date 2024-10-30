<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';

interface Category {
  id: number;
  title: string;
  description: string;
  url: string;
  startDate: string;
  endDate?: string | null;
}

interface TabsData {
  id: number;
  title: string;
  subtitle: string;
  url: string;
}

const route = useRoute();
const tabsUrl = ref(route.params.tabs ? String(route.params.tabs) : '');
const categoriesData = ref<Category[]>([]);
const tabsData = ref<TabsData | null>(null);
const loading = ref(true);
const error = ref('');

const fetchCategoriesData = async () => {
  if (!tabsUrl.value) {
    error.value = 'No valid URL provided.';
    loading.value = false;
    return;
  }

  loading.value = true;
  try {
    const response = await fetch(`http://192.168.1.90:5176/api/category/CategoriesByTabs?tabs=${tabsUrl.value}`);

    if (!response.ok) {
      throw new Error('Failed to fetch categories data');
    }
    categoriesData.value = await response.json();
  } catch (err: any) {
    error.value = err.message;
  } finally {
    loading.value = false;
  }
};

const fetchTabsData = async () => {
  if (!tabsUrl.value) return;

  try {
    const response = await fetch(`http://192.168.1.90:5176/api/tabs/TabsUrl?url=${tabsUrl.value}`);

    if (!response.ok) {
      throw new Error('Failed to fetch tabs data');
    }
    tabsData.value = await response.json();
  } catch (err: any) {
    error.value = err.message;
  }
};

onMounted(() => {
  fetchCategoriesData();
  fetchTabsData();
});

watch(() => route.params.tabs, (newTabs) => {
  tabsUrl.value = newTabs ? String(newTabs) : '';
  fetchCategoriesData();
  fetchTabsData();
});
</script>

<template>
  <div class="flex">
    <!-- Sidebar -->
    <aside class="w-1-8 p-4 bg-gray-800 text-white">
      <ul class="space-y-1">
        <li><button class="font-bold text-xl my-2">Undertaking</button></li>
        <li><button class="font-bold text-xl my-2">2025</button></li>
        <li><button class="font-bold text-xl my-2">2024</button></li>
        <li><button class="font-bold text-xl my-2">2023</button></li>
      </ul>
    </aside>

    <!-- Main Content -->
    <div class="w-3/4 p-4">
      <!-- Header with Title and Subtitle -->
      <header v-if="tabsData" class="mb-6">
        <h1 class="text-white text-4xl">{{ tabsData.title }}</h1>
        <p class="text-gray-300">{{ tabsData.subtitle }}</p>
      </header>

      <!-- Category List -->
      <main>
        <div v-if="!loading && categoriesData.length > 0">
          <ul class="space-y-4">
            <li
              v-for="category in categoriesData"
              :key="category.id"
              class="flex border border-gray-700 rounded-lg p-4 bg-gray-900"
            >
              <!-- Placeholder Image -->
              <div class="w-1/4 h-24 bg-gray-700 flex items-center justify-center text-gray-400">
                Image
              </div>

              <!-- Category Details -->
              <div class="ml-4 w-3/4">
                <h2 class="text-2xl text-white">{{ category.title }}</h2>
                <p class="text-gray-500">{{ category.startDate }} <span v-if="category.endDate">- {{ category.endDate }}</span></p>
                <p class="text-gray-300">{{ category.description }}</p>
                <a :href="category.url" class="text-blue-400 mt-2 block">Learn More</a>
              </div>
            </li>
          </ul>
        </div>
      </main>
    </div>
  </div>
</template>