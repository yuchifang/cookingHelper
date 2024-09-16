using System.Text.Json;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.Model;

namespace CookingHelper.LineDtoService;

class RecipeListAdditionBaseClass : UIWithData
{
    public static RecipeListAdditionBaseClass Instance = new RecipeListAdditionBaseClass();

    public FlexComponent DeleteButtonGroup(RecipeItem RecipeItem)
    {
        return new FlexComponent
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
                        Type = ActionTypeEnum.Postback,
                        Label = "刪除",
                        DisplayText = "刪除",
                        Data = "d" + JsonSerializer.Serialize(RecipeItem),
                    }
                },
            }
        };
    }

    public FlexComponent FlexComponentButtonGroup = new FlexComponent
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
                    Type = ActionTypeEnum.Message,
                    Label = "取消新增",
                    Text = "取消新增",
                }
            }
        }
    };

    public FlexBubbleContainer GetFlexBubbleContainer(
        RecipeItem RecipeItem,
        FlexComponent? InputFlexComponentButtonGroup
    )
    {
        if (InputFlexComponentButtonGroup == null)
        {
            InputFlexComponentButtonGroup = FlexComponentButtonGroup;
        }

        var RecipeTable = new List<FlexComponent>
        {
            new FlexComponent { Type = FlexComponentTypeEnum.Separator, Margin = "xxl" },
            InputFlexComponentButtonGroup
        };
        var RecipeInfoTable = GetRecipeInfoTable(RecipeItem);
        RecipeTable.InsertRange(0, RecipeInfoTable);

        var FlexBubbleContainer = new FlexBubbleContainer
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
        };
        if (RecipeItem.ImagePath != null)
        {
            FlexBubbleContainer.Hero = new FlexComponent
            {
                Type = FlexComponentTypeEnum.Image,
                Url =
                    $"https://b2b7-2001-b011-7002-1eb9-3093-705d-19be-a830.ngrok-free.app/{RecipeItem.ImagePath}",
                Size = "full",
                AspectMode = "cover",
                AspectRatio = "20:13"
            };
        }
        else
        {
            FlexBubbleContainer.Hero = new FlexComponent
            {
                Type = FlexComponentTypeEnum.Image,
                Url =
                    $"https://b2b7-2001-b011-7002-1eb9-3093-705d-19be-a830.ngrok-free.app/UploadFile/RecipeImage/CookingHelperLineLogo.png",
                Size = "full",
                AspectMode = "cover",
                AspectRatio = "20:13"
            };
        }
        return FlexBubbleContainer;
    }

    public List<object> GetRecipeAdditionConfirmHint(InputRecipeInfo InputRecipeInfo)
    {
        var RecipeAdditionConfirmUIBlock = new List<object>
        {
            new FlexMessageObject<FlexBubbleContainer>
            {
                AltText = "食譜新增結果",
                Contents = GetFlexBubbleContainer(InputRecipeInfo, null)
            }
        };

        return RecipeAdditionConfirmUIBlock;
    }

    public List<FlexComponent> GetRecipeInfoTable(RecipeItem RecipeItem)
    {
        var IngredientsField = FieldFlexComponent(
            RecipeKeywordGroup.Ingredients,
            RecipeItem.Ingredients
        );

        List<FlexComponent> FieldTable = new List<FlexComponent> { IngredientsField! };
        foreach (var (step, index) in RecipeItem.Step.WithIndex())
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
                        Size = "md",
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Text,
                        Text = RecipeItem.Name,
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
