using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCS.CADs.Synchronization.ViewModels
{
    public class ThumbnailImage
    {
        /// <summary>
        /// 抓取目前選擇項目(SearchItem)的縮圖路徑
        /// </summary>
        /// <param name="searchItem"></param>
        public void GetThumbnailImagePath(SearchItem searchItem)
        {
            ClsSynchronizer.ViewFilePath = ClsSynchronizer.VmSyncCADs.GetImageFullName(searchItem, ClsSynchronizer.VmFunction);

            if (String.IsNullOrWhiteSpace(ClsSynchronizer.ViewFilePath))
            {
                if (searchItem.IsVersion == false)
                {
                    PLMProperty thumbnail = searchItem.PlmProperties.Where(y => y.Name == ClsSynchronizer.VmSyncCADs.ThumbnailProperty).FirstOrDefault();
                    if (thumbnail != null) ClsSynchronizer.ViewFilePath = ClsSynchronizer.VmSyncCADs.GetImageFullName(thumbnail.DataValue);
                }
            }

            if (String.IsNullOrWhiteSpace(ClsSynchronizer.ViewFilePath)) ClsSynchronizer.ViewFilePath = @"pack://application:,,,/BCS.CADs.Synchronization;component/Images/White.bmp";
        }
    }
}
