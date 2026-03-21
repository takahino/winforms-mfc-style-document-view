#if !defined(AFX_SAMPLE_VIEW_H__)
#define AFX_SAMPLE_VIEW_H__

#if _MSC_VER > 1000
#pragma once
#endif

#include "resource.h"

class CSampleDoc;

class CSampleView : public CFormView
{
protected:
    CSampleView();
    DECLARE_DYNCREATE(CSampleView)

public:
    enum { IDD = IDD_SAMPLE_FORM };

    CSampleDoc* GetDocument();

    virtual void OnInitialUpdate();
    virtual void DoDataExchange(CDataExchange* pDX);

protected:
    virtual void OnActivateView(BOOL bActivate, CView* pActivateView, CView* pDeactiveView);

    afx_msg void OnCheckActiveClicked();
    afx_msg void OnBtnOk();
    afx_msg void OnBtnCancel();
    afx_msg void OnBtnNew();

    void SetupListColumns();
    void FillStaticLists();

    BOOL m_bUiReady;
    BOOL m_bListColumnsDone;

    DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG
inline CSampleDoc* CSampleView::GetDocument()
{
    return (CSampleDoc*)m_pDocument;
}
#endif

#endif
