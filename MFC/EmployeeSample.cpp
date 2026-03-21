#include "StdAfx.h"
#include "EmployeeSample.h"
#include "MainFrm.h"
#include "SampleDoc.h"
#include "SampleView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

CEmployeeSampleApp theApp;

BEGIN_MESSAGE_MAP(CEmployeeSampleApp, CWinApp)
END_MESSAGE_MAP()

CEmployeeSampleApp::CEmployeeSampleApp()
{
}

BOOL CEmployeeSampleApp::InitInstance()
{
#ifdef _AFXDLL
    Enable3dControls();
#else
    Enable3dControlsStatic();
#endif

    SetRegistryKey(_T("Local AppWizard-Generated Applications"));

    LoadStdProfileSettings();

    CSingleDocTemplate* pDocTemplate;
    pDocTemplate = new CSingleDocTemplate(
        IDR_MAINFRAME,
        RUNTIME_CLASS(CSampleDoc),
        RUNTIME_CLASS(CMainFrame),
        RUNTIME_CLASS(CSampleView));
    AddDocTemplate(pDocTemplate);

    CCommandLineInfo cmdInfo;
    ParseCommandLine(cmdInfo);

    if (!ProcessShellCommand(cmdInfo))
        return FALSE;

    m_pMainWnd->ShowWindow(SW_SHOW);
    m_pMainWnd->UpdateWindow();

    return TRUE;
}
