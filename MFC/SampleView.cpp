#include "StdAfx.h"
#include "EmployeeSample.h"
#include "SampleDoc.h"
#include "SampleView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

IMPLEMENT_DYNCREATE(CSampleView, CFormView)

BEGIN_MESSAGE_MAP(CSampleView, CFormView)
    ON_BN_CLICKED(IDC_CHECK_ACTIVE, OnCheckActiveClicked)
    ON_BN_CLICKED(IDC_BTN_OK, OnBtnOk)
    ON_BN_CLICKED(IDC_BTN_CANCEL, OnBtnCancel)
    ON_BN_CLICKED(IDC_BTN_NEW, OnBtnNew)
END_MESSAGE_MAP()

#ifdef _DEBUG
CSampleDoc* CSampleView::GetDocument()
{
    ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CSampleDoc)));
    return (CSampleDoc*)m_pDocument;
}
#endif

CSampleView::CSampleView()
    : CFormView(CSampleView::IDD)
    , m_bUiReady(FALSE)
    , m_bListColumnsDone(FALSE)
{
}

void CSampleView::DoDataExchange(CDataExchange* pDX)
{
    CFormView::DoDataExchange(pDX);
}

void CSampleView::SetupListColumns()
{
    if (m_bListColumnsDone)
        return;
    CListCtrl* pList = (CListCtrl*)GetDlgItem(IDC_GRID_ITEMS);
    if (!pList)
        return;
    m_bListColumnsDone = TRUE;

    pList->ModifyStyle(0, LVS_REPORT | LVS_SINGLESEL);
    pList->SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
    pList->InsertColumn(0, _T("No."), LVCFMT_LEFT, 40);
    pList->InsertColumn(1, _T("Project"), LVCFMT_LEFT, 200);
    pList->InsertColumn(2, _T("Active"), LVCFMT_LEFT, 52);
}

void CSampleView::FillStaticLists()
{
    static const TCHAR* kPrefs[] = {
        _T("Tokyo"), _T("Kanagawa"), _T("Osaka"), _T("Aichi"), _T("Saitama"),
        _T("Chiba"), _T("Hyogo"), _T("Hokkaido"), _T("Fukuoka"), _T("Miyagi"),
    };
    static const TCHAR* kLangs[] = {
        _T("C#"), _T("C++"), _T("Java"), _T("Python"), _T("TypeScript"),
        _T("Go"), _T("Rust"), _T("Kotlin"),
    };
    static const TCHAR* kDepts[] = {
        _T("Development"), _T("Infrastructure"), _T("Sales"), _T("HR"),
        _T("Accounting"), _T("Legal"),
    };

    CComboBox* pPref = (CComboBox*)GetDlgItem(IDC_COMBO_PREF);
    if (pPref && pPref->GetCount() == 0)
    {
        for (int i = 0; i < sizeof(kPrefs) / sizeof(kPrefs[0]); i++)
            pPref->AddString(kPrefs[i]);
    }

    CComboBox* pLang = (CComboBox*)GetDlgItem(IDC_COMBO_LANG);
    if (pLang && pLang->GetCount() == 0)
    {
        for (int i = 0; i < sizeof(kLangs) / sizeof(kLangs[0]); i++)
            pLang->AddString(kLangs[i]);
    }

    CListBox* pLb = (CListBox*)GetDlgItem(IDC_LIST_ITEMS);
    if (pLb && pLb->GetCount() == 0)
    {
        for (int i = 0; i < sizeof(kDepts) / sizeof(kDepts[0]); i++)
            pLb->AddString(kDepts[i]);
    }
}

void CSampleView::OnInitialUpdate()
{
    CFormView::OnInitialUpdate();

    if (!m_bUiReady)
    {
        m_bUiReady = TRUE;
        SetupListColumns();
        FillStaticLists();
    }

    GetParentFrame()->RecalcLayout();
    ResizeParentToFit(FALSE);

    CSampleDoc* pDoc = GetDocument();
    ASSERT(pDoc);
    pDoc->UpdateData(FALSE);
    pDoc->OnCheckActiveChanged();
}

void CSampleView::OnActivateView(BOOL bActivate, CView* pActivateView, CView* pDeactiveView)
{
    CFormView::OnActivateView(bActivate, pActivateView, pDeactiveView);
    if (bActivate && m_bUiReady && GetDocument())
    {
        GetDocument()->OnCheckActiveChanged();
    }
}

void CSampleView::OnCheckActiveClicked()
{
    CSampleDoc* pDoc = GetDocument();
    if (pDoc)
        pDoc->OnCheckActiveChanged();
}

void CSampleView::OnBtnOk()
{
    CSampleDoc* pDoc = GetDocument();
    if (pDoc)
        pDoc->OnBtnOk();
}

void CSampleView::OnBtnCancel()
{
    CSampleDoc* pDoc = GetDocument();
    if (pDoc)
        pDoc->OnBtnCancel();
}

void CSampleView::OnBtnNew()
{
    CSampleDoc* pDoc = GetDocument();
    if (pDoc)
        pDoc->OnNew();
}
