import { get, post, put, del } from '@/utils/request'

// 认证相关API
export const authApi = {
  // 登录
  login(data) {
    return post('/Auth/login', data)
  },
  
  // 注册
  register(data) {
    return post('/Auth/register', data)
  },
  
  // 检查是否是管理员
  isAdmin(userId) {
    return get(`/Auth/is-admin/${userId}`)
  }
}

// 患者信息相关API
export const patientApi = {
  // 获取所有患者列表
  getAll(params) {
    return get('/patientInfo/allPatients', params)
  },
  
  // 根据ID获取患者详情
  getById(id) {
    return get(`/patientInfo/${id}`)
  },
  
  // 新增患者
  create(data) {
    return post('/patientInfo/add', data)
  },
  
  // 更新患者信息
  update(id, data) {
    return put(`/patientInfo/${id}`, data)
  },
  
  // 删除患者
  delete(id) {
    return del(`/patientInfo/${id}`)
  },
  
  // 搜索患者
  search(params) {
    return get('/patientInfo/search', params)
  }
}

// 患者基本信息API
export const patientBasicInfoApi = {
  // 获取患者基本信息列表（分页）
  getAll(params) {
    return get('/PatientBasicInfo/paged', params)
  },
  
  // 获取所有患者基本信息（不分页）
  getAllWithoutPaging() {
    return get('/PatientBasicInfo')
  },
  
  // 根据患者ID获取基本信息
  getByPatientId(patientId) {
    return get(`/PatientBasicInfo/patient/${patientId}`)
  },
  
  // 新增患者基本信息
  create(data) {
    return post('/PatientBasicInfo', data)
  },
  
  // 更新患者基本信息
  update(id, data) {
    return put(`/PatientBasicInfo/${id}`, data)
  },
  
  // 删除患者基本信息
  delete(id) {
    return del(`/PatientBasicInfo/${id}`)
  },
  
  // 搜索患者
  search(params) {
    return get('/PatientBasicInfo/search', params)
  }
}

// 调研数据API
export const investigationApi = {
  // 获取所有调研数据
  getAll(params) {
    return get('/QuestionnaireData', params)
  },
  
  // 根据ID获取调研数据
  getById(id) {
    return get(`/QuestionnaireData/${id}`)
  },
  
  // 新增调研数据
  create(data) {
    return post('/QuestionnaireData', data)
  },
  
  // 更新调研数据
  update(id, data) {
    return put(`/QuestionnaireData/${id}`, data)
  },
  
  // 删除调研数据
  delete(id) {
    return del(`/QuestionnaireData/${id}`)
  },
  
  // 根据患者ID获取调研数据
  getByPatientId(patientId) {
    return get(`/QuestionnaireData/patient/${patientId}`)
  }
}

// 家庭环境数据API
export const householdEnvironmentApi = {
  // 获取所有家庭环境数据
  getAll(params) {
    return get('/HouseholdEnvironment', params)
  },
  
  // 根据ID获取家庭环境数据
  getById(id) {
    return get(`/HouseholdEnvironment/${id}`)
  },
  
  // 新增家庭环境数据
  create(data) {
    return post('/HouseholdEnvironment', data)
  },
  
  // 更新家庭环境数据
  update(id, data) {
    return put(`/HouseholdEnvironment/${id}`, data)
  },
  
  // 删除家庭环境数据
  delete(id) {
    return del(`/HouseholdEnvironment/${id}`)
  },
  
  // 根据患者ID获取家庭环境数据
  getByPatientId(patientId) {
    return get(`/HouseholdEnvironment/patient/${patientId}`)
  }
}

// 个人健康行为数据API
export const individualHealthBehaviorApi = {
  // 获取所有个人健康行为数据
  getAll(params) {
    return get('/IndividualHealthBehavior', params)
  },
  
  // 根据ID获取个人健康行为数据
  getById(id) {
    return get(`/IndividualHealthBehavior/${id}`)
  },
  
  // 新增个人健康行为数据
  create(data) {
    return post('/IndividualHealthBehavior', data)
  },
  
  // 更新个人健康行为数据
  update(id, data) {
    return put(`/IndividualHealthBehavior/${id}`, data)
  },
  
  // 删除个人健康行为数据
  delete(id) {
    return del(`/IndividualHealthBehavior/${id}`)
  },
  
  // 根据患者ID获取个人健康行为数据
  getByPatientId(patientId) {
    return get(`/IndividualHealthBehavior/patient/${patientId}`)
  }
}

// 随访记录API
export const followUpApi = {
  // 获取所有随访记录
  getAll(params) {
    return get('/FollowUpRecord', params)
  },
  
  // 根据ID获取随访记录
  getById(id) {
    return get(`/FollowUpRecord/${id}`)
  },
  
  // 新增随访记录
  create(data) {
    return post('/FollowUpRecord', data)
  },
  
  // 更新随访记录
  update(id, data) {
    return put(`/FollowUpRecord/${id}`, data)
  },
  
  // 删除随访记录
  delete(id) {
    return del(`/FollowUpRecord/${id}`)
  },
  
  // 根据患者ID获取随访记录
  getByPatientId(patientId) {
    return get(`/FollowUpRecord/patient/${patientId}`)
  }
}

// 实验室数据API
export const labDataApi = {
  // 样本信息
  specimenInfo: {
    getAll(params) {
      return get('/SpecimenInfo', params)
    },
    getById(id) {
      return get(`/SpecimenInfo/${id}`)
    },
    create(data) {
      return post('/SpecimenInfo', data)
    },
    update(id, data) {
      return put(`/SpecimenInfo/${id}`, data)
    },
    delete(id) {
      return del(`/SpecimenInfo/${id}`)
    }
  },
  
  // 样本质量
  specimenQuality: {
    getAll(params) {
      return get('/SpecimenQualities', params)
    },
    getById(id) {
      return get(`/SpecimenQualities/${id}`)
    },
    create(data) {
      return post('/SpecimenQualities', data)
    },
    update(id, data) {
      return put(`/SpecimenQualities/${id}`, data)
    },
    delete(id) {
      return del(`/SpecimenQualities/${id}`)
    }
  },
  
  // 基因组数据
  genomicData: {
    getAll(params) {
      return get('/GenomicData', params)
    },
    getById(id) {
      return get(`/GenomicData/${id}`)
    },
    create(data) {
      return post('/GenomicData', data)
    },
    update(id, data) {
      return put(`/GenomicData/${id}`, data)
    },
    delete(id) {
      return del(`/GenomicData/${id}`)
    }
  },
  
  // 蛋白质数据
  proteinData: {
    getAll(params) {
      return get('/ProteinData', params)
    },
    getById(id) {
      return get(`/ProteinData/${id}`)
    },
    create(data) {
      return post('/ProteinData', data)
    },
    update(id, data) {
      return put(`/ProteinData/${id}`, data)
    },
    delete(id) {
      return del(`/ProteinData/${id}`)
    }
  }
}

// 临床数据API
export const clinicalDataApi = {
  // 获取所有临床数据
  getAll(params) {
    return get('/ClinicalData', params)
  },
  
  // 根据ID获取临床数据
  getById(id) {
    return get(`/ClinicalData/${id}`)
  },
  
  // 新增临床数据
  create(data) {
    return post('/ClinicalData', data)
  },
  
  // 更新临床数据
  update(id, data) {
    return put(`/ClinicalData/${id}`, data)
  },
  
  // 删除临床数据
  delete(id) {
    return del(`/ClinicalData/${id}`)
  }
}

// 调研员资质API
export const investigatorQualificationApi = {
  // 获取所有调研员资质
  getAll(params) {
    return get('/InvestigatorQualification', params)
  },
  
  // 根据ID获取调研员资质
  getById(id) {
    return get(`/InvestigatorQualification/${id}`)
  },
  
  // 新增调研员资质
  create(data) {
    return post('/InvestigatorQualification', data)
  },
  
  // 更新调研员资质
  update(id, data) {
    return put(`/InvestigatorQualification/${id}`, data)
  },
  
  // 删除调研员资质
  delete(id) {
    return del(`/InvestigatorQualification/${id}`)
  }
}

// 备忘录API
export const memoApi = {
  // 获取用户的备忘录列表
  getByUserId(userId, params) {
    return get(`/mem/user/${userId}`, params)
  },
  
  // 根据ID获取备忘录
  getById(id) {
    return get(`/mem/${id}`)
  },
  
  // 新增备忘录
  create(data) {
    return post('/mem', data)
  },
  
  // 更新备忘录
  update(id, data) {
    return put(`/mem/${id}`, data)
  },
  
  // 删除备忘录
  delete(id) {
    return del(`/mem/${id}`)
  },
  
  // 标记完成/未完成
  toggleStatus(id) {
    return put(`/mem/${id}/toggle`)
  }
}

// 用户管理API
export const userApi = {
  // 获取所有用户
  getAll(params) {
    return get('/User', params)
  },
  
  // 根据ID获取用户信息
  getById(id) {
    return get(`/User/${id}`)
  },
  
  // 更新用户信息
  update(id, data) {
    return put(`/User/${id}`, data)
  },
  
  // 删除用户
  delete(id) {
    return del(`/User/${id}`)
  },
  
  // 获取用户资料
  getProfile() {
    return get('/User/profile')
  },
  
  // 更新用户资料
  updateProfile(data) {
    return put('/User/profile', data)
  }
}

// 导出所有API
export default {
  auth: authApi,
  patient: patientApi,
  patientBasicInfo: patientBasicInfoApi,
  investigation: investigationApi,
  householdEnvironment: householdEnvironmentApi,
  individualHealthBehavior: individualHealthBehaviorApi,
  followUp: followUpApi,
  labData: labDataApi,
  clinicalData: clinicalDataApi,
  investigatorQualification: investigatorQualificationApi,
  memo: memoApi,
  user: userApi
} 