<script setup lang="ts">

import { defineProps } from 'vue'
import type { CatImage } from '@/services/CatService'

const props = defineProps<{
  cats: CatImage[]
  onSelect: (url: string) => void
}>()
</script>

<template>
  <div class="gallery">
    <img
      v-for="cat in props.cats"
      :key="cat.id"
      :src="cat.url"
      :alt="`cat-${cat.id}`"
      @click="props.onSelect(cat.url)"
    />
  </div>
</template>

<style scoped>

.gallery {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 10px;
  max-width: 1000px;
  margin: 0 auto;
  padding: 0 10px 20px;
}
.gallery img {
  width: 100%;
  border-radius: 8px;
  cursor: pointer;
  transition: transform .2s;
}
.gallery img:hover { transform: scale(1.05); }
</style>
