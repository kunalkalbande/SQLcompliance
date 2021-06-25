using System;

namespace Idera.SQLcompliance.Core

{
   //--------------------------------------------------------------
   // Implements a modified XOR-Shift random number generator that 
   // produces a hash chain.
   // Reference: http://www.jstatsoft.org/v08/i14/xorshift.pdf
   //--------------------------------------------------------------
   internal class HashChain
	{
      #region Private Member Fields

      static readonly uint  FirstInitialValue  = 455376813;
      static readonly uint  SecondInitialValue = 999876527;
      static readonly uint  ThirdInitialValue  = 106245939;

		uint x;
		uint y           = FirstInitialValue;
      uint z           = SecondInitialValue;
      uint w           = ThirdInitialValue;
      uint mask;

      object syncObj = new object();

      #endregion

		#region Constructor

		internal HashChain( string instance )
		{
			mask = (uint)NativeMethods.GetHashCode(instance.ToUpper());
		}

		#endregion

      #region Initialization and Generation
      //---------------------------------------------------------
      // Initialize - initialize a new chain.
      //---------------------------------------------------------
      internal void 
         Initialize( 
            int seed 
         )
		{
         lock( syncObj )
         {
            x = ((uint)seed ) ^ mask;
            y = FirstInitialValue;
            z = SecondInitialValue;
            w = ThirdInitialValue;
         }
		}

      //---------------------------------------------------------
      // GetNext - Get the next hashcode in the chain.
      // Note: This API is for generating a chain without IDs.
      //---------------------------------------------------------
		internal int 
         GetNext()
		{
         lock( syncObj )
         {
            uint temp =( x ^ ( x << 7 ) );

            x = y; 
            y = z; 
            z = w;
            w = ( w ^ ( w >> 23 ) ) ^ ( temp ^ ( temp >> 11 ) );

            return (int)( w );
         }
		}

      //---------------------------------------------------------
      // GetHashCode - Get the next hashcode in the chain.
      // Note: the hash code is generated using the next ID and
      //       current checksum.
      //---------------------------------------------------------
      internal int
         GetHashCode( int nextId, int cs )
      {
         lock( syncObj )
         {
            uint b = ((uint)nextId) ^ mask;
            uint a = (uint)cs;
            uint temp =( b ^ ( b << 7 ) );

            cs = (int)(( a ^ ( a >> 23 ) ) ^ ( temp ^ ( temp >> 11 ) ));

         }
         return cs;

      }

      //---------------------------------------------------------
      // GetNextId - Get the next ID using current hash code and
      //             checksum.  It is used for integrity check.
      //---------------------------------------------------------
     internal int
         GetNextId( int hashcode, int cs )
      {
         uint temp = (uint)hashcode;
         uint c;
         uint b = (uint)cs;
         uint a = temp ^ ( b ^ ( b >> 23 ) );
         c = a >> 21;
         b = c << 10;
         temp = a & 0x001FFC00;
         c <<= 21;
         uint d = b ^ temp;
         c += d;
         temp = a & 0x000003FF;
         d >>= 11;
         c += d ^ temp;

         uint s = c & 0x0000007F;
         a = s << 7;
         b = c & 0x00003F80;
         b = a ^ b;
         s += b;
         b = b << 7;
         a = c & 0x001FC000;
         b = b ^ a;
         s += b;
         b = b << 7;
         a = c & 0x0FE00000;
         b = b ^ a;
         s += b;
         b = b << 7;
         a = c & 0xF0000000;
         b = a ^ b;
         s += b;
         return (int)(s ^ mask);

      }
      #endregion

	}
}
