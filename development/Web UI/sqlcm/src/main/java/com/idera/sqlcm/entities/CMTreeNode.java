/**
 * 
 */
package com.idera.sqlcm.entities;

import java.util.List;

import org.apache.log4j.Logger;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.facade.CMTreeFacade;

/**
 * @author Rajesh
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class CMTreeNode extends CMEntity {

	private static final Logger logger = Logger.getLogger(CMTreeNode.class);

	private int systemId;
	private int parentId;
	private NodeType type;
	private String description;
	@JsonProperty("isEnabled")
	private boolean enabled;

	// parent tree node
	private CMTreeNode parent;
	// child list
	private List<CMTreeNode> children;
	// whether this node is open
	private boolean open;

	public CMTreeNode() {
		super();
	}

	public CMTreeNode getNode() {
		return this;
	}

	public List<CMTreeNode> getChildren() {
		if (children == null && type.ordinal() < NodeType.Database.ordinal()) {
			try {
				children = CMTreeFacade.getTreeNodes(id, type);
				logger.info("Childrens: " + children);
			} catch (RestException e) {
				logger.error(e);
			}
		}
		return children;
	}

	public int getChildCount() {
		int count = 0;
		if (getChildren() != null) {
			count = this.children.size();
		}
		return count;
	}

	public int getSystemId() {
		return systemId;
	}

	public void setSystemId(int systemId) {
		this.systemId = systemId;
	}

	public int getParentId() {
		return parentId;
	}

	public void setParentId(int parentId) {
		this.parentId = parentId;
	}

	public NodeType getType() {
		return type;
	}

	public void setType(NodeType type) {
		this.type = type;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}

	public CMTreeNode getParent() {
		return parent;
	}

	public void setParent(CMTreeNode parent) {
		this.parent = parent;
	}

	public void setChildren(List<CMTreeNode> children) {
		this.children = children;
	}

	public boolean isOpen() {
		return open;
	}

	public void setOpen(boolean open) {
		this.open = open;
	}

	@Override
	public String toString() {
		StringBuilder builder = new StringBuilder();
		builder.append("\nCMTreeNode [systemId=");
		builder.append(systemId);
		builder.append(", type=");
		builder.append(type);
		builder.append(", description=");
		builder.append(description);
		builder.append(", enabled=");
		builder.append(enabled);
		builder.append(", parentId=");
		builder.append(parentId);
		builder.append(", parent=");
		builder.append(parent);
		builder.append(", children=");
		builder.append(children);
		builder.append(", open=");
		builder.append(open);
		builder.append(", id=");
		builder.append(id);
		builder.append(", name=");
		builder.append(name);
		builder.append("]");
		return builder.toString();
	}

}
