using System;

namespace BCS.CADs.Synchronization.ConfigProperties
{
    internal class XmlSettingAttributeName : Attribute
    {
        /// <summary>
        /// 取得PrintSettingBase的Attribute名稱
        /// </summary>
        public string AttributeName { get; set; }

        public XmlSettingAttributeName(string attributeName)
        {
            AttributeName = attributeName;
        }
    }
}