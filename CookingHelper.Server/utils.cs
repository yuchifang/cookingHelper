using CookingHelper.LineDtoService;

namespace CookingHelper;

public class Utils
{
    public static readonly char[] _colon = { ':', '：' };

    public static void StringToStorageInfo(
        string inputText,
        out StorageInfo returnObject,
        out string ErrorText
    )
    {
        // 將使用者輸入的字串, 依"/"切分成每個欄位, 依每個欄位":" 切分成Key, Value, 並驗證日期格式,
        // 格式正確則回傳更新的資訊, 格式錯誤 回傳錯誤資訊
        ErrorText = "";
        returnObject = new StorageInfo();
        var UserTextFieldArray = inputText.Split("/", StringSplitOptions.RemoveEmptyEntries);
        var TDList = new List<List<string>>();

        try
        {
            foreach (var item in UserTextFieldArray)
            {
                var ValuePairArray = item.Split(_colon, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToArray();

                var ExamineKey = ValuePairArray[0];

                if (ExamineKey == "購買日期" || ExamineKey == "有效日期")
                {
                    if (DateOnly.TryParseExact(ValuePairArray[1], "yyyyMMdd", out DateOnly Date))
                    {
                        TDList.Add(ValuePairArray.ToList());
                    }
                    else
                    {
                        ErrorText = $"{ValuePairArray[0]}, {ValuePairArray[1]}";
                        break;
                    }
                }

                if (ExamineKey == "物品名稱" || ExamineKey == "儲存位置")
                {
                    if (ValuePairArray[1].Trim() == "")
                    {
                        ErrorText = $"{ValuePairArray[0]}, {ValuePairArray[1]}";
                        break;
                    }
                    else
                    {
                        TDList.Add(ValuePairArray.ToList());
                    }
                }

                if (
                    (
                        Array.Find(
                            StorageManagementKeywordGroup.ExamineArray,
                            Key => Key == ExamineKey
                        )
                    ) != null
                )
                {
                    if (
                        ExamineKey == "購買日期"
                        || ExamineKey == "有效日期"
                        || ExamineKey == "物品名稱"
                        || ExamineKey == "儲存位置"
                    )
                    {
                        continue;
                    }

                    TDList.Add(ValuePairArray.ToList());
                }
                else
                {
                    ErrorText = $"{ValuePairArray[0]}, {ValuePairArray[1]}";
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        if (ErrorText == "")
        {
            foreach (var KeyValueList in TDList)
            {
                switch (KeyValueList[0])
                {
                    case StorageManagementKeywordGroup.Place:
                        returnObject.Place = KeyValueList[1];
                        break;
                    case StorageManagementKeywordGroup.Name:
                        returnObject.Name = KeyValueList[1];
                        break;
                    case StorageManagementKeywordGroup.Location:
                        returnObject.Location = KeyValueList[1];
                        break;
                    case StorageManagementKeywordGroup.Amount:
                        returnObject.Amount = KeyValueList[1];
                        break;
                    case StorageManagementKeywordGroup.PurchaseDate:
                        DateOnly PurchaseDate = DateOnly.ParseExact(KeyValueList[1], "yyyyMMdd");
                        returnObject.PurchaseDate = PurchaseDate;
                        break;
                    case StorageManagementKeywordGroup.ExpiryDate:
                        DateOnly ExpiryDate = DateOnly.ParseExact(KeyValueList[1], "yyyyMMdd");
                        returnObject.ExpiryDate = ExpiryDate;
                        break;
                }
            }
        }
    }

    public static string DateOnlyToString(DateOnly dateOnly, string? format)
    {
        if (format == null)
        {
            format = "yyyy-MM-dd";
        }

        return dateOnly.ToDateTime(new TimeOnly(0, 0)).ToString(format);
    }

    public class CustomComparer : IComparer<DateOnly?>
    {
        public int Compare(DateOnly? x, DateOnly? y)
        {
            if (x == null)
            {
                return 1;
            }
            else if (y == null)
            {
                return -1;
            }

            if (x < y)
            {
                return 1;
            }
            else if (x > y)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
