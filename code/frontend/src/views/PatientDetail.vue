<template>
  <div class="patient-detail-container">
    <!-- 头部信息卡片 -->
    <el-card class="patient-header">
      <template #header>
        <div class="header-title">
          <el-page-header @back="router.go(-1)">
            <template #content>
              <h2>患者详细信息</h2>
            </template>
          </el-page-header>
          <el-tag :type="patientData.status === '住院' ? 'danger' : 'success'">
            {{ patientData.status }}
          </el-tag>
        </div>
      </template>

      <!-- 基本信息 -->
      <el-descriptions :column="2" border>
        <el-descriptions-item label="病历号">{{ patientData.medical_record_no }}</el-descriptions-item>
        <el-descriptions-item label="姓名">{{ patientData.name }}</el-descriptions-item>
        <el-descriptions-item label="性别">{{ patientData.gender }}</el-descriptions-item>
        <el-descriptions-item label="年龄">{{ calculateAge(patientData.birth_date) }}岁</el-descriptions-item>
        <el-descriptions-item label="出生日期">{{ patientData.birth_date }}</el-descriptions-item>
        <el-descriptions-item label="联系电话">{{ patientData.phone }}</el-descriptions-item>
        <el-descriptions-item label="住址" :span="2">{{ patientData.address }}</el-descriptions-item>
      </el-descriptions>
    </el-card>

    <!-- 详细信息标签页 -->
    <el-tabs v-model="activeTab" class="detail-tabs">
      <el-tab-pane label="诊疗记录" name="treatment">
        <el-table :data="patientData.treatment_records" v-loading="loading">
          <el-table-column prop="date" label="日期" width="120" />
          <el-table-column prop="department" label="科室" width="120" />
          <el-table-column prop="doctor" label="医生" width="120" />
          <el-table-column prop="diagnosis" label="诊断" />
        </el-table>
      </el-tab-pane>
      
      <el-tab-pane label="检查报告" name="reports">
        <el-table :data="patientData.medical_reports" v-loading="loading">
          <el-table-column prop="date" label="日期" width="120" />
          <el-table-column prop="type" label="检查类型" width="150" />
          <el-table-column prop="result" label="结果摘要">
            <template #default="{row}">
              {{ row.result.substring(0, 30) }}...
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage} from 'element-plus'

const route = useRoute()
const router = useRouter()
const activeTab = ref('treatment')
const loading = ref(false)
const patientData = ref({
  medical_record_no: '',
  name: '',
  gender: '',
  birth_date: '',
  phone: '',
  address: '',
  status: '',
  treatment_records: [],
  medical_reports: []
})

// 计算年龄
const calculateAge = (birthDate) => {
  const today = new Date()
  const birth = new Date(birthDate)
  let age = today.getFullYear() - birth.getFullYear()
  const monthDiff = today.getMonth() - birth.getMonth()
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
    age--
  }
  return age
}

// 使用fetch获取患者详情
const fetchPatientDetail = async () => {
  loading.value = true
  try {
    if (!route.params.medicalRecordNo) {
      console.error(route.params);
      return;
    }

    const response = await fetch(
      `http://localhost:5000/api/patientInfo/detail?medicalRecordNo=${encodeURIComponent(route.params.medicalRecordNo)}`,
      {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      }
    )

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    const result = await response.json()
    
    if (result.code === 200) {
      patientData.value = result.data
    } else {
      throw new Error(result.message || '获取患者信息失败')
    }
  } catch (error) {
    console.error('Error:', error)
    ElMessage.error(error.message)
    router.push('/myPatient')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchPatientDetail()
})
</script>

<style scoped>
/* 保持之前的样式不变 */
.patient-detail-container {
  padding: 20px;
  max-width: 1200px;
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

.detail-tabs {
  margin-top: 20px;
}

:deep(.el-descriptions__body) {
  background-color: #f9f9f9;
}
</style>