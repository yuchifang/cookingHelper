using CookingHelper.Enum;
using CookingHelper.LineDto;
using Microsoft.Extensions.Caching.Memory;

namespace CookingHelper.LineDtoService;

public class StorageSearch : UIWithData
{
    public virtual void Init(IMemoryCache memoryCache) { }

    public List<object> GetSearchUIBlock(List<StorageInfo> StorageInfoList)
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
}
