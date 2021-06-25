namespace CwfAddinInstaller
{
    internal enum WizardPage : byte
    {
        NotSpecified,   
        Welcome,
        ConfiguringSQLCM,
        DashboardLocation,
        ConfiguringCWFDashboard,
        Install,
        Finish,
        Error,
        LicenseAgreement,
        RepositoryDatabase,
        InstanceName,
        ReadyToInstall,
    }

    internal enum NavigationDirection : byte
    {
        Previous,
        Next,
        Error,
        Finish
    }
}
