using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace DesignPatterns
{
    public static class UITKHelpers
    {
        public static Button Button(string labelText, Action clickEvent)
        {
            var button = new Button(clickEvent);
            var label = new Label(labelText);
            button.Add(label);
            return button;
        }

        public static VisualElement FlexContainer(FlexDirection flexDirection)
        {
            var flexContainer = new VisualElement();
            flexContainer.style.flexDirection = flexDirection;
            flexContainer.style.alignContent = Align.Stretch;
            flexContainer.style.width = Length.Percent(100);
            return flexContainer;
        }
    }
}