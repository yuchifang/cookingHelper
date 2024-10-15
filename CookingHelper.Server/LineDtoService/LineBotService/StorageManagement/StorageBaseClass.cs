using System.Text.Json;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.Model;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

class StorageBaseClass : MessageUI
{
    public static StorageBaseClass Instance = new StorageBaseClass();
    public FlexComponent DefaultButtonGroup = new FlexComponent
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
    };

    public List<object> GetStorageAdditionConfirmHint(
        StorageInfo InputStorageInfoStatic,
        FlexComponent? InputFlexComponentButtonGroup
    )
    {
        if (InputFlexComponentButtonGroup == null)
        {
            InputFlexComponentButtonGroup = DefaultButtonGroup;
        }

        var StorageDetailTable = GetStorageDetailTable(InputStorageInfoStatic);

        var StorageTable = new List<FlexComponent>
        {
            new FlexComponent { Type = FlexComponentTypeEnum.Separator, Margin = "xxl" },
            InputFlexComponentButtonGroup!
        };
        StorageTable.InsertRange(0, StorageDetailTable);
        return new List<object>
        {
            new FlexMessageObject<FlexBubbleContainer>
            {
                AltText = "庫存新增結果",
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

                        Contents = StorageTable
                    }
                }
            },
        };
    }

    public List<FlexComponent> GetStorageDetailTable(StorageInfo StorageInfo)
    {
        var NameField = FieldFlexComponent(StorageManagementKeywordGroup.Name, StorageInfo.Name);
        var AmountField = FieldFlexComponent(
            StorageManagementKeywordGroup.Amount,
            StorageInfo.Amount
        );
        var LocationField = FieldFlexComponent(
            StorageManagementKeywordGroup.Location,
            StorageInfo.Location
        );
        FlexComponent? PurchaseDateField;
        if (StorageInfo.PurchaseDate != null)
        {
            string PurchaseDateString = DateOnlyToString(StorageInfo.PurchaseDate.Value, null);
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
        if (StorageInfo.ExpiryDate != null)
        {
            string ExpiryDateString = DateOnlyToString(StorageInfo.ExpiryDate.Value, null);

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
        return new List<FlexComponent>
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
                        Size = "md",
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Text,
                        Text = StorageInfo.Place,
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
        };
    }

    public FlexComponent GetStorageDetailField(StoreItem StoreItem, int index)
    {
        var LocationText = StoreItem.Location != null ? $" {StoreItem.Location}" : "";
        var AmountText = StoreItem.Amount != null ? $" {StoreItem.Amount}" : "";
        var PurchaseDateText =
            StoreItem.PurchaseDate != null
                ? $" (p){DateOnlyToString((DateOnly)StoreItem.PurchaseDate, null)}"
                : "";
        var ExpiryDateText =
            StoreItem.ExpiryDate != null
                ? $" (e){DateOnlyToString((DateOnly)StoreItem.ExpiryDate, null)}"
                : "";

        return new FlexComponent
        {
            Type = FlexComponentTypeEnum.Box,
            Layout = FlexComponentLayoutTypeEnum.Vertical,
            PaddingBottom = "10px",
            Contents = new List<FlexComponent>
            {
                new FlexComponent
                {
                    Wrap = true,
                    Type = FlexComponentTypeEnum.Text,
                    Size = "xl",

                    Text =
                        $"{index + 1} {StoreItem.Place} {StoreItem.Name}{LocationText}{AmountText}{PurchaseDateText}{ExpiryDateText}"
                },
            }
        };
    }

    public FlexMessageObject<FlexBubbleContainer> GetStorageManagementFieldTable(
        List<FlexComponent> StorageFieldUIList,
        bool hasNextPage,
        bool hasPrevPage
    )
    {
        FlexComponent ButtonGroup = new FlexComponent
        {
            Type = FlexComponentTypeEnum.Box,
            Layout = FlexComponentLayoutTypeEnum.Horizontal,
            Contents = new List<FlexComponent> { }
        };
        var StorageUITable = new List<FlexComponent>
        {
            new FlexComponent
            {
                Type = FlexComponentTypeEnum.Box,
                Layout = FlexComponentLayoutTypeEnum.Vertical,
                Contents = new List<FlexComponent>
                {
                    new FlexComponent
                    {
                        Size = "md",
                        Wrap = true,
                        Type = FlexComponentTypeEnum.Text,
                        Text = "依編號,儲存位置,物品名稱,詳細位置,數量,購買日期(p),有效日期(e)排列"
                    },
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
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "依購買日期排序",
                            Text = "依購買日期排序"
                        }
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "依有效日期排序",
                            Text = "依有效日期排序"
                        }
                    },
                }
            },
        };
        if (hasPrevPage)
        {
            ButtonGroup.Contents.Add(
                new FlexComponent
                {
                    Type = FlexComponentTypeEnum.Button,
                    Action = new ActionDto
                    {
                        Type = ActionTypeEnum.Message,
                        Label = "上一頁",
                        Text = "上一頁"
                    }
                }
            );
        }
        if (hasNextPage)
        {
            ButtonGroup.Contents.Add(
                new FlexComponent
                {
                    Type = FlexComponentTypeEnum.Button,
                    Action = new ActionDto
                    {
                        Type = ActionTypeEnum.Message,
                        Label = "下一頁",
                        Text = "下一頁"
                    }
                }
            );
        }
        if (ButtonGroup.Contents.Count != 0)
        {
            StorageUITable.Add(ButtonGroup);
        }

        StorageUITable.InsertRange(2, StorageFieldUIList);
        return new FlexMessageObject<FlexBubbleContainer>
        {
            AltText = "StorageManagementUIBlock",

            QuickReply = new QuickReplyItemDto
            {
                Items = new List<QuickReplyButtonDto>
                {
                    new QuickReplyButtonDto
                    {
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Postback,
                            Label = KeywordGroup.StorageManagementAdded,
                            Text = KeywordGroup.StorageManagementAdded, // 有用
                            Data = KeywordGroup.StorageManagementAdded, // for open keyboard
                            InputOption = PostbackInputOptionEnum.OpenKeyboard,
                        }
                    },
                    new QuickReplyButtonDto
                    {
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Postback,
                            Label = KeywordGroup.StorageManagementSearch,
                            Data = KeywordGroup.StorageManagementSearch,
                            InputOption = PostbackInputOptionEnum.OpenKeyboard,
                        }
                    },
                    new QuickReplyButtonDto
                    {
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Postback,
                            Label = "刪除",
                            Text = "刪除",
                            Data = "刪除",
                            InputOption = PostbackInputOptionEnum.OpenKeyboard,
                        }
                    }
                }
            },
            Contents = new FlexBubbleContainer
            {
                Size = "giga",
                Type = FlexContainerTypeEnum.Bubble,
                Body = new FlexComponent
                {
                    Type = FlexComponentTypeEnum.Box,
                    Layout = FlexComponentLayoutTypeEnum.Vertical,
                    PaddingAll = "10px",
                    PaddingBottom = "0px",
                    Contents = StorageUITable
                }
            }
        };
    }

    public FlexBubbleContainer GetResultItemUI(StoreItem StoreItem)
    {
        var StorageDetailTable = GetStorageDetailTable(StoreItem);
        var StorageTable = new List<FlexComponent>
        {
            new FlexComponent { Type = FlexComponentTypeEnum.Separator, Margin = "xxl" },
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
                            Type = ActionTypeEnum.Postback,
                            Label = "刪除",
                            Data = "c" + JsonSerializer.Serialize(StoreItem),
                            DisplayText = "刪除",
                        }
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Postback,
                            Label = "修改",
                            Data = "e" + JsonSerializer.Serialize(StoreItem),
                            InputOption = PostbackInputOptionEnum.OpenKeyboard
                        }
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "返回",
                            Text = "返回",
                        }
                    }
                }
            }
        };
        StorageTable.InsertRange(0, StorageDetailTable);
        return new FlexBubbleContainer
        {
            Type = FlexContainerTypeEnum.Bubble,
            Body = new FlexComponent
            {
                Type = FlexComponentTypeEnum.Box,
                Layout = FlexComponentLayoutTypeEnum.Vertical,

                Contents = StorageTable
            }
        };
    }

    public List<object> GetSearchResult(List<StoreItem> StoreItemList)
    {
        var SearchResultList = new List<FlexBubbleContainer>();

        foreach (StoreItem StoreItem in StoreItemList)
        {
            SearchResultList.Add(GetResultItemUI(StoreItem));
        }

        return new List<object>
        {
            new FlexMessageObject<FlexCarouselContainer>
            {
                AltText = "庫存搜尋結果",
                Contents = new FlexCarouselContainer
                {
                    Type = FlexContainerTypeEnum.Carousel,
                    Contents = SearchResultList
                }
            }
        };
    }

    public List<object> GetSearchResultUIBlock(
        IQueryable<StoreItem> StorageInfoQueryable,
        int PageIndex,
        int PageSize
    )
    {
        var SearchedStoreItemQueryable = Paginate(
            StorageInfoQueryable,
            PageIndex,
            PageSize,
            out bool hasNextPage,
            out bool hasPrevPage
        );

        var SearchGroup = GetSearchResult(SearchedStoreItemQueryable.ToList());

        if (hasNextPage && hasPrevPage)
        {
            ((FlexMessageObject<FlexCarouselContainer>)SearchGroup[0]).QuickReply =
                GetPrevAndNextPageQuickItem();
        }
        else if (hasNextPage)
        {
            ((FlexMessageObject<FlexCarouselContainer>)SearchGroup[0]).QuickReply =
                GetNextPageQuickItem();
        }
        else if (hasPrevPage)
        {
            ((FlexMessageObject<FlexCarouselContainer>)SearchGroup[0]).QuickReply =
                GetPrevPageQuickItem();
        }
        return SearchGroup;
    }
}
