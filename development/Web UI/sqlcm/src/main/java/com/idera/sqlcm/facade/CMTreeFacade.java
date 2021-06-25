/**
 * 
 */
package com.idera.sqlcm.facade;

import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMEntity.NodeType;
import com.idera.sqlcm.entities.CMTreeNode;
import com.idera.sqlcm.rest.SQLCMRestClient;

import mazz.i18n.Logger;
import mazz.i18n.LoggerFactory;


/**
 * @author Rajesh
 */
public class CMTreeFacade {

	private static final Logger logger = LoggerFactory.getLogger(CMTreeFacade.class);

	public static List<CMTreeNode> getTreeNodes(long id, NodeType type)
			throws RestException {
		return new SQLCMRestClient().getTreeNodes(id, type);
	}
	
	//4.1.1.4_start
	public static List<CMDatabase> getTreeNodes(long id)
			throws RestException {
		List<CMDatabase> returnList = new SQLCMRestClient().getAuditedDatabasesForInstance(id+"");
			//	getTreeNodes(id);
		return  returnList;
	}
	
	//4.1.1.4_end

}
