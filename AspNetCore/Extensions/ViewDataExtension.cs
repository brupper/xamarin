using Brupper.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class ViewDataExtensions
{
    public static GeneralConfirmViewModel AddToViewData(this GeneralConfirmViewModel backModel, string key, ViewDataDictionary viewData, IUrlHelper url)
    {
        viewData[key] = url.Action(backModel.Action, backModel.Controller, new { area = backModel.Area, id = backModel.PrimaryKey });

        return backModel;
    }
}
