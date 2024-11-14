import { useLocation } from "react-router-dom";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import Box from "@mui/material/Box";
import { useState } from "react";
import { styled } from "@mui/system";
import Button from "@mui/material/Button";
import AnalyzeBlock from "./AnalyzeBlock";
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
  
  todo 並看看 loader 狀況
  todo 傳資料給 loader ??
  todo 把目前的 router 建立 page


  todo here 建立圖表 用假資料
  

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

// todo 把 前端的更新時間, 單位 接上 api
// todo 確認 loader 呼叫時機

//! 紀錄 HTTP GET
//? 在這邊預設時間

export function AdminPage() {
  const tabList = [<Tab label="系統分析" sx={{ fontSize: "25px" }} />];
  const [value, setValue] = useState(0);

  const location = useLocation();
  console.log({ location });
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

  //! 這邊的 tabs 要改成 Route 的寫法??
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
        <AnalyzeBlock />
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
