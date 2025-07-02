<template>
  <div class="home-container">
    <div class="search-section">
      <div class="search-box">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="输入姓名、职位或联系方式..."
          class="search-input"
          @input="handleSearch"
          @keyup.enter="handleSearch"
        />
        
        <div class="search-options">
          <label>
            <input type="radio" v-model="searchType" value="name" @change="handleSearch" /> 按姓名
          </label>
          <label>
            <input type="radio" v-model="searchType" value="profession" @change="handleSearch" /> 按职位
          </label>
          <label>
            <input type="radio" v-model="searchType" value="all" @change="handleSearch" /> 全部
          </label>
          <button class="search-btn" @click="handleSearch">
            <i class="search-icon"></i>
            应用
          </button>
        </div>
      </div>
      
      <div class="user-stats">
        <h3>用户统计</h3>
        <p>总数: {{ allUsers.length }}</p>
        <template v-for="(count, position) in positionCount" :key="position">
          <p>{{ position }}: {{ count }}</p>
        </template>
      </div>
    </div>

    <div class="user-display-section">
      <div v-if="loading" class="loading-indicator">加载中...</div>
      <div v-else class="user-grid">
        <div
          v-for="user in displayedUsers"
          :key="user.id"
          class="user-card"
          @click="showUserDetail(user)"
        >
          <div v-if="!user.urlBase64" class="avatar-placeholder">
            {{ (user.name || '未').charAt(0) }}
          </div>
          <img v-else class="avatar" :src="getAvatarUrl(user.urlBase64)" />
          <div class="user-info">
            <h3>{{ user.name || user.username }}</h3>
            <p class="position">{{ user.position }}</p>
            <p class="contact">{{ user.phone }} | {{ user.email }}</p>
          </div>
        </div>
      </div>
      <div class="pagination">
    <!-- 上一页按钮 -->
    <button
      :disabled="pagination.current === 1"
      @click="changePage(pagination.current - 1)"
    >
      上一页
    </button>

    <!-- 页码列表 -->
    <div class="page-numbers">
      <button
        v-for="page in totalPages"
        :key="page"
        :class="{ active: page === pagination.current }"
        @click="changePage(page)"
      >
        {{ page }}
      </button>
    </div>

    <!-- 下一页按钮 -->
    <button
      :disabled="pagination.current === totalPages"
      @click="changePage(pagination.current + 1)"
    >
      下一页
    </button>
  </div>
    </div>
    
    <!-- 用户详情模态框 -->
    <div v-if="selectedUser" class="user-detail-modal" @click.self="closeUserDetail">
      <div class="user-detail-content">
        <div class="detail-header">
          <h2>{{ selectedUser.name || selectedUser.username }}</h2>
          <span class="position-tag">{{ selectedUser.position }}</span>
        </div>
        <div class="detail-body">
          <p><i class="icon-phone"></i> {{ selectedUser.phone }}</p>
          <p><i class="icon-email"></i> {{ selectedUser.email }}</p>
        </div>
        <button class="close-btn" @click="closeUserDetail">
          <i class="icon-close"></i> 关闭
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'HomePage',
  data() {
    return {
      users: [], // 从API获取的所有用户数据
      allUsers: [], // 所有用户数据（用于统计）
      selectedUser: null,
      searchQuery: '',
      searchType: 'all', // 'name', 'profession', 'all'
      filteredUsers: [], // 搜索过滤后的用户
      displayedUsers: [], // 当前页显示的用户
      loading: false,
      pagination: {
        current: 1,    // 当前页码
        size: 12,      // 每页条数
        total: 0       // 总数据量
      }
    };
  },
  created() {
    this.fetchUsers();
  },
  computed: {
    positionCount() {
      return this.allUsers.reduce((acc, user) => {
        const position = user.position || '未设置职位';
        acc[position] = (acc[position] || 0) + 1;
        return acc;
      }, {});
    },
    totalPages() {
      return Math.ceil(this.pagination.total / this.pagination.size);
    }
  },
  methods: {
    async fetchUsers() {
      this.loading = true;
      try {
        // 获取所有用户数据，不使用后端分页
        const response = await fetch(
          `http://localhost:5000/api/User/allUsers`,
          {
            headers: {
              'Authorization': 'Bearer ' + localStorage.getItem('token'),
              'Content-Type': 'application/json'
            }
          }
        )

        if (!response.ok) {
          throw new Error(`HTTP错误! 状态码: ${response.status}`);
        }

        const data = await response.json();
        
        if (data.code === 200) {
          // 转换并存储所有用户数据
          this.users = data.all.map(user => ({
            id: user.Id,
            username: user.username,
            name: user.name,
            position: user.profession || '未设置职位',
            phone: user.phone || '未设置电话',
            email: user.email || '未设置邮箱',
            urlBase64: user.urlBase64 === 'null' ? null : user.urlBase64
          }));
          
          this.allUsers = [...this.users]; // 复制所有数据用于统计
          this.filteredUsers = [...this.users]; // 初始化过滤数据
          this.pagination.total = this.users.length; // 设置总数
          
          // 初始化显示第一页数据
          this.updateDisplayedUsers();
          
        } else {
          this.$message.error(data.message || '获取用户列表失败');
        }
      } catch (error) {
        console.error('获取用户列表失败:', error);
        this.$message.error('获取用户列表失败: ' + error.message);
        // 使用模拟数据作为后备
        this.users = this.getMockUsers();
        this.allUsers = [...this.users];
        this.filteredUsers = [...this.users];
        this.pagination.total = this.users.length;
        this.updateDisplayedUsers();
      } finally {
        this.loading = false;
      }
    },

    // 更新当前页显示的用户数据
    updateDisplayedUsers() {
      const startIndex = (this.pagination.current - 1) * this.pagination.size;
      const endIndex = startIndex + this.pagination.size;
      this.displayedUsers = this.filteredUsers.slice(startIndex, endIndex);
      
      console.log('分页信息:', {
        current: this.pagination.current,
        size: this.pagination.size,
        total: this.pagination.total,
        totalPages: this.totalPages,
        startIndex,
        endIndex,
        displayedCount: this.displayedUsers.length
      });
    },

    getMockUsers() {
      return [
        { id: 1, name: '张三', position: '医生', phone: '12345678901', email: 'zhangsan@example.com' },
        { id: 2, username: 'member1', position: '护士', phone: '未设置电话', email: 'member1@example.com' },
        { id: 3, name: '王五', position: '科长', phone: '12345678903', email: 'wangwu@example.com' },
        { id: 4, username: 'admin', name: '管理员', position: '系统管理员', phone: '18065041131', email: 'admin@example.com' }
      ];
    },

    getAvatarUrl(urlBase64) {
      if (!urlBase64) return '';
      return urlBase64.startsWith('data:image/') ? urlBase64 : `data:image/png;base64,${urlBase64}`;
    },

    showUserDetail(user) {
      this.selectedUser = user;
    },

    closeUserDetail() {
      this.selectedUser = null;
    },

    handleSearch() {
      const query = this.searchQuery.toLowerCase().trim();
      
      if (!query) {
        // 没有搜索词时显示所有用户
        this.filteredUsers = [...this.users];
      } else {
        // 在所有用户中搜索
        switch (this.searchType) {
          case 'name':
            this.filteredUsers = this.users.filter(user => 
              (user.name || '').toLowerCase().includes(query) ||
              (user.username || '').toLowerCase().includes(query)
            );
            break;
          case 'profession':
            this.filteredUsers = this.users.filter(user => 
              (user.position || '').toLowerCase().includes(query)
            );
            break;
          case 'all':
          default:
            this.filteredUsers = this.users.filter(user => 
              (user.name || '').toLowerCase().includes(query) || 
              (user.username || '').toLowerCase().includes(query) ||
              (user.position || '').toLowerCase().includes(query) ||
              (user.email || '').toLowerCase().includes(query) ||
              (user.phone || '').includes(query)
            );
        }
      }
      
      // 搜索后重置分页
      this.pagination.current = 1;
      this.pagination.total = this.filteredUsers.length;
      this.updateDisplayedUsers();
    },

    changePage(newPage) {
      if (newPage >= 1 && newPage <= this.totalPages) {
        this.pagination.current = newPage;
        this.updateDisplayedUsers();
      }
    }
  }
};
</script>

<style scoped>
/* 基础布局 */
.home-container {
  display: flex;
  min-height: 100vh;
  background-color: #f5f7fa;
}

.search-section {
  width: 250px;
  padding: 20px;
  background-color: #fff;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
}

.user-display-section {
  flex: 1;
  padding: 66px;
  display: flex;
  flex-direction: column;
  justify-content: space-between; 
}

/* 搜索框样式 */
.search-box {
  margin-bottom: 20px;
}

.search-input {
  width: 80%;
  padding: 10px 15px;
  margin-bottom: 10px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 14px;
}

.search-options {
  padding: 10px;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.search-options label {
  display: flex;
  align-items: center;
  font-size: 14px;
}

/* 搜索按钮样式 */
.search-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: 38px;
  width: 58px;
  border-radius: 0.375rem;
  background-color: #409eff;
  color: white;
  transition: all 0.2s ease-in-out;
  border: none;
  cursor: pointer;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

/* 搜索按钮悬停效果 */
.search-btn:hover {
  background-color: #337ecc;
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

/* 搜索按钮点击效果 */
.search-btn:active {
  transform: translateY(0);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

/* 搜索图标样式 */
.search-icon {
  font-size: 16px;
}

/* 加载状态样式（如果需要） */
.search-btn.loading {
  cursor: not-allowed;
  opacity: 0.7;
}

.search-btn.loading .search-icon {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

/* 用户卡片样式 */
.user-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 20px;
}

.user-card {
  background-color: #fff;
  border-radius: 8px;
  padding: 15px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  display: flex;
  align-items: center;
}

.avatar-placeholder {
  width: 50px;
  height: 50px;
  background-color: #409eff;
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  margin-right: 15px;
}

.avatar {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  object-fit: cover;
  margin-right: 15px;
}

.user-info h3 {
  margin: 0 0 5px;
  font-size: 16px;
}

.position {
  color: #666;
  font-size: 14px;
  margin: 0 0 5px;
}

.contact {
  color: #999;
  font-size: 12px;
  margin: 0;
}

/* 模态框样式 */
.user-detail-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.user-detail-content {
  background-color: #fff;
  border-radius: 8px;
  padding: 25px;
  width: 90%;
  max-width: 500px;
}

.detail-header {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}

.position-tag {
  margin-left: 10px;
  padding: 4px 8px;
  background-color: #ecf5ff;
  color: #409eff;
  border-radius: 4px;
  font-size: 14px;
}

.close-btn {
  margin-top: 20px;
  padding: 8px 15px;
  background-color: #f5f7fa;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.loading-indicator {
  padding: 20px;
  text-align: center;
  color: #666;
}

/* 固定在底部的分页 */
.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 15px; /* 增大间距 */
  margin-top: 20px;
  margin-bottom: 40px; /* 增大底部间距 */
}

.page-numbers {
  display: flex;
  gap: 10px;
}

.page-numbers button {
  padding: 10px 15px;
  border: 1px solid #ccc;
  border-radius: 8px;
  background-color: white;
  cursor: pointer;
  font-size: 16px;
}

.page-numbers button.active {
  background-color: #007bff;
  color: white;
}

button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>