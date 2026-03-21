#if !defined(AFX_MAIN_FRM_H__)
#define AFX_MAIN_FRM_H__

#if _MSC_VER > 1000
#pragma once
#endif

class CMainFrame : public CFrameWnd
{
protected:
    CMainFrame();
    DECLARE_DYNCREATE(CMainFrame)

public:
    virtual BOOL PreCreateWindow(CREATESTRUCT& cs);

protected:
    afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnFileNewEmployee();
    afx_msg void OnFileLoadEmp();
    afx_msg void OnFileSaveEmp();

    CStatusBar m_wndStatusBar;

    DECLARE_MESSAGE_MAP()
};

#endif
