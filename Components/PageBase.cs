using Microsoft.AspNetCore.Components;

public class PageBase : ComponentBase
{
    public string Title {get; set;}    

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}