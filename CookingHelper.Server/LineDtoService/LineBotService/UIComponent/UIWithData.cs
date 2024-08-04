using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

public class UIWithData : MessageUI
{
    private FlexComponent DefaultFlexComponentButtonGroup = new FlexComponent
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

    public List<object> GetAdditionConfirmHint(
        StorageInfo InputStorageInfoStatic,
        FlexComponent? InputFlexComponentButtonGroup
    )
    {
        if (InputFlexComponentButtonGroup == null)
        {
            InputFlexComponentButtonGroup = DefaultFlexComponentButtonGroup;
        }

        var StorageInfoTable = GetStorageInfoTable(InputStorageInfoStatic);

        var StorageTable = new List<FlexComponent>
        {
            new FlexComponent { Type = FlexComponentTypeEnum.Separator, Margin = "xxl" },
            InputFlexComponentButtonGroup!
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

    public List<FlexComponent> GetStorageInfoTable(StorageInfo StorageInfo)
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
                        Size = "xs",
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
}
