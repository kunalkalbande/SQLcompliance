// This header is used to log Installer events into the Installer log.  the MSI Handle is required to accomplish this

#ifndef MSI_Logging_
#define	MSI_Logging_

void LogString(MSIHANDLE hInstall, TCHAR* string);
int MsiMessageBox(MSIHANDLE hInstall, TCHAR* szString, DWORD dwDlgFlags);

#endif