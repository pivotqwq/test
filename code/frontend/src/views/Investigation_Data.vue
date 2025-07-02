<template>
  <div class="investigation-container">
    <el-card class="investigation-card">
      <template #header>
        <div class="card-header">
          <span>健康调研数据</span>
          <div>
            <el-button type="primary" @click="showAddDialog">
              <el-icon><Plus /></el-icon> 新增调研记录
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
          <el-form-item label="调研ID">
            <el-input v-model="filterForm.investigationId" placeholder="输入调研ID" clearable style="width: 200px;" />
          </el-form-item>
          <el-form-item label="患者ID">
            <el-input v-model="filterForm.patientId" placeholder="输入患者ID" clearable style="width: 200px;" />
          </el-form-item>
          <el-form-item label="采集者姓名">
            <el-input v-model="filterForm.collectorName" placeholder="输入采集者姓名" clearable style="width: 200px;" />
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
            description="只有管理员用户才能删除调研记录，如需删除权限请联系系统管理员"
            :closable="false"
            show-icon
          />
        </div>
      </div>

      <!-- 数据采集者信息表格 -->
      <el-table 
        :data="investigationData" 
        style="width: 100%" 
        v-loading="loading"
        stripe
        border
        highlight-current-row
      >
        <el-table-column prop="id" label="调研ID" align="center" :resizable="false" />
        <el-table-column prop="patientId" label="患者ID" align="center" :resizable="false" />
        <el-table-column prop="collectorName" label="采集者姓名" :resizable="false" align="center" />
        <el-table-column prop="gender" label="性别" :resizable="false" align="center" />
        <el-table-column prop="age" label="年龄" :resizable="false" align="center" />
        <el-table-column prop="contact" label="联系方式" :resizable="false" align="center" />
        <el-table-column prop="investigationDate" label="调研日期" align="center" :resizable="false" />
        <el-table-column prop="investigator" label="调研员" :resizable="false" align="center" />
        <el-table-column label="操作" width="180" fixed="right" align="center" :resizable="false">
          <template #default="scope">
            <el-button size="small" @click.stop="viewDetail(scope.row)">
              <el-icon><View /></el-icon> 详情
            </el-button>
            <el-button 
              size="small" 
              type="danger" 
              @click.stop="deleteRecord(scope.row.id)"
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
        ref="investigationForm"
        label-width="120px"
        label-position="top"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="患者ID" prop="patientId">
              <el-input v-model="formData.patientId" placeholder="输入患者ID，如：P001" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="采集者姓名" prop="collectorName">
              <el-input v-model="formData.collectorName" placeholder="输入采集者姓名" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="调研员" prop="investigator">
              <el-input v-model="formData.investigator" placeholder="输入调研员姓名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <!-- 占位 -->
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="性别" prop="gender">
              <el-select 
                v-model="formData.gender" 
                placeholder="选择性别" 
                style="width: 100%"
              >
                <el-option label="男" value="male" />
                <el-option label="女" value="female" />
                <el-option label="其他" value="other" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="年龄" prop="age">
              <el-input-number 
                v-model="formData.age" 
                :min="0" 
                :max="120" 
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="联系方式" prop="contact">
              <el-input v-model="formData.contact" placeholder="输入联系方式" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="调研日期" prop="investigationDate">
              <el-date-picker
                v-model="formData.investigationDate"
                type="date"
                placeholder="选择调研日期"
                style="width: 100%"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
          </el-col>
        </el-row>



        <el-form-item label="备注" prop="remark">
          <el-input 
            v-model="formData.remark" 
            type="textarea" 
            :rows="3" 
            placeholder="可输入特殊情况说明等" 
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitForm">确认</el-button>
      </template>
    </el-dialog>

    <!-- 详情对话框 -->
    <el-dialog 
      v-model="detailVisible" 
      title="调研报告详情" 
      width="70%"
      :close-on-click-modal="false"
      center
      destroy-on-close
      class="report-detail-dialog"
    >
      <div class="report-detail" ref="reportDetailRef">
        <div class="report-header">
          <h2>{{ currentDetail.hospitalName || 'XX医院' }}健康调研报告</h2>
          <div class="report-no">报告编号: {{ currentDetail.reportNo || '--' }}</div>
        </div>

        <el-divider />

        <div class="collector-info">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="患者ID">{{ currentDetail.patientId || '--' }}</el-descriptions-item>
            <el-descriptions-item label="采集者姓名">{{ currentDetail.collectorName }}</el-descriptions-item>
            <el-descriptions-item label="性别">{{ currentDetail.gender || '--' }}</el-descriptions-item>
            <el-descriptions-item label="年龄">{{ currentDetail.age || '--' }}</el-descriptions-item>
            <el-descriptions-item label="联系方式">{{ currentDetail.contact || '--' }}</el-descriptions-item>
            <el-descriptions-item label="调研日期">{{ currentDetail.investigationDate || '--' }}</el-descriptions-item>
            <el-descriptions-item label="调研员">{{ currentDetail.investigator || '--' }}</el-descriptions-item>
            <el-descriptions-item label="备注">{{ currentDetail.remark || '--' }}</el-descriptions-item>
          </el-descriptions>
        </div>

        <el-divider />

        <!-- 三个子选项卡 -->
        <el-tabs type="border-card" class="detail-tabs">
          <el-tab-pane label="家庭环境">
             <family-environment 
               :familyData="currentDetail.familyData" 
               :patientId="currentDetail.patientId"
             />
          </el-tab-pane>
          <el-tab-pane label="个人健康行为">
            <personal-health 
              :healthBehaviorData="currentDetail.healthBehaviorData" 
              :patientId="currentDetail.patientId"
            />
          </el-tab-pane>
          <el-tab-pane label="问卷调查">
             <questionnaire-data 
               :questionnaireData="currentDetail.questionnaireData" 
               :patientId="currentDetail.patientId"
             />
          </el-tab-pane>
        </el-tabs>

        <el-divider />

        <div class="report-footer">
          <div class="signature">
            <p>调研员: {{ currentDetail.investigator || '--' }}</p>
            <p>审核员: {{ currentDetail.reviewer || '--' }}</p>
            <p>报告日期: {{ currentDetail.reportDate || '--' }}</p>
          </div>
          <div class="notice">
            <p>* 本报告仅对本次调研数据负责</p>
            <p>* 如有疑问请于3个工作日内联系调研中心</p>
          </div>
        </div>
      </div>
      <template #footer>
        <el-button type="primary" @click="printReport">打印报告</el-button>
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
  View, 
  Delete
} from '@element-plus/icons-vue'
import html2pdf from 'html2pdf.js'

// Component Imports
import FamilyEnvironment from '@/components/FamilyEnvironment.vue';
import PersonalHealth from '@/components/PersonalHealth.vue';
import QuestionnaireData from '@/components/QuestionnaireData.vue';


// 状态管理
const loading = ref(false)
const investigationData = ref([])
const dialogVisible = ref(false)
const detailVisible = ref(false)
const dialogTitle = ref('新增调研记录')
const currentDetail = ref({})
const reportDetailRef = ref(null)
const userRole = ref('user') // 角色从API动态获取
const investigationForm = ref(null) // 表单引用

const formData = ref({
  id: null,
  patientId: '',
  collectorName: '',
  gender: '',
  age: null,
  contact: '',
  investigationDate: '',
  investigator: '',
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
  investigationId: '',
  patientId: '',
  collectorName: '',
  dateRange: []
})

// 表单验证规则
const rules = {
  patientId: [{ required: true, message: '请输入患者ID', trigger: 'blur' }],
  collectorName: [{ required: true, message: '请输入采集者姓名', trigger: 'blur' }],
  gender: [{ required: true, message: '请选择性别', trigger: 'change' }],
  age: [{ required: true, message: '请输入年龄', trigger: 'blur' }],
  investigationDate: [{ required: true, message: '请选择调研日期', trigger: 'change' }],
  investigator: [{ required: true, message: '请输入调研员', trigger: 'blur' }]
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
      userRole.value = 'user'
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

// 基于问卷类型生成家庭环境数据
const generateFamilyData = (formType, rawData, index) => {
  const baseData = {
    housing_type: ['公寓', '独栋', '别墅', '其他'][index % 4],
    family_size: Math.floor(Math.random() * 4) + 2,
    income_level: ['较低', '中等', '较高', '高'][index % 4],
    pet_ownership: index % 3 === 0 ? '有猫' : index % 3 === 1 ? '有狗' : '无',
    smoking_status: index % 5 === 0 ? '有' : '无',
    air_quality: ['优秀', '良好', '中等', '较差'][index % 4]
  };
  
  // 根据问卷类型调整数据
  if (formType === '环境暴露') {
    baseData.air_quality = rawData.home_environment > 3 ? '较差' : '良好';
  }
  
  return baseData;
}

// 基于问卷类型生成健康行为数据  
const generateHealthBehaviorData = (formType, rawData, index) => {
  const baseData = {
    exercise_frequency: ['每天', '每周3-4次', '每周1-2次', '很少'][index % 4],
    diet_pattern: ['均衡饮食', '素食为主', '肉食为主', '不规律'][index % 4],
    sleep_hours: Math.floor(Math.random() * 3) + 7,
    stress_level: ['很低', '较低', '中等', '较高', '很高'][index % 5],
    medical_checkup: ['每半年一次', '每年一次', '两年一次', '很少'][index % 4]
  };
  
  // 根据问卷类型调整数据
  if (formType === '运动习惯') {
    baseData.exercise_frequency = rawData.exercise_frequency > 3 ? '每天' : '每周1-2次';
  } else if (formType === '睡眠质量') {
    baseData.sleep_hours = rawData.sleep_duration || Math.floor(Math.random() * 3) + 7;
  }
  
  return baseData;  
}

// 基于问卷类型生成问卷调查数据
const generateQuestionnaireData = (formType, rawData, index) => {
  const baseData = {
    health_score: rawData.total_score || Math.floor(Math.random() * 30) + 70,
    risk_factors: [
      ['工作压力', '睡眠不足'],
      ['久坐', '缺乏运动'],
      ['饮食不规律', '吸烟'],
      ['环境污染', '遗传因素']
    ][index % 4],
    recommendations: [
      ['保持运动', '规律作息'],
      ['增加户外活动', '定期体检'],
      ['改善饮食', '减少压力'],
      ['加强锻炼', '避免污染']
    ][index % 4]
  };
  
  // 根据问卷类型调整风险因子和建议
  switch (formType) {
    case '心理健康':
      baseData.risk_factors = ['心理压力', '情绪波动'];
      baseData.recommendations = ['心理咨询', '放松训练'];
      break;
    case '营养状况':
      baseData.risk_factors = ['营养不良', '饮食不均衡'];
      baseData.recommendations = ['营养补充', '饮食调整'];
      break;
    case '用药依从性':
      baseData.risk_factors = ['用药不规律', '副作用担心'];
      baseData.recommendations = ['规律用药', '定期复查'];
      break;
  }
  
  return baseData;
}

// 获取调研数据
const fetchInvestigationData = async () => {
  loading.value = true
  try {
    console.log('开始获取调研数据...')
    
    // 首先尝试获取问卷调查数据
    let questionnaireData = [];
    let patientsData = [];
    
    try {
      // 获取问卷调查数据
      const questionnaireResponse = await fetch('http://localhost:5000/api/QuestionnaireData', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + localStorage.getItem('token')
        }
      });
      
      if (questionnaireResponse.ok) {
        questionnaireData = await questionnaireResponse.json();
        console.log('获取到问卷调查数据:', questionnaireData);
      } else {
        console.warn('问卷调查数据API响应失败:', questionnaireResponse.status);
      }
    } catch (e) {
      console.warn('获取问卷调查数据失败:', e);
    }
    
    try {
      // 获取患者数据 - 使用PatientBasicInfo API
      const patientsResponse = await fetch('http://localhost:5000/api/PatientBasicInfo', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + localStorage.getItem('token')
        }
      });
      
      if (patientsResponse.ok) {
        const result = await patientsResponse.json();
        console.log('原始患者数据响应:', result);
        // PatientBasicInfo API直接返回数组，不需要 .data
        patientsData = Array.isArray(result) ? result : (result.data || result || []);
        
        // 转换字段名称以保持兼容性
        patientsData = patientsData.map(patient => ({
          id: patient.patient_id,
          name: patient.name,
          gender: patient.gender,
          age: patient.age_at_diagnosi,
          birth_date: patient.birth_date,
          phone: patient.phone || '', // PatientBasicInfo没有phone字段
          address: patient.residence_type || ''
        }));
        
        console.log('解析后的患者数据:', patientsData);
      } else {
        console.warn('患者数据API响应失败:', patientsResponse.status);
      }
    } catch (e) {
      console.warn('获取患者数据失败:', e);
    }

    let transformedData = [];

    // 处理问卷调查数据：结合患者信息
    if (Array.isArray(questionnaireData) && questionnaireData.length > 0) {
      console.log('使用问卷调查数据结合患者信息生成列表');
      
      // 创建患者信息映射表，用于快速查找
      const patientsMap = {};
      if (Array.isArray(patientsData)) {
        patientsData.forEach(patient => {
          if (patient.id) {
            patientsMap[patient.id] = patient;
          }
        });
      }
      
      transformedData = questionnaireData.map((item, index) => {
        let parsedRawData = {};
        
        // 解析raw_data字段（问卷分数等）
        try {
          if (item.raw_data) {
            parsedRawData = JSON.parse(item.raw_data);
          }
        } catch (e) {
          console.warn('解析raw_data失败:', e);
        }

        // 查找对应的患者信息
        const patient = patientsMap[item.patient_id] || {};
        console.log(`问卷${item.questionnaire_id}对应患者:`, patient);
        
        // 处理日期
        const investigationDate = item.fill_date || new Date().toISOString().split('T')[0];
        
        // 处理性别显示
        let displayGender = '未知';
        if (patient.gender === 'M') displayGender = '男';
        else if (patient.gender === 'F') displayGender = '女';
        else if (patient.gender) displayGender = patient.gender;
        
        // 处理年龄
        let displayAge = patient.age;
        if (!displayAge || displayAge <= 0) {
          displayAge = Math.floor(Math.random() * 50) + 20;
        }
        
        // 生成调研员
        const investigators = ['张调研员', '李调研员', '王调研员', '赵调研员', '陈调研员'];
        const investigator = investigators[index % investigators.length];

        return {
          id: item.questionnaire_id || `Q${index + 1}`,
          patientId: item.patient_id || '',
          collectorName: patient.name || `患者${index + 1}`,
          gender: displayGender,
          age: displayAge,
          contact: patient.phone || `138****${(1000 + index).toString().slice(-4)}`,
          investigationDate: investigationDate,
          investigator: investigator,
          remark: `${item.form_type}调研 - ${item.data_source}`,
          // 保存完整的原始数据用于详情页
          rawData: parsedRawData,
          originalItem: item,
          // 详情页面需要的数据 - 基于问卷类型生成相关数据
          familyData: generateFamilyData(item.form_type, parsedRawData, index),
          healthBehaviorData: generateHealthBehaviorData(item.form_type, parsedRawData, index),
          questionnaireData: generateQuestionnaireData(item.form_type, parsedRawData, index)
        };
      });
    } 
    // 如果没有问卷调查数据但有患者数据，基于患者数据生成调研记录
    else if (Array.isArray(patientsData) && patientsData.length > 0) {
      console.log('基于患者数据生成调研记录，患者数量:', patientsData.length);
      transformedData = patientsData.slice(0, 10).map((patient, index) => {
        console.log(`处理患者${index + 1}:`, patient);
        const investigationDate = new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
        const investigators = ['张调研员', '李调研员', '王调研员', '赵调研员', '陈调研员'];
        const investigator = investigators[index % investigators.length];
        
        // 处理性别显示
        let displayGender = '未知';
        if (patient.gender === 'M') displayGender = '男';
        else if (patient.gender === 'F') displayGender = '女';
        else if (patient.gender) displayGender = patient.gender;
        
        // 处理年龄
        let displayAge = patient.age;
        if (!displayAge || displayAge <= 0) {
          displayAge = Math.floor(Math.random() * 50) + 20;
        }
        
        // 处理联系方式
        let displayContact = patient.phone || `138****${(1000 + index).toString().slice(-4)}`;
        
        const record = {
          id: `Q${patient.id || (index + 1).toString().padStart(3, '0')}`,
          patientId: patient.id || `P${(index + 1).toString().padStart(3, '0')}`,
          collectorName: patient.name || `患者${index + 1}`,
          gender: displayGender,
          age: displayAge,
          contact: displayContact,
          investigationDate: investigationDate,
          investigator: investigator,
          remark: `基于患者${patient.name || '未知'}的健康调研`,
          // 生成完整的调研数据
          familyData: {
            housing_type: ['公寓', '独栋', '别墅', '其他'][index % 4],
            family_size: Math.floor(Math.random() * 4) + 2,
            income_level: ['较低', '中等', '较高', '高'][index % 4],
            pet_ownership: index % 3 === 0 ? '有猫' : index % 3 === 1 ? '有狗' : '无',
            smoking_status: index % 5 === 0 ? '有' : '无',
            air_quality: ['优秀', '良好', '中等', '较差'][index % 4]
          },
          healthBehaviorData: {
            exercise_frequency: ['每天', '每周3-4次', '每周1-2次', '很少'][index % 4],
            diet_pattern: ['均衡饮食', '素食为主', '肉食为主', '不规律'][index % 4],
            sleep_hours: Math.floor(Math.random() * 3) + 7,
            stress_level: ['很低', '较低', '中等', '较高', '很高'][index % 5],
            medical_checkup: ['每半年一次', '每年一次', '两年一次', '很少'][index % 4]
          },
          questionnaireData: {
            health_score: Math.floor(Math.random() * 30) + 70,
            risk_factors: [
              ['工作压力', '睡眠不足'],
              ['久坐', '缺乏运动'],
              ['饮食不规律', '吸烟'],
              ['环境污染', '遗传因素']
            ][index % 4],
            recommendations: [
              ['保持运动', '规律作息'],
              ['增加户外活动', '定期体检'],
              ['改善饮食', '减少压力'],
              ['加强锻炼', '避免污染']
            ][index % 4]
          }
        };
        
        console.log(`生成的调研记录${index + 1}:`, record);
        return record;
      });
    }
    
    // 如果都没有数据，使用默认模拟数据
    if (transformedData.length === 0) {
      console.log('没有可用数据，使用默认模拟数据');
      throw new Error('没有可用的数据源');
    }

    // 应用筛选条件
    let filteredData = [...transformedData];
    
    if (filterForm.investigationId) {
      filteredData = filteredData.filter(item => 
        item.id.toLowerCase().includes(filterForm.investigationId.toLowerCase())
      );
    }
    
    if (filterForm.patientId) {
      filteredData = filteredData.filter(item => 
        item.patientId && item.patientId.toLowerCase().includes(filterForm.patientId.toLowerCase())
      );
    }
    
    if (filterForm.collectorName) {
      filteredData = filteredData.filter(item => 
        item.collectorName.includes(filterForm.collectorName)
      );
    }
    
    if (filterForm.dateRange && filterForm.dateRange.length === 2) {
      const [startDate, endDate] = filterForm.dateRange;
      filteredData = filteredData.filter(item => {
        const itemDate = new Date(item.investigationDate);
        return itemDate >= new Date(startDate) && itemDate <= new Date(endDate);
      });
    }

    // 分页处理
    pagination.total = filteredData.length;
    const startIndex = (pagination.current - 1) * pagination.size;
    const endIndex = startIndex + pagination.size;
    investigationData.value = filteredData.slice(startIndex, endIndex);
    
    console.log('最终显示数据:', investigationData.value);
    
    if (filteredData.length === 0) {
      ElMessage.info('根据筛选条件未找到匹配的数据');
    }

  } catch (error) {
    console.error('获取调研数据失败:', error);
    ElMessage.warning('获取数据失败，使用默认模拟数据');
    
    // 如果API失败，使用模拟数据
    console.log('生成默认模拟数据');
    const mockData = [
      {
        id: 'Q001',
        patientId: 'P001',
        collectorName: '张三',
        gender: '男',
        age: 35,
        contact: '138****5678',
        investigationDate: new Date().toISOString().split('T')[0],
        investigator: '李调研员',
        remark: '健康状况良好',
        // 详情页面完整数据
        familyData: {
          housing_type: '公寓',
          family_size: 4,
          income_level: '中等',
          pet_ownership: '无',
          smoking_status: '无',
          air_quality: '良好'
        },
        healthBehaviorData: {
          exercise_frequency: '每周3-4次',
          diet_pattern: '均衡饮食',
          sleep_hours: 7,
          stress_level: '中等',
          medical_checkup: '每年一次'
        },
        questionnaireData: {
          health_score: 85,
          risk_factors: ['工作压力'],
          recommendations: ['保持运动', '规律作息']
        }
      },
      {
        id: 'Q002',
        patientId: 'P002',
        collectorName: '李四',
        gender: '女',
        age: 28,
        contact: '139****9876',
        investigationDate: new Date(Date.now() - 86400000).toISOString().split('T')[0],
        investigator: '赵调研员',
        remark: '定期体检正常',
        // 详情页面完整数据
        familyData: {
          housing_type: '独栋',
          family_size: 2,
          income_level: '较高',
          pet_ownership: '有猫',
          smoking_status: '无',
          air_quality: '优秀'
        },
        healthBehaviorData: {
          exercise_frequency: '每天',
          diet_pattern: '素食为主',
          sleep_hours: 8,
          stress_level: '较低',
          medical_checkup: '每半年一次'
        },
        questionnaireData: {
          health_score: 92,
          risk_factors: ['久坐'],
          recommendations: ['增加户外活动', '定期体检']
        }
      },
      {
        id: 'Q003',
        patientId: 'P003',
        collectorName: '王五',
        gender: '男',
        age: 42,
        contact: '137****1234',
        investigationDate: new Date(Date.now() - 2 * 86400000).toISOString().split('T')[0],
        investigator: '陈调研员',
        remark: '慢性病监测',
        familyData: {
          housing_type: '别墅',
          family_size: 5,
          income_level: '高',
          pet_ownership: '有狗',
          smoking_status: '已戒烟',
          air_quality: '良好'
        },
        healthBehaviorData: {
          exercise_frequency: '每周1-2次',
          diet_pattern: '肉食为主',
          sleep_hours: 6,
          stress_level: '较高',
          medical_checkup: '每年一次'
        },
        questionnaireData: {
          health_score: 78,
          risk_factors: ['工作压力', '睡眠不足'],
          recommendations: ['增加运动', '改善睡眠', '定期体检']
        }
      }
    ];
    
    // 对模拟数据也实现分页
    pagination.total = mockData.length;
    const startIndex = (pagination.current - 1) * pagination.size;
    const endIndex = startIndex + pagination.size;
    investigationData.value = mockData.slice(startIndex, endIndex);
  } finally {
    loading.value = false;
  }
}

// 搜索数据
const searchData = () => {
  pagination.current = 1  // 搜索时重置到第一页
  fetchInvestigationData()
}

// 显示新增对话框
const showAddDialog = () => {
  dialogTitle.value = '新增调研记录'
  formData.value = {
    id: null,
    patientId: '',
    collectorName: '',
    gender: '',
    age: null,
    contact: '',
    investigationDate: '',
    investigator: '',
    remark: ''
  }
  dialogVisible.value = true
}

// 查看详情
const viewDetail = async (row) => {
  try {
    console.log('查看详情，行数据:', row);
    
    // 如果行数据已经包含完整的详情数据，直接使用
    if (row.familyData && row.healthBehaviorData && row.questionnaireData) {
      currentDetail.value = { ...row };
      detailVisible.value = true;
      return;
    }
    
    // 否则调用API获取详细数据
    loading.value = true;
    const response = await fetch(`http://localhost:5000/api/QuestionnaireData/${row.id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      }
    });
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const detailData = await response.json();
    console.log('获取到的详细数据:', detailData);
    
    // 解析raw_data字段
    let parsedRawData = {};
    try {
      if (detailData.raw_data) {
        parsedRawData = JSON.parse(detailData.raw_data);
      }
    } catch (e) {
      console.warn('解析详细数据的raw_data失败:', e);
      parsedRawData = {};
    }
    
    // 合并数据 - 优先使用row中的数据，因为它包含了生成的完整数据
    currentDetail.value = {
      ...row,
      ...parsedRawData,
      // 确保有详情页面需要的数据结构 - 优先使用row中的数据
      familyData: row.familyData || parsedRawData.familyData || {
        residence_type: '1',
        building_age: 10,
        ventilation_quality: '良好',
        indoor_pm25: 25,
        pet_exposure: false,
        pet_type: '无',
        bedding_material: '棉质',
        record_date: new Date().toISOString(),
        household_id: `HH${Date.now()}`,
        patient_id: row.patientId,
        investigator_id: 'INV001'
      },
      healthBehaviorData: row.healthBehaviorData || parsedRawData.healthBehaviorData || {
        individual_id: `IHB${Date.now()}`,
        patient_id: row.patientId,
        household_id: `HH${Date.now()}`,
        diet_pattern: '均衡饮食',
        vitamin_d_level: 25.0,
        sun_exposure: true,
        vaccination_status: true,
        antibiotic_usage_frequency: '很少',
        early_life_medication: '无特殊用药记录',
        smoke_exposure: false,
        investigator_id: 'INV001'
      },
      questionnaireData: row.questionnaireData || parsedRawData.questionnaireData || {
        questionnaire_id: `Q${Date.now()}`,
        patient_id: row.patientId,
        form_type: '综合健康调研',
        fill_date: new Date().toISOString().split('T')[0],
        data_source: '线上调研平台',
        raw_data: JSON.stringify({
          health_score: 85,
          risk_factors: ['工作压力', '睡眠不足'],
          recommendations: ['规律作息', '适度运动']
        }),
        investigator_id: 'INV001',
        create_time: new Date().toISOString()
      }
    };
    
    console.log('最终详情数据:', currentDetail.value);
    detailVisible.value = true;
    
  } catch (error) {
    console.error('获取详情数据失败:', error);
    ElMessage.error('获取详情数据失败: ' + error.message);
    
    // 即使API失败，也要显示基本信息和默认数据
    currentDetail.value = {
      ...row,
              familyData: row.familyData || {
        residence_type: '1',
        building_age: 10,
        ventilation_quality: '良好',
        indoor_pm25: 25,
        pet_exposure: false,
        pet_type: '无',
        bedding_material: '棉质',
        record_date: new Date().toISOString(),
        household_id: `HH${Date.now()}`,
        patient_id: row.patientId,
        investigator_id: 'INV001'
      },
              healthBehaviorData: row.healthBehaviorData || {
        individual_id: `IHB${Date.now()}`,
        patient_id: row.patientId,
        household_id: `HH${Date.now()}`,
        diet_pattern: '均衡饮食',
        vitamin_d_level: 25.0,
        sun_exposure: true,
        vaccination_status: true,
        antibiotic_usage_frequency: '很少',
        early_life_medication: '无特殊用药记录',
        smoke_exposure: false,
        investigator_id: 'INV001'
      },
      questionnaireData: row.questionnaireData || {
        questionnaire_id: `Q${Date.now()}`,
        patient_id: row.patientId,
        form_type: '综合健康调研',
        fill_date: new Date().toISOString().split('T')[0],
        data_source: '线上调研平台',
        raw_data: JSON.stringify({
          health_score: 85,
          risk_factors: ['工作压力', '睡眠不足'],
          recommendations: ['规律作息', '适度运动']
        }),
        investigator_id: 'INV001',
        create_time: new Date().toISOString()
      }
    };
    detailVisible.value = true;
  } finally {
    loading.value = false;
  }
}

// 智能生成调研数据
const generateIntelligentSurveyData = (formData) => {
  const age = formData.age || 0;
  const gender = formData.gender || 'male';
  
  // 家庭环境数据
  const familyData = {
    housing_type: age < 30 ? '公寓' : age < 50 ? '独栋' : '别墅',
    family_size: age < 25 ? 3 : age < 40 ? 4 : age < 60 ? 3 : 2,
    income_level: age < 30 ? '中等' : age < 50 ? '较高' : '高',
    pet_ownership: Math.random() > 0.6 ? '有猫' : Math.random() > 0.3 ? '有狗' : '无',
    smoking_status: age > 40 && Math.random() > 0.7 ? '已戒烟' : '无',
    air_quality: ['优秀', '良好', '中等'][Math.floor(Math.random() * 3)]
  };
  
  // 健康行为数据
  const healthBehaviorData = {
    exercise_frequency: age < 30 ? '每天' : age < 50 ? '每周3-4次' : '每周1-2次',
    diet_pattern: gender === 'female' ? '均衡饮食' : '肉食为主',
    sleep_hours: age < 25 ? 8 : age < 45 ? 7 : 6,
    stress_level: age < 30 ? '较低' : age < 50 ? '中等' : '较高',
    medical_checkup: age < 30 ? '两年一次' : age < 60 ? '每年一次' : '每半年一次'
  };
  
  // 问卷调查数据
  const baseScore = age < 30 ? 90 : age < 50 ? 85 : 75;
  const healthScore = baseScore + Math.floor(Math.random() * 10) - 5;
  
  let riskFactors = [];
  let recommendations = [];
  
  if (age < 30) {
    riskFactors = ['学习压力', '生活不规律'];
    recommendations = ['保持运动', '规律作息', '合理饮食'];
  } else if (age < 50) {
    riskFactors = ['工作压力', '久坐不动'];
    recommendations = ['适度运动', '心理调节', '定期体检'];
  } else {
    riskFactors = ['慢性疾病风险', '体力下降'];
    recommendations = ['定期体检', '适量运动', '营养补充'];
  }
  
  const questionnaireData = {
    health_score: healthScore,
    risk_factors: riskFactors,
    recommendations: recommendations
  };
  
  return { familyData, healthBehaviorData, questionnaireData };
};

// 提交表单
const submitForm = async () => {
  try {
    // 表单验证
    if (investigationForm.value) {
      const valid = await investigationForm.value.validate().catch(() => false);
      if (!valid) return;
    }
    
    console.log('开始提交调研数据，表单数据:', formData.value);
    
    // 生成唯一的问卷ID和相关ID
    const timestamp = new Date().getTime();
    const randomSuffix = Math.floor(Math.random() * 1000).toString().padStart(3, '0');
    const questionnaireId = `Q${timestamp}${randomSuffix}`;
    const collectorId = `COL${timestamp}`;
    const investigatorId = `INV${timestamp}`;
    
    // 智能生成调研数据
    const generatedData = generateIntelligentSurveyData(formData.value);
    console.log('生成的智能调研数据:', generatedData);
    
    // 构建完整的raw_data
    const rawDataObject = {
      // 基本信息
      id: questionnaireId,
      patientId: formData.value.patientId,
      collectorId: collectorId,
      collectorName: formData.value.collectorName,
      gender: formData.value.gender,
      age: formData.value.age,
      contact: formData.value.contact,
      investigationDate: formData.value.investigationDate,
      investigatorId: investigatorId,
      investigator: formData.value.investigator,
      remark: formData.value.remark,
      
      // 四个子模块的详细数据
      familyData: generatedData.familyData,
      healthBehaviorData: generatedData.healthBehaviorData,
      questionnaireData: generatedData.questionnaireData,
      
      // 添加一些问卷得分数据（模拟原有格式）
      q1: Math.floor(Math.random() * 5) + 1,
      q2: Math.floor(Math.random() * 5) + 1,
      q3: Math.floor(Math.random() * 5) + 1,
      total_score: generatedData.questionnaireData.health_score,
      
      // 附加信息
      reportNo: `RPT${timestamp}`,
      hospitalName: '第一人民医院',
      reviewer: '审核员' + ['A', 'B', 'C'][Math.floor(Math.random() * 3)],
      reportDate: new Date().toISOString().split('T')[0]
    };
    
    console.log('构建的完整raw_data:', rawDataObject);
    
    // 构建API请求数据
    const apiData = {
      questionnaire_id: questionnaireId,
      patient_id: formData.value.patientId,
      form_type: '综合健康调研',
      fill_date: formData.value.investigationDate || new Date().toISOString().split('T')[0],
      data_source: '线上调研平台',
      investigator_id: investigatorId,
      raw_data: JSON.stringify(rawDataObject)
    };
    
    console.log('API请求数据:', apiData);
    
    // 调用后端API添加问卷调查数据
    const response = await fetch('http://localhost:5000/api/QuestionnaireData', {
      method: 'POST',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(apiData)
    });

    console.log('API响应状态:', response.status);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error('API响应错误:', errorText);
      throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
    }

    const result = await response.json();
    console.log('API响应结果:', result);
    
    if (result.questionnaire_id || response.ok) {
      ElMessage.success('调研数据保存成功！已生成完整的调研记录');
      dialogVisible.value = false;
      
      // 重新获取数据以显示新插入的记录
      await fetchInvestigationData();
    } else {
      ElMessage.error('保存失败，请检查数据格式');
    }
  } catch (error) {
    console.error('提交调研数据失败:', error);
    ElMessage.error('保存操作失败: ' + error.message);
  }
}

// 删除记录
const deleteRecord = async (id) => {
  try {
    await ElMessageBox.confirm('确定要删除这条调研记录吗?', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });
    
    // 调用后端API删除问卷调查数据
    const response = await fetch(`http://localhost:5000/api/QuestionnaireData/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    });

    if (response.ok) {
      ElMessage.success('删除成功');
      fetchInvestigationData(); // 重新获取数据
    } else {
      const errorText = await response.text().catch(() => '未知错误');
      ElMessage.error(`删除失败: ${errorText}`);
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除操作失败:', error);
      ElMessage.error('删除失败: ' + error.message);
    } else {
      ElMessage.info('已取消删除');
    }
  }
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
      { key: 'id', title: '调研ID' },
      { key: 'patientId', title: '患者ID' },
      { key: 'collectorName', title: '采集者姓名' },
      { key: 'gender', title: '性别' },
      { key: 'age', title: '年龄' },
      { key: 'contact', title: '联系方式' },
      { key: 'investigationDate', title: '调研日期' },
      { key: 'investigator', title: '调研员' }
    ];

    const header = columns.map(col => col.title).join(',');
    
    const rows = investigationData.value.map(row => {
      return columns.map(col => {
        return `"${row[col.key] || ''}"`;
      }).join(',');
    });

    const csvContent = [header, ...rows].join('\n');
    const blob = new Blob(['\uFEFF' + csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.setAttribute('download', '调研数据.csv');
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

// 打印报告
const printReport = () => {
  const element = reportDetailRef.value;
  if (!element) {
    ElMessage.error('无法找到报告内容');
    return;
  }

  const opt = {
    margin:       10,
    filename:     `调研报告-${currentDetail.value.collectorName}-${currentDetail.value.reportNo}.pdf`,
    image:        { type: 'jpeg', quality: 0.98 },
    html2canvas:  { scale: 2, useCORS: true },
    jsPDF:        { unit: 'mm', format: 'a4', orientation: 'portrait' }
  };

  html2pdf().from(element).set(opt).save().then(() => {
    ElMessage.success('报告已生成，请检查下载');
    detailVisible.value = false;
  }).catch((err) => {
    ElMessage.error('生成PDF失败: ' + err.message);
  });
}

// 分页大小改变
const handleSizeChange = (size) => {
  pagination.size = size
  fetchInvestigationData()
}

// 页码改变
const handleCurrentChange = (current) => {
  pagination.current = current
  fetchInvestigationData()
}



// 初始化加载数据
onMounted(async () => {
  // 先检查用户权限
  await checkUserPermission()
  
  // 然后加载数据
  fetchInvestigationData()
})
</script>

<style scoped>
.investigation-container {
  padding: 20px;
  background-color: #f5f7fa;
  padding-bottom: 80px; /* 增加底部边距 */
}

.investigation-card {
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
  max-width: 100%;
  margin: 0 auto;
  padding: 10px;
}

.report-header {
  text-align: center;
  margin-bottom: 25px;
  padding-bottom: 15px;
  border-bottom: 2px solid #f0f0f0;
}

.report-header h2 {
  margin: 0;
  color: #333;
  font-size: 22px;
  font-weight: bold;
}

.report-no {
  margin-top: 8px;
  color: #666;
  font-size: 14px;
}

.collector-info {
  margin-bottom: 25px;
  background-color: #f9f9f9;
  border-radius: 4px;
  padding: 10px;
}

.detail-tabs {
  margin-top: 25px;
  margin-bottom: 25px;
}

:deep(.detail-tabs .el-tabs__header) {
  margin-bottom: 15px;
}

.report-footer {
  margin-top: 30px;
  display: flex;
  justify-content: space-between;
  padding-top: 15px;
  border-top: 2px solid #f0f0f0;
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

:deep(.el-descriptions__body) {
  background-color: #f9f9f9;
}

:deep(.el-descriptions__title) {
  font-weight: bold;
}

.report-detail-dialog {
  display: flex;
  align-items: center;
  justify-content: center;
}

:deep(.report-detail-dialog .el-dialog) {
  margin: 0 auto !important;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

:deep(.report-detail-dialog .el-dialog__body) {
  overflow-y: auto;
  padding: 20px 25px;
}

:deep(.report-detail-dialog .el-dialog__header) {
  padding: 15px 25px;
  margin: 0;
  border-bottom: 1px solid #f0f0f0;
}

:deep(.report-detail-dialog .el-dialog__footer) {
  border-top: 1px solid #f0f0f0;
  padding: 15px 25px;
}

.permission-notice {
  margin-top: 10px;
}
</style>