<template>
  <div v-if="loading" class="loading-container" :class="{ 'full-screen': fullScreen }">
    <div class="loading-content">
      <el-icon class="loading-icon" :size="size">
        <Loading />
      </el-icon>
      <p v-if="text" class="loading-text">{{ text }}</p>
    </div>
  </div>
</template>

<script setup>
import { Loading } from '@element-plus/icons-vue'

// 接收props
defineProps({
  loading: {
    type: Boolean,
    default: false
  },
  text: {
    type: String,
    default: '加载中...'
  },
  size: {
    type: Number,
    default: 40
  },
  fullScreen: {
    type: Boolean,
    default: false
  }
})
</script>

<style scoped>
.loading-container {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  min-height: 100px;
}

.loading-container.full-screen {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.9);
  z-index: 9999;
  min-height: 100vh;
}

.loading-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.loading-icon {
  animation: rotate 1s linear infinite;
  color: #409eff;
  margin-bottom: 8px;
}

.loading-text {
  margin: 0;
  color: #666;
  font-size: 14px;
}

@keyframes rotate {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
</style> 