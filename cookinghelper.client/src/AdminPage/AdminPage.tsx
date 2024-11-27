import { redirect, useLoaderData, useNavigate } from "react-router-dom";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import Box from "@mui/material/Box";
import { useState } from "react";
import { styled } from "@mui/system";
import Button from "@mui/material/Button";
import AnalyzeBlock from "./AnalyzeBlock";

import AccountBlock from "./AccountBlock";
import axiosInterceptor from "../axiosInterceptor";

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
  ! 測試如果斷網要怎麼處理,如果網頁斷網 axios 要怎麼處理正在執行的 api
  ! axios 攔截器 retry 斷網?? 

  
  todo loader 觸發時機  只有在換頁或是action 呼叫時在會觸發(action 呼叫時在會觸發?
      action 觸發的時機
  todo 用 SQL 產生更多假資料 使用者登入的假資料, 及line 服務的資料
  todo 了解 http cookie and Session
  todo 忘記密碼
      email 後端也寫  
  ! axios 攔截器, 斷網處理 重新命名
*/
export async function loader() {
  const response = await axiosInterceptor.get("/api/AccountIdentity/status");
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
  const navigate = useNavigate();

  if (data && data?.permission == "admin") {
    tabList.push(<Tab label="帳號管理" sx={{ fontSize: "25px" }} />);
  }

  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  const handleClick = async () => {
    await axiosInterceptor.post("/api/AccountIdentity/logout");
    navigate("/", { replace: true });
  };

  return (
    <AdminPageContainer>
      <HeaderContainer>
        <Tabs value={value} onChange={handleChange}>
          {tabList}
        </Tabs>
        <Button
          variant="outlined"
          sx={{ fontSize: "23px" }}
          onClick={handleClick}
        >
          登出
        </Button>
      </HeaderContainer>
      <CustomTabPanel value={value} index={0}>
        <AnalyzeBlock />
      </CustomTabPanel>
      <CustomTabPanel value={value} index={1}>
        <AccountBlock />
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
