package com.idera.sqlcm.url.rewrite;

import javax.servlet.ServletContext;

import org.ocpsoft.logging.Logger.Level;
import org.ocpsoft.rewrite.config.Configuration;
import org.ocpsoft.rewrite.config.ConfigurationBuilder;
import org.ocpsoft.rewrite.config.Direction;
import org.ocpsoft.rewrite.config.Log;
import org.ocpsoft.rewrite.servlet.config.Forward;
import org.ocpsoft.rewrite.servlet.config.HttpConfigurationProvider;
import org.ocpsoft.rewrite.servlet.config.Path;

import com.idera.sqlcm.server.web.WebConstants;

public class UrlRewriterConfigProvider extends HttpConfigurationProvider {

	@Override
	public Configuration getConfiguration(ServletContext arg0) {

		ConfigurationBuilder confBuilder = ConfigurationBuilder.begin();

		// Regex filter for index page
		confBuilder
				.addRule()
				.when(Direction.isInbound().and(
						Path.matches("/sqlcm/{instance}/index")))
				.perform(
						Log.message(Level.INFO,
								"Appying inbound rewrite filter from sqlcm to index")
								.and(Forward.to("/~./sqlcm/index.zul")));
		// Regex filter for auditedInstance page
		confBuilder
				.addRule()
				.when(Direction.isInbound().and(
						Path.matches("/sqlcm/{instance}/auditedInstance")))
				.perform(
						Log.message(Level.INFO,
								"Appying inbound rewrite filter from sqlcm to index")
								.and(Forward.to("/~./sqlcm/auditedInstance.zul")));
		// Regex filter for administration page
		confBuilder
				.addRule()
				.when(Direction.isInbound().and(
						Path.matches("/sqlcm/{instance}/administration")))
				.perform(
						Log.message(Level.INFO,
								"Appying inbound rewrite filter from sqlcm to administration")
								.and(Forward.to("/~./sqlcm/administration.zul")));
		// Regex filter for instance details view page
		confBuilder
		.addRule()
		.when(Direction.isInbound().and(
				Path.matches("/sqlcm/{instance}/instanceView/{id}")))
		.perform(
			Log.message(Level.INFO,
				"Appying inbound rewrite filter from SQLCM for instanceView by ID")
				.and(Forward
					.to("/~./sqlcm/instanceView.zul?" +
						WebConstants.PRM_INSTANCE_ID + "={id}&" +
						WebConstants.PRM_INSTANCE_NAME + "={instance}")))
		.where("id").matches("[0-9]+");
		// Regex filter for instance events view page
		confBuilder
			.addRule()
			.when(Direction.isInbound().and(
				Path.matches("/sqlcm/{instance}/instanceEventsView/{id}")))
			.perform(
				Log.message(Level.INFO,
					"Appying inbound rewrite filter from SQLCM for instanceEventsView by instance ID")
					.and(Forward.to("/~./sqlcm/instanceEvent.zul?" +
						WebConstants.PRM_INSTANCE_ID + "={id}&" +
						WebConstants.PRM_INSTANCE_NAME + "={instance}")))
					.where("id").matches("[0-9]+");
		// Regex filter for database events view page
		confBuilder
			.addRule()
			.when(Direction.isInbound().and(
				Path.matches("/sqlcm/{instance}/databaseEventsView/{id}/{dbId}")))
			.perform(
				Log.message(Level.INFO,
					"Appying inbound rewrite filter from SQLCM for databaseEventsView by instance ID and database ID")
					.and(Forward.to("/~./sqlcm/databaseEvent.zul?" +
						WebConstants.PRM_INSTANCE_ID + "={id}&" +
						WebConstants.PRM_DATABASE_ID + "={dbId}&" +
						WebConstants.PRM_INSTANCE_NAME + "={instance}")))
			.where("id").matches("[0-9]+");
		// Regex filter for database details view page
		confBuilder
		.addRule()
		.when(Direction.isInbound().and(
				Path.matches("/sqlcm/{instance}/databaseView/{id}/{dbId}")))
		.perform(
				Log.message(Level.INFO,
						"Appying inbound rewrite filter from SQLCM for databaseView by ID")
						.and(Forward
								.to("/~./sqlcm/databaseView.zul?" +
										WebConstants.PRM_INSTANCE_ID + "={id}&" +
										WebConstants.PRM_DATABASE_ID + "={dbId}&" +
										WebConstants.PRM_INSTANCE_NAME + "={instance}")))
		.where("id").matches("[0-9]+")
		.where("dbId").matches("[0-9]+");
		// Regex filter for all pages urls
		confBuilder
				.addRule()
				.when(Direction.isInbound().and(
						Path.matches("/sqlcm/{instance}/{filename}")))
				.perform(
						Log.message(Level.INFO,
								"Appying inbound rewrite filter from sqlcm to filename")
								.and(Forward.to("/~./sqlcm/{filename}.zul")))
				.where("filename");
		// Regex filter for instances alerts view page
		confBuilder
			.addRule()
			.when(Direction.isInbound().and(
				Path.matches("/sqlcm/{instance}/instancesAlerts")))
			.perform(
				Log.message(Level.INFO,
					"Appying inbound rewrite filter from SQLCM for databaseEventsView by instance ID and database ID")
					.and(Forward.to("/~./sqlcm/instancesAlerts.zul?" +
						WebConstants.PRM_INSTANCE_NAME + "={instance}")));
		return confBuilder;
	}

	@Override
	public int priority() {
		// Equal priority for all
		return 0;
	}

}
