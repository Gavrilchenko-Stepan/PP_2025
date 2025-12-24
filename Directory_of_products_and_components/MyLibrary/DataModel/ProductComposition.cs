using System.Collections.Generic;

namespace MyLibrary
{
    public class ProductComposition
    {
        public Product Product { get; set; }
        public List<CompositionItem> Components { get; set; }
    }
}
