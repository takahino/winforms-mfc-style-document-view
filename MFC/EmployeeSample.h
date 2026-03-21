#if !defined(AFX_EMPLOYEE_SAMPLE_H__)
#define AFX_EMPLOYEE_SAMPLE_H__

#if _MSC_VER > 1000
#pragma once
#endif

#ifndef __AFXWIN_H__
#error include 'stdafx.h' before including this file
#endif

#include "resource.h"

class CEmployeeSampleApp : public CWinApp
{
public:
    CEmployeeSampleApp();

    virtual BOOL InitInstance();

    DECLARE_MESSAGE_MAP()
};

extern CEmployeeSampleApp theApp;

#endif
