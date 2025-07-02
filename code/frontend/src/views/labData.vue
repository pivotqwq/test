<template>
  <div class="lab-container">
    <el-card class="lab-card">
      <template #header>
        <div class="card-header">
          <span>实验室数据</span>
          <div>
            <el-button type="primary" @click="showAddDialog">
              <el-icon><Plus /></el-icon> 新增实验记录
            </el-button>
            <el-button type="success" @click="exportData">
              <el-icon><Download /></el-icon> 导出数据
            </el-button>
          </div>
        </div>
      </template>

      <!-- 搜索过滤区域 -->
      <div class="filter-bar">
        <el-form :inline="true" :model="filterForm">
          <el-form-item label="患者ID">
            <el-input v-model="filterForm.patientId" placeholder="输入患者ID" clearable style="width: 200px;" />
          </el-form-item>
          <el-form-item label="检验项目">
            <el-select v-model="filterForm.testType" placeholder="选择检验项目" clearable style="width: 200px;">
              <el-option 
                v-for="item in testTypes" 
                :key="item.value" 
                :label="item.label" 
                :value="item.value" 
              />
            </el-select>
          </el-form-item>
          <el-form-item label="日期范围">
            <el-date-picker
              v-model="filterForm.dateRange"
              type="daterange"
              range-separator="至"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
              value-format="YYYY-MM-DD"
              style="width: 240px;"
            />
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="searchData">查询</el-button>
          </el-form-item>
        </el-form>
        
        <!-- 权限提示 -->
        <div v-if="!hasPermission('admin')" class="permission-notice">
          <el-alert
            title="权限提示"
            type="info"
            description="只有管理员用户才能删除实验记录，如需删除权限请联系系统管理员"
            :closable="false"
            show-icon
          />
        </div>
      </div>

      <!-- 数据表格 -->
      <el-table 
        :data="labData" 
        style="width: 100%" 
        v-loading="loading"
        stripe
        border
        highlight-current-row
      >
        <el-table-column prop="id" label="记录ID" align="center" :resizable="false" />
        <el-table-column prop="patientId" label="患者ID" align="center" :resizable="false" />
        <el-table-column prop="patientName" label="患者姓名" :resizable="false" align="center" />
        <el-table-column prop="testType" label="检验项目" :resizable="false" align="center">
          <template #default="scope">
            <el-tag :type="getTestTypeTag(scope.row.testType)">
              {{ getTestTypeLabel(scope.row.testType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="testResult" label="检验结果" :resizable="false" align="center">
          <template #default="scope">
            <el-tag :type="getStatusTag(scope.row.testResult, scope.row.referenceRange)">
              {{ scope.row.testResult }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="referenceRange" label="参考范围" :resizable="false" align="center">
          <template #default="scope">
            {{ scope.row.referenceRange }} {{ scope.row.unit }}
          </template>
        </el-table-column>
        <el-table-column prop="testDate" label="检验日期" align="center" :resizable="false" />
        <el-table-column prop="doctorId" label="医师ID" :resizable="false" align="center" />
        <el-table-column prop="doctor" label="检验医师" :resizable="false" align="center" />
        <el-table-column label="操作" width="100" fixed="right" align="center" :resizable="false">
          <template #default="scope">
            <el-button 
              size="small" 
              type="danger" 
              @click="deleteRecord(scope.row.id)"
              v-if="hasPermission('admin')"
            >
              <el-icon><Delete /></el-icon> 删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination">
        <el-pagination
          v-model:current-page="pagination.current"
          v-model:page-size="pagination.size"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          :total="pagination.total"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog 
      v-model="dialogVisible" 
      :title="dialogTitle" 
      width="60%"
      :close-on-click-modal="false"
    >
      <el-form 
        :model="formData" 
        :rules="rules" 
        ref="labForm"
        label-width="120px"
        label-position="top"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="患者ID" prop="patientId">
              <el-input v-model="formData.patientId" placeholder="输入患者ID" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="患者姓名" prop="patientName">
              <el-input v-model="formData.patientName" placeholder="输入患者姓名" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="检验项目" prop="testType">
              <el-select 
                v-model="formData.testType" 
                placeholder="选择检验项目" 
                style="width: 100%"
              >
                <el-option 
                  v-for="item in testTypes" 
                  :key="item.value" 
                  :label="item.label" 
                  :value="item.value" 
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="检验日期" prop="testDate">
              <el-date-picker
                v-model="formData.testDate"
                type="date"
                placeholder="选择检验日期"
                style="width: 100%"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="检验结果" prop="testResult">
              <el-input 
                v-model="formData.testResult" 
                placeholder="输入检验结果" 
                type="number"
              >
                <template #append>
                  <span>{{ getUnit(formData.testType) }}</span>
                </template>
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="参考范围" prop="referenceRange">
              <el-input 
                v-model="formData.referenceRange" 
                placeholder="如: 3.5-5.5" 
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="医师ID" prop="doctorId">
              <el-input v-model="formData.doctorId" placeholder="输入检验医师ID" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="检验医师" prop="doctor">
              <el-input v-model="formData.doctor" placeholder="输入检验医师姓名" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="备注" prop="remark">
          <el-input 
            v-model="formData.remark" 
            type="textarea" 
            :rows="3" 
            placeholder="可输入异常情况说明等" 
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitForm">确认</el-button>
      </template>
    </el-dialog>


  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { 
  ElMessage, 
  ElMessageBox,
  ElLoading
} from 'element-plus'
import { 
  Plus, 
  Download, 
  Delete
} from '@element-plus/icons-vue'

// 状态管理
const loading = ref(false)
const labData = ref([])
const dialogVisible = ref(false)
const dialogTitle = ref('新增实验记录')
const userRole = ref('user') // 角色从API动态获取
const labForm = ref(null) // 表单引用
const formData = ref({
  id: null,
  patientId: '',
  patientName: '',
  testType: '',
  testResult: '',
  referenceRange: '',
  testDate: '',
  doctorId: '',
  doctor: '',
  remark: ''
})

// 分页
const pagination = reactive({
  current: 1,
  size: 10,
  total: 0
})

// 筛选表单
const filterForm = reactive({
  patientId: '',
  testType: '',
  dateRange: []
})

// 检验项目类型
const testTypes = ref([
  { value: 'blood_test', label: '血液检查' },
  { value: 'imaging_test', label: '影像学检查' },
  { value: 'pulmonary_function_test', label: '肺功能检查' }
])

// 表单验证规则
const rules = {
  patientId: [{ required: true, message: '请输入患者ID', trigger: 'blur' }],
  patientName: [{ required: true, message: '请输入患者姓名', trigger: 'blur' }],
  testType: [{ required: true, message: '请选择检验项目', trigger: 'change' }],
  testResult: [{ required: true, message: '请输入检验结果', trigger: 'blur' }],
  referenceRange: [{ required: true, message: '请输入参考范围', trigger: 'blur' }],
  testDate: [{ required: true, message: '请选择检验日期', trigger: 'change' }],
  doctorId: [{ required: true, message: '请输入医师ID', trigger: 'blur' }],
  doctor: [{ required: true, message: '请输入检验医师', trigger: 'blur' }]
}

// 获取检验项目中文名
const getTestTypeLabel = (type) => {
  const found = testTypes.value.find(item => item.value === type);
  return found ? found.label : type;
};

// 获取检验项目单位
const getUnit = (type) => {
  const units = {
    'blood_routine': '×10⁹/L',
    'biochemistry': 'mmol/L',
    'liver_function': 'U/L',
    'kidney_function': 'μmol/L',
    'blood_sugar': 'mmol/L',
    'lipid': 'mmol/L',
    'tumor_markers': 'ng/mL',
    'hepatitis_b': '-',
    'thyroid_function': 'pmol/L'
  }
  return units[type] || ''
}

// 获取检验项目标签样式
const getTestTypeTag = (type) => {
  const tags = {
    'blood_test': 'danger',
    'imaging_test': 'success',
    'pulmonary_function_test': '' // primary
  }
  return tags[type] || 'info'
}

// 获取检验状态
const getStatusTag = (result, range, isQualitative = false) => {
  if (isQualitative) {
    return result === '阴性' || result === '正常' ? 'success' : 'danger';
  }
  if (!result || !range) return 'info'
  
  try {
    const num = parseFloat(result)
    const [min, max] = range.split('-').map(Number)
    
    if (isNaN(num) || isNaN(min) || isNaN(max)) return 'info'
    
    if (num < min) return 'warning'
    if (num > max) return 'danger'
    return 'success'
  } catch (e) {
    return 'info'
  }
}

// 检查权限
const hasPermission = (role) => {
  return userRole.value === role
}

// 检查用户权限
const checkUserPermission = async () => {
  try {
    const userId = localStorage.getItem('userId')
    if (!userId) {
      console.log('用户未登录')
      return
    }

    const response = await fetch(`http://localhost:5000/api/Auth/is-admin/${userId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    if (response.ok) {
      const result = await response.json()
      userRole.value = result.isAdmin ? 'admin' : 'user'
      console.log('用户权限检查结果:', { userId, isAdmin: result.isAdmin, userRole: userRole.value })
    } else {
      console.error('权限检查失败:', response.status)
      userRole.value = 'user'
    }
  } catch (error) {
    console.error('权限检查错误:', error)
    userRole.value = 'user'
  }
}

// 患者ID到姓名的映射函数已废弃，现在直接使用后端返回的 patient_name 字段

// 获取实验数据
const fetchLabData = async () => {
  loading.value = true
  try {
    // 构建查询参数
    const params = {
      page: pagination.current,
      limit: pagination.size
    };

    // 添加筛选条件（只有当值存在时才添加）
    if (filterForm.patientId && filterForm.patientId.trim()) {
      params.patientId = filterForm.patientId.trim();
    }

    const { get } = await import('@/utils/request');
    console.log('Fetching lab data with params:', params); // 调试信息
    const result = await get('/ClinicalData/lab-testsInfo', params);
    
    console.log('API response:', result); // 调试信息
    
    if (result.success) {
      // 检查是否有数据
      const dataArray = Array.isArray(result.data) ? result.data : [];
      
      if (dataArray.length === 0) {
        ElMessage.info('暂无实验室数据');
        labData.value = [];
        pagination.total = 0;
        return;
      }
      
      // 转换数据格式以匹配前端显示需求
      const transformedData = dataArray.map(item => ({
        id: item.lab_id || Math.random().toString(36).substr(2, 9),
        patientId: item.patient_id || 'N/A',
        patientName: item.patient_name || `患者${item.patient_id}` || 'N/A', // 使用后端返回的真实患者姓名
        testType: getTestTypeFromExamType(item.exam_type || 'blood'),
        testResult: item.exam_value || 'N/A',
        referenceRange: getReferenceRangeByExamType(item.exam_type), // 根据检查类型提供参考范围
        unit: getUnitByExamType(item.exam_type),
        testDate: new Date().toISOString().split('T')[0], // 使用当前日期作为默认值
        doctor: '检验医师', // 后端没有医生字段，使用默认值
        doctorId: 'D001',
        labName: '中心实验室',
        details: null,
        diseaseDiagnosis: null,
        clinicalInfo: null
      }));
      
      // 应用前端筛选（除了patientId，其他的前端筛选）
      let filteredData = [...transformedData];
      if (filterForm.testType) {
        filteredData = filteredData.filter(item => 
          item.testType === filterForm.testType
        );
      }
      if (filterForm.dateRange && filterForm.dateRange.length === 2) {
        const [startDate, endDate] = filterForm.dateRange;
        filteredData = filteredData.filter(item => {
          const itemDate = new Date(item.testDate);
          return itemDate >= new Date(startDate) && itemDate <= new Date(endDate);
        });
      }
      
      labData.value = filteredData;
      pagination.total = result.total || filteredData.length;
      
      if (filteredData.length === 0) {
        ElMessage.info('根据筛选条件未找到匹配的数据');
      }
    } else {
      throw new Error(result.message || '获取数据失败');
    }
  } catch (error) {
    console.error('API Error:', error);
    ElMessage.error('获取实验数据失败: ' + (error.message || '未知错误'));
    
    // 如果API失败，使用模拟数据
    const mockData = [
      {
        id: 'L001',
        patientId: 'P10001',
        patientName: '张三',
        testType: 'blood_test',
        testResult: '12.5',
        referenceRange: '11.5-15.0',
        unit: 'g/dL',
        testDate: '2023-05-10',
        doctor: '王医生',
        doctorId: 'D001',
        labName: '中心实验室A栋',
        details: {
          sampleInfo: { sample_id: 'S001-B', collection_time: '2023-05-10 08:30', sample_type: '全血' },
          bloodRoutine: [
            { name: '白细胞计数(WBC)', value: '6.8', unit: 'x10^9/L', range: '4.0-10.0' },
            { name: '红细胞计数(RBC)', value: '4.5', unit: 'x10^12/L', range: '3.5-5.5' },
            { name: '血红蛋白(HGB)', value: '12.5', unit: 'g/dL', range: '11.5-15.0' },
            { name: '血小板计数(PLT)', value: '250', unit: 'x10^9/L', range: '100-300' },
          ]
        },
        diseaseDiagnosis: {
            diagnosisId: 'DIAG-001',
            patientId: 'P10001',
            diseaseName: '过敏性哮喘',
            severity: '中度',
            description: '根据血液检查结果和临床表现，初步诊断为中度持续性过敏性哮喘。'
        },
        clinicalInfo: {
          patient: { patientId: 'P10001', medicalRecordNo: 'MRN-000123', name: '张三', gender: '男', dob: '1988-08-08', address: '北京市朝阳区幸福街道123号' },
          insurance: { insuranceId: 'INS-001', patientId: 'P10001', insuranceType: '城镇职工基本医疗保险' },
          contact: { contactId: 'CON-001', patientId: 'P10001', name: '张太太', contactMethod: '138-0001-0001' },
          pastHistory: { historyId: 'PH-001', patientId: 'P10001', allergyHistory: '青霉素过敏' },
          familyHistory: { familyHistoryId: 'FH-001', patientId: 'P10001', historyDetails: '父亲患有高血压' }
        }
      }
    ];
    
    labData.value = mockData;
    pagination.total = mockData.length;
  } finally {
    loading.value = false;
        }
}

// 根据后端exam_type转换为前端testType
const getTestTypeFromExamType = (examType) => {
  const typeMap = {
    'blood': 'blood_test',
    'imaging': 'imaging_test',
    'pulmonary': 'pulmonary_function_test',
    'allergy': 'blood_test'
  };
  return typeMap[examType] || 'blood_test';
}

// 根据检查类型获取参考范围
const getReferenceRangeByExamType = (examType) => {
  const rangeMap = {
    'blood': '正常范围',
    'imaging': '—',
    'pulmonary': '80-120%',
    'allergy': '阴性'
  };
  return rangeMap[examType] || '正常范围';
}

// 根据检查类型获取单位
const getUnitByExamType = (examType) => {
  const unitMap = {
    'blood': 'x10^9/L',
    'imaging': '—',
    'pulmonary': '%',
    'allergy': '—'
  };
  return unitMap[examType] || '—';
}

// 搜索数据
const searchData = () => {
  pagination.current = 1
  fetchLabData()
}

// 显示新增对话框
const showAddDialog = () => {
  dialogTitle.value = '新增实验记录'
  formData.value = {
    id: null,
    patientId: '',
    patientName: '',
    testType: '',
    testResult: '',
    referenceRange: '',
    testDate: '',
    doctorId: '',
    doctor: '',
    remark: ''
  }
  dialogVisible.value = true
}



// 提交表单
const submitForm = async () => {
  if (!labForm.value) return
  try {
    // 表单验证
    await labForm.value.validate(async (valid) => {
      if (valid) {
        const { post } = await import('@/utils/request');
        
        const result = await post('/ClinicalData/lab-testsAdd', {
          PatientId: formData.value.patientId,
          ItemName: formData.value.testType,
          ExamValue: formData.value.testResult,
          ExamType: getExamTypeFromTestType(formData.value.testType),
          ExamDetails: formData.value.remark || ''
        });
        
        if (result.success) {
          ElMessage.success('保存成功');
          dialogVisible.value = false;
          fetchLabData(); // 重新获取数据
        } else {
          ElMessage.error(result.message || '保存失败');
        }
      } else {
        ElMessage.error('表单验证失败，请检查输入');
        return false;
      }
    });
  } catch (error) {
    console.error('Submit error:', error);
    // 错误信息已经在请求拦截器中处理了
  }
}

// 根据前端testType转换为后端exam_type
const getExamTypeFromTestType = (testType) => {
  const typeMap = {
    'blood_test': 'blood',
    'imaging_test': 'imaging',
    'pulmonary_function_test': 'pulmonary'
  };
  return typeMap[testType] || 'blood';
}

// 删除记录
const deleteRecord = (id) => {
  // 再次检查权限
  if (!hasPermission('admin')) {
    ElMessage.error('您没有删除权限，请联系管理员');
    return;
  }

  ElMessageBox.confirm('确定要删除这条实验记录吗？此操作无法撤销。', '删除确认', {
    confirmButtonText: '确定删除',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      const { del } = await import('@/utils/request');
      
      const result = await del(`/ClinicalData/lab-testsDel?labId=${id}`);
      
      if (result.success) {
        ElMessage.success('删除成功');
        fetchLabData(); // 重新获取数据
      } else {
        ElMessage.error(result.message || '删除失败');
      }
    } catch (error) {
      console.error('Delete error:', error);
      // 错误信息已经在请求拦截器中处理了
    }
  }).catch(() => {
    ElMessage.info('已取消删除')
  })
}

// 导出数据
const exportData = () => {
  const loadingInstance = ElLoading.service({
    lock: true,
    text: '正在导出数据...',
    background: 'rgba(0, 0, 0, 0.7)'
  })
  
  try {
    const columns = [
      { key: 'id', title: '记录ID' },
      { key: 'patientId', title: '患者ID' },
      { key: 'patientName', title: '患者姓名' },
      { key: 'testType', title: '检验项目' },
      { key: 'testResult', title: '检验结果' },
      { key: 'referenceRange', title: '参考范围' },
      { key: 'unit', title: '单位' },
      { key: 'testDate', title: '检验日期' },
      { key: 'doctorId', title: '医师ID' },
      { key: 'doctor', title: '检验医师' },
      { key: 'labName', title: '检验实验室' },
    ];

    const header = columns.map(col => col.title).join(',');
    
    const rows = labData.value.map(row => {
      return columns.map(col => {
        if (col.key === 'testType') {
          return getTestTypeLabel(row[col.key]);
        }
        return `"${row[col.key] || ''}"`;
      }).join(',');
    });

    const csvContent = [header, ...rows].join('\n');
    const blob = new Blob(['\uFEFF' + csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.setAttribute('download', '实验室数据.csv');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    ElMessage.success('导出成功');
  } catch (error) {
    ElMessage.error('导出失败: ' + error.message);
  } finally {
    loadingInstance.close();
  }
}



// 分页大小改变
const handleSizeChange = (size) => {
  pagination.size = size
  fetchLabData()
}

// 页码改变
const handleCurrentChange = (current) => {
  pagination.current = current
  fetchLabData()
}

// 初始化加载数据
onMounted(async () => {
  // 先检查用户权限
  await checkUserPermission()
  
  // 然后加载数据
  fetchLabData()
})
</script>

<style scoped>
.lab-container {
  padding: 20px;
  background-color: #f5f7fa;
  padding-bottom: 80px; /* 增加底部边距 */
}

.lab-card {
  max-width: 1400px;
  margin: 0 auto;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  margin-bottom: 60px; /* 确保卡片底部有足够空间 */
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 0;
}

.filter-bar {
  margin-bottom: 20px;
  padding: 15px;
  background-color: #f9f9f9;
  border-radius: 4px;
}

.filter-bar .el-form {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap; /* 允许表单元素换行 */
  gap: 10px; /* 添加元素间距 */
}

.pagination {
  margin-top: 20px;
  margin-bottom: 40px; /* 增加底部边距 */
  display: flex;
  justify-content: flex-end;
  padding: 20px 0; /* 增加上下内边距 */
}

.report-detail {
  font-family: 'Microsoft YaHei', sans-serif;
}

.report-header {
  text-align: center;
  margin-bottom: 20px;
}

.report-header h2 {
  margin: 0;
  color: #333;
}

.report-no {
  margin-top: 5px;
  color: #666;
}

.patient-info {
  margin-bottom: 20px;
}

.test-info {
  margin: 20px 0;
}

.test-info h3 {
  margin-bottom: 15px;
  color: #333;
}

.report-footer {
  margin-top: 30px;
  display: flex;
  justify-content: space-between;
}

.signature p {
  margin: 5px 0;
  color: #666;
}

.notice p {
  margin: 5px 0;
  color: #999;
  font-size: 12px;
}

.high-value {
  color: #f56c6c;
  font-weight: bold;
}

.low-value {
  color: #67c23a;
  font-weight: bold;
}

:deep(.el-descriptions__body) {
  background-color: #f9f9f9;
}

:deep(.el-descriptions__title) {
  font-weight: bold;
}

.permission-notice {
  margin-top: 10px;
}

.detail-dialog {
  /* Add your styles here */
}

.detail-section {
  padding: 10px;
}
.image-container {
  margin-top: 20px;
  text-align: center;
  background-color: #f9f9f9;
  padding: 20px;
  border-radius: 8px;
}
.detail-subtitle {
  margin-bottom: 15px;
  font-size: 16px;
  color: #333;
  text-align: left;
}
.image-container .el-image {
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.1);
}
.image-container .image-slot {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 200px;
  background: var(--el-fill-color-light);
  color: var(--el-text-color-secondary);
  font-size: 14px;
}
pre {
  white-space: pre-wrap;
  word-wrap: break-word;
  font-family: 'Consolas', 'Monaco', 'Menlo', monospace;
  background-color: #fafafa;
  padding: 10px;
  border-radius: 4px;
  border: 1px solid #eee;
}

.detail-dialog .el-descriptions__title {
  font-size: 16px;
  color: #1a558d;
}

.detail-dialog .detail-block {
  margin-bottom: 20px;
}

.image-container {
  margin-top: 20px;
  text-align: center;
}

.detail-subtitle {
  font-size: 14px;
  color: #606266;
  margin-bottom: 10px;
}

.image-slot {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 200px;
  background: #f5f7fa;
  color: #909399;
}

.permission-notice {
  margin-top: 10px;
}
</style>