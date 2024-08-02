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
}
