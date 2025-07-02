<template>
  <div class="family-environment-container">
    <div v-if="loading" class="loading-container">
      <el-loading :loading="loading" text="加载中..."></el-loading>
    </div>
    
    <!-- 查看模式 -->
    <div v-if="!editMode">
      <div class="header-actions">
        <el-button type="primary" size="small" @click="enterEditMode">
          <el-icon><Edit /></el-icon>
          编辑家庭环境
        </el-button>
      </div>
      
      <el-descriptions
        title="家庭环境数据"
        :column="2"
        border
        v-loading="loading"
      >
        <el-descriptions-item label="家庭ID">{{ familyData.household_id || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="患者ID">{{ familyData.patient_id || '无数据' }}</el-descriptions-item>
        
        <el-descriptions-item label="居住类型" :span="2">{{ getResidenceTypeName(familyData.residence_type) }}</el-descriptions-item>
        <el-descriptions-item label="建筑年龄" :span="2">{{ familyData.building_age ? `${familyData.building_age}年` : '无数据' }}</el-descriptions-item>

        <el-descriptions-item label="通风质量" :span="2">
          <el-tag :type="getVentilationTagType(familyData.ventilation_quality)">
            {{ familyData.ventilation_quality || '无数据' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="室内PM2.5" :span="2">
          <span :class="getPM25Class(familyData.indoor_pm25)">
            {{ familyData.indoor_pm25 ? `${familyData.indoor_pm25} μg/m³` : '无数据' }}
          </span>
        </el-descriptions-item>

        <el-descriptions-item label="宠物饲养" :span="2">
          <el-tag :type="familyData.pet_exposure ? 'warning' : 'info'">
            {{ familyData.pet_exposure ? '有' : '无' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="宠物类型" :span="2">{{ familyData.pet_type || '无数据' }}</el-descriptions-item>

        <el-descriptions-item label="床上用品材质" :span="2">{{ familyData.bedding_material || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="记录日期" :span="2">{{ familyData.record_date ? new Date(familyData.record_date).toLocaleDateString() : '无数据' }}</el-descriptions-item>

        <el-descriptions-item label="调查员ID" :span="2">{{ familyData.investigator_id || '无数据' }}</el-descriptions-item>
      </el-descriptions>
    </div>

    <!-- 编辑模式 -->
    <div v-else>
      <div class="header-actions">
        <el-button type="success" size="small" @click="saveChanges" :loading="saving">
          <el-icon><Check /></el-icon>
          保存修改
        </el-button>
        <el-button size="small" @click="cancelEdit">
          <el-icon><Close /></el-icon>
          取消
        </el-button>
      </div>

      <el-form :model="editForm" :rules="editRules" ref="editFormRef" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="居住类型" prop="residence_type">
              <el-select v-model="editForm.residence_type" style="width: 100%">
                <el-option label="公寓" value="1" />
                <el-option label="独立住宅" value="2" />
                <el-option label="联排别墅" value="3" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="建筑年龄" prop="building_age">
              <el-input-number v-model="editForm.building_age" :min="0" :max="200" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="通风质量" prop="ventilation_quality">
              <el-select v-model="editForm.ventilation_quality" style="width: 100%">
                <el-option label="良好" value="良好" />
                <el-option label="一般" value="一般" />
                <el-option label="较差" value="较差" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="室内PM2.5" prop="indoor_pm25">
              <el-input-number v-model="editForm.indoor_pm25" :min="0" :max="500" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="宠物饲养" prop="pet_exposure">
              <el-switch v-model="editForm.pet_exposure" active-text="有" inactive-text="无" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="宠物类型" prop="pet_type">
              <el-input v-model="editForm.pet_type" placeholder="如：猫、狗等" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="床上用品材质" prop="bedding_material">
              <el-input v-model="editForm.bedding_material" placeholder="如：棉质、丝质等" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
  </div>
</template>

<script setup>
import { defineProps, ref, onMounted, watch } from 'vue';
import { ElMessage } from 'element-plus';
import { Edit, Check, Close } from '@element-plus/icons-vue';

const props = defineProps({
  familyData: {
    type: Object,
    default: () => ({})
  },
  patientId: {
    type: String,
    default: ''
  }
});

const loading = ref(false);
const saving = ref(false);
const editMode = ref(false);
const familyData = ref({});
const editForm = ref({});
const editFormRef = ref(null);

const editRules = {
  residence_type: [{ required: true, message: '请选择居住类型', trigger: 'change' }],
  ventilation_quality: [{ required: true, message: '请选择通风质量', trigger: 'change' }],
  pet_type: [
    { 
      validator: (rule, value, callback) => {
        // 如果有宠物但没有填写宠物类型，则报错
        if (editForm.value.pet_exposure && (!value || value.trim() === '')) {
          callback(new Error('请输入宠物类型'));
        } else {
          callback();
        }
      }, 
      trigger: 'blur' 
    }
  ]
};

// 监听props变化
watch(() => props.familyData, (newData) => {
  if (newData && Object.keys(newData).length > 0) {
    familyData.value = { ...newData };
    console.log('家庭环境 - props变化，更新数据:', newData);
  }
}, { immediate: true, deep: true });

// 监听宠物饲养状态变化，重新验证宠物类型
watch(() => editForm.value.pet_exposure, (newValue) => {
  if (editFormRef.value) {
    // 清除宠物类型的验证状态
    editFormRef.value.clearValidate(['pet_type']);
    // 如果没有宠物，清空宠物类型
    if (!newValue) {
      editForm.value.pet_type = '';
    }
  }
}, { immediate: false });

onMounted(async () => {
  // 优先使用传入的familyData，如果没有再尝试通过API获取
  if (props.familyData && Object.keys(props.familyData).length > 0) {
    familyData.value = { ...props.familyData };
    console.log('家庭环境 - onMounted: 使用传入的数据:', familyData.value);
  } else if (props.patientId) {
    console.log('家庭环境 - onMounted: 没有传入数据，尝试通过API获取，患者ID:', props.patientId);
    await fetchFamilyEnvironmentData(props.patientId);
  } else {
    // 如果既没有传入数据也没有patientId，使用默认数据
    console.log('家庭环境 - onMounted: 无数据，使用空数据');
    familyData.value = {};
  }
});

const fetchFamilyEnvironmentData = async (patientId) => {
  loading.value = true;
  try {
    // 修正API路径，使用正确的路由
    const response = await fetch(`http://localhost:5000/api/HouseholdEnvironment/patient/${patientId}`, {
      method: 'GET',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      if (response.status === 404) {
        console.log(`患者 ${patientId} 暂无家庭环境数据`);
        familyData.value = {};
        return;
      }
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    
    if (result && result.length > 0) {
      familyData.value = result[0]; // 取第一条记录
      console.log('获取到家庭环境数据:', familyData.value);
    } else {
      familyData.value = {};
      console.log('没有找到家庭环境数据');
    }
  } catch (error) {
    console.error('API Error:', error);
    ElMessage.warning('暂无家庭环境数据: ' + error.message);
    familyData.value = {};
  } finally {
    loading.value = false;
  }
};

const enterEditMode = async () => {
  editMode.value = true;
  
  // 先获取有效的调查员ID
  let validInvestigatorId = 'INV001'; // 默认值
  try {
    const investigatorResponse = await fetch('http://localhost:5000/api/InvestigatorQualification', {
      method: 'GET',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    });
    
    if (investigatorResponse.ok) {
      const investigators = await investigatorResponse.json();
      if (Array.isArray(investigators) && investigators.length > 0) {
        validInvestigatorId = investigators[0].investigator_id;
        console.log('家庭环境 - 使用有效的调查员ID:', validInvestigatorId);
      }
    }
  } catch (error) {
    console.warn('家庭环境 - 获取调查员ID失败，使用默认值:', error);
  }
  
  // 复制当前数据到编辑表单，确保所有字段都有默认值
  editForm.value = {
    household_id: familyData.value.household_id || `HH${Date.now()}`,
    patient_id: familyData.value.patient_id || props.patientId || '',
    residence_type: familyData.value.residence_type || '1',
    building_age: familyData.value.building_age || null,
    ventilation_quality: familyData.value.ventilation_quality || '良好',
    indoor_pm25: familyData.value.indoor_pm25 || null,
    pet_exposure: familyData.value.pet_exposure || false,
    pet_type: familyData.value.pet_type || '',
    bedding_material: familyData.value.bedding_material || '棉质',
    record_date: familyData.value.record_date || new Date().toISOString(),
    investigator_id: (familyData.value.investigator_id || validInvestigatorId).substring(0, 20)
  };
  console.log('家庭环境 - 编辑表单初始化:', editForm.value);
};

const cancelEdit = () => {
  editMode.value = false;
  editForm.value = {};
  if (editFormRef.value) {
    editFormRef.value.clearValidate();
  }
};

const saveChanges = async () => {
  if (!editFormRef.value) return;
  
  try {
    const valid = await editFormRef.value.validate();
    if (!valid) return;
    
    saving.value = true;
    
    console.log('家庭环境 - 保存前的表单数据:', editForm.value);
    console.log('家庭环境 - 当前数据:', familyData.value);
    
    const updateData = {
      ...editForm.value,
      record_date: new Date().toISOString(),
      patient_id: props.patientId || '',
      // 确保所有必填字段都有值（基于数据库schema）
      household_id: editForm.value.household_id || familyData.value.household_id || `HH${Date.now()}`,
      residence_type: editForm.value.residence_type || '1',
      ventilation_quality: editForm.value.ventilation_quality || '良好',
      pet_type: editForm.value.pet_exposure ? (editForm.value.pet_type || '猫') : '无',
      bedding_material: editForm.value.bedding_material || '棉质',
      investigator_id: (editForm.value.investigator_id || localStorage.getItem('userId') || 'INV001').substring(0, 20),
      // 确保数值字段不为undefined
      building_age: editForm.value.building_age || null,
      indoor_pm25: editForm.value.indoor_pm25 || null,
      pet_exposure: editForm.value.pet_exposure || false
    };
    
    // 清理数据，移除导航属性和不需要的字段
    const cleanData = {
      household_id: updateData.household_id,
      patient_id: updateData.patient_id,
      residence_type: updateData.residence_type,
      building_age: updateData.building_age,
      ventilation_quality: updateData.ventilation_quality,
      indoor_pm25: updateData.indoor_pm25,
      pet_exposure: updateData.pet_exposure,
      pet_type: updateData.pet_type,
      bedding_material: updateData.bedding_material,
      record_date: updateData.record_date,
      investigator_id: updateData.investigator_id
    };
    
    console.log('家庭环境 - 准备发送的数据:', updateData);
    console.log('家庭环境 - 清理后的数据:', cleanData);
    
    let response;
    let apiUrl;
    let method;
    
    if (familyData.value.household_id) {
      // 更新现有记录
      apiUrl = `http://localhost:5000/api/HouseholdEnvironment/${familyData.value.household_id}`;
      method = 'PUT';
    } else {
      // 创建新记录
      apiUrl = 'http://localhost:5000/api/HouseholdEnvironment';
      method = 'POST';
    }
    
    console.log(`家庭环境 - 发送${method}请求到: ${apiUrl}`);
    console.log('家庭环境 - 请求体:', JSON.stringify(cleanData, null, 2));
    
    response = await fetch(apiUrl, {
      method: method,
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(cleanData)
    });
    
    console.log('家庭环境 - API响应状态:', response.status);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error('家庭环境API错误 - 状态码:', response.status);
      console.error('家庭环境API错误 - 完整响应:', errorText);
      
      try {
        const errorData = JSON.parse(errorText);
        console.error('家庭环境API错误 - 解析后的错误:', errorData);
        
        if (errorData.errors) {
          console.error('家庭环境API错误 - 字段验证错误:', errorData.errors);
          const fieldErrors = Object.entries(errorData.errors).map(([field, messages]) => 
            `${field}: ${Array.isArray(messages) ? messages.join(', ') : messages}`
          ).join('; ');
          throw new Error(`字段验证错误: ${fieldErrors}`);
        } else {
          throw new Error(errorData.message || errorData.title || `HTTP error! status: ${response.status}`);
        }
      } catch (parseError) {
        console.error('家庭环境API错误 - JSON解析失败:', parseError);
        throw new Error(`HTTP error! status: ${response.status}, response: ${errorText}`);
      }
    }
    
    // 处理成功响应
    const responseData = await response.json();
    console.log('家庭环境 - API成功响应:', responseData);
    
    // 更新本地数据
    familyData.value = { ...updateData };
    editMode.value = false;
    
    ElMessage.success('家庭环境数据保存成功');
    
  } catch (error) {
    console.error('保存失败:', error);
    ElMessage.error('保存失败: ' + error.message);
  } finally {
    saving.value = false;
  }
};

const getResidenceTypeName = (residenceType) => {
  const typeMap = {
    '1': '公寓',
    '2': '独立住宅',
    '3': '联排别墅'
  };
  return typeMap[residenceType] || '无数据';
};

const getVentilationTagType = (ventilation) => {
  if (!ventilation) return 'info';
  switch (ventilation) {
    case '良好': return 'success';
    case '一般': return 'warning';
    case '较差': return 'danger';
    default: return 'info';
  }
};

const getPM25Class = (pm25) => {
  if (pm25 === undefined || pm25 === null) return '';
  const value = parseFloat(pm25);
  if (value <= 15) return 'good-value';
  if (value <= 35) return 'moderate-value';
  return 'bad-value';
};
</script>

<style scoped>
.family-environment-container {
  padding: 10px;
}

.header-actions {
  margin-bottom: 20px;
  text-align: right;
}

.good-value {
  color: #67c23a;
  font-weight: bold;
}
.moderate-value {
  color: #e6a23c;
  font-weight: bold;
}
.bad-value {
  color: #f56c6c;
  font-weight: bold;
}
:deep(.el-descriptions__label) {
  width: 150px;
}
</style> 