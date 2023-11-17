using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RockPaperScissorsUI
{
    class ElementSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is VisualFighter fighter)
            {
                return ((FrameworkElement)container).FindResource("FighterTemplate") as DataTemplate;
            }
            else
            {
                return ((FrameworkElement)container).FindResource("ConnectionLineTemplate") as DataTemplate;

            }


        }
    }
}
