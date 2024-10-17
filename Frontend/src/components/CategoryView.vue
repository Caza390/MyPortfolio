<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';

// Define the Category interface
interface Category {
  id: number; // Assuming there's an ID
  name: string;
  description: string;
  url: string;
}

const route = useRoute();
const categoryName = ref(route.params.category ? String(route.params.category).toLowerCase() : '');
const categoryData = ref<Category | null>(null); // Use the Category interface
const loading = ref(true);
const error = ref('');

// Function to fetch category data
const fetchCategoryData = async () => {
  loading.value = true;
  try {
    const response = await fetch(`http://localhost:5176/api/categories/byname/${categoryName.value}`);
    if (!response.ok) {
      throw new Error('Failed to fetch category data');
    }
    categoryData.value = await response.json();
  } catch (err: any) { // Explicitly assert err as any
    error.value = err.message;
    loading.value = false;
  }
};

onMounted(() => {
  fetchCategoryData();
});

// Watch for changes in the route parameter
watch(() => route.params.category, (newCategory) => {
  categoryName.value = newCategory ? String(newCategory).toLowerCase() : '';
  fetchCategoryData();
});
</script>

<template>
  <main>
    <div v-if="error" class="text-red-500">{{ error }}</div>
    
    <div v-if="categoryData">
      <h1 class="text-white text-3xl">{{ categoryName }}</h1>
      <p class="text-gray-300">{{ categoryData.description }}</p>
    </div>
  </main>
</template>

<style scoped>
/* Add styles as needed */
</style>
