<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Gallery     from './components/Gallery.vue'
import ImageModal  from './components/ImageModal.vue'
import Loader      from './components/Loader.vue'
import { fetchRandomCats, type CatImage } from './services/CatService'

const cats         = ref<CatImage[]>([])
const loading      = ref(false)
const modalVisible = ref(false)
const modalUrl     = ref('')

function openModal(url: string) {
  modalUrl.value = url
  modalVisible.value = true
}
function closeModal() {
  modalVisible.value = false
}

async function loadCats() {
  loading.value = true
  try {
    cats.value = await fetchRandomCats()
  } catch {
    alert('😿 failed to get picture')
  } finally {
    loading.value = false
  }
}

onMounted(loadCats)
</script>

<template>
  <h1>Mini Cat Gallery</h1>
  <button @click="loadCats" :disabled="loading">refresh picture </button>

  <Loader v-if="loading" />

  <Gallery v-else :cats="cats" :on-select="openModal" />

  <ImageModal
    :visible="modalVisible"
    :img-url="modalUrl"
    @close="closeModal"
  />
</template>
