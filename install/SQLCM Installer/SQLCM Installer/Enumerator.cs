namespace SQLCM_Installer
{
    public enum WizardPage : byte
    {
        NotSpecified,
        Introduction,
        SetupType,
        UpgradeIntroduction,
        InstallationDirectory,
        TraceDirectory,
        Repositories,
        ServiceAccount,
        AgentSQLServer,
        AgentCollectionServer,
        Summary,
        Install,
        Success,
        Error,
        DashboardDetection
    }

    public enum NavigationDirection : byte
    {
        Previous,
        Next,
        Error,
        Finish
    }

    public enum InstallType : byte
    { 
        NotSpecified,
        CMOnly,
        ConsoleOnly,
        AgentOnly,
        DashboardOnly,
        CMAndDashboard,
        ConsoleAndDashboard,
        AgentAndDashboard
    }

    public enum Products : byte
    { 
        Agent,
        Console,
        Compliance,
        Dashboard,
        Registration,
        NA
    }

    public enum InstalledProducts : byte
    {
        AgentX86,
        AgentX64,
        ComplianceX86,
        ComplianceX64,
        DashboardX86,
        DashboardX64
    }

    public enum ServiceStatus : byte
    { 
        NotSpecified,
        NotRunning,
        NotFound,
        NotReachable,
        OK
    }
}
