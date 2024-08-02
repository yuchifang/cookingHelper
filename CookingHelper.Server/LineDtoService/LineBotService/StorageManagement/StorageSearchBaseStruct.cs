using System.Text.Json;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.Model;
using Microsoft.Extensions.Caching.Memory;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

public class StorageSearchBaseStruct : UIWithData
{
    public static StorageSearchBaseStruct Instance = new StorageSearchBaseStruct();

    public FlexBubbleContainer GetFlexBubbleContainer(StoreItem StoreItem)
    {
        var StorageInfoTable = GetStorageInfoTable(StoreItem);
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
                            Label = "返回",
                            Text = "返回",
                        }
                    }
                }
            }
        };
        StorageTable.InsertRange(0, StorageInfoTable);
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

    public List<object> GetSearchResultItemList(List<StoreItem> StoreItemList)
    {
        var FlexCarouselContents = new List<FlexBubbleContainer>();

        foreach (StoreItem StoreItem in StoreItemList)
        {
            FlexCarouselContents.Add(GetFlexBubbleContainer(StoreItem));
        }

        return new List<object>
        {
            new FlexMessageObject<FlexCarouselContainer>
            {
                AltText = "庫存搜尋結果",
                Contents = new FlexCarouselContainer
                {
                    Type = FlexContainerTypeEnum.Carousel,
                    Contents = FlexCarouselContents
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

        var SearchGroup = GetSearchResultItemList(SearchedStoreItemQueryable.ToList());

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
