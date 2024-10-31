import { useLocation } from "react-router-dom";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import Box from "@mui/material/Box";
import { useState } from "react";
import { styled } from "@mui/material/styles";
interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

// 建立 styled page

export function AdminPage() {
  const [value, setValue] = useState(0);

  const location = useLocation();
  console.log("location", location);
  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  return (
    <AdminPageContainer>
      <Box>
        <Tabs value={value} onChange={handleChange}>
          <Tab label="使用分析" sx={{ fontSize: "25px" }} />
          <Tab label="帳號管理" sx={{ fontSize: "25px" }} />
        </Tabs>
      </Box>
      <CustomTabPanel value={value} index={0}>
        分析
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
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

const AdminPageContainer = styled(Box)(() => ({
  minWidth: "1360px",
  width: "100%",
  height: "100%",
}));
