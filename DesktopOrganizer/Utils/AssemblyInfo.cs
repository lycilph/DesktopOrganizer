using System;
using System.IO;
using System.Reflection;

namespace DesktopOrganizer.Utils
{
    public class AssemblyInfo
    {
        private readonly Assembly assembly;

        public AssemblyInfo(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            this.assembly = assembly;
        }


        public string ProductTitle
        {
            get { return GetAttributeValue<AssemblyTitleAttribute>(a => a.Title, Path.GetFileNameWithoutExtension(assembly.CodeBase)); }
        }

        public string Version
        {
            get
            {
                var version = assembly.GetName().Version;
                return version != null ? version.ToString() : "1.0.0.0";
            }
        }

        public string Description
        {
            get { return GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description); }
        }


        public string Product
        {
            get { return GetAttributeValue<AssemblyProductAttribute>(a => a.Product); }
        }

        public string Copyright
        {
            get { return GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright); }
        }

        public string Company
        {
            get { return GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company); }
        }

        protected string GetAttributeValue<T>(Func<T,string> resolveFunc, string defaultResult = null) where T : Attribute
        {
            var attributes = assembly.GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? resolveFunc((T)attributes[0]) : defaultResult;
        }
    }
}
