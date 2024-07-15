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

    //! 可不可以把程式碼包成 物件
    public async Task InputStorage(WebhookEventDto WebHookEventDto)
    {
        string? WebHookEventMessage = WebHookEventDto.Message!.Text;
        var _StatusProcessor = new Dictionary<string, Func<Task>>
        {
            { "init", StatusInit },
            { "place", () => StatusPlace(WebHookEventMessage!) },
            { "name", () => StatusName(WebHookEventMessage!) },
            { "amount", () => StatusAmount(WebHookEventMessage!) },
            { "location", () => StatusLocation(WebHookEventMessage!) },
            { "purchaseDate", () => StatusPurchaseDate(WebHookEventMessage!) },
            { "expiryDate", () => StatusExpiryDate(WebHookEventMessage!) },
            { "edit", () => EditAddedResultConfirm(WebHookEventMessage!) }
        };

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

        Console.WriteLine(WebHookEventMessage);

        await _StatusProcessor[_InputStorageInfoStatic.Status]();

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

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

    public async Task InputPlaceHint()
    {
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject { Text = "依儲存位置,物品名稱,詳細位置,購買時間,過期時間輸入", },
                new TextMessageObject { Text = "儲存位置及物品名稱一定要填入, 沒填入將無法紀錄", },
                new TextMessageObject
                {
                    Text = "請輸入儲存位置:",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消新增",
                                    Text = "取消新增",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task InputNameHint()
    {
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
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消新增",
                                    Text = "取消新增",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task InputAmountHint()
    {
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject
                {
                    Text = "請輸入數量:",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消新增",
                                    Text = "取消新增",
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "略過",
                                    Text = "略過",
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "新增完成",
                                    Text = "新增完成",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task InputLocationHint()
    { //! 還沒做 略過 完成輸入
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject
                {
                    Text = "請輸入詳細位置:",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消新增",
                                    Text = "取消新增",
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "略過",
                                    Text = "略過",
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "新增完成",
                                    Text = "新增完成",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task InputPurchaseDateHint()
    { //!  略過 完成輸入還沒做
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject
                {
                    Text = "請輸入購買時間(格式: YYYYMMDD):",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消新增",
                                    Text = "取消新增",
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "略過",
                                    Text = "略過",
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "新增完成",
                                    Text = "新增完成",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task InputExpiryDateHint()
    { //!  略過 完成輸入還沒做
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject
                {
                    Text = "請輸入過期時間(格式: YYYYMMDD):",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消新增",
                                    Text = "取消新增",
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "略過",
                                    Text = "略過",
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "新增完成",
                                    Text = "新增完成",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task DateTypeErrorHint(string HintText)
    {
        _ReplyMessageListStatic = new List<object>
        {
            new TextMessageObject
            {
                Text = HintText,
                Emojis = new List<TextMessageEmojiDto>
                {
                    new TextMessageEmojiDto
                    {
                        Index = 0,
                        ProductId = "5ac21ae3040ab15980c9b440",
                        EmojiId = "067"
                    }
                },
                QuickReply = new QuickReplyItemDto
                {
                    Items = new List<QuickReplyButtonDto>
                    {
                        new QuickReplyButtonDto
                        {
                            Action = new ActionDto
                            {
                                Type = ActionTypeEnum.Message,
                                Label = "取消新增",
                                Text = "取消新增",
                            }
                        },
                        new QuickReplyButtonDto
                        {
                            Action = new ActionDto
                            {
                                Type = ActionTypeEnum.Message,
                                Label = "略過",
                                Text = "略過",
                            }
                        },
                        new QuickReplyButtonDto
                        {
                            Action = new ActionDto
                            {
                                Type = ActionTypeEnum.Message,
                                Label = "新增完成",
                                Text = "新增完成",
                            }
                        }
                    }
                }
            }
        };
    }

    public async Task AddedResultConfirm()
    {
        //? 要button 字大一點?
        //? button 上面要有線嗎?
        //? 樣式還要改
        // ! 新增 取消還沒做
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
            new FlexMessageObject<FlexBubbleContainer>
            {
                AltText = "Display Temporary Input",
                Contents = new FlexBubbleContainer
                {
                    Type = FlexContainerTypeEnum.Bubble,
                    Styles = new FlexBubbleContainerStyle
                    {
                        Footer = new FlexBlockStyle { Separator = false }
                    },
                    Body = new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Box,
                        Layout = FlexComponentLayoutTypeEnum.Vertical,

                        Contents = new List<FlexComponent>
                        {
                            new FlexComponent
                            {
                                Type = FlexComponentTypeEnum.Box,
                                Layout = FlexComponentLayoutTypeEnum.Horizontal,
                                AlignItems = "center",
                                Contents = new List<FlexComponent>
                                {
                                    new FlexComponent
                                    {
                                        Type = FlexComponentTypeEnum.Text,
                                        Text = StorageManagementKeywordGroup.Place,
                                        Size = "xs",
                                    },
                                    new FlexComponent
                                    {
                                        Type = FlexComponentTypeEnum.Text,
                                        Text = _InputStorageInfoStatic.Place,
                                        Size = "xl",
                                        Align = "end"
                                    }
                                }
                            },
                            new FlexComponent
                            {
                                Type = FlexComponentTypeEnum.Box,
                                Layout = FlexComponentLayoutTypeEnum.Vertical,
                                Margin = "xxl",
                                Spacing = "sm",
                                Contents = FieldTable
                            },
                            new FlexComponent
                            {
                                Type = FlexComponentTypeEnum.Separator,
                                Margin = "xxl"
                            },
                            new FlexComponent
                            {
                                Type = FlexComponentTypeEnum.Box,
                                Layout = FlexComponentLayoutTypeEnum.Vertical,
                                Contents = new List<FlexComponent>
                                {
                                    new FlexComponent
                                    {
                                        Type = FlexComponentTypeEnum.Button,
                                        Action = new ActionDto
                                        {
                                            Type = ActionTypeEnum.Message,
                                            Label = "新增",
                                            Text = "新增"
                                        }
                                    },
                                    new FlexComponent
                                    {
                                        Type = FlexComponentTypeEnum.Button,
                                        Action = new ActionDto
                                        {
                                            Type = ActionTypeEnum.Postback,
                                            Label = "修改",
                                            Data = "修改",
                                            InputOption = PostbackInputOptionEnum.OpenKeyboard
                                        }
                                    },
                                    new FlexComponent
                                    {
                                        Type = FlexComponentTypeEnum.Button,
                                        Action = new ActionDto
                                        {
                                            Type = ActionTypeEnum.Message,
                                            Label = "取消新增",
                                            Text = "取消新增",
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    public FlexComponent? FieldFlexComponent(string? keyString, string? valueString)
    {
        if (valueString != null && valueString != "")
        {
            return new FlexComponent
            {
                Type = FlexComponentTypeEnum.Box,
                Layout = FlexComponentLayoutTypeEnum.Horizontal,
                Contents = new List<FlexComponent>
                {
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Text,
                        Text = keyString,
                        Size = "sm",
                        Color = "#555555",
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Text,
                        Text = valueString,
                        Size = "sm",
                        Color = "#111111",
                        Align = "end"
                    }
                }
            };
        }
        else
        {
            return null;
        }
    }

    public async Task EditAddedResultConfirm(string InputEditString)
    {
        // 將使用者輸入字串, 解析成可以輸入 Field 的格式, 並檢查有沒有輸入錯誤, 沒有錯誤則 重新顯示給使用者確認 AddedResultConfirm
        // 有錯誤則 告知使用者哪邊出錯, 並回到AddedResultConfirm

        var FieldArray = InputEditString!.Split("/", StringSplitOptions.RemoveEmptyEntries);
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
                    if (DateOnly.TryParseExact(ValuePairArray[1], "yyyyMMdd", out DateOnly Date))
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
                        DateOnly PurchaseDate = DateOnly.ParseExact(KeyValueList[1], "yyyyMMdd");
                        _InputStorageInfoStatic.PurchaseDate = PurchaseDate;
                        break;
                    case StorageManagementKeywordGroup.ExpiryDate:
                        DateOnly ExpiryDate = DateOnly.ParseExact(KeyValueList[1], "yyyyMMdd");
                        _InputStorageInfoStatic.ExpiryDate = ExpiryDate;
                        break;
                }
            }
            await AddedResultConfirm();
        }
        else
        {
            await AddedResultConfirm();
            _ReplyMessageListStatic.Add(
                new TextMessageObject { Text = "發生錯誤: 此欄位出現問題 " + GetError }
            );
        }
    }

    public async Task StatusInit()
    {
        LineBotService._WebhookEventStateStatic = "新增物品至庫存";
        await InputPlaceHint();
        _InputStorageInfoStatic.Status = "place";
    }

    public async Task StatusPlace(string WebHookEventMessage)
    {
        _InputStorageInfoStatic.Place = WebHookEventMessage;
        await InputNameHint();
        _InputStorageInfoStatic.Status = "name";
    }

    public async Task StatusName(string WebHookEventMessage)
    {
        _InputStorageInfoStatic.Name = WebHookEventMessage!;
        await InputAmountHint();
        _InputStorageInfoStatic.Status = "amount";
    }

    public async Task StatusAmount(string WebHookEventMessage)
    {
        _InputStorageInfoStatic.Amount = WebHookEventMessage!;
        await InputLocationHint();
        _InputStorageInfoStatic.Status = "location";
    }

    public async Task StatusLocation(string WebHookEventMessage)
    {
        _InputStorageInfoStatic.Location = WebHookEventMessage!;
        await InputPurchaseDateHint();
        _InputStorageInfoStatic.Status = "purchaseDate";
    }

    public async Task StatusPurchaseDate(string WebHookEventMessage)
    {
        var dateString = WebHookEventMessage;
        if (dateString == null)
        {
            _InputStorageInfoStatic.PurchaseDate = null;
            await InputExpiryDateHint();
            _InputStorageInfoStatic.Status = "expiryDate";
        }
        else if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly PurchaseDate))
        {
            _InputStorageInfoStatic.PurchaseDate = PurchaseDate;
            await InputExpiryDateHint();
            _InputStorageInfoStatic.Status = "expiryDate";
        }
        else
        {
            await DateTypeErrorHint("$ 購買日期格式錯誤, 請重新輸入(格式: YYYYMMDD)");
        }
    }

    public async Task StatusExpiryDate(string WebHookEventMessage)
    {
        var dateString = WebHookEventMessage;
        if (dateString == null)
        {
            _InputStorageInfoStatic.ExpiryDate = null;
            await AddedResultConfirm();
        }
        else if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly ExpiryDate))
        {
            _InputStorageInfoStatic.ExpiryDate = ExpiryDate;
            await AddedResultConfirm();
        }
        else
        {
            await DateTypeErrorHint("$ 過期日期格式錯誤, 請重新輸入(格式: YYYYMMDD)");
        }
    }
}
