using Microsoft.AspNetCore.Components;

public class PageBase : ComponentBase
{
    public string Title {get; set;}
    public bool Pagination {get; set;}

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}