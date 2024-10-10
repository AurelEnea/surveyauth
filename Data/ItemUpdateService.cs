/// <summary>
/// Service for notifying components of item updates.
/// </summary>
public class ItemUpdateService
{
    /// <summary>
    /// Event raised when an item is updated.
    /// </summary>
    public event Action OnItemUpdated;

    public event Action OnPropertyUpdated;

    /// <summary>
    /// Notifies all subscribers of an item update.
    /// </summary>
    public void NotifyItemUpdated()
    {
        OnItemUpdated?.Invoke();
    }

    public void NotifyPropertyUpdated()
    {
        OnPropertyUpdated?.Invoke();
    }
}