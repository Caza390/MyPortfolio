<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { useRoute } from 'vue-router';

interface Subcategory {
  id: number;
  heading: string | null;
  title: string;
  description: string;
  startDate?: string | null; 
  endDate?: string | null;
  category: string;
}

interface CategoryData {
  id: number;
  title: string;
  description: string;
  url: string;
}

const route = useRoute();
const tabsUrl = ref(route.params.tabs ? String(route.params.tabs) : '');
const categoryUrl = ref(route.params.category ? String(route.params.category) : '');

const subcategoriesData = ref<Subcategory[]>([]);
const categoryData = ref<CategoryData | null>(null);
const loading = ref(true);
const error = ref('');

// Fetch subcategories data
const fetchSubcategoriesData = async () => {
  if (!tabsUrl.value || !categoryUrl.value) {
    error.value = 'Tabs or Category URL is missing.';
    loading.value = false;
    return;
  }

  loading.value = true;
  try {
    console.log(`Fetching subcategories for tabs: ${tabsUrl.value} and category: ${categoryUrl.value}`);
    const response = await fetch(
      `http://192.168.1.90:5176/api/subcategory/SubCategoriesByCategory?tabs=${tabsUrl.value}&category=${categoryUrl.value}`
    );

    if (!response.ok) {
      throw new Error('Failed to fetch subcategories data');
    }
    const data: Subcategory[] = await response.json();

    // Sort by startDate (old to new), then by heading
    subcategoriesData.value = data.sort((a: Subcategory, b: Subcategory) => {
      const dateA = a.startDate ? new Date : new Date(0);
      const dateB = b.startDate ? new Date : new Date(0);
      
      if (dateA.getTime() !== dateB.getTime()) return dateA.getTime() - dateB.getTime();
      return (a.heading || '').localeCompare(b.heading || '');
    });
  } catch (err: any) {
    error.value = err.message;
  } finally {
    loading.value = false;
  }
};

// Fetch category data
const fetchCategoryData = async () => {
  if (!tabsUrl.value || !categoryUrl.value) return;

  try {
    console.log(`Fetching category data for tabs: ${tabsUrl.value} and category: ${categoryUrl.value}`);
    const response = await fetch(
      `http://192.168.1.90:5176/api/category/CategoryUrl?tabs=${tabsUrl.value}&url=${categoryUrl.value}`
    );

    if (!response.ok) {
      throw new Error('Failed to fetch category data');
    }
    categoryData.value = await response.json();
  } catch (err: any) {
    error.value = err.message;
  }
};

// Group subcategories by heading
const groupedSubcategories = computed(() => {
  const groups: { [key: string]: Subcategory[] } = {};
  
  subcategoriesData.value.forEach(subcategory => {
    const heading = subcategory.heading || 'Uncategorized';
    if (!groups[heading]) {
      groups[heading] = [];
    }
    groups[heading].push(subcategory);
  });

  return Object.values(groups);
});

// Compute headings from grouped subcategories
const headings = computed(() => {
  return groupedSubcategories.value
    .map(group => group[0].heading)
    .filter((heading): heading is string => heading !== null); // Type guard to ensure heading is string
});

const scrollToTop = () => {
  window.scrollTo({
    top: 0,
    behavior: "smooth",
  });
};

const scrollToHeading = (heading: string) => { // Changed from Heading to heading
  const headingElement = document.getElementById(`heading-${heading}`);
  if (headingElement) {
    const offset = -25; // Adjust for fixed header or offset if needed
    window.scrollTo({
      top: headingElement.offsetTop + offset,
      behavior: "smooth",
    });
  }
};

onMounted(() => {
  fetchSubcategoriesData();
  fetchCategoryData();
});

watch(
  () => [route.params.tabs, route.params.category],
  ([newTabs, newCategory]) => {
    tabsUrl.value = newTabs ? String(newTabs) : '';
    categoryUrl.value = newCategory ? String(newCategory) : '';
    fetchSubcategoriesData();
    fetchCategoryData();
  }
);
</script>


<template>
  <body class="md:flex">
    <aside class="hidden md:block sticky top-0 h-screen px-6 py-4 bg-cz-background-700 border-r border-cz-background-900 text-white">
      <ul class="space-y-1 h-full mx-8 flex flex-col items-center">
        <li><button @click="scrollToTop" class="font-bold text-xl my-2 text-center">Top</button></li>
        <li v-for="heading in headings" :key="heading">
          <button @click="scrollToHeading(heading)" class="font-bold text-xl my-2 text-center">{{ heading }}</button>
        </li>
      </ul>
    </aside>

    <div class="md:w-5/6 p-4">
      <header v-if="categoryData" class="md:mx-40 mb-6 text-center">
        <h1 class="text-white text-3xl md:text-5xl">{{ categoryData.title }}</h1>
        <p class="text-gray-300 md:text-lg">{{ categoryData.description }}</p>
      </header>

      <main class="md:px-20">
        <div v-if="!loading && subcategoriesData.length > 0">
          <template v-for="(group, index) in groupedSubcategories">
            <h2 v-if="group.length > 0" class="text-2xl font-bold text-white mb-4">{{ group[0].heading }}</h2>
            <ul class="space-y-4">
              <li v-for="subcategory in group" :key="subcategory.id"
                  class="md:flex border border-cz-red-950 rounded-lg p-4 bg-cz-background-700">
                <div class="md:w-1/4 h-24 md:h-24 bg-cz-red-950 bg-opacity-50 md:flex md:items-center md:justify-center text-gray-400">
                  Image
                </div>
                <div class="mt-4 md:mt-0 md:ml-4 md:w-3/4">
                  <h3 class="md:text-2xl text-white">{{ subcategory.title }}</h3>
                  <p class="text-cz-red-700 text-sm md:text-base text-opacity-50"><span v-if="subcategory.endDate">{{ subcategory.startDate }}</span><span v-if="subcategory.endDate">- {{ subcategory.endDate }}</span></p>
                  <p class="text-gray-300">{{ subcategory.description }}</p>
                </div>
              </li>
            </ul>
          </template>
        </div>
        <div v-else-if="error" class="text-red-500 text-center">{{ error }}</div>
      </main>
    </div>
  </body>
</template>
