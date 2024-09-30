public class SurveyConfig
{
    public List<HiddenFieldConfig> HiddenFields { get; set; }
    public Dictionary<string, List<string>> DropdownFields { get; set; }
}

public class HiddenFieldConfig
{
    public string Item { get; set; }
    public List<string> Fields { get; set; }
}