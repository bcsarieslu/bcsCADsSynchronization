using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCS.CADs.Synchronization.Models
{
    /// <summary>
    /// 是否要執行Lock
    /// </summary>
    public enum IsLock { True = 1, False = 0, None = -1 };

    /// <summary>
    /// 是否要執行產生版次
    /// </summary>
    public enum IsNewGeneration { True = 1, False = 0, None = -1 };


    //OnGetPLMStructure;OnGetCADsProperties;OnUpdateCADStructure

    /// <summary>
    /// 程式相關事件
    /// </summary>
    public enum SyncEvents
    {
        OnAddTemplate,
        OnExportFiles,
        OnGetPLMStructure,
        OnGetCADStructure,
        OnGetCADsProperties,
        OnUpdateCADStructure,
        OnUpdateCADsProperties,
        OnAddBefore,
        OnAddAfter,
        OnAddItemBefore,
        OnAddItemAfter,
        OnAddFromTemplateBefore,
        OnAddFromTemplateAfter,
        OnAddFromTemplateItemBefore,
        OnAddFromTemplateItemAfter,
        OnLoadFromPLMDownloadBefore,
        OnLoadFromPLMDownloadAfter,
        OnLoadFromPLMDownloadItemBefore,
        OnLoadFromPLMDownloadItemAfter,
        OnCopyToAddBefore,
        OnCopyToAddAfter,
        OnCopyToAddItemBefore,
        OnCopyToAddItemAfter,
        OnLockItemsBefore,
        OnLockItemsAfter,
        OnLockItemBefore,
        OnLockItemAfter,
        OnSyncFormPLMBefore,
        OnSyncFormPLMAfter,
        OnSyncFormPLMItemBefore,
        OnSyncFormPLMItemAfter,
        OnSyncFormStructureBefore,
        OnSyncFormStructureAfter,
        OnSyncFormItemStructureBefore,
        OnSyncFormItemStructureAfter,
        OnSyncToPLMBefore,
        OnSyncToPLMAfter,
        OnSyncToPLMItemBefore,
        OnSyncToPLMItemAfter,
        OnSyncToPLMStructureBefore,
        OnSyncToPLMStructureAfter,
        OnSyncToPLMItemStructureBefore,
        OnSyncToPLMItemStructureAfter,
        //onUploadsBefore,
        //onUploadAfter,
        //onUploadItemBefore,
        //onUploadItemAfter,
        OnUnlockItemsBefore,
        OnUnlockItemsAfter,
        OnUnlockItemBefore,
        OnUnlockItemAfter,
        OpenFile,
        OpenFiles,
        CloseFiles,
        UserLogin,
        None
    };

    /// <summary>
    /// 程式觸發一些事件,對應Client CAD程式的方法
    /// </summary>
    public enum SyncCadCommands { IsActiveCAD, ExportPropertyFile, AddTemplate, CopyToAdd, OpenFile, OpenFiles, MoveFiles, CloseFiles, GetActiveCADDocument, GetActiveCADStructure, UpdateCADKeys, UpdateCADsProperties, GetSelectedCADsProperties, GetCADsProperties, RunFunction, SystemSyncToPLMProperties, SystemSyncFromPLMProperties, SystemSyncToPLMStructure, SystemSyncFromPLMStructure, SystemAddFromTemplate, SystemItemLocked, SystemLogin, SystemLoadFromPLM, SystemCopyToAdd }

    /// <summary>
    /// 功能事件
    /// </summary>
    public enum SyncType { Login, NewFormTemplateFile, SyncFromPLM, SyncToPLM, LoadFromPLM, CopyToAddSearch, CopyToAdd, LockOrUnlock, PluginModule, Structure, Properties, Locked, Add, Download, Unlocked, DownloadFile, Files,None }

    /// <summary>
    /// 物件類型定義,但要replace _為空白
    /// </summary>
    public enum SyncLinkItemTypes { CAD_Structure, BCS_CAD_Drawing}

    /// <summary>
    /// CAD的Key的名稱屬性
    /// </summary>
    public enum ProductKeys { BCS_CAD_CONFIG_ID, BCS_CAD_ITEM_ID }

    /// <summary>
    /// 同步屬性編輯的文字背景顏色
    /// </summary>
    public enum SyncColorType { Defalue = 0, SyncDifferentValues = 1, IsRequired = 2 }

    /// <summary>
    /// 操作功能
    /// </summary>
    public enum SyncOperation { None, QueryListItems, EditorProperties, CADStructure, AddTemplates, AddOnItems, LoadListItems, CopyToAddSearch }

    public enum SpecialFeatures
    {
        None,
        Color,
        Highlight
    }

    /// <summary>
    /// CAD結構變動型態:(1)插入分件(2)替換分件(3)複製轉新增(4)插入另存分件
    /// </summary>
    public enum ChangeType
    {
        Insert,
        Replacement,
        CopyToAdd,
        InsertSaveAs
    }

    /// <summary>
    /// 對應到圖示:(1)無:通過(2)鎖定物件為自己(3)鎖定物件為其他人
    /// </summary>
    public enum SyncAccessRights { None, FlaggedByMe, FlaggedByOthers };

    /// <summary>
    /// 對應到圖示:(1)無:通過(2)物件狀態,不允許修改(3)共享物件,不允許修改(4)其他屬性驗證是禁止修改
    /// </summary>
    public enum SyncRestrictedStatus { None, NoModification, SharedNoModification, Prohibited };

    /// <summary>
    /// 對應到圖示:(1)無:尚未比較(2)最新版本(3)不是最新版本(4)物件不存在系統
    /// </summary>
    public enum SyncVersionStatus { Uncompared, LatestVersion, NonLatestVersion, NonSystem };

    public enum SearchType { Search, CADRevisionSearch, CADAllRevisionsSearch }

    public enum ItemTypeName {CAD,File }

    public enum ClassName { Assembly, Part, Drawing };
}
