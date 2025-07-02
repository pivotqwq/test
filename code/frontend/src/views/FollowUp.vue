<template>
  <div class="followup-container">
    <el-card class="followup-card">
      <template #header>
        <div class="card-header">
          <span>患者随访信息</span>
          <div>
            <el-button type="primary" @click="showAddDialog">
              <el-icon><Plus /></el-icon> 新增随访记录
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
          <el-form-item label="患者姓名">
            <el-input v-model="filterForm.patientName" placeholder="输入患者姓名" clearable style="width: 200px;" />
          </el-form-item>
          <el-form-item label="随访日期">
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
          <el-form-item label="症状改善">
            <el-select v-model="filterForm.improvement" placeholder="选择症状改善情况" clearable style="width: 200px;">
              <el-option label="显著改善" value="significant" />
              <el-option label="部分改善" value="partial" />
              <el-option label="无改善" value="none" />
              <el-option label="恶化" value="worse" />
            </el-select>
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
            description="只有管理员用户才能删除随访记录，如需删除权限请联系系统管理员"
            :closable="false"
            show-icon
          />
        </div>
      </div>

      <!-- 数据表格 -->
      <el-table 
        :data="followupData" 
        style="width: 100%" 
        v-loading="loading"
        stripe
        border
        highlight-current-row
      >
        <el-table-column prop="followupId" label="随访ID" align="center" :resizable="false" />
        <el-table-column prop="patientId" label="患者ID" align="center" :resizable="false" />
        <el-table-column prop="patientName" label="患者姓名" align="center" :resizable="false" />
        <el-table-column prop="staffId" label="医务人员ID" align="center" :resizable="false" />
        <el-table-column prop="followupDate" label="随访日期" align="center" :resizable="false" />
        <el-table-column prop="improvement" label="症状改善" align="center" :resizable="false">
          <template #default="scope">
            <el-tag :type="getImprovementTag(scope.row.improvement)">
              {{ getImprovementLabel(scope.row.improvement) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right" align="center" :resizable="false">
          <template #default="scope">
            <el-button size="small" @click="viewDetail(scope.row)">
              <el-icon><View /></el-icon> 详情
            </el-button>
            <el-button 
              size="small" 
              type="danger" 
              @click="deleteRecord(scope.row.followupId)"
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
          background
        />
      </div>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog 
      v-model="dialogVisible" 
      :title="dialogTitle" 
      width="50%"
      :close-on-click-modal="false"
    >
      <el-form 
        :model="formData" 
        :rules="rules" 
        ref="followupForm"
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
            <el-form-item label="医务人员ID" prop="staffId">
              <el-input v-model="formData.staffId" placeholder="输入医务人员ID" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="随访日期" prop="followupDate">
              <el-date-picker
                v-model="formData.followupDate"
                type="date"
                placeholder="选择随访日期"
                style="width: 100%"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="症状改善" prop="improvement">
          <el-select 
            v-model="formData.improvement" 
            placeholder="选择症状改善情况" 
            style="width: 100%"
          >
            <el-option label="显著改善" value="significant" />
            <el-option label="部分改善" value="partial" />
            <el-option label="无改善" value="none" />
            <el-option label="恶化" value="worse" />
          </el-select>
        </el-form-item>

        <el-form-item label="备注" prop="remark">
          <el-input 
            v-model="formData.remark" 
            type="textarea" 
            :rows="3" 
            placeholder="可输入随访详细情况说明等" 
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
      title="随访详细信息" 
      width="70%"
      top="5vh"
      class="detail-dialog"
      destroy-on-close
    >
      <div ref="detailContentRef" v-if="currentDetail && currentDetail.details">
        <el-tabs type="border-card">
          <el-tab-pane label="患者综合信息">
            <el-descriptions title="患者基本信息" :column="2" border class="detail-block">
              <el-descriptions-item label="患者ID">{{ currentDetail.details.patientInfo.patientId || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="门诊号">{{ currentDetail.details.patientInfo.outpatientNo || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="姓名">{{ currentDetail.details.patientInfo.name || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="性别">{{ currentDetail.details.patientInfo.gender || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="出生日期">{{ currentDetail.details.patientInfo.dob || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="身高(cm)">{{ currentDetail.details.patientInfo.height || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="体重(kg)">{{ currentDetail.details.patientInfo.weight || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="就诊类型">{{ currentDetail.details.patientInfo.visitType || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="家庭住址" :span="2">{{ currentDetail.details.patientInfo.address || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="创建时间">{{ currentDetail.details.patientInfo.createTime || '无数据' }}</el-descriptions-item>
            </el-descriptions>
            
            <el-divider />

            <el-descriptions title="医患关系信息" :column="2" border class="detail-block">
              <el-descriptions-item label="关系ID">{{ currentDetail.details.doctorPatientRelationship.relationshipId || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="医务人员ID">{{ currentDetail.details.doctorPatientRelationship.staffId || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="关系类型">{{ currentDetail.details.doctorPatientRelationship.relationshipType || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="开始日期">{{ currentDetail.details.doctorPatientRelationship.startDate || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="结束日期">{{ currentDetail.details.doctorPatientRelationship.endDate || '无数据' }}</el-descriptions-item>
            </el-descriptions>
          </el-tab-pane>

          <el-tab-pane label="体征与用药记录">
            <el-descriptions title="体征检查" :column="2" border class="detail-block">
              <el-descriptions-item label="检查ID">{{ currentDetail.details.physicalExam.examId || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="检查日期">{{ currentDetail.details.physicalExam.examDate || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="体温(℃)">{{ currentDetail.details.physicalExam.temperature || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="脉搏(次/分)">{{ currentDetail.details.physicalExam.pulse || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="血氧饱和度(%)">{{ currentDetail.details.physicalExam.spo2 || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="肺部听诊结果">{{ currentDetail.details.physicalExam.lungAuscultation || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="皮疹描述" :span="2">{{ currentDetail.details.physicalExam.rashDescription || '无数据' }}</el-descriptions-item>
            </el-descriptions>

            <el-divider />

            <el-descriptions title="用药记录" :column="2" border class="detail-block">
              <el-descriptions-item label="用药ID">{{ currentDetail.details.medicationRecord.medicationId || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="药物名称">{{ currentDetail.details.medicationRecord.drugName || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="剂量">{{ currentDetail.details.medicationRecord.dosage || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="用药频率">{{ currentDetail.details.medicationRecord.frequency || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="开始日期">{{ currentDetail.details.medicationRecord.startDate || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="结束日期">{{ currentDetail.details.medicationRecord.endDate || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="药物类别">{{ currentDetail.details.medicationRecord.drugCategory || '无数据' }}</el-descriptions-item>
            </el-descriptions>
          </el-tab-pane>

          <el-tab-pane label="随访与费用">
            <el-descriptions title="随访记录" :column="2" border class="detail-block">
              <el-descriptions-item label="随访ID">{{ currentDetail.details.followUpRecord.followupId || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="随访日期">{{ currentDetail.details.followUpRecord.followupDate || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="症状改善">{{ getImprovementLabel(currentDetail.details.followUpRecord.improvement) }}</el-descriptions-item>
              <el-descriptions-item label="药物不良反应">{{ currentDetail.details.followUpRecord.adverseReaction || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="ACT评分(哮喘控制)">{{ currentDetail.details.followUpRecord.actScore || '无数据' }}</el-descriptions-item>
            </el-descriptions>

            <el-divider />

            <el-descriptions title="医疗费用" :column="2" border class="detail-block">
              <el-descriptions-item label="费用ID">{{ currentDetail.details.medicalCost.costId || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="费用类型">{{ currentDetail.details.medicalCost.costType || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="金额">{{ currentDetail.details.medicalCost.amount || '无数据' }}</el-descriptions-item>
              <el-descriptions-item label="费用日期">{{ currentDetail.details.medicalCost.costDate || '无数据' }}</el-descriptions-item>
            </el-descriptions>
          </el-tab-pane>
        </el-tabs>
      </div>
      <el-empty v-else description="暂无详细随访数据" />
      <template #footer>
        <el-button @click="detailVisible = false">关闭</el-button>
        <el-button type="primary" @click="printReport" :loading="printing">
          <el-icon><Printer /></el-icon> 打印报告
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox, ElLoading } from 'element-plus'
import { Plus, Download, View, Delete, Printer } from '@element-plus/icons-vue'
import html2pdf from 'html2pdf.js'

// 状态管理
const loading = ref(false)
const followupData = ref([])
const dialogVisible = ref(false)
const detailVisible = ref(false)
const printing = ref(false) // 打印状态
const dialogTitle = ref('新增随访记录')
const followupForm = ref(null) // 表单引用
const currentDetail = ref(null)
const detailContentRef = ref(null) // 详情内容引用
const userRole = ref('user') // 角色从API动态获取
const formData = ref({
  followupId: '',
  patientId: '',
  patientName: '',
  staffId: '',
  followupDate: '',
  improvement: '',
  remark: '',
  details: {
    patientInfo: {
      patientId: '',
      outpatientNo: '',
      name: '',
      gender: '',
      dob: '',
      address: '',
      height: '',
      weight: '',
      visitType: '',
      createTime: ''
    },
    doctorPatientRelationship: {
      relationshipId: '',
      staffId: '',
      relationshipType: '',
      startDate: '',
      endDate: ''
    },
    physicalExam: {
      examId: '',
      examDate: '',
      temperature: '',
      pulse: '',
      spo2: '',
      lungAuscultation: '',
      rashDescription: ''
    },
    medicationRecord: {
      medicationId: '',
      drugName: '',
      dosage: '',
      frequency: '',
      startDate: '',
      endDate: '',
      drugCategory: ''
    },
    followUpRecord: {
      followupId: '',
      followupDate: '',
      improvement: '',
      adverseReaction: '',
      actScore: ''
    },
    medicalCost: {
      costId: '',
      costType: '',
      amount: '',
      costDate: ''
    }
  }
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
  patientName: '',
  dateRange: [],
  improvement: ''
})

// 表单验证规则
const rules = {
  patientId: [{ required: true, message: '请输入患者ID', trigger: 'blur' }],
  patientName: [{ required: true, message: '请输入患者姓名', trigger: 'blur' }],
  staffId: [{ required: true, message: '请输入医务人员ID', trigger: 'blur' }],
  followupDate: [{ required: true, message: '请选择随访日期', trigger: 'change' }],
  improvement: [{ required: true, message: '请选择症状改善情况', trigger: 'change' }]
}

// 获取症状改善标签样式
const getImprovementTag = (improvement) => {
  const tags = {
    'significant': 'success',
    'partial': 'primary',
    'none': 'warning',
    'worse': 'danger'
  }
  return tags[improvement] || 'info'
}

// 获取症状改善中文标签
const getImprovementLabel = (improvement) => {
  const labels = {
    'significant': '显著改善',
    'partial': '部分改善',
    'none': '无改善',
    'worse': '恶化'
  }
  return labels[improvement] || improvement
}

// 映射症状改善文本到枚举值
const mapImprovementText = (text) => {
  if (!text) return 'none';
  const textLower = text.toLowerCase();
  if (textLower.includes('明显改善') || textLower.includes('显著改善') || textLower.includes('significant')) {
    return 'significant';
  } else if (textLower.includes('部分改善') || textLower.includes('有所改善') || textLower.includes('partial')) {
    return 'partial';
  } else if (textLower.includes('恶化') || textLower.includes('worse')) {
    return 'worse';
  } else {
    return 'none';
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

// 获取随访数据
const fetchFollowupData = async () => {
  loading.value = true
  try {
    console.log('开始获取随访数据...')
    
    // 获取所有随访记录
    const response = await fetch('http://localhost:5000/api/FollowUpRecord', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    });

    console.log('API响应状态:', response.status)

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const followupDataResponse = await response.json();
    console.log('获取到的原始数据:', followupDataResponse)
    
    if (!Array.isArray(followupDataResponse)) {
      throw new Error('API返回数据格式不正确');
    }

    // 转换数据格式以匹配前端显示需求
    const transformedData = followupDataResponse.map((item, index) => ({
      followupId: item.followup_id || `FU${index + 1}`,
      patientId: item.patient_id || 'P000',
      patientName: item.patient_name || `患者${item.patient_id}` || `患者${index + 1}`,
      staffId: 'DOC001',
      followupDate: item.followup_date ? new Date(item.followup_date).toISOString().split('T')[0] : new Date().toISOString().split('T')[0],
      improvement: mapImprovementText(item.symptom_improvement),
              details: {
          patientInfo: {
            patientId: item.patient_id || 'P000',
            outpatientNo: `OP-${item.patient_id}`,
            name: item.patient_name || `患者${item.patient_id}`,
            gender: '未知',
            dob: '未知',
            address: '未知',
            height: 0,
            weight: 0,
            visitType: '复诊',
            createTime: new Date().toISOString()
          },
        doctorPatientRelationship: {
          relationshipId: `DP-${item.patient_id}`,
          staffId: 'DOC001',
          relationshipType: '主治',
          startDate: new Date().toISOString().split('T')[0],
          endDate: null
        },
        physicalExam: {
          examId: `PE-${item.followup_id}`,
          examDate: item.followup_date ? new Date(item.followup_date).toISOString().split('T')[0] : new Date().toISOString().split('T')[0],
          temperature: 36.5,
          pulse: 72,
          spo2: 98,
          lungAuscultation: '正常',
          rashDescription: '无皮疹'
        },
        medicationRecord: {
          medicationId: `MED-${item.patient_id}`,
          drugName: '未知药物',
          dosage: '未知',
          frequency: '未知',
          startDate: new Date().toISOString().split('T')[0],
          endDate: null,
          drugCategory: '未知'
        },
        followUpRecord: {
          followupId: item.followup_id,
          followupDate: item.followup_date ? new Date(item.followup_date).toISOString().split('T')[0] : new Date().toISOString().split('T')[0],
          improvement: item.symptom_improvement || '无记录',
          adverseReaction: item.adverse_effects || '无',
          actScore: item.act_score || 0
        },
        medicalCost: {
          costId: `C-${item.patient_id}`,
          costType: '随访费',
          amount: '50.00元',
          costDate: item.followup_date ? new Date(item.followup_date).toISOString().split('T')[0] : new Date().toISOString().split('T')[0]
        }
      }
    }));
    
    console.log('转换后的数据:', transformedData)
    
    // 应用筛选
    let filteredData = [...transformedData];
    if (filterForm.patientId) {
      filteredData = filteredData.filter(item => 
        item.patientId.includes(filterForm.patientId)
      );
    }
    if (filterForm.patientName) {
      filteredData = filteredData.filter(item => 
        item.patientName.includes(filterForm.patientName)
      );
    }
    if (filterForm.improvement) {
      filteredData = filteredData.filter(item => 
        item.improvement === filterForm.improvement
      );
    }
    
    // 设置总数
    pagination.total = filteredData.length;
    
    // 实现前端分页
    const startIndex = (pagination.current - 1) * pagination.size;
    const endIndex = startIndex + pagination.size;
    const pagedData = filteredData.slice(startIndex, endIndex);
    
    followupData.value = pagedData;
    
    console.log('分页信息:', {
      current: pagination.current,
      size: pagination.size,
      total: pagination.total,
      startIndex,
      endIndex,
      pagedData: pagedData.length
    })
    console.log('最终显示数据:', pagedData)
    
  } catch (error) {
    console.error('API Error:', error);
    ElMessage.warning('无法连接到服务器，使用模拟数据显示');
    
    // 如果API失败，使用模拟数据
    const mockData = [
      {
        followupId: 'FU001',
        patientId: 'P001',
        patientName: '患者P001',
        staffId: 'DOC001',
        followupDate: new Date().toISOString().split('T')[0],
        improvement: 'significant',
        details: {
          patientInfo: {
            patientId: 'P001',
            outpatientNo: 'OP-P001',
            name: '患者P001',
            gender: '男',
            dob: '1988-08-08',
            address: '北京市朝阳区',
            height: 175,
            weight: 70,
            visitType: '初诊',
            createTime: '2023-01-15 10:00:00'
          },
          doctorPatientRelationship: {
            relationshipId: 'DP-001',
            staffId: 'DOC001',
            relationshipType: '主治',
            startDate: '2023-01-15',
            endDate: null
          },
          physicalExam: {
            examId: 'PE-001',
            examDate: new Date().toISOString().split('T')[0],
            temperature: 36.8,
            pulse: 78,
            spo2: 99,
            lungAuscultation: '呼吸音清晰',
            rashDescription: '无明显皮疹'
          },
          medicationRecord: {
            medicationId: 'MED-001',
            drugName: '布地奈德福莫特罗粉吸入剂',
            dosage: '160/4.5μg',
            frequency: '每日两次',
            startDate: '2023-01-15',
            endDate: null,
            drugCategory: 'ICS/LABA'
          },
          followUpRecord: {
            followupId: 'FU001',
            followupDate: new Date().toISOString().split('T')[0],
            improvement: 'significant',
            adverseReaction: '无',
            actScore: 25
          },
          medicalCost: {
            costId: 'C-001',
            costType: '药品费',
            amount: '350.00元',
            costDate: new Date().toISOString().split('T')[0]
          }
        }
      },
      {
        followupId: 'FU002',
        patientId: 'P002',
        patientName: '患者P002',
        staffId: 'NUR001',
        followupDate: new Date(Date.now() - 86400000).toISOString().split('T')[0], // 昨天
        improvement: 'partial',
        details: {
          patientInfo: {
            patientId: 'P002',
            outpatientNo: 'OP-P002',
            name: '患者P002',
            gender: '女',
            dob: '1985-05-15',
            address: '上海市浦东新区',
            height: 165,
            weight: 55,
            visitType: '复诊',
            createTime: '2023-01-20 09:30:00'
          },
          doctorPatientRelationship: {
            relationshipId: 'DP-002',
            staffId: 'NUR001',
            relationshipType: '随访护士',
            startDate: '2023-01-20',
            endDate: null
          },
          physicalExam: {
            examId: 'PE-002',
            examDate: new Date(Date.now() - 86400000).toISOString().split('T')[0],
            temperature: 36.5,
            pulse: 72,
            spo2: 98,
            lungAuscultation: '轻微哮鸣音',
            rashDescription: '无皮疹'
          },
          medicationRecord: {
            medicationId: 'MED-002',
            drugName: '孟鲁司特钠片',
            dosage: '10mg',
            frequency: '每晚一次',
            startDate: '2023-01-20',
            endDate: null,
            drugCategory: 'LTRA'
          },
          followUpRecord: {
            followupId: 'FU002',
            followupDate: new Date(Date.now() - 86400000).toISOString().split('T')[0],
            improvement: 'partial',
            adverseReaction: '轻微口干',
            actScore: 20
          },
          medicalCost: {
            costId: 'C-002',
            costType: '药品费',
            amount: '280.00元',
            costDate: new Date(Date.now() - 86400000).toISOString().split('T')[0]
          }
        }
      }
    ];
    
    // 对模拟数据也实现分页
    pagination.total = mockData.length;
    const startIndex = (pagination.current - 1) * pagination.size;
    const endIndex = startIndex + pagination.size;
    followupData.value = mockData.slice(startIndex, endIndex);
  } finally {
    loading.value = false;
  }
}

// 搜索数据
const searchData = () => {
  pagination.current = 1  // 搜索时重置到第一页
  fetchFollowupData()
}

// 显示新增对话框
const showAddDialog = () => {
  dialogTitle.value = '新增随访记录'
  formData.value = {
    followupId: '',
    patientId: '',
    patientName: '',
    staffId: '',
    followupDate: '',
    improvement: '',
    remark: '',
    details: {
      patientInfo: {
        patientId: '',
        outpatientNo: '',
        name: '',
        gender: '',
        dob: '',
        address: '',
        height: '',
        weight: '',
        visitType: '',
        createTime: ''
      },
      doctorPatientRelationship: {
        relationshipId: '',
        staffId: '',
        relationshipType: '',
        startDate: '',
        endDate: ''
      },
      physicalExam: {
        examId: '',
        examDate: '',
        temperature: '',
        pulse: '',
        spo2: '',
        lungAuscultation: '',
        rashDescription: ''
      },
      medicationRecord: {
        medicationId: '',
        drugName: '',
        dosage: '',
        frequency: '',
        startDate: '',
        endDate: '',
        drugCategory: ''
      },
      followUpRecord: {
        followupId: '',
        followupDate: '',
        improvement: '',
        adverseReaction: '',
        actScore: ''
      },
      medicalCost: {
        costId: '',
        costType: '',
        amount: '',
        costDate: ''
      }
    }
  }
  dialogVisible.value = true
}

// 查看详情
const viewDetail = (row) => {
  currentDetail.value = row
  detailVisible.value = true
}

// 提交表单
const submitForm = async () => {
  try {
    // 表单验证 - 使用正确的引用方式
    if (!followupForm.value) return
    
    await followupForm.value.validate(async (valid, fields) => {
      if (!valid) {
        console.error('表单验证失败:', fields)
        return
      }
      
      // 调用后端API添加随访记录
      const response = await fetch('http://localhost:5000/api/FollowUpRecord', {
        method: 'POST',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          patient_id: formData.value.patientId,
          followup_date: new Date(formData.value.followupDate).toISOString(),
          symptom_improvement: formData.value.improvement,
          adverse_effects: formData.value.remark || '',
          act_score: 20 // 设置默认的ACT评分
        })
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result = await response.json();
      
      if (result.followup_id || response.ok) {
        ElMessage.success('保存成功');
        dialogVisible.value = false;
        fetchFollowupData(); // 重新获取数据
      } else {
        ElMessage.error('保存失败');
      }
    });
  } catch (error) {
    console.error('Submit error:', error);
    ElMessage.error('保存失败: ' + error.message);
  }
}

// 删除记录
const deleteRecord = async (id) => {
  try {
    await ElMessageBox.confirm('确定要删除这条随访记录吗?', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });
    
    // 调用后端API删除随访记录
    const response = await fetch(`http://localhost:5000/api/FollowUpRecord/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    });

    if (response.ok) {
      ElMessage.success('删除成功');
      fetchFollowupData(); // 重新获取数据
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
      { key: 'followupId', title: '随访ID' },
      { key: 'patientId', title: '患者ID' },
      { key: 'patientName', title: '患者姓名' },
      { key: 'staffId', title: '医务人员ID' },
      { key: 'followupDate', title: '随访日期' },
      { key: 'improvement', title: '症状改善' }
    ]

    const header = columns.map(col => col.title).join(',')
    
    const rows = followupData.value.map(row => {
      return columns.map(col => {
        if (col.key === 'improvement') {
          return getImprovementLabel(row[col.key])
        }
        return `"${row[col.key] || ''}"`
      }).join(',')
    })

    const csvContent = [header, ...rows].join('\n')
    const blob = new Blob(['\uFEFF' + csvContent], { type: 'text/csv;charset=utf-8;' })
    const link = document.createElement('a')
    link.href = URL.createObjectURL(blob)
    link.setAttribute('download', '随访数据.csv')
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)

    ElMessage.success('导出成功')
  } catch (error) {
    ElMessage.error('导出失败: ' + error.message)
  } finally {
    loadingInstance.close()
  }
}

// 打印报告
const printReport = () => {
  if (!currentDetail.value || !detailContentRef.value) {
    ElMessage.warning('没有可打印的数据。')
    return
  }

  printing.value = true

  const patientName = currentDetail.value.patientName || '未知患者'
  const patientId = currentDetail.value.patientId || '未知ID'
  const filename = `随访报告-${patientName}-${patientId}.pdf`

  const opt = {
    margin: [20, 10, 20, 10], // 上、左、下、右边距
    filename: filename,
    image: { type: 'jpeg', quality: 0.98 },
    html2canvas: { scale: 2, useCORS: true, logging: false },
    jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
  }

  html2pdf().from(detailContentRef.value).set(opt).save().then(() => {
    ElMessage.success('报告已开始下载')
  }).catch((err) => {
    ElMessage.error('打印报告失败: ' + err.message)
    console.error(err)
  }).finally(() => {
    printing.value = false
  })
}

// 分页大小改变
const handleSizeChange = (size) => {
  pagination.size = size
  pagination.current = 1 //  当改变每页条数时，重置到第一页
  fetchFollowupData()
}

// 页码改变
const handleCurrentChange = (current) => {
  pagination.current = current
  fetchFollowupData()
}

// 初始化加载数据
onMounted(async () => {
  console.log('组件已挂载，开始加载数据')
  // 确保分页初始值正确
  pagination.current = 1
  pagination.size = 10
  
  // 先检查用户权限
  await checkUserPermission()
  
  // 然后加载数据
  fetchFollowupData()
})
</script>

<style scoped>
.followup-container {
  padding: 20px;
  background-color: #f5f7fa;
  padding-bottom: 80px; /* 增加底部边距 */
}

.followup-card {
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

:deep(.el-pagination) {
  padding: 0;
  font-weight: normal;
}

:deep(.el-pagination .el-select .el-input) {
  width: 120px;
}

:deep(.el-pagination .el-pagination__jump) {
  margin-left: 10px;
}

.permission-notice {
  margin-top: 10px;
}

.detail-dialog .el-descriptions__title {
  font-size: 16px;
  color: #1a558d;
}

.detail-dialog .detail-block {
  margin-bottom: 20px;
}
</style>