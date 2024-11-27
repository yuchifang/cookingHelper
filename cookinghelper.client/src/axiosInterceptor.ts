import axios from "axios";

const axiosInstance = axios.create();
const MAX_RETRIES = 3;

// 添加回應攔截器
axiosInstance.interceptors.response.use(
  (response) => response, // 如果請求成功，直接返回回應
  async (error) => {
    const config = error.config;
    console.log({ config });
    // 如果沒有 config，或不需要重試，直接拋出錯誤
    if (!config || config.retryCount >= MAX_RETRIES) {
      return Promise.reject(error);
    }

    // 初始化重試計數
    if (!config.retryCount) {
      config.retryCount = 0;
    }

    // 增加重試次數
    config.retryCount += 1;

    console.log(`重試第 ${config.retryCount} 次...`);

    // 等待 1 秒後再嘗試
    await new Promise((resolve) => setTimeout(resolve, 1000));

    // 返回重試的請求
    return axiosInstance(config);
  },
);

export default axiosInstance;
