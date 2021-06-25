using System;
using System.Collections ;
using System.Drawing ;
using System.Text ;
using System.Windows.Forms ;

namespace Idera.SQLcompliance.Application.GUI
{
	public class LinkString
	{
		private ArrayList _segments ;
      private int _maxLinkLength ;

      public LinkString() : this(-1){}

      public LinkString(int maxLinkLength)
      {
         _maxLinkLength = maxLinkLength ;
         _segments = new ArrayList() ;
      }

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("") ;
			foreach(LinkSegment seg in _segments)
				builder.Append(seg.Value) ;
			return builder.ToString() ;
		}

		public string Value
		{
			get { return ToString() ; }
		}

		public IList Segments
		{
			get { return (IList)_segments.Clone(); }
		}

		public IList Links
		{
			get 
			{
				ArrayList retVal = new ArrayList() ;
				foreach(LinkSegment seg in _segments)
					if(seg.IsLink)
						retVal.Add(seg) ;
				return retVal ;
			}
		}

		public void Append(string s)
		{
			_segments.Add(new LinkSegment(Value.Length, s, null)) ;
		}

		public string AppendLink(string s, object tag)
		{
         string realString = s ;
         if(_maxLinkLength > 4 && s.Length > _maxLinkLength)
         {
            realString = s.Substring(0, _maxLinkLength - 3) + "..." ;
         }
			_segments.Add(new LinkSegment(Value.Length, realString, tag)) ;
         return realString ;
		}

		public void Append(LinkSegment s)
		{
			_segments.Add(new LinkSegment(Value.Length, s.Value, s.Tag)) ;
		}

      //private Rectangle priorRect = new Rectangle(0, 0, 0, 0) ;
      public LinkSegment LinkHitTest(int x, int y, RichTextBox parent, Graphics g)
		{
         //Pen pen = new Pen(Color.Red, 1) ;
         //Pen whitePen = new Pen(Color.White, 1) ;
			int index = parent.GetCharIndexFromPosition(new Point(x, y)) ;
			if(index >= parent.Text.Length)
				return null ;
         
         // GetIndexFromPosition will return the closest origin position to the cursor.
         //  We are actually interested in the closest center.  Therefore, we use the 
         //  supplied index as a proximity hint.  After this, we use the x position of the
         //  cursor to determine which character we are actually hovering over and
         //  modify the index appropriately.  We use pointNext to properly
         //  contain our character rectangle.  MesaureString did not seem to work.
			Point pointChar = parent.GetPositionFromCharIndex(index) ;
         Point pointPrior = pointChar ;
         Point pointNext = pointChar ;
         if(index > 0)
            pointPrior = parent.GetPositionFromCharIndex(index - 1) ;
         if(index < parent.Text.Length - 1)
            pointNext = parent.GetPositionFromCharIndex(index + 1) ;

         if(pointPrior.X < pointChar.X && x < pointChar.X)
         {
            index-- ;
            pointNext = pointChar;
            pointChar = pointPrior ;
         }
         if(pointNext.X > pointChar.X && x > pointNext.X)
         {
            index++ ;
            pointChar = pointNext ;
            pointNext = parent.GetPositionFromCharIndex(index + 1) ;
         }

			foreach(LinkSegment boundary in _segments)
			{
				if(boundary.Contains(index))
				{
					Rectangle rect = new Rectangle(pointChar.X, pointChar.Y, pointNext.X - pointChar.X, parent.Font.Height) ;
               //g.DrawRectangle(whitePen, priorRect) ;
               //g.DrawRectangle(pen, rect) ;
               //priorRect = rect ;
					if(rect.Contains(x, y))
						return boundary ;
					break ;
				}
				if(boundary.Index > index)
					break; 
			}
			return null ;
		}
	}

	public class LinkSegment
	{
		private int _index = -1 ;
		private string _strValue ;
		private object _tag ;

		public LinkSegment(int index, string s, object tag)
		{
			_tag = tag ; 
			_index = index ;
			_strValue = s ;

		}

		public LinkSegment(int index, string s) :this(index, s, null)
		{
		}

		public string Value
		{
			get { return _strValue ; }
		}

		// Inclusive
		public int Index
		{
			get { return _index;  }
			set { _index = value ; }
		}

		public int Length
		{
			get { return (_strValue == null) ? 0 : _strValue.Length ; }
		}

		public object Tag
		{
			get { return _tag ; }
			set { _tag = value ; }
		}

		public bool IsLink
		{
			get { return (_tag != null) ; }
		}

		public bool Contains(int i)
		{
			return (_index <= i && (_index + Length) > i) ;
		}
	}
}
