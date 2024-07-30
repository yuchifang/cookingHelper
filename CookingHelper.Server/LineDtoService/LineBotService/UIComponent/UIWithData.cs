using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

public class UIWithData : MessageUI
{
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

    public FlexBubbleContainer GetFlexBubbleContainer(StorageInfo StorageInfo)
    {
        var StorageInfoTable = GetStorageInfoTable(StorageInfo);
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
                            Type = ActionTypeEnum.Message,
                            Label = "刪除",
                            Text = "刪除"
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
}
