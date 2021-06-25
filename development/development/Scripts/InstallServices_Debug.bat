rem Registers Agent and Collection services to point to debug directory

pushd ..\Idera\SQLcompliance\CollectionService\bin\debug
"c:\windows\microsoft.net\framework\v1.1.4322\installutil.exe" SQLcomplianceCollectionService.exe
popd

pushd ..\Idera\SQLcompliance\AgentService\bin\debug
"c:\windows\microsoft.net\framework\v1.1.4322\installutil.exe" SQLcomplianceAgent.exe
popd

