rem Registers Agent and Collection services to point to release directory

pushd ..\Idera\SQLcompliance\CollectionService\bin\release
"c:\windows\microsoft.net\framework\v2.0.50727\installutil.exe" SQLcomplianceCollectionService.exe
popd

pushd ..\Idera\SQLcompliance\AgentService\bin\release
"c:\windows\microsoft.net\framework\v2.0.50727\installutil.exe" SQLcomplianceAgent.exe
popd

