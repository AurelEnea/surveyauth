using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SurveyAuth.Data;

public static class PropertyHelperOld
{
    private static readonly ApplicationDbContext _context;

    static PropertyHelperOld()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DefaultConnection")
            .Options;

        _context = new ApplicationDbContext(options);
    }

    public static async Task<List<TItem>> GetItems<TItem>() where TItem : class
    {
        return await _context.Set<TItem>().ToListAsync();
    }

    public static object InitializeItem(object item)        
    {
        var idProperty = item.GetType().GetProperty("Id");
        idProperty.SetValue(item, GetDefaultIdValue(idProperty.PropertyType));

        var nameProperty = GetDisplayNameProperty(item.GetType());
        if (nameProperty != null)
        {
            nameProperty.SetValue(item, "New Item");
        }
        else
        {
            // Handle the case where no display name property is found
        }

        return item;
    }

    private static object GetDefaultIdValue(Type propertyType)
    {
        if (propertyType == typeof(int)) return -1;
        if (propertyType == typeof(Guid)) return Guid.NewGuid();

        throw new NotSupportedException($"Unsupported ID property type: {propertyType}");
    }
    
    public static string GetId(object item)
    {
        return item?.GetType().GetProperty("Id")?.GetValue(item)?.ToString();
    }

    public static string GetDisplayName(object item)
    {
        return GetPropertyValue(item, GetDisplayNameProperty(item.GetType()));
    }

    public static string GetPropertyValue(object item, PropertyInfo prop)
    {
        return prop.GetValue(item)?.ToString();
    }

    public static PropertyInfo GetDisplayNameProperty(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        var attributedProperty = type.GetProperties()
                                    .Where(p => Attribute.IsDefined(p, typeof(DisplayNameAttribute)))
                                    .FirstOrDefault();

        if (attributedProperty != null) return attributedProperty;

        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            if (prop.Name != null && prop.Name.ToLower().Contains("name") && prop.PropertyType == typeof(string))
            {
                return prop;
            }
        }

        return properties.FirstOrDefault(p => p != null && p.PropertyType == typeof(string));
    }

    public static async Task<TItem> GetItemById<TItem>(object id) where TItem : class
    {
        var keyProperty = typeof(TItem).GetProperty("Id");
        var keyType = keyProperty.PropertyType;
        
        // Convert id to the correct type
        var convertedId = Convert.ChangeType(id, keyType);
        
        return await _context.Set<TItem>().FindAsync(new object[] { convertedId });
    }

    public static bool IsNewItem(object item)
    {
        var id = GetId(item);
        return string.IsNullOrWhiteSpace(id) || id == "-1";
    }
    
   public static async Task AddItem(object item)
    {
        await AddItem(item);
    }

    public static async Task UpdateItem(object item)
    {
        await UpdateItem(item);
    }    

    public static async Task DeleteItem<TItem>(TItem item) where TItem : class
    {
        _context.Set<TItem>().Remove(item);
        await _context.SaveChangesAsync();
    }

    public static async Task UpdateItem<TItem>(TItem item) where TItem : class
    {        
        _context.Update(item);        
        await _context.SaveChangesAsync();
    }

    public static async Task AddItem<TItem>(TItem item) where TItem : class
    {
        _context.Set<TItem>().Add(item);
        await _context.SaveChangesAsync();
    }
}