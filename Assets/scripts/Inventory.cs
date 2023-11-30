using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class Inventory
{
    static List<string> _contents = new List<string>();

    public static List<string> Contents { get => _contents; }

    public static void Add(string value)
    {
        _contents.Add(value);
    }

    public static void Remove(string value)
    {
        if (_contents.Contains(value))
        {
            _contents.Remove(value);
        }
    }
}
