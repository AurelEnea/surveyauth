using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SurveyAuth.Data;

public static class PropertyHelper
{
    private static readonly ApplicationDbContext _context;

    static PropertyHelper()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DefaultConnection")
            .Options;

        _context = new ApplicationDbContext(options);
    }
    
    public static string GetId(object item)
    {
        return GetPropertyValue(item, GetIdProperty(item.GetType()));
    }

    public static string GetDisplayName(object item)
    {
        return GetPropertyValue(item, GetDisplayNameProperty(item.GetType()));
    }

    public static string GetPropertyValue(object item, PropertyInfo prop)
    {
        return prop.GetValue(item)?.ToString();
    }

    public static PropertyInfo GetIdProperty(Type type)
    {
        return type.GetProperties().FirstOrDefault(p => p.Name.ToLower().Contains("id"));
    }

    public static PropertyInfo GetDisplayNameProperty(Type type)
    {
        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            if (prop.Name.ToLower().Contains("name") && prop.PropertyType == typeof(string))
            {
                return prop;
            }
        }
        return properties.FirstOrDefault(p => p.PropertyType == typeof(string));
    }

    public static bool IsNewItem(object item)
    {
        return GetId(item).Equals(-1);
    }
    
   public static async Task AddNewItem(object item)
    {
        await SaveItem(item);
    }

    public static async Task EditItem(object item)
    {
        await SaveItem(item);
    }

    public static async Task<List<TItem>> GetItems<TItem>() where TItem : class
    {
        return await _context.Set<TItem>().ToListAsync();
    }

    public static async Task DeleteItem<TItem>(TItem item) where TItem : class
    {
        _context.Set<TItem>().Remove(item);
        await _context.SaveChangesAsync();
    }

    public static async Task SaveItem<TItem>(TItem item) where TItem : class
    {
        var existingItem = await _context.Set<TItem>().FindAsync(GetId(item));
        if (existingItem == null)
        {
            _context.Set<TItem>().Add(item);
        }
        else
        {
            _context.Update(item);
        }
        await _context.SaveChangesAsync();
    }
}