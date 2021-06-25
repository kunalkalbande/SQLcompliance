SQL Compliance Manager 5.4 - IMPORTANT  INSTALLATION INSTRUCTIONS

SQL Compliance Manager 5.4 depends on certain Microsoft components that did not ship with SQL Server versions prior to SQL 2012 SP1. If you will be installing SQL Compliance Manager’s collection service on a repository running on SQL 2012 or below, it is necessary to install these components manually.

For your convenience these components are already included in this download. You may also download them from the Microsoft download page for the SQL Server 2016 Feature Pack (https://www.microsoft.com/en-us/download/details.aspx?id=52676 and select SharedManagementObjects.msi and SQLSysClrTypes.msi for 32 bit or 64 bit, depending on which version you are installing). The SQL 2016 Feature Pack is backwards compatible with prior versions of SQL Server and can coexist with SQL 2012 and below.

Installation / Upgrade Instructions
SQL Compliance Manager Windows Desktop Client or Collection Service

SQL 2012 SP1, SQL 2014, or SQL 2016
Run IDERASQLCmInstallationKit to upgrade or install SQL Compliance Manager 5.4

SQL 2012 or below
Install the following 3 packages in this order:
1. Run SQLSysClrTypes.msi to install the System CLR Types assembly
2. Run SharedManagementObjects.msi to install the Shared Management Objects assembly
3. Run IDERASQLCmInstallationKit to upgrade or install SQL Compliance Manager 5.4


SQL Compliance Manager Agent
You may deploy the SQL Compliance Manager agents from the SQL Compliance Manager desktop client. 







Copyright © 2018 IDERA