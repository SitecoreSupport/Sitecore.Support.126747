using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Sitecore.Pipelines;
using Sitecore.Web;

namespace Sitecore.Support.Pipelines.HealthMonitor
{
  public class HealthMonitor : Sitecore.Pipelines.HealthMonitor.HealthMonitor
  {
    public override void DumpRenderingsStatistics(PipelineArgs args)
    {

      HtmlTable htmlTable = new HtmlTable
      {
        Border = 1,
        CellPadding = 2
      };
      HtmlUtil.AddRow(htmlTable, new string[]
      {
        "Rendering",
        "Site",
        "Count",
        "From cache",
        "Avg. time (ms)",
        "Avg. items",
        "Max. time",
        "Max. items",
        "Total time",
        "Total items",
        "Last run"
      });
      SortedList<string, Diagnostics.Statistics.RenderingData> sortedList = new SortedList<string, Diagnostics.Statistics.RenderingData>();
      foreach (Diagnostics.Statistics.RenderingData current in Diagnostics.Statistics.RenderingStatistics)
      {
        sortedList.Add(current.SiteName + 255 + current.TraceName, current);
      }
      foreach (Diagnostics.Statistics.RenderingData current2 in sortedList.Values)
      {
        HtmlUtil.AddRow(htmlTable, new object[]
        {
          current2.TraceName,
          current2.SiteName,
          current2.RenderCount,
          current2.UsedCache,
          current2.AverageTime.TotalMilliseconds,
          current2.AverageItemsAccessed,
          current2.MaxTime.TotalMilliseconds,
          current2.MaxItemsAccessed,
          current2.TotalTime,
          current2.TotalItemsAccessed,
          DateUtil.ToServerTime(current2.LastRendered)
        });
      }
      string text;
      using (StringWriter stringWriter = new StringWriter())
      {
        htmlTable.RenderControl(new HtmlTextWriter(stringWriter));
        text = stringWriter.ToString();
      }
      this.DumpInFile(text);
    }
  }
}