const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    client: {
      overlay: {
        runtimeErrors: (error) => {
          if (error.message.includes('ResizeObserver loop completed with undelivered notifications')) {
            return false;
          }
          return true;
        },
      },
    },
  },
})

