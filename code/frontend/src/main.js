import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

const app = createApp(App)

// 注册所有图标
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

app.use(router)
app.use(ElementPlus)

// 全局方法
app.config.globalProperties.$getUserIdFromToken = (token) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const payload = JSON.parse(atob(base64));
    return payload.sub || payload.nameid; 
  } catch (e) {
    console.error('解析Token失败', e);
    return null;
  }
}

// 全局错误处理
app.config.errorHandler = (err, vm, info) => {
  console.error('Vue全局错误:', {
    error: err,
    instance: vm,
    info: info,
    stack: err?.stack
  })
  
  // 避免处理空错误
  if (!err) {
    console.warn('捕获到空错误')
    return
  }
  
  const errorMessage = err.message || String(err)
  
  // 根据错误类型显示不同的提示
  if (errorMessage.includes('Network')) {
    ElMessage.error('网络连接失败，请检查网络状态')
  } else if (errorMessage.includes('401')) {
    ElMessage.error('登录已过期，请重新登录')
    localStorage.clear()
    router.push('/login')
  } else if (errorMessage.includes('403')) {
    ElMessage.error('权限不足，无法访问该资源')
  } else if (errorMessage.includes('404')) {
    ElMessage.error('请求的资源不存在')
  } else if (errorMessage.includes('500')) {
    ElMessage.error('服务器内部错误，请稍后重试')
  } else if (errorMessage.includes('Cannot read property') || 
             errorMessage.includes('Cannot read properties') ||
             errorMessage.includes('is not a function') ||
             errorMessage.includes('is not defined')) {
    // 这些通常是开发时的错误，在生产环境中不显示给用户
    console.error('代码错误:', errorMessage)
    // 只在开发环境显示详细错误
    if (process.env.NODE_ENV === 'development') {
      ElMessage.error(`开发错误: ${errorMessage}`)
    }
  } else {
    // 记录未知错误，但不自动显示给用户
    console.error('未分类错误:', errorMessage)
  }
}

// 全局未捕获的Promise错误
window.addEventListener('unhandledrejection', event => {
  console.error('未处理的Promise错误:', {
    reason: event.reason,
    promise: event.promise,
    stack: event.reason?.stack
  })
  
  // 避免处理空错误
  if (!event.reason) {
    console.warn('捕获到空Promise错误')
    return
  }
  
  const errorMessage = event.reason.message || String(event.reason)
  
  // 对于网络相关错误，给出具体提示
  if (errorMessage.includes('fetch') || errorMessage.includes('Network')) {
    ElMessage.error('网络请求失败，请检查网络连接')
  } else if (errorMessage.includes('401')) {
    ElMessage.error('登录已过期，请重新登录')
    localStorage.clear()
    router.push('/login')
  } else if (errorMessage.includes('403')) {
    ElMessage.error('权限不足')
  } else if (errorMessage.includes('404')) {
    ElMessage.error('请求的资源不存在')
  } else if (errorMessage.includes('500')) {
    ElMessage.error('服务器错误，请稍后重试')
  } else {
    // 记录其他类型的Promise错误，但不自动显示
    console.error('未分类Promise错误:', errorMessage)
  }
  
  event.preventDefault()
})

// 全局JavaScript错误
window.addEventListener('error', event => {
  // 详细的错误信息记录
  console.error('全局JavaScript错误:', {
    error: event.error,
    message: event.message,
    filename: event.filename,
    lineno: event.lineno,
    colno: event.colno,
    stack: event.error?.stack
  })
  
  // 如果是资源加载错误，不显示错误提示
  if (event.target && event.target !== window) {
    console.warn('资源加载失败:', event.target.src || event.target.href)
    return
  }
  
  // 避免显示空错误或重复错误提示
  if (!event.error && !event.message) {
    console.warn('捕获到空错误事件')
    return
  }
  
  // 对于一些常见的无害错误，不显示提示
  const errorMessage = event.message || (event.error && event.error.message) || '未知错误'
  if (errorMessage.includes('Script error') || 
      errorMessage.includes('Non-Error promise rejection captured') ||
      errorMessage.includes('ResizeObserver loop limit exceeded')) {
    return
  }
  
  // 只在发生严重错误时才显示提示
  console.error('严重JavaScript错误:', errorMessage)
  // 移除了自动显示错误提示，避免干扰用户体验
})

app.mount('#app')