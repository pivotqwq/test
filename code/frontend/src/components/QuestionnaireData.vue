<template>
  <div class="questionnaire-container">
    <div v-if="loading" class="loading-container">
      <el-loading :loading="loading" text="加载中..."></el-loading>
    </div>
    
    <!-- 查看模式 -->
    <div v-if="!editMode">
      <div class="header-actions">
        <el-button type="primary" size="small" @click="enterEditMode">
          <el-icon><Edit /></el-icon>
          编辑问卷数据
        </el-button>
      </div>
      
      <el-descriptions title="问卷调查信息" :column="2" border>
        <el-descriptions-item label="问卷ID">{{ questionnaireData.questionnaire_id || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="关联患儿ID">{{ questionnaireData.patient_id || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="问卷类型">{{ questionnaireData.form_type || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="填写日期">{{ questionnaireData.fill_date || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="数据来源">{{ questionnaireData.data_source || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="调查员ID">{{ questionnaireData.investigator_id || '无数据' }}</el-descriptions-item>
        <el-descriptions-item label="创建时间" :span="2">{{ formatDateTime(questionnaireData.create_time) || '无数据' }}</el-descriptions-item>
      </el-descriptions>

      <el-card style="margin-top: 20px;">
        <template #header>
          <div class="card-header">
            <span>原始问卷数据</span>
          </div>
        </template>
        <div class="raw-data">
          <pre>{{ questionnaireData.raw_data || '无原始数据' }}</pre>
        </div>
      </el-card>
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
            <el-form-item label="问卷类型" prop="form_type">
              <el-select v-model="editForm.form_type" style="width: 100%">
                <el-option label="健康评估" value="健康评估" />
                <el-option label="环境暴露" value="环境暴露" />
                <el-option label="心理健康" value="心理健康" />
                <el-option label="营养状况" value="营养状况" />
                <el-option label="用药依从性" value="用药依从性" />
                <el-option label="运动习惯" value="运动习惯" />
                <el-option label="睡眠质量" value="睡眠质量" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="数据来源" prop="data_source">
              <el-select v-model="editForm.data_source" style="width: 100%">
                <el-option label="线上问卷" value="线上问卷" />
                <el-option label="电话访谈" value="电话访谈" />
                <el-option label="面对面访谈" value="面对面访谈" />
                <el-option label="邮寄问卷" value="邮寄问卷" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="填写日期" prop="fill_date">
              <el-date-picker
                v-model="editForm.fill_date"
                type="date"
                style="width: 100%"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <!-- 预留空间 -->
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="原始问卷数据" prop="raw_data">
              <el-input 
                v-model="editForm.raw_data" 
                type="textarea" 
                :rows="8" 
                placeholder="请输入JSON格式的问卷数据"
              />
              <div style="margin-top: 8px; color: #666; font-size: 12px;">
                提示：请输入有效的JSON格式数据，例如：{"q1": 4, "q2": 5, "health_score": 85}
              </div>
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
  questionnaireData: {
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
const questionnaireData = ref({});
const editForm = ref({});
const editFormRef = ref(null);

const editRules = {
  form_type: [{ required: true, message: '请选择问卷类型', trigger: 'change' }],
  data_source: [{ required: true, message: '请选择数据来源', trigger: 'change' }],
  fill_date: [{ required: true, message: '请选择填写日期', trigger: 'change' }],
  raw_data: [
    { 
      validator: (rule, value, callback) => {
        if (!value || value.trim() === '') {
          callback(new Error('请输入原始问卷数据'));
        } else {
          validateJSON(rule, value, callback);
        }
      }, 
      trigger: 'blur' 
    }
  ]
};

// 监听props变化
watch(() => props.questionnaireData, (newData) => {
  if (newData && Object.keys(newData).length > 0) {
    questionnaireData.value = { ...newData };
    console.log('问卷数据 - props变化，更新数据:', newData);
  }
}, { immediate: true, deep: true });

function validateJSON(rule, value, callback) {
  if (!value) {
    callback();
    return;
  }
  
  try {
    JSON.parse(value);
    callback();
  } catch (error) {
    callback(new Error('请输入有效的JSON格式'));
  }
}

onMounted(async () => {
  if (props.questionnaireData && Object.keys(props.questionnaireData).length > 0) {
    questionnaireData.value = { ...props.questionnaireData };
    console.log('问卷数据 - onMounted: 使用传入的数据:', questionnaireData.value);
  } else if (props.patientId) {
    console.log('问卷数据 - onMounted: 没有传入数据，尝试通过API获取，患者ID:', props.patientId);
    await fetchQuestionnaireData(props.patientId);
  } else {
    console.log('问卷数据 - onMounted: 无数据，使用空数据');
    questionnaireData.value = {};
  }
});

const fetchQuestionnaireData = async (patientId) => {
  loading.value = true;
  try {
    // 修正API调用，添加按患者ID过滤的逻辑
    const response = await fetch(`http://localhost:5000/api/QuestionnaireData`, {
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
    
    // 过滤出当前患者的问卷数据
    if (Array.isArray(result)) {
      const patientQuestionnaires = result.filter(q => q.patient_id === patientId);
      if (patientQuestionnaires.length > 0) {
        questionnaireData.value = patientQuestionnaires[0]; // 取第一条记录
        console.log('获取到问卷调查数据:', questionnaireData.value);
      } else {
        questionnaireData.value = {};
        console.log('没有找到该患者的问卷调查数据');
      }
    } else {
      questionnaireData.value = {};
    }
  } catch (error) {
    console.error('API Error:', error);
    ElMessage.warning('暂无问卷调查数据: ' + error.message);
    questionnaireData.value = {};
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
        console.log('问卷数据 - 使用有效的调查员ID:', validInvestigatorId);
      }
    }
  } catch (error) {
    console.warn('问卷数据 - 获取调查员ID失败，使用默认值:', error);
  }
  
  // 复制当前数据到编辑表单，确保所有字段都有默认值
  editForm.value = {
    questionnaire_id: questionnaireData.value.questionnaire_id || `Q${Date.now()}`,
    patient_id: questionnaireData.value.patient_id || props.patientId || '',
    form_type: questionnaireData.value.form_type || '综合健康调研',
    fill_date: questionnaireData.value.fill_date || new Date().toISOString().split('T')[0],
    data_source: questionnaireData.value.data_source || '线上问卷',
    investigator_id: (questionnaireData.value.investigator_id || validInvestigatorId).substring(0, 20),
    raw_data: typeof questionnaireData.value.raw_data === 'string' 
      ? questionnaireData.value.raw_data 
      : JSON.stringify(questionnaireData.value.raw_data || {
          health_score: 85,
          risk_factors: ['待评估'],
          recommendations: ['定期复查']
        }, null, 2),
    create_time: questionnaireData.value.create_time || new Date().toISOString()
  };
  console.log('问卷数据 - 编辑表单初始化:', editForm.value);
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
    
    console.log('问卷数据 - 保存前的表单数据:', editForm.value);
    console.log('问卷数据 - 当前数据:', questionnaireData.value);
    
    const updateData = {
      ...editForm.value,
      // 确保所有必填字段都有值（基于数据库schema）
      questionnaire_id: editForm.value.questionnaire_id || questionnaireData.value.questionnaire_id || `Q${Date.now()}`,
      patient_id: props.patientId || '',
      form_type: editForm.value.form_type || '综合健康调研',
      fill_date: editForm.value.fill_date || new Date().toISOString().split('T')[0],
      data_source: editForm.value.data_source || '线上问卷',
      investigator_id: (editForm.value.investigator_id || localStorage.getItem('userId') || 'INV001').substring(0, 20),
      raw_data: editForm.value.raw_data || JSON.stringify({
        health_score: 85,
        risk_factors: ['待评估'],
        recommendations: ['定期复查']
      }),
      create_time: new Date().toISOString()
    };
    
    // 清理数据，移除导航属性和不需要的字段
    const cleanData = {
      questionnaire_id: updateData.questionnaire_id,
      patient_id: updateData.patient_id,
      form_type: updateData.form_type,
      fill_date: updateData.fill_date,
      data_source: updateData.data_source,
      investigator_id: updateData.investigator_id,
      raw_data: updateData.raw_data,
      create_time: updateData.create_time
    };
    
    console.log('问卷数据 - 准备发送的数据:', updateData);
    console.log('问卷数据 - 清理后的数据:', cleanData);
    
    let response;
    let apiUrl;
    let method;
    
    if (questionnaireData.value.questionnaire_id) {
      // 更新现有记录
      apiUrl = `http://localhost:5000/api/QuestionnaireData/${questionnaireData.value.questionnaire_id}`;
      method = 'PUT';
    } else {
      // 创建新记录
      apiUrl = 'http://localhost:5000/api/QuestionnaireData';
      method = 'POST';
    }
    
    console.log(`问卷数据 - 发送${method}请求到: ${apiUrl}`);
    console.log('问卷数据 - 请求体:', JSON.stringify(cleanData, null, 2));
    
    response = await fetch(apiUrl, {
      method: method,
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(cleanData)
    });
    
    console.log('问卷数据 - API响应状态:', response.status);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error('问卷API错误 - 状态码:', response.status);
      console.error('问卷API错误 - 完整响应:', errorText);
      
      try {
        const errorData = JSON.parse(errorText);
        console.error('问卷API错误 - 解析后的错误:', errorData);
        
        if (errorData.errors) {
          console.error('问卷API错误 - 字段验证错误:', errorData.errors);
          const fieldErrors = Object.entries(errorData.errors).map(([field, messages]) => 
            `${field}: ${Array.isArray(messages) ? messages.join(', ') : messages}`
          ).join('; ');
          throw new Error(`字段验证错误: ${fieldErrors}`);
        } else {
          throw new Error(errorData.message || errorData.title || `HTTP error! status: ${response.status}`);
        }
      } catch (parseError) {
        console.error('问卷API错误 - JSON解析失败:', parseError);
        throw new Error(`HTTP error! status: ${response.status}, response: ${errorText}`);
      }
    }
    
    // 处理成功响应
    const responseData = await response.json();
    console.log('问卷数据 - API成功响应:', responseData);
    
    // 更新本地数据
    questionnaireData.value = { ...updateData };
    editMode.value = false;
    
    ElMessage.success('问卷调查数据保存成功');
    
  } catch (error) {
    console.error('保存失败:', error);
    ElMessage.error('保存失败: ' + error.message);
  } finally {
    saving.value = false;
  }
};

const formatDateTime = (dateTime) => {
  if (!dateTime) return '';
  try {
    return new Date(dateTime).toLocaleString('zh-CN');
  } catch (error) {
    return dateTime;
  }
};
</script>

<style scoped>
.questionnaire-container {
  padding: 10px;
}

.header-actions {
  margin-bottom: 20px;
  text-align: right;
}

.card-header {
  font-weight: bold;
}
.raw-data {
  background-color: #f9f9f9;
  border: 1px solid #eaeaea;
  padding: 15px;
  border-radius: 4px;
  max-height: 300px;
  overflow-y: auto;
  margin-top: 10px;
}
pre {
  margin: 0;
  white-space: pre-wrap;
  word-break: break-all;
  font-family: 'Courier New', Courier, monospace;
}
:deep(.el-descriptions__label) {
  width: 150px;
}
</style> 