using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Models;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using System;
using System.Globalization;

namespace Lombiq.DataTables.Tests.UI.Models;

public class TableDrawState : IWebContentState
{
    private readonly UITestContext _context;
    private readonly By _by;
    private readonly int _draw;

    public TableDrawState(UITestContext context, By wrapper = null)
    {
        _context = context;
        _by = wrapper ?? By.ClassName("dataTables_wrapper");
        _draw = GetCurrentDraw();
    }

    public bool CheckIfNavigationHasOccurred() => _draw < GetCurrentDraw();

    public void Wait(TimeSpan? timeout = null, TimeSpan? interval = null) =>
        _context.DoWithRetriesOrFail(CheckIfNavigationHasOccurred, timeout, interval);

    private int GetCurrentDraw()
    {
        var dataDraw = _context.Get(_by).GetAttribute("data-draw");
        return int.Parse(dataDraw, CultureInfo.InvariantCulture);
    }
}
