namespace MyLibrary
{
    public class CompositionItem
    {
        public Component Component { get; set; }
        public int Quantity { get; set; }

        public string ComponentName => Component?.Name ?? "—";
        public string ComponentArticle => Component?.Article ?? "—";
        public string DisplayText => $"{ComponentName} — {Quantity} шт.";
        public string FullDescription => $"{ComponentName} ({ComponentArticle}) — {Quantity} шт.";
    }
}
