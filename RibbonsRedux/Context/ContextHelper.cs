using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RibbonsRedux.Graphics;
using RibbonsRedux.Content;

namespace RibbonsRedux.Context
{
    public static class ContextHelper
    {
        public static MultiOptionSelector BuildMultiOptionSelector(AssetManager assets, string theme, string selectorName, List<Option> options, Cursor cursor, int initialValue = 0)
        {
            return BuildMultiOptionSelector(assets, theme, selectorName, options, MultiOptionArrangement.ListY, cursor, initialValue);
        }

        public static MultiOptionSelector BuildMultiOptionSelector(AssetManager assets, string theme, string selectorName, List<Option> options, MultiOptionArrangement arrangement, Cursor cursor, int initialValue = 0)
        {
            TextDictionary assetDictionary = new TextDictionary(assets.GetText("selector"));
            string selectorTheme = assetDictionary.LookupString(theme, selectorName + "SelectorTheme");
            Vector2 selectorPosition = assetDictionary.LookupVector2(theme, selectorName + "SelectorPosition");
            Anchor selectorAnchor = Anchor.Center;
            if (assetDictionary.CheckPropertyExists(theme, selectorName + "SelectorAnchor"))
                Enum.TryParse<Anchor>(assetDictionary.LookupString(theme, selectorName + "SelectorAnchor"), out selectorAnchor);
            return new MultiOptionSelector(assets, selectorTheme, selectorPosition, selectorAnchor, options, arrangement, cursor, initialValue);
        }
    }
}
