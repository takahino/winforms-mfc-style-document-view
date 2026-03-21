#include "StdAfx.h"
#include "EmployeeSample.h"
#include "MainFrm.h"
#include "SampleDoc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

IMPLEMENT_DYNCREATE(CMainFrame, CFrameWnd)

BEGIN_MESSAGE_MAP(CMainFrame, CFrameWnd)
    ON_WM_CREATE()
    ON_COMMAND(ID_FILE_NEW, OnFileNewEmployee)
    ON_COMMAND(ID_FILE_LOAD_EMP, OnFileLoadEmp)
    ON_COMMAND(ID_FILE_SAVE_EMP, OnFileSaveEmp)
END_MESSAGE_MAP()

static UINT indicators[] =
{
    ID_SEPARATOR,
    ID_INDICATOR_CAPS,
    ID_INDICATOR_NUM,
    ID_INDICATOR_SCRL,
};

CMainFrame::CMainFrame()
{
}

int CMainFrame::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CFrameWnd::OnCreate(lpCreateStruct) == -1)
        return -1;

    if (!m_wndStatusBar.Create(this) ||
        !m_wndStatusBar.SetIndicators(indicators, sizeof(indicators) / sizeof(UINT)))
    {
        TRACE0("Failed to create status bar\n");
        return -1;
    }

    return 0;
}

BOOL CMainFrame::PreCreateWindow(CREATESTRUCT& cs)
{
    if (!CFrameWnd::PreCreateWindow(cs))
        return FALSE;
    cs.style = WS_OVERLAPPED | WS_CAPTION | FWS_ADDTOTITLE
        | WS_THICKFRAME | WS_SYSMENU | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_MAXIMIZE;
    return TRUE;
}

void CMainFrame::OnFileNewEmployee()
{
    CSampleDoc* pDoc = (CSampleDoc*)GetActiveDocument();
    if (pDoc)
        pDoc->OnNew();
}

void CMainFrame::OnFileLoadEmp()
{
    CSampleDoc* pDoc = (CSampleDoc*)GetActiveDocument();
    if (pDoc)
        pDoc->OnMenuLoad();
}

void CMainFrame::OnFileSaveEmp()
{
    CSampleDoc* pDoc = (CSampleDoc*)GetActiveDocument();
    if (pDoc)
        pDoc->OnBtnOk();
}
