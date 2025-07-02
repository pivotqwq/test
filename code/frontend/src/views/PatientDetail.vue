<template>
  <div class="patient-detail-container">
    <!-- 头部信息卡片 -->
    <el-card class="patient-header" v-loading="loading">
      <template #header>
        <div class="header-title">
          <el-page-header @back="router.go(-1)">
            <template #content>
              <h2>患者详细信息</h2>
            </template>
          </el-page-header>
          <el-tag :type="patientData.gender === 'M' ? 'primary' : 'danger'" v-if="patientData.gender">
            {{ formatGender(patientData.gender) }}
          </el-tag>
        </div>
      </template>

      <!-- 基本信息 -->
      <el-descriptions :column="2" border>
        <el-descriptions-item label="患者ID">{{ patientData.patient_id || '暂无' }}</el-descriptions-item>
        <el-descriptions-item label="姓名">{{ patientData.name || '暂无' }}</el-descriptions-item>
        <el-descriptions-item label="性别">{{ formatGender(patientData.gender) }}</el-descriptions-item>
        <el-descriptions-item label="年龄">{{ calculateAge(patientData.birth_date) }}岁</el-descriptions-item>
        <el-descriptions-item label="出生日期">{{ formatDate(patientData.birth_date) }}</el-descriptions-item>
        <el-descriptions-item label="诊断时年龄">{{ patientData.age_at_diagnosi || 0 }}岁</el-descriptions-item>
        <el-descriptions-item label="居住类型">{{ formatResidenceType(patientData.residence_type) }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ formatDateTime(patientData.create_time) }}</el-descriptions-item>
        <el-descriptions-item label="过敏史" :span="2">{{ patientData.allergy_history || '无' }}</el-descriptions-item>
      </el-descriptions>
    </el-card>

    <!-- 数据统计概览 -->
    <el-row :gutter="20" class="summary-cards">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-number">{{ completeData.summary?.totalFollowUps || 0 }}</div>
          <div class="stat-label">随访记录</div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-number">{{ completeData.summary?.totalQuestionnaires || 0 }}</div>
          <div class="stat-label">调研数据</div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-number">{{ completeData.summary?.totalSpecimens || 0 }}</div>
          <div class="stat-label">标本信息</div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-number">{{ completeData.summary?.totalCosts?.toFixed(2) || '0.00' }}</div>
          <div class="stat-label">总费用(元)</div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 详细信息标签页 -->
    <el-tabs v-model="activeTab" class="detail-tabs">
      <!-- 随访记录 -->
      <el-tab-pane label="随访记录" name="followups">
        <el-table :data="completeData.followUps" v-loading="loading" empty-text="暂无随访记录">
          <el-table-column prop="followup_date" label="随访日期" width="120">
            <template #default="{row}">
              {{ formatDate(row.followup_date) }}
            </template>
          </el-table-column>
          <el-table-column prop="symptom_improvement" label="症状改善情况" />
          <el-table-column prop="adverse_effects" label="不良反应" />
          <el-table-column prop="act_score" label="ACT评分" width="100" />
          <el-table-column label="操作" width="100">
            <template #default="{row}">
              <el-button size="small" type="danger" @click="deleteFollowUpRecord(row.followup_id)" v-if="isAdmin">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
      
      <!-- 调研数据 -->
      <el-tab-pane label="调研数据" name="questionnaires">
        <el-table :data="completeData.questionnaires" v-loading="loading" empty-text="暂无调研数据">
          <el-table-column prop="fill_date" label="填写日期" width="120" />
          <el-table-column prop="form_type" label="表单类型" width="150" />
          <el-table-column prop="data_source" label="数据来源" />
          <el-table-column prop="investigator_id" label="调研员ID" width="120" />
          <el-table-column label="操作" width="150">
            <template #default="{row}">
              <el-button size="small" @click="viewRawData(row.raw_data)">查看详情</el-button>
              <el-button size="small" type="danger" @click="deleteQuestionnaireData(row.questionnaire_id)" v-if="isAdmin">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>

      <!-- 用药记录 -->
      <el-tab-pane label="用药记录" name="medications">
        <el-table :data="completeData.medications" v-loading="loading" empty-text="暂无用药记录">
          <el-table-column prop="drug_name" label="药物名称" />
          <el-table-column prop="dosage" label="用量" />
          <el-table-column prop="frequency" label="频次" />
          <el-table-column prop="start_date" label="开始日期" width="120">
            <template #default="{row}">
              {{ formatDate(row.start_date) }}
            </template>
          </el-table-column>
          <el-table-column prop="end_date" label="结束日期" width="120">
            <template #default="{row}">
              {{ row.end_date ? formatDate(row.end_date) : '持续用药' }}
            </template>
          </el-table-column>
          <el-table-column prop="drug_category" label="药物类别" />
        </el-table>
      </el-tab-pane>

      <!-- 体检数据 -->
      <el-tab-pane label="体检数据" name="exams">
        <el-table :data="completeData.physicalExams" v-loading="loading" empty-text="暂无体检数据">
          <el-table-column prop="exam_date" label="体检日期" width="120">
            <template #default="{row}">
              {{ formatDate(row.exam_date) }}
            </template>
          </el-table-column>
          <el-table-column prop="temperature" label="体温(°C)" />
          <el-table-column prop="pulse" label="脉搏(次/分)" />
          <el-table-column prop="oxygen_saturation" label="血氧饱和度(%)" />
          <el-table-column prop="lung_sounds" label="肺部听诊" />
          <el-table-column prop="rash_description" label="皮疹描述" />
        </el-table>
      </el-tab-pane>

      <!-- 标本信息 -->
      <el-tab-pane label="标本信息" name="specimens">
        <el-table :data="completeData.specimens" v-loading="loading" empty-text="暂无标本信息">
          <el-table-column prop="specimen_id" label="标本ID" />
          <el-table-column prop="collection_date" label="采集日期" width="120">
            <template #default="{row}">
              {{ formatDate(row.collection_date) }}
            </template>
          </el-table-column>
          <el-table-column prop="specimen_type" label="标本类型" />
          <el-table-column prop="collection_site" label="采集地点" />
          <el-table-column prop="volume_ml" label="体积(ml)" />
          <el-table-column prop="storage_condition" label="保存条件" />
          <el-table-column prop="storage_location" label="保存位置" />
          <el-table-column label="操作" width="100">
            <template #default="{row}">
              <el-button size="small" type="danger" @click="deleteSpecimen(row.specimen_id)" v-if="isAdmin">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>

      <!-- 检查报告 -->
      <el-tab-pane label="检查报告" name="labtests">
        <el-table :data="completeData.labTests" v-loading="loading" empty-text="暂无检查报告">
          <el-table-column prop="lab_id" label="检查ID" />
          <el-table-column prop="item_name" label="检查项目" />
          <el-table-column prop="exam_type" label="检查类型" />
          <el-table-column prop="exam_value" label="检查结果" />
        </el-table>
      </el-tab-pane>

      <!-- 基因组数据 -->
      <el-tab-pane label="基因组数据" name="genomic">
        <el-table :data="completeData.genomicData" v-loading="loading" empty-text="暂无基因组数据">
          <el-table-column prop="specimen_id" label="标本ID" />
          <el-table-column prop="il4_genotype" label="IL4基因型" />
          <el-table-column prop="il13_genotype" label="IL13基因型" />
          <el-table-column prop="analysis_date" label="分析日期" width="120">
            <template #default="{row}">
              {{ formatDate(row.analysis_date) }}
            </template>
          </el-table-column>
          <el-table-column prop="data_path" label="数据路径" />
          <el-table-column label="操作" width="100">
            <template #default="{row}">
              <el-button size="small" type="danger" @click="deleteGenomicData(row.data_id)" v-if="isAdmin">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>

      <!-- 蛋白质数据 -->
      <el-tab-pane label="蛋白质数据" name="protein">
        <el-table :data="completeData.proteinData" v-loading="loading" empty-text="暂无蛋白质数据">
          <el-table-column prop="specimen_id" label="标本ID" />
          <el-table-column prop="ige_level" label="IgE水平(IU/mL)" />
          <el-table-column prop="cytokine_profile" label="细胞因子谱" />
          <el-table-column prop="analysis_date" label="分析日期" width="120">
            <template #default="{row}">
              {{ formatDate(row.analysis_date) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100">
            <template #default="{row}">
              <el-button size="small" type="danger" @click="deleteProteinData(row.data_id)" v-if="isAdmin">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>

      <!-- 费用信息 -->
      <el-tab-pane label="费用信息" name="costs">
        <el-table :data="completeData.medicalCosts" v-loading="loading" empty-text="暂无费用信息">
          <el-table-column prop="cost_type" label="费用类型" />
          <el-table-column prop="amount" label="金额(元)" />
          <el-table-column prop="cost_date" label="产生日期" width="120">
            <template #default="{row}">
              {{ formatDate(row.cost_date) }}
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
    </el-tabs>

    <!-- 调研数据详情对话框 -->
    <el-dialog v-model="rawDataDialogVisible" title="调研数据详情" width="50%">
      <pre>{{ currentRawData }}</pre>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'

const route = useRoute()
const router = useRouter()
const activeTab = ref('followups')
const loading = ref(false)
const rawDataDialogVisible = ref(false)
const currentRawData = ref('')
const isAdmin = ref(false)

// 患者基本信息
const patientData = ref({})
// 完整关联数据
const completeData = ref({
  followUps: [],
  questionnaires: [],
  medications: [],
  physicalExams: [],
  specimens: [],
  labTests: [],
  diagnoses: [],
  medicalCosts: [],
  householdEnvironment: [],
  healthBehaviors: [],
  specimenQualities: [],
  genomicData: [],
  proteinData: [],
  clinicalData: [],
  summary: {
    totalFollowUps: 0,
    totalQuestionnaires: 0,
    totalSpecimens: 0,
    totalCosts: 0
  }
})

// 计算年龄
const calculateAge = (birthDate) => {
  if (!birthDate) return 0
  const today = new Date()
  const birth = new Date(birthDate)
  let age = today.getFullYear() - birth.getFullYear()
  const monthDiff = today.getMonth() - birth.getMonth()
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
    age--
  }
  return age
}

// 格式化性别显示
const formatGender = (gender) => {
  if (gender === 'M' || gender === 'm') return '男'
  if (gender === 'F' || gender === 'f') return '女'
  return '未知'
}

// 格式化居住类型
const formatResidenceType = (type) => {
  const typeMap = { '1': '城市', '2': '城镇', '3': '农村' }
  return typeMap[type] || '未知'
}

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 格式化日期时间
const formatDateTime = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleString('zh-CN')
}

// 查看调研数据详情
const viewRawData = (rawData) => {
  try {
    if (!rawData) {
      currentRawData.value = '暂无数据'
    } else if (typeof rawData === 'string') {
      // 尝试解析JSON字符串
      try {
        const parsed = JSON.parse(rawData)
        currentRawData.value = JSON.stringify(parsed, null, 2)
      } catch (jsonError) {
        // 如果不是有效的JSON，直接显示原始数据
        currentRawData.value = rawData
      }
    } else if (typeof rawData === 'object') {
      // 如果已经是对象，直接格式化显示
      currentRawData.value = JSON.stringify(rawData, null, 2)
    } else {
      // 其他类型转换为字符串
      currentRawData.value = String(rawData)
    }
  } catch (error) {
    console.error('处理调研数据时出错:', error)
    currentRawData.value = '数据解析错误，请联系管理员'
  }
  
  // 确保对话框能正常显示
  try {
    rawDataDialogVisible.value = true
  } catch (error) {
    console.error('显示对话框时出错:', error)
    ElMessage.error('无法显示数据详情')
  }
}

// 检查管理员权限
const checkAdminPermission = async () => {
  try {
    const userId = localStorage.getItem('userId')
    if (!userId) {
      isAdmin.value = false
      return
    }
    
    const response = await fetch(`http://localhost:5000/api/Auth/is-admin/${userId}`, {
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      }
    })
    
    if (response.ok) {
      const result = await response.json()
      isAdmin.value = result.isAdmin || false
    } else {
      isAdmin.value = false
    }
  } catch (error) {
    console.error('检查管理员权限失败:', error)
    isAdmin.value = false
  }
}

// 删除随访记录
const deleteFollowUpRecord = async (recordId) => {
  if (!isAdmin.value) {
    ElMessage.error('只有管理员才能删除数据')
    return
  }
  
  try {
    await ElMessageBox.confirm(
      '确定要删除这条随访记录吗？删除后无法恢复。',
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      }
    )
    
    try {
      const response = await fetch(`http://localhost:5000/api/FollowUpRecord/${recordId}`, {
        method: 'DELETE',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token')
        }
      })
      
      if (response.ok) {
        ElMessage.success('删除成功')
        await fetchPatientCompleteInfo()
      } else {
        const errorText = await response.text().catch(() => '未知错误')
        ElMessage.error(`删除失败: ${errorText}`)
      }
    } catch (networkError) {
      console.error('网络请求失败:', networkError)
      ElMessage.error('网络连接失败，请检查网络后重试')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除操作失败:', error)
      ElMessage.error('删除失败，请稍后重试')
    }
  }
}

// 删除调研数据
const deleteQuestionnaireData = async (recordId) => {
  if (!isAdmin.value) {
    ElMessage.error('只有管理员才能删除数据')
    return
  }
  
  try {
    await ElMessageBox.confirm(
      '确定要删除这条调研数据吗？删除后无法恢复。',
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      }
    )
    
    const response = await fetch(`http://localhost:5000/api/QuestionnaireData/${recordId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      }
    })
    
    if (response.ok) {
      ElMessage.success('删除成功')
      await fetchPatientCompleteInfo()
    } else {
      ElMessage.error('删除失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败: ' + error.message)
    }
  }
}

// 删除标本信息
const deleteSpecimen = async (specimenId) => {
  if (!isAdmin.value) {
    ElMessage.error('只有管理员才能删除数据')
    return
  }
  
  try {
    await ElMessageBox.confirm(
      '确定要删除这个标本信息吗？删除后无法恢复。',
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      }
    )
    
    const response = await fetch(`http://localhost:5000/api/Specimens/${specimenId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      }
    })
    
    if (response.ok) {
      ElMessage.success('删除成功')
      await fetchPatientCompleteInfo()
    } else {
      ElMessage.error('删除失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败: ' + error.message)
    }
  }
}

// 删除基因组数据
const deleteGenomicData = async (recordId) => {
  if (!isAdmin.value) {
    ElMessage.error('只有管理员才能删除数据')
    return
  }
  
  try {
    await ElMessageBox.confirm(
      '确定要删除这条基因组数据吗？删除后无法恢复。',
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      }
    )
    
    const response = await fetch(`http://localhost:5000/api/GenomicDatas/${recordId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      }
    })
    
    if (response.ok) {
      ElMessage.success('删除成功')
      await fetchPatientCompleteInfo()
    } else {
      ElMessage.error('删除失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败: ' + error.message)
    }
  }
}

// 删除蛋白质数据
const deleteProteinData = async (recordId) => {
  if (!isAdmin.value) {
    ElMessage.error('只有管理员才能删除数据')
    return
  }
  
  try {
    await ElMessageBox.confirm(
      '确定要删除这条蛋白质数据吗？删除后无法恢复。',
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      }
    )
    
    const response = await fetch(`http://localhost:5000/api/ProteinDatas/${recordId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      }
    })
    
    if (response.ok) {
      ElMessage.success('删除成功')
      await fetchPatientCompleteInfo()
    } else {
      ElMessage.error('删除失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败: ' + error.message)
    }
  }
}

// 获取患者完整信息
const fetchPatientCompleteInfo = async () => {
  loading.value = true
  
  // 使用try-catch包装整个函数以防止未捕获的错误
  try {
    // 从路由参数获取患者ID，支持多种方式
    let patientId = route.params.id || route.query.patientId || route.query.id
    
    console.log('路由参数:', route.params)
    console.log('查询参数:', route.query)
    console.log('获取到的患者ID:', patientId)

    if (!patientId) {
      console.error('无法获取患者ID参数')
      ElMessage.error('缺少患者ID参数，请从患者列表页面进入')
      
      // 设置默认数据以防止页面崩溃
      patientData.value = {}
      completeData.value = {
        followUps: [], questionnaires: [], medications: [], physicalExams: [],
        specimens: [], labTests: [], diagnoses: [], medicalCosts: [],
        householdEnvironment: [], healthBehaviors: [], specimenQualities: [],
        genomicData: [], proteinData: [], clinicalData: [],
        summary: { totalFollowUps: 0, totalQuestionnaires: 0, totalSpecimens: 0, totalCosts: 0 }
      }
      loading.value = false
      return
    }

    // 确保patientId是字符串并去除空格
    patientId = String(patientId).trim()
    
    if (!patientId) {
      console.error('患者ID为空')
      ElMessage.error('患者ID不能为空，请从患者列表页面进入')
      
      // 设置默认数据以防止页面崩溃
      patientData.value = {}
      completeData.value = {
        followUps: [], questionnaires: [], medications: [], physicalExams: [],
        specimens: [], labTests: [], diagnoses: [], medicalCosts: [],
        householdEnvironment: [], healthBehaviors: [], specimenQualities: [],
        genomicData: [], proteinData: [], clinicalData: [],
        summary: { totalFollowUps: 0, totalQuestionnaires: 0, totalSpecimens: 0, totalCosts: 0 }
      }
      loading.value = false
      return
    }

    console.log('正在请求患者详情，ID:', patientId)
    
    const response = await fetch(
      `http://localhost:5000/api/PatientBasicInfo/${patientId}/complete`,
      {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      }
    )

    console.log('API响应状态:', response.status)

    if (!response.ok) {
      let errorMsg = '请求失败'
      if (response.status === 404) {
        errorMsg = '患者不存在或已被删除'
      } else if (response.status === 403) {
        errorMsg = '权限不足，无法访问患者信息'
      } else if (response.status === 500) {
        errorMsg = '服务器内部错误，请稍后重试'
      } else {
        errorMsg = `请求失败: ${response.status}`
      }
      
      ElMessage.error(errorMsg)
      
      // 设置默认数据以防止页面崩溃，而不是跳转到其他页面
      patientData.value = {}
      completeData.value = {
        followUps: [],
        questionnaires: [],
        medications: [],
        physicalExams: [],
        specimens: [],
        labTests: [],
        diagnoses: [],
        medicalCosts: [],
        householdEnvironment: [],
        healthBehaviors: [],
        specimenQualities: [],
        genomicData: [],
        proteinData: [],
        clinicalData: [],
        summary: {
          totalFollowUps: 0,
          totalQuestionnaires: 0,
          totalSpecimens: 0,
          totalCosts: 0
        }
      }
      loading.value = false
      return
    }

    const result = await response.json()
    console.log('患者完整数据:', result)
    
    // 安全地设置数据
    try {
      patientData.value = result.patient || {}
      
      // 安全地计算统计数据
      const medicalCosts = Array.isArray(result.medicalCosts) ? result.medicalCosts : []
      const totalCosts = medicalCosts.reduce((sum, cost) => {
        const amount = parseFloat(cost?.amount) || 0
        return sum + amount
      }, 0)
      
      completeData.value = {
        followUps: Array.isArray(result.followUps) ? result.followUps : [],
        questionnaires: Array.isArray(result.questionnaires) ? result.questionnaires : [],
        medications: Array.isArray(result.medications) ? result.medications : [],
        physicalExams: Array.isArray(result.physicalExams) ? result.physicalExams : [],
        specimens: Array.isArray(result.specimens) ? result.specimens : [],
        labTests: Array.isArray(result.labTests) ? result.labTests : [],
        diagnoses: Array.isArray(result.diagnoses) ? result.diagnoses : [],
        medicalCosts: medicalCosts,
        householdEnvironment: Array.isArray(result.householdEnvironment) ? result.householdEnvironment : [],
        healthBehaviors: Array.isArray(result.healthBehaviors) ? result.healthBehaviors : [],
        specimenQualities: Array.isArray(result.specimenQualities) ? result.specimenQualities : [],
        genomicData: Array.isArray(result.genomicData) ? result.genomicData : [],
        proteinData: Array.isArray(result.proteinData) ? result.proteinData : [],
        clinicalData: Array.isArray(result.clinicalData) ? result.clinicalData : [],
        summary: {
          totalFollowUps: Array.isArray(result.followUps) ? result.followUps.length : 0,
          totalQuestionnaires: Array.isArray(result.questionnaires) ? result.questionnaires.length : 0,
          totalSpecimens: Array.isArray(result.specimens) ? result.specimens.length : 0,
          totalCosts: totalCosts
        }
      }
    } catch (dataError) {
      console.error('数据处理错误:', dataError)
      ElMessage.warning('数据处理时出现问题，部分信息可能无法正常显示')
      
      // 设置默认数据以防止页面崩溃
      patientData.value = result.patient || {}
      completeData.value = {
        followUps: [],
        questionnaires: [],
        medications: [],
        physicalExams: [],
        specimens: [],
        labTests: [],
        diagnoses: [],
        medicalCosts: [],
        householdEnvironment: [],
        healthBehaviors: [],
        specimenQualities: [],
        genomicData: [],
        proteinData: [],
        clinicalData: [],
        summary: {
          totalFollowUps: 0,
          totalQuestionnaires: 0,
          totalSpecimens: 0,
          totalCosts: 0
        }
      }
    }

    console.log('数据设置完成')
  } catch (error) {
    console.error('获取患者信息时发生错误:', error)
    
    // 根据错误类型显示不同的提示信息
    let errorMsg = '获取患者信息失败'
    if (error.message) {
      if (error.message.includes('fetch')) {
        errorMsg = '网络连接失败，请检查网络后重试'
      } else if (error.message.includes('404')) {
        errorMsg = '患者信息不存在'
      } else if (error.message.includes('403')) {
        errorMsg = '权限不足，无法访问患者信息'
      } else {
        errorMsg = `获取患者信息失败: ${error.message}`
      }
    }
    
    ElMessage.error(errorMsg)
    
    // 设置默认数据以防止页面崩溃，而不是跳转到其他页面
    patientData.value = {}
    completeData.value = {
      followUps: [],
      questionnaires: [],
      medications: [],
      physicalExams: [],
      specimens: [],
      labTests: [],
      diagnoses: [],
      medicalCosts: [],
      householdEnvironment: [],
      healthBehaviors: [],
      specimenQualities: [],
      genomicData: [],
      proteinData: [],
      clinicalData: [],
      summary: {
        totalFollowUps: 0,
        totalQuestionnaires: 0,
        totalSpecimens: 0,
        totalCosts: 0
      }
    }
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  checkAdminPermission()
  fetchPatientCompleteInfo()
})
</script>

<style scoped>
.patient-detail-container {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.patient-header {
  margin-bottom: 20px;
}

.header-title {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.summary-cards {
  margin-bottom: 20px;
}

.stat-card {
  text-align: center;
  padding: 20px;
}

.stat-number {
  font-size: 32px;
  font-weight: bold;
  color: #409EFF;
  margin-bottom: 8px;
}

.stat-label {
  font-size: 14px;
  color: #666;
}

.detail-tabs {
  margin-top: 20px;
}

:deep(.el-descriptions__body) {
  background-color: #f9f9f9;
}

:deep(.el-tab-pane) {
  padding-top: 20px;
}

pre {
  background-color: #f5f5f5;
  padding: 15px;
  border-radius: 4px;
  white-space: pre-wrap;
  word-wrap: break-word;
}
</style>