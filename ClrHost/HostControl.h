
class HostControl : public IHostControl
{
	LONG referencesCount;
	ICustomAppDomainManager* appDomainManager;

public:
	HostControl() :
		referencesCount(0),
		appDomainManager(nullptr)
	{
	}

	virtual ~HostControl()
	{
	}

	ULONG __stdcall AddRef() override
	{
		return InterlockedIncrement(&referencesCount);
	}

	ULONG __stdcall Release() override
	{
		return InterlockedDecrement(&referencesCount);
	}

	HRESULT __stdcall QueryInterface(REFIID riid, void** ppvObject) override
	{
		if (riid == IID_IUnknown)
		{
			*ppvObject = static_cast<IUnknown*>(static_cast<IHostControl*>(this));
			return S_OK;
		}
		if (riid == IID_IHostControl)
		{
			*ppvObject = static_cast<IHostControl*>(this);
			return S_OK;
		}

		*ppvObject = nullptr;
		return E_NOINTERFACE;
	}

	HRESULT __stdcall GetHostManager(REFIID riid, void** ppObject) override
	{
		*ppObject = nullptr;
		return E_NOINTERFACE;
	}

	HRESULT __stdcall SetAppDomainManager(DWORD dwAppDomainID, IUnknown* pUnkAppDomainManager) override
	{
		return pUnkAppDomainManager->QueryInterface(__uuidof(ICustomAppDomainManager), reinterpret_cast<PVOID*>(&appDomainManager));
	}

	ICustomAppDomainManager* GetDomainManagerForDefaultDomain() const
	{
		if (appDomainManager)
		{
			appDomainManager->AddRef();
		}
		return appDomainManager;
	}
};
