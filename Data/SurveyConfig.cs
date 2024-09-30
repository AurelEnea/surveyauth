public class SurveyConfig
{
    public List<HiddenFieldConfig> HiddenFields { get; set; }
    public List<ReadOnlyFieldConfig> ReadOnlyFields { get; set; }
    public List<ItemFieldSizeConfig> FieldSizes { get; set; }
    public Dictionary<string, List<string>> DropdownFields { get; set; }
    public int DefaultId { get; set; }
}

public class HiddenFieldConfig
{
    public string Item { get; set; }
    public List<string> Fields { get; set; }
}

public class ReadOnlyFieldConfig
{
    public string Item { get; set; }
    public List<string> Fields { get; set; }
}

public class ItemFieldSizeConfig
{
    public string Item { get; set; }
    public string Field { get; set; }
    public int Width { get; set; }
}