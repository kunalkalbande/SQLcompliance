using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Idera.SQLcompliance.Core.Templates
{
   interface ITemplate
   {
      #region Properties
      
      string Name{ get; set; }
      string Description{ get; set; }
      string FullFilename{ get; set; }
      //List <ITElement> Elements { get; set; }
      
      #endregion
   
      #region Methods
      
      // Apply the template to the specified instance
      //bool Apply( string instance, bool replace );
      
      // Load a template from a file
      T Load<T>( string filename );
      
      // Save the template to a file
      bool Save<T>( string filename, T obj, bool overwrite );
      
      bool Import( string instance );
      
      bool Export( string instance, string filename, bool overwrite );
      
      // Add an template element to the template
      //bool Add( ITElement element );
      //bool Remove( ITElement element );
      
      #endregion
      
      
   }
}
