import { redirect, useLoaderData } from "react-router-dom";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import Box from "@mui/material/Box";
import { useState } from "react";
import { styled } from "@mui/system";
import Button from "@mui/material/Button";
import AnalyzeBlock from "./AnalyzeBlock";
import axios from "axios";
interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}
/*
    ? fetcher.formdata 使用方式
    ? loader 觸發時機  只有在換頁或是action 呼叫時在會觸發(action 呼叫時在會觸發?
    <Form method="post" action="/songs" />;  action 為送到哪個路由的 action?
*/

/*
  {
  "email": "fangfelipe@gmail.com",
  "password": "123456",
  "permission": "admin",
  "username": "fangfelipe@gmail.com"
}
*/

/*
  ! 怎麼上到 azure
  ! check azure cost
  ! 正式機 也要 dotnet ef migrations add someThing
  

  todo 把 圖表完成 後端
  ? loader 觸發時機  只有在換頁或是action 呼叫時在會觸發(action 呼叫時在會觸發?
  todo 用 SQL 產生更多假資料

  ?? 用 react router dom 的 state 判斷登入狀態安全嗎
  todo here
  ? //? 帳號判斷要用 session 嗎?  chatgpt


  todo 建立帳號的 page
  ?成功創建帳號 開提示
    https://mui.com/material-ui/react-snackbar/
  
  ?右上角登出 

  ? 登入, 註冊 加入 loading 

  ?忘記密碼
    email 後端也寫  
*/
export async function loader() {
  const response = await axios.get("/api/AccountIdentity/status");
  if (response.data.isAuthenticated) {
    const data = await response.data;
    return data;
  } else {
    return redirect("/");
  }
}

interface Status {
  isAuthenticated: boolean;
  username: string;
  permission: string;
}

export function AdminPage() {
  const tabList = [<Tab label="系統分析" sx={{ fontSize: "25px" }} />];
  const [value, setValue] = useState(0);
  const data: Status = useLoaderData() as Status;

  if (data && data?.permission == "admin") {
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
