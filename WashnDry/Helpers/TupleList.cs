using System;
using System.Collections.Generic;



namespace WashnDry
{
	public class TupleList<T1, T2, T3, T4, T5> : List<Tuple<T1, T2, T3, T4, T5>>
	{
		public void Add(T1 item, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			Add(new Tuple<T1, T2, T3, T4, T5>(item, item2, item3, item4, item5));
		}

	}
}
