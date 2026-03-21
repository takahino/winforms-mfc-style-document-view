#if !defined(AFX_SAMPLE_DOC_H__)
#define AFX_SAMPLE_DOC_H__

#if _MSC_VER > 1000
#pragma once
#endif

class CProjectRow : public CObject
{
public:
    int     m_nNo;
    CString m_strName;
    BOOL    m_bActive;

    CProjectRow() : m_nNo(0), m_bActive(TRUE) {}
    CProjectRow(int nNo, LPCTSTR pszName, BOOL bActive)
        : m_nNo(nNo), m_strName(pszName), m_bActive(bActive) {}
};

class CSampleDoc : public CDocument
{
protected:
    CSampleDoc();
    DECLARE_DYNCREATE(CSampleDoc)

public:
    virtual BOOL OnNewDocument();
    virtual void Serialize(CArchive& ar);

    // ── Same fields as C# SampleDocument ─────────────────────────────────
    CString m_strEmpNo;
    CString m_strName;
    int     m_nAge;
    CString m_strMemo;
    BOOL    m_bActive;
    BOOL    m_bNotify;
    int     m_nPrefIndex;
    CString m_strLang;
    int     m_nPriority;
    BOOL    m_bMale;
    BOOL    m_bFemale;
    BOOL    m_bFullTime;
    BOOL    m_bContract;
    BOOL    m_bPartTime;
    int     m_nItemIndex;
    CString m_strStatus;
    CTypedPtrArray<CObArray, CProjectRow*> m_gridItems;

    // ── Same as C# MfcDocument.UpdateData / handlers ───────────────────
    BOOL UpdateData(BOOL bSaveAndValidate);
    void OnBtnOk();
    void OnMenuLoad();
    void OnBtnCancel();
    void OnNew();
    void OnCheckActiveChanged();

    CWnd* GetAttachedWnd() const;

    // ResourceId-style: GetDlgItem / EnableWindow
    CWnd* GetControl(int nId) const;
    void SetEnabled(int nId, BOOL bEnable);

protected:
    virtual ~CSampleDoc();
    void ClearGridItems();
    void AddDefaultProjects();
    BOOL ValidateBusinessRule(CString& err) const;
    BOOL SaveToJsonFile(const CString& path);
    BOOL LoadFromJsonFile(const CString& path);
    CString GetDataDirectory() const;

    DECLARE_MESSAGE_MAP()
};

#endif
