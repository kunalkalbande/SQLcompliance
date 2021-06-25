<#
.SYNOPSIS
    Move SQLcompliance files from one directory to another while also files which cannot be process into a separte directory.
.DESCRIPTION
    Move SQLcompliance files from one directory to another while also files which cannot be process into a separte directory.
.PARAMETER BatchSize
    The number of files to be processed in a single execution of the script.
.PARAMETER ProcessablePath
    The folder path where the script will move processable files into. 
    Processable files will be processed using the Trace Register utility.
.PARAMETER UnprocessablePath
    The folder path where the script will move unprocessable files into.
.PARAMETER MaxFiles
    The maximum number of files within the Collection Server Trace File directory. 
    If the total count of files in the Collection Server Trace File directory exceeds
    this value, the script will not move any files.
.EXAMPLE
    .\SQLCM_ProcessBacklog.ps1 
.EXAMPLE
    .\SQLCM_ProcessBacklog.ps1 -BatchSize 5000
.EXAMPLE
    .\SQLCM_ProcessBacklog.ps1 -BatchSize 5000 -ProcessablePath "D:\FilesToBeProcessed"
.EXAMPLE
    .\SQLCM_ProcessBacklog.ps1 -BatchSize 5000 -ProcessablePath "D:\FilesToBeProcessed" -UnprocessablePath "D:\FilesCannotProcess" -MaxFiles 5000
.NOTES
    This PowerShell script should be executed from the SQLcompliance Collection Server.
    The user executing this script must have sysadmin rights on the SQL instance hosting the
    SQL Compliance Manager repository databases and local admin rights on the server itself.

    Author: Tep Chantra
    Date: 10/4/2019
#>

Param(
        [Parameter(Mandatory=$False,Position=0)] 
        [ValidateNotNullOrEmpty()]
        [int]$BatchSize=1000,

        [Parameter(Mandatory=$False,Position=1)] 
        [ValidateNotNullOrEmpty()]
        [string]$ProcessablePath,

        [Parameter(Mandatory=$False,Position=2)] 
        [ValidateNotNullOrEmpty()]
        [string]$UnprocessablePath,

        [Parameter(Mandatory=$False,Position=3)] 
        [ValidateNotNullOrEmpty()]
        [int]$MaxFiles=500
    )

#Requires -RunAsAdministrator

Function ResultHeader {
        Write-Host "`n"
        Write-Host "-----------------------------------------------------------------"
        Write-Host "Result" -ForegroundColor Green
        Write-Host "-----------------------------------------------------------------"    
}

Function InitiateCount ($Path, [ref]$Count)
{
    If (Test-Path -Path $Path) {
        $Count.value = (Get-ChildItem -Path $Path -File | Measure-Object).Count
    } Else {
        $Count.value = 0
    }
}

Function CheckProcessablePath {
    If ((Get-ChildItem -Path $ProcessablePath -File | Measure-Object).Count -ne 0 ) {
        ResultHeader
        Write-Host "Description`t`t" -ForegroundColor Cyan -NoNewLine;Write-Host "|" -NoNewLine;Write-Host " Count`t" -ForegroundColor Cyan -NoNewLine;Write-Host "|" -NoNewLine;Write-Host " Directory" -ForegroundColor Cyan;
        Write-Host "-----------------------------------------------------------------"
        Write-Host "Processable Files`t| $TotalProcessableFileCount`t| $ProcessablePath"
        Write-Host "-----------------------------------------------------------------"
        Write-Host "`n"
        Write-Host "NOTE: There are existing files in the processable files directory which needs to be processed using the Trace Register utility." -ForegroundColor Yellow
        Write-Host "`n"
        Exit 1
    }
}

Function ProcessFiles {
        $Counter = 0
        $UnprocessedFileCount = (Get-ChildItem -Path $UnprocessedTraceDirectoryPath -File | Measure-Object).Count
        $UnprocessableFileCount = 0

        If ($UnprocessedFileCount -lt $BatchSize) {
            $BatchSize = $UnprocessedFileCount
        }

        WHILE ($Counter -lt $BatchSize) {

            $Files = Get-ChildItem -Path $UnprocessedTraceDirectoryPath -File -Force | Select-Object -First 2

            If($Files[0].Basename -eq $Files[1].Basename -And $Files[0].Extension -in (".biz",".bi7z") -And $Files[1].Extension -in (".trz",".tr7z") -and $Files[0].Basename -notlike "*_1_998_*" -And $Files[1].Basename -notlike "*_1_998_*") {
                $Files | Move-Item -Destination $ProcessablePath
                $Counter = $Counter + 2
            } Else {
                
                New-Item -Path $UnprocessablePath -ItemType "Directory" -Force | Out-Null
                $Files[0] | Move-Item -Destination $UnprocessablePath
                $UnprocessableFileCount = $UnprocessableFileCount + 1
                $Counter = $Counter + 1
            }

        }

        $ProcessedFileCount = $BatchSize - $UnprocessableFileCount
        $RemainingFileCount = $UnprocessedFileCount - $BatchSize

        ResultHeader
        Write-Host "Description`t`t`t" -ForegroundColor Cyan -NoNewLine;Write-Host "|" -NoNewLine;Write-Host " Count`t" -ForegroundColor Cyan -NoNewLine;Write-Host "|" -NoNewLine;Write-Host " Directory" -ForegroundColor Cyan;
        Write-Host "-----------------------------------------------------------------"
        Write-Host "Processable Files Moved`t`t| $ProcessedFileCount`t| $ProcessablePath"
        Write-Host "Unprocessable Files Moved`t| $UnprocessableFileCount`t| $UnprocessablePath"
        Write-Host "Total Files Moved`t`t| $BatchSize`t|" 
        Write-Host "Remaining Files`t`t`t| $RemainingFileCount`t|" 
        Write-Host "-----------------------------------------------------------------"
        Write-Host "`n"
        Write-Host "NOTE: Use the Trace Register utility to process the files in the processable directory." -ForegroundColor Green
        If ($UnprocessableFileCount -ne 0) {
            Write-Host "NOTE: Some files were moved to the unprocessable files directory because they cannot be processed by the Trace Register tool." -ForegroundColor Yellow
        }
        Write-Host "`n"
}

$ErrorActionPreference = "Stop"

If ($PSVersionTable.PSVersion.Major -lt 5) {
    Write-Host "`nThis script requires PowerShell version 5.0 or later.`n" -ForegroundColor Red -Backgroundcolor Black
    Exit 1
}

If (!($BatchSize % 2 -eq 0)) {
    Write-Host "`nBatchSize must be an even number.`nAborting script.`n" -ForegroundColor Red -Backgroundcolor Black
    Exit 1
}

<# BEGIN -- IMPORTANT VARIABLES #>

Try {$OriginalTraceDirectoryPath = (Get-ItemProperty -Path HKLM:\SOFTWARE\Idera\SQLCM\CollectionService -Name TraceDirectory).TraceDirectory}
Catch { Write-Host "`nThe SQL Compliance Collection Service TraceDirectory key could not be located in the registry.`n" -Foregroundcolor Red -Backgroundcolor Black;Write-Error $_.Exception.Message }

If ($OriginalTraceDirectoryPath.Length -eq $OriginalTraceDirectoryPath.LastIndexOf("\")+1) {
    $OriginalTraceDirectoryPath = $OriginalTraceDirectoryPath.Substring(0,$OriginalTraceDirectoryPath.LastIndexOf("\"))
}

If ((Split-Path -Path $OriginalTraceDirectoryPath) -eq "") {
    $UnprocessedTraceDirectoryPath = $OriginalTraceDirectoryPath + "\UnprocessedTraceFiles"
} Else {
    $UnprocessedTraceDirectoryPath = (Split-Path -Path $OriginalTraceDirectoryPath) + "\UnprocessedTraceFiles"
}

If ($ProcessablePath -eq "") {
    $ProcessablePath = $UnprocessedTraceDirectoryPath + "\Processable"
} Else {
    Try {New-Item -Path $ProcessablePath -ItemType "Directory" -Force -ErrorAction Stop | Out-Null}
    Catch { Write-Error $_.Exception.Message }
}

If ($UnprocessablePath -eq "") {
    $UnprocessablePath = $UnprocessedTraceDirectoryPath + "\Unprocessable" 
} Else {
    Try {New-Item -Path $UnprocessablePath -ItemType "Directory" -Force -ErrorAction Stop | Out-Null}
    Catch { Write-Error $_.Exception.Message }
}

$TotalUnprocessedFileCount = 0
InitiateCount $UnprocessedTraceDirectoryPath ([ref]$TotalUnprocessedFileCount)

$TotalProcessableFileCount = 0
InitiateCount $ProcessablePath ([ref]$TotalProcessableFileCount)

$TotalUnprocessableFileCount = 0
InitiateCount $UnprocessablePath ([ref]$TotalUnprocessableFileCount)

$TotalCollectionServerTraceDirectoryFileCount = 0
InitiateCount $OriginalTraceDirectoryPath ([ref]$TotalCollectionServerTraceDirectoryFileCount)

<# END -- IMPORTANT VARIABLES #>

If (!(Test-Path -Path $UnprocessedTraceDirectoryPath)) {
    <#
        Creates the unprocessed trace directory by renaming the original collect server trace file directory.
        The script also makes a copy of the [Jobs] table in the SQLcompliance database.
        Then, it proceeds to move files into the Processable/Unprocessable directories accordingly.
    #>
    Try {$SQLInstance = (Get-ItemProperty -Path HKLM:\SOFTWARE\Idera\SQLCM\CollectionService -Name ServerInstance).ServerInstance}
    Catch { Write-Host "`nThe SQL Compliance Collection Service ServerInstance key could not be located in the registry.`n" -Foregroundcolor Red -Backgroundcolor Black;Write-Error $_.Exception.Message }

    Stop-Service "SQLcomplianceCollectionService"

    Try {Invoke-Sqlcmd -ServerInstance $SQLInstance -Query "IF OBJECT_ID('[SQLcompliance].[dbo].[JobsTemp]', 'U') IS NOT NULL DROP TABLE [SQLcompliance].[dbo].[JobsTemp]; SELECT * INTO [SQLcompliance].[dbo].[JobsTemp] FROM [SQLcompliance].[dbo].[Jobs];TRUNCATE TABLE [SQLcompliance].[dbo].[Jobs];" }
    Catch {Start-Service "SQLcomplianceCollectionService";Write-Error $_.Exception.Message}

    Try {Rename-Item -Path $OriginalTraceDirectoryPath -NewName $UnprocessedTraceDirectoryPath -ErrorAction Stop} 
    Catch {Write-Error $_.Exception.Message}

    New-Item -Path $OriginalTraceDirectoryPath -ItemType "Directory" -Force | Out-Null
    Get-Acl -Path $UnprocessedTraceDirectoryPath | Set-Acl -Path $OriginalTraceDirectoryPath

    Start-Service "SQLcomplianceCollectionService"  

    New-Item -Path $ProcessablePath -ItemType "Directory" -Force | Out-Null

    CheckProcessablePath

    ProcessFiles

} Else {
    <#
        The unprocessed trace directory already exists. First, check to see if there are any files located in 
        the directory which needs to be processed.
    #>
    If ($TotalUnprocessedFileCount -eq 0) {
        ResultHeader
        Write-Host "Description`t`t`t" -ForegroundColor Cyan -NoNewLine;Write-Host "|" -NoNewLine;Write-Host " Count`t" -ForegroundColor Cyan -NoNewLine;Write-Host "|" -NoNewLine;Write-Host " Directory" -ForegroundColor Cyan;
        Write-Host "-----------------------------------------------------------------"
        Write-Host "Trace Directory`t`t`t| $TotalCollectionServerTraceDirectoryFileCount`t| $OriginalTraceDirectoryPath"
        Write-Host "Unprocessed Directory`t`t| $TotalUnprocessedFileCount`t| $UnprocessedTraceDirectoryPath"
        Write-Host "- Processable Directory`t`t| $TotalProcessableFileCount`t| $ProcessablePath"
        Write-Host "- Unprocessable Directory`t| $TotalUnprocessableFileCount`t| $UnprocessablePath"
        Write-Host "-----------------------------------------------------------------"
        Write-Host "`n"
        Write-Host "NOTE: There are no more files in the unprocessed directory." -ForegroundColor Green
        If ($TotalProcessableFileCount -eq 0 -And $TotalUnprocessableFileCount -eq 0) {
            Write-Host "NOTE: All files have been processed successfully." -ForegroundColor Green
        } Else {
            Write-Host "NOTE: There are files which have not been completely processed." -ForegroundColor Yellow
        }
        Write-Host "`n"
    } Else {
        <# 
            There are files in the unprocessed trace directory which needs to be processed. First, check to
            make sure that the number of files in the collection server trace directory is lower than the
            MaxFiles threshold.
        #>
        If ($TotalCollectionServerTraceDirectoryFileCount -lt $MaxFiles) {
            CheckProcessablePath
            ProcessFiles
        } Else {
            ResultHeader
            Write-Host "-----------------------------------------------------------------"
            Write-Host "Description`t" -ForegroundColor Cyan -NoNewLine;Write-Host "|" -NoNewLine;Write-Host " Count`t" -ForegroundColor Cyan -NoNewLine;Write-Host "|" -NoNewLine;Write-Host " Directory" -ForegroundColor Cyan;
            Write-Host "-----------------------------------------------------------------"
            Write-Host "Trace Directory`t| $TotalCollectionServerTraceDirectoryFileCount`t| $OriginalTraceDirectoryPath"
            Write-Host "-----------------------------------------------------------------"
            Write-Host "`n"
            Write-Host "NOTE: There are too many files in the Collection Server Trace directory. Please wait until the count is lower than $MaxFiles." -ForegroundColor Yellow
            Write-Host "`n"
            Exit 1
        }
    }
}





