<script setup lang="ts">
import { ref, onMounted, watch, computed, onUnmounted } from 'vue';
import { useRoute } from 'vue-router';

interface Subcategory {
  id: number;
  heading: string | null;
  title: string;
  description: string;
  startDate?: string | null;
  endDate?: string | null;
  category: string;
  imagePath?: string;
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
const uniqueHeadings = ref<string[]>([]);
const backendBaseUrl = "http://192.168.1.90:5176";
const isMobile = ref(window.innerWidth < 768);
const showScrollTopButton = ref(false);

const fetchSubcategoriesData = async () => {
  if (!tabsUrl.value || !categoryUrl.value) {
    error.value = 'Tabs or Category URL is missing.';
    loading.value = false;
    return;
  }

  loading.value = true;
  try {
    const response = await fetch(
      `http://192.168.1.90:5176/api/subcategory/SubCategoriesByCategory?tabs=${tabsUrl.value}&category=${categoryUrl.value}`
    );

    if (!response.ok) {
      throw new Error('Failed to fetch subcategories data');
    }

    const data: Subcategory[] = await response.json();

    // Set imagePath with backendBaseUrl for each subcategory if it exists
    subcategoriesData.value = data.map((subcategory: Subcategory) => {
      if (subcategory.imagePath) {
        subcategory.imagePath = `${backendBaseUrl}/${subcategory.imagePath.replace(/^\//, '')}`;
      }
      return subcategory;
    }).sort((a, b) => {
      const dateA = a.startDate ? new Date(a.startDate) : new Date(0);
      const dateB = b.startDate ? new Date(b.startDate) : new Date(0);

      if (dateA.getTime() !== dateB.getTime()) return dateA.getTime() - dateB.getTime();
      return (a.heading || '').localeCompare(b.heading || '');
    });

    const uniqueHeadingSet = new Set<string>();
    subcategoriesData.value.forEach((subcategory) => {
      if (subcategory.heading) {
        uniqueHeadingSet.add(subcategory.heading);
      }
    });
    uniqueHeadings.value = Array.from(uniqueHeadingSet).sort();

  } catch (err: any) {
    error.value = err.message;
  } finally {
    loading.value = false;
  }
};


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

const scrollToTop = () => {
  window.scrollTo({
    top: 0,
    behavior: "smooth",
  });
};

const scrollToHeading = (heading: string) => {
  console.log(`Trying to scroll to: heading-${heading}`);
  const headingElement = document.getElementById(`heading-${heading}`);
  if (headingElement) {
    const offset = -25;
    window.scrollTo({
      top: headingElement.offsetTop + offset,
      behavior: "smooth",
    });
  } else {
    console.error(`Element not found: heading-${heading}`);
  }
};

const handleScroll = () => {
  showScrollTopButton.value = window.scrollY > 100;
};

onMounted(() => {
  fetchSubcategoriesData();
  fetchCategoryData();
  window.addEventListener('scroll', handleScroll);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
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

const groupedSubcategories = computed(() => {
  const groups: { [key: string]: Subcategory[] } = {};
  subcategoriesData.value.forEach((subcategory) => {
    const heading = subcategory.heading || 'Uncategorized';
    if (!groups[heading]) {
      groups[heading] = [];
    }
    groups[heading].push(subcategory);
  });
  return Object.values(groups);
});
</script>


<template>

  <body class="md:flex">
    <aside
      class="hidden md:block sticky top-0 h-screen px-6 py-4 bg-cz-background-700 border-r border-cz-background-900 text-white"
      style="width: 160px; max-width: 160; overflow: hidden;">
      <ul class="space-y-1 h-full flex flex-col items-center">
        <li>
          <button @click="scrollToTop" class="font-bold text-xl my-2 text-center hover:text-cz-red-100">Top</button>
        </li>
        <li v-for="heading in uniqueHeadings" :key="heading">
          <button @click="scrollToHeading(heading)" class="font-bold text-xl my-2 text-center hover:text-cz-red-100">{{ heading }}</button>
        </li>
      </ul>
    </aside>

    <button v-show="isMobile && showScrollTopButton" @click="scrollToTop"
      class="fixed top-20 right-4 bg-cz-red-950 text-cz-red-50 border border-cz-red-900 p-3 rounded-full shadow-lg">
      ↑ Top
    </button>

    <div class="md:w-5/6 p-4">
      <header v-if="categoryData" class="md:mx-40 text-center">
        <h1 class="text-white text-3xl md:text-5xl">{{ categoryData.title }}</h1>
        <p class="text-gray-300 md:text-lg">{{ categoryData.description }}</p>
      </header>

      <main class="md:px-20">
        <div v-if="!loading && subcategoriesData.length > 0">
          <template v-for="(group, index) in groupedSubcategories" :key="index">
            <h2 v-if="group.length > 0" :id="'heading-' + (group[0].heading || 'Unnamed')"
              class="text-2xl font-bold text-cz-red-50 mt-8 mb-2">
              {{ group[0].heading }}
            </h2>

            <ul class="space-y-4">
              <li v-for="subcategory in group" :key="subcategory.id"
                class="md:flex border border-cz-red-950 rounded-lg p-4 bg-cz-background-700">
                
                <div v-if="subcategory.imagePath" class="md:w-1/4 aspect-ratio-box">
                  <img :src="subcategory.imagePath" alt="Subcategory Image" class="subcategory-image rounded-lg" />
                </div>
                <div v-else
                  class="md:w-1/4 h-24 md:h-24 bg-cz-red-950 bg-opacity-50 md:flex md:items-center md:justify-center text-gray-400">
                  Image
                </div>

                <div class="mt-4 md:mt-0 md:ml-4 md:w-3/4">
                  <h3 class="text-xl md:text-2xl text-white">{{ subcategory.title }}</h3>
                  <p class="text-cz-red-700 text-sm md:text-base text-opacity-50">
                    <span v-if="subcategory.startDate">{{ subcategory.startDate }}</span>
                    <span v-if="subcategory.endDate"> - {{ subcategory.endDate }}</span>
                  </p>
                  <p class="text-gray-300 mt-2 md:mt-0">{{ subcategory.description }}</p>
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
