using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

class InputStorageBaseClass : UIWithData
{
    public static InputStorageBaseClass Instance = new InputStorageBaseClass();
}
