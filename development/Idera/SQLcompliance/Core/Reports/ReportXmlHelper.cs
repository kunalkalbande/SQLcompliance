using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;


namespace Idera.SQLcompliance.Core.Reports
{
   public class ReportXmlHelper
   {
      #region RDL Constants
      
      // RDL.xml tags
      public const string TagReportFile = "reportfile";
      public const string TagRDLName = "name";
      public const string TagRDLFile = "file";
      public const string TagRDLSproc = "sproc";
      public const string TagRDLDescription = "description";
      
      #endregion

      #region static and public methods

      public static List<CMReport> LoadReportListFromXmlFile( string filename )
      {
         FileInfo info = new FileInfo( filename );
         if ( !info.Exists )
         {
            throw new FileNotFoundException( filename );
         }

         List<CMReport> list = new List<CMReport>();
         string path = info.DirectoryName;
         XmlDocument doc = new XmlDocument();
         doc.Load( filename );
         XmlNodeList xmlList = doc.GetElementsByTagName( TagReportFile );
         string categoryName = "";
         foreach ( XmlNode node in xmlList )
         {
            categoryName = node.ParentNode.Attributes[0].Value;
            // SQLCM 5.7 Requirement 5.3.4.5 If category name is set something else in rdl.xml
            if(categoryName == null && (categoryName != "Alerts /  History" || categoryName != "Configuration"))
            {
                categoryName = "Alerts /  History";
            }
            list.Add( XmlNodeToCMReportInfo( node, path, categoryName ));
         }
         list.Sort();

         return list;
      }

      #endregion

      #region Xml Node/Element to Report Parameter

      static public CMReport XmlNodeToCMReportInfo(XmlNode node, string directory, string category)
      {
         CMReport info = new CMReport();
         if ( !node.HasChildNodes )
            return info;

         foreach ( XmlNode child in node.ChildNodes )
         {
            info.RootHead = category;
            switch ( child.Name )
            {
               case TagRDLName:
                  info.Name = child.InnerText;
                  break;
               case TagRDLFile:
                  info.FileName = Path.Combine(directory, child.InnerText);
                  break;
               case TagRDLSproc:
                  info.StoredProcedure = child.InnerText;
                  break;
               case TagRDLDescription:
                  info.Description = child.InnerText;
                  break;
                }
         }
         return info;
      }

      #endregion

      static public Stream ReplaceDataSetReferences(string filename, Dictionary<string, string> dsrLookup)
      {
         FileInfo info = new FileInfo(filename);
         if (!info.Exists)
         {
            throw new FileNotFoundException(filename);
         }

         XmlDocument doc = new XmlDocument();
         doc.Load(filename);
         XmlNamespaceManager xmlns = new XmlNamespaceManager(doc.NameTable);
         string nsReport = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition";
         string xpath = "/r:Report/r:ReportParameters/r:ReportParameter/r:ValidValues/r:DataSetReference/../..";
         xmlns.AddNamespace("r", nsReport);

         XPathNavigator nav = doc.CreateNavigator();
         XPathNodeIterator iterator = nav.Select(xpath, xmlns);
         while (iterator.MoveNext())
         {
            XPathNavigator currentNode = iterator.Current;
            string name = currentNode.GetAttribute("Name", "");
            if(dsrLookup.ContainsKey(name) &&
               currentNode.MoveToChild("ValidValues", nsReport) &&
               currentNode.MoveToChild("DataSetReference", nsReport))
            {
               currentNode.ReplaceSelf(dsrLookup[name]);
            }
         }
         MemoryStream retVal = new MemoryStream();
         XmlWriter writer = XmlWriter.Create(retVal);
         doc.WriteTo(writer);
         writer.Flush();
         retVal.Seek(0, SeekOrigin.Begin);
         return retVal;
      }

      static public XmlDocument LoadRdlFile(string filename)
      {
         FileInfo info = new FileInfo(filename);
         if (!info.Exists)
         {
            throw new FileNotFoundException(filename);
         }

         XmlDocument doc = new XmlDocument();
         doc.Load(filename);
         return doc;
      }
   }
}

