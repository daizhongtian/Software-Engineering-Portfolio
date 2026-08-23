<script setup lang="ts">

import { defineProps, defineEmits } from 'vue'

const props = defineProps<{
  visible: boolean
  imgUrl: string
}>()

const emit = defineEmits<{ (e: 'close'): void }>()


function handleBackdrop (e: MouseEvent) {
  if (e.target === e.currentTarget) emit('close')
}
</script>

<template>

  <teleport to="body">
    <div
      v-if="props.visible"
      class="modal"
      @click="handleBackdrop"
    >
      <span class="close" @click="emit('close')">&times;</span>
      <img :src="props.imgUrl" alt="cat-large" />
    </div>
  </teleport>
</template>

<style scoped>
.modal {
  position: fixed;
  inset: 0;                              
  display: flex;
  justify-content: center;
  align-items: center;
  background: rgba(0,0,0,.8);
  z-index: 999;
}
.modal img {
  max-width: 90%;
  max-height: 90%;
  border-radius: 8px;
}
.close {
  position: absolute;
  top: 20px;
  right: 30px;
  font-size: 2rem;
  color: #fff;
  cursor: pointer;
}
</style>
