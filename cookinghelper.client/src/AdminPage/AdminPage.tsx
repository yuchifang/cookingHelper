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
AdminPage.displayName = "AdminPage";
export function AdminPage() {
  const tabList = [<Tab label="系統分析" sx={{ fontSize: "25px" }} />];
  const [value, setValue] = useState(0);
  const data: Status = useLoaderData() as Status;
  const navigate = useNavigate();

  if (data && data?.permission == "admin") {
    tabList.push(<Tab label="帳號管理" sx={{ fontSize: "25px" }} />);
  }

  const handleChange = (_event: React.SyntheticEvent, newValue: number) => {
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
