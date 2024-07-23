using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

class StorageInputStatus : MessageUI
{
    public virtual void Init() { }

    public List<object> GetAdditionConfirmHint(InputStorageInfo InputStorageInfoStatic)
    {
        var NameField = FieldFlexComponent(
            StorageManagementKeywordGroup.Name,
            InputStorageInfoStatic.Name
        );
        var AmountField = FieldFlexComponent(
            StorageManagementKeywordGroup.Amount,
            InputStorageInfoStatic.Amount
        );
        var LocationField = FieldFlexComponent(
            StorageManagementKeywordGroup.Location,
            InputStorageInfoStatic.Location
        );
        FlexComponent? PurchaseDateField;
        if (InputStorageInfoStatic.PurchaseDate != null)
        {
            string customFormat = "yyyy-MM-dd";
            string PurchaseDateString = InputStorageInfoStatic
                .PurchaseDate.Value.ToDateTime(new TimeOnly(0, 0))
                .ToString(customFormat);

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
        if (InputStorageInfoStatic.ExpiryDate != null)
        {
            string customFormat = "yyyy-MM-dd";
            string ExpiryDateString = InputStorageInfoStatic
                .ExpiryDate.Value.ToDateTime(new TimeOnly(0, 0))
                .ToString(customFormat);

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
        return new List<object>
        {
            GetBubbleFlexMessageObject(InputStorageInfoStatic.Place, FieldTable)
        };
    }
}
