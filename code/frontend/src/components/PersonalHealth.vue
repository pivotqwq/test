<template>
  <div class="health-behavior-container">
    <div v-if="loading" class="loading-container">
      <el-loading :loading="loading" text="加载中..."></el-loading>
    </div>
    
    <!-- 查看模式 -->
    <div v-if="!editMode">
      <div class="header-actions">
        <el-button type="primary" size="small" @click="enterEditMode">
          <el-icon><Edit /></el-icon>
          编辑健康行为
        </el-button>
      </div>
      
      <el-descriptions
        title="个人健康行为数据"
        :column="2"
        border
        v-loading="loading"
      >
        <el-descriptions-item label="行为健康信息ID">{{ healthBehaviorData.individual_id || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="患儿ID">{{ healthBehaviorData.patient_id || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="关联家庭ID">{{ healthBehaviorData.household_id || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="饮食模式" :span="2">{{ healthBehaviorData.diet_pattern || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="维生素D水平" :span="2">
          <span :class="getVitaminDClass(healthBehaviorData.vitamin_d_level)">
            {{ healthBehaviorData.vitamin_d_level ? `${healthBehaviorData.vitamin_d_level} ng/mL` : '无数据' }}
          </span>
        </el-descriptions-item>
        <el-descriptions-item label="日照情况" :span="2">
          <el-tag :type="healthBehaviorData.sun_exposure ? 'success' : 'info'">
            {{ healthBehaviorData.sun_exposure ? '充足' : '不足' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="疫苗接种状态" :span="2">
          <el-tag :type="healthBehaviorData.vaccination_status ? 'success' : 'warning'">
            {{ healthBehaviorData.vaccination_status ? '已接种' : '未接种' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="抗生素使用频率" :span="2">{{ healthBehaviorData.antibiotic_usage_frequency || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="生命早期用药记录" :span="2">{{ healthBehaviorData.early_life_medication || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="二手烟暴露" :span="2">
          <el-tag :type="healthBehaviorData.smoke_exposure ? 'danger' : 'success'">
            {{ healthBehaviorData.smoke_exposure ? '有' : '无' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="调查员ID" :span="2">{{ healthBehaviorData.investigator_id || '无数据' }}</el-descriptions-item>
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

      <el-form :model="editForm" :rules="editRules" ref="editFormRef" label-width="140px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="饮食模式" prop="diet_pattern">
              <el-input v-model="editForm.diet_pattern" placeholder="如：均衡饮食、素食为主" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="维生素D水平" prop="vitamin_d_level">
              <el-input-number v-model="editForm.vitamin_d_level" :min="0" :max="200" :precision="1" style="width: 100%" />
              <span style="margin-left: 8px; color: #666;">ng/mL</span>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="日照情况" prop="sun_exposure">
              <el-switch v-model="editForm.sun_exposure" active-text="充足" inactive-text="不足" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="疫苗接种状态" prop="vaccination_status">
              <el-switch v-model="editForm.vaccination_status" active-text="已接种" inactive-text="未接种" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="抗生素使用频率" prop="antibiotic_usage_frequency">
              <el-select v-model="editForm.antibiotic_usage_frequency" style="width: 100%">
                <el-option label="很少" value="很少" />
                <el-option label="偶尔" value="偶尔" />
                <el-option label="经常" value="经常" />
                <el-option label="从不" value="从不" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="二手烟暴露" prop="smoke_exposure">
              <el-switch v-model="editForm.smoke_exposure" active-text="有" inactive-text="无" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="生命早期用药记录" prop="early_life_medication">
              <el-input 
                v-model="editForm.early_life_medication" 
                type="textarea" 
                :rows="3" 
                placeholder="详细记录早期用药情况"
              />
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
  healthBehaviorData: {
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
const healthBehaviorData = ref({});
const editForm = ref({});
const editFormRef = ref(null);

const editRules = {
  diet_pattern: [{ required: true, message: '请输入饮食模式', trigger: 'blur' }],
  antibiotic_usage_frequency: [{ required: true, message: '请选择抗生素使用频率', trigger: 'change' }],
  vitamin_d_level: [
    { 
      validator: (rule, value, callback) => {
        if (value !== null && value !== undefined && (value < 0 || value > 200)) {
          callback(new Error('维生素D水平应在0-200之间'));
        } else {
          callback();
        }
      }, 
      trigger: 'blur' 
    }
  ]
};

// 监听props变化
watch(() => props.healthBehaviorData, (newData) => {
  if (newData && Object.keys(newData).length > 0) {
    healthBehaviorData.value = { ...newData };
    console.log('健康行为 - props变化，更新数据:', newData);
  }
}, { immediate: true, deep: true });

onMounted(async () => {
  if (props.healthBehaviorData && Object.keys(props.healthBehaviorData).length > 0) {
    healthBehaviorData.value = { ...props.healthBehaviorData };
    console.log('健康行为 - onMounted: 使用传入的数据:', healthBehaviorData.value);
  } else if (props.patientId) {
    console.log('健康行为 - onMounted: 没有传入数据，尝试通过API获取，患者ID:', props.patientId);
    await fetchHealthBehaviorData(props.patientId);
  } else {
    console.log('健康行为 - onMounted: 无数据，使用空数据');
    healthBehaviorData.value = {};
  }
});

const fetchHealthBehaviorData = async (patientId) => {
  loading.value = true;
  try {
    // 修正API路径，使用正确的路由
    const response = await fetch(`http://localhost:5000/api/IndividualHealthBehavior/ByPatient/${patientId}`, {
      method: 'GET',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      if (response.status === 404) {
        console.log(`患者 ${patientId} 暂无个人健康行为数据`);
        healthBehaviorData.value = {};
        return;
      }
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    
    if (result && result.length > 0) {
      healthBehaviorData.value = result[0]; // 取第一条记录
      console.log('获取到个人健康行为数据:', healthBehaviorData.value);
    } else {
      healthBehaviorData.value = {};
      console.log('没有找到个人健康行为数据');
    }
  } catch (error) {
    console.error('API Error:', error);
    ElMessage.warning('暂无个人健康行为数据: ' + error.message);
    healthBehaviorData.value = {};
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
        console.log('健康行为 - 使用有效的调查员ID:', validInvestigatorId);
      }
    }
  } catch (error) {
    console.warn('健康行为 - 获取调查员ID失败，使用默认值:', error);
  }
  
  // 复制当前数据到编辑表单，确保所有字段都有默认值
  editForm.value = {
    individual_id: healthBehaviorData.value.individual_id || `IHB${Date.now()}`,
    patient_id: healthBehaviorData.value.patient_id || props.patientId || '',
    household_id: healthBehaviorData.value.household_id || `HH${Date.now()}`,
    diet_pattern: healthBehaviorData.value.diet_pattern || '均衡饮食',
    vitamin_d_level: healthBehaviorData.value.vitamin_d_level || null,
    sun_exposure: healthBehaviorData.value.sun_exposure || false,
    vaccination_status: healthBehaviorData.value.vaccination_status || false,
    antibiotic_usage_frequency: healthBehaviorData.value.antibiotic_usage_frequency || '很少',
    early_life_medication: healthBehaviorData.value.early_life_medication || '无特殊用药记录',
    smoke_exposure: healthBehaviorData.value.smoke_exposure || false,
    investigator_id: (healthBehaviorData.value.investigator_id || validInvestigatorId).substring(0, 20)
  };
  console.log('健康行为 - 编辑表单初始化:', editForm.value);
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
    
    console.log('健康行为 - 保存前的表单数据:', editForm.value);
    console.log('健康行为 - 当前数据:', healthBehaviorData.value);
    
    const updateData = {
      ...editForm.value,
      // 确保所有必填字段都有值（基于数据库schema）
      individual_id: editForm.value.individual_id || healthBehaviorData.value.individual_id || `IHB${Date.now()}`,
      patient_id: props.patientId || '',
      household_id: editForm.value.household_id || healthBehaviorData.value.household_id || `HH${Date.now()}`,
      diet_pattern: editForm.value.diet_pattern || '均衡饮食',
      antibiotic_usage_frequency: editForm.value.antibiotic_usage_frequency || '很少',
      early_life_medication: editForm.value.early_life_medication || '无特殊用药记录',
      investigator_id: (editForm.value.investigator_id || localStorage.getItem('userId') || 'INV001').substring(0, 20),
      // 确保布尔字段有值
      sun_exposure: editForm.value.sun_exposure || false,
      vaccination_status: editForm.value.vaccination_status || false,
      smoke_exposure: editForm.value.smoke_exposure || false,
      // 确保数值字段不为undefined
      vitamin_d_level: editForm.value.vitamin_d_level || null
    };
    
    // 清理数据，移除导航属性和不需要的字段
    const cleanData = {
      individual_id: updateData.individual_id,
      patient_id: updateData.patient_id,
      household_id: updateData.household_id,
      diet_pattern: updateData.diet_pattern,
      vitamin_d_level: updateData.vitamin_d_level,
      sun_exposure: updateData.sun_exposure,
      vaccination_status: updateData.vaccination_status,
      antibiotic_usage_frequency: updateData.antibiotic_usage_frequency,
      early_life_medication: updateData.early_life_medication,
      smoke_exposure: updateData.smoke_exposure,
      investigator_id: updateData.investigator_id
    };
    
    console.log('健康行为 - 准备发送的数据:', updateData);
    console.log('健康行为 - 清理后的数据:', cleanData);
    
    let response;
    let apiUrl;
    let method;
    
    if (healthBehaviorData.value.individual_id) {
      // 更新现有记录
      apiUrl = `http://localhost:5000/api/IndividualHealthBehavior/${healthBehaviorData.value.individual_id}`;
      method = 'PUT';
    } else {
      // 创建新记录
      apiUrl = 'http://localhost:5000/api/IndividualHealthBehavior';
      method = 'POST';
    }
    
    console.log(`健康行为 - 发送${method}请求到: ${apiUrl}`);
    console.log('健康行为 - 请求体:', JSON.stringify(cleanData, null, 2));
    
    response = await fetch(apiUrl, {
      method: method,
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(cleanData)
    });
    
    console.log('健康行为 - API响应状态:', response.status);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error('健康行为API错误 - 状态码:', response.status);
      console.error('健康行为API错误 - 完整响应:', errorText);
      
      try {
        const errorData = JSON.parse(errorText);
        console.error('健康行为API错误 - 解析后的错误:', errorData);
        
        if (errorData.errors) {
          console.error('健康行为API错误 - 字段验证错误:', errorData.errors);
          const fieldErrors = Object.entries(errorData.errors).map(([field, messages]) => 
            `${field}: ${Array.isArray(messages) ? messages.join(', ') : messages}`
          ).join('; ');
          throw new Error(`字段验证错误: ${fieldErrors}`);
        } else {
          throw new Error(errorData.message || errorData.title || `HTTP error! status: ${response.status}`);
        }
      } catch (parseError) {
        console.error('健康行为API错误 - JSON解析失败:', parseError);
        throw new Error(`HTTP error! status: ${response.status}, response: ${errorText}`);
      }
    }
    
    // 处理成功响应
    const responseData = await response.json();
    console.log('健康行为 - API成功响应:', responseData);
    
    // 更新本地数据
    healthBehaviorData.value = { ...updateData };
    editMode.value = false;
    
    ElMessage.success('个人健康行为数据保存成功');
    
  } catch (error) {
    console.error('保存失败:', error);
    ElMessage.error('保存失败: ' + error.message);
  } finally {
    saving.value = false;
  }
};

const getVitaminDClass = (level) => {
  if (level === undefined || level === null) return '';
  const value = parseFloat(level);
  if (value < 20) return 'bad-value';
  if (value < 30) return 'moderate-value';
  return 'good-value';
};
</script>

<style scoped>
.health-behavior-container {
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
  width: 180px;
}
</style> 