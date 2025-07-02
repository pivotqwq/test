import { createRouter, createWebHistory } from 'vue-router';
import LoginPage from '../views/LoginPage.vue';
import HomePage from '../views/HomePage.vue';
import MainLayout from '../layout/MainLayout.vue'
import aboutSystem from '../views/aboutSystem.vue'
import InvestigationData from '../views/Investigation_Data.vue'
import LabData from '../views/labData.vue'
import ClinicalDetail from '../views/ClinicalDetail.vue'
import FollowUp from '../views/FollowUp.vue'

const routes = [
  {
    path: '/login',
    name: '登录页',
    component: LoginPage,
  },
  {
    path: '/',
    component: MainLayout,
    redirect: '/login',
    children: [
      {
        path: 'home',
        name: 'home',
        component: HomePage,
        meta: { title: '首页' }
      },
      {
        path: 'user/list',
        component: () => import('../views/List.vue'),
        meta: { title: '用户列表' }
      },
      {
        path: 'user/profile',
        name: '个人中心',
        component: () => import('../views/UserProfile.vue'),
        meta: { requiresAuth: true }
      },
      {
        path: 'myPatient',
        name: '我的病患',
        component: () => import('../views/myPatient.vue'),
        meta: { requiresAuth: true }
      },
      {
        path: 'allPatient',
        name: '全部病患',
        component: () => import('../views/allPatient.vue'),
        meta: { requiresAuth: true }
      },
      {
        path: 'detail/:id',
        name: 'PatientDetail',
        component: () => import('../views/PatientDetail.vue'),
        meta: { title: '患者详情',requiresAuth: true}
      },
      {
        path: 'record',
        name: 'myRecord',
        component: () => import('../views/myRecord.vue'),
        meta: { title: '我的代办',requiresAuth: true}
      },
      {
        path: 'labDt',
        name: 'labData',
        component: LabData,
        meta: { title: '实验室数据',requiresAuth: true}
      },
      {
        path: 'InvestigationDt',
        name: 'Investigation_Data',
        component: InvestigationData,
        meta: { title: '调研数据',requiresAuth: true}
      },
      {
        path: 'follow-up',
        name: 'FollowUp',
        component: FollowUp,
        meta: { title: '随访数据', requiresAuth: true }
      },
      {
        path: 'about',
        name: 'about',
        component: aboutSystem
      },
      {
        path: 'clinical-detail/:patientId',
        name: 'ClinicalDetail',
        component: ClinicalDetail,
        props: true
      }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// 路由守卫
router.beforeEach((to, from, next) => {
  console.log('路由跳转:', from.path, '->', to.path)
  
  const token = localStorage.getItem('token')
  
  // 如果要去登录页，直接跳转
  if (to.path === '/login') {
    next()
    return
  }
  
  // 暂时简化token验证，允许所有页面访问（调试用）
  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!token) {
      console.log('没有token，跳转到登录页')
      // 暂时设置一个临时token用于测试
      localStorage.setItem('token', 'test-token')
      localStorage.setItem('username', 'test-user')
      localStorage.setItem('userId', 'test-user-id')
      next()
    } else {
      next()
    }
  } else {
    next()
  }
})

export default router;
