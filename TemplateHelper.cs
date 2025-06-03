using System;
using System.Collections.Generic;
using Comfort.Common;
using EFT.InventoryLogic;

#nullable enable

namespace tarkin.moonitem
{
    // no idea what is happening in this class, i stole this from someone on github
    internal class TemplateHelper
    {
        private static readonly Dictionary<string, ItemTemplate> _templates = new Dictionary<string, ItemTemplate>();

        private static void UpdateTemplates()
        {
            if (!Singleton<ItemFactoryClass>.Instantiated)
                return;

            var mongoTemplates = Singleton<ItemFactoryClass>
                .Instance
                .ItemTemplates;

            if (_templates.Count == mongoTemplates.Count)
                return;

            foreach (var kv in mongoTemplates)
            {
                if (!_templates.ContainsKey(kv.Key.ToString()))
                {
                    _templates.Add(kv.Key.ToString(), kv.Value);
                }
            }
        }

        internal static ItemTemplate[] FindTemplates(string searchShortNameOrTemplateId)
        {
            UpdateTemplates();

            ItemTemplate? template;
            if (_templates.TryGetValue(searchShortNameOrTemplateId, out template) && template != null)
            {
                return new ItemTemplate[] { template };
            }

            List<ItemTemplate> foundTemplates = new List<ItemTemplate>();
            foreach (ItemTemplate t in _templates.Values)
            {
                string? shortName = t.ShortNameLocalizationKey?.Localized();
                string? name = t.NameLocalizationKey?.Localized();

                bool match = false;
                if (shortName != null && shortName.IndexOf(searchShortNameOrTemplateId, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    match = true;
                }

                if (!match && name != null && name.IndexOf(searchShortNameOrTemplateId, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    match = true;
                }

                if (match)
                {
                    foundTemplates.Add(t);
                }
            }

            return foundTemplates.ToArray();
        }
    }
}