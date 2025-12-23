using System.Collections.Generic;
using System.Linq;

namespace MyLibrary
{
    public class ProductComposition
    {
        public Product Product { get; set; }
        public List<CompositionItem> Components { get; set; }
    }
}
