using System;
using System.Windows.Forms;
using Idera.SQLcompliance.Core.Agent;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    public class ServerTreeNodeHelper
    {
        public static ServerRecord GetServerRecordFromServerTreeNode(TreeNode node)
        {
            var server = node.Tag as ServerRecord;
            return server;
        }

        public static string GetInstanceNameFromServerTreeNode(TreeNode node)
        {
            var server = GetServerRecordFromServerTreeNode(node);
            return server.Instance;
        }

        public static SQLcomplianceTreeNode FindServerNode(string instanceName, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                var instance = GetInstanceNameFromServerTreeNode(node);
                if (instanceName.Equals(instance, StringComparison.InvariantCultureIgnoreCase))
                    return node as SQLcomplianceTreeNode;
            }
            return null;
        }
    }
}
