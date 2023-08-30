namespace Brupper.AspNetCore.Models;

public class GeneralConfirmViewModel
{
    public string PrimaryKey { get; set; } = default!;

    public string SummaryText { get; set; } = default!;// = Resources.Labels.general_confirm_dialog_question;

    public string ConfirmAndCloseText { get; set; } = default!;// = Resources.Labels.general_save_close;

    public string ConfirmText { get; set; } = default!; // = Resources.Labels.general_confirm_dialog_ok;

    public string CancelText { get; set; } = default!; // = Resources.Labels.general_confirm_dialog_cancel;

    public string? Area { get; set; }

    public string? Controller { get; set; }

    public string Action { get; set; } = "index";
}
