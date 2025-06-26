<template>
  <div class="patient-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>病患信息管理</span>
          <el-button type="primary" @click="handleAdd">新增病患</el-button>
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
        {{ console.log('当前行数据:', scope.row) }}
        <el-icon><View /></el-icon>
            <span>查看详情</span>
        </el-button>
            <el-button size="small" @click="handleEdit(scope.$index, scope.row)"
              >修改</el-button
            >
            <el-button
              size="small"
              type="danger"
              @click="handleDelete(scope.$index)"
              >删除</el-button
            >
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

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="500px"
      @close="resetForm"
    >
      <el-form
        ref="patientForm"
        :model="formData"
        :rules="rules"
        label-width="80px"
      >
        <el-form-item label="姓名" prop="name">
          <el-input v-model="formData.name" placeholder="请输入姓名" />
        </el-form-item>
        <el-form-item label="病例号" prop="caseNumber">
          <el-input v-model="formData.caseNumber" placeholder="请输入病例号" />
        </el-form-item>
        <el-form-item label="年龄" prop="age">
          <el-input-number
            v-model="formData.age"
            :min="0"
            :max="120"
            controls-position="right"
          />
        </el-form-item>
        <el-form-item label="性别" prop="gender">
          <el-radio-group v-model="formData.gender">
            <el-radio :label="1">男</el-radio>
            <el-radio :label="2">女</el-radio>
            <el-radio :label="0">其他</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitForm">确认</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { View } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'

const router = useRouter()
// 模拟数据
const mockPatients = [
  { id: 1, name: '张三', caseNumber: 'CASE20230001', age: 35, gender: 1 },
  { id: 2, name: '李四', caseNumber: 'CASE20230002', age: 28, gender: 2 },
  { id: 3, name: '王五', caseNumber: 'CASE20230003', age: 42, gender: 1 },
  { id: 4, name: '赵六', caseNumber: 'CASE20230004', age: 65, gender: 2 },
  { id: 5, name: '钱七', caseNumber: 'CASE20230005', age: 19, gender: 0 }
]

// 状态管理
const loading = ref(false)
const patientList = ref([])
const currentPage = ref(1)
const pageSize = ref(10)
const total = ref(0)
const dialogVisible = ref(false)
const dialogTitle = ref('新增病患')
const formData = ref({
  id: null,
  name: '',
  caseNumber: '',
  age: null,
  gender: 1
})
const currentIndex = ref(-1) // 当前编辑的索引

// 表单验证规则
const rules = {
  name: [{ required: true, message: '请输入姓名', trigger: 'blur' }],
  caseNumber: [{ required: true, message: '请输入病例号', trigger: 'blur' }],
  age: [{ required: true, message: '请输入年龄', trigger: 'blur' }],
  gender: [{ required: true, message: '请选择性别', trigger: 'change' }]
}

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

// 新增病患
const handleAdd = () => {
  dialogTitle.value = '新增病患'
  dialogVisible.value = true
}

// 编辑病患
const handleEdit = (index, row) => {
  dialogTitle.value = '编辑病患'
  currentIndex.value = index
  formData.value = { ...row }
  dialogVisible.value = true
}

const handleViewDetail = (medicalRecordNo) => {
  //console.error(medicalRecordNo+' is medicalRecordNo');
  router.push({
    name: 'PatientDetail',
    params: { medicalRecordNo }
  })
}

// 删除病患
const handleDelete = (index) => {
  ElMessageBox.confirm('确定要删除该病患信息吗?', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      // 模拟API调用
      await new Promise(resolve => setTimeout(resolve, 300))
      mockPatients.splice(index, 1)
      ElMessage.success('删除成功')
      fetchPatients()
    } catch (error) {
      ElMessage.error('删除失败: ' + error.message)
    }
  }).catch(() => {
    ElMessage.info('已取消删除')
  })
}

// 提交表单
const submitForm = async () => {
  try {
    // 模拟表单验证
    // 实际项目中这里应该有更严格的验证
    
    if (currentIndex.value === -1) {
      // 新增
      const newPatient = {
        ...formData.value,
        id: mockPatients.length + 1
      }
      mockPatients.push(newPatient)
      ElMessage.success('新增成功')
    } else {
      // 编辑
      mockPatients[currentIndex.value] = formData.value
      ElMessage.success('修改成功')
    }
    
    dialogVisible.value = false
    fetchPatients()
  } catch (error) {
    ElMessage.error('操作失败: ' + error.message)
  }
}

// 重置表单
const resetForm = () => {
  formData.value = {
    id: null,
    name: '',
    caseNumber: '',
    age: null,
    gender: 1
  }
  currentIndex.value = -1
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
    