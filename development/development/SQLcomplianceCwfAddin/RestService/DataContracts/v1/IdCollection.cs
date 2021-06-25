using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [CollectionDataContract]
	public class IdCollection : List<int> 
	{
	    public IdCollection() : base() { }
	    public IdCollection(IEnumerable<int> i) : base(i) { }
	}
}
