public struct KeywordGroup
{
    // 採買清單
    public const string InputPurchaseList = "輸入採買清單";
    public const string PurchaseList = "採買清單";

    // 食譜清單 //? 可以用 FlexMessage sample 來顯示搜尋結果
    public const string MenuList = "食譜清單";

    // 意見反饋
    public const string Feedback = "意見反饋";
    public const string InputFeedback = "輸入意見反饋";

    // 庫存管理
    public const string StorageManagement = "庫存管理";
}

public struct StorageManagementKeywordGroup
{
    public const string Place = "儲存位置";
    public const string Name = "物品名稱";
    public const string Location = "詳細位置";
    public const string Amount = "數量";

    public const string PurchaseDate = "購買日期";

    public const string ExpiryDate = "有效日期";
    public static string[] ExamineArray = ["購買日期", "數量", "有效日期", "儲存位置", "物品名稱", "詳細位置"];
}
