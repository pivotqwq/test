<template>
  <div class="regional-environment-container">
    <div v-if="loading" class="loading-container">
      <el-loading :loading="loading" text="加载中..."></el-loading>
    </div>
    <el-descriptions
      title="区域环境数据"
      :column="2"
      border
      v-loading="loading"
    >
      <el-descriptions-item label="区域名称" :span="2">{{ regionData.region_name || '无数据' }}</el-descriptions-item>
      <el-descriptions-item label="绿化率" :span="2">
        <el-tag :type="getGreenSpaceColor(regionData.green_space_rate)">
          {{ regionData.green_space_rate ? percentFormat(regionData.green_space_rate) : '无数据' }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="空气质量指数" :span="2">
        <el-tag :type="getAQIType(regionData.air_quality_index)">
          {{ getAQIText(regionData.air_quality_index) }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="花粉浓度" :span="2">
        <el-tag :type="getPollenType(regionData.pollen_concentration)">
          {{ regionData.pollen_concentration || '无数据' }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="气候类型" :span="2">{{ regionData.climate_type || '无数据' }}</el-descriptions-item>
      <el-descriptions-item label="平均温度" :span="2">
        <span :class="getTemperatureClass(regionData.avg_temperature)">
          {{ regionData.avg_temperature ? `${regionData.avg_temperature}°C` : '无数据' }}
        </span>
      </el-descriptions-item>
      <el-descriptions-item label="湿度水平" :span="2">{{ regionData.humidity_level ? percentFormat(regionData.humidity_level) : '无数据' }}</el-descriptions-item>
      <el-descriptions-item label="数据更新日期" :span="2">{{ regionData.update_date ? new Date(regionData.update_date).toLocaleDateString() : '无数据' }}</el-descriptions-item>
    </el-descriptions>
  </div>
</template>

<script setup>
import { defineProps, ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';

const props = defineProps({
  regionData: {
    type: Object,
    default: () => ({})
  },
  regionId: {
    type: String,
    default: ''
  }
});

const loading = ref(false);
const regionData = ref({});

onMounted(async () => {
  if (props.regionData && Object.keys(props.regionData).length > 0) {
    regionData.value = props.regionData;
    console.log('使用传入的区域环境数据:', regionData.value);
  } else if (props.regionId) {
    await fetchRegionalEnvironmentData(props.regionId);
  } else {
    regionData.value = getDefaultRegionData();
  }
});

const fetchRegionalEnvironmentData = async (regionId) => {
  loading.value = true;
  try {
    const response = await fetch(`http://localhost:5000/api/RegionalEnvironment/${regionId}`, {
      method: 'GET',
      headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token'),
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      if (response.status === 404) {
        console.log(`区域 ${regionId} 暂无环境数据，使用默认数据`);
        regionData.value = getDefaultRegionData();
        return;
      }
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    
    if (result) {
      regionData.value = result;
      console.log('获取到区域环境数据:', regionData.value);
    } else {
      regionData.value = getDefaultRegionData();
    }
  } catch (error) {
    console.error('API Error:', error);
    ElMessage.warning('使用默认区域环境数据: ' + error.message);
    regionData.value = getDefaultRegionData();
  } finally {
    loading.value = false;
  }
};

const getDefaultRegionData = () => {
  return {
    region_name: '默认区域',
    green_space_rate: 30,
    air_quality_index: 85,
    pollen_concentration: '中',
    climate_type: '温带',
    avg_temperature: 20,
    humidity_level: 60,
    update_date: new Date().toISOString()
  };
};

const percentFormat = (percentage) => {
  return percentage ? `${percentage}%` : '无数据';
};

const getGreenSpaceColor = (value) => {
  if (!value) return '#909399';
  const val = parseFloat(value);
  if (val >= 40) return '#67c23a';
  if (val >= 20) return '#e6a23c';
  return '#f56c6c';
};

const getAQIType = (value) => {
  if (value === undefined || value === null) return 'info';
  const aqi = parseInt(value, 10);
  if (aqi <= 50) return 'success';
  if (aqi <= 100) return 'primary';
  if (aqi <= 150) return 'warning';
  return 'danger';
};

const getAQIText = (value) => {
  if (value === undefined || value === null) return '无数据';
  const aqi = parseInt(value, 10);
  if (aqi <= 50) return `优 (${aqi})`;
  if (aqi <= 100) return `良 (${aqi})`;
  if (aqi <= 150) return `轻度污染 (${aqi})`;
  if (aqi <= 200) return `中度污染 (${aqi})`;
  if (aqi <= 300) return `重度污染 (${aqi})`;
  return `严重污染 (${aqi})`;
};

const getPollenType = (level) => {
  if (!level) return 'info';
  switch (level) {
    case '低': return 'success';
    case '中': return 'warning';
    case '高': return 'danger';
    default: return 'info';
  }
};

const getTemperatureClass = (temp) => {
  if (temp === undefined || temp === null) return '';
  const value = parseFloat(temp);
  if (value <= 10) return 'cold-temp';
  if (value >= 30) return 'hot-temp';
  return 'normal-temp';
};
</script>

<style scoped>
.regional-environment-container {
  padding: 10px;
}
.cold-temp, .hot-temp, .normal-temp {
  font-weight: bold;
}
.cold-temp { color: #409eff; }
.normal-temp { color: #67c23a; }
.hot-temp { color: #f56c6c; }
:deep(.el-descriptions__label) {
  width: 150px;
}
</style> 