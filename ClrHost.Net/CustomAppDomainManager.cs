using System;
using System.Runtime.InteropServices;

namespace ClrHost.Net
{
    [Guid("99CD51DF-632F-4382-9C35-47E7E3367E90"), ComVisible(true)]
    public class CustomAppDomainManager : AppDomainManager, ICustomAppDomainManager
    {
        public override void InitializeNewDomain(AppDomainSetup appDomainInfo)
        {
            InitializationFlags = AppDomainManagerInitializationOptions.RegisterWithHost;

            base.InitializeNewDomain(appDomainInfo);
        }

        public void Run(string assemblyPath)
        {
            AppDomain domain = null;

            try
            {
                domain = System.AppDomain.CreateDomain(Guid.NewGuid().ToString());
                domain.ExecuteAssembly(assemblyPath);
            }
            finally
            {
                if (domain != null)
                    AppDomain.Unload(domain);
            }
        }
    }
}