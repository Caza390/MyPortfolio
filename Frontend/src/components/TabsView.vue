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

  <body class="md:flex">
    <!-- Sidebar -->
    <aside class="hidden md:block sticky top-0 h-screen px-6 py-4 bg-cz-background-700 border-r border-cz-background-900 text-white">
      <ul class="space-y-1 h-full flex flex-col items-center">
        <li><button class="font-bold text-xl my-2 text-center">Top</button></li>
        <li><button class="font-bold text-xl my-2 text-center">Undertaking</button></li>
        <li><button class="font-bold text-xl my-2 text-center">2025</button></li>
        <li><button class="font-bold text-xl my-2 text-center">2024</button></li>
        <li><button class="font-bold text-xl my-2 text-center">2023</button></li>
      </ul>
    </aside>


    <!-- Main Content -->
    <div class="md:w-5/6 p-4">
      <!-- Header with Title and Subtitle -->
      <header v-if="tabsData" class="md:mx-40 mb-6 text-center">
        <h1 class="text-white text-3xl md:text-5xl">{{ tabsData.title }}</h1>
        <p class="text-gray-300 md:text-lg">{{ tabsData.subtitle }}</p>
      </header>

      <!-- Category List -->
      <main class="md:px-20">
        <div v-if="!loading && categoriesData.length > 0">
          <ul class="space-y-4">
            <li v-for="category in categoriesData" :key="category.id"
              class="md:flex border border-cz-red-950 rounded-lg p-4 bg-cz-background-700">
              <!-- Placeholder Image -->
              <div class="md:w-1/4 h-24 md:h-24 bg-cz-red-950 bg-opacity-50 md:flex md:items-center md:justify-center text-gray-400">
                Image
              </div>

              <!-- Category Details -->
              <div class="mt-4 md:mt-0 md:ml-4 md:w-3/4">
                <h2 class="md:text-2xl text-white">{{ category.title }}</h2>
                <p class="text-cz-red-700 text-sm md:text-base text-opacity-50">{{ category.startDate }} <span v-if="category.endDate">- {{
                  category.endDate
                    }}</span></p>
                <p class="text-gray-300">{{ category.description }}</p>
                <a :href="category.url" class="text-cz-red-400 hover:text-cz-red-200 md:mt-2 md:block">Find out more</a>
              </div>
            </li>
          </ul>
        </div>
      </main>
    </div>
  </body>

</template>