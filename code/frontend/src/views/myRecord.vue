<template>
  <div class="memo-container">
    <el-card class="memo-card">
      <template #header>
        <div class="card-header">
          <span>个人备忘录</span>
          <el-button type="primary" @click="showAddDialog">新增备忘录</el-button>
        </div>
      </template>

      <!-- 备忘录列表 -->
      <el-table :data="memos" style="width: 100%" v-loading="loading">
        <el-table-column prop="title" label="标题" width="180" />
        <el-table-column prop="content" label="内容" />
        <el-table-column prop="createdAt" label="创建时间" width="200" />
        <el-table-column label="操作" width="150">
          <template #default="scope">
            <el-button size="small" @click="editMemo(scope.row)">编辑</el-button>
            <el-button size="small" type="danger" @click="deleteMemo(scope.row.id)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="50%">
      <el-form :model="formData" :rules="rules" ref="memoForm">
        <el-form-item label="标题" prop="title">
          <el-input v-model="formData.title" placeholder="请输入标题" />
        </el-form-item>
        <el-form-item label="内容" prop="content">
          <el-input 
            v-model="formData.content" 
            type="textarea" 
            :rows="5" 
            placeholder="请输入内容" 
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitForm">确认</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'

// 状态管理
const loading = ref(false)
const memos = ref([])
const dialogVisible = ref(false)
const dialogTitle = ref('新增备忘录')
const formData = ref({
  id: null,
  title: '',
  content: ''
})
const currentMemoId = ref(null)

// 获取当前用户ID - 实际应从登录状态获取
const getCurrentUserId = () => {
  //const token = localStorage.getItem('token');
  //const userId = this.$getUserIdFromToken(token);
  return localStorage.getItem('userId') || 'default_user_id'
}

// 获取备忘录列表
const fetchMemos = async () => {
  loading.value = true
  try {
    const userId = getCurrentUserId()
    const response = await fetch(`http://localhost:5000/api/mem/myMemos?userId=${userId}`, {
      method: 'GET',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    
    if (result.code === 200) {
      memos.value = result.data || [];
    } else {
      throw new Error(result.message || '获取备忘录失败');
    }
  } catch (error) {
    console.error('API Error:', error);
    ElMessage.error('获取备忘录失败: ' + error.message);
    memos.value = [];
  } finally {
    loading.value = false;
  }
}

// 显示新增对话框
const showAddDialog = () => {
  dialogTitle.value = '新增备忘录'
  currentMemoId.value = null
  formData.value = { title: '', content: '' }
  dialogVisible.value = true
}

// 编辑备忘录
const editMemo = (memo) => {
  dialogTitle.value = '编辑备忘录'
  currentMemoId.value = memo.id
  formData.value = { ...memo }
  dialogVisible.value = true
}

// 提交表单
const submitForm = async () => {
  try {
    const userId = getCurrentUserId()
    const url = currentMemoId.value 
      ? `http://localhost:5000/api/mem/changeMemos?id=${currentMemoId.value}`
      : 'http://localhost:5000/api/mem/addMemos'
    
    const method = currentMemoId.value ? 'PUT' : 'POST'
    
    const requestBody = currentMemoId.value
      ? { title: formData.value.title, content: formData.value.content }
      : { 
          title: formData.value.title, 
          content: formData.value.content,
          userid: userId,
          isdone: 0,
          created_at: new Date().toISOString()
        }
    
    const response = await fetch(url, {
      method,
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(requestBody)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    
    if (result.code === 200) {
      ElMessage.success(currentMemoId.value ? '更新成功' : '新增成功');
      dialogVisible.value = false;
      fetchMemos();
    } else {
      throw new Error(result.message || '操作失败');
    }
  } catch (error) {
    console.error('Submit error:', error);
    ElMessage.error('操作失败: ' + error.message);
  }
}

// 删除备忘录
const deleteMemo = (id) => {
  ElMessageBox.confirm('确定要删除这条备忘录吗?', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      const response = await fetch(`http://localhost:5000/api/mem/del?id=${id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': 'Bearer ' + localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result = await response.json();
      
      if (result.code === 200) {
        ElMessage.success('删除成功');
        fetchMemos();
      } else {
        throw new Error(result.message || '删除失败');
      }
    } catch (error) {
      console.error('Delete error:', error);
      ElMessage.error('删除失败: ' + error.message);
    }
  }).catch(() => {
    ElMessage.info('已取消删除');
  })
}

// 初始化加载数据
onMounted(() => {
  fetchMemos()
})
</script>

<style scoped>
.memo-container {
  padding: 20px;
}

.memo-card {
  max-width: 1200px;
  margin: 0 auto;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>