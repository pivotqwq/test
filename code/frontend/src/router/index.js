import { createRouter, createWebHistory } from 'vue-router';
import LoginPage from '../views/LoginPage.vue';
import HomePage from '../views/HomePage.vue';
import MainLayout from '../layout/MainLayout.vue'
import aboutSystem from '../views/aboutSystem.vue'

const routes = [
  {
    path: '/',
    name: '登录页',
    component: LoginPage,
    alias: '/login'
  },
  {
    path: '/',
    component: MainLayout,
    redirect: '/home',
    children: [
      {
        path: 'home',
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
        path: 'detail/:medicalRecordNo',
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
    ]
  },
  {
    path: '/',
    name: '系统设置/关于我们',
    component: aboutSystem,
    alias: '/about'
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
