import Box from "@mui/material/Box";

import { styled as muiStyled } from "@mui/system";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import styled from "styled-components";

import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Rectangle,
  ResponsiveContainer,
} from "recharts";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import DateUnitButton from "./DateUnitButton";

export interface Log {
  id: number;
  logTime: number;
  userId: string;
}
//! 怎麼上到 azure
//! check azure cost
export default function AnalyzeBlock({ loader }: { loader: Log[] }) {
  console.log(loader);

  const data = [
    {
      name: "Page A",
      uv: 4000,
      pv: 2400,
      amt: 2400,
    },
    {
      name: "Page B",
      uv: 3000,
      pv: 1398,
      amt: 2210,
    },
    {
      name: "Page C",
      uv: 2000,
      pv: 9800,
      amt: 2290,
    },
    {
      name: "Page D",
      uv: 2780,
      pv: 3908,
      amt: 2000,
    },
    {
      name: "Page E",
      uv: 1890,
      pv: 4800,
      amt: 2181,
    },
    {
      name: "Page F",
      uv: 2390,
      pv: 3800,
      amt: 2500,
    },
    {
      name: "Page G",
      uv: 3490,
      pv: 4300,
      amt: 2100,
    },
  ];
  return (
    <AnalyzeContainer>
      <DateRangeBlock>
        <DateBlock>
          <DateText>選擇起始日期</DateText>
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DatePicker
              label="起始日期"
              slotProps={{
                textField: {
                  helperText: "MM/DD/YYYY",
                  color: "error",
                },
              }}
            />
          </LocalizationProvider>
        </DateBlock>
        <DateBlock>
          <DateText>選擇結束日期</DateText>
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DatePicker
              label="結束日期"
              slotProps={{
                textField: {
                  helperText: "MM/DD/YYYY",
                  color: "error",
                },
              }}
            />
          </LocalizationProvider>
        </DateBlock>
      </DateRangeBlock>
      <ResponsiveContainer width={"100%"} height={300}>
        <BarChart
          data={data}
          margin={{
            top: 5,
            right: 30,
            left: 20,
            bottom: 5,
          }}
        >
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="name" />
          <YAxis />
          <Tooltip />
          <Bar
            dataKey="uv"
            fill="#B3CDAD"
            activeBar={<Rectangle fill="pink" stroke="blue" />}
          />
          <Bar
            dataKey="pv"
            fill="#FF5F5E"
            activeBar={<Rectangle fill="gold" stroke="purple" />}
          />
        </BarChart>
      </ResponsiveContainer>
      <DateUnitBlock>
        <DateUnitButton />
      </DateUnitBlock>
    </AnalyzeContainer>
  );
}

const AnalyzeContainer = muiStyled(Box)(() => ({
  display: "flex",
  flexDirection: "column",
  position: "relative",
  padding: "50px",
  alignItems: "end",
}));

const DateRangeBlock = muiStyled(Box)(() => ({
  display: "flex",
  margin: "0 0 25px 0",
}));

const DateText = styled.p`
  font-size: 22px;
  line-height: 24px;
  margin: 17px 8px 0 0;
  box-sizing: border-box;
  display: inline-block;
`;

const DateBlock = styled.div`
  margin-right: 10px;
`;

const DateUnitBlock = muiStyled(Box)(() => ({
  display: "flex",
}));
//! todo Dropdown 獨立一個 Component 整個調整頁面高度
