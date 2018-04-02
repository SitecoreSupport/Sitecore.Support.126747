namespace Sitecore.Support.sitecore.admin
{
  using Sitecore;
  using Sitecore.Collections;
  using Sitecore.Diagnostics;
  using Sitecore.Web;
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  /// <summary>
  /// The statistics page.
  /// </summary>
  public class stats : Sitecore.sitecore.admin.stats
  {
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="arguments">
    /// The <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected new void Page_Load(object sender, EventArgs arguments)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(arguments, "arguments");
      base.CheckSecurity(true);
      this.ShowSiteSelector();
      this.ShowRenderingStats(base.Request.QueryString["site"]);
    }

    /// <summary>
    /// Gets the site names.
    /// </summary>
    /// <returns>
    /// The site names.
    /// </returns>
    private static string[] GetSiteNames()
    {
      List<string> list = new List<string>();
      foreach (Statistics.RenderingData renderingStatistic in Statistics.RenderingStatistics)
      {
        if (!list.Contains(renderingStatistic.SiteName))
        {
          list.Add(renderingStatistic.SiteName);
        }
      }
      return list.ToArray();
    }

    /// <summary>
    /// Shows the rendering stats.
    /// </summary>
    /// <param name="siteName">
    /// Name of the site.
    /// </param>
    private void ShowRenderingStats(string siteName)
    {
      HtmlTable htmlTable = new HtmlTable();
      htmlTable.Border = 1;
      htmlTable.CellPadding = 2;
      HtmlTable htmlTable2 = htmlTable;
      HtmlUtil.AddRow(htmlTable2, "Rendering", "Site", "Count", "From cache", "Avg. time (ms)", "Avg. items", "Max. time", "Max. items", "Total time", "Total items", "Last run");
      SortedList<string, Statistics.RenderingData> sortedList = new SortedList<string, Statistics.RenderingData>();
      #region Modified code
      SafeDictionary<string, Statistics.RenderingData> renderingData = typeof(Diagnostics.Statistics).GetField("_renderingData", BindingFlags.Static | BindingFlags.NonPublic)
        .GetValue(null) as Sitecore.Collections.SafeDictionary<string, Statistics.RenderingData>;
      lock (renderingData.SyncRoot)
      {
        foreach (Statistics.RenderingData renderingStatistic in Statistics.RenderingStatistics)
        {
          if (siteName == null || renderingStatistic.SiteName.Equals(siteName, StringComparison.OrdinalIgnoreCase))
          {
            sortedList.Add(renderingStatistic.SiteName + 255 + renderingStatistic.TraceName, renderingStatistic);
          }
        }
      }
      #endregion
      foreach (Statistics.RenderingData value in sortedList.Values)
      {
        HtmlTableRow htmlTableRow = HtmlUtil.AddRow(htmlTable2, value.TraceName, value.SiteName, value.RenderCount, value.UsedCache, value.AverageTime.TotalMilliseconds, value.AverageItemsAccessed, value.MaxTime.TotalMilliseconds, value.MaxItemsAccessed, value.TotalTime, value.TotalItemsAccessed, DateUtil.ToServerTime(value.LastRendered));
        for (int i = 2; i < htmlTableRow.Cells.Count; i++)
        {
          htmlTableRow.Cells[i].Align = "right";
        }
      }
      this.renderings.Controls.Add(htmlTable2);
    }

    /// <summary>
    /// Shows the site selector.
    /// </summary>
    private void ShowSiteSelector()
    {
      string[] siteNames = stats.GetSiteNames();
      Array.Sort(siteNames);
      HtmlTable htmlTable = HtmlUtil.CreateTable(1, siteNames.Length + 1);
      htmlTable.Border = 0;
      htmlTable.CellPadding = 5;
      htmlTable.Rows[0].Cells[0].InnerHtml = "<a href=\"?\">All sites</a>";
      int num = 1;
      string[] array = siteNames;
      foreach (string arg in array)
      {
        htmlTable.Rows[0].Cells[num].InnerHtml = string.Format("<a href=\"?site={0}\">{0}</a>", arg);
        num++;
      }
      this.siteSelector.Controls.Add(htmlTable);
    }
  }
}