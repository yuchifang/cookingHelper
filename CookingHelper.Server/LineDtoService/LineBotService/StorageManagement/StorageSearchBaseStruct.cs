using CookingHelper.Enum;
using CookingHelper.LineDto;
using Microsoft.Extensions.Caching.Memory;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

public class StorageSearchBaseStruct : UIWithData
{
    public static StorageSearchBaseStruct Instance = new StorageSearchBaseStruct();

    public List<object> GetSearchResultItemList(List<StorageInfo> StorageInfoList)
    {
        var FlexCarouselContents = new List<FlexBubbleContainer>();

        foreach (StorageInfo StorageInfo in StorageInfoList)
        {
            FlexCarouselContents.Add(GetFlexBubbleContainer(StorageInfo));
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
        IQueryable<StorageInfo> StorageInfoEnumerable,
        int PageIndex,
        int PageSize
    )
    {
        var SearchedStoreItemEnumerable = Paginate(
            StorageInfoEnumerable,
            PageIndex,
            PageSize,
            out bool hasNextPage,
            out bool hasPrevPage
        );

        var SearchGroup = GetSearchResultItemList(SearchedStoreItemEnumerable.ToList());

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
