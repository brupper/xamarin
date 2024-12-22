using System.Collections.Generic;

namespace Brupper;

/// <summary> </summary>
[System.Diagnostics.DebuggerDisplay("{Id,nq} - {LanguageCode,nq}")]
public class LabelTranslation
{
    public string Id { get; set; } = default!;

    public string LanguageCode { get; set; } = default!;

    public string? Text { get; set; }
}

/// <summary> </summary>
[System.Diagnostics.DebuggerDisplay("{Id,nq} - {DefaultValue,nq}")]
public class TranslatableLabel
{
    public string Id { get; set; } = default!;

    public string? DefaultValue { get; set; }

    public List<LabelTranslation>? LabelTranslations { get; set; } = [];
}