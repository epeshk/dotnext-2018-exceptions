#include "VirtualMemoryLocker.h"

class HostMalloc : public IHostMalloc
{
    LONG m_cRef;
    HANDLE m_hMallocHeap;
    DWORD m_dwAllocFlags;
    VirtualMemoryLocker* virtualMemoryLocker;

public:
    HostMalloc(MALLOC_TYPE type, VirtualMemoryLocker* locker) : 
        m_cRef(0), virtualMemoryLocker(locker)
    {
        m_dwAllocFlags = 0;

        DWORD dwHeapFlags = 0;
        if (type & MALLOC_EXECUTABLE)
        {
            dwHeapFlags |= HEAP_CREATE_ENABLE_EXECUTE;
        }
        if ((type & MALLOC_THREADSAFE) == 0)
        {
            dwHeapFlags |= HEAP_NO_SERIALIZE;
            m_dwAllocFlags |= HEAP_NO_SERIALIZE;
        }
        m_hMallocHeap = ::HeapCreate(dwHeapFlags, 0, 0);
    }

    virtual ~HostMalloc()
    {
    }

    ULONG __stdcall AddRef() override
    {
        return InterlockedIncrement(&m_cRef);
    }

    ULONG __stdcall Release() override
    {
        ULONG cRef = InterlockedDecrement(&m_cRef);
        if (cRef == 0)
        {
            delete this;
        }
        return cRef;
    }

    HRESULT __stdcall QueryInterface(REFIID riid, void** ppvObject) override
    {
        if (riid == IID_IUnknown)
        {
            *ppvObject = static_cast<IUnknown*>(this);
            return S_OK;
        }
        if (riid == IID_IHostMalloc)
        {
            *ppvObject = static_cast<IHostMalloc*>(this);
            return S_OK;
        }

        *ppvObject = nullptr;
        return E_NOINTERFACE;
    }

    HRESULT __stdcall Alloc(SIZE_T cbSize, EMemoryCriticalLevel dwCriticalLevel, void** ppMem) override
    {
        *ppMem = ::HeapAlloc(m_hMallocHeap, m_dwAllocFlags, cbSize);
        if (*ppMem == nullptr)
        {
            return E_OUTOFMEMORY;
        }

        if (cbSize > 0) 
        {
            virtualMemoryLocker->RefreshAllocatedMemory();
            virtualMemoryLocker->AdjustWorkingSetSizeQuota();
            virtualMemoryLocker->Lock(*ppMem, cbSize);
        }

        return S_OK;
    }

    HRESULT __stdcall DebugAlloc(SIZE_T cbSize, EMemoryCriticalLevel dwCriticalLevel, char* pszFileName, int iLineNo, void** ppMem) override
    {
        return Alloc(cbSize, dwCriticalLevel, ppMem);
    }

    HRESULT __stdcall Free(void* pMem) override
    {
        if (FALSE == ::HeapFree(m_hMallocHeap, 0, pMem))
        {
            return E_FAIL;
        }

        virtualMemoryLocker->RefreshAllocatedMemory();
        virtualMemoryLocker->AdjustWorkingSetSizeQuota();

        return S_OK;
    }
};