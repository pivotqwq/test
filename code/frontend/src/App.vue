<template>
  <div id="app">
    <router-view></router-view>
  </div>
</template>

<script>
export default {
  name: 'App'
};
</script>

<style lang="less">
.el-scrollbar__wrap {
  overflow-x: hidden !important;
  margin-bottom: -5px !important;
}

* {
  margin: 0;
  padding: 0;
}

li {
  list-style: none !important;
}

body {
  overflow: hidden;
  padding-right:0 !important;
}

a {
  text-decoration: none;
}

.clearfix::before,
.clearfix::after {
  content: "";
  display: block;
  clear: both;
}
</style>
