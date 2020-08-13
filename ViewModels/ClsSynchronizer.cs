﻿#region "                   名稱空間"

using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.Entities;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
#endregion

namespace BCS.CADs.Synchronization.ViewModels
{
    public static class ClsSynchronizer
    {
        #region "                   宣告區"
        public static ObservableCollection<SearchItem> _vmObsSearchItems;

        public static ObservableCollection<SearchItem> _vmObsDialogSearchItems;
        #endregion

        #region "                   屬性"

        public static CADsSynchronizer VmSyncCADs { get; set; } = new CADsSynchronizer();

        public static VMCommon VmCommon { get; set; } = new VMCommon();

        public static SyncType VmFunction { get; set; }

        public static SyncOperation VmOperation { get; set; }

        public static SyncMessages VmMessages { get; set; }

        public static string VmDirectory { get; set; } = "";

        public static string VmSelectedItemId { get; set; } = "";

        public static CultureInfo CurrentCultureInfo { get; set; }

        public static ResourceDictionary LangResource { get; set; }

        public static ObservableCollection<ClassPlugin> ClassPlugins { get; set; }

        public static List<SearchItem> SearchItemsList { get; set; }

        public static ObservableCollection<SearchItem> SearchItemsCollection { get; set; }

        public static ObservableCollection<SearchItem> DialogSearchItemsCollection { get; set; }

        public static ObservableCollection<SearchItemsViewModel> TreeSearchItemsCollection { get; set; }

        public static SearchItem ActiveSearchItem { get; set; }

        public static SearchItem NewSearchItem { get; set; }

        public static SearchItem NewSubSearchItem { get; set; }

        public static dynamic SyncListView { get; set; }

        public static dynamic SyncSubListView { get; set; }

        public static dynamic SyncDialogView { get; set; }

        public static dynamic SyncSubDialogView { get; set; }

        public static bool IsActiveSubDialogView { get; set; } = false;

        public static dynamic EditPropertiesView { get; set; }

        public static dynamic ItemFilterSearchView { get; set; }


        public static dynamic TreeStructureView { get; set; }

        public static dynamic SyncMessagesView { get; set; }

        public static dynamic LoadingAdornerView { get; set; }

        public static dynamic MainWindows { get; set; }

        public static Window ActiveWindow { get; set; }

        public static bool IsShowDialog { get; set; } = false;

        public static string ShowDialogItemType { get; set; } = "";

        public static string CurrentDialog { get; set; } = "";

        public static string DialogReturnValue { get; set; } = "";

        public static string DialogReturnKeyedName { get; set; } = "";

        public static string SubDialogReturnValue { get; set; } = "";

        public static string SubDialogReturnKeyedName { get; set; } = "";

        public static List<StructuralChange> ItemStructureChanges { get; set; } = new List<StructuralChange>();

        public static string ViewFilePath { get; set; } = "";

        public static string Language { get; set; } = "";

        public static ObservableCollection<ClassTemplateFile> ClassTemplateFiles { get; set; }

        public static string Status { get; set; } = "";

        public static Style RowStyle { get; set; }

        public static ItemType SearchItemTypeItem { get; set; }

        //public enum SyncImages'
        public static Dictionary<string, string> SyncImages =new Dictionary<string, string>(){
            {"NewFormTemplateFile", "syncFromPLMDrawingImageIsEnableUse"}, {"SyncFromPLM", "syncFromPLMDrawingImageIsEnableUse"},{"SyncToPLM", "syncToPLMDrawingImageIsEnableUse"}, {"LoadFromPLM", "ConversionServerDrawingImageIsEnableUse"}, {"CopyToAdd", "CopyToAddDrawingImageStyleImageIsEnableUse"}, {"CopyToAddSearch", "CopyToAddDrawingImageStyleImageIsEnableUse"},{"LockOrUnlock", "FlaggedByDrawingImageIsEnableUse"}, {"PluginModule", "CreateRelatedItemDrawingImageIsEnableUse"},
            {"QueryListItems", "ExecuteSearchDrawingImage"}, {"EditorProperties", "ReplicationTxnLogDrawingImage"},{"CADStructure", "BlockExternalDrawingImage"}, {"AddTemplates", "ReplicationTxnLogDrawingImage"},{"AddOnItems", "ExecuteSearchDrawingImage"},{"LoadListItems", "ExecuteSearchDrawingImage"}, {"File","File" },
            {"Executing", "Executing"},{"End", "End"},{"Error", "Error"},{"Finish", "Finish"}
        };
        //public static Frame ViewPage { get; set; } = null;

        


        #endregion



        //SyncOperation
    }
}
