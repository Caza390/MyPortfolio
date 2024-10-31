<script setup lang="ts">
import { ref } from 'vue';
import { RouterLink, RouterView, useRoute } from 'vue-router';
import pdfPath from '@/assets/Callum_Carter-Begbie_CV.pdf';

const isDropdownOpen = ref(false);
const openPdf = () => {
  window.open(pdfPath, '_blank');
};

const closeDropdown = () => {
  isDropdownOpen.value = false;
};
</script>

<template>
  <div class="bg-cz-background-900 min-h-screen">
    <div>
      <header class="bg-cz-background-800 pt-5px pb-3 px-20px">
        <div>
          <nav class="flex items-center justify-between">
            <RouterLink to="/" class="text-cz-red-800 text-6xl lg:text-6xl font-bold no-underline hover:brightness-110"
              @click="closeDropdown">
              Caz</RouterLink>

            <div class="md:hidden">
              <button @click="isDropdownOpen = !isDropdownOpen" class="text-white focus:outline-none">
                <!-- Hamburger Icon -->
                <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                  xmlns="http://www.w3.org/2000/svg">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16m-7 6h7">
                  </path>
                </svg>
              </button>
            </div>

            <div class="hidden md:flex flex-grow justify-center">
              <div class="flex space-x-8">
                <RouterLink to="/" class="text-white lg:text-xl hover:text-cz-red-100"
                  active-class="!text-cz-red-500 !hover:text-cz-red-700"
                  exact-active-class="!text-cz-red-500 !hover:text-cz-red-700">Home</RouterLink>
                <RouterLink to="/tech" class="text-white lg:text-xl hover:text-cz-red-100"
                  active-class="!text-cz-red-500 hover:text-cz-red-700"
                  exact-active-class="!text-cz-red-500 hover:text-cz-red-700">Tech</RouterLink>
                <RouterLink to="/auto" class="text-white lg:text-xl hover:text-cz-red-100"
                  active-class="!text-cz-red-500 hover:text-cz-red-700"
                  exact-active-class="!text-cz-red-500 hover:text-cz-red-700">Auto</RouterLink>
                <RouterLink to="/models" class="text-white lg:text-xl hover:text-cz-red-100"
                  active-class="!text-cz-red-500 hover:text-cz-red-700"
                  exact-active-class="!text-cz-red-500 hover:text-cz-red-700">Models</RouterLink>
                <button @click="openPdf" class="text-white lg:text-xl hover:text-cz-red-100">CV</button>
              </div>
            </div>
            <RouterLink to="/contact-me"
              class="hidden md:block text-cz-red-50 lg:text-xl bg-cz-red-950 border-cz-red-900 border rounded-lg px-3 py-2 hover:brightness-110">
              Contact Me</RouterLink>
          </nav>

          <!-- Dropdown Menu for Mobile -->
          <div v-if="isDropdownOpen"
            class="absolute top-16 left-0 w-full bg-cz-background-800 mt-2 rounded-lg shadow-lg z-50">
            <div class="flex flex-col space-y-4 p-4">
              <RouterLink to="/" class="text-white" active-class="!text-cz-red-500"
                exact-active-class="!text-cz-red-500" @click="closeDropdown">Home</RouterLink>
              <RouterLink to="/tech" class="text-white" active-class="!text-cz-red-500"
                exact-active-class="!text-cz-red-500" @click="closeDropdown">Tech</RouterLink>
              <RouterLink to="/auto" class="text-white" active-class="!text-cz-red-500"
                exact-active-class="!text-cz-red-500" @click="closeDropdown">Auto</RouterLink>
              <RouterLink to="/models" class="text-white" active-class="!text-cz-red-500"
                exact-active-class="!text-cz-red-500" @click="closeDropdown">Models</RouterLink>
              <a href="javascript:void(0)" @click="openPdf" class="text-white">CV</a>
              <RouterLink to="/contact-me" class="text-white" active-class="!text-cz-red-500"
                exact-active-class="!text-cz-red-500" @click="closeDropdown">Contact Me</RouterLink>
            </div>
          </div>
        </div>
      </header>
    </div>

    <main class="">
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
/** When it comes to making different screen views phones have no prefix, tablets are sm:, standard screens are md:, higher monitors are lg:. */
</style>
