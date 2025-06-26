<template>
  <div class="app-container">
    <SideBar />
    <div class="main-wrapper">
      <TopNavbar @command="handleCommand" />
      <div class="app-main">
        <router-view v-slot="{ Component }">
          <transition name="fade-transform" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </div>
    </div>
  </div>
</template>

<script>
import SideBar from './SideBar.vue'
import TopNavbar from './TopNavbar.vue'

export default {
  components: { SideBar, TopNavbar },
  methods: {
    handleCommand(command) {
      switch (command) {
        case 'profile':
          this.$router.push('/user/profile')
          break
        case 'logout':
          this.handleLogout()
          break
      }
    },
    handleLogout() {
      this.$confirm('确定要退出登录吗?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.$router.push('/login')
        this.$message({
          type: 'success',
          message: '退出成功!'
        })
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消退出'
        })         
      })
    }
  }
}
</script>

<style scoped>
.app-container {
  display: flex;
  min-height: 100vh;
  background-color: #f5f5f5;
}

.main-wrapper {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  margin-left: 220px; /* 侧边栏宽度 */
}

.app-main {
  flex: 1;
  padding: 20px;
  overflow: auto;
  background-color: #fff;
  margin-top: 10px; /* 顶部导航栏高度 */
}

/* 过渡动画 */
.fade-transform-leave-active,
.fade-transform-enter-active {
  transition: all 0.3s cubic-bezier(0.55, 0, 0.1, 1);
}

.fade-transform-enter-from {
  opacity: 0;
  transform: translateX(30px);
}

.fade-transform-leave-to {
  opacity: 0;
  transform: translateX(130px);
}

/* 响应式调整 */
@media (max-width: 768px) {
  .main-wrapper {
    margin-left: 0;
  }
  
  .app-main {
    margin-top: 50px;
  }
}
</style>