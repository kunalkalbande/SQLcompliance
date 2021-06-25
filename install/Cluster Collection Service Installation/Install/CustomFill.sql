USE SQLcompliance
GO

declare @Network_Name nvarchar(256),
	@Instance_Name nvarchar(400)

-- *****************   SET NAMES HERE   *******************************

select @Network_Name = N'REPLACE_ME_WITH_SQL_NETWORK_NAME',
	@Instance_Name = N'REPLACE_ME_WITH_INSTANCE_NAME'

-- *****************    END SET NAMES   *******************************


INSERT INTO [Configuration] ([timeLastModified],[warnNoHeartbeatWarning],
                             [auditLogins],[auditFailedLogins],[auditDDL],[auditSecurity],[auditAdmin],[auditDBCC],
                             [auditDML],[auditSELECT],[auditFailures],[auditSystemEvents],
                             [auditDmlAll],[auditUserTables],[auditSystemTables],[auditStoredProcedures],[auditDmlOther],
                             [auditUserAll],
                             [auditUserLogins],[auditUserFailedLogins],[auditUserDDL],[auditUserSecurity],[auditUserAdmin],
                             [auditUserDML],[auditUserSELECT],[auditUserFailures],
                             [serverPort],[agentPort],[recoveryModel],
			                 [sqlComplianceDbSchemaVersion],[eventsDbSchemaVersion],
  			                 [archiveOn],[archiveInterval],[archiveAge],[archivePeriod],[archivePrefix],
			                 [groomLogAge],[groomAlertAge],[groomEventAllow],[groomEventAge],
	                         [archiveTimeZoneName],[server],
                             [reportingVersion],[repositoryVersion],
					         [loginCollapse],[loginTimespan],[loginCacheSize],[auditLogouts],[auditUserLogouts] -- SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
   )
	VALUES (null,0,
                0,1,1,1,1,0,
                0,0,0,0,
                0,1,0,0,0,
                0,
                0,1,1,1,1,
                1,1,0,
                5201,5200,0,
		        502,402,
                1,1,60,3,'SQLcmArchive',
                60,60,1,90,
                '(UTC) Universal Coordinated Time',@Network_Name,
                101,101,
                1,60,250,
				1,1 -- SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
   )


INSERT INTO [SystemDatabases] ([instance],[databaseName],[databaseType],[dateCreated])
	VALUES (@Instance_Name,'SQLcompliance','System',GETUTCDATE())

INSERT INTO [SystemDatabases] ([instance],[databaseName],[databaseType],[dateCreated])
	VALUES (@Instance_Name,'SQLcompliance.Processing','System',GETUTCDATE())


DECLARE @dbid smallint;

SELECT @dbid = (SELECT dbid FROM master..sysdatabases WHERE name='SQLcompliance');
UPDATE SystemDatabases set sqlDatabaseId=@dbid where databaseName='SQLcompliance';

SELECT @dbid = (SELECT dbid FROM master..sysdatabases WHERE name='SQLcompliance.Processing');
UPDATE SystemDatabases set sqlDatabaseId=@dbid where databaseName='SQLcompliance.Processing';

GO
