<template>
  <div class="clinical-detail-container" ref="reportContent">
    <el-page-header @back="goBack" class="page-header">
      <template #content>
        <span class="text-large font-600 mr-3"> 临床信息详情 (患者: {{ patientId }}) </span>
      </template>
    </el-page-header>

    <div v-if="clinicalData" class="content-area">
      <el-card class="box-card">
        <template #header>
          <div class="card-header">
            <span>患者基本信息</span>
          </div>
        </template>
        <el-descriptions :column="2" border>
                  <el-descriptions-item label="患者ID">{{ clinicalData.patientInfo.id }}</el-descriptions-item>
        <el-descriptions-item label="病历号">{{ clinicalData.patientInfo.medical_record_no }}</el-descriptions-item>
        <el-descriptions-item label="姓名">{{ clinicalData.patientInfo.name }}</el-descriptions-item>
        <el-descriptions-item label="性别">{{ clinicalData.patientInfo.gender }}</el-descriptions-item>
        <el-descriptions-item label="出生日期">{{ clinicalData.patientInfo.birth_date }}</el-descriptions-item>
          <el-descriptions-item label="地址" :span="2">{{ clinicalData.patientInfo.address }}</el-descriptions-item>
        </el-descriptions>
      </el-card>

      <el-card class="box-card">
        <template #header>
          <div class="card-header">
            <span>医保与联系人</span>
          </div>
        </template>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-descriptions title="医保信息" :column="1" border>
              <el-descriptions-item label="医保ID">{{ clinicalData.insuranceInfo.insurance_id }}</el-descriptions-item>
              <el-descriptions-item label="医保类型">{{ clinicalData.insuranceInfo.insurance_type }}</el-descriptions-item>
            </el-descriptions>
          </el-col>
          <el-col :span="12">
            <el-descriptions title="联系人信息" :column="1" border>
              <el-descriptions-item label="联系人ID">{{ clinicalData.contactInfo.contact_id }}</el-descriptions-item>
              <el-descriptions-item label="姓名">{{ clinicalData.contactInfo.name }}</el-descriptions-item>
              <el-descriptions-item label="联系方式">{{ clinicalData.contactInfo.contact_info }}</el-descriptions-item>
            </el-descriptions>
          </el-col>
        </el-row>
      </el-card>

      <el-card class="box-card">
        <template #header>
          <div class="card-header">
            <span>病史信息</span>
          </div>
        </template>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-descriptions title="既往病史" :column="1" border>
              <el-descriptions-item label="既往史ID">{{ clinicalData.pastMedicalHistory.history_id }}</el-descriptions-item>
              <el-descriptions-item label="过敏史">{{ clinicalData.pastMedicalHistory.allergy_history }}</el-descriptions-item>
            </el-descriptions>
          </el-col>
          <el-col :span="12">
            <el-descriptions title="家族史" :column="1" border>
              <el-descriptions-item label="家族史ID">{{ clinicalData.familyHistory.family_history_id }}</el-descriptions-item>
              <el-descriptions-item label="过敏史">{{ clinicalData.familyHistory.allergy_history }}</el-descriptions-item>
            </el-descriptions>
          </el-col>
        </el-row>
      </el-card>
    </div>
    <el-empty v-else description="未找到该患者的临床信息" />
    <div class="print-button-container">
       <el-button type="primary" @click="printReport">打印报告</el-button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { clinicalDetailsData } from '@/mockdata/clinicalDetails';
import html2pdf from 'html2pdf.js';
import { ElMessage } from 'element-plus';

const route = useRoute();
const router = useRouter();

const patientId = ref(null);
const clinicalData = ref(null);
const reportContent = ref(null);

// 格式化性别显示
const formatGender = (gender) => {
  // 处理后端返回的M/F格式
  if (gender === 'M' || gender === 'm') return '男';
  if (gender === 'F' || gender === 'f') return '女';
  // 处理数字格式（向后兼容）
  const genderMap = { 1: '男', 2: '女', 0: '其他' }
  return genderMap[gender] || '未知'
}

onMounted(async () => {
  patientId.value = route.params.patientId;
  if (patientId.value) {
    await fetchClinicalData(patientId.value);
  }
});

const fetchClinicalData = async (patientId) => {
  try {
    // 获取患者基本信息 - 使用PatientBasicInfo API
    const patientResponse = await fetch(`http://localhost:5000/api/PatientBasicInfo/${patientId}`, {
      method: 'GET',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    });

    if (!patientResponse.ok) {
      throw new Error(`HTTP error! status: ${patientResponse.status}`);
    }

    const patientData = await patientResponse.json();
    
    if (patientData) {
      
      // 获取医保信息
      const insuranceResponse = await fetch(`http://localhost:5000/api/ClinicalData/insuranceInfo?patientId=${patientId}`, {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      });

      // 获取联系人信息
      const contactResponse = await fetch(`http://localhost:5000/api/ClinicalData/contactsInfo?patientId=${patientId}`, {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      });

      // 获取既往病史
      const medicalHistoryResponse = await fetch(`http://localhost:5000/api/ClinicalData/medical-historiesInfo?patientId=${patientId}`, {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      });

      // 获取家族病史
      const familyHistoryResponse = await fetch(`http://localhost:5000/api/ClinicalData/family-historiesInfo?patientId=${patientId}`, {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      });

      // 组装临床数据 - 适配PatientBasicInfo字段结构
      clinicalData.value = {
        patientInfo: {
          id: patientData.patient_id || patientId,
          medical_record_no: patientData.patient_id || patientId,
          name: patientData.name || '未知',
          gender: formatGender(patientData.gender),
          birth_date: patientData.birth_date ? new Date(patientData.birth_date).toISOString().split('T')[0] : '未知',
          address: patientData.residence_type || '未知'
        },
        insuranceInfo: {
          insurance_id: '未知',
          insurance_type: '未知'
        },
        contactInfo: {
          contact_id: '未知',
          name: '未知',
          contact_info: '未知'
        },
        pastMedicalHistory: {
          history_id: '未知',
          allergy_history: '未知'
        },
        familyHistory: {
          family_history_id: '未知',
          allergy_history: '未知'
        }
      };

      // 处理医保信息
      if (insuranceResponse.ok) {
        const insuranceResult = await insuranceResponse.json();
        if (insuranceResult.success && insuranceResult.data) {
          clinicalData.value.insuranceInfo = {
            insurance_id: insuranceResult.data.insurance_id || '未知',
            insurance_type: insuranceResult.data.insurance_type || '未知'
          };
        }
      }

      // 处理联系人信息
      if (contactResponse.ok) {
        const contactResult = await contactResponse.json();
        if (contactResult.success && contactResult.data && contactResult.data.length > 0) {
          // 取第一个联系人
          const firstContact = contactResult.data[0];
          clinicalData.value.contactInfo = {
            contact_id: firstContact.contact_id || '未知',
            name: firstContact.name || '未知',
            contact_info: firstContact.contact_info || '未知'
          };
        }
      }

      // 处理既往病史
      if (medicalHistoryResponse.ok) {
        const medicalHistoryResult = await medicalHistoryResponse.json();
        if (medicalHistoryResult.success && medicalHistoryResult.data) {
          clinicalData.value.pastMedicalHistory = {
            history_id: medicalHistoryResult.data.history_id || '未知',
            allergy_history: medicalHistoryResult.data.allergy_history || '未知'
          };
        }
      }

      // 处理家族病史
      if (familyHistoryResponse.ok) {
        const familyHistoryResult = await familyHistoryResponse.json();
        if (familyHistoryResult.success && familyHistoryResult.data) {
          clinicalData.value.familyHistory = {
            family_history_id: familyHistoryResult.data.family_history_id || '未知',
            allergy_history: familyHistoryResult.data.allergy_history || '未知'
          };
        }
      }
    } else {
      ElMessage.error('获取患者信息失败');
    }
  } catch (error) {
    console.error('API Error:', error);
    ElMessage.error('获取临床数据失败: ' + error.message);
    
    // 如果API失败，使用模拟数据
    if (clinicalDetailsData[patientId]) {
      clinicalData.value = clinicalDetailsData[patientId];
    }
  }
};

const goBack = () => {
  router.back();
};

const printReport = () => {
  if (!reportContent.value) {
    ElMessage.error('无法找到报告内容，打印失败');
    return;
  }
  
  const opt = {
    margin:       10,
    filename:     `临床信息报告-${patientId.value}.pdf`,
    image:        { type: 'jpeg', quality: 0.98 },
    html2canvas:  { scale: 2, useCORS: true },
    jsPDF:        { unit: 'mm', format: 'a4', orientation: 'portrait' }
  };

  html2pdf().from(reportContent.value).set(opt).save().then(() => {
    ElMessage.success('报告已生成，请检查下载');
  }).catch((err) => {
    ElMessage.error('生成PDF失败: ' + err.message);
  });
};
</script>

<style scoped>
.clinical-detail-container {
  padding: 20px;
  background-color: #f5f7fa;
  /* 限制容器高度并启用滚动 */
  height: calc(100vh - 120px); /* 120px 是对顶部导航栏和各种间距的估算值 */
  overflow-y: auto;
  box-sizing: border-box;
}
.page-header {
  margin-bottom: 20px;
  background-color: #fff;
  padding: 16px 24px;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.1);
}
.content-area {
  display: flex;
  flex-direction: column;
  gap: 20px;
}
.box-card {
  border-radius: 4px;
}
.card-header {
  font-weight: bold;
  font-size: 16px;
}
.print-button-container {
  position: fixed;
  bottom: 40px;
  right: 40px;
  z-index: 100;
}
</style> 