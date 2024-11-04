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
const years = ref<number[]>([]); // Array to store unique sorted years

const fetchCategoriesData = async () => {
  if (!tabsUrl.value) {
    error.value = 'No valid URL provided.';
    return;
  }

  try {
    const response = await fetch(`http://192.168.1.90:5176/api/category/CategoriesByTabs?tabs=${tabsUrl.value}`);

    if (!response.ok) {
      throw new Error('Failed to fetch categories data');
    }
    const data = await response.json();

    categoriesData.value = data.sort((a: Category, b: Category) => {
      return new Date(b.startDate).getTime() - new Date(a.startDate).getTime();
    });

    const uniqueYears = new Set<number>();
    categoriesData.value.forEach((category) => {
      const startYear = new Date(category.startDate).getFullYear();
      uniqueYears.add(startYear);
      if (category.endDate) {
        const endYear = new Date(category.endDate).getFullYear();
        uniqueYears.add(endYear);
      }
    });
    years.value = Array.from(uniqueYears).sort((a, b) => b - a);
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

const scrollToTop = () => {
  window.scrollTo({
    top: 0,
    behavior: "smooth",
  });
};

const scrollToYear = (year: number) => {
  const yearElement = document.getElementById(`year-${year}`);
  if (yearElement) {
    const offset = -25;
    window.scrollTo({
      top: yearElement.offsetTop + offset,
      behavior: "smooth",
    });
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
    <aside
      class="hidden md:block sticky top-0 h-screen px-6 py-4 bg-cz-background-700 border-r border-cz-background-900 text-white">
      <ul class="space-y-1 h-full mx-8 flex flex-col items-center">
        <li><button @click="scrollToTop" class="font-bold text-xl my-2 text-center">Top</button></li>
        <li v-for="year in years" :key="year">
          <button @click="scrollToYear(year)" class="font-bold text-xl my-2 text-center">{{ year }}</button>
        </li>
      </ul>
    </aside>

    <div class="md:w-5/6 p-4">
      <header v-if="tabsData" class="md:mx-40 mb-6 text-center">
        <h1 class="text-white text-3xl md:text-5xl">{{ tabsData.title }}</h1>
        <p class="text-gray-300 md:text-lg">{{ tabsData.subtitle }}</p>
      </header>

      <main class="md:px-20">
        <div v-if="!loading && categoriesData.length > 0">
          <div v-for="year in years" :key="year" :id="'year-' + year" class="pt-4">
            <h2 class="text-2xl font-bold text-white mb-4">{{ year }}</h2>

            <ul class="space-y-4">
              <li v-for="category in categoriesData.filter(cat => new Date(cat.startDate).getFullYear() === year)"
                :key="category.id" class="md:flex border border-cz-red-950 rounded-lg p-4 bg-cz-background-700">

                <div
                  class="md:w-1/4 h-24 md:h-24 bg-cz-red-950 bg-opacity-50 md:flex md:items-center md:justify-center text-gray-400">
                  Image
                </div>

                <div class="mt-4 md:mt-0 md:ml-4 md:w-3/4">
                  <h3 class="md:text-2xl text-white">{{ category.title }}</h3>
                  <p class="text-cz-red-700 text-sm md:text-base text-opacity-50">
                    {{ category.startDate }} <span v-if="category.endDate">- {{ category.endDate }}</span>
                  </p>
                  <p class="text-gray-300">{{ category.description }}</p>
                  <router-link v-if="category.url" :to="`/${tabsUrl}/${category.url}`"
                    class="text-cz-red-400 hover:text-cz-red-200 md:mt-2 md:block">Find out more</router-link>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </main>
    </div>
  </body>

</template>