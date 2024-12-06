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
import { getBarChart } from "../api";
import { AxiosError } from "axios";
import Snackbar from "@mui/material/Snackbar";

interface Log {
  count: number;
  date: string;
}

export default function AnalyzeBlock() {
  const [startTime, setStartTime] = useState<Dayjs | null>(
    dayjs.unix(Math.floor(Date.now() / 1000) - 60 * 60 * 24 * 7),
  );
  const [endTime, setEndTime] = useState<Dayjs | null>(
    dayjs.unix(Math.floor(Date.now() / 1000)),
  );
  const [dateUnit, setDateUnit] = useState<"day" | "month" | "year">("day");
  const [dateRangeHasError, setDateRangeHasError] = useState(false);
  const [overLimit, setOverLimit] = useState(false);
  const [barChart, setBarChart] = useState<Log[] | null>(null);
  const [error, setError] = useState<string>("");

  const dateUnitDisplay =
    dateUnit == "day" ? "日" : dateUnit == "month" ? "月" : "年";

  const handleClose = () => {
    setError("");
  };

  useEffect(() => {
    const secondStartTime = startTime!.unix();
    const secondEndTime = endTime!.unix();

    async function runAsync() {
      try {
        const response = await getBarChart({
          startUtcZSecondTimestamp: secondStartTime,
          endUtcZSecondTimestamp: secondEndTime,
          dateUnit: dateUnit,
        });
        setBarChart(response.data.barChartData);
        setOverLimit(response.data.overLimit);
      } catch (error) {
        const err = error as AxiosError<Error>;
        err.message && setError(err.message);
      }
    }
    if (secondStartTime >= secondEndTime) {
      setDateRangeHasError(true);
    } else {
      setDateRangeHasError(false);
      runAsync();
    }
  }, [endTime, startTime, dateUnit]);

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
        {overLimit && (
          <Alert
            style={{
              marginRight: "15px",
            }}
            severity="error"
          >
            資料筆數超過100筆,會調整時間單位呈現
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
        {barChart && (
          <>
            <ResponsiveContainer
              style={{ marginBottom: "15px" }}
              width={"100%"}
              height={300}
            >
              <BarChart
                data={barChart!}
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
            <XAxisDateUnit>{dateUnitDisplay}</XAxisDateUnit>
            <YAxisFrequencyUnit>次數</YAxisFrequencyUnit>
          </>
        )}
      </BarChartBlock>
      <DateUnitBlock>
        <DateUnitButton setDateUnit={setDateUnit} />
      </DateUnitBlock>
      <Snackbar
        open={error != ""}
        autoHideDuration={3000}
        message={error}
        onClose={handleClose}
      />
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
  bottom: 2px;
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
  margin-bottom: 5px;
  min-height: 315px;
  .recharts-tooltip-cursor {
    visibility: hidden;
  }
`;
