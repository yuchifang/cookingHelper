import axios from "axios";

const axiosInstance = axios.create();
const MAX_RETRIES = 2;
const ApiRetryList = ["api/applog/getLogList"];

axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const config = error.config;
    const APIString = config.url;
    let isRetryApi = false;

    for (const item of ApiRetryList) {
      if (item.indexOf(APIString) != -1) {
        isRetryApi = true;
        break;
      }
    }

    // 如果沒有 config，或不需要重試，直接拋出錯誤
    if (!config || config.retryCount >= MAX_RETRIES || !isRetryApi) {
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
    await new Promise((resolve) => setTimeout(resolve, 3000));

    // 返回重試的請求
    return axiosInstance(config);
  },
);

export default axiosInstance;
