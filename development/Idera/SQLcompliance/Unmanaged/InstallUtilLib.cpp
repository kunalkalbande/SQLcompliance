//**********************************************************************
//*
//* File: InstallUtilLib.cpp
//*
//* Copyright Idera (BBS Technologies) 2005
//*
//**********************************************************************
#include "stdafx.h"

typedef BOOL (WINAPI *SetSecurityDescriptorControlFnPtr)(
   IN PSECURITY_DESCRIPTOR pSecurityDescriptor,
   IN SECURITY_DESCRIPTOR_CONTROL ControlBitsOfInterest,
   IN SECURITY_DESCRIPTOR_CONTROL ControlBitsToSet);


//========================================================================================
// Static helper functions.
//========================================================================================
static void
   l_InitLsaString(
      const PWCHAR         srcStrIn,
      PLSA_UNICODE_STRING  lsaStrOut
   )
{
   if(!lsaStrOut || !srcStrIn) { return; }
   ::memset (lsaStrOut, 0, sizeof lsaStrOut);
   lsaStrOut->Buffer = srcStrIn;
   lsaStrOut->Length = (USHORT) (::wcslen(srcStrIn) * sizeof srcStrIn[0]);
   lsaStrOut->MaximumLength = lsaStrOut->Length + sizeof srcStrIn[0];
}

static DWORD 
	l_GivePriv (
		LPCTSTR        accountIn,
      const PWCHAR   privIn
	)
{
   //----------------------------------------------------------------------------------------
   // Validate input.
   //----------------------------------------------------------------------------------------
   if(accountIn == 0 || accountIn[0] == 0)
   {
      return ERROR_INVALID_ACCOUNT_NAME; 
   }
   if(privIn == 0 || privIn[0] == 0)
   {
      return ERROR_NO_SUCH_PRIVILEGE;
   }

   //----------------------------------------------------------------------------------------
   // Lookup the account name to get its SID.   Also verify if the account is a user.
   //----------------------------------------------------------------------------------------
   BYTE           sid[100];    
   DWORD          dwSid = sizeof(sid);
   TCHAR          domain[32];  
   DWORD          dwDomain = DIM(domain);
   SID_NAME_USE   sidNameUse;
   DWORD          rc = ERROR_SUCCESS;
   int strIndex = 0 ;

   if(_tcsnccmp(".\\", accountIn, 2) == 0)
	   strIndex = 2 ;

   if (::LookupAccountName 
         (NULL, &(accountIn[strIndex]), sid, &dwSid,
            domain, &dwDomain, &sidNameUse))   
   {
      if(sidNameUse != SidTypeUser)
      {
         rc = ERROR_NO_SUCH_USER; 
      }
   }
   else
   {
      rc = ::GetLastError();
   }

   //----------------------------------------------------------------------------------------
   // Open the local computer's LSA handle.
   //----------------------------------------------------------------------------------------
   LSA_HANDLE lsaHandle = 0;
   if(rc == ERROR_SUCCESS)
   {
      LSA_OBJECT_ATTRIBUTES   lsaObjectAttr;
      memset(&lsaObjectAttr, 0, sizeof lsaObjectAttr);
      rc = ::LsaNtStatusToWinError (::LsaOpenPolicy (NULL, &lsaObjectAttr,
                                       POLICY_LOOKUP_NAMES|POLICY_CREATE_ACCOUNT,
                                          &lsaHandle));
      if(rc != ERROR_SUCCESS) { lsaHandle = 0; }
   }

   //----------------------------------------------------------------------------------------
   // Grant privilege to the account, if the privilege is already
   // granted the API will ignore the request.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      LSA_UNICODE_STRING lsaLogonAsServiceRightStr;
      l_InitLsaString(privIn,&lsaLogonAsServiceRightStr);
      rc = ::LsaNtStatusToWinError
               (::LsaAddAccountRights (lsaHandle, sid, &lsaLogonAsServiceRightStr, 1));
   }

   //----------------------------------------------------------------------------------------
   // Close the LSA handle.
   //----------------------------------------------------------------------------------------
   if(lsaHandle) { ::LsaClose(lsaHandle); }

   return rc;
}

static DWORD 
	l_AddNtfsPermissions (
		BYTE     *sidIn,        
		LPCTSTR  fileOrDirPathIn,
      DWORD    accessMaskIn
	)
{
   //----------------------------------------------------------------------------------------
   // Get security descriptor of the specified file.
   //----------------------------------------------------------------------------------------
   // Figure out the size of the buffer needed.
   PSECURITY_DESCRIPTOR pFileSD      = NULL;
   DWORD                dwFileSDSize = 0;
   SECURITY_INFORMATION secInfo      = DACL_SECURITY_INFORMATION;
   ::GetFileSecurity(fileOrDirPathIn,
            secInfo, pFileSD, 0, &dwFileSDSize); // ignore return status.
   DWORD rc = ::GetLastError();
   if(rc == ERROR_INSUFFICIENT_BUFFER)
   {
      pFileSD = new BYTE[dwFileSDSize];
      if(!pFileSD) { rc = ERROR_OUTOFMEMORY; }
      else { rc = ERROR_SUCCESS; }
   }

   // Buffer is allocated now get the security descriptor.
   if(rc == ERROR_SUCCESS)
   {
      if(!::GetFileSecurity(fileOrDirPathIn,
            secInfo, pFileSD, dwFileSDSize, &dwFileSDSize))
      {
         rc = ::GetLastError();
      }
   }

   //----------------------------------------------------------------------------------------
   // Initialize new SD.
   //----------------------------------------------------------------------------------------
   SECURITY_DESCRIPTOR  newSD;
   if(rc == ERROR_SUCCESS)
   {
      if(!::InitializeSecurityDescriptor(&newSD, SECURITY_DESCRIPTOR_REVISION))
      {
         rc = ::GetLastError();
      }
   }

   //----------------------------------------------------------------------------------------
   // Get DACL from file SD.
   //----------------------------------------------------------------------------------------
   PACL pACL           = NULL;
   BOOL isDaclPresent  = FALSE, 
        isDaclDefaulted = FALSE;
   if(rc == ERROR_SUCCESS)
   {
      if (!::GetSecurityDescriptorDacl(pFileSD, &isDaclPresent, &pACL,
            &isDaclDefaulted)) 
      {
         rc = ::GetLastError();
      }
   }

   //----------------------------------------------------------------------------------------
   // Get DACL size information.
   //----------------------------------------------------------------------------------------
   ACL_SIZE_INFORMATION aclInfo;
   if(rc == ERROR_SUCCESS)
   {
      aclInfo.AceCount = 0; // Assume NULL DACL.
      aclInfo.AclBytesFree = 0;
      aclInfo.AclBytesInUse = sizeof(ACL);

      if (pACL == NULL)
         isDaclPresent = FALSE;

      // If not NULL DACL, gather size information from DACL.
      if (isDaclPresent) {

         if (!::GetAclInformation(pACL, &aclInfo,
               sizeof(ACL_SIZE_INFORMATION), AclSizeInformation)) 
         {
            rc = ::GetLastError();
         }
      }
   }

   //----------------------------------------------------------------------------------------
   // Compute size, allocate memory and initialize new DACL.
   //----------------------------------------------------------------------------------------
   PACL  pNewACL    = NULL;
   DWORD newACLSize = 0;
   if(rc == ERROR_SUCCESS)
   {
      // calculate new ACL size.
      newACLSize = aclInfo.AclBytesInUse + sizeof(ACCESS_ALLOWED_ACE)
                     + ::GetLengthSid(sidIn) - sizeof(DWORD);

      // allocate new ACL memory.
      pNewACL = (PACL) new BYTE [newACLSize];
      if(!pNewACL)
      {
         rc = ERROR_OUTOFMEMORY;
      }

      // initialize the ACL.
      if(rc == ERROR_SUCCESS)
      {
         if(!::InitializeAcl(pNewACL,newACLSize,ACL_REVISION2))
         {
            rc = ::GetLastError();
         }
      }
   }

   //----------------------------------------------------------------------------------------
   // If DACL is present, copy all the ACEs from the old DACL
   // to the new DACL.
   //
   // The following code assumes that the old DACL is
   // already in Windows 2000 preferred order.  To conform
   // to the new Windows 2000 preferred order, first we will
   // copy all non-inherited ACEs from the old DACL to the
   // new DACL, irrespective of the ACE type.
   //----------------------------------------------------------------------------------------
   DWORD  newAceIndex = 0, currentAceIndex = 0;
   LPVOID pTempAce    = NULL;
   if (rc == ERROR_SUCCESS)
   {
      if (isDaclPresent && aclInfo.AceCount) 
      {
         for (currentAceIndex = 0; 
               currentAceIndex < aclInfo.AceCount && rc == ERROR_SUCCESS;
                  currentAceIndex++) 
         {
            // Get an ACE.
            if (!::GetAce(pACL, currentAceIndex, &pTempAce)) 
            {
               rc = ::GetLastError();
               continue;
            }

            // Check if it is a non-inherited ACE.
            // If it is an inherited ACE, break from the loop so
            // that the new access allowed non-inherited ACE can
            // be added in the correct position, immediately after
            // all non-inherited ACEs.
            if (((ACCESS_ALLOWED_ACE *)pTempAce)->Header.AceFlags
               & INHERITED_ACE)
               break;

            // Skip adding the ACE, if the SID matches
            // with the account specified, as we are going to
            // add an access allowed ACE with a different access
            // mask.
            if (::EqualSid(sidIn,
               &(((ACCESS_ALLOWED_ACE *)pTempAce)->SidStart)))
               continue;

            // Add the ACE to the new ACL.
            if (!::AddAce(pNewACL, ACL_REVISION, MAXDWORD, pTempAce,
                  ((PACE_HEADER) pTempAce)->AceSize)) 
            {
               rc = ::GetLastError();
               continue;
            }

            newAceIndex++;
         }
      }
   }

   //----------------------------------------------------------------------------------------
   // Add the access-allowed ACE to the new DACL.
   // The new ACE added here will be in the correct position,
   // immediately after all existing non-inherited ACEs.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      if (!::AddAccessAllowedAce(pNewACL, ACL_REVISION2, accessMaskIn,
            sidIn)) 
      {
         rc = ::GetLastError();
      }
   }

   //----------------------------------------------------------------------------------------
   // To conform to the new Windows 2000 preferred order,
   // we will now copy the rest of inherited ACEs from the
   // old DACL to the new DACL.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      if (isDaclPresent && aclInfo.AceCount) 
      {
         for (;currentAceIndex < aclInfo.AceCount; currentAceIndex++) 
         {
            // Get an ACE.
            if (!::GetAce(pACL, currentAceIndex, &pTempAce)) 
            {
               rc = ::GetLastError();
               break;
            }

            // Add the ACE to the new ACL.
            if (!::AddAce(pNewACL, ACL_REVISION, MAXDWORD, pTempAce,
                  ((PACE_HEADER) pTempAce)->AceSize)) 
            {
               rc = ::GetLastError();
               break;
            }
         }
      }
   }

   //----------------------------------------------------------------------------------------
   // Set the new DACL to the new SD.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      if (!::SetSecurityDescriptorDacl(&newSD, TRUE, pNewACL,FALSE)) 
      {
         rc = ::GetLastError();
      }
   }

   //----------------------------------------------------------------------------------------
   // Copy the old security descriptor control flags
   // regarding DACL automatic inheritance for Windows 2000 or
   // later where SetSecurityDescriptorControl() API is available
   // in advapi32.dll.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      SetSecurityDescriptorControlFnPtr _SetSecurityDescriptorControl = NULL;
      _SetSecurityDescriptorControl = (SetSecurityDescriptorControlFnPtr)
            GetProcAddress(GetModuleHandle(TEXT("advapi32.dll")),
            "SetSecurityDescriptorControl");
      if (_SetSecurityDescriptorControl) 
      {
         SECURITY_DESCRIPTOR_CONTROL controlBitsOfInterest = 0;
         SECURITY_DESCRIPTOR_CONTROL controlBitsToSet = 0;
         SECURITY_DESCRIPTOR_CONTROL oldControlBits = 0;
         DWORD dwRevision = 0;

         if (!::GetSecurityDescriptorControl(pFileSD, &oldControlBits,
            &dwRevision)) 
         {
            rc = ::GetLastError();
         }

         if(rc == ERROR_SUCCESS)
         {
            if (oldControlBits & SE_DACL_AUTO_INHERITED) 
            {
               controlBitsOfInterest = SE_DACL_AUTO_INHERIT_REQ | SE_DACL_AUTO_INHERITED;
               controlBitsToSet = controlBitsOfInterest;
            }
            else if (oldControlBits & SE_DACL_PROTECTED) 
            {
               controlBitsOfInterest = SE_DACL_PROTECTED;
               controlBitsToSet = controlBitsOfInterest;
            }

            if (controlBitsOfInterest) 
            {
               if (!_SetSecurityDescriptorControl(&newSD,controlBitsOfInterest,
                     controlBitsToSet)) 
               {
                  rc = ::GetLastError();
               }
            }
         }
      }
   }

   //----------------------------------------------------------------------------------------
   // Set the new SD to the File.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      if (!::SetFileSecurity(fileOrDirPathIn, secInfo, &newSD)) 
      {
         rc = ::GetLastError();
      }
   }

   // Deallocate objects.
   if(pFileSD) { delete [] pFileSD; }
   if(pNewACL) { delete [] pNewACL; }

   return rc;
}

static DWORD
   l_ConvertStringSidToSid (
      LPCTSTR  strSidIn,
      PSID    *sidOut
   )
{
   // Initialize return sid.
   *sidOut = 0;

   // Validate input.
   DWORD rc = ERROR_SUCCESS;
   if(strSidIn == 0 || _tcslen(strSidIn) == 0)
   {
      rc = ERROR_INVALID_SID;
   }

   // Parse the sid string and create SID.
   PSID pSid = NULL;
   LPTSTR szSidCopy = NULL;
   if(rc == ERROR_SUCCESS)
   {
      try
      {
         int i;
         LPTSTR ptr, ptr1;
         SID_IDENTIFIER_AUTHORITY sia; ZeroMemory(&sia, sizeof(sia));
         BYTE nByteAuthorityCount = 0;
         DWORD dwSubAuthority[8] = {0, 0, 0, 0, 0, 0, 0, 0};

         szSidCopy = new TCHAR[lstrlen(strSidIn) + 1];
         lstrcpy(szSidCopy, strSidIn);

         // S-SID_REVISION- + IdentifierAuthority- + subauthorities- + NULL

         // Skip 'S'
         if(!(ptr = _tcschr(szSidCopy, _T('-'))))
            rc = ERROR_INVALID_SID;

         if(rc == ERROR_SUCCESS)
         {
            // Skip '-'
            ptr++;

            // Skip SID_REVISION
            if(!(ptr = _tcschr(ptr, _T('-'))))
               rc = ERROR_INVALID_SID;

            if (rc == ERROR_SUCCESS)
            {
               // Skip '-'
               ptr++;

               // Skip IdentifierAuthority
               if(!(ptr1 = _tcschr(ptr, _T('-'))))
                  rc = ERROR_INVALID_SID;

               if(rc == ERROR_SUCCESS)
               {
                  *ptr1= 0;

                  if((*ptr == _T('0')) && (*(ptr + 1) == _T('x')))
                  {
                     _stscanf(ptr, _T("0x%02hx%02hx%02hx%02hx%02hx%02hx"),
                           &sia.Value[0],
                           &sia.Value[1],
                           &sia.Value[2],
                           &sia.Value[3],
                           &sia.Value[4],
                           &sia.Value[5]);
                  }
                  else
                  {
                     DWORD dwValue;
                     _stscanf(ptr, _T("%lu"), &dwValue);

                     sia.Value[5] = (BYTE)(dwValue & 0x000000FF);
                     sia.Value[4] = (BYTE)(dwValue & 0x0000FF00) >> 8;
                     sia.Value[3] = (BYTE)(dwValue & 0x00FF0000) >> 16;
                     sia.Value[2] = (BYTE)(dwValue & 0xFF000000) >> 24;
                  }

                  // Skip '-'
                  *ptr1 = '-';
                  ptr = ptr1;
                  ptr1++;

                  for(i = 0; i < 8; i++)
                  {
                     // Get subauthority
                     if(!(ptr = _tcschr(ptr, _T('-'))))
                           break;
                     *ptr = 0;
                     ptr++;
                     nByteAuthorityCount++;
                  }

                  for(i = 0; i < nByteAuthorityCount; i++)
                  {
                     // Get subauthority
                     _stscanf(ptr1, _T("%lu"), &dwSubAuthority[i]);
                     ptr1 += lstrlen(ptr1) + 1;
                  }
                  delete[] szSidCopy;
                  szSidCopy = NULL;

                  if(!AllocateAndInitializeSid(&sia,
                     nByteAuthorityCount,
                     dwSubAuthority[0],
                     dwSubAuthority[1],
                     dwSubAuthority[2],
                     dwSubAuthority[3],
                     dwSubAuthority[4],
                     dwSubAuthority[5],
                     dwSubAuthority[6],
                     dwSubAuthority[7],
                     &pSid))
                  {
                     pSid = NULL;
                  }

                  // Everything is okay update the return pointer.
                  if(pSid) 
                  { 
                     *sidOut = pSid; 
                     pSid = NULL;
                  }
                  else
                  {
                     rc = ERROR_INVALID_SID;
                  }
               }
            }
         }
      }
      catch(...)
      {
         delete[] szSidCopy;
         pSid = NULL;
      }
   }

   return rc;
}

//========================================================================================
// DllMain
//========================================================================================
BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}


//========================================================================================
// GiveLogonAsServicePriv
//========================================================================================
DWORD __stdcall
	GiveLogonAsServicePriv (
		LPCTSTR  accountIn
	)
{
   return l_GivePriv(accountIn,L"SeServiceLogonRight");
}

//========================================================================================
// GiveActAsPartOfOSPriv
//========================================================================================
DWORD __stdcall
	GiveActAsPartOfOSPriv (
		LPCTSTR  accountIn
	)
{
   return l_GivePriv(accountIn,L"SeTcbPrivilege");
}

//========================================================================================
// AddNtfsPermissions
//========================================================================================
DWORD __stdcall
   AddNtfsPermissions (
		LPCTSTR  accountIn,           // account in domain\user format
		LPCTSTR  fileOrDirPathIn,     // file or directory path.
      DWORD    accessMaskIn         // access rights.
)
{
   //----------------------------------------------------------------------------------------
   // Lookup the account name to get its SID.   Also verify if the account is a user.
   //----------------------------------------------------------------------------------------
   BYTE           sid[100];    
   DWORD          dwSid = sizeof(sid);
   TCHAR          domain[32];  
   DWORD          dwDomain = DIM(domain);
   SID_NAME_USE   sidNameUse;
   DWORD          rc = ERROR_SUCCESS;
   int strIndex = 0 ;

   if(_tcsnccmp(".\\", accountIn, 2) == 0)
	   strIndex = 2 ;

   if (::LookupAccountName 
         (NULL, &(accountIn[strIndex]), sid, &dwSid,
            domain, &dwDomain, &sidNameUse))   
   {
      if(sidNameUse != SidTypeUser)
      {
         rc = ERROR_NO_SUCH_USER; 
      }
   }
   else
   {
      rc = ::GetLastError();
   }

   //----------------------------------------------------------------------------------------
   // Set file/dir permissions.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      rc = l_AddNtfsPermissions(sid,fileOrDirPathIn,accessMaskIn);
   }

   return rc;
}

//========================================================================================
// AddNtfsPermissionsBySid
//========================================================================================
DWORD __stdcall
   AddNtfsPermissionsBySid (
		LPCTSTR  sidStrIn,            // SID string.
		LPCTSTR  fileOrDirPathIn,     // file or directory path.
      DWORD    accessMaskIn         // access rights.
)
{
   //----------------------------------------------------------------------------------------
   // Convert SID string to SID.
   //----------------------------------------------------------------------------------------
   PSID  pSid = 0;
   DWORD rc   = l_ConvertStringSidToSid(sidStrIn,&pSid);

   //----------------------------------------------------------------------------------------
   // Set file/dir permissions.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      rc = l_AddNtfsPermissions((BYTE*)pSid,fileOrDirPathIn,accessMaskIn);
   }

   //----------------------------------------------------------------------------------------
   // Free resources.
   //----------------------------------------------------------------------------------------
   if(pSid) { ::FreeSid(pSid); }

   return rc;
}

//========================================================================================
// CreateDirAndGiveFullControl
//========================================================================================
DWORD __stdcall
   CreateDirAndGiveFullControl (
		LPCTSTR  dirPathIn,     // Path of directory to create.
		LPCTSTR  sidIn                // Account SID to give full control to.
)
{
   DWORD rc = ERROR_SUCCESS;

   // Validate input.
   if(dirPathIn == 0 || ::_tcslen(dirPathIn) == 0  // non-empty path.
      || sidIn == 0 || ::_tcslen(sidIn) == 0)      // non-empty sid.
   {
      rc = ERROR_INVALID_PARAMETER;
   }

   // Convert string SID to SID.
   PSID pSid = 0;
   if(rc == ERROR_SUCCESS)
   {
      rc = l_ConvertStringSidToSid(sidIn,&pSid);
   }

   // Setup security descriptor.
   EXPLICIT_ACCESS      ea;
   PACL                 pACL = 0;
   PSECURITY_DESCRIPTOR pSD  = 0;
   SECURITY_ATTRIBUTES  sa;
   if(rc == ERROR_SUCCESS)
   {
      // Create explicit access for the input SID to have full control.
      ::memset((void*)&ea,0,sizeof(ea));
      ea.grfAccessMode        = SET_ACCESS;
      ea.grfAccessPermissions = FILE_ALL_ACCESS;
      ea.grfInheritance       = SUB_CONTAINERS_AND_OBJECTS_INHERIT;
      ea.Trustee.TrusteeForm  = TRUSTEE_IS_SID;
      ea.Trustee.TrusteeType  = TRUSTEE_IS_UNKNOWN;
      ea.Trustee.ptstrName    = (LPTSTR)pSid;

      // Create ACL.
      rc = ::SetEntriesInAcl(1,&ea,0,&pACL);

      // Create and initialize security descriptor.
      if(rc == ERROR_SUCCESS)
      {
         pSD = new BYTE[SECURITY_DESCRIPTOR_MIN_LENGTH];
         if(!pSD)
         {
            rc = ERROR_OUTOFMEMORY;
         }

         if(rc == ERROR_SUCCESS)
         {
            if(!::InitializeSecurityDescriptor(pSD,SECURITY_DESCRIPTOR_REVISION))
            {
               rc = ::GetLastError();
            }
         }
      }

      // Add the ACL to the security descriptor.
      if(rc == ERROR_SUCCESS)
      {
         if(!::SetSecurityDescriptorDacl(pSD,TRUE,pACL,FALSE))
         {
            rc = ::GetLastError();
         }
      }

      // Initialize security attributes.
      if(rc == ERROR_SUCCESS)
      {
         sa.nLength              = sizeof(SECURITY_ATTRIBUTES);
         sa.lpSecurityDescriptor = pSD;
         sa.bInheritHandle       = FALSE;
      }
   }

   // Create the directory.
   if(rc == ERROR_SUCCESS)
   {
      if(!::CreateDirectory(dirPathIn,&sa))
      {
         rc = ::GetLastError();
      }
   }

   // Free resources.
   if(pSid) { ::FreeSid(pSid); }
   if(pACL) { ::LocalFree(pACL); }
   if(pSD) { delete [] pSD; }

   return rc;
}

//========================================================================================
// SetObjectAccess 
//========================================================================================
DWORD __stdcall
   SetObjectAccess (
		LPCTSTR  sidIn,               // SID string.
      DWORD    objTypeIn,           // Object type 
                                    // 1 - file/dir type.
                                    // 4 - registry.
		LPCTSTR  pathIn,              // Object path.
      DWORD    accessMaskIn         // Access rights.
)
{
   DWORD rc = ERROR_SUCCESS;

   // Validate input.
   if(pathIn == 0 || ::_tcslen(pathIn) == 0       // non-empty path.
      || sidIn == 0 || ::_tcslen(sidIn) == 0)     // non-empty sid.
   {
      rc = ERROR_INVALID_PARAMETER;
   }

   // Convert string SID to SID.
   PSID pSid = 0;
   if(rc == ERROR_SUCCESS)
   {
      rc = l_ConvertStringSidToSid(sidIn,&pSid);
   }

   // Setup ACL.
   EXPLICIT_ACCESS      ea;
   PACL                 pACL = 0;
   if(rc == ERROR_SUCCESS)
   {
      // Create explicit access for the input SID to have full control.
      ::memset((void*)&ea,0,sizeof(ea));
      ea.grfAccessMode        = SET_ACCESS;
      ea.grfAccessPermissions = accessMaskIn;
      ea.grfInheritance       = SUB_CONTAINERS_AND_OBJECTS_INHERIT;
      ea.Trustee.TrusteeForm  = TRUSTEE_IS_SID;
      ea.Trustee.TrusteeType  = TRUSTEE_IS_UNKNOWN;
      ea.Trustee.ptstrName    = (LPTSTR)pSid;

      // Create ACL.
      rc = ::SetEntriesInAcl(1,&ea,0,&pACL);
   }

   // Set the DACL on the object and prevent inheritance.
   if(rc == ERROR_SUCCESS)
   {
      rc = ::SetNamedSecurityInfo((LPSTR)pathIn,(SE_OBJECT_TYPE)objTypeIn,
               DACL_SECURITY_INFORMATION | PROTECTED_DACL_SECURITY_INFORMATION,0,0,pACL,0);
   }

   // Free resources.
   if(pSid) { ::FreeSid(pSid); }
   if(pACL) { ::LocalFree(pACL); }

   return rc;
}

//========================================================================================
// VerifyPassword
//========================================================================================
DWORD __stdcall
   VerifyPassword (
		LPCTSTR  accountIn,           // account in domain\user format
		LPCTSTR  passwordIn           // password
)
{
   //----------------------------------------------------------------------------------------
   // Validate inputs.
   //----------------------------------------------------------------------------------------
   if(accountIn == 0 || accountIn[0] == 0)
   {
      return ERROR_INVALID_ACCOUNT_NAME; 
   }
   if(passwordIn == 0 || passwordIn[0] == 0)
   {
      return ERROR_ILL_FORMED_PASSWORD; 
   }

   //----------------------------------------------------------------------------------------
   // Make a copy of the input account and parse into domain and user pieces.
   // Make sure domain and user are parsed out and not empty.
   //----------------------------------------------------------------------------------------
   // Copy input account to local buffer.
   TCHAR *acct = new TCHAR[::_tcslen(accountIn)+1];
   if (acct == 0) { return ERROR_OUTOFMEMORY; }
   ::_tcscpy(acct,accountIn);

   // Parse the account to domain and user.
   const TCHAR *WHACK_SEP = _T("\\");
   DWORD rc = ERROR_SUCCESS;
   TCHAR *dom = 0, *user = 0;
   dom = ::_tcstok(acct,WHACK_SEP);
   if(dom) { user = ::_tcstok(0, WHACK_SEP); }
   if (dom == 0 || dom[0] == 0 || user == 0 || user[0] == 0)
   {
      rc = ERROR_INVALID_ACCOUNT_NAME; 
   }

   TCHAR hostName[128] ;
   if(dom[0] == '.')
   {
      gethostname(hostName, 127) ;
      dom = hostName ;
   }
   //----------------------------------------------------------------------------------------
   // Check if this is being run on XP.
   //----------------------------------------------------------------------------------------
   bool isXp = false;
   if(rc == ERROR_SUCCESS)
   {
      OSVERSIONINFO osVerInfo;
      osVerInfo.dwOSVersionInfoSize = sizeof(osVerInfo);
      if(::GetVersionEx(&osVerInfo))
      {
         isXp = osVerInfo.dwMajorVersion == 5 && osVerInfo.dwMinorVersion == 1;
      }
      else
      {
         rc = ::GetLastError();
      }
   }

   //----------------------------------------------------------------------------------------
   // If XP use LogonUser API or use the SSPI method of validating password.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      if(isXp)
      {
         HANDLE token = NULL;
         if(!::LogonUser(user,dom,passwordIn,LOGON32_LOGON_NETWORK,
               LOGON32_PROVIDER_DEFAULT,&token))
         {
            rc = ::GetLastError();
         }
         if(token != NULL) { ::CloseHandle(token); }
      }
      else
      {
         if(!SSPI_Helpers::LogonUser(dom,user,passwordIn))
         {
            rc = ERROR_LOGON_FAILURE;
         }
      }
   }

   //----------------------------------------------------------------------------------------
   // Free resources.
   //----------------------------------------------------------------------------------------
   delete [] acct;

   return rc;
}

//========================================================================================
// GetEventLogFileOptions
//========================================================================================
DWORD __stdcall
	GetEventLogRetentionInfo (
		LPCTSTR   eventLogNameIn,     // event log name - Application, System, Security, etc.
      DWORD    *retentionTypeOut,   // retention type - 0:overwrite events as needed, 1: do not overwrite, 2:overwrite events older then N days.
      DWORD    *retentionDaysOut    // retention days - if retentionTypeOut is 2, than this value is number of days events are retained.
	)
{
   const TCHAR * const APPLICATION_LOG          = _T("Application");
   const TCHAR * const SYSTEM_LOG               = _T("System");
   const TCHAR * const SECURITY_LOG             = _T("Security");
   const TCHAR * const REG_EVENT_LOG_KEY_STR    = _T("System\\CurrentControlSet\\Services\\EventLog\\");
   const TCHAR * const REG_RETENTION_VAL_STR    = _T("Retention");
   const DWORD OVERWRITE_AS_NEEDED              = 0;
   const DWORD DO_NOT_OVERWRITE                 = 1;
   const DWORD OVERWRITE_EVENTS_OLDER_THEN      = 2;
   const DWORD SECS_IN_A_DAY                    = 86400;

   //----------------------------------------------------------------------------------------
   // Validate inputs.
   //----------------------------------------------------------------------------------------
   if(!eventLogNameIn || !retentionTypeOut || !retentionDaysOut)
   { 
      return ERROR_INVALID_PARAMETER;
   }
   if(_tcsicmp(eventLogNameIn, APPLICATION_LOG)
      && _tcsicmp(eventLogNameIn, SYSTEM_LOG)
      && _tcsicmp(eventLogNameIn, SECURITY_LOG))
   {
      return ERROR_INVALID_PARAMETER;
   }

   //----------------------------------------------------------------------------------------
   // Construct the registry key string for the specified event log.
   //----------------------------------------------------------------------------------------
   TCHAR *regKeyNameStr = new TCHAR[_tcslen(REG_EVENT_LOG_KEY_STR)+_tcslen(eventLogNameIn)+1];
   if(!regKeyNameStr)
   {
      return E_OUTOFMEMORY;
   }
   _tcscpy(regKeyNameStr,REG_EVENT_LOG_KEY_STR);
   _tcscat(regKeyNameStr,eventLogNameIn);

   //----------------------------------------------------------------------------------------
   // Open the registry key that contains the log file settings.
   //----------------------------------------------------------------------------------------
   CRegKey regKey;
   DWORD rc = regKey.Open(HKEY_LOCAL_MACHINE,regKeyNameStr,KEY_READ);

   //----------------------------------------------------------------------------------------
   // Read the retentions value, and determine retention type and days.
   //----------------------------------------------------------------------------------------
   if(rc == ERROR_SUCCESS)
   {
      DWORD val = 0;
      rc = regKey.QueryDWORDValue(REG_RETENTION_VAL_STR,val);

      if(rc == ERROR_SUCCESS)
      {
         switch(val)
         {
         case 0:
            *retentionTypeOut = OVERWRITE_AS_NEEDED;
            *retentionDaysOut = 0;
            break;
         case 0xFFFFFFFF:
            *retentionTypeOut = DO_NOT_OVERWRITE;
            *retentionDaysOut = 0;
            break;
         default:
            *retentionTypeOut = OVERWRITE_EVENTS_OLDER_THEN;
            *retentionDaysOut = val/SECS_IN_A_DAY;
            break;
         }
      }
   }

   //----------------------------------------------------------------------------------------
   // Deallocate reg key str.
   //----------------------------------------------------------------------------------------
   delete [] regKeyNameStr;

   return rc;
}

//========================================================================================
// GetMediaType
//========================================================================================
DWORD __stdcall
	GetMediaType (
		LPCTSTR  driveIn,             // drive letter in c:\ format.
      DWORD   *driveTypeOut         // drive type returned from the API.
	)
{
   //----------------------------------------------------------------------------------------
   // Validate input.
   //----------------------------------------------------------------------------------------
   if (driveIn == 0 || driveIn[0] == 0
       || ::_tcslen(driveIn) != 3 || driveIn[1] != _T(':') || driveIn[2] != _T('\\'))
   {
      return ERROR_INVALID_NAME; // The filename, directory name, or volume label syntax is incorrect.
   }
   if (driveTypeOut == 0)
   {
      return ERROR_INVALID_PARAMETER;
   }

   *driveTypeOut = ::GetDriveType(driveIn);
   return ERROR_SUCCESS;
}
//========================================================================================
// ConvertHexEncodedString
//========================================================================================
DWORD __stdcall
   ConvertHexEncodedString (
      WCHAR *hexStringIn,              // Hex coded string in.
      WCHAR *stringOut              // Decoded string out.
   )
{
   // Validate inputs.
   if(hexStringIn == 0 || stringOut == 0 
         || (::wcslen(hexStringIn) % 4) != 0)
   {
      return ERROR_INVALID_PARAMETER;
   }

   // Initialize return string.
   stringOut[0] = 0;

   // Now convert the chars in groups of 4.
   WCHAR *hPtr = hexStringIn,
         *ptr  = stringOut;
   while (*hPtr != 0)
   {
      // Copy 4 chars.
      WCHAR temp[5];
      temp[4] = 0;
      ::wcsncpy(temp,hPtr,4);

      // Convert to unsigned long and cast to UNICODE.
      WCHAR *ignore = 0;
      unsigned long ul = ::wcstoul(temp,&ignore,16);
      *ptr = (WCHAR)ul;

      // Increment pointers.
      hPtr += 4;
      ++ptr;
   }

   // Put null terminating char.
   *ptr = 0;

   return ERROR_SUCCESS;
}
