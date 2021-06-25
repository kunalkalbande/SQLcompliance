// ComplianceActions.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

LONG RegistryKeyExists(HKEY baseKey, WCHAR* subKey)
{
   HKEY hKey;
   LONG lRet;

   lRet = RegOpenKeyEx(baseKey, subKey,
      0, KEY_QUERY_VALUE, &hKey );
   if( lRet != ERROR_SUCCESS )
      return lRet;
   RegCloseKey( hKey );
   return lRet ;
}

LONG RegistryValueExists(HKEY baseKey, WCHAR* subKey, WCHAR* valueName)
{
   HKEY hKey ;
   DWORD dwType ;
   LONG retVal ;

   retVal = RegOpenKeyEx( baseKey, subKey,
      0, KEY_QUERY_VALUE, &hKey );
   if( retVal != ERROR_SUCCESS )
      return retVal ;

   retVal = RegQueryValueEx(hKey, valueName, 0, &dwType, NULL, NULL) ;
   RegCloseKey( hKey );
   return retVal ;
}

LONG RegistrySetValue(HKEY baseKey, WCHAR* subKey, WCHAR* valueName, DWORD value)
{
   HKEY hKey ;
   DWORD dwType ;
   LONG retVal ;

   retVal = RegOpenKeyEx( baseKey, subKey,
      0, KEY_WRITE, &hKey );
   if( retVal != ERROR_SUCCESS )
      return retVal ;

   retVal = RegSetValueEx(hKey, valueName, 0, REG_DWORD, (BYTE*)&value, sizeof(DWORD)) ;
   RegCloseKey( hKey );
   return retVal ;
}


int _tmain(int argc, _TCHAR* argv[])
{
   if(RegistryValueExists(HKEY_LOCAL_MACHINE, L"SOFTWARE\\Idera\\SQLcompliance\\SQLcomplianceAgent", L"Instances") == ERROR_SUCCESS)
   {
      printf("Yay\n") ;
      RegistrySetValue(HKEY_LOCAL_MACHINE, L"SOFTWARE\\Idera\\SQLcompliance\\", L"Allan", 8) ;
   }else
   {
      printf("No\n") ;
   }
   char junk ;

   scanf("%c", &junk) ;
	return 0;
}

