using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    public  class CategoryResponse
    {
        public CategoryResponse()
        {
            categoryTable = new List<CategoryData>();
        }

        [DataMember]
        public List<CategoryData> categoryTable { get; set; }

        internal void Add(CategoryData categoryTableArg)
        {
            categoryTable.Add(categoryTableArg);
        }
    }
}
