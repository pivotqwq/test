<template>
  <div class="patient-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>病患信息查看</span>
        </div>
      </template>

      <!-- 搜索栏 -->
      <div class="search-bar" style="margin-bottom: 20px;">
        <el-row :gutter="20">
          <el-col :span="6">
            <el-input 
              v-model="searchForm.name" 
              placeholder="请输入患者姓名" 
              clearable
              @keyup.enter="handleSearch"
              @clear="handleClear"
            >
              <template #prefix>
                <el-icon><Search /></el-icon>
              </template>
            </el-input>
          </el-col>
          <el-col :span="6">
            <el-input 
              v-model="searchForm.medicalRecordNo" 
              placeholder="请输入病例号" 
              clearable
              @keyup.enter="handleSearch"
              @clear="handleClear"
            >
              <template #prefix>
                <el-icon><Document /></el-icon>
              </template>
            </el-input>
          </el-col>
          <el-col :span="4">
            <el-button type="primary" @click="handleSearch">
              <el-icon><Search /></el-icon>
              搜索
            </el-button>
          </el-col>
          <el-col :span="4">
            <el-button @click="resetSearch">
              <el-icon><Refresh /></el-icon>
              重置
            </el-button>
          </el-col>
        </el-row>
      </div>

      <!-- 病患信息表格 -->
      <el-table
        :data="patientList"
        border
        style="width: 100%"
        :row-style="{ height: '60px' }"
        v-loading="loading"
        :header-cell-style="{ cursor: 'default' }" 
      >
        <el-table-column prop="index" label="序号" width="80" align="center" />
        <el-table-column prop="name" label="姓名" width="120" align="center" />
        <el-table-column prop="caseNumber" label="病例号" width="150" align="center" />
        <el-table-column prop="age" label="年龄" width="80" align="center" />
        <el-table-column prop="gender" label="性别" width="80" align="center">
          <template #default="scope">
            {{ formatGender(scope.row.gender) }}
          </template>
        </el-table-column>
        <el-table-column prop="address" label="地址" min-width="200" show-overflow-tooltip />
        <el-table-column label="操作" fixed="right" width="120" align="center">
          <template #default="{ row }">
            <el-button 
              type="primary" 
              size="small" 
              @click="viewPatientDetail(row)"
            >
              查看详情
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页组件 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 20, 30, 50]"
          :small="false"
          :background="true"
          layout="total, sizes, prev, pager, next, jumper"
          :total="total"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Search, Document, Refresh } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'
import { patientBasicInfoApi } from '@/api'

const router = useRouter()

// 状态管理
const loading = ref(false)
const patientList = ref([])
const currentPage = ref(1)
const pageSize = ref(10)
const total = ref(0)
const searchForm = ref({
  name: '',
  medicalRecordNo: ''
})

// 格式化性别显示
const formatGender = (gender) => {
  // 处理后端返回的M/F格式
  if (gender === 'M' || gender === 'm') return '男';
  if (gender === 'F' || gender === 'f') return '女';
  // 处理数字格式（向后兼容）
  const genderMap = { 1: '男', 2: '女', 0: '其他' }
  return genderMap[gender] || '未知'
}

// 格式化居住类型
const formatResidenceType = (type) => {
  const typeMap = { '1': '城市', '2': '城镇', '3': '农村' }
  return typeMap[type] || '未知'
}

// 获取病患列表
const fetchPatients = async () => {
  loading.value = true;
  try {
    const params = {
      page: currentPage.value,
      limit: pageSize.value
    };
    
    // 添加搜索条件
    if (searchForm.value.name && searchForm.value.name.trim()) {
      params.name = searchForm.value.name.trim();
    }
    if (searchForm.value.medicalRecordNo && searchForm.value.medicalRecordNo.trim()) {
      params.medicalRecordNo = searchForm.value.medicalRecordNo.trim();
    }
    
    const result = await patientBasicInfoApi.getAll(params);
    
    // 处理分页API返回的数据格式
    if (result && result.code === 200) {
      total.value = result.total; // 使用后端返回的总数
      patientList.value = result.data.map((item, index) => ({
        id: item.patient_id, // 使用patient_id作为主要ID
        patient_id: item.patient_id, // 保留原始patient_id
        name: item.name || '',
        caseNumber: item.patient_id || '', // PatientBasicInfo 没有 medical_record_no，使用 patient_id
        age: calculateAge(item.birth_date), // 根据出生日期计算年龄
        gender: item.gender, // 直接使用后端返回的性别值，在formatGender中处理显示
        address: formatResidenceType(item.residence_type) || '',
        index: index + 1 + (currentPage.value - 1) * pageSize.value // 计算正确的序号
      }));
      
      console.log('患者列表数据:', {
        total: total.value,
        currentPage: currentPage.value,
        pageSize: pageSize.value,
        dataLength: patientList.value.length
      }); // 调试信息
    } else if (Array.isArray(result)) {
      // 兼容旧格式（非分页API返回的数组）
      console.warn('使用了非分页API，分页功能将失效');
      total.value = result.length; // 暂时使用数组长度作为总数
      patientList.value = result.map((item, index) => ({
        id: item.patient_id,
        patient_id: item.patient_id,
        name: item.name || '',
        caseNumber: item.patient_id || '',
        age: calculateAge(item.birth_date),
        gender: item.gender,
        address: formatResidenceType(item.residence_type) || '',
        index: index + 1 + (currentPage.value - 1) * pageSize.value
      }));
    } else {
      ElMessage.error('获取数据失败');
    }
  } catch (error) {
    console.error('获取患者列表失败:', error);
    ElMessage.error('获取患者列表失败，请重试');
  } finally {
    loading.value = false;
  }
};

// 根据出生日期计算年龄的辅助函数
const calculateAge = (birthDate) => {
  const today = new Date();
  const birth = new Date(birthDate);
  let age = today.getFullYear() - birth.getFullYear();
  const monthDiff = today.getMonth() - birth.getMonth();
  
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
    age--;
  }
  
  return age;
};

// 分页大小改变
const handleSizeChange = (val) => {
  pageSize.value = val
  fetchPatients()
}

// 当前页改变
const handleCurrentChange = (val) => {
  currentPage.value = val
  fetchPatients()
}

// 查看患者详情
const viewPatientDetail = (patient) => {
  console.log('跳转患者详情，患者数据:', patient); // 调试信息
  
  // 使用患者ID跳转到详情页面，优先使用patient_id
  const patientId = patient.patient_id || patient.id
  
  console.log('使用的患者ID:', patientId); // 调试信息
  
  if (patientId && patientId.trim()) {
    router.push({
      name: 'PatientDetail',
      params: { id: patientId.trim() }
    })
  } else {
    console.error('患者ID为空:', { patient_id: patient.patient_id, id: patient.id });
    ElMessage.error('无法获取患者ID，请联系管理员')
  }
}

// 患者信息不允许编辑，已移除编辑功能

// 搜索
const handleSearch = () => {
  currentPage.value = 1; // 重置到第一页
  fetchPatients();
}

// 清空搜索条件
const handleClear = () => {
  fetchPatients();
}

// 重置搜索
const resetSearch = () => {
  searchForm.value = {
    name: '',
    medicalRecordNo: ''
  };
  currentPage.value = 1; // 重置到第一页
  fetchPatients();
}

// 初始化加载数据
onMounted(() => {
  fetchPatients()
})
</script>

<style scoped>
.patient-container {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-bar {
  padding: 16px;
  background-color: #f8f9fa;
  border-radius: 8px;
  margin-bottom: 20px;
}

.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}

/* 确保固定列的分割线显示 */
.el-table__fixed-right {
  box-shadow: -2px 0 8px rgba(0, 0, 0, 0.1);
}
</style>