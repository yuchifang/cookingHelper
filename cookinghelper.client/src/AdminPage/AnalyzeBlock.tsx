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
  ResponsiveContainer,
} from "recharts";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import DateUnitButton from "./DateUnitButton";
import { useEffect, useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import Alert from "@mui/material/Alert";

export interface Log {
  id: number;
  logTime: number;
  userId: string;
}
//! 怎麼上到 azure
//! check azure cost

//! todo dayJs Ms?
export default function AnalyzeBlock({ loader }: { loader: Log[] }) {
  const [startTime, setStartTime] = useState<Dayjs | null>(dayjs("2022-04-10"));
  const [endTime, setEndTime] = useState<Dayjs | null>(dayjs("2022-04-17"));
  const [dateRangeHasError, setDateRangeHasError] = useState(false);
  const msStartTime = startTime!.valueOf();
  const msEndTime = endTime!.valueOf();

  useEffect(() => {
    if (msStartTime >= msEndTime) {
      setDateRangeHasError(true);
    } else {
      setDateRangeHasError(false);
    }
  }, [endTime, startTime]);

  console.log({ startTime });
  console.log({ endTime });

  console.log({ loader });
  // todo 建立假資料
  // todo 用 SQL

  return (
    <AnalyzeContainer>
      <DateRangeBlock>
        {dateRangeHasError && (
          <Alert
            style={{
              marginRight: "15px",
            }}
            severity="error"
          >
            結束時間早於起始時間
          </Alert>
        )}
        <DateBlock>
          <DateText>選擇起始日期</DateText>
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DatePicker
              label="起始日期"
              value={startTime}
              onChange={(newValue) => setStartTime(newValue)}
            />
          </LocalizationProvider>
        </DateBlock>
        <DateBlock>
          <DateText>選擇結束日期</DateText>
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DatePicker
              value={endTime}
              onChange={(newValue) => setEndTime(newValue)}
              label="結束日期"
            />
          </LocalizationProvider>
        </DateBlock>
      </DateRangeBlock>
      <BarChartBlock>
        <ResponsiveContainer
          style={{ marginBottom: "15px" }}
          width={"100%"}
          height={300}
        >
          <BarChart
            data={loader}
            margin={{
              top: 5,
              right: 30,
              left: 20,
              bottom: 5,
            }}
          >
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="date" />
            <YAxis />
            <Tooltip />
            <Bar dataKey="count" fill="#B3CDAD" activeBar={false} />
          </BarChart>
        </ResponsiveContainer>
        <XAxisDateUnit>月</XAxisDateUnit>
        <YAxisFrequencyUnit>次數</YAxisFrequencyUnit>
      </BarChartBlock>
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
  alignItems: "center",
}));

const DateText = styled.p`
  font-size: 22px;
  line-height: 24px;
  margin-right: 10px;
  box-sizing: border-box;
  display: inline-block;
`;

const DateBlock = styled.div`
  margin-right: 10px;
  display: flex;
  align-items: center;
`;

const DateUnitBlock = muiStyled(Box)(() => ({
  display: "flex",
}));

const XAxisDateUnit = styled.p`
  position: absolute;
  bottom: 15px;
  right: 15px;
  font-size: 25px;
  font-weight: bold;
`;
const YAxisFrequencyUnit = styled.p`
  position: absolute;
  top: -35px;
  left: 30px;
  font-size: 25px;
  font-weight: bold;
`;
const BarChartBlock = styled.div`
  position: relative;
  width: 100%;
  .recharts-tooltip-cursor {
    visibility: hidden;
  }
`;
