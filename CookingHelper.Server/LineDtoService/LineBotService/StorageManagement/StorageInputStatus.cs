using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

class StorageInputStatus : UIWithData
{
    public virtual void Init() { }

    public List<object> GetAdditionConfirmHint(InputStorageInfo InputStorageInfoStatic)
    {
        var StorageInfoTable = GetStorageInfoTable(InputStorageInfoStatic);
        var StorageTable = new List<FlexComponent>
        {
            new FlexComponent { Type = FlexComponentTypeEnum.Separator, Margin = "xxl" },
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
        };
        StorageTable.InsertRange(0, StorageInfoTable);
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
}
