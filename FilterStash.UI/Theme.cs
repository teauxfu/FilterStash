namespace FilterStash.UI
{
    public class Theme
    {
        string? _backgroundImage = "url('_content/FilterStash.UI/images/Act2_Concept.png')";

        public string? BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                _backgroundImage = value;
                ValuesChanged?.Invoke();
            }
        }

        public readonly List<string> BgUrls = [.. _bgUrls?.Select(i => $"url('_content/FilterStash.UI/images/{Uri.EscapeDataString(i)}')")];

        private static readonly string[] _bgUrls = [
            "Act1_Concept1.png",
            "Act2_Concept.png",
            "Act3_Concept.png",
            "Monk_Base.jpg",
            "Ranger_Base.jpg",
            "Witch_base.jpg",
            "Warrior_base.jpg",
            "sorceress_base.jpg",
        ];



        public Action? ValuesChanged;
    }
}
