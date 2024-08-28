using System.Collections.Generic;
using CookingHelper.Enum;
using CookingHelper.LineDto;

namespace CookingHelper.LineDtoService;

class RecipeListAdditionBaseClass : UIWithData
{
    public static RecipeListAdditionBaseClass Instance = new RecipeListAdditionBaseClass();

    //! 建立 Ui給 填寫完成用
    //! 圖片要怎麼處理?? 圖片 byte 轉 url
    //! 靜態圖片??
    //! 寫修改 新增

    public List<object> GetRecipeAdditionConfirmHint(InputRecipeInfo InputRecipeInfo)
    {
        var RecipeTable = new List<FlexComponent>
        {
            new FlexComponent { Type = FlexComponentTypeEnum.Separator, Margin = "xxl" },
            DefaultFlexComponentButtonGroup
        };
        var RecipeInfoTable = GetRecipeInfoTable(InputRecipeInfo);
        RecipeTable.InsertRange(0, RecipeInfoTable);

        var RecipeAdditionConfirmUIBlock = new List<object>
        {
            new FlexMessageObject<FlexBubbleContainer>
            {
                AltText = "食譜新增結果",
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

                        Contents = RecipeTable
                    }
                }
            }
        };

        if (InputRecipeInfo.ImageContent != null)
        {
            ((FlexMessageObject<FlexBubbleContainer>)RecipeAdditionConfirmUIBlock[0])
                .Contents
                .Hero = new FlexComponent
            {
                Type = FlexComponentTypeEnum.Image,
                Url = "https://developers-resource.landpress.line.me/fx/img/01_1_cafe.png",
                Size = "full",
                AspectMode = "cover",
                AspectRatio = "20:13"
            };
        }

        return RecipeAdditionConfirmUIBlock;
    }

    public List<FlexComponent> GetRecipeInfoTable(InputRecipeInfo InputRecipeInfo)
    {
        var IngredientsField = FieldFlexComponent(
            RecipeKeywordGroup.Ingredients,
            InputRecipeInfo.Ingredients
        );

        List<FlexComponent> FieldTable = new List<FlexComponent> { IngredientsField! };
        foreach (var (step, index) in InputRecipeInfo.Step.WithIndex())
        {
            var StepField = FieldFlexComponent(RecipeKeywordGroup.Step + $"{index + 1}", step);
            FieldTable.Add(StepField!);
        }
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
                        Text = RecipeKeywordGroup.Name,
                        Size = "xs",
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Text,
                        Text = InputRecipeInfo.Name,
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

public static class IEnumerableExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) =>
        self.Select((item, index) => (item, index));
}
