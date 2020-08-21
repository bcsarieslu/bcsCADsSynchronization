using Aras.IOM;
using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BCS.CADs.Synchronization.Entities
{
    internal class SyncCADEvents
    {


        /// <summary>
        /// 類別名稱
        /// </summary>
        private List<ClassItem> _classItems = null;
        public List<ClassItem> ClassItems {
            get { return _classItems; }
            set
            {
                _classItems = value;
            }

        }

        public ResourceDictionary LanguageResources { get; set; }

        public CommonPartsLibrary PartsLibrary { get; set; }

        public bool IsResolveAllLightweightSuppres { get; set; } = true;
        public bool IsResolveAllSuppres { get; set; } = true;

        //public SyncCADEvents(List<ClassItem> classItems)
        //{
        //    _classItems = classItems;
        //}


        //public Aras.IOM.Innovator AsInnovator { private get; set; }
        /// <summary>
        /// CAD是否有Active CAD
        /// </summary>
        /// <returns></returns>
        public bool IsActiveCADEvent()
        {

            try
            {

                SyncReachedEventArgs args = new SyncReachedEventArgs();
                args.ClassItems = _classItems;
                args.Name = SyncEvents.None;
                args.Type = SyncType.Login;
                args.IsResolveAllLightweightSuppres = IsResolveAllLightweightSuppres;
                args.IsResolveAllSuppres = IsResolveAllSuppres;
                //args.SyncMessages = ClsSynchronizer.VmMessages;
                args.LanguageResources = LanguageResources;
                args.PartsLibrary = PartsLibrary;
                args.Url = ClsSynchronizer.VmSyncCADs.GetUrl();

                SyncCommand.Invoke(SyncCadCommands.IsActiveCAD.ToString(), this, args);
                return args.IsActiveCAD;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// PLM事件觸發
        /// </summary>
        /// <param name="AsInnovator"></param>
        /// <param name="syncName"></param>
        /// <param name="searchItem"></param>
        /// <param name="integrationEvent"></param>
        /// <param name="syncEvent"></param>
        /// <param name="type"></param>
        public void ExecCadEvent(Aras.IOM.Innovator AsInnovator, string syncName, ref SearchItem searchItem, List<StructuralChange> structureChanges, IntegrationEvents integrationEvent, SyncEvents syncEvent, SyncType type)
        {

            try
            {
                List<SearchItem> searchItems = null;
                
                Aras.IOM.Item item = (String.IsNullOrWhiteSpace(searchItem.ItemId) ==false) ? AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId) : null;
                ExecCadEvent(AsInnovator, syncName, ref searchItems, ref searchItem, structureChanges, integrationEvent, syncEvent, type, ref item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// PLM事件觸發
        /// </summary>
        /// <param name="AsInnovator"></param>
        /// <param name="syncName"></param>
        /// <param name="searchItem"></param>
        /// <param name="integrationEvent"></param>
        /// <param name="syncEvent"></param>
        /// <param name="type"></param>
        public void ExecCadEvent(Aras.IOM.Innovator AsInnovator, string syncName,  SearchItem searchItem, List<StructuralChange> structureChanges, IntegrationEvents integrationEvent, SyncEvents syncEvent, SyncType type)
        {

            try
            {
                //Aras.IOM.Item item = null;
                List<SearchItem> searchItems = null;

                Aras.IOM.Item item = (String.IsNullOrWhiteSpace(searchItem.ItemId) ==false) ? AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId) : null;
                ExecCadEvent(AsInnovator, syncName, ref searchItems, ref searchItem, structureChanges, integrationEvent, syncEvent, type, ref item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// PLM事件觸發
        /// </summary>
        /// <param name="AsInnovator"></param>
        /// <param name="syncName"></param>
        /// <param name="searchItems"></param>
        /// <param name="integrationEvent"></param>
        /// <param name="syncEvent"></param>
        /// <param name="type"></param>
        public void ExecCadEvent(Aras.IOM.Innovator AsInnovator, string syncName, ref List<SearchItem> searchItems, List<StructuralChange> structureChanges, IntegrationEvents integrationEvent, SyncEvents syncEvent, SyncType type)
        {

            try
            {
                Aras.IOM.Item item = null;
                SearchItem searchItem = null;
                ExecCadEvent(AsInnovator, syncName, ref searchItems, ref searchItem, structureChanges, integrationEvent, syncEvent, type, ref item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// PLM事件觸發
        /// </summary>
        /// <param name="AsInnovator"></param>
        /// <param name="syncName"></param>
        /// <param name="searchItem"></param>
        /// <param name="integrationEvent"></param>
        /// <param name="syncEvent"></param>
        /// <param name="type"></param>
        /// <param name="item"></param>
        public void ExecCadEvent(Aras.IOM.Innovator AsInnovator, string syncName, ref SearchItem searchItem, IntegrationEvents integrationEvent, SyncEvents syncEvent, SyncType type, ref Aras.IOM.Item item)
        {

            try
            {
                List<SearchItem> searchItems = null;
                ExecCadEvent(AsInnovator, syncName,ref searchItems, ref searchItem,null, integrationEvent, syncEvent, type, ref item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// PLM事件觸發
        /// </summary>
        /// <param name="AsInnovator"></param>
        /// <param name="syncName"></param>
        /// <param name="searchItems"></param>
        /// <param name="searchItem"></param>
        /// <param name="integrationEvent"></param>
        /// <param name="syncEvent"></param>
        /// <param name="type"></param>
        /// <param name="item"></param>
        private void ExecCadEvent(Aras.IOM.Innovator AsInnovator, string syncName,ref List<SearchItem> searchItems, ref SearchItem searchItem, List<StructuralChange> structureChanges, IntegrationEvents integrationEvent, SyncEvents syncEvent, SyncType type, ref Aras.IOM.Item item)
        {

            try
            {
                
                //SyncCadCommands
                SyncReachedEventArgs args = new SyncReachedEventArgs();
                args.ClassItems = _classItems;
                args.SearchItem = searchItem;
                args.SearchItems = searchItems;
                args.Name = syncEvent;                
                args.Type = type;
                args.AsItem = item;
                args.AsInnovator = AsInnovator;
                args.IsResolveAllLightweightSuppres = IsResolveAllLightweightSuppres;
                args.IsResolveAllSuppres = IsResolveAllSuppres;
                args.FunctionName = (integrationEvent != null) ? integrationEvent.Name : "";
                args.IntegrationEvent = integrationEvent;
                args.StructureChanges = structureChanges;
                args.SyncMessages = ClsSynchronizer.VmMessages;

                args.LanguageResources = LanguageResources;
                args.PartsLibrary = PartsLibrary;
                
                args.Url = ClsSynchronizer.VmSyncCADs.GetUrl();

                SyncCommand.Invoke(syncName, this, args);
                searchItem = args.SearchItem;
                searchItems = args.SearchItems;
                ClsSynchronizer.VmMessages = args.SyncMessages;
                item = args.AsItem;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// PLM事件觸發
        /// </summary>
        /// <param name="AsInnovator"></param>
        /// <param name="syncName"></param>
        /// <param name="searchItem"></param>
        /// <param name="integrationEvents"></param>
        /// <param name="syncEvent"></param>
        /// <param name="type"></param>
        /// <param name="item"></param>
        public void ExecCadEvents(Aras.IOM.Innovator AsInnovator, string syncName, SearchItem searchItem, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, SyncEvents syncEvent, SyncType type, ref Aras.IOM.Item item)
        {

            try
            {
                //List<SearchItem> searchItems = null;
                foreach (IntegrationEvents integrationEvent in integrationEvents.OrderBy(x => x.Order).Where(x => x.EventName == syncEvent))
                {
                    
                    //if (integrationEvent.MethodName != "")
                   if (String.IsNullOrWhiteSpace(integrationEvent.MethodName) ==false)
                        item = item.apply(integrationEvent.MethodName);
                    else
                        ExecCadEvent(AsInnovator, syncName, ref searchItem, integrationEvent, syncEvent, type, ref item);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// PLM事件觸發
        /// </summary>
        /// <param name="AsInnovator"></param>
        /// <param name="syncName"></param>
        /// <param name="searchItems"></param>
        /// <param name="integrationEvents"></param>
        /// <param name="syncEvent"></param>
        /// <param name="type"></param>
        public void ExecCadEvents(Aras.IOM.Innovator AsInnovator,string syncName,  List<SearchItem> searchItems, List<StructuralChange> structureChanges, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, SyncEvents syncEvent, SyncType type)
        {

            try
            {
                //Events
                foreach (var integrationEvent in integrationEvents.OrderBy(x => x.Order).Where(x => x.EventName == syncEvent))
                {
                    
                    //if (integrationEvent.MethodName == "")
                    if (String.IsNullOrWhiteSpace(integrationEvent.MethodName))
                        ExecCadEvent(AsInnovator, syncName, ref searchItems, structureChanges, integrationEvent, integrationEvent.EventName, type);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// PLM事件觸發
        /// </summary>
        /// <param name="AsInnovator"></param>
        /// <param name="syncName"></param>
        /// <param name="searchItem"></param>
        /// <param name="integrationEvents"></param>
        /// <param name="syncEvent"></param>
        /// <param name="type"></param>
        public void ExecCadEvents(Aras.IOM.Innovator AsInnovator, string syncName, SearchItem searchItem, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, SyncEvents syncEvent, SyncType type)
        {

            try
            {
                //Events
                foreach (var integrationEvent in integrationEvents.OrderBy(x => x.Order).Where(x => x.EventName == syncEvent))
                {
                    
                    //if (integrationEvent.MethodName == "")
                    if (String.IsNullOrWhiteSpace(integrationEvent.MethodName))
                        ExecCadEvent(AsInnovator, syncName, ref searchItem,null, integrationEvent, integrationEvent.EventName, type);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
