#include "StdAfx.h"
#include "resource.h"
#include "EmployeeSample.h"
#include "SampleDoc.h"
#include "SampleView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

IMPLEMENT_DYNCREATE(CSampleDoc, CDocument)

BEGIN_MESSAGE_MAP(CSampleDoc, CDocument)
END_MESSAGE_MAP()

namespace {

CString JsonEscape(const CString& s)
{
    CString r;
    for (int i = 0; i < s.GetLength(); i++)
    {
        TCHAR c = s[i];
        if (c == _T('\\')) r += _T("\\\\");
        else if (c == _T('"')) r += _T("\\\"");
        else if (c == _T('\n')) r += _T("\\n");
        else if (c == _T('\r')) r += _T("\\r");
        else if (c == _T('\t')) r += _T("\\t");
        else r += c;
    }
    return r;
}

BOOL JsonExtractString(const CString& src, LPCTSTR key, CString& val)
{
    CString pat;
    pat.Format(_T("\"%s\":\""), key);
    int i = src.Find(pat);
    if (i < 0)
        return FALSE;
    i += pat.GetLength();
    CString out;
    while (i < src.GetLength())
    {
        TCHAR c = src[i];
        if (c == _T('"'))
            break;
        if (c == _T('\\') && i + 1 < src.GetLength())
        {
            TCHAR n = src[i + 1];
            if (n == _T('n')) { out += _T('\n'); i += 2; continue; }
            if (n == _T('r')) { out += _T('\r'); i += 2; continue; }
            if (n == _T('t')) { out += _T('\t'); i += 2; continue; }
            if (n == _T('\\') || n == _T('"')) { out += n; i += 2; continue; }
        }
        out += c;
        i++;
    }
    val = out;
    return TRUE;
}

BOOL JsonExtractInt(const CString& src, LPCTSTR key, int& out)
{
    CString pat;
    pat.Format(_T("\"%s\":"), key);
    int i = src.Find(pat);
    if (i < 0)
        return FALSE;
    i += pat.GetLength();
    while (i < src.GetLength() && _istspace((unsigned char)src[i]))
        i++;
    BOOL neg = FALSE;
    if (i < src.GetLength() && src[i] == _T('-')) { neg = TRUE; i++; }
    if (i >= src.GetLength() || !_istdigit((unsigned char)src[i]))
        return FALSE;
    int v = 0;
    while (i < src.GetLength() && _istdigit((unsigned char)src[i]))
    {
        v = v * 10 + (src[i] - _T('0'));
        i++;
    }
    out = neg ? -v : v;
    return TRUE;
}

BOOL JsonExtractBool(const CString& src, LPCTSTR key, BOOL& out)
{
    CString pat;
    pat.Format(_T("\"%s\":"), key);
    int i = src.Find(pat);
    if (i < 0)
        return FALSE;
    i += pat.GetLength();
    while (i < src.GetLength() && _istspace((unsigned char)src[i]))
        i++;
    if (src.Mid(i, 4).CompareNoCase(_T("true")) == 0) { out = TRUE; return TRUE; }
    if (src.Mid(i, 5).CompareNoCase(_T("false")) == 0) { out = FALSE; return TRUE; }
    return FALSE;
}

void JsonLoadProjects(const CString& src, CTypedPtrArray<CObArray, CProjectRow*>& arr)
{
    CString key = _T("\"Projects\":[");
    int p0 = src.Find(key);
    if (p0 < 0)
        return;
    int endArr = src.Find(_T(']'), p0);
    if (endArr < 0)
        return;
    int p = src.Find(_T('{'), p0);
    while (p >= 0 && p < endArr)
    {
        int e = src.Find(_T('}'), p);
        if (e < 0 || e > endArr)
            break;
        CString obj = src.Mid(p, e - p + 1);
        int no = 0;
        CString name;
        BOOL active = TRUE;
        JsonExtractInt(obj, _T("No"), no);
        JsonExtractString(obj, _T("Name"), name);
        JsonExtractBool(obj, _T("Active"), active);
        arr.Add(new CProjectRow(no, name, active));
        p = src.Find(_T('{'), e + 1);
    }
}

#ifndef _UNICODE

BOOL WriteUtf8File(LPCTSTR path, const CString& body)
{
    int wlen = MultiByteToWideChar(CP_ACP, 0, body, -1, NULL, 0);
    if (wlen <= 0)
        return FALSE;
    WCHAR* w = new WCHAR[wlen];
    MultiByteToWideChar(CP_ACP, 0, body, -1, w, wlen);
    int u8len = WideCharToMultiByte(CP_UTF8, 0, w, -1, NULL, 0, NULL, NULL);
    if (u8len <= 0) { delete[] w; return FALSE; }
    char* u8 = new char[u8len];
    WideCharToMultiByte(CP_UTF8, 0, w, -1, u8, u8len, NULL, NULL);
    delete[] w;

    CFile f;
    if (!f.Open(path, CFile::modeCreate | CFile::modeWrite))
    {
        delete[] u8;
        return FALSE;
    }
    unsigned char bom[] = { 0xEF, 0xBB, 0xBF };
    f.Write(bom, 3);
    f.Write(u8, u8len - 1);
    delete[] u8;
    return TRUE;
}

BOOL ReadUtf8File(LPCTSTR path, CString& out)
{
    CFile f;
    if (!f.Open(path, CFile::modeRead))
        return FALSE;
    DWORD len = (DWORD)f.GetLength();
    if (len == 0)
    {
        out.Empty();
        return TRUE;
    }
    BYTE* buf = new BYTE[len + 1];
    f.Read(buf, len);
    buf[len] = 0;
    DWORD off = 0;
    if (len >= 3 && buf[0] == 0xEF && buf[1] == 0xBB && buf[2] == 0xBF)
        off = 3;
    int wlen = MultiByteToWideChar(CP_UTF8, 0, (LPCSTR)(buf + off), len - off, NULL, 0);
    if (wlen <= 0) { delete[] buf; return FALSE; }
    WCHAR* w = new WCHAR[wlen + 1];
    MultiByteToWideChar(CP_UTF8, 0, (LPCSTR)(buf + off), len - off, w, wlen);
    w[wlen] = 0;
    delete[] buf;
    int alen = WideCharToMultiByte(CP_ACP, 0, w, -1, NULL, 0, NULL, NULL);
    char* a = new char[alen];
    WideCharToMultiByte(CP_ACP, 0, w, -1, a, alen, NULL, NULL);
    delete[] w;
    out = a;
    delete[] a;
    return TRUE;
}

#else

BOOL WriteUtf8File(LPCTSTR path, const CString& body)
{
    int u8len = WideCharToMultiByte(CP_UTF8, 0, body, -1, NULL, 0, NULL, NULL);
    if (u8len <= 0)
        return FALSE;
    char* u8 = new char[u8len];
    WideCharToMultiByte(CP_UTF8, 0, body, -1, u8, u8len, NULL, NULL);
    CFile f;
    if (!f.Open(path, CFile::modeCreate | CFile::modeWrite))
    {
        delete[] u8;
        return FALSE;
    }
    unsigned char bom[] = { 0xEF, 0xBB, 0xBF };
    f.Write(bom, 3);
    f.Write(u8, u8len - 1);
    delete[] u8;
    return TRUE;
}

BOOL ReadUtf8File(LPCTSTR path, CString& out)
{
    CFile f;
    if (!f.Open(path, CFile::modeRead))
        return FALSE;
    DWORD len = (DWORD)f.GetLength();
    BYTE* buf = new BYTE[len + 1];
    f.Read(buf, len);
    buf[len] = 0;
    DWORD off = 0;
    if (len >= 3 && buf[0] == 0xEF && buf[1] == 0xBB && buf[2] == 0xBF)
        off = 3;
    int wlen = MultiByteToWideChar(CP_UTF8, 0, (LPCSTR)(buf + off), len - off, NULL, 0);
    if (wlen <= 0)
    {
        delete[] buf;
        return FALSE;
    }
    WCHAR* w = new WCHAR[wlen + 1];
    MultiByteToWideChar(CP_UTF8, 0, (LPCSTR)(buf + off), len - off, w, wlen);
    w[wlen] = 0;
    delete[] buf;
    out = w;
    delete[] w;
    return TRUE;
}

#endif

} // namespace

CSampleDoc::CSampleDoc()
{
    m_nAge = 0;
    m_bActive = FALSE;
    m_bNotify = FALSE;
    m_nPrefIndex = 0;
    m_nPriority = 1;
    m_bMale = TRUE;
    m_bFemale = FALSE;
    m_bFullTime = TRUE;
    m_bContract = FALSE;
    m_bPartTime = FALSE;
    m_nItemIndex = -1;
    m_strStatus = _T("Ready");
}

CSampleDoc::~CSampleDoc()
{
    ClearGridItems();
}

BOOL CSampleDoc::OnNewDocument()
{
    if (!CDocument::OnNewDocument())
        return FALSE;

    m_strEmpNo.Empty();
    m_strName.Empty();
    m_nAge = 0;
    m_strMemo.Empty();
    m_bActive = FALSE;
    m_bNotify = FALSE;
    m_nPrefIndex = 0;
    m_strLang.Empty();
    m_nPriority = 1;
    m_bMale = TRUE;
    m_bFemale = FALSE;
    m_bFullTime = TRUE;
    m_bContract = FALSE;
    m_bPartTime = FALSE;
    m_nItemIndex = -1;
    m_strStatus = _T("Ready");
    ClearGridItems();
    AddDefaultProjects();
    return TRUE;
}

void CSampleDoc::Serialize(CArchive& ar)
{
    CDocument::Serialize(ar);
}

void CSampleDoc::ClearGridItems()
{
    for (int i = 0; i < m_gridItems.GetSize(); i++)
        delete m_gridItems[i];
    m_gridItems.RemoveAll();
}

void CSampleDoc::AddDefaultProjects()
{
    ClearGridItems();
    m_gridItems.Add(new CProjectRow(1, _T("Core system modernization"), TRUE));
    m_gridItems.Add(new CProjectRow(2, _T("Mobile app development"), TRUE));
    m_gridItems.Add(new CProjectRow(3, _T("AI adoption support"), FALSE));
}

CWnd* CSampleDoc::GetAttachedWnd() const
{
    POSITION pos = GetFirstViewPosition();
    if (!pos)
        return NULL;
    CView* pView = GetNextView(pos);
    return pView;
}

CWnd* CSampleDoc::GetControl(int nId) const
{
    CWnd* p = GetAttachedWnd();
    return p ? p->GetDlgItem(nId) : NULL;
}

void CSampleDoc::SetEnabled(int nId, BOOL bEnable)
{
    CWnd* p = GetControl(nId);
    if (p)
        p->EnableWindow(bEnable);
}

void CSampleDoc::OnCheckActiveChanged()
{
    CWnd* pWnd = GetAttachedWnd();
    if (!pWnd)
        return;
    CButton* pChk = (CButton*)pWnd->GetDlgItem(IDC_CHECK_ACTIVE);
    BOOL active = pChk && pChk->GetCheck() == BST_CHECKED;

    SetEnabled(IDC_EDIT_NAME, active);
    SetEnabled(IDC_EDIT_AGE, active);
    SetEnabled(IDC_EDIT_MEMO, active);
    SetEnabled(IDC_COMBO_PREF, active);
    SetEnabled(IDC_COMBO_LANG, active);
    SetEnabled(IDC_SPIN_PRIORITY, active);
}

BOOL CSampleDoc::UpdateData(BOOL bSaveAndValidate)
{
    CWnd* pWnd = GetAttachedWnd();
    if (!pWnd)
        return FALSE;

    if (!bSaveAndValidate)
    {
        pWnd->SetDlgItemText(IDC_EDIT_EMPNO, m_strEmpNo);
        pWnd->SetDlgItemText(IDC_EDIT_NAME, m_strName);
        CString s; s.Format(_T("%d"), m_nAge);
        pWnd->SetDlgItemText(IDC_EDIT_AGE, s);
        pWnd->SetDlgItemText(IDC_EDIT_MEMO, m_strMemo);
        ((CButton*)pWnd->GetDlgItem(IDC_CHECK_ACTIVE))->SetCheck(m_bActive ? BST_CHECKED : BST_UNCHECKED);
        ((CButton*)pWnd->GetDlgItem(IDC_CHECK_NOTIFY))->SetCheck(m_bNotify ? BST_CHECKED : BST_UNCHECKED);

        CComboBox* pPref = (CComboBox*)pWnd->GetDlgItem(IDC_COMBO_PREF);
        if (pPref && pPref->GetCount() > 0)
            pPref->SetCurSel(m_nPrefIndex >= 0 && m_nPrefIndex < pPref->GetCount() ? m_nPrefIndex : 0);

        CComboBox* pLang = (CComboBox*)pWnd->GetDlgItem(IDC_COMBO_LANG);
        if (pLang)
        {
            int found = pLang->FindStringExact(-1, m_strLang);
            if (found >= 0)
                pLang->SetCurSel(found);
            else
                pLang->SetWindowText(m_strLang);
        }

        CString sp; sp.Format(_T("%d"), m_nPriority);
        pWnd->SetDlgItemText(IDC_SPIN_PRIORITY, sp);

        ((CButton*)pWnd->GetDlgItem(IDC_RADIO_MALE))->SetCheck(m_bMale ? BST_CHECKED : BST_UNCHECKED);
        ((CButton*)pWnd->GetDlgItem(IDC_RADIO_FEMALE))->SetCheck(m_bFemale ? BST_CHECKED : BST_UNCHECKED);

        ((CButton*)pWnd->GetDlgItem(IDC_RADIO_FULL_TIME))->SetCheck(m_bFullTime ? BST_CHECKED : BST_UNCHECKED);
        ((CButton*)pWnd->GetDlgItem(IDC_RADIO_CONTRACT))->SetCheck(m_bContract ? BST_CHECKED : BST_UNCHECKED);
        ((CButton*)pWnd->GetDlgItem(IDC_RADIO_PART_TIME))->SetCheck(m_bPartTime ? BST_CHECKED : BST_UNCHECKED);

        CListBox* pLb = (CListBox*)pWnd->GetDlgItem(IDC_LIST_ITEMS);
        if (pLb && pLb->GetCount() > 0)
        {
            if (m_nItemIndex >= 0 && m_nItemIndex < pLb->GetCount())
                pLb->SetCurSel(m_nItemIndex);
            else
                pLb->SetCurSel(-1);
        }

        pWnd->SetDlgItemText(IDC_STATIC_STATUS, m_strStatus);

        CListCtrl* pList = (CListCtrl*)pWnd->GetDlgItem(IDC_GRID_ITEMS);
        if (pList)
        {
            pList->DeleteAllItems();
            for (int i = 0; i < m_gridItems.GetSize(); i++)
            {
                CProjectRow* r = m_gridItems[i];
                CString sNo; sNo.Format(_T("%d"), r->m_nNo);
                int idx = pList->InsertItem(i, sNo);
                pList->SetItemText(idx, 1, r->m_strName);
                pList->SetItemText(idx, 2, r->m_bActive ? _T("true") : _T("false"));
            }
        }
        return TRUE;
    }

    CString t;
    pWnd->GetDlgItemText(IDC_EDIT_EMPNO, t);
    t.TrimLeft();
    t.TrimRight();
    if (t.GetLength() > 10)
    {
        AfxMessageBox(_T("Employee number must be at most 10 characters."), MB_OK | MB_ICONWARNING);
        pWnd->GetDlgItem(IDC_EDIT_EMPNO)->SetFocus();
        return FALSE;
    }

    pWnd->GetDlgItemText(IDC_EDIT_NAME, m_strName);
    if (m_strName.GetLength() > 30)
    {
        AfxMessageBox(_T("Name must be at most 30 characters."), MB_OK | MB_ICONWARNING);
        pWnd->GetDlgItem(IDC_EDIT_NAME)->SetFocus();
        return FALSE;
    }

    pWnd->GetDlgItemText(IDC_EDIT_AGE, t);
    m_nAge = _ttoi(t);
    if (m_nAge < 0 || m_nAge > 150)
    {
        AfxMessageBox(_T("Age must be between 0 and 150."), MB_OK | MB_ICONWARNING);
        pWnd->GetDlgItem(IDC_EDIT_AGE)->SetFocus();
        return FALSE;
    }

    pWnd->GetDlgItemText(IDC_EDIT_MEMO, m_strMemo);
    if (m_strMemo.GetLength() > 200)
    {
        AfxMessageBox(_T("Notes must be at most 200 characters."), MB_OK | MB_ICONWARNING);
        pWnd->GetDlgItem(IDC_EDIT_MEMO)->SetFocus();
        return FALSE;
    }

    m_bActive = ((CButton*)pWnd->GetDlgItem(IDC_CHECK_ACTIVE))->GetCheck() == BST_CHECKED;
    m_bNotify = ((CButton*)pWnd->GetDlgItem(IDC_CHECK_NOTIFY))->GetCheck() == BST_CHECKED;

    CComboBox* pPref = (CComboBox*)pWnd->GetDlgItem(IDC_COMBO_PREF);
    m_nPrefIndex = pPref ? pPref->GetCurSel() : 0;
    if (m_nPrefIndex == CB_ERR)
        m_nPrefIndex = 0;

    CComboBox* pLang = (CComboBox*)pWnd->GetDlgItem(IDC_COMBO_LANG);
    if (pLang)
        pLang->GetWindowText(m_strLang);

    pWnd->GetDlgItemText(IDC_SPIN_PRIORITY, t);
    m_nPriority = _ttoi(t);
    if (m_nPriority < 1 || m_nPriority > 10)
    {
        AfxMessageBox(_T("Priority must be between 1 and 10."), MB_OK | MB_ICONWARNING);
        pWnd->GetDlgItem(IDC_SPIN_PRIORITY)->SetFocus();
        return FALSE;
    }

    m_bMale = ((CButton*)pWnd->GetDlgItem(IDC_RADIO_MALE))->GetCheck() == BST_CHECKED;
    m_bFemale = ((CButton*)pWnd->GetDlgItem(IDC_RADIO_FEMALE))->GetCheck() == BST_CHECKED;

    m_bFullTime = ((CButton*)pWnd->GetDlgItem(IDC_RADIO_FULL_TIME))->GetCheck() == BST_CHECKED;
    m_bContract = ((CButton*)pWnd->GetDlgItem(IDC_RADIO_CONTRACT))->GetCheck() == BST_CHECKED;
    m_bPartTime = ((CButton*)pWnd->GetDlgItem(IDC_RADIO_PART_TIME))->GetCheck() == BST_CHECKED;

    CListBox* pLb = (CListBox*)pWnd->GetDlgItem(IDC_LIST_ITEMS);
    m_nItemIndex = pLb ? pLb->GetCurSel() : -1;

    pWnd->GetDlgItemText(IDC_EDIT_EMPNO, m_strEmpNo);
    m_strEmpNo.TrimLeft();
    m_strEmpNo.TrimRight();

    CListCtrl* pList = (CListCtrl*)pWnd->GetDlgItem(IDC_GRID_ITEMS);
    if (pList)
    {
        ClearGridItems();
        int n = pList->GetItemCount();
        for (int i = 0; i < n; i++)
        {
            CString sNo = pList->GetItemText(i, 0);
            int no = _ttoi(sNo);
            CString name = pList->GetItemText(i, 1);
            CString sa = pList->GetItemText(i, 2);
            BOOL active = (sa.CompareNoCase(_T("true")) == 0 || sa == _T("1"));
            m_gridItems.Add(new CProjectRow(no, name, active));
        }
    }

    return TRUE;
}

BOOL CSampleDoc::ValidateBusinessRule(CString& err) const
{
    if (m_strEmpNo.IsEmpty())
    {
        err = _T("Employee number is required.");
        return FALSE;
    }
    if (m_strName.IsEmpty())
    {
        err = _T("Name is required.");
        return FALSE;
    }
    err.Empty();
    return TRUE;
}

CString CSampleDoc::GetDataDirectory() const
{
    TCHAR buf[MAX_PATH];
    GetModuleFileName(NULL, buf, MAX_PATH);
    CString path(buf);
    int pos = path.ReverseFind(_T('\\'));
    if (pos > 0)
        path = path.Left(pos);
    path += _T("\\employees");
    return path;
}

BOOL CSampleDoc::SaveToJsonFile(const CString& path)
{
    CString json;
    json += _T("{\r\n");
    json += _T("  \"EmpNo\":\"") + JsonEscape(m_strEmpNo) + _T("\",\r\n");
    json += _T("  \"Name\":\"") + JsonEscape(m_strName) + _T("\",\r\n");
    CString s;
    s.Format(_T("  \"Age\":%d,\r\n"), m_nAge);
    json += s;
    json += _T("  \"Memo\":\"") + JsonEscape(m_strMemo) + _T("\",\r\n");
    json += m_bActive ? _T("  \"Active\":true,\r\n") : _T("  \"Active\":false,\r\n");
    json += m_bNotify ? _T("  \"Notify\":true,\r\n") : _T("  \"Notify\":false,\r\n");
    s.Format(_T("  \"PrefIndex\":%d,\r\n"), m_nPrefIndex);
    json += s;
    json += _T("  \"Lang\":\"") + JsonEscape(m_strLang) + _T("\",\r\n");
    s.Format(_T("  \"Priority\":%d,\r\n"), m_nPriority);
    json += s;
    json += m_bMale ? _T("  \"Male\":true,\r\n") : _T("  \"Male\":false,\r\n");
    json += m_bFemale ? _T("  \"Female\":true,\r\n") : _T("  \"Female\":false,\r\n");
    json += m_bFullTime ? _T("  \"FullTime\":true,\r\n") : _T("  \"FullTime\":false,\r\n");
    json += m_bContract ? _T("  \"Contract\":true,\r\n") : _T("  \"Contract\":false,\r\n");
    json += m_bPartTime ? _T("  \"PartTime\":true,\r\n") : _T("  \"PartTime\":false,\r\n");
    s.Format(_T("  \"ItemIndex\":%d,\r\n"), m_nItemIndex);
    json += s;
    json += _T("  \"Projects\": [\r\n");
    for (int i = 0; i < m_gridItems.GetSize(); i++)
    {
        CProjectRow* r = m_gridItems[i];
        if (i > 0)
            json += _T(",\r\n");
        s.Format(_T("    {\"No\":%d,\"Name\":\""), r->m_nNo);
        json += s + JsonEscape(r->m_strName) + _T("\",");
        json += r->m_bActive ? _T("\"Active\":true}") : _T("\"Active\":false}");
    }
    json += _T("\r\n  ]\r\n}\r\n");
    return WriteUtf8File(path, json);
}

BOOL CSampleDoc::LoadFromJsonFile(const CString& path)
{
    CString body;
    if (!ReadUtf8File(path, body))
        return FALSE;

    JsonExtractString(body, _T("EmpNo"), m_strEmpNo);
    JsonExtractString(body, _T("Name"), m_strName);
    JsonExtractInt(body, _T("Age"), m_nAge);
    JsonExtractString(body, _T("Memo"), m_strMemo);
    JsonExtractBool(body, _T("Active"), m_bActive);
    JsonExtractBool(body, _T("Notify"), m_bNotify);
    JsonExtractInt(body, _T("PrefIndex"), m_nPrefIndex);
    JsonExtractString(body, _T("Lang"), m_strLang);
    JsonExtractInt(body, _T("Priority"), m_nPriority);
    JsonExtractBool(body, _T("Male"), m_bMale);
    JsonExtractBool(body, _T("Female"), m_bFemale);
    JsonExtractBool(body, _T("FullTime"), m_bFullTime);
    JsonExtractBool(body, _T("Contract"), m_bContract);
    JsonExtractBool(body, _T("PartTime"), m_bPartTime);
    JsonExtractInt(body, _T("ItemIndex"), m_nItemIndex);

    ClearGridItems();
    JsonLoadProjects(body, m_gridItems);
    return TRUE;
}

void CSampleDoc::OnBtnOk()
{
    if (!UpdateData(TRUE))
        return;

    CString err;
    if (!ValidateBusinessRule(err))
    {
        AfxMessageBox(err, MB_OK | MB_ICONWARNING);
        return;
    }

    CString dir = GetDataDirectory();
    if (!CreateDirectory(dir, NULL) && GetLastError() != ERROR_ALREADY_EXISTS)
    {
        AfxMessageBox(_T("Failed to create data folder."), MB_OK | MB_ICONERROR);
        return;
    }

    CString path = dir + _T("\\") + m_strEmpNo + _T(".json");
    if (!SaveToJsonFile(path))
    {
        AfxMessageBox(_T("Save failed."), MB_OK | MB_ICONERROR);
        return;
    }

    CString st;
    st.Format(_T("Saved — [%s] %s (age %d)"),
        (LPCTSTR)m_strEmpNo, (LPCTSTR)m_strName, m_nAge);
    m_strStatus = st;
    SetModifiedFlag(FALSE);
    UpdateData(FALSE);
}

void CSampleDoc::OnMenuLoad()
{
    CWnd* pWnd = GetAttachedWnd();
    if (!pWnd)
        return;

    CString emp;
    pWnd->GetDlgItemText(IDC_EDIT_EMPNO, emp);
    emp.TrimLeft();
    emp.TrimRight();
    m_strEmpNo = emp;

    if (m_strEmpNo.IsEmpty())
    {
        AfxMessageBox(_T("Please enter an employee number."), MB_OK | MB_ICONWARNING);
        return;
    }

    CString path = GetDataDirectory() + _T("\\") + m_strEmpNo + _T(".json");
    if (GetFileAttributes(path) == 0xFFFFFFFF)
    {
        CString msg;
        msg.Format(_T("No data found for employee number \"%s\"."), (LPCTSTR)m_strEmpNo);
        AfxMessageBox(msg, MB_OK | MB_ICONWARNING);
        return;
    }

    if (!LoadFromJsonFile(path))
    {
        AfxMessageBox(_T("Load failed."), MB_OK | MB_ICONERROR);
        return;
    }

    CString st;
    st.Format(_T("Loaded — [%s] %s"), (LPCTSTR)m_strEmpNo, (LPCTSTR)m_strName);
    m_strStatus = st;
    SetModifiedFlag(FALSE);
    UpdateData(FALSE);
}

void CSampleDoc::OnBtnCancel()
{
    UpdateData(FALSE);
}

void CSampleDoc::OnNew()
{
    m_strEmpNo.Empty();
    m_strName.Empty();
    m_nAge = 0;
    m_strMemo.Empty();
    m_bActive = FALSE;
    m_bNotify = FALSE;
    m_nPrefIndex = 0;
    m_strLang.Empty();
    m_nPriority = 1;
    m_bMale = TRUE;
    m_bFemale = FALSE;
    m_bFullTime = TRUE;
    m_bContract = FALSE;
    m_bPartTime = FALSE;
    m_nItemIndex = -1;
    m_strStatus = _T("Enter new employee data");
    ClearGridItems();
    SetModifiedFlag(FALSE);
    UpdateData(FALSE);
    OnCheckActiveChanged();
}
