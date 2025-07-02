export const clinicalDetailsData = {
  'P10001': {
    patientInfo: {
      id: 'P10001',
      medical_record_no: 'MRN-000123',
      name: '张三',
      gender: '男',
      birth_date: '1988-08-08',
      address: '北京市朝阳区XX街道XX号'
    },
    insuranceInfo: {
      insurance_id: 'INS-001',
      patient_id: 'P10001',
      insurance_type: '基本医疗保险'
    },
    contactInfo: {
      contact_id: 'CON-001',
      patient_id: 'P10001',
      name: '张三丰',
      contact_info: '13800138000'
    },
    pastMedicalHistory: {
      history_id: 'PMH-001',
      patient_id: 'P10001',
      allergy_history: '青霉素过敏'
    },
    familyHistory: {
      family_history_id: 'FH-001',
      patient_id: 'P10001',
      allergy_history: '父亲有哮喘史'
    }
  },
  'P10002': {
    patientInfo: {
      id: 'P10002',
      medical_record_no: 'MRN-000124',
      name: '李四',
      gender: '女',
      birth_date: '1995-05-10',
      address: '上海市浦东新区YY大道YY号'
    },
    insuranceInfo: {
      insurance_id: 'INS-002',
      patient_id: 'P10002',
      insurance_type: '商业健康险'
    },
    contactInfo: {
      contact_id: 'CON-002',
      patient_id: 'P10002',
      name: '李四妹',
      contact_info: '13900139000'
    },
    pastMedicalHistory: {
      history_id: 'PMH-002',
      patient_id: 'P10002',
      allergy_history: '无'
    },
    familyHistory: {
      family_history_id: 'FH-002',
      patient_id: 'P10002',
      allergy_history: '母亲有花粉过敏史'
    }
  }
}; 