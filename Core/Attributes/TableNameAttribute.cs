using System;

public class TableNameAttribute : Attribute
{
    public string Name { get; set; }

    public TableNameAttribute()
    {
    }

    public TableNameAttribute(string name)
    {
        Name = name;
    }
}
