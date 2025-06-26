<template>
  <div class="home-container">
    <div class="welcome-box">
      <h1>Welcome to the Home Page</h1>
      <div class="button-group" v-if="!buttonsHidden">
        <button @click="hideButtons">隐藏按钮</button>
        <button @click="goToAbout">Go to About Page</button>
      </div>
    </div>
    
    <div class="grid-container">
      <div class="grid-item" v-for="(item, index) in 6" :key="index">
        Grid {{ index + 1 }}
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'HomePage',
  data() {
    return {
      buttonsHidden: false
    }
  },
  methods: {
    goToAbout() {
      this.$router.push('/about'); 
    },
    hideButtons() {
      this.buttonsHidden = true;
    }
  },
  mounted() {
    this.buttonsHidden = false;
  }
};
</script>

<style scoped>
.home-container {
  display: grid;
  grid-template-rows: auto 1fr;
  height: 90vh;
  width: 85vw;
  margin: 0;
  padding: 0;
  overflow: hidden;
}

.welcome-box {
  text-align: center;
  padding: 20px;
  background-color: #f0f0f0;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

h1 {
  color: #333;
  font-size: 2em;
  margin-bottom: 20px;
}

.button-group {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-bottom: 10px;
}

button {
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: background-color 0.3s;
}

button:hover {
  background-color: #0056b3;
}

.grid-container {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  grid-template-rows: repeat(2, 1fr);
  gap: 10px;
  padding: 10px;
  height: 100%;
  box-sizing: border-box;
}

.grid-item {
  background-color: #e9e9e9;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s, box-shadow 0.3s;
}

.grid-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}
</style>