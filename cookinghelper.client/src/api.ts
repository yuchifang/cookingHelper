export async function getBarChart({
  startUtcZSecondTimestamp,
  endUtcZSecondTimestamp,
  dateUnit,
}: {
  startUtcZSecondTimestamp: number;
  endUtcZSecondTimestamp: number;
  dateUnit?: string;
}) {
  try {
    const response = await fetch(
      `api/applog/getLogList?startTime=${startUtcZSecondTimestamp}&endTime=${endUtcZSecondTimestamp}&dateUnit=${dateUnit}`,
      {
        method: "GET",
      },
    );

    return response;
  } catch (error) {
    console.log(error);
    throw error;
  }
}
