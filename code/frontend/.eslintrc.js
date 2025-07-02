module.exports = {
  root: true,
  env: {
    node: true
  },
  globals: {
    labForm: 'readonly'
  },
  extends: [
    'plugin:vue/vue3-essential',
    'eslint:recommended'
  ],
  parserOptions: {
    parser: '@babel/eslint-parser',
    requireConfigFile: false,
    ecmaVersion: 'latest' 
  },
  rules: {
// ... existing code ...
  }
} 