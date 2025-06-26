<template>
  <div class="patient-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>病患信息查看</span>
        </div>
      </template>

      <!-- 病患信息表格 -->
      <el-table
        :data="patientList"
        border
        style="width: 100%"
        :row-style="{ height: '60px' }"
        v-loading="loading"
        :header-cell-style="{ cursor: 'default' }" 
        :column-resize="false" 
      >
        <el-table-column prop="index" label="序号" width="185" align="center" />
        <el-table-column prop="name" label="姓名" width="235" align="center" />
        <el-table-column prop="caseNumber" label="病例号" width="280" align="center" />
        <el-table-column prop="age" label="年龄" width="185" align="center" />
        <el-table-column prop="gender" label="性别" width="185" align="center">
          <template #default="scope">
            {{ formatGender(scope.row.gender) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="285" align="center" fixed="right">
          <template #default="scope">
            <el-button 
              size="small" 
              type="info"
              @click="handleViewDetail(scope.row.caseNumber)"
            >
              <el-icon><View /></el-icon>
              <span>查看详情</span>
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
import { View } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// 状态管理
const loading = ref(false)
const patientList = ref([])
const currentPage = ref(1)
const pageSize = ref(10)
const total = ref(0)

// 格式化性别显示
const formatGender = (gender) => {
  const genderMap = { 1: '男', 2: '女', 0: '其他' }
  return genderMap[gender] || '未知'
}

// 获取病患列表
const fetchPatients = async () => {
  loading.value = true;
  try {
    const response = await fetch('http://localhost:5000/api/patientInfo/allPatients', {
      method: 'GET',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    
    if (result.code === 200) {
      total.value = result.tot;
      patientList.value = result.data.map((item, index) => ({
        id: item.id.trim(), // 清理ID中的空格
        name: item.name,
        caseNumber: item.medical_record_no.trim(), // 使用medical_record_no并清理空格
        age: calculateAge(item.birth_date), // 根据出生日期计算年龄
        gender: item.gender === '男' ? 1 : item.gender === '女' ? 2 : 0, // 转换为数字类型
        address: item.address,
        index: index + 1 + (currentPage.value - 1) * pageSize.value // 计算正确的序号
      }));
    } else {
      throw new Error(result.message || '获取数据失败');
    }
  } catch (error) {
    ElMessage.error('获取病患列表失败: ' + error.message);
    console.error('API Error:', error);
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

const handleViewDetail = (medicalRecordNo) => {
  router.push({
    name: 'PatientDetail',
    params: { medicalRecordNo }
  })
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