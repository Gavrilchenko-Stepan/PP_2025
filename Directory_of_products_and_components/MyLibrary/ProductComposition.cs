using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class ProductComposition
    {
        public Product Product { get; set; }
        public List<CompositionItem> Components { get; set; }

        public string ProductFullName => $"{Product?.Name} ({Product?.Article})";
        public int ComponentTypesCount => Components?.Count ?? 0;
        public int TotalComponents => Components?.Sum(c => c.Quantity) ?? 0;

        public string CompositionSummary
        {
            get
            {
                if (Components == null || !Components.Any())
                    return "Состав не задан";

                var items = Components
                    .OrderByDescending(c => c.Quantity)
                    .Take(3)
                    .Select(c => $"{c.ComponentName} ×{c.Quantity}");

                string result = string.Join("; ", items);

                if (Components.Count > 3)
                    result += $"; и ещё {Components.Count - 3} позиций";

                return result;
            }
        }
    }
}
