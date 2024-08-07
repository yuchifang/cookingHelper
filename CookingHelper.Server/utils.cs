using CookingHelper.LineDtoService;

namespace CookingHelper;

public class Utils
{
    public static readonly char[] _colon = { ':', '：' };
    public static readonly char[] _tilde = { '~' };

    static IEnumerable<int> GenerateRange(int inputNumberFirst, int inputNumberSecond)
    {
        int start,
            end;
        if (inputNumberFirst > inputNumberSecond)
        {
            start = inputNumberFirst;
            end = inputNumberSecond;
        }
        else
        {
            end = inputNumberFirst;
            start = inputNumberSecond;
        }
        // 计算要生成的整数数量
        int count = end - start + 1;

        // 使用 Enumerable.Range 生成整数序列
        return Enumerable.Range(start, count);
    }

    //  1/5/2
    //  1~3/7/8
    //  2~4/3/9
    //  2~2/3/9
    //  4~3/3/9
    public static void StringSlashAndTildeToStorageInfo(
        string inputText,
        out List<int> ListInt,
        out string ErrorText
    )
    {
        //!! 整理判斷是不是正整數
        //!! 處理重複的
        //!! 數字存不存在 List<int>
        ErrorText = "";
        ListInt = new List<int>();
        var UserTextDeleteNumberArray = inputText.Split("/", StringSplitOptions.RemoveEmptyEntries);
        if (UserTextDeleteNumberArray.Count() == 0)
        {
            ErrorText = "輸入錯誤";
        }
        foreach (var text in UserTextDeleteNumberArray)
        {
            if (text.Count() == 3)
            {
                var stringArray = text.Split(_tilde, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToArray();

                if (
                    int.TryParse(stringArray[0], out int numberFirst)
                    && numberFirst > 0
                    && int.TryParse(stringArray[1], out int numberSecond)
                    && numberSecond > 0
                )
                {
                    if (numberSecond == numberFirst)
                    {
                        ListInt.Add(numberFirst);
                    }
                    else
                    {
                        ListInt.AddRange(GenerateRange(numberFirst, numberSecond));
                    }
                }
                else
                {
                    ErrorText += $"/{stringArray[0]},{stringArray[1]} 此項目輸入錯誤";
                    return;
                }
            }
            else if (text.Count() == 1)
            {
                if (int.TryParse(text, out int number) && number > 0)
                {
                    ListInt.Add(number);
                }
                else
                {
                    ErrorText += $"/{text} 此項目輸入錯誤";
                    return;
                }
            }
            else
            {
                ErrorText += $"/{text} 此項目輸入錯誤";
                return;
            }
        }
        ListInt = ListInt.Distinct().ToList();
    }

    public static void StringSlashAndColonToStorageInfo(
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
                    if (ValuePairArray.Length == 2)
                    {
                        ErrorText = $"{ValuePairArray[0]}, {ValuePairArray[1]}";
                    }
                    else
                    {
                        ErrorText = $"{ValuePairArray[0]}";
                    }
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return;
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

    public static bool ClassMatch<Class>(Class item, Class target)
    {
        foreach (var prop in typeof(Class).GetProperties())
        {
            var targetValue = prop.GetValue(target);
            if (targetValue != null)
            {
                var itemValue = prop.GetValue(item);
                if (!targetValue.Equals(itemValue))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static IQueryable<T> Paginate<T>(
        IQueryable<T> source,
        int pageIndex,
        int pageSize,
        out bool hasNextPage,
        out bool hasPrevPage
    )
    {
        hasPrevPage = false;
        hasNextPage = false;
        var count = source.Count();
        var totalPage = (int)Math.Ceiling(count / (double)pageSize);
        if (pageIndex > 1)
        {
            hasPrevPage = true;
        }
        if (totalPage > pageIndex)
        {
            hasNextPage = true;
        }

        return source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
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
