<template>
  <div class="patient-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>病患信息管理</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            新增病患
          </el-button>
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
        :column-resize="false" 
      >
        <el-table-column prop="index" label="序号" width="80" align="center" />
        <el-table-column prop="name" label="姓名" width="120" align="center" />
        <el-table-column prop="caseNumber" label="病例号" width="180" align="center" />
        <el-table-column prop="age" label="年龄" width="80" align="center" />
        <el-table-column prop="gender" label="性别" width="80" align="center">
          <template #default="scope">
            {{ formatGender(scope.row.gender) }}
          </template>
        </el-table-column>
        <el-table-column prop="phone" label="联系电话" width="150" align="center" />
        <el-table-column prop="address" label="地址" min-width="200" align="center" />
        <el-table-column label="操作" width="300" align="center" fixed="right">
          <template #default="scope">
            <el-button 
              size="small" 
              type="info"
              @click="handleViewDetail(scope.row)"
            >
              <el-icon><View /></el-icon>
              查看详情
            </el-button>
            <el-button 
              size="small" 
              type="primary"
              @click="handleEdit(scope.row)"
            >
              <el-icon><Edit /></el-icon>
              修改
            </el-button>
            <el-button
              size="small"
              type="danger"
              @click="handleDelete(scope.row)"
            >
              <el-icon><Delete /></el-icon>
              删除
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

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      :close-on-click-modal="false"
      @close="resetForm"
    >
      <el-form
        ref="patientFormRef"
        :model="formData"
        :rules="rules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="姓名" prop="name">
              <el-input v-model="formData.name" placeholder="请输入姓名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="病例号" prop="medical_record_no">
              <el-input v-model="formData.medical_record_no" placeholder="病例号按库内自增" disabled/>
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="性别" prop="gender">
              <el-radio-group v-model="formData.gender">
                <el-radio label="男">男</el-radio>
                <el-radio label="女">女</el-radio>
                <el-radio label="其他">其他</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="出生日期" prop="birth_date">
              <el-date-picker
                v-model="formData.birth_date"
                type="date"
                placeholder="选择出生日期"
                style="width: 100%"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                @change="onBirthDateChange"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="年龄" prop="age">
              <el-input-number
                v-model="formData.age"
                :min="0"
                :max="150"
                placeholder="请输入年龄"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <!-- 空白列，保持布局平衡 -->
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="居住类型" prop="address">
              <el-select v-model="formData.address" placeholder="请选择居住类型" style="width: 100%">
                <el-option label="城市" value="1" />
                <el-option label="城镇" value="2" />
                <el-option label="农村" value="3" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话" prop="phone">
              <el-input v-model="formData.phone" placeholder="请输入联系电话" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="过敏史" prop="allergy_history">
              <el-input 
                v-model="formData.allergy_history" 
                type="textarea" 
                placeholder="请输入过敏史，如无过敏史请填写'无'"
                :rows="3"
              />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitForm" :loading="submitLoading">
            {{ dialogTitle === '新增病患' ? '新增' : '修改' }}
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { View, Edit, Delete, Plus, Search, Document, Refresh } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'
import { patientBasicInfoApi } from '@/api'

const router = useRouter()

// 状态管理
const loading = ref(false)
const submitLoading = ref(false)
const patientList = ref([])
const currentPage = ref(1)
const pageSize = ref(10)
const total = ref(0)
const dialogVisible = ref(false)
const dialogTitle = ref('新增病患')
const patientFormRef = ref(null)

// 搜索表单
const searchForm = reactive({
  name: '',
  medicalRecordNo: ''
})

// 表单数据
const formData = ref({
  id: '',
  name: '',
  medical_record_no: '',
  gender: '男',
  birth_date: '',
  age: null,
  phone: '',
  address: '',
  allergy_history: '' // 添加过敏史字段
})

// 表单验证规则
const rules = {
  name: [{ required: true, message: '请输入姓名', trigger: 'blur' }],
  gender: [{ required: true, message: '请选择性别', trigger: 'change' }],
  birth_date: [{ required: true, message: '请选择出生日期', trigger: 'change' }],
  address: [{ required: true, message: '请选择居住类型', trigger: 'change' }],
  allergy_history: [
    // 过敏史改为非必填项
    { min: 0, message: '过敏史信息过长', trigger: 'blur' }
  ],
  phone: [
    // 电话号码为可选字段，如果填写则验证格式
    { 
      validator: (rule, value, callback) => {
        if (!value || value.trim() === '') {
          // 电话号码为可选字段
          callback();
        } else if (!/^1[3-9]\d{9}$/.test(value)) {
          callback(new Error('请输入正确的手机号码格式'));
        } else {
          callback();
        }
      }, 
      trigger: 'blur' 
    }
  ]
}

// 格式化性别显示
const formatGender = (gender) => {
  return gender || '未知'
}

// 格式化居住类型
const formatResidenceType = (type) => {
  const typeMap = { '1': '城市', '2': '城镇', '3': '农村' }
  return typeMap[type] || '未知'
}

// 根据出生日期计算年龄
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

// 出生日期变化时自动计算年龄
const onBirthDateChange = (date) => {
  if (date) {
    formData.value.age = calculateAge(date)
  } else {
    formData.value.age = null
  }
}

// 获取病患列表
const fetchPatients = async () => {
  loading.value = true
  try {
    const params = {
      page: currentPage.value,
      limit: pageSize.value
    }
    
    // 添加搜索条件
    if (searchForm.name) params.name = searchForm.name
    if (searchForm.medicalRecordNo) params.medicalRecordNo = searchForm.medicalRecordNo

    const result = await patientBasicInfoApi.getAll(params)
    
    // 处理分页API返回的数据格式
    if (result && result.code === 200) {
      total.value = result.total // 使用后端返回的总数
      patientList.value = result.data.map((item, index) => ({
        id: item.patient_id, // 使用patient_id作为主要ID
        patient_id: item.patient_id, // 保留原始patient_id
        name: item.name || '',
        caseNumber: item.patient_id || '', // PatientBasicInfo 没有 medical_record_no，使用 patient_id
        age: calculateAge(item.birth_date),
        gender: item.gender || '未知',
        address: formatResidenceType(item.residence_type) || '',
        residence_type: item.residence_type, // 保留原始居住类型
        phone: item.phone || '', // 获取phone字段
        birth_date: item.birth_date, // 保留原始出生日期
        allergy_history: item.allergy_history || '无', // 添加过敏史字段
        index: index + 1 + (currentPage.value - 1) * pageSize.value
      }))
      
      console.log('myPatient页面 - 患者列表数据:', {
        total: total.value,
        currentPage: currentPage.value,
        pageSize: pageSize.value,
        dataLength: patientList.value.length
      }); // 调试信息
    } else if (Array.isArray(result)) {
      // 兼容旧格式（非分页API返回的数组）
      console.warn('使用了非分页API，分页功能将失效')
      total.value = result.length // 暂时使用数组长度作为总数
      patientList.value = result.map((item, index) => ({
        id: item.patient_id,
        patient_id: item.patient_id,
        name: item.name || '',
        caseNumber: item.patient_id || '',
        age: calculateAge(item.birth_date),
        gender: item.gender || '未知',
        address: formatResidenceType(item.residence_type) || '',
        residence_type: item.residence_type,
        phone: item.phone || '',
        birth_date: item.birth_date,
        allergy_history: item.allergy_history || '无',
        index: index + 1 + (currentPage.value - 1) * pageSize.value
      }))
    } else {
      ElMessage.error('获取数据失败')
    }
  } catch (error) {
    console.error('API Error:', error)
    ElMessage.error('获取患者列表失败，请重试')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  currentPage.value = 1
  fetchPatients()
}

// 重置搜索
const resetSearch = () => {
  searchForm.name = ''
  searchForm.medicalRecordNo = ''
  currentPage.value = 1
  fetchPatients()
}

// 分页大小改变
const handleSizeChange = (val) => {
  pageSize.value = val
  currentPage.value = 1
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
  resetForm()
  dialogVisible.value = true
}

// 编辑病患
const handleEdit = (row) => {
  dialogTitle.value = '编辑病患'
  
  // 性别转换：M -> 男, F -> 女
  const genderMap = {'M': '男', 'F': '女'}
  
  formData.value = {
    id: row.id,
    name: row.name,
    medical_record_no: row.caseNumber,
    gender: genderMap[row.gender] || row.gender,
    birth_date: row.birth_date || '',
    age: row.age || calculateAge(row.birth_date),
    phone: row.phone,
    address: row.residence_type || row.address, // 优先使用residence_type
    allergy_history: row.allergy_history || '无'
  }
  dialogVisible.value = true
}

// 查看详情
const handleViewDetail = (row) => {
  console.log('myPatient页面 - 查看详情，患者数据:', row); // 调试信息
  
  // 使用患者ID跳转到详情页面
  const patientId = row.id || row.patient_id
  
  console.log('myPatient页面 - 使用的患者ID:', patientId); // 调试信息
  
  if (patientId && patientId.trim()) {
    router.push({
      name: 'PatientDetail',
      params: { id: patientId.trim() }
    })
  } else {
    console.error('myPatient页面 - 患者ID为空:', { id: row.id, patient_id: row.patient_id });
    ElMessage.error('无法获取患者ID，请联系管理员')
  }
}

// 删除病患
const handleDelete = (row) => {
  ElMessageBox.confirm(
    `确定要删除患者 "${row.name}" 的信息吗？此操作无法撤销。`,
    '删除确认',
    {
      confirmButtonText: '确定删除',
      cancelButtonText: '取消',
      type: 'warning'
    }
  ).then(async () => {
    try {
      loading.value = true
      await patientBasicInfoApi.delete(row.id)
      ElMessage.success('删除成功')
      fetchPatients()
    } catch (error) {
      console.error('Delete error:', error)
      ElMessage.error(error.message || '删除失败，请重试')
    } finally {
      loading.value = false
    }
  }).catch(() => {
    ElMessage.info('已取消删除')
  })
}

// 提交表单
const submitForm = async () => {
  if (!patientFormRef.value) return
  
  patientFormRef.value.validate(async (valid) => {
    if (!valid) return
    
    submitLoading.value = true
    try {
      // 转换前端数据格式为后端期望的格式
      const convertToBackendFormat = (frontendData) => {
        // 性别转换：男 -> M, 女 -> F, 其他 -> O
        const genderMap = {'男': 'M', '女': 'F', '其他': 'O'}
        
        const result = {
          name: frontendData.name,
          gender: genderMap[frontendData.gender] || frontendData.gender,
          birth_date: frontendData.birth_date,
          residence_type: frontendData.address, // 居住类型 (1/2/3)
          allergy_history: frontendData.allergy_history || '无',
          age_at_diagnosi: frontendData.age || calculateAge(frontendData.birth_date),
          phone: frontendData.phone || '' // 添加phone字段
        }
        
        // 验证必填字段
        console.log('转换后的数据:', result)
        
        return result
      }
      
      if (dialogTitle.value === '新增病患') {
        // 新增患者
        const backendData = convertToBackendFormat(formData.value)
        console.log('提交新增数据:', backendData)
        await patientBasicInfoApi.create(backendData)
        ElMessage.success('新增成功')
      } else {
        // 编辑患者
        const backendData = convertToBackendFormat(formData.value)
        console.log('提交更新数据，患者ID:', formData.value.id, '数据:', backendData)
        await patientBasicInfoApi.update(formData.value.id, backendData)
        ElMessage.success('修改成功')
      }
      
      dialogVisible.value = false
      fetchPatients()
    } catch (error) {
      console.error('Submit error:', error)
      ElMessage.error(error.message || '操作失败，请重试')
    } finally {
      submitLoading.value = false
    }
  })
}

// 重置表单
const resetForm = () => {
  formData.value = {
    id: '',
    name: '',
    medical_record_no: '',
    gender: '男',
    birth_date: '',
    age: null,
    phone: '',
    address: '1', // 默认选择城市
    allergy_history: '无' // 设置默认过敏史为"无"
  }
  if (patientFormRef.value) {
    patientFormRef.value.clearValidate()
  }
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

.search-bar {
  background-color: #f5f7fa;
  padding: 20px;
  border-radius: 8px;
  margin-bottom: 20px;
}

/* 确保固定列的分割线显示 */
.el-table__fixed-right {
  box-shadow: -2px 0 8px rgba(0, 0, 0, 0.1);
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}
</style>
    