using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.Model;
using Microsoft.AspNetCore.Components;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

/*

*/
//! 寫完要更新 Database migrations
public class StorageManagementPurchaseService
{
    public StorageManagementPurchaseService() { }

    public static InputStorageInfo _InputStorageInfoStatic = new InputStorageInfo();

    public static dynamic _ReplyMessageListStatic = new List<object>();

    public async Task InputStorage(WebhookEventDto WebHookEventDto)
    {
        if (WebHookEventDto.Message!.Text == "新增物品至庫存")
            _InputStorageInfoStatic.Status = "init";
        if (_InputStorageInfoStatic.Status == "init")
        {
            LineBotService._WebhookEventStateStatic = "新增物品至庫存";
            await InputPlaceHint();
            _InputStorageInfoStatic.Status = "place";
        }
        else if (_InputStorageInfoStatic.Status == "place")
        {
            _InputStorageInfoStatic.Place = WebHookEventDto.Message!.Text!;
            //?
            await InputNameHint();
            _InputStorageInfoStatic.Status = "name";
        }
        else if (_InputStorageInfoStatic.Status == "name")
        {
            _InputStorageInfoStatic.Name = WebHookEventDto.Message!.Text!;
            //?
            await InputAmountHint();
            _InputStorageInfoStatic.Status = "amount";
        }
        else if (_InputStorageInfoStatic.Status == "amount")
        {
            _InputStorageInfoStatic.Amount = WebHookEventDto.Message!.Text!;
            //?
            await InputLocationHint();
            _InputStorageInfoStatic.Status = "location";
        }
        else if (_InputStorageInfoStatic.Status == "location")
        {
            _InputStorageInfoStatic.Location = WebHookEventDto.Message!.Text!;
            //?
            await InputPurchaseDateHint();
            _InputStorageInfoStatic.Status = "purchaseDate";
        }
        else if (_InputStorageInfoStatic.Status == "purchaseDate")
        {
            var dateString = WebHookEventDto.Message!.Text;
            if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly PurchaseDate))
            {
                _InputStorageInfoStatic.PurchaseDate = PurchaseDate;

                await InputExpiryDateHint();
                _InputStorageInfoStatic.Status = "expiryDate";
            }
            else
            {
                _ReplyMessageListStatic = new List<object>
                {
                    new TextMessageObject
                    {
                        Text = "$ 購買日期格式錯誤, 請重新輸入(格式: YYYYMMDD)",
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
                                        Label = "完成輸入",
                                        Text = "完成輸入",
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
        else if (_InputStorageInfoStatic.Status == "expiryDate")
        {
            var dateString = WebHookEventDto.Message!.Text;
            if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly ExpiryDate))
            {
                _InputStorageInfoStatic.ExpiryDate = ExpiryDate;

                await AddedResultConfirm();
            }
            else
            {
                _ReplyMessageListStatic = new List<object>
                {
                    new TextMessageObject
                    {
                        Text = "$ 過期日期格式錯誤, 請重新輸入(格式: YYYYMMDD)",
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
                                        Label = "完成輸入",
                                        Text = "完成輸入",
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
        else if (_InputStorageInfoStatic.Status == "edit")
        {
            await EditAddedResultConfirm(WebHookEventDto.Message!.Text!);
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    //! 日期格式??
    //! 日期格式填錯??
    //! 完成時間

    //! 完成新增 簡化程式碼

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
    { //! 取消新增還沒做
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
    { //! 取消新增還沒做
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
                                    Label = "完成輸入",
                                    Text = "完成輸入",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task InputLocationHint()
    { //! 取消新增還沒做 略過 完成輸入
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
                                    Label = "完成輸入",
                                    Text = "完成輸入",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task InputPurchaseDateHint()
    { //! 取消新增 略過 完成輸入還沒做
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
                                    Label = "完成輸入",
                                    Text = "完成輸入",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task InputExpiryDateHint()
    { //! 取消新增 略過 完成輸入還沒做
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
                                    Label = "完成輸入",
                                    Text = "完成輸入",
                                }
                            }
                        }
                    }
                }
            ]
        );
    }

    public async Task AddedResultConfirm()
    {
        //? 要button 字大一點?
        //? button 上面要有線嗎?
        // ! 新增 修改 取消還沒做
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
        if (valueString != null || valueString != "")
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
                            StorageManagementKeywordGroup.ExamineList,
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
}
