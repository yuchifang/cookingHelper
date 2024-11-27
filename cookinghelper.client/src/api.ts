import axios from "axios";

export async function getBarChart({
  startUtcZSecondTimestamp,
  endUtcZSecondTimestamp,
  dateUnit,
}: {
  startUtcZSecondTimestamp: number;
  endUtcZSecondTimestamp: number;
  dateUnit?: string;
}) {
  const response = await axios.get(
    `api/applog/getLogList?startTime=${startUtcZSecondTimestamp}&endTime=${endUtcZSecondTimestamp}&dateUnit=${dateUnit}`,
  );

  return response;
}
