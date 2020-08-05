using BCS.CADs.Synchronization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCS.CADs.Synchronization.Entities
{
    public class SyncMessages
    {

        public  SyncType Function { get; set; }
        public SyncOperation Operation { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Status { get; set; }

        public List<ItemMessage> ItemMessages { get; set; } = new List<ItemMessage>();


        public SyncMessages(SyncType function, SyncOperation operation, string name, string value, string status)
        {
            this.Function = function;
            this.Operation = operation;
            this.Name = name;
            this.Value = value;
            this.Status = status;
        }

        public ItemMessage AddItemMessage (string name, string value, string detail, string status)
        {
            ItemMessage itemMessage =new ItemMessage(this,DateTime.Now ,name, value, detail, status);
            this.ItemMessages.Add(itemMessage);
            return itemMessage;
        }

        public void AddItemMessage(string name, string value, string detail, string status, ItemException itemException)
        {
            ItemMessage itemMessage = new ItemMessage(this, DateTime.Now,name, value, detail, status);
            this.ItemMessages.Add(itemMessage);
            itemMessage.IsError = true;
            itemMessage.SyncItemException = itemException;
        }


        public void AddItemException(ItemMessage itemMessage, ItemException itemException)
        {
            itemMessage.IsError = true;
            itemMessage.SyncItemException = itemException;
        }

    }
}
