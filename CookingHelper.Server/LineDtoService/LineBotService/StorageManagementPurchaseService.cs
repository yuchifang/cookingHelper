using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

/*

*/
//! 寫完要更新 Database migrations
//! interface?? 宣告的
//! abstract 功用
//! virtual
//! override
//! 一功用整理 筆記
//! 看看 DbContext 寫法
//! DI 注入?? 整理一篇
//! Dictionary??
//! Func<>

//? interface 可以換成 abstract?

public class StorageManagementPurchaseService
{
    private readonly StorageManagementService _storageManagementService;

    public StorageManagementPurchaseService(StorageManagementService StorageManagementService)
    {
        _storageManagementService = StorageManagementService;
    }

    public static InputStorageInfo _InputStorageInfoStatic = new InputStorageInfo();

    public static dynamic _ReplyMessageListStatic = new List<object>();

    class StatusInitClass : Status
    {
        public override void Init()
        {
            LineBotService._WebhookEventStateStatic = "新增物品至庫存";
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject { Text = "依儲存位置,物品名稱,詳細位置,購買日期,過期日期輸入", },
                    new TextMessageObject { Text = "儲存位置及物品名稱一定要填入, 沒填入將無法紀錄", },
                    new TextMessageObject
                    {
                        Text = "請輸入儲存位置:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                GetCancelAdditionQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "place";
        }
    }

    class StatusPlaceClass : Status
    {
        private readonly string _WebHookEventMessage;

        public StatusPlaceClass(string WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            _InputStorageInfoStatic.Place = _WebHookEventMessage;
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject { Text = "此欄位為必填, 沒填入將無法紀錄", },
                    new TextMessageObject
                    {
                        Text = "請輸入物品名稱:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                GetCancelAdditionQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "name";
        }
    }

    class StatusNameClass : Status
    {
        private readonly string _WebHookEventMessage;

        public StatusNameClass(string WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            _InputStorageInfoStatic.Name = _WebHookEventMessage;
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入數量:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                GetCancelAdditionQuickReplyButton(),
                                GetSkipQuickReplyButton(),
                                GetAdditionCompleteQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "amount";
        }
    }

    class StatusAmountClass : Status
    {
        private readonly string? _WebHookEventMessage;

        public StatusAmountClass(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            _InputStorageInfoStatic.Amount = _WebHookEventMessage;
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入詳細位置:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                GetCancelAdditionQuickReplyButton(),
                                GetSkipQuickReplyButton(),
                                GetAdditionCompleteQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "location";
        }
    }

    class StatusLocationClass : Status
    {
        private readonly string? _WebHookEventMessage;

        public StatusLocationClass(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            _InputStorageInfoStatic.Location = _WebHookEventMessage;
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入購買日期(格式: YYYYMMDD):",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                GetCancelAdditionQuickReplyButton(),
                                GetSkipQuickReplyButton(),
                                GetAdditionCompleteQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "purchaseDate";
        }
    }

    class StatusPurchaseDateClass : Status
    {
        private readonly string? _WebHookEventMessage;

        public StatusPurchaseDateClass(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            var dateString = _WebHookEventMessage;

            if (dateString == null)
            {
                _InputStorageInfoStatic.PurchaseDate = null;
                _ReplyMessageListStatic = new List<object>(
                    [
                        new TextMessageObject
                        {
                            Text = "請輸入過期日期(格式: YYYYMMDD):",
                            QuickReply = new QuickReplyItemDto
                            {
                                Items = new List<QuickReplyButtonDto>
                                {
                                    GetCancelAdditionQuickReplyButton(),
                                    GetSkipQuickReplyButton(),
                                    GetAdditionCompleteQuickReplyButton()
                                }
                            }
                        }
                    ]
                );
                _InputStorageInfoStatic.Status = "expiryDate";
            }
            else if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly PurchaseDate))
            {
                _InputStorageInfoStatic.PurchaseDate = PurchaseDate;
                _ReplyMessageListStatic = new List<object>(
                    [
                        new TextMessageObject
                        {
                            Text = "請輸入過期日期(格式: YYYYMMDD):",
                            QuickReply = new QuickReplyItemDto
                            {
                                Items = new List<QuickReplyButtonDto>
                                {
                                    GetCancelAdditionQuickReplyButton(),
                                    GetSkipQuickReplyButton(),
                                    GetAdditionCompleteQuickReplyButton()
                                }
                            }
                        }
                    ]
                );
                _InputStorageInfoStatic.Status = "expiryDate";
            }
            else
            {
                _ReplyMessageListStatic = DateTypeErrorHint("$ 購買日期格式錯誤, 請重新輸入(格式: YYYYMMDD)");
            }
        }
    }

    class StatusExpiryDateClass : Status
    {
        private readonly string? _WebHookEventMessage;

        public StatusExpiryDateClass(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            var dateString = _WebHookEventMessage;
            if (dateString == null)
            {
                _InputStorageInfoStatic.ExpiryDate = null;
                var NameField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Name,
                    _InputStorageInfoStatic.Name
                );
                var AmountField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Amount,
                    _InputStorageInfoStatic.Amount
                );
                var LocationField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Location,
                    _InputStorageInfoStatic.Location
                );
                FlexComponent? PurchaseDateField;
                if (_InputStorageInfoStatic.PurchaseDate != null)
                {
                    string customFormat = "yyyy-MM-dd";
                    string PurchaseDateString = _InputStorageInfoStatic
                        .PurchaseDate.Value.ToDateTime(new TimeOnly(0, 0))
                        .ToString(customFormat);

                    PurchaseDateField = FieldFlexComponent(
                        StorageManagementKeywordGroup.PurchaseDate,
                        PurchaseDateString
                    );
                }
                else
                {
                    PurchaseDateField = null;
                }
                FlexComponent? ExpiryDateField;
                if (_InputStorageInfoStatic.ExpiryDate != null)
                {
                    string customFormat = "yyyy-MM-dd";
                    string ExpiryDateString = _InputStorageInfoStatic
                        .ExpiryDate.Value.ToDateTime(new TimeOnly(0, 0))
                        .ToString(customFormat);

                    ExpiryDateField = FieldFlexComponent(
                        StorageManagementKeywordGroup.ExpiryDate,
                        ExpiryDateString
                    );
                }
                else
                {
                    ExpiryDateField = null;
                }

                List<FlexComponent> FieldTable = new List<FlexComponent> { };

                if (NameField != null)
                    FieldTable.Add(NameField);
                if (AmountField != null)
                    FieldTable.Add(AmountField);
                if (LocationField != null)
                    FieldTable.Add(LocationField);
                if (PurchaseDateField != null)
                    FieldTable.Add(PurchaseDateField);
                if (ExpiryDateField != null)
                    FieldTable.Add(ExpiryDateField);
                _ReplyMessageListStatic = new List<object>
                {
                    GetBubbleFlexMessageObject(_InputStorageInfoStatic.Place, FieldTable)
                };
            }
            else if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly ExpiryDate))
            {
                _InputStorageInfoStatic.ExpiryDate = ExpiryDate;
                var NameField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Name,
                    _InputStorageInfoStatic.Name
                );
                var AmountField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Amount,
                    _InputStorageInfoStatic.Amount
                );
                var LocationField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Location,
                    _InputStorageInfoStatic.Location
                );
                FlexComponent? PurchaseDateField;
                if (_InputStorageInfoStatic.PurchaseDate != null)
                {
                    string customFormat = "yyyy-MM-dd";
                    string PurchaseDateString = _InputStorageInfoStatic
                        .PurchaseDate.Value.ToDateTime(new TimeOnly(0, 0))
                        .ToString(customFormat);

                    PurchaseDateField = FieldFlexComponent(
                        StorageManagementKeywordGroup.PurchaseDate,
                        PurchaseDateString
                    );
                }
                else
                {
                    PurchaseDateField = null;
                }
                FlexComponent? ExpiryDateField;
                if (_InputStorageInfoStatic.ExpiryDate != null)
                {
                    string customFormat = "yyyy-MM-dd";
                    string ExpiryDateString = _InputStorageInfoStatic
                        .ExpiryDate.Value.ToDateTime(new TimeOnly(0, 0))
                        .ToString(customFormat);

                    ExpiryDateField = FieldFlexComponent(
                        StorageManagementKeywordGroup.ExpiryDate,
                        ExpiryDateString
                    );
                }
                else
                {
                    ExpiryDateField = null;
                }

                List<FlexComponent> FieldTable = new List<FlexComponent> { };

                if (NameField != null)
                    FieldTable.Add(NameField);
                if (AmountField != null)
                    FieldTable.Add(AmountField);
                if (LocationField != null)
                    FieldTable.Add(LocationField);
                if (PurchaseDateField != null)
                    FieldTable.Add(PurchaseDateField);
                if (ExpiryDateField != null)
                    FieldTable.Add(ExpiryDateField);
                _ReplyMessageListStatic = new List<object>
                {
                    GetBubbleFlexMessageObject(_InputStorageInfoStatic.Place, FieldTable)
                };
            }
            else
            {
                _ReplyMessageListStatic = DateTypeErrorHint("$ 過期日期格式錯誤, 請重新輸入(格式: YYYYMMDD)");
            }
        }
    }

    class EditAddedResultConfirmClass : Status
    {
        private readonly string? _WebHookEventMessage;

        public EditAddedResultConfirmClass(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            var FieldArray = _WebHookEventMessage!.Split(
                "/",
                StringSplitOptions.RemoveEmptyEntries
            );
            var StorageTableList = new List<List<string>>();
            var GetError = "";
            try
            {
                foreach (var item in FieldArray)
                {
                    char[] Colon = { ':', '：' };
                    var ValuePairArray = item.Split(Colon, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToArray();

                    var ExamineKey = ValuePairArray[0];

                    if (ExamineKey == "購買日期" || ExamineKey == "有效日期")
                    {
                        if (
                            DateOnly.TryParseExact(ValuePairArray[1], "yyyyMMdd", out DateOnly Date)
                        )
                        {
                            StorageTableList.Add(ValuePairArray.ToList());
                        }
                        else
                        {
                            GetError = $"{ValuePairArray[0]}, {ValuePairArray[1]}";
                            break;
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
                        StorageTableList.Add(ValuePairArray.ToList());
                    }
                    else
                    {
                        GetError = $"{ValuePairArray[0]}, {ValuePairArray[1]}";
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Console.WriteLine(ex.StackTrace);
            }

            if (GetError == "")
            {
                foreach (var KeyValueList in StorageTableList)
                {
                    Console.WriteLine(KeyValueList[0]);
                    switch (KeyValueList[0])
                    {
                        case StorageManagementKeywordGroup.Place:
                            _InputStorageInfoStatic.Place = KeyValueList[1];
                            break;
                        case StorageManagementKeywordGroup.Name:
                            _InputStorageInfoStatic.Name = KeyValueList[1];
                            break;
                        case StorageManagementKeywordGroup.Location:
                            _InputStorageInfoStatic.Location = KeyValueList[1];
                            break;
                        case StorageManagementKeywordGroup.Amount:
                            _InputStorageInfoStatic.Amount = KeyValueList[1];
                            break;
                        case StorageManagementKeywordGroup.PurchaseDate:
                            DateOnly PurchaseDate = DateOnly.ParseExact(
                                KeyValueList[1],
                                "yyyyMMdd"
                            );
                            _InputStorageInfoStatic.PurchaseDate = PurchaseDate;
                            break;
                        case StorageManagementKeywordGroup.ExpiryDate:
                            DateOnly ExpiryDate = DateOnly.ParseExact(KeyValueList[1], "yyyyMMdd");
                            _InputStorageInfoStatic.ExpiryDate = ExpiryDate;
                            break;
                    }
                }
                _InputStorageInfoStatic.ExpiryDate = null;
                var NameField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Name,
                    _InputStorageInfoStatic.Name
                );
                var AmountField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Amount,
                    _InputStorageInfoStatic.Amount
                );
                var LocationField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Location,
                    _InputStorageInfoStatic.Location
                );
                FlexComponent? PurchaseDateField;
                if (_InputStorageInfoStatic.PurchaseDate != null)
                {
                    string customFormat = "yyyy-MM-dd";
                    string PurchaseDateString = _InputStorageInfoStatic
                        .PurchaseDate.Value.ToDateTime(new TimeOnly(0, 0))
                        .ToString(customFormat);

                    PurchaseDateField = FieldFlexComponent(
                        StorageManagementKeywordGroup.PurchaseDate,
                        PurchaseDateString
                    );
                }
                else
                {
                    PurchaseDateField = null;
                }
                FlexComponent? ExpiryDateField;
                if (_InputStorageInfoStatic.ExpiryDate != null)
                {
                    string customFormat = "yyyy-MM-dd";
                    string ExpiryDateString = _InputStorageInfoStatic
                        .ExpiryDate.Value.ToDateTime(new TimeOnly(0, 0))
                        .ToString(customFormat);

                    ExpiryDateField = FieldFlexComponent(
                        StorageManagementKeywordGroup.ExpiryDate,
                        ExpiryDateString
                    );
                }
                else
                {
                    ExpiryDateField = null;
                }

                List<FlexComponent> FieldTable = new List<FlexComponent> { };

                if (NameField != null)
                    FieldTable.Add(NameField);
                if (AmountField != null)
                    FieldTable.Add(AmountField);
                if (LocationField != null)
                    FieldTable.Add(LocationField);
                if (PurchaseDateField != null)
                    FieldTable.Add(PurchaseDateField);
                if (ExpiryDateField != null)
                    FieldTable.Add(ExpiryDateField);
                _ReplyMessageListStatic = new List<object>
                {
                    GetBubbleFlexMessageObject(_InputStorageInfoStatic.Place, FieldTable)
                };
            }
            else
            {
                var NameField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Name,
                    _InputStorageInfoStatic.Name
                );
                var AmountField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Amount,
                    _InputStorageInfoStatic.Amount
                );
                var LocationField = FieldFlexComponent(
                    StorageManagementKeywordGroup.Location,
                    _InputStorageInfoStatic.Location
                );
                FlexComponent? PurchaseDateField;
                if (_InputStorageInfoStatic.PurchaseDate != null)
                {
                    string customFormat = "yyyy-MM-dd";
                    string PurchaseDateString = _InputStorageInfoStatic
                        .PurchaseDate.Value.ToDateTime(new TimeOnly(0, 0))
                        .ToString(customFormat);

                    PurchaseDateField = FieldFlexComponent(
                        StorageManagementKeywordGroup.PurchaseDate,
                        PurchaseDateString
                    );
                }
                else
                {
                    PurchaseDateField = null;
                }
                FlexComponent? ExpiryDateField;
                if (_InputStorageInfoStatic.ExpiryDate != null)
                {
                    string customFormat = "yyyy-MM-dd";
                    string ExpiryDateString = _InputStorageInfoStatic
                        .ExpiryDate.Value.ToDateTime(new TimeOnly(0, 0))
                        .ToString(customFormat);

                    ExpiryDateField = FieldFlexComponent(
                        StorageManagementKeywordGroup.ExpiryDate,
                        ExpiryDateString
                    );
                }
                else
                {
                    ExpiryDateField = null;
                }

                List<FlexComponent> FieldTable = new List<FlexComponent> { };

                if (NameField != null)
                    FieldTable.Add(NameField);
                if (AmountField != null)
                    FieldTable.Add(AmountField);
                if (LocationField != null)
                    FieldTable.Add(LocationField);
                if (PurchaseDateField != null)
                    FieldTable.Add(PurchaseDateField);
                if (ExpiryDateField != null)
                    FieldTable.Add(ExpiryDateField);
                _ReplyMessageListStatic = new List<object>
                {
                    GetBubbleFlexMessageObject(_InputStorageInfoStatic.Place, FieldTable)
                };
                _ReplyMessageListStatic.Add(
                    new TextMessageObject { Text = "發生錯誤: 此欄位出現問題 " + GetError }
                );
            }
        }
    }

    //! FlexComponent
    public async Task InputStorage(WebhookEventDto WebHookEventDto)
    {
        string? WebHookEventMessage = WebHookEventDto.Message!.Text;

        if (WebHookEventMessage == "新增物品至庫存")
            _InputStorageInfoStatic.Status = "init";

        if (WebHookEventMessage == "取消新增")
        {
            _InputStorageInfoStatic = new InputStorageInfo();
            await _storageManagementService.Init(WebHookEventDto);
            return;
        }

        if (WebHookEventMessage == "略過")
        {
            WebHookEventMessage = null;
        }

        var _StatusProcessorClass = new Dictionary<string, Status>
        {
            { "init", new StatusInitClass() },
            { "place", new StatusPlaceClass(WebHookEventMessage!) },
            { "name", new StatusNameClass(WebHookEventMessage!) },
            { "amount", new StatusAmountClass(WebHookEventMessage) },
            { "location", new StatusLocationClass(WebHookEventMessage) },
            { "purchaseDate", new StatusPurchaseDateClass(WebHookEventMessage) },
            { "expiryDate", new StatusExpiryDateClass(WebHookEventMessage) },
            { "edit", new EditAddedResultConfirmClass(WebHookEventMessage) }
        };
        // var _StatusProcessor = new Dictionary<string, Func<Task>>
        // {
        //     { "init", StatusInit },
        //     { "place", () => StatusPlace(WebHookEventMessage!) },
        //     { "name", () => StatusName(WebHookEventMessage!) },
        //     { "amount", () => StatusAmount(WebHookEventMessage!) },
        //     { "location", () => StatusLocation(WebHookEventMessage!) },
        //     { "purchaseDate", () => StatusPurchaseDate(WebHookEventMessage!) },
        //     { "expiryDate", () => StatusExpiryDate(WebHookEventMessage!) },
        //     { "edit", () => EditAddedResultConfirm(WebHookEventMessage!) }
        // };



        Console.WriteLine(WebHookEventMessage);

        _StatusProcessorClass[_InputStorageInfoStatic.Status].Init();

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    //? 新增完成 還沒做或改為 填寫完成
    //! function Type
    /*
        var _StatusProcessor = new Dictionary<string, Func<Task>>
        {
        { "init", StatusInit },
        { "place", () => StatusPlace(WebHookEventMessage!) },
    */
    //! C# new List<List<string>>(); 變成 array
    //! 重啟 database? 有時出問題會重啟
    //! var data = new List() data[0]??
    //! 要怎麼寫錯誤處理
    // 看 edit
    //! 把get set 看一下
    //! 看一下目前邏輯
    //! Select??
    //! search "out" 自己建立有 out 的function
    //! AddedResultConfirm 在做修改
    //! List<(string,int)>
    //? 完成修改 狀態要改嗎 _InputStorageInfoStatic.Status??
    /*
        ? 當使用者打修改時會觸發 修改的按鈕
        ? 用 postBack 處理// 如果 webhookStatus = StorageManagement && postback Data == 修改
        ? 觸發
    */

    // 回覆一個 flexMessage 顯你使用者剛剛輸入的資料
    //      顯示按鈕完成, 修改, 取消
    //          修改 用字串表示 物品名稱:XXX/
    //          錯誤顯示輸入錯誤
    // 將資料樹入到資料庫?
    // 清空 _InputStorageDataStatusStatic, _inputStorageStatus="init"
    // 回復成功訊息
    // 回到 有資料的那頁
    //! 紀錄 在List 泛型中加入兩個類別 的方式 chatGPT
    public async Task EditAddedResultConfirmPostBack(WebhookEventDto WebHookEventDto)
    {
        //? 這樣寫好嗎
        //! FlexMessage Final Result 要改一下要加欄位名稱
        // 要不要進 edit // 可以進, 進 edit 後做修改
        //? 如果點取消修改 要不要用 postback
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject { Text = "若要修改欄位, 例如修改物品名稱, 請輸入: 物品名稱:XXX並送出. XXX為要修改的資料", },
                new TextMessageObject { Text = "送出的結果為: 物品名稱:XXX", },
                new TextMessageObject
                {
                    Text = "若要修改多個欄位, 請輸入: 購買日期:YYYYMMDD/有效日期:YYYYMMDD/數量:XX並送出",
                },
                new TextMessageObject
                {
                    Text = "請依上述說明, 輸入要改的欄位",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消修改",
                                    Text = "取消修改",
                                }
                            }
                        }
                    }
                }
            ]
        );
        _InputStorageInfoStatic.Status = "edit";
        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }
}
