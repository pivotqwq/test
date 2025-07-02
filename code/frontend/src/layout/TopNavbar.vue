<template>
  <div class="header-bar">
    <div class="header-bar-logo">
      <span>疾病数据库管理系统 2025</span>
    </div>
    <div class="search-container">
      <el-input
        v-model="searchKeyword"
        placeholder="搜索菜单"
        prefix-icon="search"
        clearable
        @clear="clearSearch"
        @input="filterMenus"
        class="search-input"
      />
    </div>
    <div class="user-info">
      <el-dropdown @command="handleCommand">
        <div class="user-avatar">
          <el-avatar :size="40" :src="user.avatar || defaultAvatar" />
          <span class="user-name">{{ user.name || '未登录用户' }}</span>
          <el-icon class="el-icon--right"><arrow-down /></el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="profile">个人中心</el-dropdown-item>
            <el-dropdown-item divided command="logout">退出登录</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script>
import { ArrowDown } from '@element-plus/icons-vue'
import defaultAvatar from '@/assets/default-avatar.png'

export default {
  components: { ArrowDown },
  data() {
    return {
      user: {
        name: '管理员',
        avatar: ''
      },
      defaultAvatar: defaultAvatar
    }
  },
  created() {
    this.fetchUserInfo()
  },
  methods: {
    async fetchUserInfo(){
      try {
        // 1. 获取当前登录用户名（假设存储在localStorage）
        const currentUsername = localStorage.getItem('username');
        /*if (!username) {
          this.$router.push('/login'); // 如果无用户名，跳转到登录页
          return;
        }*/
        if (!currentUsername) {
          throw new Error('未获取到当前用户名');
        }

        // 2. 发送请求获取特定用户信息
        const response = await fetch(`http://localhost:5000/api/User/findUser?username=${encodeURIComponent(currentUsername)}`, {
          method: 'GET',
          headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token'),
            'Content-Type': 'application/json'
          }
        });

        if (!response.ok) {
          throw new Error(`HTTP错误! 状态码: ${response.status}`);
        }

        const data = await response.json();
    
        if (data.code === 200){
          // 安全处理头像数据
          let avatar = defaultAvatar;
          if (data.data.urlBase64) {
            avatar = data.data.urlBase64.startsWith('data:image/') 
              ? data.data.urlBase64 
              : `data:image/png;base64,${data.data.urlBase64}`;
          }
          
          this.user = {
            name: data.data.name || "姓名未设置",
            avatar: avatar
          }
        }
      } catch (error) {
        this.$message.error('获取用户信息失败: ' + error.message);
      }
    },
    handleCommand(command) {
      this.$emit('command', command)
      if (command === 'profile') {
        setTimeout(() => {
        window.location.reload()
        }, 30)
      } 
  }
  }
}
</script>

<style scoped>
.header-bar {
  height: 60px;
  background: #CFD8DC;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  display: flex;
  justify-content: space-between; 
  align-items: center;
  padding: 0 20px;
}

.header-bar-logo {
  height: 60px;
  display: flex;
  align-items: center;
  color: black;
  font-size: 18px;
  font-weight: bold;
}

.search-container {
  display: flex;
  justify-content: flex-end;
  width: 300px;
}

.search-input {
  width: 100%;
}

.user-avatar {
  display: flex;
  align-items: center;
  padding: 10px;
}

.user-name {
  margin-left: 10px;
  margin-right: 5px;
  font-size: 14px;
}
</style>