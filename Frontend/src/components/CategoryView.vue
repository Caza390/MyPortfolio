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
const categoryUrl = ref(route.params.category ? String(route.params.category) : ''); // Change variable to categoryUrl
const categoryData = ref<Category | null>(null); // Use the Category interface
const loading = ref(true);
const error = ref('');

// Function to fetch category data
const fetchCategoryData = async () => {
  loading.value = true;
  try {
    const response = await fetch(`http://192.168.1.90:5176/api/categories/byurl/${categoryUrl.value}`);
    if (!response.ok) {
      throw new Error('Failed to fetch category data');
    }
    categoryData.value = await response.json();
  } catch (err: any) {
    error.value = err.message;
    loading.value = false;
  }
};

onMounted(() => {
  fetchCategoryData();
});

// Watch for changes in the route parameter
watch(() => route.params.category, (newCategory) => {
  categoryUrl.value = newCategory ? String(newCategory) : ''; // Change to categoryUrl
  fetchCategoryData();
});
</script>

<template>
  <main>
    <div v-if="error" class="text-red-500">{{ error }}</div>
    
    <div v-if="categoryData">
      <h1 class="text-white text-3xl">{{ categoryData.name }}</h1>
      <p class="text-gray-300">{{ categoryData.description }}</p>
    </div>
  </main>
</template>

<style scoped>
/* Add styles as needed */
</style>
