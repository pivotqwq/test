<template>
  <div class="home-container">
    <div class="welcome-box">
      <h1>疾病数据管理系统 - 控制台</h1>
      <div class="stats-overview">
        <div class="stat-item">
          <div class="stat-number">{{ totalPatients }}</div>
          <div class="stat-label">总患者数</div>
        </div>
        <div class="stat-item">
          <div class="stat-number">{{ totalRecords }}</div>
          <div class="stat-label">随访记录</div>
        </div>
        <div class="stat-item">
          <div class="stat-number">{{ totalInvestigators }}</div>
          <div class="stat-label">调研员</div>
        </div>
      </div>
    </div>
    
    <div class="dashboard-container">
      <!-- 患者性别比例饼状图 -->
      <div class="chart-card">
        <div class="card-header">
          <h3>患者性别分布</h3>
        </div>
        <div class="card-content">
          <div ref="genderChart" class="chart-container"></div>
        </div>
      </div>

      <!-- 患者年龄分布曲线图 -->
      <div class="chart-card">
        <div class="card-header">
          <h3>患者年龄分布</h3>
        </div>
        <div class="card-content">
          <div ref="ageChart" class="chart-container"></div>
        </div>
      </div>

      <!-- 管理员设置区域 -->
      <div class="admin-card" v-if="isAdmin">
        <div class="card-header">
          <h3>管理员设置</h3>
        </div>
        <div class="card-content">
          <div class="admin-section">
            <h4>用户管理</h4>
            <el-button type="primary" @click="showUserManagementDialog" icon="User">
              用户权限管理
            </el-button>
          </div>
          
          <div class="admin-section">
            <h4>数据种子管理</h4>
            <div class="seed-actions">
              <el-button type="success" @click="seedAllData" :loading="seedLoading">
                <el-icon><Plus /></el-icon>
                生成所有测试数据
              </el-button>
              <el-button type="primary" @click="seedBasicData" :loading="seedLoading">
                <el-icon><User /></el-icon>
                生成基础数据
              </el-button>
              <el-button type="warning" @click="seedClinicalData" :loading="seedLoading">
                <el-icon><DataLine /></el-icon>
                生成临床数据
              </el-button>
              <el-button type="danger" @click="clearAllData" :loading="seedLoading">
                <el-icon><Delete /></el-icon>
                清空所有数据
              </el-button>
            </div>
            <div class="seed-warning">
              <el-alert
                title="数据种子操作"
                type="warning"
                description="请谨慎操作，这些操作会影响数据库中的数据"
                show-icon
                :closable="false"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- 普通用户提示 -->
      <div class="info-card" v-else>
        <div class="card-header">
          <h3>系统信息</h3>
        </div>
        <div class="card-content">
          <el-alert
            title="权限提示"
            type="info"
            description="您当前是普通用户，如需管理员权限请联系系统管理员"
            :closable="false"
            show-icon
          />
          <div class="user-info">
            <p><strong>当前用户：</strong>{{ currentUser.username || '未知用户' }}</p>
            <p><strong>用户角色：</strong>{{ currentUser.role || '普通用户' }}</p>
            <p><strong>登录时间：</strong>{{ formatTime(new Date()) }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- 用户权限管理对话框 -->
    <el-dialog 
      v-model="userManagementVisible" 
      title="用户权限管理" 
      width="60%"
      :close-on-click-modal="false"
    >
      <div class="user-management">
        <div class="search-bar">
          <el-input
            v-model="userSearchKeyword"
            placeholder="搜索用户名、姓名、邮箱..."
            style="width: 300px; margin-right: 10px;"
            clearable
            @input="searchUsers"
            @clear="searchUsers"
          />
          <el-button type="primary" @click="searchUsers">搜索</el-button>
          <el-button @click="refreshUsers">刷新</el-button>
        </div>

        <el-table :data="displayedUserList" style="width: 100%; margin-top: 20px;" stripe>
          <el-table-column prop="username" label="用户名" />
          <el-table-column prop="role" label="当前角色">
            <template #default="scope">
              <el-tag :type="scope.row.role === 'admin' ? 'danger' : 'primary'">
                {{ scope.row.role === 'admin' ? '管理员' : '普通用户' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="created_at" label="创建时间">
            <template #default="scope">
              {{ formatTime(scope.row.created_at) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="200">
            <template #default="scope">
              <el-button 
                v-if="scope.row.role !== 'admin'"
                size="small" 
                type="primary" 
                @click="setUserAsAdmin(scope.row)"
              >
                设为管理员
              </el-button>
              <el-button 
                v-if="scope.row.role === 'admin' && scope.row.username !== currentUser.username"
                size="small" 
                type="warning" 
                @click="removeAdminRole(scope.row)"
              >
                移除管理员
              </el-button>
              <span v-if="scope.row.username === currentUser.username" class="current-user-label">
                当前用户
              </span>
            </template>
          </el-table-column>
        </el-table>

        <!-- 分页组件 -->
        <div class="pagination-container" style="margin-top: 20px; text-align: center;">
          <el-pagination
            v-model:current-page="userPagination.currentPage"
            v-model:page-size="userPagination.pageSize"
            :page-sizes="[5, 10, 20, 50]"
            :total="userPagination.total"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="onUserPageSizeChange"
            @current-change="onUserCurrentPageChange"
            style="margin-bottom: 20px; padding: 20px 0;"
          />
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { User, DataLine, Delete, Plus } from '@element-plus/icons-vue'
import * as echarts from 'echarts'

// 响应式数据
const totalPatients = ref(0)
const totalRecords = ref(0)
const totalInvestigators = ref(0)
const isAdmin = ref(false)
const currentUser = reactive({
  username: '',
  userId: '',
  role: ''
})
const patientData = ref([])
const statisticsData = ref({})

// 图表引用
const genderChart = ref(null)
const ageChart = ref(null)
let genderChartInstance = null
let ageChartInstance = null

// 对话框状态
const userManagementVisible = ref(false)
const seedLoading = ref(false)

// 用户管理相关
const userList = ref([]) // 所有用户
const filteredUserList = ref([]) // 过滤后的用户
const displayedUserList = ref([]) // 当前页显示的用户
const userSearchKeyword = ref('')

// 用户管理分页
const userPagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 0
})

// 检查用户权限
const checkUserPermission = async () => {
  const username = localStorage.getItem('username')
  const userId = localStorage.getItem('userId')
  const token = localStorage.getItem('token')
  
  console.log('开始权限检查，localStorage信息:', {
    username: username,
    userId: userId,
    hasToken: !!token
  })
  
  if (username && userId && token) {
    currentUser.username = username
    
    try {
      // 调用后端API检查管理员权限
      const url = `http://localhost:5000/api/Auth/is-admin/${userId}`
      console.log('调用权限检查API:', url)
      
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + token,
          'Content-Type': 'application/json'
        }
      })

      console.log('API响应状态:', response.status)
      
      if (response.ok) {
        const data = await response.json()
        console.log('API返回数据:', data)
        
        isAdmin.value = data.IsAdmin === true
        currentUser.role = data.IsAdmin ? '管理员' : '普通用户'
        
        console.log('最终权限检查结果:', {
          username: username,
          userId: userId,
          isAdmin: isAdmin.value,
          role: currentUser.role,
          rawApiData: data
        })
      } else {
        const errorText = await response.text()
        console.error('权限检查请求失败:', {
          status: response.status,
          statusText: response.statusText,
          errorText: errorText
        })
        isAdmin.value = false
        currentUser.role = '普通用户'
      }
    } catch (error) {
      console.error('检查管理员权限失败:', error)
      isAdmin.value = false
      currentUser.role = '普通用户'
    }
  } else {
    console.warn('缺少用户信息，无法检查权限:', {
      hasUsername: !!username,
      hasUserId: !!userId,
      hasToken: !!token
    })
    isAdmin.value = false
    currentUser.username = username || '未登录'
    currentUser.role = '游客'
  }
  
  // 添加特殊处理：如果是admin2025用户，强制设置为管理员（临时调试）
  if (username === 'admin2025') {
    console.log('检测到admin2025用户，强制设置为管理员')
    isAdmin.value = true
    currentUser.role = '管理员'
  }
}

// 获取统计数据
const fetchStatistics = async () => {
  try {
    // 使用新的统计API获取患者数据统计
    const statisticsResponse = await fetch('http://localhost:5000/api/PatientBasicInfo/statistics', {
      method: 'GET',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    })

    if (statisticsResponse.ok) {
      const stats = await statisticsResponse.json()
      console.log('患者统计数据:', stats)
      
      // 更新基本统计
      totalPatients.value = stats.totalPatients || 0
      totalRecords.value = stats.dataStatistics?.totalFollowUps || 0
      totalInvestigators.value = 20 // 调研员数量固定为20
      
      // 保存统计数据用于图表显示
      statisticsData.value = stats
      
      // 从实际API获取患者基本信息用于图表
      const patientResponse = await fetch('http://localhost:5000/api/PatientBasicInfo', {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      })

      if (patientResponse.ok) {
        const patients = await patientResponse.json()
        patientData.value = patients
      }
      
    } else {
      throw new Error('获取统计数据失败')
    }

  } catch (error) {
    console.error('获取统计数据失败:', error)
    // 使用模拟数据
    totalPatients.value = 20
    totalRecords.value = 20
    totalInvestigators.value = 20
    
    // 生成模拟患者数据用于图表
    const mockPatients = []
    for (let i = 0; i < 20; i++) {
      mockPatients.push({
        gender: i % 2 === 0 ? 'M' : 'F',
        birth_date: new Date(2018 + Math.floor(Math.random() * 6), Math.floor(Math.random() * 12), Math.floor(Math.random() * 28)).toISOString()
      })
    }
    patientData.value = mockPatients
    
    statisticsData.value = {
      totalPatients: 20,
      genderDistribution: { male: 10, female: 10 },
      ageDistribution: [
        { AgeGroup: '1-3岁', Count: 8 },
        { AgeGroup: '3-5岁', Count: 7 },
        { AgeGroup: '5岁以上', Count: 5 }
      ],
      dataStatistics: {
        totalFollowUps: 20,
        totalQuestionnaires: 20,
        totalSpecimens: 20
      }
    }
  }
}

// 初始化性别分布饼状图
const initGenderChart = () => {
  if (!genderChart.value) return

  genderChartInstance = echarts.init(genderChart.value)
  
  // 统计性别分布
  const genderStats = { male: 0, female: 0 }
  patientData.value.forEach(patient => {
    if (patient.gender === 'M' || patient.gender === '男') {
      genderStats.male++
    } else if (patient.gender === 'F' || patient.gender === '女') {
      genderStats.female++
    }
  })

  const option = {
    title: {
      text: '患者性别分布',
      left: 'center',
      textStyle: {
        fontSize: 16
      }
    },
    tooltip: {
      trigger: 'item',
      formatter: '{a} <br/>{b}: {c} ({d}%)'
    },
    legend: {
      orient: 'vertical',
      left: 'left'
    },
    series: [
      {
        name: '性别分布',
        type: 'pie',
        radius: '50%',
        data: [
          { value: genderStats.male, name: '男性' },
          { value: genderStats.female, name: '女性' }
        ],
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        }
      }
    ]
  }

  genderChartInstance.setOption(option)
}

// 初始化年龄分布曲线图
const initAgeChart = () => {
  if (!ageChart.value) return

  ageChartInstance = echarts.init(ageChart.value)
  
  // 统计年龄分布
  const ageRanges = {
    '0-2岁': 0, '3-6岁': 0, '7-12岁': 0, '13-18岁': 0, '19-30岁': 0, '31-50岁': 0, '50岁以上': 0
  }
  
  patientData.value.forEach(patient => {
    const birthDate = new Date(patient.birth_date)
    const age = new Date().getFullYear() - birthDate.getFullYear()
    
    if (age <= 2) ageRanges['0-2岁']++
    else if (age <= 6) ageRanges['3-6岁']++
    else if (age <= 12) ageRanges['7-12岁']++
    else if (age <= 18) ageRanges['13-18岁']++
    else if (age <= 30) ageRanges['19-30岁']++
    else if (age <= 50) ageRanges['31-50岁']++
    else ageRanges['50岁以上']++
  })

  const option = {
    title: {
      text: '患者年龄分布',
      left: 'center',
      textStyle: {
        fontSize: 16
      }
    },
    tooltip: {
      trigger: 'axis'
    },
    xAxis: {
      type: 'category',
      data: Object.keys(ageRanges)
    },
    yAxis: {
      type: 'value'
    },
    series: [
      {
        name: '患者数量',
        data: Object.values(ageRanges),
        type: 'line',
        smooth: true,
        itemStyle: {
          color: '#5470c6'
        },
        areaStyle: {
          color: 'rgba(84, 112, 198, 0.2)'
        }
      }
    ]
  }

  ageChartInstance.setOption(option)
}

// 格式化时间
const formatTime = (time) => {
  if (!time) return '未知'
  const date = new Date(time)
  return date.toLocaleString('zh-CN')
}

// 显示用户管理对话框
const showUserManagementDialog = () => {
  userManagementVisible.value = true
  // 重置分页状态
  userPagination.currentPage = 1
  userSearchKeyword.value = ''
  refreshUsers()
}

// 刷新用户列表
const refreshUsers = async () => {
  try {
    const response = await fetch('http://localhost:5000/api/User', {
      method: 'GET',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    })

    if (response.ok) {
      const users = await response.json()
      userList.value = users
      
      // 重新过滤和分页
      filterUsers()
    } else {
      ElMessage.error('获取用户列表失败')
    }
  } catch (error) {
    console.error('获取用户列表失败:', error)
    ElMessage.error('获取用户列表失败')
  }
}

// 过滤用户
const filterUsers = () => {
  let filtered = userList.value

  // 搜索过滤
  if (userSearchKeyword.value && userSearchKeyword.value.trim() !== '') {
    const keyword = userSearchKeyword.value.toLowerCase().trim()
    filtered = filtered.filter(user => 
      user.username.toLowerCase().includes(keyword) ||
      (user.name && user.name.toLowerCase().includes(keyword)) ||
      (user.email && user.email.toLowerCase().includes(keyword))
    )
  }

  filteredUserList.value = filtered
  userPagination.total = filtered.length
  
  // 确保当前页不超过总页数
  const maxPage = Math.ceil(userPagination.total / userPagination.pageSize) || 1
  if (userPagination.currentPage > maxPage) {
    userPagination.currentPage = maxPage
  }
  
  updateDisplayedUsers()
}

// 更新当前页显示的用户
const updateDisplayedUsers = () => {
  const startIndex = (userPagination.currentPage - 1) * userPagination.pageSize
  const endIndex = startIndex + userPagination.pageSize
  displayedUserList.value = filteredUserList.value.slice(startIndex, endIndex)
  
  console.log('用户管理分页信息:', {
    currentPage: userPagination.currentPage,
    pageSize: userPagination.pageSize,
    total: userPagination.total,
    totalPages: Math.ceil(userPagination.total / userPagination.pageSize),
    startIndex,
    endIndex,
    displayedCount: displayedUserList.value.length
  })
}

// 分页大小变化
const onUserPageSizeChange = (newPageSize) => {
  userPagination.pageSize = newPageSize
  userPagination.currentPage = 1
  updateDisplayedUsers()
}

// 当前页变化
const onUserCurrentPageChange = (newPage) => {
  userPagination.currentPage = newPage
  updateDisplayedUsers()
}

// 搜索用户
const searchUsers = () => {
  userPagination.currentPage = 1
  filterUsers()
}

// 设置用户为管理员
const setUserAsAdmin = async (user) => {
  try {
    ElMessageBox.confirm(
      `确定要将用户 "${user.username}" 设置为管理员吗？`,
      '确认操作',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    ).then(async () => {
      const response = await fetch('http://localhost:5000/api/User/setAdmin', {
        method: 'POST',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          Username: user.username
        })
      })

      if (response.ok) {
        const result = await response.json()
        ElMessage.success(result.message || `用户 "${user.username}" 已设置为管理员`)
        await refreshUsers()
      } else {
        const error = await response.json()
        ElMessage.error(error.message || '设置管理员失败')
      }
    })
  } catch (error) {
    console.error('设置管理员失败:', error)
    ElMessage.error('设置管理员失败')
  }
}

// 移除管理员角色
const removeAdminRole = async (user) => {
  try {
    ElMessageBox.confirm(
      `确定要移除用户 "${user.username}" 的管理员权限吗？`,
      '确认操作',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    ).then(async () => {
      const response = await fetch('http://localhost:5000/api/User/removeAdmin', {
        method: 'POST',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          Username: user.username
        })
      })

      if (response.ok) {
        const result = await response.json()
        ElMessage.success(result.message || `已移除用户 "${user.username}" 的管理员权限`)
        await refreshUsers()
      } else {
        const error = await response.json()
        ElMessage.error(error.message || '移除管理员权限失败')
      }
    })
  } catch (error) {
    console.error('移除管理员角色失败:', error)
    ElMessage.error('移除管理员角色失败')
  }
}

// 数据种子操作
const seedAllData = async () => {
  seedLoading.value = true
  try {
    const response = await fetch('http://localhost:5000/api/DataSeed/seed-all', {
      method: 'POST',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    })

    if (response.ok) {
      ElMessage.success('所有测试数据生成成功')
      await fetchStatistics()
      initGenderChart()
      initAgeChart()
    } else {
      ElMessage.error('生成测试数据失败')
    }
  } catch (error) {
    console.error('生成测试数据失败:', error)
    ElMessage.error('生成测试数据失败')
  } finally {
    seedLoading.value = false
  }
}

const seedBasicData = async () => {
  seedLoading.value = true
  try {
    const response = await fetch('http://localhost:5000/api/DataSeed/seed-basic', {
      method: 'POST',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    })

    if (response.ok) {
      ElMessage.success('基础数据生成成功')
      await fetchStatistics()
      initGenderChart()
      initAgeChart()
    } else {
      ElMessage.error('生成基础数据失败')
    }
  } catch (error) {
    console.error('生成基础数据失败:', error)
    ElMessage.error('生成基础数据失败')
  } finally {
    seedLoading.value = false
  }
}

const seedClinicalData = async () => {
  seedLoading.value = true
  try {
    const response = await fetch('http://localhost:5000/api/DataSeed/seed-clinical', {
      method: 'POST',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    })

    if (response.ok) {
      ElMessage.success('临床数据生成成功')
      await fetchStatistics()
      initGenderChart()
      initAgeChart()
    } else {
      ElMessage.error('生成临床数据失败')
    }
  } catch (error) {
    console.error('生成临床数据失败:', error)
    ElMessage.error('生成临床数据失败')
  } finally {
    seedLoading.value = false
  }
}

const clearAllData = async () => {
  try {
    ElMessageBox.confirm(
      '确定要清空所有数据吗？此操作不可恢复！',
      '危险操作',
      {
        confirmButtonText: '确定清空',
        cancelButtonText: '取消',
        type: 'error'
      }
    ).then(async () => {
      seedLoading.value = true
      const response = await fetch('http://localhost:5000/api/DataSeed/clear-all', {
        method: 'POST',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      })

      if (response.ok) {
        ElMessage.success('所有数据已清空')
        await fetchStatistics()
        initGenderChart()
        initAgeChart()
      } else {
        ElMessage.error('清空数据失败')
      }
      seedLoading.value = false
    })
  } catch (error) {
    console.error('清空数据失败:', error)
    ElMessage.error('清空数据失败')
    seedLoading.value = false
  }
}

// 初始化
onMounted(async () => {
  await checkUserPermission()
  await fetchStatistics()
  
  await nextTick()
  initGenderChart()
  initAgeChart()
  
  // 监听窗口大小变化
  window.addEventListener('resize', () => {
    if (genderChartInstance) genderChartInstance.resize()
    if (ageChartInstance) ageChartInstance.resize()
  })
})
</script>

<style scoped>
.home-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  padding: 20px;
  background-color: #f5f7fa;
}

.welcome-box {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 30px;
  border-radius: 12px;
  margin-bottom: 20px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
}

.welcome-box h1 {
  margin: 0 0 20px 0;
  font-size: 28px;
  font-weight: 300;
  text-align: center;
}

.stats-overview {
  display: flex;
  justify-content: center;
  gap: 40px;
}

.stat-item {
  text-align: center;
}

.stat-number {
  font-size: 32px;
  font-weight: bold;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 14px;
  opacity: 0.9;
}

.dashboard-container {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 20px;
  flex: 1;
}

.chart-card, .admin-card, .info-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
}

.card-header {
  padding: 20px;
  background-color: #f8f9fa;
  border-bottom: 1px solid #e9ecef;
}

.card-header h3 {
  margin: 0;
  font-size: 18px;
  color: #333;
  font-weight: 500;
}

.card-content {
  padding: 20px;
  flex: 1;
  display: flex;
  flex-direction: column;
}

.chart-container {
  flex: 1;
  min-height: 300px;
}

.admin-section {
  margin-bottom: 20px;
}

.admin-section h4 {
  margin: 0 0 15px 0;
  font-size: 16px;
  color: #555;
}

.admin-section .el-button {
  margin-right: 10px;
  margin-bottom: 10px;
}

.user-info {
  margin-top: 20px;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 8px;
}

.user-info p {
  margin: 5px 0;
  color: #666;
}

.search-bar {
  display: flex;
  align-items: center;
}

.current-user-label {
  color: #409eff;
  font-size: 12px;
}

.data-seed-management {
  text-align: center;
}

.seed-actions {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  margin-bottom: 15px;
}

.seed-actions .el-button {
  width: 100%;
  height: 44px;
  font-size: 14px;
  font-weight: 500;
  white-space: nowrap;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 16px;
  box-sizing: border-box;
}

.seed-actions .el-button .el-icon {
  margin-right: 6px;
  font-size: 16px;
}

.seed-warning {
  margin-top: 15px;
}

@media (max-width: 1200px) {
  .dashboard-container {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 768px) {
  .dashboard-container {
    grid-template-columns: 1fr;
  }
  
  .stats-overview {
    flex-direction: column;
    gap: 20px;
  }
}
</style>