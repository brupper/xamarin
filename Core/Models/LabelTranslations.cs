using System.Collections.Generic;

namespace Brupper;

/// <summary> </summary>
[System.Diagnostics.DebuggerDisplay("{Id,nq} - {LanguageCode,nq}")]
public class LabelTranslation
{
    public LabelTranslation() { }
    public LabelTranslation(string id, string code, string text)
    {
        Id = id;
        LanguageCode = code;
        Text = text;
    }
    public LabelTranslation(string code, string text)
    {
        LanguageCode = code;
        Text = text;
    }

    public string Id { get; set; } = default!;

    public string LanguageCode { get; set; } = default!;

    public string? Text { get; set; }

    public LabelTranslation GenerateId()
    {
        Id = System.Guid.NewGuid().ToString();
        return this;
    }
}

/// <summary> </summary>
[System.Diagnostics.DebuggerDisplay("{Id,nq} - {DefaultValue,nq}")]
// [Keyless]
public class TranslatableLabel
{
    public string Id { get; set; } = default!;

    public string? DefaultValue { get; set; }

    public List<LabelTranslation>? LabelTranslations { get; set; } = [];

    public TranslatableLabel GenerateId()
    {
        Id = System.Guid.NewGuid().ToString();
        return this;
    }
}