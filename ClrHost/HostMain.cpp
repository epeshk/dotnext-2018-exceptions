#include "stdafx.h"
#include "HostControl.h"
#pragma comment (lib, "mscoree.lib")
#pragma comment(lib, "psapi.lib")

void CheckHRImpl(HRESULT hr, int line) {
	if (FAILED(hr)) {
		wprintf(L"ClrHost failed to start. Failure on line: %d", line);
		ExitProcess(1);
	}
}

#define CheckHR(x) CheckHRImpl(x, __LINE__)

int wmain(int argc, wchar_t* argv[])
{
	setvbuf(stdout, nullptr, _IONBF, 0);

	if (argc < 2 || argc > 3)
	{
		wprintf(L"Usage: %s <ExecutableAssemblyPath>\n", argv[0]);
        ExitProcess(-1);
	}

	wchar_t assemblyPath[MAX_PATH] = { 0 };
	GetFullPathName(argv[1], MAX_PATH, assemblyPath, NULL);
    wchar_t* imageRuntimeVersion = new wchar_t[128];
    wchar_t* loadedRuntimeVersion = new wchar_t[128];
    DWORD imageRuntimeVersionSize = 128;
    DWORD loadedRuntimeVersionSize = 128;

	imageRuntimeVersion[0] = 0;
	loadedRuntimeVersion[0] = 0;
	memset(imageRuntimeVersion, 0, sizeof(wchar_t) * 128);
	memset(loadedRuntimeVersion, 0, sizeof(wchar_t) * 128);

    ICLRMetaHostPolicy* pMetaHostPolicy = nullptr;
    ICLRMetaHost* pMetaHost             = nullptr;
    ICLRRuntimeInfo *pRuntimeInfo       = nullptr;
    ICLRRuntimeHost *pRuntimeHost       = nullptr;
    ICLRControl* pCLRControl            = nullptr;
	ICLRPolicyManager *policyMgr        = nullptr;
    
    CheckHR(CLRCreateInstance(CLSID_CLRMetaHostPolicy, IID_ICLRMetaHostPolicy, reinterpret_cast<LPVOID*>(&pMetaHostPolicy)));
	CheckHR(CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, reinterpret_cast<LPVOID*>(&pMetaHost)));
	CheckHR(pMetaHost->GetVersionFromFile(assemblyPath, imageRuntimeVersion, &imageRuntimeVersionSize));
	CheckHR(pMetaHostPolicy->GetRequestedRuntime(METAHOST_POLICY_HIGHCOMPAT, assemblyPath, nullptr, loadedRuntimeVersion, &loadedRuntimeVersionSize, imageRuntimeVersion, &imageRuntimeVersionSize, nullptr, IID_ICLRRuntimeInfo, reinterpret_cast<LPVOID*>(&pRuntimeInfo)));
	CheckHR(pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&pRuntimeHost)));
	CheckHR(pRuntimeHost->GetCLRControl(&pCLRControl));
	CheckHR(pCLRControl->GetCLRManager(IID_ICLRPolicyManager, reinterpret_cast<void**>(&policyMgr)));
	
	// don't crash process on unhandled exceptions
	CheckHR(policyMgr->SetUnhandledExceptionPolicy(eHostDeterminedPolicy));

	// just unload appdomain on stack overflow
	CheckHR(policyMgr->SetActionOnFailure(FAIL_StackOverflow, eRudeUnloadAppDomain));
    
	HostControl* hostControl = new HostControl();
	CheckHR(pRuntimeHost->SetHostControl(hostControl));
	CheckHR(pCLRControl->SetAppDomainManagerType(L"ClrHost.Net", L"ClrHost.Net.CustomAppDomainManager"));
	CheckHR(pRuntimeHost->Start());
    ICustomAppDomainManager* pAppDomainManager = hostControl->GetDomainManagerForDefaultDomain();
	CheckHR(pAppDomainManager->Run(assemblyPath));
	CheckHR(pRuntimeHost->Stop());
    return 0;
}

