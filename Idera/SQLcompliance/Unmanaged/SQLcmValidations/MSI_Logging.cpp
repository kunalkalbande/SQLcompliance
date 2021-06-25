// MSI_Logging.cpp
//

#include "stdafx.h" 
#include <stdio.h>



void LogString(MSIHANDLE hInstall, TCHAR* szString)
{
	// if you are curious what the PMSIHANDLE is, look it up in the msi.chm.  It is actually a good idea to use it
	// rather than MSIHANDLE.  Basically it will free itself when it goes out of scope.
	PMSIHANDLE newHandle = ::MsiCreateRecord(2);
	TCHAR szTemp[MAX_PATH * 2];
	swprintf(szTemp, L"***%s", szString); // *** prefix is added for clarity in log file to easily identify our entries
	MsiRecordSetString(newHandle, 0, szTemp);
	MsiProcessMessage(hInstall, INSTALLMESSAGE(INSTALLMESSAGE_INFO), newHandle);
}




int MsiMessageBox(MSIHANDLE hInstall, TCHAR* szString, DWORD dwDlgFlags)
{
	PMSIHANDLE newHandle = ::MsiCreateRecord(2);
	MsiRecordSetString(newHandle, 0, szString);
	return (MsiProcessMessage(hInstall, INSTALLMESSAGE(INSTALLMESSAGE_USER + dwDlgFlags), newHandle));
}