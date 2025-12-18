using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public interface IProductDetailView
    {
        ProductComposition Composition { get; set; }
        CompositionItem SelectedComponent { get; }

        event EventHandler LoadEvent;
        event EventHandler AddComponentEvent;
        event EventHandler EditComponentEvent;
        event EventHandler RemoveComponentEvent;
        event EventHandler SaveEvent;
        event EventHandler CloseEvent;
        event EventHandler RefreshEvent;

        void Show();
        void Close();
        void DisplayComposition(ProductComposition composition);
        void ShowMessage(string message, string title);
        void ShowComponentSelectionForm(List<Component> availableComponents);
        void ShowQuantityForm(CompositionItem component, int currentQuantity);
        bool ConfirmRemove(string message, string title);
        void RefreshComposition();
    }
}
