copy BlackHen.Threading.dll \windows\Microsoft.net\Framework\v1.1.4322\*.*
copy Microsoft.Samples.Runtime.Remoting.Security.dll \windows\Microsoft.net\Framework\v1.1.4322\*.*
copy Microsoft.Samples.Security.SSPI.dll \windows\Microsoft.net\Framework\v1.1.4322\*.*
copy Xceed.Compression.dll \windows\Microsoft.net\Framework\v1.1.4322\*.*
copy Xceed.Compression.Formats.dll \windows\Microsoft.net\Framework\v1.1.4322\*.*
copy Xceed.Grid.dll \windows\Microsoft.net\Framework\v1.1.4322\*.*
copy Xceed.Grid.UIStyle.dll \windows\Microsoft.net\Framework\v1.1.4322\*.*
copy Xceed.SmartUI.dll \windows\Microsoft.net\Framework\v1.1.4322\*.*
copy Xceed.UI.dll \windows\Microsoft.net\Framework\v1.1.4322\*.*
rem Note that Interop.SQLDMO isnt on this list - it wont load - no strong names

cd \windows\Microsoft.net\Framework\v1.1.4322\*.*

gacutil -i BlackHen.Threading.dll
gacutil -i Microsoft.Samples.Runtime.Remoting.Security.dll
gacutil -i Microsoft.Samples.Security.SSPI.dll
gacutil -i Xceed.Compression.dll
gacutil -i Xceed.Compression.Formats.dll
gacutil -i Xceed.Grid.dll
gacutil -i Xceed.Grid.UIStyle.dll
gacutil -i Xceed.SmartUI.dll
gacutil -i Xceed.UI.dll

