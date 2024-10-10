using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SurveyAuth.Data;

public static class ItemHelper
{
    private static readonly ApplicationDbContext _context;
    
    private static SurveyConfig _config;

    public static SurveyConfig Config
    {
        get
        {
            if (_config == null)
                _config = ConfigService.GetConfig();
            return _config;
        }
    }

    static ItemHelper()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DefaultConnection")
            .Options;

        _context = new ApplicationDbContext(options);
    }

    public static bool IsNewItem<TItem>(TItem item) where TItem : class    // Reconsider, Id can be 0, but item is null (even if Id is not -1)
    {
        var id = item != null ? GetId(item) : null;
        return string.IsNullOrWhiteSpace(id) || id == "-1";
    }

    public static bool IsHidden<TItem>(TItem item, PropertyInfo property) where TItem : class
    {        
        if (item != null && property != null)
        {
            foreach (var hiddenField in Config.HiddenFields)
            {
                if (hiddenField.Item == "all" || hiddenField.Item == typeof(TItem).Name)
                {
                    if (hiddenField.Fields.Any(f => f.Equals(property.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool IsReadOnly<TItem>(TItem item, PropertyInfo property) where TItem : class
    {
        if (item != null && property != null)
        {
            foreach (var readOnlyField in Config.ReadOnlyFields)
            {
                if (readOnlyField.Item == "all" || readOnlyField.Item == typeof(TItem).Name)
                {                                
                    if (readOnlyField.Fields.Any(f => f.Equals(property.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }    

    public static string GetFieldWidthStyle<TItem>(TItem item, PropertyInfo property) where TItem : class
    {
        if (item == null || property == null)
        {
            return string.Empty;
        }
        return $"width:{GetFieldWidth(item, property)}px";        
    }

    public static int? GetFieldWidth<TItem>(TItem item, PropertyInfo property) where TItem : class
    {
        if (item == null || property == null)
        {
            return null;        // Consider returning a default number (100)
        }
        return Config.FieldSizes
            .FirstOrDefault(fs => fs.Item == typeof(TItem).Name && fs.Field == property.Name)?.Width;    
    }

    public static string GetDropDownDisplay<TItem>(TItem item) where TItem : class
    {        
        if (item == null)
        {
            return string.Empty;
        }
        var itemType = typeof(TItem).Name;
        var dropdownFields = Config.DropdownFields[itemType];
        return string.Join(" - ", dropdownFields.Select(field => 
            GetValue<TItem>(item, field)));        
    }

    public static object? GetValue<TItem>(TItem item, string propertyName)   // Consider using simply 'property' for consistency
    {
        if (item == null || propertyName == null)
        {
            return null;
        }
        var propertyValue = typeof(TItem).GetProperty(propertyName).GetValue(item);
        if (propertyValue is DateOnly dateOnly)
        {
            return dateOnly.ToString("dd-mmm-yy"); 
        }
        return propertyValue;        
    }    
    
    public static async Task<List<TItem>> GetItemsAsync<TItem>(this TItem _) where TItem : class
    {
        return await _context.Set<TItem>().ToListAsync();
    }

    public static TItem InitializeItem<TItem>(this TItem item) where TItem : class
    {
        if (item == null)
        {
            return null;
        }
        var idProperty = typeof(TItem).GetProperty("Id");
        idProperty?.SetValue(item, GetDefaultIdValue(idProperty.PropertyType));

        var nameProperty = GetDisplayNameProperty(typeof(TItem));
        if (nameProperty != null)
        {
            nameProperty.SetValue(item, $"New {typeof(TItem).Name}");
        }
        else
        {
            // Handle the case where no display name property is found
        }

        return item;
    }

    private static object GetDefaultIdValue(Type propertyType)
    {
        if (propertyType == null)
        {
            return null;
        }
        if (propertyType == typeof(int)) return -1;
        if (propertyType == typeof(Guid)) return Guid.NewGuid();

        throw new NotSupportedException($"Unsupported ID property type: {propertyType}");
    }
    
    public static void UpdateRelatedItem<TItem, TRelatedItem>(TItem item, TRelatedItem relatedItem) where TItem : class where TRelatedItem : class
    {
        if (item == null || relatedItem == null)
        {
            return;
        }
        var property = item.GetType().GetProperties()
            .FirstOrDefault(p => p.PropertyType == typeof(TRelatedItem));

        if (property != null)
        {
            // Update the related item property
            property.SetValue(item, relatedItem);

            // Update the related ID property
            var idProperty = typeof(TRelatedItem).GetProperty("Id");    // Id of related item
            var relatedIdProperty = item.GetType().GetProperties()      // Parent item's property for related item's id
                .FirstOrDefault(p => p.Name.ToLower().EndsWith("id") && p.Name.ToLower().StartsWith(typeof(TRelatedItem).Name.ToLower()));

            if (idProperty != null && relatedIdProperty != null)
            {
                relatedIdProperty.SetValue(item, idProperty.GetValue(relatedItem));
            }
        }
    }

    public static string? GetId<TItem>(TItem item) where TItem : class
    {
        if (item == null)
        {
            return null;
        }
        return typeof(TItem).GetProperty("Id")?.GetValue(item)?.ToString();
    }

    public static async Task<TItem> GetItemById<TItem>(object id) where TItem : class
    {
        if (id == null)
        {
            return null;
        }
        var keyProperty = typeof(TItem).GetProperty("Id");
        var keyType = keyProperty.PropertyType;
        
        // Convert id to the correct type
        var convertedId = Convert.ChangeType(id, keyType);
        
        return await _context.Set<TItem>().FindAsync(new object[] { convertedId });
    }    

    public static string GetDisplayName<TItem>(TItem item) where TItem : class
    {
        if (item == null)
        {
            return string.Empty;
        }
        return GetPropertyValue(item, GetDisplayNameProperty(typeof(TItem)));
    }

    public static string GetPropertyValue<TItem>(TItem item, PropertyInfo property) where TItem : class
    {
        if (item == null || property == null)
        {
            return string.Empty;
        }
        return property.GetValue(item)?.ToString();
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

    public static async Task AddItem<TItem>(TItem item) where TItem : class
    {
        var idProperty = typeof(TItem).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item, null);
        }

        _context.Set<TItem>().Add(item);
        await _context.SaveChangesAsync();        
    }        

    public static async Task UpdateItem<TItem>(TItem item) where TItem : class
    {        
        _context.Update(item);        
        await _context.SaveChangesAsync();
    }
    
    public static async Task DeleteItem<TItem>(TItem item) where TItem : class
    {
        _context.Set<TItem>().Remove(item);
        await _context.SaveChangesAsync();
    }    
}