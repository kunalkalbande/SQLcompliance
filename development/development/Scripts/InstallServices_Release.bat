rem Registers Agent and Collection services to point to release directory

pushd ..\Idera\SQLcompliance\CollectionService\bin\release
"c:\windows\microsoft.net\framework\v1.1.4322\installutil.exe" SQLcomplianceCollectionService.exe
popd

pushd ..\Idera\SQLcompliance\AgentService\bin\release
"c:\windows\microsoft.net\framework\v1.1.4322\installutil.exe" SQLcomplianceAgent.exe
popd

