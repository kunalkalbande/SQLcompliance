using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// Summary description for SerializationHelper.
	/// </summary>
	public class SerializationHelper
	{
      private const string assemblySeparators = ",=";

      #region Constructors

		public SerializationHelper()
		{
		}

      #endregion

      #region Serialization

      //--------------------------------------------------------------------------------------
      // Customized serialization - manually serializes the base class of an object
      //--------------------------------------------------------------------------------------
      private static void
         SerializeBaseType( 
            object            obj,
            Type              type,
            SerializationInfo info,
            StreamingContext  context )
      {
         if( typeof(object) == type )
         {
            // base type is object, do nothing
            return;
         }

         BindingFlags flags = BindingFlags.Instance |
                              BindingFlags.DeclaredOnly |   // types defined at this level
                              BindingFlags.NonPublic    |
                              BindingFlags.Public;

         FieldInfo[] fields = type.GetFields( flags );  // get all qualified fields

         string fieldName;
         foreach( FieldInfo field in fields )
         {
            if( field.IsNotSerialized ) // skip NonSerialized fields
               continue;

            // construct unique field names by prefixing the field name with the type name so
            // that same field names in different levels have different names
            fieldName = field.FieldType + "+" + field.Name;
            info.AddValue( fieldName, field.GetValue( obj ), field.FieldType );
         }

         // Recurisve call to serialize the base type
         SerializeBaseType( obj, type.BaseType, info, context );
      }

      //--------------------------------------------------------------------------------------
      //  Customized serialization - manually serializes an object to its base class
      //--------------------------------------------------------------------------------------
      public static void
         SerializeType(
            object               obj,
            SerializationInfo   info,
            StreamingContext    context )
      {
         SerializeType( obj, true, info, context );
      }

      //--------------------------------------------------------------------------------------
      //  Customized serialization - manually serializes an object
      //--------------------------------------------------------------------------------------
      public static void
         SerializeType(
            object               obj,
            bool                 includeBaseType,
            SerializationInfo    info,
            StreamingContext     context )
      {
         Type type = obj.GetType();

         if( typeof(object) == type )
         {
            // base type is object, do nothing
            return;
         }

         BindingFlags flags = BindingFlags.Instance |
                              BindingFlags.DeclaredOnly |   // types defined at this level
                              BindingFlags.NonPublic    |
                              BindingFlags.Public;

         FieldInfo[] fields = type.GetFields( flags );  // get all qualified fields

         string fieldName;
         foreach( FieldInfo field in fields )
         {
            if( field.IsNotSerialized ) // skip NonSerialized fields
               continue;

            // construct unique field names by prefixing the field name with the type name so
            // that same field names in different levels have different names
            fieldName = field.Name;
            info.AddValue( fieldName, field.GetValue( obj ), field.FieldType );
         }

         if( includeBaseType )
         {
            // Recurisve call to serialize the base type
            SerializeBaseType( obj, type.BaseType, info, context );
         }
      }

      #endregion

      #region Deserialization

      //--------------------------------------------------------------------------------------
      //  Deserialize a type and its base type
      //--------------------------------------------------------------------------------------
      private static void
         DeserializeBaseType( 
            object            obj,
            Type              type,
            SerializationInfo info,
            StreamingContext  context)
      {
         if( typeof(object) == type )
         {
            // End of recursion.  Do nothing.
            return;
         }

         // Set flags for desired field types
         BindingFlags flags = BindingFlags.Instance |
                              BindingFlags.DeclaredOnly  |
                              BindingFlags.NonPublic     |
                              BindingFlags.Public;
         FieldInfo[]  fields = type.GetFields( flags );

         string fieldName;
         object value;
         foreach( FieldInfo field in fields )
         {
            if( field.IsNotSerialized )
               continue;

            // construct the unique field name
            fieldName = field.FieldType + "+" + field.Name;
            try
            {
               value = info.GetValue( fieldName, field.FieldType );  // retrieve field value
               field.SetValue( obj, value );
            }
            catch( Exception e)
            {
               Debug.WriteLine( "Error deserializing " + type.ToString() + "Exception: " + e.Message );
            }
         }
         DeserializeBaseType( obj, type.BaseType, info, context );
      }

      public static void
         DeserializeType(
            object               obj,
            SerializationInfo    info,
            StreamingContext     context )
      {
         DeserializeType( obj, true, info, context );
      }

      //--------------------------------------------------------------------------------------
      //  Customized Deserialization - manually deserializes the base class of an object
      //--------------------------------------------------------------------------------------
      public static void
         DeserializeType(
            object                obj,
            bool                  includeBase,
            SerializationInfo     info,
            StreamingContext      context )
      {
         Type type = obj.GetType();

         if( typeof(object) == type )
         {
            // End of recursion.  Do nothing.
            return;
         }

         // Set flags for desired field types
         BindingFlags flags = BindingFlags.Instance |
                              BindingFlags.DeclaredOnly  |
                              BindingFlags.NonPublic     |
                              BindingFlags.Public;

         FieldInfo[]  fields = type.GetFields( flags );

         string fieldName;
         object value;

         foreach( FieldInfo field in fields )
         {
            if( field.IsNotSerialized )
               continue;

            // construct the unique field name
            fieldName = field.FieldType + "+" + field.Name;
            try
            {
               value = info.GetValue( fieldName, field.FieldType );  // retrieve field value
               field.SetValue( obj, value );
            }
            catch( Exception e)
            {
               Debug.WriteLine( "Error deserializing " + type.ToString() + "Exception: " + e.Message );
            }
         }

         if( includeBase )
            DeserializeBaseType( obj, type.BaseType, info, context );
      }

      //--------------------------------------------------------------------------------------------
      //  Customized Deserialization - manually deserializes an object with its base class fields
      //--------------------------------------------------------------------------------------------
      public static void DeserializeObject( object obj, SerializationInfo info, StreamingContext context )
      {
         Type type = obj.GetType();
         MemberInfo[] memberInfoList = FormatterServices.GetSerializableMembers( type, context);

         foreach(MemberInfo memberInfo in memberInfoList)
         {
            if(memberInfo is FieldInfo)
            {
               FieldInfo fieldInfo = (FieldInfo)memberInfo ;
               if(fieldInfo.DeclaringType != null &&
                  fieldInfo.DeclaringType == type )
               {
                  try
                  {
                     // We only want to serialize the members we are directly responsible
                     //  for declaring to avoid permissions problems
                     object tempValue = info.GetValue(fieldInfo.Name, fieldInfo.FieldType) ;
                     fieldInfo.SetValue(obj, tempValue);
                  }
                  catch( Exception e )
                  {
                     Debug.WriteLine( "Exception when deserializing " + type.ToString() + " : " + e.Message );
                     throw e;
                  }
               }
            }
         }
         Debug.WriteLine( String.Format("{0} deserializaed", type ));
      }


      #endregion

      #region Helper utilities

      //--------------------------------------------------------------------------------------
      // GetTypeVerion
      //--------------------------------------------------------------------------------------
      public static Version
         GetTypeVersion(
         SerializationInfo info
         )
      {
         // Assembly name format <name>, <assembly version number>, ...
         string[] parts = info.AssemblyName.Split( assemblySeparators.ToCharArray());

         // Assembly version number format "Version=<version number>"
         return new Version( parts[2]);
      }

      public static void PrintTypeInfo( Type type )
      {
         if( type.IsArray )
            Debug.WriteLine( String.Format( "{0} is an array type", type ));
         else if( type.IsByRef )
            Debug.WriteLine( String.Format( "{0} is byRef ", type ));
         else if( type.IsSpecialName )
            Debug.WriteLine( String.Format( "{0} is a special name and need special handling ", type ));
         else if( !type.IsPrimitive && type.IsValueType )
            Debug.WriteLine( String.Format( "{0} is a non-primitive value type ", type ));
      }

      public static void ThrowSerializationException(
         Exception e,
         Type      type )
      {
         string msg = String.Format( CoreConstants.Exception_SerializationError, type, e.Message );
         Debug.Write( msg );
         throw new SerializationException( msg, e );
      }

      public static void ThrowDeserializationException(
         Exception e,
         Type      type )
      {
         string msg = String.Format( CoreConstants.Exception_DeserializationError, type, e.Message );
         Debug.Write( msg );
         throw new SerializationException( msg, e );
      }

      #endregion

	}
}
