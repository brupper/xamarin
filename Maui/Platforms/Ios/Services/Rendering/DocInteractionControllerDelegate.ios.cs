using UIKit;

namespace Brupper.Maui.Platforms.iOS;

public class DocInteractionControllerDelegate : UIDocumentInteractionControllerDelegate
{
    readonly UIViewController m_oParentViewController;

    public DocInteractionControllerDelegate(UIViewController controller)
    {
        m_oParentViewController = controller;
    }

    public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
    {
        return m_oParentViewController;
    }

    public override UIView ViewForPreview(UIDocumentInteractionController controller)
    {
        return m_oParentViewController.View;
    }
}