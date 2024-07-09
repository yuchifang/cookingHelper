using System.Diagnostics.Eventing.Reader;
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

    public static InputStorageDataStatus _InputStorageDataStatusStatic;

    public static List<BaseMessageObject> _ReplyMessageListStatic = new List<BaseMessageObject>();

    public async Task InputStorage(WebhookEventDto WebHookEventDto)
    {
        if (_InputStorageDataStatusStatic.InputStorageStatus == "init")
        {
            LineBotService._WebhookEventStateStatic = "新增物品至庫存";
            await InputPlaceHint();
        }
        else if (_InputStorageDataStatusStatic.InputStorageStatus == "place")
        {
            _InputStorageDataStatusStatic.Place = WebHookEventDto.Message!.Text!;
            await InputNameHint();
        }
        else if (_InputStorageDataStatusStatic.InputStorageStatus == "amount")
        {
            _InputStorageDataStatusStatic.Name = WebHookEventDto.Message!.Text!;
            await InputAmountHint();
        }
        else if (_InputStorageDataStatusStatic.InputStorageStatus == "location")
        {
            _InputStorageDataStatusStatic.Location = WebHookEventDto.Message!.Text!;
            await InputLocationHint();
        }
        else if (_InputStorageDataStatusStatic.InputStorageStatus == "purchaseDate")
        {
            _InputStorageDataStatusStatic.Amount = WebHookEventDto.Message!.Text!;
            await InputPurchaseDateHint();
        }
        else if (_InputStorageDataStatusStatic.InputStorageStatus == "expiryDate")
        {
            _InputStorageDataStatusStatic.PurchaseDate = WebHookEventDto.Message!.Text;
            await InputExpiryDateHint();
        }
        else if (_InputStorageDataStatusStatic.InputStorageStatus == "end")
        {
            _InputStorageDataStatusStatic.ExpiryDate = WebHookEventDto.Message!.Text;
            await AddedResultConfirm();
        }
        else if (_InputStorageDataStatusStatic.InputStorageStatus == "edit")
        {
            // 將使用者輸入字串, 解析成可以輸入 Field 的格式, 並檢查有沒有輸入錯誤, 沒有錯誤則 重新顯示給使用者確認
            // 有錯誤則 告知使用者哪邊出錯, 並回到上一頁

            // 更新成功
            // 更新失敗
            // 購買日期:XXX/有效日期:XXX
            // WebHookEventDto.Message!.Text
            var InputEditString = WebHookEventDto.Message!.Text;
            var FieldArray = InputEditString!.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var StorageTableArray = new List<List<string>>();
            var GetError = "";
            foreach (var item in FieldArray)
            {
                var ValuePairArray = item.Split(":", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim());
                var ExamineKey = new List<string>(ValuePairArray)[0];
                if (Array.IndexOf(StorageManagementKeywordGroup.ExamineList, ExamineKey) != -1)
                {
                    StorageTableArray.Add(new List<string>(ValuePairArray));
                }
                else
                {
                    GetError =
                        new List<string>(ValuePairArray)[0] + new List<string>(ValuePairArray)[1];
                }
            }
            if (GetError == "")
            {
                // 先檢查
                foreach (var KeyValue in StorageTableArray)
                {
                    switch (KeyValue[0])
                    {
                        case StorageManagementKeywordGroup.Place:
                            _InputStorageDataStatusStatic.Place = KeyValue[1];
                            break;
                        case StorageManagementKeywordGroup.Name:
                            _InputStorageDataStatusStatic.Name = KeyValue[1];
                            break;
                        case StorageManagementKeywordGroup.Location:
                            _InputStorageDataStatusStatic.Location = KeyValue[1];
                            break;
                        case StorageManagementKeywordGroup.Amount:
                            _InputStorageDataStatusStatic.Amount = KeyValue[1];
                            break;
                        case StorageManagementKeywordGroup.PurchaseDate:
                            _InputStorageDataStatusStatic.PurchaseDate = KeyValue[1];
                            break;
                        case StorageManagementKeywordGroup.ExpiryDate:
                            _InputStorageDataStatusStatic.ExpiryDate = KeyValue[1];
                            break;
                    }
                }
            }
            else
            {
                await AddedResultConfirm();
                _ReplyMessageListStatic.Add(new TextMessageObject { Text = "發生錯誤: " + GetError });
            }
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<BaseMessageObject>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    //! AddedResultConfirm 要改一下
    //! 把get set 看一下
    //! 看一下目前邏輯
    //! Select??
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
    public async Task EditAddedResultConfirmPostBack()
    {
        //! FlexMessage Final Result 要改一下要加欄位名稱
        // 要不要進 edit // 可以進, 進 edit 後做修改
        //? 如果點取消修改 要不要用 postback
        _ReplyMessageListStatic.AddRange(
            [
                new TextMessageObject { Text = "若要修改欄位, 例如修改物品名稱, 請輸入: 物品名稱:XXX並送出. XXX為要修改的資料", },
                new TextMessageObject { Text = "送出的結果為: 物品名稱:XXX", },
                new TextMessageObject { Text = "若要修改多個欄位, 請輸入: 購買日期:XXX/有效日期:XXX/數量:XX並送出", },
                new TextMessageObject { Text = "請依上述說明, 輸入要改的欄位", },
                new TextMessageObject
                {
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
        _InputStorageDataStatusStatic.InputStorageStatus = "edit";
    }

    public async Task InputPlaceHint()
    { //! 取消新增還沒做
        _ReplyMessageListStatic.AddRange(
            [
                new TextMessageObject { Text = "依儲存位置,物品名稱,詳細位置,購買時間,過期時間儲存", },
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
        _InputStorageDataStatusStatic.InputStorageStatus = "place";
    }

    public async Task InputNameHint()
    { //! 取消新增還沒做
        _ReplyMessageListStatic.AddRange(
            [
                new TextMessageObject { Text = "儲存位置及物品名稱一定要填入, 沒填入將無法紀錄", },
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
        _InputStorageDataStatusStatic.InputStorageStatus = "amount";
    }

    public async Task InputAmountHint()
    {
        _ReplyMessageListStatic.AddRange(
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
        _InputStorageDataStatusStatic.InputStorageStatus = "location";
    }

    public async Task InputLocationHint()
    { //! 取消新增還沒做 略過 完成輸入
        _ReplyMessageListStatic.AddRange(
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
        _InputStorageDataStatusStatic.InputStorageStatus = "purchaseDate";
    }

    public async Task InputPurchaseDateHint()
    { //! 取消新增 略過 完成輸入還沒做
        _ReplyMessageListStatic.AddRange(
            [
                new TextMessageObject
                {
                    Text = "請輸入購買時間(格式: YYYY-MM-DD):",
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
        _InputStorageDataStatusStatic.InputStorageStatus = "expiryDate";
    }

    public async Task InputExpiryDateHint()
    { //! 取消新增 略過 完成輸入還沒做
        _ReplyMessageListStatic.AddRange(
            [
                new TextMessageObject
                {
                    Text = "請輸入過期時間(格式: YYYY-MM-DD):",
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
        _InputStorageDataStatusStatic.InputStorageStatus = "end";
    }

    public async Task AddedResultConfirm()
    {
        _ReplyMessageListStatic.Add(
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
                                Type = FlexComponentTypeEnum.Text,
                                Text = "範例蘋果",
                                Weight = "bold",
                                Size = "lg",
                                Margin = "md"
                            },
                            new FlexComponent
                            {
                                Type = FlexComponentLayoutTypeEnum.Vertical,
                                Margin = "xxl",
                                Spacing = "sm",
                                Contents = new List<FlexComponent>
                                {
                                    new FlexComponent
                                    {
                                        Type = FlexComponentTypeEnum.Box,
                                        Layout = FlexComponentLayoutTypeEnum.Horizontal,
                                        Contents = new List<FlexComponent>
                                        {
                                            new FlexComponent
                                            {
                                                Type = FlexComponentTypeEnum.Text,
                                                Text = "Key One",
                                                Size = "sm",
                                                Color = "#555555",
                                                Flex = 0
                                            },
                                            new FlexComponent
                                            {
                                                Type = FlexComponentTypeEnum.Text,
                                                Text = "Value One",
                                                Size = "sm",
                                                Color = "#111111",
                                                Align = "end"
                                            }
                                        }
                                    },
                                    new FlexComponent
                                    {
                                        Type = FlexComponentTypeEnum.Box,
                                        Layout = FlexComponentLayoutTypeEnum.Horizontal,
                                        Contents = new List<FlexComponent>
                                        {
                                            new FlexComponent
                                            {
                                                Type = FlexComponentTypeEnum.Text,
                                                Text = "Key Second",
                                                Size = "sm",
                                                Color = "#555555",
                                                Flex = 0
                                            },
                                            new FlexComponent
                                            {
                                                Type = FlexComponentTypeEnum.Text,
                                                Text = "Value Second",
                                                Size = "sm",
                                                Color = "#111111",
                                                Align = "end"
                                            }
                                        }
                                    },
                                    new FlexComponent
                                    {
                                        Type = FlexComponentTypeEnum.Box,
                                        Layout = FlexComponentLayoutTypeEnum.Horizontal,
                                        Contents = new List<FlexComponent>
                                        {
                                            new FlexComponent
                                            {
                                                Type = FlexComponentTypeEnum.Text,
                                                Text = "Key third",
                                                Size = "sm",
                                                Color = "#555555",
                                                Flex = 0
                                            },
                                            new FlexComponent
                                            {
                                                Type = FlexComponentTypeEnum.Text,
                                                Text = "Value third",
                                                Size = "sm",
                                                Color = "#111111",
                                                Align = "end"
                                            }
                                        }
                                    }
                                }
                            },
                            new FlexComponent
                            {
                                Type = FlexComponentTypeEnum.Separator,
                                Margin = "xxl"
                            },
                            new FlexComponent
                            {
                                Type = FlexComponentTypeEnum.Box,
                                Layout = FlexComponentLayoutTypeEnum.Horizontal,
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
                                            Text = "修改",
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
                                            Label = "取消",
                                            Text = "取消",
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        );
    }
}
