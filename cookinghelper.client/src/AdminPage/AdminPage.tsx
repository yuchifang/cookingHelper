import { useLocation } from "react-router-dom";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import Box from "@mui/material/Box";
import { useState } from "react";
import { styled } from "@mui/material/styles";
import Button from "@mui/material/Button";
interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}
/*
  fangfelipe@gmail.com
  123456
  admin

  test@gmail.com
  123456
  guest
*/

/*
  ? //? 帳號判斷要用 session 嗎?  chatgpt
  ? 分頁上的 icon

  ?成功創建帳號 開提示
    https://mui.com/material-ui/react-snackbar/
  
  ?右上角登出 
*/

export function AdminPage() {
  const tabList = [<Tab label="使用分析" sx={{ fontSize: "25px" }} />];
  const [value, setValue] = useState(0);

  const location = useLocation();
  let userInfo;
  if ("userInfo" in location.state) {
    userInfo = location.state.userInfo;
  } else {
    throw new Error("AdminPage locationData error");
  }

  console.log("locationData", userInfo);

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
        分析
      </CustomTabPanel>
      <CustomTabPanel value={value} index={1}>
        帳號
      </CustomTabPanel>
    </AdminPageContainer>
  );
}
// 使用 recharts 完成圖表
// 使用 https://codesandbox.io/p/sandbox/simple-bar-chart-72d7y5?file=%2Fsrc%2FApp.tsx

// Y軸 次數=> 在當天有使用的人數
// X軸 時間=>
// 操作不同功能呈現不同圖表
//? 在 server 建立 middleware 看看可不可以取的使用者的 ID 並記錄
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
