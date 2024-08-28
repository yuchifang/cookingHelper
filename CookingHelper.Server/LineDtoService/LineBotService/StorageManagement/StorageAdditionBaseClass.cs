using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

class StorageAdditionBaseClass : UIWithData
{
    public static StorageAdditionBaseClass Instance = new StorageAdditionBaseClass();
}
