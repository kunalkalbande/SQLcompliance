rem Removes service registration for Agent and Collection services in the debug directory

pushd ..\Idera\SQLcompliance\CollectionService\bin\debug
"c:\windows\microsoft.net\framework\v2.0.50727\installutil.exe" /u SQLcomplianceCollectionService.exe
popd

pushd ..\Idera\SQLcompliance\AgentService\bin\debug
"c:\windows\microsoft.net\framework\v2.0.50727\installutil.exe" /u SQLcomplianceAgent.exe
popd

