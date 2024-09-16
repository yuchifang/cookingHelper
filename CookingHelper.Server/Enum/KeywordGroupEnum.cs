public struct KeywordGroup
{
    // 採買清單
    public const string InputPurchaseList = "輸入採買清單";
    public const string PurchaseList = "採買清單";

    // 食譜清單
    public const string RecipeList = "食譜清單";
    public const string RecipeListAddition = "新增食譜";

    public const string RecipeListSearch = "食譜查詢";

    // 意見反饋
    public const string Feedback = "意見反饋";
    public const string InputFeedback = "輸入意見反饋";

    // 庫存管理
    public const string StorageManagement = "庫存管理";

    public const string StorageManagementAdded = "新增物品至庫存";

    public const string StorageManagementSearch = "庫存查詢";
}

public struct RecipeKeywordGroup
{
    public const string Name = "食譜名稱";
    public const string Ingredients = "食材";
    public const string Step = "步驟";
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
