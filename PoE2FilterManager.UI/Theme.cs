namespace PoE2FilterManager.UI
{
    public class Theme
    {
        string? _backgroundImage = "url('_content/PoE2FilterManager.UI/images/Act2_Concept.png')";

        public string? BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                _backgroundImage = value;
                ValuesChanged?.Invoke();
            }
        }
        
        public readonly List<string> BgUrls = [.. _bgUrls?.Select(i => $"url('_content/PoE2FilterManager.UI/images/{Uri.EscapeDataString(i)}')")];

        private static readonly string[] _bgUrls = [
            "Act1_Concept1.png",
            "Act2_Concept.png",
            "Act3_Concept.png",
            "ChaosTrial_Cinematic3.png",
            "Executioner_Cinematic2.png",
            "MapDevice_Cinematic.png",
            "Mercenary_Base.jpg",
            "Mercenary_Gemling_Legionnaire.jpg",
            "Mercenary_Witchhunter.jpg",
            "Monk_Acolyte_of_Chayula.jpg",
            "Monk_Base.jpg",
            "Monk_Master_of_the_Natural_Element.jpg",
            "Ranger_Deadeye.jpg",
            "Ranger_Survivalist.jpg",
            "Ranger_Base.jpg",
            "sorceress_Base.jpg",
            "Sorceress_Chronomancer.jpg",
            "Sorceress_Stormcaller.jpg",
            "TrialofSekhemaEntrance_Cinematic.png",
            "Warrior_Titan.jpg",
            "Warrior_Warbringer.jpg",
            "Warrior_Base.jpg",
            "Warrior_Cinematic_2.png",
            "Witch_base.jpg",
            "Witch_Blood_Mage.jpg",
            "Witch_infernalist.jpg",
        ];



        public Action? ValuesChanged;
    }
}
