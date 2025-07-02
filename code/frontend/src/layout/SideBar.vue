<template>
  <div class="sidebar-full-height">
    <div class="sidebar-logo">
      <span>菜单列表</span>
    </div>

    <el-scrollbar class="menu-scrollbar">
      <el-menu
        :default-active="$route.path"
        class="el-menu-vertical"
        background-color="#455A64"
        text-color="#bfcbd9"
        active-text-color="#2196F3"
        router
        unique-opened
        @select="handleMenuSelect"
      >
        <el-menu-item index="/home">
          <el-icon><HomeFilled /></el-icon>
          <span>首页</span>
        </el-menu-item>

        <el-menu-item index="/user/profile">
          <el-icon><Postcard /></el-icon>
          <span>个人中心</span>
        </el-menu-item>

        <el-menu-item index="/user/list">
          <el-icon><User /></el-icon>
          <span>用户列表</span>
        </el-menu-item>
        
        <el-sub-menu index="2">
          <template #title>
            <el-icon><Discount /></el-icon>
            <span>患者管理</span>
          </template>
          <el-menu-item index="/myPatient">我的患者</el-menu-item>
          <el-menu-item index="/allPatient">全部患者</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="3">
          <template #title>
            <el-icon><Histogram /></el-icon>
            <span>数据中心</span>
          </template>
          <el-menu-item index="/follow-up">随访数据</el-menu-item>
          <el-menu-item index="/labDt">实验数据</el-menu-item>
          <el-menu-item index="/InvestigationDt">调研数据</el-menu-item>
        </el-sub-menu>

        <el-menu-item index="/record">
          <el-icon><EditPen /></el-icon>
          <span>我的待办</span>
        </el-menu-item>

        <el-menu-item index="/about">
          <el-icon><Setting /></el-icon>
          <span>系统设置/关于我们</span>
        </el-menu-item>
              </el-menu>
    </el-scrollbar>
  </div>
</template>

<script>
import { 
  HomeFilled, 
  User, 
  Setting, 
  Postcard, 
  Discount, 
  Histogram, 
  EditPen 
} from '@element-plus/icons-vue'

export default {
  name: 'SideBar',
  components: { 
    HomeFilled, 
    User, 
    Setting, 
    Postcard, 
    Discount, 
    Histogram, 
    EditPen 
  },
  methods: {
    handleMenuSelect(index) {
      console.log('菜单选择:', index)
      console.log('当前路由:', this.$route.path)
      
      // 如果当前路由和目标路由相同，强制刷新
      if (this.$route.path === index) {
        console.log('相同路由，强制刷新')
        this.$router.push({ path: index, query: { t: Date.now() } })
      }
    }
  }
}
</script>

<style scoped>
.sidebar-full-height {
  width: 220px;
  height: 100vh;
  background: #455A64; 
  position: fixed;
  left: 0;
  top: 0;
  z-index: 1000;
}

.sidebar-logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 18px;
  border-bottom: 1px solid #2b2f3a;
}

.menu-scrollbar {
  height: calc(100vh - 60px);
}

.el-menu-vertical {
  border-right: none;
  height: 100%;
}

.el-menu-vertical:not(.el-menu--collapse) {
  width: 220px;
}
</style>