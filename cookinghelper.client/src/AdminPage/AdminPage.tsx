import { useLoaderData, useLocation } from "react-router-dom";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import Box from "@mui/material/Box";
import { useState } from "react";
import { styled } from "@mui/system";
import Button from "@mui/material/Button";
import AnalyzeBlock, { Log } from "./AnalyzeBlock";
interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

/*
  ? 前後端做 Trim
  fangfelipe@gmail.com
  123456
  admin

  test@gmail.com
  123456
  guest
*/

/*
  todo 分析 page
  ? 建立 loader fetch 資料
  ? 印象 loader react-router-dom 有些東西要釐清
  todo 調整一個參數 觸發 Form? 或是觸發 state 透過 rerender 觸發 loader?

  ?? X軸的值超過100個 想辦法計算成100個
  使用 recharts 完成圖表
  使用 https://codesandbox.io/p/sandbox/simple-bar-chart-72d7y5?file=%2Fsrc%2FApp.tsx

  ?? 用 react router dom 的 state 判斷登入狀態安全嗎
  ? //? 帳號判斷要用 session 嗎?  chatgpt


  todo 建立帳號的 page
  ?成功創建帳號 開提示
    https://mui.com/material-ui/react-snackbar/
  
  ?右上角登出 

  ? 登入, 註冊 加入 loading 

  ?忘記密碼
    email 後端也寫

  
  
*/

// Y軸 次數=> 在當天有使用的人數
// X軸 時間=>
// 操作不同功能呈現不同圖表
//! 進行 dotnet migration
//! 紀錄 HTTP GET
//? 在這邊預設時間
// export async function loader() {
//   // ?先設定預設時間 現在, 到前7天
//   // 先把 UTC+8的時間轉成 UTC時間, 再轉成msTimeStamp

//   const startUTCZTimestamp = Date.parse(new Date(Date.now()).toISOString());
//   const endUTCZTimestamp = Date.parse(
//     new Date(Date.now() - 1000 * 60 * 60 * 24 * 7).toISOString(),
//   );

//   const response = await fetch(
//     `api/applog/getLogList?startTime=${startUTCZTimestamp}&endTime=${endUTCZTimestamp}`,
//     {
//       method: "GET",
//     },
//   );
//   const responseData = await response.json();
//   return responseData;
// }

export function AdminPage() {
  const tabList = [<Tab label="系統分析" sx={{ fontSize: "25px" }} />];
  const [value, setValue] = useState(0);
  const loader: Log[] = useLoaderData() as Log[];
  console.log(loader);
  const location = useLocation();
  let userInfo;
  if ("userInfo" in location.state) {
    userInfo = location.state.userInfo;
  } else {
    throw new Error("AdminPage locationData error");
  }

  if (userInfo.permission == "admin") {
    tabList.push(<Tab label="帳號管理" sx={{ fontSize: "25px" }} />);
  }

  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  return (
    <AdminPageContainer>
      <HeaderContainer>
        <Tabs value={value} onChange={handleChange}>
          {tabList}
        </Tabs>
        <Button variant="outlined" sx={{ fontSize: "23px" }}>
          登出
        </Button>
      </HeaderContainer>
      <CustomTabPanel value={value} index={0}>
        <AnalyzeBlock loader={loader} />
        {/* <AnalyzeBlock /> */}
      </CustomTabPanel>
      <CustomTabPanel value={value} index={1}>
        帳號
      </CustomTabPanel>
    </AdminPageContainer>
  );
}

function CustomTabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div hidden={value !== index} {...other}>
      {value === index && <Box sx={{ p: 3, color: "black" }}>{children}</Box>}
    </div>
  );
}

const AdminPageContainer = styled(Box)(() => ({
  minWidth: "1360px",
  width: "100%",
  height: "100%",
}));

const HeaderContainer = styled(Box)(() => ({
  display: "flex",
  justifyContent: "space-between",
}));
