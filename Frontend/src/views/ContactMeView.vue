<script setup lang="ts">
import { computed } from 'vue';

const emailAddress = 'Callum.carterbegbie@gmail.com';
const emailLink = computed(() => `mailto:${emailAddress}`);
</script>

<template>

  <main class="p-5 md:py-44 md:px-52">
    <p class="text-white text-xl md:text-3xl">Contact me with either email, phone or discord.
      I often check my email account and made a discord profile for business use.
      <br>Personally I prefer to be contacted by these but if you would like to use phone call/text that is also okay -
      I do
      have WhatsApp.
    </p>
    <div class="text-cz-red-400 text-xl md:text-3xl my-5">
      <p class="hover:text-cz-red-300">Email : <a :href="emailLink">{{ emailAddress }}</a></p>
      <p class="hover:text-cz-red-300">Discord : CallumCB</p>
      <p class="hover:text-cz-red-300">Number : 07842 608878</p>
      <p class="hover:text-cz-red-300">LinkedIn : <a href="https://www.linkedin.com/in/callum-carter-begbie-7854bb183/" target="_blank">Callum Carter-Begbie</a></p>
    </div>
    <p class="text-white text-xl md:text-3xl">I look forward to hearing from you!</p>
  </main>

</template>
