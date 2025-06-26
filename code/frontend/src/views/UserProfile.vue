<template>
  <div class="user-profile-container">
    <el-card class="profile-card">
      <div class="profile-header">
        <h2>个人信息管理</h2>
      </div>
      
      <div class="profile-body">
        <!-- 头像区域 -->
        <div class="avatar-section">
          <el-upload
            class="avatar-uploader"
            :action="`http://localhost:5000/api/User/uploadAvatar?username=${userInfo.username}`"
            :headers="uploadHeaders"
            :show-file-list="false"
            :on-success="handleAvatarSuccess"
            :before-upload="beforeAvatarUpload"
            :on-error="handleAvatarError">
            <img v-if="userInfo.avatar" :src="getAvatarUrl(userInfo.avatar)" class="avatar">
            <el-icon v-else class="avatar-uploader-icon"><User /></el-icon>
          </el-upload>
          <p class="avatar-tip">点击头像更改</p>
        </div>
        
        <!-- 基本信息 -->
        <div class="info-section">
          <el-form 
            ref="profileForm" 
            :model="userInfo" 
            :rules="rules" 
            label-width="100px">
            
            <el-form-item label="用户名" prop="username">
              <el-input v-model="userInfo.username" disabled></el-input>
            </el-form-item>
            
            <el-form-item label="姓名" prop="fullname">
              <el-input v-model="userInfo.fullname"></el-input>
            </el-form-item>

            <el-form-item label="职位" prop="profession">
              <el-input v-model="userInfo.profession"></el-input>
            </el-form-item>

            <el-form-item label="邮箱" prop="email">
              <el-input v-model="userInfo.email"></el-input>
            </el-form-item>
            
            <el-form-item label="手机号" prop="phone">
              <el-input v-model="userInfo.phone"></el-input>
            </el-form-item>
          </el-form>
          
          <div class="action-buttons">
            <el-button 
              type="primary" 
              @click="submitForm" 
              :loading="loading">
              保存修改
            </el-button>
            <el-button @click="resetForm">重置</el-button>
          </div>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script>
import { User } from '@element-plus/icons-vue'

export default {
  components: { User },
  data() {
    // 手机号验证规则
    const validatePhone = (rule, value, callback) => {
      if (value && !/^1[3-9]\d{9}$/.test(value)) {
        callback(new Error('请输入正确的手机号码'))
      } else {
        callback()
      }
    }
    
    // 邮箱验证规则
    const validateEmail = (rule, value, callback) => {
      if (value && !/^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(value)) {
        callback(new Error('请输入正确的邮箱地址'))
      } else {
        callback()
      }
    }

    return {
      userInfo: {
        id: '',
        username: '',
        fullname: '',
        profession:'',
        email: '',
        phone: '',
        avatar: ''
      },
      originalInfo: {},
      loading: false,
      uploadHeaders: {
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      },
      rules: {
        email: [
          { validator: validateEmail, trigger: 'blur' }
        ],
        phone: [
          { validator: validatePhone, trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    this.fetchUserInfo()
  },
  methods: {
    // 获取当前用户信息
    async fetchUserInfo() {
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
    
        if (data.code === 200) {
          // 3. 映射返回数据到userInfo对象
          this.userInfo = {
            id: data.data.id || '',
            fullname: data.data.name || '',
            profession: data.data.profession || '',
            username: data.data.username || '',
            email: data.data.email || '',
            phone: data.data.phone || '',
            avatar: data.data.urlBase64 || ''  // 使用urlBase64作为头像字段
          };
          this.originalInfo = JSON.parse(JSON.stringify(this.userInfo));
        } else {
          this.$message.error(data.message || '获取用户信息失败');
        }
      } catch (error) {
        this.$message.error('获取用户信息失败: ' + error.message);
        // 设置默认值
        this.userInfo = {
          id: '',
          username: '',
          email: '',
          phone: '',
          avatar: ''
        };
      }
    },
    
    // 提交表单
    submitForm() {
      this.$refs.profileForm.validate(async (valid) => {
        if (!valid) return
        
        this.loading = true
        try {
          const response = await fetch('http://localhost:5000/api/User/change', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': 'Bearer ' + localStorage.getItem('token')
            },
            body: JSON.stringify({
              Username: this.userInfo.username,
              Email: this.userInfo.email,
              Phone: this.userInfo.phone,
              Name: this.userInfo.fullname,
              Profession: this.userInfo.profession
            })
          })
          
          const data = await response.json()
          
          if (data.code === 200) {
            this.$message.success('个人信息更新成功')
            this.originalInfo = JSON.parse(JSON.stringify(this.userInfo))
          } else {
            this.$message.error(data.message || '保存失败')
          }
        } catch (error) {
          this.$message.error('保存失败: ' + error.message)
        } finally {
          this.loading = false
        }
      })
    },
    
    // 重置表单
    resetForm() {
      this.userInfo = JSON.parse(JSON.stringify(this.originalInfo))
      this.$message.info('已重置修改')
    },
    
    // 头像上传成功
    handleAvatarSuccess(response) {
      if (response.code === 200) {
        this.userInfo.avatar = response.data.avatarPath
        this.$message.success('头像上传成功')
      } else {
        this.$message.error(response.message || '头像上传失败')
      }
    },
    
    // 头像上传失败
    handleAvatarError(error) {
      this.$message.error('头像上传失败: ' + error.message)
    },
    
    // 头像上传前验证
    beforeAvatarUpload(file) {
      const isImage = file.type.startsWith('image/')
      const isLt2M = file.size / 1024 / 1024 < 2
      
      if (!isImage) {
        this.$message.error('只能上传图片文件')
        return false
      }
      if (!isLt2M) {
        this.$message.error('头像图片大小不能超过2MB')
        return false
      }
      
      return true
    },
    getAvatarUrl(avatarPath) {
    // 如果已经是完整的data URL就直接返回
    if (avatarPath.startsWith('data:image/')) {
      return avatarPath;
    }
    // 否则添加base64前缀（假设是png格式）
    return `data:image/png;base64,${avatarPath}`;
  }
  }
}
</script>

<style scoped>
.user-profile-container {
  padding: 60px 40px;
  max-width: 1200px;
  margin: 0 auto;
  background-color: #F5F5F5;
}

.profile-card {
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  min-height: 400px;
}

.profile-header {
  text-align: center;
  margin-bottom: 30px;
  padding-top: 20px;
}

.profile-header h2 {
  color: #333;
  font-size: 24px;
}

.profile-body {
  display: flex;
  flex-wrap: wrap;
  padding: 0 20px 20px;
}

.avatar-section {
  width: 200px;
  padding: 20px;
  text-align: center;
  margin-right: 40px;
}

.info-section {
  flex: 1;
  min-width: 500px;
  padding: 20px;
}

.avatar-uploader {
  display: flex; /* 使用 flex 布局确保内容居中 */
  justify-content: center;
  align-items: center;
  width: 210px;  /* 明确设置宽度 */
  height: 210px; /* 高度必须和宽度相等 */
  margin: 0 auto 15px;
  border: 1px dashed #d9d9d9;
  border-radius: 50%;
  cursor: pointer;
  overflow: hidden;
  background-color: #f5f7fa; /* 可选：添加背景色更美观 */
}

.avatar {
  width: 100%;
  height: 100%;
  object-fit: cover; /* 关键：保持图片比例，填充容器 */
}

.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 120px;
  height: 120px;
  line-height: 120px;
  text-align: center;
}

.avatar-tip {
  font-size: 14px;
  color: #999;
  margin-top: 10px;
}

.action-buttons {
  text-align: center;
  margin-top: 30px;
}

.el-form-item {
  margin-bottom: 22px;
}

.el-form-item__label {
  font-weight: bold;
  color: #333;
}

/* 响应式调整 */
@media (max-width: 1024px) {
  .user-profile-container {
    padding: 40px 20px; /* 中等屏幕减少内边距 */
  }
}

@media (max-width: 768px) {
  .profile-body {
    flex-direction: column;
  }
  
  .avatar-section {
    width: 100%;
    margin-right: 0;
  }
  
  .user-profile-container {
    padding: 20px; /* 小屏幕进一步减少内边距 */
  }
}
</style>