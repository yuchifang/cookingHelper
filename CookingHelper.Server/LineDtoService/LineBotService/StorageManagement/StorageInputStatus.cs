using CookingHelper.LineDto;
using static CookingHelper.Utils;

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
            string PurchaseDateString = DateOnlyToString(
                InputStorageInfoStatic.PurchaseDate.Value,
                null
            );
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
            string ExpiryDateString = DateOnlyToString(
                InputStorageInfoStatic.ExpiryDate.Value,
                null
            );

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
