using System;

namespace BCS.CADs.Synchronization.ConfigProperties
{
    internal class XmlSettingTagNameAttribute : Attribute
    {
        /// <summary>
        /// 取得PrintSettingBase的Attribute名稱
        /// </summary>
        public string TagName { get; set; }

        public XmlSettingTagNameAttribute(string tagName)
        {
            TagName = tagName;
        }
    }
}