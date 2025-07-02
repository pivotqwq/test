<template>
  <div v-if="hasError" class="error-boundary">
    <el-result
      icon="error"
      title="页面出现错误"
      :sub-title="errorMessage"
    >
      <template #extra>
        <el-button type="primary" @click="handleRetry">
          <el-icon><Refresh /></el-icon>
          重新加载
        </el-button>
        <el-button @click="handleGoHome">
          <el-icon><HomeFilled /></el-icon>
          返回首页
        </el-button>
      </template>
    </el-result>
  </div>
  <div v-else>
    <slot />
  </div>
</template>

<script setup>
import { ref, onErrorCaptured } from 'vue'
import { ElMessage } from 'element-plus'
import { Refresh, HomeFilled } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const hasError = ref(false)
const errorMessage = ref('')

// 捕获子组件错误
onErrorCaptured((err, vm, info) => {
  console.error('Error captured by ErrorBoundary:', err, info)
  
  hasError.value = true
  
  // 根据错误类型设置友好的错误信息
  if (err.message) {
    if (err.message.includes('Network')) {
      errorMessage.value = '网络连接失败，请检查网络状态后重试'
    } else if (err.message.includes('401')) {
      errorMessage.value = '登录已过期，请重新登录'
    } else if (err.message.includes('403')) {
      errorMessage.value = '权限不足，无法访问该资源'
    } else if (err.message.includes('404')) {
      errorMessage.value = '请求的资源不存在'
    } else if (err.message.includes('500')) {
      errorMessage.value = '服务器内部错误，请稍后重试'
    } else {
      errorMessage.value = `发生错误: ${err.message}`
    }
  } else {
    errorMessage.value = '页面出现未知错误，请联系管理员'
  }
  
  // 显示错误提示
  ElMessage.error(errorMessage.value)
  
  // 返回 false 阻止错误继续向上传播
  return false
})

// 重新加载
const handleRetry = () => {
  hasError.value = false
  errorMessage.value = ''
  // 刷新当前页面
  window.location.reload()
}

// 返回首页
const handleGoHome = () => {
  hasError.value = false
  errorMessage.value = ''
  router.push('/home')
}
</script>

<style scoped>
.error-boundary {
  min-height: 400px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.el-result {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 40px;
}
</style> 