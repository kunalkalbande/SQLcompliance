-------------------------------------------INSTALLSHIELD SCHEMA VERSION UPDATE-------------------------------------------
Please ensure that you have changed schema version both in sql scripts and in InstallShield projects for:
install\x64\SQLcompliance-x64.ism
install\x86\SQLcompliance.ism
install\IA64\SQLcompliance-ia64.ism

Code example where properies is located:

	<row><td>SCHEMA_VERSION</td><td>1401</td><td>Current repository version.</td></row>
	<row><td>SCHEMA_VERSION_UPDATED</td><td>1402</td><td>New repository version.</td></row>

-------------------------------------------SQL SCRIPTS SCHEMA VERSION UPDATE-------------------------------------------

With new release of CM, schema should be updated as well (UpdateSPs.sql, upgrade.sql):
Code example:

	UPDATE [SQLcompliance]..[Configuration] SET [sqlComplianceDbSchemaVersion]=1402,[eventsDbSchemaVersion ]=703;  