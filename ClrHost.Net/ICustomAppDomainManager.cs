using System;
using System.Runtime.InteropServices;

namespace ClrHost.Net
{
    [Guid("CF8E74F3-77B5-47FC-8BCD-248D0D79772F"), ComVisible(true)]
    public interface ICustomAppDomainManager
    {
        void InitializeNewDomain(AppDomainSetup appDomainInfo);

        void Run(string assemblyPath);
    }
}
