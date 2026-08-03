using System;
using System.Collections.Generic;
using BulletDevil.Utilities;

public class GraphicsResourceContainer<T> : IDisposable where T : class, IDisposable
{
    private Dictionary<string, T> resourceDictionary = [];

    public bool ContainsKey(string key) => resourceDictionary.ContainsKey(key);

    public bool TryAdd(string key, Lazy<T> value)
    {
        if (!resourceDictionary.ContainsKey(key))
        {
            resourceDictionary.Add(key, value.Value);

            return true;
        }
        else
        {
            Utils.ThrowWarning(this, $"Could not add {typeof(T)}, because the given key \'{key}\' already exists!");

            return false;
        }
    }

    public T GetResource(string key) => resourceDictionary[key];

    public bool TryGet(string key, out T resource) => resourceDictionary.TryGetValue(key, out resource);

    #region IDisposable Implementation

    private bool disposed = false;

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            // Dispose of graphics resources
            foreach (KeyValuePair<string, T> keyResourcePair in resourceDictionary)
            {
                resourceDictionary.Remove(keyResourcePair.Key, out T resource);
                resource.Dispose();
            }

            resourceDictionary = null;

            disposed = true;
        }
    }

    ~GraphicsResourceContainer()
    {
        if (!disposed)
        {
            Utils.ThrowError(this, "GPU resource leak! Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}