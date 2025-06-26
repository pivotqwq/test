<template>
  <div>
    <header class="clearfix">
      <el-image style="margin-left: 30px; display: block; width: 60px; height: 60px" :src="require('@/assets/logo.png')"></el-image>
      <span class="title">过敏性疾病数据库管理系统</span>
    </header>
    <main>
      <el-image class="img-wrap" :src="require('@/assets/manager.png')"></el-image>
      <div class="loginwrap">
        <div>
          <div class="header-form">
            <p>{{ activeTab === '1' ? '系统登录' : '用户注册' }}</p>
          </div>
          <div class="info">
            <el-tabs tab-position="top" :stretch="true" v-model="activeTab">
              <!-- 登录标签页 -->
              <el-tab-pane label="用户名登录" name="1">
                <el-form ref="loginForm" :model="loginForm" :rules="loginRules" class="demo-ruleForm">
                  <el-form-item label="用户名" prop="username">
                    <el-input suffix-icon="el-icon-user-solid" v-model="loginForm.username" placeholder="请输入用户名"></el-input>
                  </el-form-item>
                  <el-form-item label="密码" prop="password">
                    <el-input v-model="loginForm.password" :type="inputType" placeholder="请输入密码" @keyup.enter="handleLogin">
                      <template v-slot:suffix>
                        <i @click="toggleEye" :class="eyeIconClass"></i>
                      </template>
                    </el-input>
                  </el-form-item>
                </el-form>
                <el-button type="primary" :loading="loading" style="width: 100%" @click="handleLogin">登录</el-button>
              </el-tab-pane>

              <!-- 注册标签页 -->
              <el-tab-pane label="注册" name="2">
                <el-form ref="registerForm" :model="registerForm" :rules="registerRules" class="demo-ruleForm">
                  <el-form-item label="用户名" prop="username">
                    <el-input suffix-icon="el-icon-user-solid" v-model="registerForm.username" placeholder="请输入用户名"></el-input>
                  </el-form-item>
                  <el-form-item label="密码" prop="password">
                    <el-input v-model="registerForm.password" :type="inputType" placeholder="请输入密码(至少6位)">
                      <template v-slot:suffix>
                        <i @click="toggleEye" :class="eyeIconClass"></i>
                      </template>
                    </el-input>
                  </el-form-item>
                  <el-form-item label="确认密码" prop="confirmPassword">
                    <el-input v-model="registerForm.confirmPassword" :type="inputType" placeholder="请再次输入密码">
                      <template v-slot:suffix>
                        <i @click="toggleEye" :class="eyeIconClass"></i>
                      </template>
                    </el-input>
                  </el-form-item>
                  <el-form-item label="邮箱" prop="email">
                    <el-input v-model="registerForm.email" placeholder="请输入邮箱"></el-input>
                  </el-form-item>
                </el-form>
                <el-button type="primary" :loading="registerLoading" style="width: 100%" @click="handleRegister">注册</el-button>
              </el-tab-pane>
            </el-tabs>
          </div>
        </div>
      </div>
    </main>
    <footer>
      <p>© 2025 过敏性疾病数据库管理系统</p>
    </footer>
  </div>
</template>

<script>
export default {
  data() {
    // 密码确认验证规则
    const validateConfirmPassword = (rule, value, callback) => {
      if (value !== this.registerForm.password) {
        callback(new Error('两次输入的密码不一致!'))
      } else {
        callback()
      }
    }

    return {
      activeTab: '1', // 默认激活登录标签页
      inputType: 'password', // 密码输入框类型
      eyeIconClass: 'el-icon-view', // 眼睛图标类名
      loading: false, // 登录按钮加载状态
      registerLoading: false, // 注册按钮加载状态

      // 登录表单数据
      loginForm: {
        username: '',
        password: ''
      },

      // 注册表单数据
      registerForm: {
        username: '',
        password: '',
        confirmPassword: '',
        email: ''
      },

      // 登录表单验证规则
      loginRules: {
        username: [
          { required: true, message: '请输入用户名', trigger: 'blur' }
        ],
        password: [
          { required: true, message: '请输入密码', trigger: 'blur' }
        ]
      },

      // 注册表单验证规则
      registerRules: {
        username: [
          { required: true, message: '请输入用户名', trigger: 'blur' },
          { min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' }
        ],
        password: [
          { required: true, message: '请输入密码', trigger: 'blur' },
          { min: 6, message: '密码长度不能少于6位', trigger: 'blur' }
        ],
        confirmPassword: [
          { required: true, message: '请再次输入密码', trigger: 'blur' },
          { validator: validateConfirmPassword, trigger: 'blur' }
        ],
        email: [
          { required: true, message: '请输入邮箱地址', trigger: 'blur' },
          { type: 'email', message: '请输入正确的邮箱地址', trigger: ['blur', 'change'] }
        ]
      }
    }
  },
  methods: {
    // 切换密码显示/隐藏
    toggleEye() {
      this.inputType = this.inputType === 'password' ? 'text' : 'password'
      this.eyeIconClass = this.eyeIconClass === 'el-icon-view' ? 'el-icon-lock' : 'el-icon-view'
    },

    // 处理登录
    async handleLogin() {
      this.$refs.loginForm.validate(async (valid) => {
        if (!valid) return
        
        this.loading = true
        try {
          const response = await fetch('http://localhost:5000/api/Auth/login', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify(this.loginForm)
          })
          
          const data = await response.json()

          if (response.ok && data.success) {
            // 存储token和用户名
            //localStorage.setItem('token', data.token);
            localStorage.setItem('username', this.loginForm.username);
            this.$message.success('登录成功')
            this.$router.push('/home')
          } else {
            this.$message.error(data.message || '登录失败')
          }
        } catch (error) {
          this.$message.error('网络错误，请稍后再试')
        } finally {
          this.loading = false
        }
      })
    },

    // 处理注册
    async handleRegister() {
      this.$refs.registerForm.validate(async (valid) => {
        if (!valid) return
        
        this.registerLoading = true
        try {
          const response = await fetch('http://localhost:5000/api/Auth/register', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({
              username: this.registerForm.username,
              password: this.registerForm.password,
              email: this.registerForm.email
            })
          })
          
          const data = await response.json()

          if (response.ok && data.success) {
            this.$message.success('注册成功')
            this.activeTab = '1'
            this.$refs.registerForm.resetFields()
          } else {
            this.$message.error(data.message || '注册失败')
          }
        } catch (error) {
          this.$message.error('网络错误，请稍后再试')
        } finally {
          this.registerLoading = false
        }
      })
    }
  }
}
</script>

<style scoped>
/* 头部 */
header {
  width: 100%;
  height: 70px;
  display: flex;
  justify-content: start;
  align-items: center;
  gap: 12px;
}

header img {
  margin-left: 40px;
  display: block;
  width: 50px;
  height: 50px;
}

header span {
  color: #0089f3;
  display: block;
  font-weight: 700;
  line-height: 70px;
  height: 70px;
  font-size: 1.5rem;
}

/* 中间部分 */
main {
  width: 100%;
  height: calc(100vh - 140px);
  position: relative;
  overflow: hidden;
  background-image: url(@/assets/background.png);
  background-size: cover;
}

main .img-wrap {
  position: absolute;
  left: 12vw;
  top: 8vh;
  width: 32vw;
  height: 32vw;
}

main .loginwrap {
  min-height: 50vh;
  width: 450px;
  border: 1px solid #ddd;
  background-color: white;
  padding-bottom: 20px;
  position: absolute;
  right: -501px;
  top: calc((100vh - 140px - 50vh - 40px) / 2);
  border-radius: 4px;
  animation: forwards move 0.8s;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.12), 0 0 6px rgba(0, 0, 0, 0.04);
}

main .loginwrap:hover {
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.32), 0 0 6px rgba(0, 0, 0, 0.24);
}

main .loginwrap p {
  text-align: center;
  font-size: 22px;
  margin: 10px 0;
}

main .loginwrap p i {
  color: #ebb563;
}

main .loginwrap .info {
  width: 70%;
  margin: 0 auto;
  padding: 2%;
}

main .loginwrap .info .el-form-item {
  margin: 20px 0 0;
  padding: 0;
  margin-bottom: 20px;
}

main .loginwrap .info .el-form-item .el-form-item__label {
  line-height: 40px;
}

main .loginwrap .info .el-form-item .el-input i {
  line-height: 40px;
  font-size: 22px;
  cursor: pointer;
}

@keyframes move {
  0% {
    transform: translateX(0);
  }
  100% {
    transform: translateX(-600px);
  }
}

footer {
  height: 70px;
  width: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

footer p {
  font-size: 13px;
  color: #303030;
}

.header-form {
  margin: 24px;
  text-align: center;
}

.el-form-item {
  margin-bottom: 22px;
}

.el-form-item__label {
  font-weight: bold;
}
</style>