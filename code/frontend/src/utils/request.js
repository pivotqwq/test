import { ElMessage } from 'element-plus'
import router from '@/router'

// API 基础配置
const BASE_URL = 'http://localhost:5000/api'

// 创建请求实例
class HttpRequest {
  constructor() {
    this.timeout = 10000
    this.baseURL = BASE_URL
  }

  // 获取配置
  getInsideConfig() {
    const config = {
      baseURL: this.baseURL,
      timeout: this.timeout,
      headers: {
        'Content-Type': 'application/json'
      }
    }
    return config
  }

  // 拦截器
  interceptors(instance) {
    // 请求拦截器
    instance.requestInterceptor = (url, options = {}) => {
      // 添加认证token
      const token = localStorage.getItem('token')
      if (token) {
        options.headers = {
          ...options.headers,
          'Authorization': `Bearer ${token}`
        }
      }
      return { url, options }
    }

    // 响应拦截器
    instance.responseInterceptor = async (response) => {
      // 检查HTTP状态码
      if (!response.ok) {
        // 处理HTTP错误
        if (response.status === 401) {
          ElMessage.error('登录已过期，请重新登录')
          localStorage.removeItem('token')
          localStorage.removeItem('username')
          localStorage.removeItem('userId')
          router.push('/login')
          return Promise.reject(new Error('登录已过期'))
        } else if (response.status === 403) {
          ElMessage.error('没有权限访问此资源')
          return Promise.reject(new Error('没有权限'))
        } else if (response.status === 404) {
          ElMessage.error('请求的资源不存在')
          return Promise.reject(new Error('资源不存在'))
        } else if (response.status >= 500) {
          ElMessage.error('服务器内部错误，请稍后重试')
          return Promise.reject(new Error('服务器错误'))
        } else {
          ElMessage.error(`请求失败: ${response.status} ${response.statusText}`)
          return Promise.reject(new Error(`HTTP ${response.status}`))
        }
      }

      try {
        const data = await response.json()
        
        // 检查业务状态码
        if (data.success === false) {
          let errorMessage = data.message || '操作失败'
          
          // 特殊处理登录错误，统一显示友好的错误信息
          if (response.url.includes('/Auth/login')) {
            errorMessage = '用户名或密码错误'
          }
          
          ElMessage.error(errorMessage)
          return Promise.reject(new Error(errorMessage))
        }
        
        return data
      } catch (error) {
        ElMessage.error('响应数据解析失败')
        return Promise.reject(new Error('数据解析失败'))
      }
    }

    return instance
  }

  // 创建请求实例
  request() {
    const instance = this.interceptors(this)
    return instance
  }
}

const http = new HttpRequest()

// 通用请求方法
export const request = async (url, options = {}) => {
  const config = http.getInsideConfig()
  const instance = http.request()
  
  // 处理请求拦截
  const { url: finalUrl, options: finalOptions } = instance.requestInterceptor(url, {
    ...options,
    headers: {
      ...config.headers,
      ...options.headers
    }
  })

  const fullUrl = finalUrl.startsWith('http') ? finalUrl : `${config.baseURL}${finalUrl}`

  try {
    const response = await fetch(fullUrl, finalOptions)
    return await instance.responseInterceptor(response)
  } catch (error) {
    // 网络错误或其他错误
    if (error.name === 'TypeError' || error.message.includes('fetch')) {
      ElMessage.error('网络连接失败，请检查网络或稍后重试')
    } else if (!error.message.includes('登录已过期') && !error.message.includes('没有权限')) {
      // 避免重复显示已经显示过的错误
      console.error('请求错误:', error)
    }
    throw error
  }
}

// GET 请求
export const get = (url, params = {}) => {
  const queryString = new URLSearchParams(params).toString()
  const finalUrl = queryString ? `${url}?${queryString}` : url
  return request(finalUrl, { method: 'GET' })
}

// POST 请求
export const post = (url, data = {}) => {
  return request(url, {
    method: 'POST',
    body: JSON.stringify(data)
  })
}

// PUT 请求
export const put = (url, data = {}) => {
  return request(url, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
}

// DELETE 请求
export const del = (url) => {
  return request(url, { method: 'DELETE' })
}

// 文件上传
export const upload = (url, formData) => {
  return request(url, {
    method: 'POST',
    body: formData,
    headers: {} // 文件上传不设置Content-Type，让浏览器自动设置
  })
}

export default {
  get,
  post,
  put,
  delete: del,
  upload,
  request
} 