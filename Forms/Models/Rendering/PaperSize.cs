namespace Brupper.Forms.Models.Rendering
{
    public class PaperSize
    {
        public const double ExperimentalA4Ratio = 1.4142f; // Real: 1.4142f

        public PaperSize(PaperKind kind)
        {
            Kind = kind;

            switch (Kind)
            {
                case PaperKind.A4:
                    // https://www.graphic-design-employment.com/a4-paper-dimensions.html
                    /*
                     Equivalent A4 paper dimensions in pixels at 300 DPI and 72 DPI respectively are:

                    2480 pixels x 3508 pixels (print resolution)
                    595 pixels x 842 pixels (screen resolution)
                     */
                    Width = 793;
                    Height = 1122;//(int)(793 * ExperimentalA4Ratio); // 1121,4606
                    PageRatio = ExperimentalA4Ratio;
                    break;
                case PaperKind.Pos_104mm_832px:
                    Width = 832;
                    break;
                case PaperKind.Pos_80mm_576px:
                    Width = 576;
                    break;
                default:
                    break;
            }
        }

        public double PageRatio { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public PaperKind Kind { get; }

        //public string PaperName { get; set; }

        //public int RawKind { get; set; }
    }
}
