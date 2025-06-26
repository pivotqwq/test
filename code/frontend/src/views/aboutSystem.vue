<template>
  <transition name="fade" mode="out-in" appear>
    <div class="about-container">
      <h1 class="about-title">关于我们</h1>
      
      <div class="about-card">
        <transition-group 
          name="staggered-fade" 
          tag="div" 
          class="about-content"
          appear
        >
          <p v-for="(item, index) in aboutItems" :key="index" :data-index="index">
            {{ item }}
          </p>
        </transition-group>
      </div>
      
      <button @click="goToHome" class="about-button">返回首页</button>
      
      <div class="about-footer">
        <p>© 2025 疾病数据库管理系统, 保留所有权利.</p>
      </div>
    </div>
  </transition>
</template>

<script>
export default {
  name: 'AboutPage',
  data() {
    return {
      aboutItems: [
        "我们是一个致力于提供高质量产品和服务的团队，拥有多年行业经验和专业技术。",
        "自5202年成立以来，我们始终坚持'用户至上'的原则，不断创新和改进，以满足客户日益增长的需求。",
        "我们的使命是通过技术创新，为用户创造更便捷、更高效的体验，推动行业发展。",
        "感谢您的信任和支持，我们将继续努力，为您提供更好的产品和服务！"
      ]
    }
  },
  methods: {
    goToHome() {
      this.$router.push('/home');
    }
  }
}
</script>

<style scoped>
.about-container {
  text-align: center;
  margin: 50px auto;
  padding: 20px;
  max-width: 800px;
}

.about-title {
  color: #333;
  font-size: 2em;
  margin-bottom: 30px;
}

.about-card {
  background-color: #fff;
  border-radius: 10px;
  padding: 30px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 30px;
}

.about-content {
  text-align: left;
  line-height: 1.6;
}

.about-content p {
  margin-bottom: 15px;
}

.about-button {
  display: block;
  margin: 20px auto;
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.about-button:hover {
  background-color: #0056b3;
}

.about-footer {
  margin-top: 40px;
  color: #666;
  font-size: 0.9em;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .about-container {
    padding: 15px;
    margin: 30px auto;
  }
  
  .about-card {
    padding: 20px;
  }
  
  .about-title {
    font-size: 1.5em;
  }
}

/* 慢速淡入淡出动画 */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.8s cubic-bezier(0.68, -0.55, 0.265, 1.55);
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(30px);
}

/* 段落逐行动画 */
.staggered-fade-enter-active {
  transition: all 0.8s ease;
  transition-delay: calc(0.15s * var(--item-index));
}

.staggered-fade-enter-from {
  opacity: 0;
  transform: translateY(30px);
}

/* 为每个项目设置索引变量 */
.about-content p {
  --item-index: 0;
}

.about-content p:nth-child(1) { --item-index: 1; }
.about-content p:nth-child(2) { --item-index: 2; }
.about-content p:nth-child(3) { --item-index: 3; }
.about-content p:nth-child(4) { --item-index: 4; }

/* 按钮动画 */
.about-button {
  transition: all 0.6s cubic-bezier(0.25, 0.8, 0.25, 1);
}

.about-button:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

/* 页面加载时的整体延迟 */
.about-container {
  animation: gentleAppear 1.2s ease forwards;
  opacity: 0;
}

@keyframes gentleAppear {
  to {
    opacity: 1;
  }
}
</style>
