<template>
  <div class="loader" />
</template>

<style scoped>
/* 简单旋转圈 Loader */
.loader {
  border: 5px solid #f3f3f3;
  border-top: 5px solid #4caf50;
  border-radius: 50%;
  width: 48px;
  height: 48px;
  margin: 40px auto;
  animation: spin 1s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }
</style>
