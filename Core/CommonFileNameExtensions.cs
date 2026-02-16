using System;

namespace Brupper;

public sealed class CommonFileNameExtensions(string name) : IEquatable<CommonFileNameExtensions>
{
    #region Members

    /// <summary> Windows audio file</summary>
    public static CommonFileNameExtensions aac { get; private set; } = new(nameof(aac));
    /// <summary> Windows audio file</summary>
    public static CommonFileNameExtensions adt { get; private set; } = new(nameof(adt));
    /// <summary> Windows audio file</summary>
    public static CommonFileNameExtensions adts { get; private set; } = new(nameof(adts));
    /// <summary> Microsoft Access database file</summary>
    public static CommonFileNameExtensions accdb { get; private set; } = new(nameof(accdb));
    /// <summary> Microsoft Access execute-only file</summary>
    public static CommonFileNameExtensions accde { get; private set; } = new(nameof(accde));
    /// <summary> Microsoft Access runtime database</summary>
    public static CommonFileNameExtensions accdr { get; private set; } = new(nameof(accdr));
    /// <summary> Microsoft Access database template</summary>
    public static CommonFileNameExtensions accdt { get; private set; } = new(nameof(accdt));
    /// <summary> Audio Interchange File format file</summary>
    public static CommonFileNameExtensions aif { get; private set; } = new(nameof(aif));
    /// <summary> Audio Interchange File format file</summary>
    public static CommonFileNameExtensions aifc { get; private set; } = new(nameof(aifc));
    /// <summary> Audio Interchange File format file</summary>
    public static CommonFileNameExtensions aiff { get; private set; } = new(nameof(aiff));
    /// <summary> ASP.NET Active Server page</summary>
    public static CommonFileNameExtensions aspx { get; private set; } = new(nameof(aspx));
    /// <summary> Audio Video Interleave movie or sound file</summary>
    public static CommonFileNameExtensions avi { get; private set; } = new(nameof(avi));
    /// <summary> PC batch file</summary>
    public static CommonFileNameExtensions bat { get; private set; } = new(nameof(bat));
    /// <summary> Binary compressed file</summary>
    public static CommonFileNameExtensions bin { get; private set; } = new(nameof(bin));
    /// <summary> Bitmap file</summary>
    public static CommonFileNameExtensions bmp { get; private set; } = new(nameof(bmp));
    /// <summary> Windows Cabinet file</summary>
    public static CommonFileNameExtensions cab { get; private set; } = new(nameof(cab));
    /// <summary> CD Audio Track</summary>
    public static CommonFileNameExtensions cda { get; private set; } = new(nameof(cda));
    /// <summary> Comma-separated values file</summary>
    public static CommonFileNameExtensions csv { get; private set; } = new(nameof(csv));
    /// <summary> Spreadsheet data interchange format file</summary>
    public static CommonFileNameExtensions dif { get; private set; } = new(nameof(dif));
    /// <summary> Dynamic Link Library file</summary>
    public static CommonFileNameExtensions dll { get; private set; } = new(nameof(dll));
    /// <summary> Microsoft Word document before Word 2007</summary>
    public static CommonFileNameExtensions doc { get; private set; } = new(nameof(doc));
    /// <summary> Microsoft Word macro-enabled document</summary>
    public static CommonFileNameExtensions docm { get; private set; } = new(nameof(docm));
    /// <summary> Microsoft Word document</summary>
    public static CommonFileNameExtensions docx { get; private set; } = new(nameof(docx));
    /// <summary> Microsoft Word template before Word 2007</summary>
    public static CommonFileNameExtensions dot { get; private set; } = new(nameof(dot));
    /// <summary> Microsoft Word template</summary>
    public static CommonFileNameExtensions dotx { get; private set; } = new(nameof(dotx));
    /// <summary> Email file created by Outlook Express, Windows Live Mail, and other programs</summary>
    public static CommonFileNameExtensions eml { get; private set; } = new(nameof(eml));
    /// <summary> Encapsulated Postscript file</summary>
    public static CommonFileNameExtensions eps { get; private set; } = new(nameof(eps));
    /// <summary> Executable program file</summary>
    public static CommonFileNameExtensions exe { get; private set; } = new(nameof(exe));
    /// <summary> Flash-compatible video file</summary>
    public static CommonFileNameExtensions flv { get; private set; } = new(nameof(flv));
    /// <summary> Graphical Interchange Format file</summary>
    public static CommonFileNameExtensions gif { get; private set; } = new(nameof(gif));
    /// <summary> Hypertext markup language page</summary>
    public static CommonFileNameExtensions htm { get; private set; } = new(nameof(htm));
    /// <summary> Hypertext markup language page</summary>
    public static CommonFileNameExtensions html { get; private set; } = new(nameof(html));
    /// <summary> Windows initialization configuration file</summary>
    public static CommonFileNameExtensions ini { get; private set; } = new(nameof(ini));
    /// <summary> ISO-9660 disc image</summary>
    public static CommonFileNameExtensions iso { get; private set; } = new(nameof(iso));
    /// <summary> Java architecture file</summary>
    public static CommonFileNameExtensions jar { get; private set; } = new(nameof(jar));
    /// <summary> Joint Photographic Experts Group photo file</summary>
    public static CommonFileNameExtensions jpg { get; private set; } = new(nameof(jpg));
    /// <summary> Joint Photographic Experts Group photo file</summary>
    public static CommonFileNameExtensions jpeg { get; private set; } = new(nameof(jpeg));
    /// <summary> MPEG-4 audio file</summary>
    public static CommonFileNameExtensions m4a { get; private set; } = new(nameof(m4a));
    /// <summary> Microsoft Access database before Access 2007</summary>
    public static CommonFileNameExtensions mdb { get; private set; } = new(nameof(mdb));
    /// <summary> Musical Instrument Digital Interface file</summary>
    public static CommonFileNameExtensions mid { get; private set; } = new(nameof(mid));
    /// <summary> Musical Instrument Digital Interface file</summary>
    public static CommonFileNameExtensions midi { get; private set; } = new(nameof(midi));
    /// <summary> Apple QuickTime movie file</summary>
    public static CommonFileNameExtensions mov { get; private set; } = new(nameof(mov));
    /// <summary> MPEG layer 3 audio file</summary>
    public static CommonFileNameExtensions mp3 { get; private set; } = new(nameof(mp3));
    /// <summary> MPEG 4 video</summary>
    public static CommonFileNameExtensions mp4 { get; private set; } = new(nameof(mp4));
    /// <summary> Moving Picture Experts Group movie file</summary>
    public static CommonFileNameExtensions mpeg { get; private set; } = new(nameof(mpeg));
    /// <summary> MPEG 1 system stream</summary>
    public static CommonFileNameExtensions mpg { get; private set; } = new(nameof(mpg));
    /// <summary> Microsoft installer file</summary>
    public static CommonFileNameExtensions msi { get; private set; } = new(nameof(msi));
    /// <summary> Multilingual User Interface file</summary>
    public static CommonFileNameExtensions mui { get; private set; } = new(nameof(mui));
    /// <summary> Portable Document Format file</summary>
    public static CommonFileNameExtensions pdf { get; private set; } = new(nameof(pdf));
    /// <summary> Portable Network Graphics file</summary>
    public static CommonFileNameExtensions png { get; private set; } = new(nameof(png));
    /// <summary> Microsoft PowerPoint template before PowerPoint 2007</summary>
    public static CommonFileNameExtensions pot { get; private set; } = new(nameof(pot));
    /// <summary> Microsoft PowerPoint macro-enabled template</summary>
    public static CommonFileNameExtensions potm { get; private set; } = new(nameof(potm));
    /// <summary> Microsoft PowerPoint template</summary>
    public static CommonFileNameExtensions potx { get; private set; } = new(nameof(potx));
    /// <summary> Microsoft PowerPoint add-in</summary>
    public static CommonFileNameExtensions ppam { get; private set; } = new(nameof(ppam));
    /// <summary> Microsoft PowerPoint slideshow before PowerPoint 2007</summary>
    public static CommonFileNameExtensions pps { get; private set; } = new(nameof(pps));
    /// <summary> Microsoft PowerPoint macro-enabled slideshow</summary>
    public static CommonFileNameExtensions ppsm { get; private set; } = new(nameof(ppsm));
    /// <summary> Microsoft PowerPoint slideshow</summary>
    public static CommonFileNameExtensions ppsx { get; private set; } = new(nameof(ppsx));
    /// <summary> Microsoft PowerPoint format before PowerPoint 2007</summary>
    public static CommonFileNameExtensions ppt { get; private set; } = new(nameof(ppt));
    /// <summary> Microsoft PowerPoint macro-enabled presentation</summary>
    public static CommonFileNameExtensions pptm { get; private set; } = new(nameof(pptm));
    /// <summary> Microsoft PowerPoint presentation</summary>
    public static CommonFileNameExtensions pptx { get; private set; } = new(nameof(pptx));
    /// <summary> Adobe Photoshop file</summary>
    public static CommonFileNameExtensions psd { get; private set; } = new(nameof(psd));
    /// <summary> /// <summary> Outlook data store</summary>
    public static CommonFileNameExtensions pst { get; private set; } = new(nameof(pst));
    /// <summary> Microsoft Publisher file</summary>
    public static CommonFileNameExtensions pub { get; private set; } = new(nameof(pub));
    /// <summary> Roshal Archive compressed file</summary>
    public static CommonFileNameExtensions rar { get; private set; } = new(nameof(rar));
    /// <summary> Rich Text Format file</summary>
    public static CommonFileNameExtensions rtf { get; private set; } = new(nameof(rtf));
    /// <summary> Microsoft PowerPoint macro-enabled slide</summary>
    public static CommonFileNameExtensions sldm { get; private set; } = new(nameof(sldm));
    /// <summary> Microsoft PowerPoint slide</summary>
    public static CommonFileNameExtensions sldx { get; private set; } = new(nameof(sldx));
    /// <summary> Shockwave Flash file</summary>
    public static CommonFileNameExtensions swf { get; private set; } = new(nameof(swf));
    /// <summary> Microsoft DOS and Windows system settings and variables file</summary>
    public static CommonFileNameExtensions sys { get; private set; } = new(nameof(sys));
    /// <summary> Tagged Image Format file</summary>
    public static CommonFileNameExtensions tif { get; private set; } = new(nameof(tif));
    /// <summary> Tagged Image Format file</summary>
    public static CommonFileNameExtensions tiff { get; private set; } = new(nameof(tiff));
    /// <summary> Temporary data file</summary>
    public static CommonFileNameExtensions tmp { get; private set; } = new(nameof(tmp));
    /// <summary> Unformatted text file</summary>
    public static CommonFileNameExtensions txt { get; private set; } = new(nameof(txt));
    /// <summary> Video object file</summary>
    public static CommonFileNameExtensions vob { get; private set; } = new(nameof(vob));
    /// <summary> Microsoft Visio drawing before Visio 2013</summary>
    public static CommonFileNameExtensions vsd { get; private set; } = new(nameof(vsd));
    /// <summary> Microsoft Visio macro-enabled drawing</summary>
    public static CommonFileNameExtensions vsdm { get; private set; } = new(nameof(vsdm));
    /// <summary> Microsoft Visio drawing file</summary>
    public static CommonFileNameExtensions vsdx { get; private set; } = new(nameof(vsdx));
    /// <summary> Microsoft Visio stencil before Visio 2013</summary>
    public static CommonFileNameExtensions vss { get; private set; } = new(nameof(vss));
    /// <summary> Microsoft Visio macro-enabled stencil</summary>
    public static CommonFileNameExtensions vssm { get; private set; } = new(nameof(vssm));
    /// <summary> Microsoft Visio template before Visio 2013</summary>
    public static CommonFileNameExtensions vst { get; private set; } = new(nameof(vst));
    /// <summary> Microsoft Visio macro-enabled template</summary>
    public static CommonFileNameExtensions vstm { get; private set; } = new(nameof(vstm));
    /// <summary> Microsoft Visio template</summary>
    public static CommonFileNameExtensions vstx { get; private set; } = new(nameof(vstx));
    /// <summary> Wave audio file</summary>
    public static CommonFileNameExtensions wav { get; private set; } = new(nameof(wav));
    /// <summary> Microsoft Word backup document</summary>
    public static CommonFileNameExtensions wbk { get; private set; } = new(nameof(wbk));
    /// <summary> Microsoft Works file</summary>
    public static CommonFileNameExtensions wks { get; private set; } = new(nameof(wks));
    /// <summary> Windows Media Audio file</summary>
    public static CommonFileNameExtensions wma { get; private set; } = new(nameof(wma));
    /// <summary> Windows Media Download file</summary>
    public static CommonFileNameExtensions wmd { get; private set; } = new(nameof(wmd));
    /// <summary> Windows Media Video file</summary>
    public static CommonFileNameExtensions wmv { get; private set; } = new(nameof(wmv));
    /// <summary> Windows Media skins file</summary>
    public static CommonFileNameExtensions wmz { get; private set; } = new(nameof(wmz));
    /// <summary> Windows Media skins file</summary>
    public static CommonFileNameExtensions wms { get; private set; } = new(nameof(wms));
    /// <summary> WordPerfect document</summary>
    public static CommonFileNameExtensions wpd { get; private set; } = new(nameof(wpd));
    /// <summary> WordPerfect document</summary>
    public static CommonFileNameExtensions wp5 { get; private set; } = new(nameof(wp5));
    /// <summary> Microsoft Excel add-in or macro file</summary>
    public static CommonFileNameExtensions xla { get; private set; } = new(nameof(xla));
    /// <summary> Microsoft Excel add-in after Excel 2007</summary>
    public static CommonFileNameExtensions xlam { get; private set; } = new(nameof(xlam));
    /// <summary> Microsoft Excel DLL-based add-in</summary>
    public static CommonFileNameExtensions xll { get; private set; } = new(nameof(xll));
    /// <summary> Microsoft Excel macro before Excel 2007</summary>
    public static CommonFileNameExtensions xlm { get; private set; } = new(nameof(xlm));
    /// <summary> Microsoft Excel workbook before Excel 2007</summary>
    public static CommonFileNameExtensions xls { get; private set; } = new(nameof(xls));
    /// <summary> Microsoft Excel macro-enabled workbook after Excel 2007</summary>
    public static CommonFileNameExtensions xlsm { get; private set; } = new(nameof(xlsm));
    /// <summary> Microsoft Excel workbook after Excel 2007</summary>
    public static CommonFileNameExtensions xlsx { get; private set; } = new(nameof(xlsx));
    /// <summary> Microsoft Excel template before Excel 2007</summary>
    public static CommonFileNameExtensions xlt { get; private set; } = new(nameof(xlt));
    /// <summary> Microsoft Excel macro-enabled template after Excel 2007</summary>
    public static CommonFileNameExtensions xltm { get; private set; } = new(nameof(xltm));
    /// <summary> Microsoft Excel template after Excel 2007</summary>
    public static CommonFileNameExtensions xltx { get; private set; } = new(nameof(xltx));
    /// <summary> XML-based document</summary>
    public static CommonFileNameExtensions xps { get; private set; } = new(nameof(xps));
    /// <summary> Compressed file</summary>
    public static CommonFileNameExtensions zip { get; private set; } = new(nameof(zip));


    #endregion

    public string Name { get; private set; } = name;

    #region IEquatable<CommonFileNameExtensions> Members

    public bool Equals(CommonFileNameExtensions? other)
    {
        if (other == null)
        {
            return false;
        }

        if (object.ReferenceEquals(Name, other.Name))
        {
            // Strings are interned, so there is a good chance that two equal Names use the same reference
            // (unless they differ in case).
            return true;
        }

        return (string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase) == 0);
    }

    #endregion

    public override bool Equals(object obj) => Equals(obj as CommonFileNameExtensions);

    public override int GetHashCode() => Name.ToUpperInvariant().GetHashCode();

    public override string ToString() => Name.ToString();

    public static bool operator ==(CommonFileNameExtensions? left, CommonFileNameExtensions? right)
    {
        if (left == null)
        {
            return (right == null);
        }
        else if (right == null)
        {
            return (left == null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(CommonFileNameExtensions? left, CommonFileNameExtensions? right)
    {
        return !(left == right);
    }
}
